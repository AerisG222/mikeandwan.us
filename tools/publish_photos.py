#!/usr/bin/python
import os
import readline
import subprocess
from dataclasses import dataclass
from multiprocessing import Pool

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

def exportTif(file):
    subprocess.run([
        "rawtherapee-cli",
        "-d",  # default profile
        "-s",  # sidecar pp3 profile (if exists)
        "-t",  # tif output
        "-c", file
    ])

def resize_photo(srcFile, categorySpec):
    return 1

def resize_photos(ctx: Context):
    print(f"{Colors.HEADER}Resizing Photos{Colors.ENDC}")
    return 1

def verify_destination_does_not_exist(ctx: Context):
    return os.path.isDir()

def prompt_directory(prompt: str):
    val = None

    readline.set_completer_delims(' \t\n=')
    readline.parse_and_bind("tab: complete")

    while not os.path.isDir(val):
        val = input(prompt)

    return val

def prompt_string_required(prompt: str):
    val = input(prompt)
    print(type(val))

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
    resize_photos(ctx)

if __name__=="__main__":
    main()
