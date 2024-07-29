#!/usr/bin/python
import glob
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
class SizeSpec:
    name: str
    subdir: str
    isOrig: bool
    resizeGeometry: str = None
    cropGeometry: str = None

@dataclass
class EnvContext:
    pod: str
    pgEnvFile: str
    systemdService: str

class CategorySpec:
    def __init__(self, photoDir: str, name: str, year: int, allowedRoles: list, assetRoot: str):
        self.rootDir = photoDir
        self.name = name
        self.year = year
        self.allowedRoles = allowedRoles
        self.sizeSpecs = build_size_specs(photoDir)
        self.srcDir = next(filter(lambda spec: spec.isOrig, self.sizeSpecs)).subdir
        self.deployYearRoot = os.path.join(assetRoot, str(year))
        self.deployCategoryRoot = os.path.join(assetRoot, str(year), os.path.basename(photoDir))

class Context:
    sshRemoteHost = "tifa"
    sshUsername = "svc_www_maw"
    dirAssetRoot = "/srv/www/website_assets/images"
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

    def __init__(self, photoDir, name, year, allowedRoles):
        self.categorySpec = CategorySpec(photoDir, name, year, allowedRoles, Context.dirAssetRoot)

exifTags = [
    # exif
    "BitsPerSample",
    "Compression",
    "Contrast",
    "CreateDate",
    "DigitalZoomRatio",
    "ExposureCompensation",
    "ExposureMode",
    "ExposureProgram",
    "ExposureTime",
    "FNumber",
    "Flash",
    "FocalLength",
    "FocalLengthIn35mmFormat",
    "GainControl",
    "GPSAltitude",
    "GPSAltitudeRef",
    "GPSDateStamp",
    "GPSImgDirection",
    "GPSImgDirectionRef",
    "GPSLatitude",
    "GPSLatitudeRef",
    "GPSLongitude",
    "GPSLongitudeRef",
    "GPSMeasureMode",
    "GPSSatellites",
    "GPSStatus",
    "GPSVersionID",
    "ISO",
    "LightSource",
    "Make",
    "MeteringMode",
    "Model",
    "Orientation",
    "Saturation",
    "SceneCaptureType",
    "SceneType",
    "SensingMethod",
    "Sharpness",

    # nikon
    "AFAreaMode",
    "AFPoint",
    "ActiveD-Lighting",
    "ColorSpace",
    "ExposureDifference",
    "FlashColorFilter",
    "FlashCompensation",
    "FlashControlMode",
    "FlashExposureComp",
    "FlashFocalLength",
    "FlashMode",
    "FlashSetting",
    "FlashType",
    "FocusDistance",
    "FocusMode",
    "FocusPosition",
    "HighIsoNoiseReduction",
    "HueAdjustment",
    "NoiseReduction",
    "PictureControlName",
    "PrimaryAFPoint",
    "VRMode",
    "VibrationReduction",
    "VignetteControl",
    "WhiteBalance",

    # composite
    "Aperture",
    "AutoFocus",
    "DOF",
    "FOV",
    "HyperfocalDistance",
    "LensID",
    "LightValue",
    "ScaleFactor35efl",
    "ShutterSpeed"
]

def build_size_specs(dir):
    return [
        SizeSpec("Source",          os.path.join(dir, "src"),   True),
        SizeSpec("Thumbnail",       os.path.join(dir, "xs"),    False, "160x120"),
        SizeSpec("Thumbnail Fixed", os.path.join(dir, "xs_sq"), False, "160x120", "160x120+0+0"),
        SizeSpec("Small",           os.path.join(dir, "sm"),    False, "640x480"),
        SizeSpec("Medium",          os.path.join(dir, "md"),    False, "1024x768"),
        SizeSpec("Large",           os.path.join(dir, "lg"),    False)
    ]

def is_raw(file):
    return (
        file.casefold().endswith(".NEF".casefold())
        or
        file.casefold().endswith(".DNG".casefold())
    )

def export_tif(file):
    subprocess.run([
        "rawtherapee-cli",
        "-d",  # default profile
        "-s",  # sidecar pp3 profile (if exists)
        "-t",  # tif output
        "-c", file
    ], stdout = subprocess.DEVNULL, stderr = subprocess.DEVNULL)

def change_extension(path: str, newExtension: str):
    return path.replace(PurePath(path).suffix, newExtension)

def finalize_image(file: str, spec: SizeSpec):
    magickArgs = [
        "magick",
        file
    ]

    if spec.resizeGeometry:
        magickArgs += [ "-resize", spec.resizeGeometry ]

    if spec.cropGeometry:
        magickArgs += [
            "-gravity", "center",
            "-crop", spec.cropGeometry
        ]

    magickArgs += [
        "-strip",
        "-quality", "88",
        os.path.join(spec.subdir, change_extension(PurePath(file).name, ".jpg"))
    ]

    subprocess.run(magickArgs, stdout = subprocess.DEVNULL, stderr = subprocess.DEVNULL)

def resize_photo(srcFile: str, ctx: Context):
    if is_raw(srcFile):
        export_tif(srcFile)
        srcFile = change_extension(srcFile, ".tif")

    for sizeSpec in ctx.categorySpec.sizeSpecs:
        if sizeSpec.isOrig:
            continue

        finalize_image(srcFile, sizeSpec)

    if PurePath(srcFile).suffix == ".tif":
        os.remove(srcFile)

def resize_photos(ctx: Context):
    print(f"{Colors.HEADER}Resizing Photos{Colors.ENDC}")

    imageFiles = list(filter(
        lambda x: os.path.isfile(x),
        glob.glob(os.path.join(ctx.categorySpec.rootDir, "*[!.pp3]"))
    ))

    # my 16core/32thread cpu reports 32. observed optimal time was using ~12, where 8-16 were roughly
    # same amount of time, so we div by 2 and sub two to try and leave one core avail
    poolSize = max(2, int(len(os.sched_getaffinity(0)) / 2) - 2)

    start = time.time()

    with Pool(poolSize) as pool:
        pool.starmap(resize_photo, zip(imageFiles, repeat(ctx)))

    end = time.time()

    return end - start

def verify_destination_does_not_exist(ctx: Context):
    if os.path.isdir(ctx.categorySpec.deployCategoryRoot):
        print(f"{Colors.WARNING}Destination is already taken, please ensure unique category names!{Colors.ENDC}")
        sys.exit()

def prompt_directory(prompt: str):
    val = None

    readline.set_completer_delims(' \t\n=')
    readline.parse_and_bind("tab: complete")

    while not os.path.isdir(val):
        val = input(prompt)

    return val

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

def clean_prior_attempts(ctx: Context):
    if(not os.path.isdir(ctx.categorySpec.srcDir)):
        return

    for f in glob.glob(os.path.join(ctx.categorySpec.srcDir, "*")):
        shutil.move(f, ctx.categorySpec.rootDir)

    for size in ctx.categorySpec.sizeSpecs:
        if(os.path.isdir(size.subdir)):
            shutil.rmtree(size.subdir)

def prepare_size_dirs(ctx: Context):
    for size in ctx.categorySpec.sizeSpecs:
        if(not os.path.isdir(size.subdir)):
            os.mkdir(size.subdir)

def correct_intermediate_filenames(ctx: Context):
    for f in glob.glob(os.path.join(ctx.categorySpec.rootDir, "*-NEF.*")):
        os.rename(f, f.replace("-NEF", ""))

def move_source_files_with_dng(ctx: Context):
    dngs = glob.glob(os.path.join(ctx.categorySpec.rootDir, "*.dng"))
    stemmedDngs = list(map(lambda x: PurePath(x).stem, dngs))
    nonDngs = list(filter(
        lambda x: (os.path.isfile(x)) and (not x.endswith(".dng")),
        glob.glob(os.path.join(ctx.categorySpec.rootDir, "*"))
    ))

    for f in nonDngs:
        if PurePath(f).stem in stemmedDngs:
            shutil.move(f, ctx.categorySpec.srcDir)

def move_non_dng_source_files(ctx: Context):
    nonDngs = list(filter(
        lambda x: os.path.isfile(x),
        glob.glob(os.path.join(ctx.categorySpec.rootDir, "*[!.dng]"))
    ))

    for f in nonDngs:
        shutil.move(f, ctx.categorySpec.srcDir)

def process_photos(ctx: Context):
    clean_prior_attempts(ctx)
    prepare_size_dirs(ctx)
    correct_intermediate_filenames(ctx)
    move_source_files_with_dng(ctx)
    resizeDuration = resize_photos(ctx)
    move_non_dng_source_files(ctx)

def deploy(ctx: Context):
    print("deploy: todo")

def backup(ctx: Context):
    print("backup: todo")

def build_context():
    # dir = prompt_directory("Please enter the path to the photos: ")
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
    process_photos(ctx)

    doContinue = prompt_string_required("Would you like to backup and deploy at this time? [y|N]: ")

    if(doContinue != "y"):
        sys.exit()

    # note: we no longer get aws hashtree ids from storing in s3 glacier deep archive
    #       so we might as well push sooner than later so we can verify the images on
    #       the site while the backup runs
    deploy(ctx)
    backup(ctx)

    print(f"{Colors.OKGREEN}Completed!{Colors.ENDC}")

if __name__=="__main__":
    main()
