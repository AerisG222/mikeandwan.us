#!~/.dotnet/tools/pwsh

$exifTags = @(
    # exif
    "BitsPerSample"
    "Compression"
    "Contrast"
    "CreateDate"
    "DigitalZoomRatio"
    "ExposureCompensation"
    "ExposureMode"
    "ExposureProgram"
    "ExposureTime"
    "FNumber"
    "Flash"
    "FocalLength"
    "FocalLengthIn35mmFormat"
    "GainControl"
    "GPSAltitude"
    "GPSAltitudeRef"
    "GPSDateStamp"
    "GPSImgDirection"
    "GPSImgDirectionRef"
    "GPSLatitude"
    "GPSLatitudeRef"
    "GPSLongitude"
    "GPSLongitudeRef"
    "GPSMeasureMode"
    "GPSSatellites"
    "GPSStatus"
    "GPSVersionID"
    "ISO"
    "LightSource"
    "Make"
    "MeteringMode"
    "Model"
    "Orientation"
    "Saturation"
    "SceneCaptureType"
    "SceneType"
    "SensingMethod"
    "Sharpness"

    # nikon
    "AFAreaMode"
    "AFPoint"
    "ActiveD-Lighting"
    "ColorSpace"
    "ExposureDifference"
    "FlashColorFilter"
    "FlashCompensation"
    "FlashControlMode"
    "FlashExposureComp"
    "FlashFocalLength"
    "FlashMode"
    "FlashSetting"
    "FlashType"
    "FocusDistance"
    "FocusMode"
    "FocusPosition"
    "HighIsoNoiseReduction"
    "HueAdjustment"
    "NoiseReduction"
    "PictureControlName"
    "PrimaryAFPoint"
    "VRMode"
    "VibrationReduction"
    "VignetteControl"
    "WhiteBalance"

    # composite
    "Aperture"
    "AutoFocus"
    "DOF"
    "FOV"
    "HyperfocalDistance"
    "LensID"
    "LightValue"
    "ScaleFactor35efl"
    "ShutterSpeed"
)

function BuildSizeSpecs {
    param(
        [Parameter(Mandatory = $true)] [string] $dir
    )

    $sizeSpecs = @(
        @{
            name = "Thumbnail"
            subdir = Join-Path $dir "xs"
            resizePp3 = Join-Path $PSScriptRoot "resize_xs.pp3"
        }
        @{
            name = "Small"
            subdir = Join-Path $dir "sm"
            resizePp3 = Join-Path $PSScriptRoot "resize_sm.pp3"
        }
        @{
            name = "Medium"
            subdir = Join-Path $dir "md"
            resizePp3 = Join-Path $PSScriptRoot "resize_md.pp3"
        }
        @{
            name = "Large"
            subdir = Join-Path $dir "lg"
        }
    )

    return $sizeSpecs
}

function BuildRawTherapeeArgs {
    param(
        [Parameter(Mandatory = $true)] [string] $dir,
        [Parameter(Mandatory = $true)] [hashtable] $spec
    )

    $pp3StripMetadata = Join-Path $PSScriptRoot "strip_metadata.pp3"

    $rtArgs = @(
        "-o", "$($spec.subdir)"
        "-d"
        "-p", "${pp3StripMetadata}"
    )

    if($null -ne $spec.resizePp3) {
        $rtArgs += @("-p", "$($spec.resizePp3)")
    }

    $rtArgs += "-s"
    $rtArgs += "-j82"
    $rtArgs += "-js3"
    $rtArgs += @("-c", "${dir}")

    return $rtArgs
}

function CleanSidecarPp3Files {
    param(
        [Parameter(Mandatory = $true)] [string] $dir
    )

    $files = Get-ChildItem "$(Join-Path $dir "*.pp3")"

    # sidecar file will contain resize settings that would overrule the ones passed in the -p arg
    # to make sure resizing works, remove those settings from the sidecars
    foreach($file in $files) {
        crudini `
            --del "$($file.FullName)" Resize `
            --del "$($file.FullName)" PostResizeSharpening
    }
}

function ResizePhotos {
    param(
        [Parameter(Mandatory = $true)] [string] $dir,
        [Parameter(Mandatory = $true)] [array] $sizeSpecs
    )

    foreach($spec in $sizeSpecs) {
        Write-Host -ForegroundColor Blue "Resizing: $($spec.name)"

        if(-not (Test-Path $spec.subdir)) {
            mkdir "$($spec.subdir)"
        }

        $rtArgs = BuildRawTherapeeArgs -dir $dir -spec $spec

        rawtherapee-cli @rtArgs > $null 2>&1
    }
}

function BuildSrcDir {
    param(
        [Parameter(Mandatory = $true)] [string] $dir
    )

    return Join-Path $dir "src"
}

function MoveSourceFiles {
    param(
        [Parameter(Mandatory = $true)] [string] $dir,
        [Parameter(Mandatory = $true)] [string] $pattern
    )

    $srcDir = BuildSrcDir $dir

    if(-not (Test-Path $srcDir)) {
        mkdir "${srcDir}"
    }

    mv $(Join-Path $dir $pattern) "${srcDir}"
}

function DeleteDngFiles {
    param(
        [Parameter(Mandatory = $true)] [string] $dir
    )

    $res = Read-Host -Prompt "Would you like to delete the DNG files?  [y/N]"

    if($res -eq "y") {
        rm "$(Join-Path $dir "*.dng")"
    }
}

function PrintStats {
    param(
        [Parameter(Mandatory = $true)] [array] $sizeSpecs,
        [Parameter(Mandatory = $true)] [Diagnostics.Stopwatch] $timer
    )

    $filecount = (Get-ChildItem $sizeSpecs[0].subdir).Length
    $totalTime = @()

    if($timer.Elapsed.Days -gt 0) {
        $totalTime += @("$($timer.Elapsed.Days)d")
    }

    if($timer.Elapsed.Hours -gt 0) {
        $totalTime += @("$($timer.Elapsed.Hours)h")
    }

    if($timer.Elapsed.Minutes -gt 0) {
        $totalTime += @("$($timer.Elapsed.Minutes)m")
    }

    $totalTime += @("$($timer.Elapsed.Seconds)s")

    Write-Host -ForegroundColor Green "Processed Files: ${filecount}"
    Write-Host -ForegroundColor Green "Total Execution Time: ${totalTime}"
    Write-Host -ForegroundColor Green "Average per photo: $([math]::Round($timer.Elapsed.TotalSeconds / $filecount, 2))s"
}

function CorrectIntermediateFilenames {
    param(
        [Parameter(Mandatory = $true)] [string] $dir
    )

    # default pure raw filename will include the orig extension as "-NEF", remove that if present so the intermediate
    # file names (dng/pp3) are in line w/ the original filenames
    rename -- '-NEF' '' $(Join-Path $dir "*.dng")
    rename -- '-NEF' '' $(Join-Path $dir "*.pp3")
}

function BuildExifToolArgs {
    param(
        [Parameter(Mandatory = $true)] [string] $dir
    )

    foreach($tag in $exifTags) {
        $etArgs += @( "-${tag}" )
    }

    $etArgs += @(
        "-json"
        "-tab"
        "-long"
        $dir
    )

    return $etArgs
}

function ReadExif {
    param(
        [Parameter(Mandatory = $true)] [string] $dir
    )

    # i first looked to see if exiv2 could replace exiftool (it seems faster)
    # but does not support all the tags i've used (one example is flashinfo data which is extracted by exiftool)
    # given this: sticking w/ exiftool for now

    $etArgs = BuildExifToolArgs -dir $dir

    return exiftool @etArgs `
        2>$null `
        | ConvertFrom-Json
}

function Main {
    $dir = "/home/mmorano/Desktop/testing"
    $sizeSpecs = BuildSizeSpecs -dir $dir
    $timer = [Diagnostics.Stopwatch]::StartNew()

    # new process assumes all RAW files will be converted to dng first (via DxO PureRaw 4)
    # MoveSourceFiles -dir $dir -pattern "*.NEF"
    # CorrectIntermediateFilenames -dir $dir
    # CleanSidecarPp3Files -dir $dir
    # ResizePhotos -dir $dir -sizeSpecs $sizeSpecs
    # MoveSourceFiles -dir $dir -pattern "*.jpg"
    # MoveSourceFiles -dir $dir -pattern "*.pp3"
    $exif = ReadExif -dir (BuildSrcDir $dir)

    write-host $exif

    $timer.Stop()

    # DeleteDngFiles -dir $dir
    # PrintStats -sizeSpecs $sizeSpecs -timer $timer
}

Main
