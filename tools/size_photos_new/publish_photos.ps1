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

        # secondary conversions
        @{
            name = "Thumbnail Fixed Size"
            subdir = Join-Path $dir "xs_sq"
            resizeGeometry = "160x120^"
            cropGeometry = "160x120+0+0"
            resizeSourceDir = Join-Path $dir "md"
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

        if(-not $spec.resizeSourceDir) {
            $rtArgs = BuildRawTherapeeArgs -dir $dir -spec $spec

            rawtherapee-cli @rtArgs > $null 2>&1
        } else {
            # could not figure out how to resize and center crop with rawtherapee,so using imagemagick here
            # https://stackoverflow.com/questions/32466048/imagemagick-convert-resize-then-crop
            magick mogrify `
                -path $spec.subdir `
                -resize $spec.resizeGeometry `
                -gravity center `
                -crop $spec.cropGeometry `
                (Join-Path $spec.resizeSourceDir "*.*")
        }
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

function ReadFilesystemInfo {
    param(
        [Parameter(Mandatory = $true)] [string] $dir
    )

    $result = @{}
    $output = du -b (Join-Path $dir "**/*")

    foreach($line in $output) {
        $parts = $line.Split("`t")

        $result[$parts[1]] = $parts[0]
    }

    return $result
}

function ReadImageDimensions {
    param(
        [Parameter(Mandatory = $true)] [string] $dir
    )

    $result = @{}
    $output = magick identify (Join-Path $dir "**/*") `
        2>$null

    foreach($line in $output) {
        $parts = $line.Split(' ')
        $dimensions = $parts[2].Split('x')

        $result[$parts[0]] = @{ width = $dimensions[0]; height = $dimensions[1] }
    }

    return $result
}

function MergeMetadata {
    param(
        [Parameter(Mandatory = $true)] [array] $exif,
        [Parameter(Mandatory = $true)] [hashtable] $dimensions,
        [Parameter(Mandatory = $true)] [hashtable] $fileSizes
    )

    $metadata = @{}

    foreach($dimKey in $dimensions.Keys) {
        if($fileSizes.ContainsKey($dimKey)) {
            $dim = $dimensions[$dimKey]
            $fs = $fileSizes[$dimKey]
            $sizeDir = Get-Item ([System.IO.Path]::GetDirectoryName($dimKey))
            $sizeKey = $sizeDir.Name
            $file = [System.IO.Path]::GetFilenameWithoutExtension($dimKey)

            if(-not $metadata.ContainsKey($file)) {
                $metadata.Add($file, @{})
            }

            $metadata[$file].Add($sizeKey, @{
                path = $dimKey
                width = $dim.width
                height = $dim.height
                size = $fs
            })
        }
    }

    foreach($exifFile in $exif) {
        $file = [System.IO.Path]::GetFilenameWithoutExtension($exifFile.SourceFile)

        $metadata[$file].Add("exif", $exifFile)
    }

    return $metadata
}

function GetMetadata {
    param(
        [Parameter(Mandatory = $true)] [string] $dir
    )

    $exif = ReadExif -dir (BuildSrcDir $dir)
    $fs = ReadFilesystemInfo -dir $dir
    $dim = ReadImageDimensions -dir $dir

    return MergeMetadata `
        -exif $exif `
        -dimensions $dim `
        -fileSizes $fs `
}

function SqlString {
    param(
        [Parameter(Mandatory = $false)] [object] $val
    )

    if ($null -eq $val || [string]::IsNullOrWhiteSpace($val))
    {
        return "NULL";
    }

    $val = "${val}".Replace("'", "''");

    return "'${val}'";
}

function SqlNumber {
    param(
        [Parameter(Mandatory = $false)] [object] $val
    )

    if ($null -eq $val || [string]::IsNullOrWhiteSpace($val))
    {
        return "NULL";
    }

    return "${val}";
}

function ParseDate {
    param(
        [Parameter(Mandatory = $true)] [object] $val
    )

    [datetime]$dt = New-Object DateTime

    [string[]] $formats = @(
        "yyyy:MM:dd HH:mm:ss"
    )

    if([System.DateTime]::TryParseExact(
        $val,
        $formats,
        [System.Globalization.CultureInfo]::CurrentCulture,
        [System.Globalization.DateTimeStyles]::AssumeLocal,
        [ref]$dt
    )) {
        return $dt
    }

    return $null
}

function SqlTimestamp {
    param(
        [Parameter(Mandatory = $false)] [object] $dt
    )

    if ($null -eq $dt)
    {
        return "NULL";
    }

    $type = $dt.GetType().Name

    if($type -eq 'PSCustomObject') {
        $dt = ParseDate -val $dt.val
    }

    if($type -eq 'string') {
        $dt = ParseDate -val $dt
    }

    if ($null -eq $dt)
    {
        return "NULL";
    }

    return SqlString -val $dt.ToString("yyyy-MM-dd HH:mm:sszzz")
}

function SqlCreateLookup {
    param(
        [Parameter(Mandatory = $true)] [string] $table,
        [Parameter(Mandatory = $true)] [string] $value
    )

    if ([string]::IsNullOrWhiteSpace($value)) {
        return string.Empty;
    }

    $val = SqlString -val $value

    return @"
IF NOT EXISTS (SELECT 1 FROM ${table} WHERE name = ${val}) THEN
    INSERT INTO ${table}
    (
        name
    )
    VALUES
    (
        ${val}
    );
END IF;

"@
}

function SqlLookupId {
    param(
        [Parameter(Mandatory = $true)] [string] $table,
        [Parameter(Mandatory = $false)] [string] $value
    )

    if ([string]::IsNullOrWhiteSpace($value))
    {
        return "NULL";
    }

    return "(SELECT id FROM ${table} WHERE name = $(SqlString -val $value)";
}

function WriteSqlHeader {
    param(
        [Parameter(Mandatory = $true)] [System.IO.StreamWriter] $writer
    )

    $writer.WriteLine("DO");
    $writer.WriteLine("`$`$");
    $writer.WriteLine("BEGIN");
    $writer.WriteLine("");
}

function WriteSqlFooter {
    param(
        [Parameter(Mandatory = $true)] [System.IO.StreamWriter] $writer
    )

    $writer.WriteLine("END");
    $writer.WriteLine("`$`$");
}

function WriteSqlLookup {
    param(
        [Parameter(Mandatory = $true)] [System.IO.StreamWriter] $writer,
        [Parameter(Mandatory = $true)] [string] $table,
        [Parameter(Mandatory = $true)] [array] $values
    )

    foreach ($val in $values)
    {
        $lookup = SqlCreateLookup -table $table -value $val

        if (-not [string]::IsNullOrWhiteSpace($lookup)) {
            $writer.WriteLine($lookup)
        }
    }
}

function WriteSqlLookups {
    param(
        [Parameter(Mandatory = $true)] [System.IO.StreamWriter] $writer,
        [Parameter(Mandatory = $true)] [hashtable] $metadata
    )

    WriteSqlLookup -writer $writer -table "photo.active_d_lighting" -values ($metadata.Values | Select-Object $_.exif | Select-Object $_.ActiveDLighting -Unique)
    WriteSqlLookup -writer $writer -table "photo.af_area_mode" -values ($metadata.Values | Select-Object $_.exif | Select-Object $_.AutoFocusAreaMode -Unique)
    WriteSqlLookup -writer $writer -table "photo.af_point" -values ($metadata.Values | Select-Object $_.exif | Select-Object $_.AutoFocusPoint -Unique)
    WriteSqlLookup -writer $writer -table "photo.colorspace" -values ($metadata.Values | Select-Object $_.exif | Select-Object $_.Colorspace -Unique)
    WriteSqlLookup -writer $writer -table "photo.flash_color_filter" -values ($metadata.Values | Select-Object $_.exif | Select-Object $_.FlashColorFilter -Unique)
    WriteSqlLookup -writer $writer -table "photo.flash_mode" -values ($metadata.Values | Select-Object $_.exif | Select-Object $_.FlashMode -Unique)
    WriteSqlLookup -writer $writer -table "photo.flash_setting" -values ($metadata.Values | Select-Object $_.exif | Select-Object $_.FlashSetting -Unique)
    WriteSqlLookup -writer $writer -table "photo.flash_type" -values ($metadata.Values | Select-Object $_.exif | Select-Object $_.FlashType -Unique)
    WriteSqlLookup -writer $writer -table "photo.focus_mode" -values ($metadata.Values | Select-Object $_.exif | Select-Object $_.FocusMode -Unique)
    WriteSqlLookup -writer $writer -table "photo.high_iso_noise_reduction" -values ($metadata.Values | Select-Object $_.exif | Select-Object $_.HighIsoNoiseReduction -Unique)
    WriteSqlLookup -writer $writer -table "photo.hue_adjustment" -values ($metadata.Values | Select-Object $_.exif | Select-Object $_.HueAdjustment -Unique)
    WriteSqlLookup -writer $writer -table "photo.lens" -values ($metadata.Values | Select-Object $_.exif | Select-Object $_.LensId -Unique)
    WriteSqlLookup -writer $writer -table "photo.make" -values ($metadata.Values | Select-Object $_.exif | Select-Object $_.Make -Unique)
    WriteSqlLookup -writer $writer -table "photo.model" -values ($metadata.Values | Select-Object $_.exif | Select-Object $_.Model -Unique)
    WriteSqlLookup -writer $writer -table "photo.noise_reduction" -values ($metadata.Values | Select-Object $_.exif | Select-Object $_.NoiseReduction -Unique)
    WriteSqlLookup -writer $writer -table "photo.picture_control_name" -values ($metadata.Values | Select-Object $_.exif | Select-Object $_.PictureControlName -Unique)
    WriteSqlLookup -writer $writer -table "photo.saturation" -values ($metadata.Values | Select-Object $_.exif | Select-Object $_.Saturation -Unique)
    WriteSqlLookup -writer $writer -table "photo.vibration_reduction" -values ($metadata.Values | Select-Object $_.exif | Select-Object $_.VibrationReduction -Unique)
    WriteSqlLookup -writer $writer -table "photo.vignette_control" -values ($metadata.Values | Select-Object $_.exif | Select-Object $_.VignetteControl -Unique)
    WriteSqlLookup -writer $writer -table "photo.vr_mode" -values ($metadata.Values | Select-Object $_.exif | Select-Object $_.VRMode -Unique)
    WriteSqlLookup -writer $writer -table "photo.white_balance" -values ($metadata.Values | Select-Object $_.exif | Select-Object $_.WhiteBalance -Unique)
    WriteSqlLookup -writer $writer -table "photo.gps_altitude_ref" -values ($metadata.Values | Select-Object $_.exif | Select-Object $_.GpsAltitudeRef -Unique)
}

function BuildUrl {
    param(
        [Parameter(Mandatory = $true)] [hashtable] $category,
        [Parameter(Mandatory = $true)] [string] $file
    )

    $fileInfo = Get-Item $file
    $fileName = $fileInfo.Name
    $sizeDir = $fileInfo.Directory.BaseName
    $categoryDir = $fileInfo.Directory.Parent.BaseName

    return "/images/$($category.year)/${categoryDir}/${sizeDir}/${filename}";
}

function WriteSqlResult {
    param(
        [Parameter(Mandatory = $true)] [System.IO.StreamWriter] $writer,
        [Parameter(Mandatory = $true)] [hashtable] $category,
        [Parameter(Mandatory = $true)] [hashtable] $metadata
    )

    foreach ($photo in $metadata.Values) {
        $items = @{
            "category_id" = "(SELECT currval('photo.category_id_seq'))"
            # scaled images
            "xs_height" = $photo.xs.height
            "xs_width" = $photo.xs.width
            "xs_size" = $photo.xs.size
            "xs_path" = SqlString -val (BuildUrl -category $category -file $photo.xs.path)
            "xs_sq_height" = $photo.xs_sq.height
            "xs_sq_width" = $photo.xs_sq.width
            "xs_sq_size" = $photo.xs_sq.size
            "xs_sq_path" = SqlString -val (BuildUrl -category $category -file $photo.xs_sq.path)
            "sm_height" = $photo.sm.Height
            "sm_width" = $photo.sm.Width
            "sm_size" = $photo.sm.size
            "sm_path" = SqlString -val (BuildUrl -category $category -file $photo.sm.path)
            "md_height" = $photo.md.height
            "md_width" = $photo.md.width
            "md_size" = $photo.md.size
            "md_path" = SqlString -val (BuildUrl -category $category -file $photo.md.path)
            "lg_height" = $photo.lg.height
            "lg_width" = $photo.lg.width
            "lg_size" = $photo.lg.sizeInBytes
            "lg_path" = SqlString -val (BuildUrl -category $category -file $photo.lg.path)
            "prt_height" = "NULL" # prt
            "prt_width" = "NULL" # prt
            "prt_size" = "NULL" # prt
            "prt_path" = "NULL" # prt
            "src_height" = $photo.src.height
            "src_width" = $photo.src.width
            "src_size" = $photo.src.size
            "src_path" = SqlString -val (BuildUrl -category $category -file $photo.src.path)
            # exif
            "bits_per_sample" = SqlNumber -val "$($photo.exif?.BitsPerSample?.val)"
            "compression_id" = SqlNumber -val "$($photo.exif?.Compression?.val)"
            "contrast_id" = SqlString -val "$($photo.exif?.Contrast?.val)"
            "create_date" = SqlTimestamp -dt "$($photo.exif?.CreateDate?.val)"
            "digital_zoom_ratio" = SqlNumber -val "$($photo.exif?.DigitalZoomRatio?.val)"
            "exposure_compensation" = SqlString -val "$($photo.exif?.ExposureCompensation?.val)"
            "exposure_mode_id" = SqlNumber -val "$($photo.exif?.ExposureMode?.val)"
            "exposure_program_id" = SqlNumber -val "$($photo.exif?.ExposureProgram?.val)"
            "exposure_time" = SqlString -val "$($photo.exif?.ExposureTime?.val)"
            "f_number" = SqlNumber -val "$($photo.exif?.FNumber?.val)"
            "flash_id" = SqlNumber -val "$($photo.exif?.Flash?.val)"
            "focal_length" = SqlNumber -val "$($photo.exif?.FocalLength?.val)"
            "focal_length_in_35_mm_format" = SqlNumber -val "$($photo.exif?.FocalLengthIn35mmFormat?.val)"
            "gain_control_id" = SqlNumber -val "$($photo.exif?.GainControl?.val)"
            "gps_altitude" = SqlNumber -val "$($photo.exif?.GpsAltitude?.val)"
            "gps_altitude_ref_id" = SqlLookupId -table "photo.gps_altitude_ref" -value "$($photo.exif?.GpsAltitudeRef?.val)"
            "gps_date_time_stamp" = SqlTimestamp -dt "$($photo.exif?.GpsDateStamp?.val)"
            "gps_direction" = SqlNumber -val "$($photo.exif?.GpsDirection?.val)"
            "gps_direction_ref_id" = SqlString -val "$($photo.exif?.GpsDirectionRef?.val)"
            "gps_latitude" = SqlNumber -val "$($photo.exif?.GpsLatitude?.val)"
            "gps_latitude_ref_id" = SqlString -val "$($photo.exif?.GpsLatitudeRef?.val)"
            "gps_longitude" = SqlNumber -val "$($photo.exif?.GpsLongitude?.val)"
            "gps_longitude_ref_id" = SqlString -val "$($photo.exif?.GpsLongitudeRef?.val)"
            "gps_measure_mode_id" = SqlString -val "$($photo.exif?.GpsMeasureMode?.val)"
            "gps_satellites" = SqlString -val "$($photo.exif?.GpsSatellites?.val)"
            "gps_status_id" = SqlString -val "$($photo.exif?.GpsStatus?.val)"
            "gps_version_id" = SqlString -val "$($photo.exif?.GpsVersionId?.val)"
            "iso" = SqlNumber -val "$($photo.exif?.Iso?.val)"
            "light_source_id" = SqlNumber -val "$($photo.exif?.LightSource?.val)"
            "make_id" = SqlLookupId -table "photo.make" -value "$($photo.exif?.Make?.val)"
            "metering_mode_id" = SqlNumber -val "$($photo.exif?.MeteringMode?.val)"
            "model_id" = SqlLookupId -table "photo.model" -value "$($photo.exif?.Model?.val)"
            "orientation_id" = SqlNumber -val "$($photo.exif?.Orientation?.val)"
            "saturation_id" = SqlLookupId -table "photo.saturation" -value "$($photo.exif?.Saturation?.val)"
            "scene_capture_type_id" = SqlNumber -val "$($photo.exif?.SceneCaptureType?.val)"
            "scene_type_id" = SqlNumber -val "$($photo.exif?.SceneType?.val)"
            "sensing_method_id" = SqlNumber -val "$($photo.exif?.SensingMethod?.val)"
            "sharpness_id" = SqlNumber -val "$($photo.exif?.Sharpness?.val)"
            # nikon
            "af_area_mode_id" = SqlLookupId -table "photo.af_area_mode" -value "$($photo.exif?.AutoFocusAreaMode?.val)"
            "af_point_id" = SqlLookupId -table "photo.af_point" -value "$($photo.exif?.AutoFocusPoint?.val)"
            "active_d_lighting_id" = SqlLookupId -table "photo.active_d_lighting" -value "$($photo.exif?.ActiveDLighting?.val)"
            "colorspace_id" = SqlLookupId -table "photo.colorspace" -value "$($photo.exif?.Colorspace?.val)"
            "exposure_difference" = SqlNumber -val "$($photo.exif?.ExposureDifference?.val)"
            "flash_color_filter_id" = SqlLookupId -table "photo.flash_color_filter" -value "$($photo.exif?.FlashColorFilter?.val)"
            "flash_compensation" = SqlString -val "$($photo.exif?.FlashCompensation?.val)"
            "flash_control_mode" = SqlNumber -val "$($photo.exif?.FlashControlMode?.val)"
            "flash_exposure_compensation" = SqlString -val "$($photo.exif?.FlashExposureCompensation?.val)"
            "flash_focal_length" = SqlNumber -val "$($photo.exif?.FlashFocalLength?.val)"
            "flash_mode_id" = SqlLookupId -table "photo.flash_mode" -value "$($photo.exif?.FlashMode?.val)"
            "flash_setting_id" = SqlLookupId -table "photo.flash_setting" -value "$($photo.exif?.FlashSetting?.val)"
            "flash_type_id" = SqlLookupId -table "photo.flash_type" -value "$($photo.exif?.FlashType?.val)"
            "focus_distance" = SqlNumber -val "$($photo.exif?.FocusDistance?.val)"
            "focus_mode_id" = SqlLookupId -table "photo.focus_mode" -value "$($photo.exif?.FocusMode?.val)"
            "focus_position" = SqlNumber -val "$($photo.exif?.FocusPosition?.val)"
            "high_iso_noise_reduction_id" = SqlLookupId -table "photo.high_iso_noise_reduction" -value "$($photo.exif?.HighIsoNoiseReduction?.val)"
            "hue_adjustment_id" = SqlLookupId -table "photo.hue_adjustment" -value "$($photo.exif?.HueAdjustment?.val)"
            "noise_reduction_id" = SqlLookupId -table "photo.noise_reduction" -value "$($photo.exif?.NoiseReduction?.val)"
            "picture_control_name_id" = SqlLookupId -table "photo.picture_control_name" -value "$($photo.exif?.PictureControlName?.val)"
            "primary_af_point" = SqlString -val "$($photo.exif?.PrimaryAFPoint?.val)"
            "vibration_reduction_id" = SqlLookupId -table "photo.vibration_reduction" -value "$($photo.exif?.VibrationReduction?.val)"
            "vignette_control_id" = SqlLookupId -table "photo.vignette_control" -value "$($photo.exif?.VignetteControl?.val)"
            "vr_mode_id" = SqlLookupId -table "photo.vr_mode" -value "$($photo.exif?.VRMode?.val)"
            "white_balance_id" = SqlLookupId -table "photo.white_balance" -value "$($photo.exif?.WhiteBalance?.val)"
            # composite
            "aperture" = SqlNumber -val "$($photo.exif?.Aperture?.val)"
            "auto_focus_id" = SqlNumber -val "$($photo.exif?.AutoFocus?.val)"
            "depth_of_field" = SqlString -val "$($photo.exif?.DepthOfField?.val)"
            "field_of_view" = SqlString -val "$($photo.exif?.FieldOfView?.val)"
            "hyperfocal_distance" = SqlNumber -val "$($photo.exif?.HyperfocalDistance?.val)"
            "lens_id" = SqlLookupId -table "photo.lens" -value "$($photo.exif?.LensId?.val)"
            "light_value" = SqlNumber -val "$($photo.exif?.LightValue?.val)"
            "scale_factor_35_efl" = SqlNumber -val "$($photo.exif?.ScaleFactor35Efl?.val)"
            "shutter_speed" = SqlString -val "$($photo.exif?.ShutterSpeed?.val)"
        }

        $colNames = @()
        $colValues = @()

        # hashtables make no guarantees about ordering, so build lists to enforce this
        foreach($item in $items) {
            $colNames += $item.Key
            $colValues += $item.Value
        }

        $writer.WriteLine(@"
INSERT INTO photo.photo
(
    $([string]::Join(", ", $colNames))
)
VALUES
(
    $([string]::Join(", ", $colValues))
);

"@
        )
    }
}

function WriteSqlCategoryCreate {
    param(
        [Parameter(Mandatory = $true)] [System.IO.StreamWriter] $writer,
        [Parameter(Mandatory = $true)] [hashtable] $categorySpec,
        [Parameter(Mandatory = $true)] [hashtable] $metadata
    )

    $firstKey = $($metadata.Keys)[0]
    $photo = $metadata[$firstKey];

    $writer.WriteLine(@"
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
    $(SqlString -val categorySpec.name),
    $($category.year),
    $($photo.xs.width),
    $($photo.xs.height),
    $($photo.xs.size),
    $(SqlString -val (BuildUrl -category $categorySpec -file $photo.xs.path)),
    $($photo.xs_sq.width),
    $($photo.xs_sq.height),
    $($photo.xs_sq.size),
    $(SqlString -val (BuildUrl -category $categorySpec -file $photo.xs_sq.path))
);

"@
    )

    foreach ($role in $category.allowedRoles) {
        $writer.WriteLine(@"
INSERT INTO photo.category_role
(
    category_id,
    role_id
)
VALUES
(
    (SELECT currval('photo.category_id_seq')),
    (SELECT id FROM maw.role WHERE name = $(SqlString -val $role))
);

"@
        )
    }
}

function WriteSqlCategoryUpdate {
    param(
        [Parameter(Mandatory = $true)] [System.IO.StreamWriter] $writer
    )

    $writer.WriteLine(@"
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

"@
    )
}

function WriteSql {
    param(
        [Parameter(Mandatory = $true)] [hashtable] $categorySpec,
        [Parameter(Mandatory = $true)] [hashtable] $metadata
    )

    $writer = [System.IO.StreamWriter]::new($categorySpec.sqlFile)

    WriteSqlHeader -writer $writer
    WriteSqlCategoryCreate -writer $writer -category $categorySpec -metadata $metadata
    WriteSqlLookups -writer $writer -metadata $metadata
    WriteSqlResult -writer $writer -category $categorySpec -metadata $metadata
    WriteSqlCategoryUpdate -writer $writer
    WriteSqlFooter -writer $writer

    $writer.Flush()
    $writer.Close()
}

function GetCategoryDetails {
    $dir = "/home/mmorano/Desktop/testing"
    $year = 2024
    $name = "Testing"
    $roles = @( "friend", "admin" )
    $sqlFile = (Join-Path $dir "photocategory.sql")
    $sizeSpecs = BuildSizeSpecs -dir $dir

    return @{
        srcDir = $dir
        year = $year
        name = $name
        allowedRoles = $roles
        sqlFile = $sqlFile
        sizeSpecs = $sizeSpecs
    }
}

function Main {
    $categorySpec = GetCategoryDetails
    $timer = [Diagnostics.Stopwatch]::StartNew()

    # new process assumes all RAW files will be converted to dng first (via DxO PureRaw 4)
    # MoveSourceFiles -dir $dir -pattern "*.NEF"
    # CorrectIntermediateFilenames -dir $dir
    # CleanSidecarPp3Files -dir $dir
    # ResizePhotos -dir $dir -sizeSpecs $sizeSpecs
    # MoveSourceFiles -dir $dir -pattern "*.jpg"
    # MoveSourceFiles -dir $dir -pattern "*.pp3"
    $metadata = GetMetadata -dir $categorySpec.srcDir
    WriteSql -categorySpec $categorySpec -metadata $metadata

    $timer.Stop()

    # DeleteDngFiles -dir $dir
    # PrintStats -sizeSpecs $sizeSpecs -timer $timer
}

Main
