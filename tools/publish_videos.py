#!/usr/bin/python
import glob
import json
import os
import readline
import shutil
import subprocess
import sys
import time
from dataclasses import dataclass
from itertools import repeat
from multiprocessing import Pool
from pathlib import PurePath

# https://stackoverflow.com/questions/287871/how-do-i-print-colored-text-to-the-terminal
class Colors:
    HEADER = '\033[95m'
    OKBLUE = '\033[94m'
    OKCYAN = '\033[96m'
    OKGREEN = '\033[92m'
    WARNING = '\033[93m'
    FAIL = '\033[91m'
    ENDC = '\033[0m'
    BOLD = '\033[1m'
    UNDERLINE = '\033[4m'

@dataclass
class Dimension:
    width: int
    height: int

@dataclass
class SizeSpec:
    name: str
    subdir: str
    isOrig: bool
    minVideoDimension: int = None
    resizeGeometry: str = None
    cropGeometry: str = None

@dataclass
class EnvContext:
    pod: str
    pgEnvFile: str
    systemdService: str

class CategorySpec:
    def __init__(self, videoDir: str, name: str, year: int, allowedRoles: list, assetRoot: str):
        self.rootDir = videoDir
        self.name = name
        self.year = year
        self.allowedRoles = allowedRoles
        self.sqlFile = os.path.join(videoDir, "videocategory.sql")
        self.sizeSpecs = build_size_specs(videoDir)
        self.rawDir = next(filter(lambda spec: spec.isOrig, self.sizeSpecs)).subdir
        self.deployYearRoot = os.path.join(assetRoot, str(year))
        self.deployCategoryRoot = os.path.join(assetRoot, str(year), os.path.basename(videoDir))
        self.deployCategoryRawDir = os.path.join(assetRoot, str(year), os.path.basename(videoDir), "raw")
        self.awsBackupRoot = f"s3://mikeandwan-us-videos/{year}/{PurePath(videoDir).name}"

class Context:
    sshRemoteHost = "tifa"
    sshUsername = "svc_www_maw"
    dirAssetRoot = "/srv/www/website_assets/movies"
    postgresImage = "docker.io/postgres:16-alpine"
    awsProfile = "mawpower"
    dev = EnvContext(
        "dev-maw-pod",
        "/home/mmorano/maw_dev/podman-env/maw-postgres.env",
        "pod-dev-maw-pod"
    )
    prod = EnvContext(
        "prod-maw-pod",
        "/home/svc_www_maw/maw_prod/podman-env/maw-postgres.env",
        "pod-prod-maw-pod"
    )

    def __init__(self, videoDir, name, year, allowedRoles):
        self.categorySpec = CategorySpec(videoDir, name, year, allowedRoles, Context.dirAssetRoot)

exifTags = [
    "FileSize",
    "ImageWidth",
    "ImageHeight",
    "Rotation",
    "CreateDate",
    "GPSLatitude",
    "GPSLatitudeRef",
    "GPSLongitude",
    "GPSLongitudeRef"
]

def build_size_specs(dir):
    return [
        SizeSpec("Raw",             os.path.join(dir, "raw"),        True),
        SizeSpec("Full",            os.path.join(dir, "full"),       False, 480),
        SizeSpec("Scaled",          os.path.join(dir, "scaled"),     False, 240),
        SizeSpec("Thumbnail",       os.path.join(dir, "thumbnails"), False, None, "160x120"),
        SizeSpec("Thumbnail Fixed", os.path.join(dir, "thumb_sq"),   False, None, "160x120", "160x120+0+0")
    ]

def verify_destination_does_not_exist(ctx: Context):
    if os.path.isdir(ctx.categorySpec.deployCategoryRoot):
        print(f"{Colors.WARNING}Destination is already taken, please ensure unique category names!{Colors.ENDC}")
        sys.exit()

def prompt_directory(prompt: str):
    val = ""

    readline.set_completer_delims(' \t\n=')
    readline.parse_and_bind("tab: complete")

    while not os.path.isdir(val):
        val = input(prompt)

    return os.path.normpath(val)

def prompt_string_required(prompt: str):
    val = None

    while True:
        val = input(prompt)

        if val:
            return val

def prompt_int(prompt: str):
    val = None

    while True:
        val = input(prompt)

        if not val.isnumeric():
            continue

        if not float(val).is_integer():
            continue

        return int(val)

def prompt_string_list(prompt: str, default: list):
    val = input(prompt)

    if val:
        return val.split(" ")
    else:
        return default

def ensure_aws_is_logged_in(ctx: Context):
    result = subprocess.run([
        "aws",
        "sts",
        "get-caller-identity",
        "--profile", ctx.awsProfile
    ])

    while result.returncode != 0:
        print(f"{Colors.WARNING}You will be prompted to authenticate with AWS to backup files to S3 Glacier{Colors.ENDC}")

        result = subprocess.run([
            "aws",
            "sso",
            "login",
            "--profile", ctx.awsProfile
        ])

def start_dev_pod(ctx: Context):
    subprocess.run([
        "systemctl",
        "--user",
        "start",
        ctx.dev.systemdService
    ])

    # allow services to start
    time.sleep(3)

def clean_prior_attempts(ctx: Context):
    if(not os.path.isdir(ctx.categorySpec.rawDir)):
        return

    for f in glob.glob(os.path.join(ctx.categorySpec.rawDir, "*")):
        shutil.move(f, ctx.categorySpec.rootDir)

    for size in ctx.categorySpec.sizeSpecs:
        if(os.path.isdir(size.subdir)):
            shutil.rmtree(size.subdir)

    if os.path.isfile(ctx.categorySpec.sqlFile):
        os.remove(ctx.categorySpec.sqlFile)

def prepare_size_dirs(ctx: Context):
    for size in ctx.categorySpec.sizeSpecs:
        if(not os.path.isdir(size.subdir)):
            os.mkdir(size.subdir)

def print_stats(photoCount: int, resizeDuration: float, deployDuration: float, backupDuration: float):
    totalTime = resizeDuration + deployDuration + backupDuration

    print(f"{Colors.HEADER}Processed Files: {photoCount}{Colors.ENDC}")

    print(f"{Colors.OKGREEN} - Total Resize Time: {round(resizeDuration, 1)}s{Colors.ENDC}")
    print(f"{Colors.OKGREEN} - Average Resize Time per Photo: {round(resizeDuration / photoCount, 1)}s{Colors.ENDC}")

    print(f"{Colors.OKBLUE} - Total Backup Time: {round(backupDuration, 1)}s{Colors.ENDC}")
    print(f"{Colors.OKBLUE} - Average Backup Time per Photo: {round(backupDuration / photoCount, 1)}s{Colors.ENDC}")

    print(f"{Colors.OKCYAN} - Total Deploy Time: {round(deployDuration, 1)}s{Colors.ENDC}")
    print(f"{Colors.OKCYAN} - Average Deploy Time per Photo: {round(deployDuration / photoCount, 1)}s{Colors.ENDC}")

    print(f"{Colors.WARNING} - Total Time: {round(totalTime, 1)}s{Colors.ENDC}")
    print(f"{Colors.WARNING} - Average Time per Photo: {round(totalTime / photoCount, 1)}s{Colors.ENDC}")

def move_to_local_archive(ctx: Context):
    if(not os.path.isdir(ctx.categorySpec.deployYearRoot)):
        os.mkdir(ctx.categorySpec.deployYearRoot)

    shutil.move(ctx.categorySpec.rootDir, ctx.categorySpec.deployCategoryRoot)

def apply_sql_to_local(ctx: Context):
    subprocess.run([
        "podman", "run", "-it", "--rm",
        "--pull", "newer",
        "--pod", ctx.dev.pod,
        "--env-file", ctx.dev.pgEnvFile,
        "--volume", f"{ctx.categorySpec.deployCategoryRoot}:/output:ro",
        "--security-opt", "label=disable",
        ctx.postgresImage,
            "psql",
                "-h", "localhost",
                "-U", "postgres",
                "-d", "maw_website",
                "-f", f"/output/{os.path.basename(ctx.categorySpec.sqlFile)}"
    ])

def copy_to_remote(ctx: Context):
    subprocess.run([
        "rsync",
        "-ah",
        "--exclude", "*/raw*",
        "--exclude", "*.dng",
        ctx.categorySpec.deployCategoryRoot,
        f"{ctx.sshUsername}@{ctx.sshRemoteHost}:~/"
    ])

def read_exif(ctx: Context):
    etArgs = [
        "exiftool"
    ]

    for t in exifTags:
        etArgs.append(f"-{t}")

    etArgs += [
        "-recurse",
        "-json",
        "-tab",
        "-long",
        ctx.categorySpec.rootDir
    ]

    result = subprocess.run(etArgs, capture_output=True, text=True)

    return json.loads(result.stdout)

def get_exif_num_or_val(exif, field: str):
    num = exif.get(field, {}).get("num", None)

    # 0 is treated as false, but this is a legit number here, so check for it explicitly
    if num == 0 or num:
        return num

    return exif.get(field, {}).get("val", None)

def calc_scaled_dimension(targetMinDimension: int, actualMinDimension: int, actualMaxDimension: int):
    targetMaxDimension = int(targetMinDimension * (actualMaxDimension / actualMinDimension))

    # dimensions must be a multiple of 2 for h264
    if targetMaxDimension % 2 != 0:
        targetMaxDimension += 1

    return targetMaxDimension

def get_scaled_video_dimensions(minDimension: int, exif):
    width = int(get_exif_num_or_val(exif, "ImageWidth"))
    height = int(get_exif_num_or_val(exif, "ImageHeight"))

    if height <= width:
        scaledHeight = minDimension
        scaledWidth = calc_scaled_dimension(minDimension, height, width)
    else:
        scaledWidth = minDimension
        scaledHeight = calc_scaled_dimension(minDimension, width, height)

    return (scaledWidth, scaledHeight)

def transcode_video(file: str, spec: SizeSpec, videoExif):
    (w,h) = get_scaled_video_dimensions(spec.minVideoDimension, videoExif)
    dest = f"{PurePath(file).stem}.mp4"

    res = subprocess.run([
        "ffmpeg",
        "-i", file,
        "-vf", f"scale={w}:{h}",
        "-c:v", "libx264",
        "-crf", str(22),
        "-movflags", "+faststart",
        "-c:a", "copy",
        os.path.join(spec.subdir, dest)
    ], stdout = subprocess.DEVNULL, stderr = subprocess.DEVNULL)

    if(res.returncode != 0):
        print("** ERROR - transcode_video **")

def scale_thumbnail(tifPath: str, jpgPath: str, spec: SizeSpec):
    magickArgs = [
        "magick",
        tifPath
    ]

    if spec.resizeGeometry:
        if spec.cropGeometry:
            magickArgs += [
                "-resize", f"{spec.resizeGeometry}^",
                "-gravity", "center",
                "-crop", spec.cropGeometry
            ]
        else:
            magickArgs += [ "-resize", spec.resizeGeometry ]

    magickArgs += [
        "-strip",
        "-quality", "88",
        jpgPath
    ]

    res = subprocess.run(magickArgs, stdout = subprocess.DEVNULL, stderr = subprocess.DEVNULL)

    if(res.returncode != 0):
        print("** ERROR - scale_thumbnail **")

def gen_thumbnail(file: str, spec: SizeSpec):
    origjpg = os.path.join(spec.subdir, f"{PurePath(file).stem}_tmp.jpg")
    jpg = os.path.join(spec.subdir, f"{PurePath(file).stem}.jpg")

    res = subprocess.run([
        "ffmpeg",
        "-i", file,
        "-ss", "00:00:02",
        "-qscale:v", str(2),
        "-frames:v", str(1),
        origjpg
    ], stdout = subprocess.DEVNULL, stderr = subprocess.DEVNULL)

    if(res.returncode != 0):
        print("** ERROR - gen_thumbnail **")

    # now use imagemagick and its scaling calc to generate the thumbs
    scale_thumbnail(origjpg, jpg, spec)
    os.remove(origjpg)

def resize_video(srcFile: str, ctx: Context, exif):
    print(f"{Colors.OKBLUE}  - {os.path.basename(srcFile)}{Colors.ENDC}")

    for sizeSpec in ctx.categorySpec.sizeSpecs:
        if sizeSpec.isOrig:
            continue

        info = next(filter(lambda x: x["SourceFile"] == srcFile, exif))

        if sizeSpec.minVideoDimension:
            transcode_video(srcFile, sizeSpec, info)

        if sizeSpec.resizeGeometry:
            gen_thumbnail(srcFile, sizeSpec)

def resize_videos(ctx: Context, exif):
    files = list(filter(
        lambda x: os.path.isfile(x),
        glob.glob(os.path.join(ctx.categorySpec.rootDir, "*"))
    ))

    # my 16core/32thread cpu reports 32. observed optimal time was using ~12, where 8-16 were roughly
    # same amount of time, so we div by 2 and sub two to try and leave one core avail
    poolSize = max(2, int(len(os.sched_getaffinity(0)) / 2) - 2)

    with Pool(poolSize) as pool:
        pool.starmap(resize_video, zip(files, repeat(ctx), repeat(exif)))

def fixup_rotation(exif):
    for e in exif:
        rot = get_exif_num_or_val(e, "Rotation")

        if rot == 90 or rot == 270:
            h = get_exif_num_or_val(e, "ImageHeight")
            w = get_exif_num_or_val(e, "ImageWidth")

            e["ImageHeight"]["val"] = w
            e["ImageWidth"]["val"] = h

def read_metadata(ctx: Context, orientationCorrectedSrcExif):
    metadata = {}
    exif = read_exif(ctx)

    for e in orientationCorrectedSrcExif:
        file = PurePath(e["SourceFile"]).stem

        if not file in metadata.keys():
            metadata[file] = {}

        metadata[file]["exif"] = e

    for e in exif:
        file = PurePath(e["SourceFile"]).stem
        size = PurePath(e["SourceFile"]).parent.name

        metadata[file][size] = e

    return metadata

def move_source_files(ctx: Context):
    files = list(filter(
        lambda x: os.path.isfile(x),
        glob.glob(os.path.join(ctx.categorySpec.rootDir, "*"))
    ))

    for f in files:
        shutil.move(f, ctx.categorySpec.rawDir)

def process_videos(ctx: Context):
    start = time.time()

    clean_prior_attempts(ctx)
    prepare_size_dirs(ctx)
    exif = read_exif(ctx)
    fixup_rotation(exif)
    resize_videos(ctx, exif)
    metadata = read_metadata(ctx, exif)
    move_source_files(ctx)
    # write_sql(ctx, metadata)

    end = time.time()
    return end - start

def deploy(ctx: Context):
    start = time.time()

    # local deploy
    start_dev_pod(ctx)
    move_to_local_archive(ctx)
    apply_sql_to_local(ctx)

    # remote deploy
    copy_to_remote(ctx)
    # execute_remote_deploy(ctx)

    end = time.time()
    return end - start

def backup(ctx: Context):
    start = time.time()

    ensure_aws_is_logged_in(ctx)
    subprocess.run([
        "aws",
        "s3",
        "sync",
        ctx.categorySpec.deployCategoryRawDir,
        ctx.categorySpec.awsBackupRoot,
        "--storage-class", "DEEP_ARCHIVE",
        "--profile", ctx.awsProfile
    ])

    end = time.time()
    return end - start

def build_context():
    # dir = prompt_directory("Please enter the path to the videos: ")
    # name = prompt_string_required("Please enter the name of the category: ")
    # year = prompt_int("Please enter the category year: ")
    # roles = prompt_string_list("Please enter role names that should have access to category (default: 'admin friend'): ", ["admin", "friend"])

    dir = "/home/mmorano/Desktop/testing"
    name = "Test"
    year = 2024
    roles = ["admin", "friend"]

    return Context(dir, name, year, roles)

def main():
    ctx = build_context()
    verify_destination_does_not_exist(ctx)

    print(f"{Colors.HEADER}Processing Videos...{Colors.ENDC}")
    resizeDuration = process_videos(ctx)

    doContinue = prompt_string_required("Would you like to backup and deploy at this time? [y|N]: ")

    if(doContinue != "y"):
        sys.exit()

    # note: we no longer get aws hashtree ids from storing in s3 glacier deep archive
    #       so we might as well push sooner than later so we can verify the images on
    #       the site while the backup runs
    # print(f"{Colors.HEADER}Deploying Videos...{Colors.ENDC}")
    # deployDuration = deploy(ctx)

    # print(f"{Colors.HEADER}Backing Up Videos...{Colors.ENDC}")
    # backupDuration = backup(ctx)

    videos = len(glob.glob(os.path.join(ctx.categorySpec.deployCategoryRoot, "xs", "*")))
    print_stats(videos, resizeDuration, deployDuration, backupDuration)

    print(f"{Colors.HEADER}Completed!{Colors.ENDC}")

if __name__=="__main__":
    main()
