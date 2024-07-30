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
        self.sqlFile = os.path.join(photoDir, "photocategory.sql")
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

    with Pool(poolSize) as pool:
        pool.starmap(resize_photo, zip(imageFiles, repeat(ctx)))

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

def read_exif(ctx: Context):
    etArgs = [
        "exiftool"
    ]

    for t in exifTags:
        etArgs.append(f"-{t}")

    etArgs += [
        "-json",
        "-tab",
        "-long",
        ctx.categorySpec.srcDir
    ]

    result = subprocess.run(etArgs, capture_output=True, text=True)

    return json.loads(result.stdout)

def read_filesystem_info(ctx: Context):
    sizes = {}
    result = subprocess.run(["du", "-ab", ctx.categorySpec.rootDir], capture_output=True, text=True)

    for line in result.stdout.splitlines():
        parts = line.split("\t")

        sizes[parts[1]] = int(parts[0])

    return sizes

def read_image_dimensions(ctx: Context):
    files = list(filter(
        lambda x: os.path.isfile(x),
        glob.glob(os.path.join(ctx.categorySpec.rootDir, "*/*[!.pp3]"))
    ))

    dimensions = {}

    for f in files:
        result = subprocess.run(["magick", "identify", f], capture_output=True, text=True)

        for line in result.stdout.splitlines():
            parts = line.split(" ")
            dim = parts[2].split("x")

            dimensions[parts[0]] = Dimension(int(dim[0]), int(dim[1]))

    return dimensions

def merge_metadata(exif, fileSizes, dimensions):
    metadata = {}

    for dimKey in dimensions:
        if dimKey in fileSizes.keys():
            dim = dimensions[dimKey]
            fs = fileSizes[dimKey]
            sizeKey = PurePath(dimKey).parent.name
            file = PurePath(dimKey).stem

            if not file in metadata.keys():
                metadata[file] = {}

            metadata[file][sizeKey] = {
                'path': dimKey,
                'width': dim.width,
                'height': dim.height,
                'size': fs
            }

    for e in exif:
        file = PurePath(e["SourceFile"]).stem

        metadata[file]["exif"] = e

    return metadata

def read_metadata(ctx: Context):
    exif = read_exif(ctx)
    fs = read_filesystem_info(ctx)
    dims = read_image_dimensions(ctx)

    return merge_metadata(exif, fs, dims)

def build_url(ctx: Context, path: str):
    filePath = PurePath(path)
    sizePart = filePath.parent.name
    categoryPart = filePath.parent.parent.name

    return f"/images/{ctx.categorySpec.year}/{categoryPart}/{sizePart}/{filePath.name}"

def sql_str(val):
    if not val:
        return "NULL"

    val.replace("'", "''")
    return f"'{val}'"

def write_sql_header(f):
    f.write(
"""
DO
$$
BEGIN
"""
    )

def write_sql_footer(f):
    f.write(
"""
END
$$

\\q
"""
    )

def write_sql_category_create(f, ctx: Context, metadata):
    photo = next(iter(metadata.values()))

    f.write(
f"""
INSERT INTO photo.category
(
    name,
    year,
    teaser_photo_width,
    teaser_photo_height,
    teaser_photo_size,
    teaser_photo_path,
    teaser_photo_sq_width,
    teaser_photo_sq_height,
    teaser_photo_sq_size,
    teaser_photo_sq_path
)
VALUES
(
    {sql_str(ctx.categorySpec.name)},
    {ctx.categorySpec.year},
    {photo["xs"]["width"]},
    {photo["xs"]["height"]},
    {photo["xs"]["size"]},
    {sql_str(build_url(ctx, photo["xs"]["path"]))},
    {photo["xs_sq"]["width"]},
    {photo["xs_sq"]["height"]},
    {photo["xs_sq"]["size"]},
    {sql_str(build_url(ctx, photo["xs_sq"]["path"]))}
);
"""
    )

def write_sql_category_update(f):
    f.write(
"""
UPDATE photo.category c
   SET photo_count = (SELECT COUNT(1) FROM photo.photo WHERE category_id = c.id),
       create_date = (SELECT create_date FROM photo.photo WHERE id = (SELECT MIN(id) FROM photo.photo where category_id = c.id AND create_date IS NOT NULL)),
       gps_latitude = (SELECT gps_latitude FROM photo.photo WHERE id = (SELECT MIN(id) FROM photo.photo WHERE category_id = c.id AND gps_latitude IS NOT NULL)),
       gps_latitude_ref_id = (SELECT gps_latitude_ref_id FROM photo.photo WHERE id = (SELECT MIN(id) FROM photo.photo WHERE category_id = c.id AND gps_latitude IS NOT NULL)),
       gps_longitude = (SELECT gps_longitude FROM photo.photo WHERE id = (SELECT MIN(id) FROM photo.photo WHERE category_id = c.id AND gps_latitude IS NOT NULL)),
       gps_longitude_ref_id = (SELECT gps_longitude_ref_id FROM photo.photo WHERE id = (SELECT MIN(id) FROM photo.photo WHERE category_id = c.id AND gps_latitude IS NOT NULL)),
       total_size_xs = (SELECT SUM(xs_size) FROM photo.photo WHERE category_id = c.id),
       total_size_xs_sq = (SELECT SUM(xs_sq_size) FROM photo.photo WHERE category_id = c.id),
       total_size_sm = (SELECT SUM(sm_size) FROM photo.photo WHERE category_id = c.id),
       total_size_md = (SELECT SUM(md_size) FROM photo.photo WHERE category_id = c.id),
       total_size_lg = (SELECT SUM(lg_size) FROM photo.photo WHERE category_id = c.id),
       total_size_prt = (SELECT SUM(prt_size) FROM photo.photo WHERE category_id = c.id),
       total_size_src = (SELECT SUM(src_size) FROM photo.photo WHERE category_id = c.id),
       teaser_photo_size = (SELECT xs_size FROM photo.photo WHERE category_id = c.id AND xs_path = c.teaser_photo_path),
       teaser_photo_sq_height = (SELECT xs_sq_height FROM photo.photo WHERE category_id = c.id AND xs_path = c.teaser_photo_path),
       teaser_photo_sq_width = (SELECT xs_sq_width FROM photo.photo WHERE category_id = c.id AND xs_path = c.teaser_photo_path),
       teaser_photo_sq_path = (SELECT xs_sq_path FROM photo.photo WHERE category_id = c.id AND xs_path = c.teaser_photo_path),
       teaser_photo_sq_size = (SELECT xs_sq_size FROM photo.photo WHERE category_id = c.id AND xs_path = c.teaser_photo_path)
 WHERE c.id = (SELECT currval('photo.category_id_seq'));
"""
    )

def write_sql_lookup(f, table: str, values: list):
    for v in values:
        if not v:
            continue

        val = sql_str(v)

        f.write(
f"""
IF NOT EXISTS (SELECT 1 FROM {table} WHERE name = {val}) THEN
    INSERT INTO {table}
    (
        name
    )
    VALUES
    (
        {val}
    );
END IF;
"""
        )

def unique_lookups(metadata, exifField: str):
    valSet = set()

    for v in iter(metadata.values()):
        if exifField in v["exif"]:
            val = v["exif"][exifField]

            if val:
                valSet.add(val["val"])

    return valSet

def write_sql_lookups(f, metadata):
    write_sql_lookup(f, "photo.compression", unique_lookups(metadata, "Compression"))
    write_sql_lookup(f, "photo.contrast", unique_lookups(metadata, "Contrast"))
    write_sql_lookup(f, "photo.exposure_mode", unique_lookups(metadata, "ExposureMode"))
    write_sql_lookup(f, "photo.exposure_program", unique_lookups(metadata, "ExposureProgram"))
    write_sql_lookup(f, "photo.flash", unique_lookups(metadata, "Flash"))
    write_sql_lookup(f, "photo.gain_control", unique_lookups(metadata, "GainControl"))
    write_sql_lookup(f, "photo.gps_altitude_ref", unique_lookups(metadata, "GpsAltitudeRef"))
    write_sql_lookup(f, "photo.gps_direction_ref", unique_lookups(metadata, "GpsImgDirectionRef"))
    write_sql_lookup(f, "photo.gps_latitude_ref", unique_lookups(metadata, "GpsLatitudeRef"))
    write_sql_lookup(f, "photo.gps_longitude_ref", unique_lookups(metadata, "GpsLongitudeRef"))
    write_sql_lookup(f, "photo.gps_measure_mode", unique_lookups(metadata, "GpsMeasureMode"))
    write_sql_lookup(f, "photo.gps_status", unique_lookups(metadata, "GpsStatus"))
    write_sql_lookup(f, "photo.light_source", unique_lookups(metadata, "LightSource"))
    write_sql_lookup(f, "photo.make", unique_lookups(metadata, "Make"))
    write_sql_lookup(f, "photo.metering_mode", unique_lookups(metadata, "MeteringMode"))
    write_sql_lookup(f, "photo.model", unique_lookups(metadata, "Model"))
    write_sql_lookup(f, "photo.orientation", unique_lookups(metadata, "Orientation"))
    write_sql_lookup(f, "photo.saturation", unique_lookups(metadata, "Saturation"))
    write_sql_lookup(f, "photo.scene_capture_type", unique_lookups(metadata, "SceneCaptureType"))
    write_sql_lookup(f, "photo.scene_type", unique_lookups(metadata, "SceneType"))
    write_sql_lookup(f, "photo.sensing_method", unique_lookups(metadata, "SensingMethod"))
    write_sql_lookup(f, "photo.sharpness", unique_lookups(metadata, "Sharpness"))
    write_sql_lookup(f, "photo.af_area_mode", unique_lookups(metadata, "AFAreaMode"))
    write_sql_lookup(f, "photo.af_point", unique_lookups(metadata, "AFPoint"))
    write_sql_lookup(f, "photo.active_d_lighting", unique_lookups(metadata, "'"))
    write_sql_lookup(f, "photo.colorspace", unique_lookups(metadata, "Colorspace"))
    write_sql_lookup(f, "photo.flash_color_filter", unique_lookups(metadata, "FlashColorFilter"))
    write_sql_lookup(f, "photo.flash_mode", unique_lookups(metadata, "FlashMode"))
    write_sql_lookup(f, "photo.flash_setting", unique_lookups(metadata, "FlashSetting"))
    write_sql_lookup(f, "photo.flash_type", unique_lookups(metadata, "FlashType"))
    write_sql_lookup(f, "photo.focus_mode", unique_lookups(metadata, "FocusMode"))
    write_sql_lookup(f, "photo.high_iso_noise_reduction", unique_lookups(metadata, "HighIsoNoiseReduction"))
    write_sql_lookup(f, "photo.hue_adjustment", unique_lookups(metadata, "HueAdjustment"))
    write_sql_lookup(f, "photo.noise_reduction", unique_lookups(metadata, "NoiseReduction"))
    write_sql_lookup(f, "photo.picture_control_name", unique_lookups(metadata, "PictureControlName"))
    write_sql_lookup(f, "photo.vibration_reduction", unique_lookups(metadata, "VibrationReduction"))
    write_sql_lookup(f, "photo.vignette_control", unique_lookups(metadata, "VignetteControl"))
    write_sql_lookup(f, "photo.vr_mode", unique_lookups(metadata, "VRMode"))
    write_sql_lookup(f, "photo.white_balance", unique_lookups(metadata, "WhiteBalance"))
    write_sql_lookup(f, "photo.auto_focus", unique_lookups(metadata, "AutoFocus"))
    write_sql_lookup(f, "photo.lens", unique_lookups(metadata, "LensId"))

def write_sql(ctx: Context, metadata):
    f = open(ctx.categorySpec.sqlFile, "w")

    write_sql_header(f)
    write_sql_category_create(f, ctx, metadata)
    write_sql_lookups(f, metadata)
    # write_sql_result(f, ctx, metadata)
    write_sql_category_update(f)
    write_sql_footer(f)

    f.close()

def process_photos(ctx: Context):
    start = time.time()

    # clean_prior_attempts(ctx)
    # prepare_size_dirs(ctx)
    # correct_intermediate_filenames(ctx)
    # move_source_files_with_dng(ctx)
    # resizeDuration = resize_photos(ctx)
    # move_non_dng_source_files(ctx)
    metadata = read_metadata(ctx)
    write_sql(ctx, metadata)

    end = time.time()
    return end - start

def deploy(ctx: Context):
    start = time.time()

    print("deploy: todo")

    end = time.time()
    return end - start

def backup(ctx: Context):
    start = time.time()

    print("backup: todo")

    end = time.time()
    return end - start

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
    resizeDuration = process_photos(ctx)

    doContinue = prompt_string_required("Would you like to backup and deploy at this time? [y|N]: ")

    if(doContinue != "y"):
        sys.exit()

    # note: we no longer get aws hashtree ids from storing in s3 glacier deep archive
    #       so we might as well push sooner than later so we can verify the images on
    #       the site while the backup runs
    deployDuration = deploy(ctx)
    backupDuration = backup(ctx)

    # 1 = thumbnail folder
    photos = len(glob.glob(os.path.join(ctx.categorySpec.sizeSpecs[1].subdir, "*")))

    print_stats(photos, resizeDuration, deployDuration, backupDuration)

    print(f"{Colors.HEADER}Completed!{Colors.ENDC}")

if __name__=="__main__":
    main()
