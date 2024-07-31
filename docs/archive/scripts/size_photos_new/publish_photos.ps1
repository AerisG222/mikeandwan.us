#!/home/mmorano/.dotnet/tools/pwsh

param(
    [Parameter(Mandatory = $true)] [string] $photoDir
)

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
        "-s"
        "-p", "${pp3StripMetadata}"
    )

    if($null -ne $spec.resizePp3) {
        $rtArgs += @("-p", "$($spec.resizePp3)")
    }

    $rtArgs += "-j88"
    $rtArgs += "-js3"
    $rtArgs += @("-c", "${dir}")

    return $rtArgs
}

# function CleanSidecarPp3Files {
#     param(
#         [Parameter(Mandatory = $true)] [hashtable] $categorySpec
#     )

#     $files = Get-ChildItem "$(Join-Path $categorySpec.rootDir "*.pp3")"

#     # sidecar file will contain resize settings that would overrule the ones passed in the -p arg
#     # to make sure resizing works, remove those settings from the sidecars
#     foreach($file in $files) {
#         crudini `
#             --del "$($file.FullName)" Resize `
#             --del "$($file.FullName)" PostResizeSharpening
#     }
# }

function ResizePhotos {
    param(
        [Parameter(Mandatory = $true)] [hashtable] $categorySpec
    )

    foreach($spec in $categorySpec.sizeSpecs) {
        Write-Host -ForegroundColor Blue "Resizing: $($spec.name)"

        if(-not (Test-Path $spec.subdir)) {
            mkdir "$($spec.subdir)"
        }

        if(-not $spec.resizeSourceDir) {
            $rtArgs = BuildRawTherapeeArgs -dir $categorySpec.rootDir -spec $spec

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

function CleanPriorAttempts {
    param(
        [Parameter(Mandatory = $true)] [hashtable] $categorySpec
    )

    if(Test-Path -PathType Container $categorySpec.srcDir) {
        mv (Join-Path $categorySpec.srcDir "*") $categorySpec.rootDir

        # remove all the subdirs
        rmdir "$($categorySpec.srcDir)"

        foreach($spec in $categorySpec.sizeSpecs) {
            rm -rf "$($spec.subdir)"
        }

        if(Test-Path -PathType Leaf $categorySpec.sqlFile) {
            rm "$($categorySpec.sqlFile)"
        }
    }
}

function MoveSourceFiles {
    param(
        [Parameter(Mandatory = $true)] [hashtable] $categorySpec
    )

    if(-not (Test-Path $categorySpec.srcDir)) {
        mkdir "$($categorySpec.srcDir)"
    }

    $nonDngFiles = Get-ChildItem -File $categorySpec.rootDir `
        | Where-Object { [System.IO.Path]::GetExtension($_.Name) -ne ".dng" }

    foreach($file in $nonDngFiles) {
        $dest = Join-Path $categorySpec.srcDir $file.Name

        $file.MoveTo($dest)
    }
}

function MoveSourceFilesWithDng {
    param(
        [Parameter(Mandatory = $true)] [hashtable] $categorySpec
    )

    if(-not (Test-Path $categorySpec.srcDir)) {
        mkdir "$($categorySpec.srcDir)"
    }

    $dngPrefixes = Get-ChildItem (Join-Path $categorySpec.rootDir "*.dng") `
        | Select-Object -Property BaseName -ExpandProperty BaseName

    $filesWithDng = Get-ChildItem -File $categorySpec.rootDir `
        | Where-Object { `
            [System.IO.Path]::GetExtension($_.Name) -ne ".dng" `
            -and `
            $dngPrefixes.Contains($_.BaseName) `
        }

    foreach($file in $filesWithDng) {
        $dest = Join-Path $categorySpec.srcDir $file.Name

        $file.MoveTo($dest)
    }
}

function ApplySqlToLocalDatabase {
    param(
        [Parameter(Mandatory = $true)] [hashtable] $categorySpec,
        [Parameter(Mandatory = $true)] [hashtable] $config
    )

    podman run -it --rm `
        --pull "newer" `
        --pod "$($config.dev.pod)" `
        --env-file "$($config.dev.postgresEnvFile)" `
        --volume "$($categorySpec.deploySpec.destDir):/output:ro" `
        --security-opt label=disable `
        "$($config.postgresImage)" `
            psql `
                -h "localhost" `
                -U "postgres" `
                -d "maw_website" `
                -f "/output/$([System.IO.Path]::GetFilename($categorySpec.sqlFile))"
}

function CopyImagesAndSqlToRemoteDestination {
    param(
        [Parameter(Mandatory = $true)] [hashtable] $categorySpec,
        [Parameter(Mandatory = $true)] [hashtable] $config
    )

    rsync -ah `
        --exclude "*/src*" `
        --exclude "*.dng" `
        "$($categorySpec.deploySpec.destDir)" `
        "$($config.sshUsername)@$($config.sshRemoteHost):~/"
}

function MoveProcessedImagesToLocalDestination {
    param(
        [Parameter(Mandatory = $true)] [hashtable] $categorySpec,
        [Parameter(Mandatory = $true)] [hashtable] $config
    )

    if(-not (Test-Path -PathType Container $categorySpec.deploySpec.yearDir)) {
        mkdir -p "$($categorySpec.deploySpec.yearDir)"
    }

    mv "$($categorySpec.rootDir)" "$($categorySpec.deploySpec.yearDir)"
}

function ZipPp3s {
    param(
        [Parameter(Mandatory = $true)] [hashtable] $categorySpec
    )

    cd "$($categorySpec.srcDir)" && tar --remove-files -czf pp3s.tar.gz *.pp3 && cd -
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

function FormatDuration {
    param(
        [Parameter(Mandatory = $true)] [TimeSpan] $duration
    )

    $formattedTime = @()

    if($duration.Days -gt 0) {
        $formattedTime += "$($duration.Days)d"
    }

    if($duration.Hours -gt 0) {
        $formattedTime += "$($duration.Hours)h"
    }

    if($duration.Minutes -gt 0) {
        $formattedTime += "$($duration.Minutes)m"
    }

    $formattedTime += "$($duration.Seconds)s"

    return [string]::Join(" ", $formattedTime)
}

function PrintStats {
    param(
        [Parameter(Mandatory = $true)] [array] $sizeSpecs,
        [Parameter(Mandatory = $true)] [TimeSpan] $resizeDuration,
        [Parameter(Mandatory = $true)] [TimeSpan] $backupDuration,
        [Parameter(Mandatory = $true)] [TimeSpan] $deployDuration
    )

    $filecount = (Get-ChildItem $sizeSpecs[0].subdir).Length
    $totalTime = $resizeDuration + $backupDuration + $deployDuration

    Write-Host -ForegroundColor Magenta "Processed Files: ${filecount}"

    Write-Host -ForegroundColor Green " - Total Resize Time: $(FormatDuration $resizeDuration)"
    Write-Host -ForegroundColor Green " - Average Resize Time per Photo: $(FormatDuration ($resizeDuration / $filecount))"

    Write-Host -ForegroundColor Blue " - Total Backup Time: $(FormatDuration $backupDuration)"
    Write-Host -ForegroundColor Blue " - Average Backup Time per Photo: $(FormatDuration ($backupDuration / $filecount))"

    Write-Host -ForegroundColor Cyan " - Total Deploy Time: $(FormatDuration $deployDuration)"
    Write-Host -ForegroundColor Cyan " - Average Deploy Time per Photo: $(FormatDuration ($deployDuration / $filecount))"

    Write-Host -ForegroundColor Yellow " - Total Time: $(FormatDuration $totalTime)"
    Write-Host -ForegroundColor Yellow " - Average Time per Photo: $(FormatDuration ($totalTime / $filecount))"
}

function CorrectIntermediateFilenames {
    param(
        [Parameter(Mandatory = $true)] [hashtable] $categorySpec
    )

    # default pure raw filename will include the orig extension as "-NEF", remove that if present so the intermediate
    # file names (dng/pp3) are in line w/ the original filenames
    rename -- '-NEF' '' $(Join-Path $categorySpec.rootDir "*.dng")
    rename -- '-NEF' '' $(Join-Path $categorySpec.rootDir "*.pp3")
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
        [Parameter(Mandatory = $true)] [hashtable] $categorySpec
    )

    $exif = ReadExif -dir $categorySpec.srcDir
    $fs = ReadFilesystemInfo -dir $categorySpec.rootDir
    $dim = ReadImageDimensions -dir $categorySpec.rootDir

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

    return "(SELECT id FROM ${table} WHERE name = $(SqlString -val $value))";
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
    $writer.WriteLine("");
    $writer.WriteLine("\q");
}

function WriteSqlLookup {
    param(
        [Parameter(Mandatory = $true)] [System.IO.StreamWriter] $writer,
        [Parameter(Mandatory = $true)] [string] $table,
        [Parameter(Mandatory = $false)] [array] $values
    )

    foreach ($val in $values)
    {
        if($val) {
            $lookup = SqlCreateLookup -table $table -value $val

            if (-not [string]::IsNullOrWhiteSpace($lookup)) {
                $writer.WriteLine($lookup)
            }
        }
    }
}

function WriteSqlLookups {
    param(
        [Parameter(Mandatory = $true)] [System.IO.StreamWriter] $writer,
        [Parameter(Mandatory = $true)] [hashtable] $metadata
    )

    WriteSqlLookup -writer $writer -table "photo.compression" -values ($metadata.Values.exif?.Compression?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.contrast" -values ($metadata.Values.exif?.Contrast?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.exposure_mode" -values ($metadata.Values.exif?.ExposureMode?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.exposure_program" -values ($metadata.Values.exif?.ExposureProgram?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.flash" -values ($metadata.Values.exif?.Flash?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.gain_control" -values ($metadata.Values.exif?.GainControl?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.gps_altitude_ref" -values ($metadata.Values.exif?.GpsAltitudeRef?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.gps_direction_ref" -values ($metadata.Values.exif?.GpsImgDirectionRef?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.gps_latitude_ref" -values ($metadata.Values.exif?.GpsLatitudeRef?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.gps_longitude_ref" -values ($metadata.Values.exif?.GpsLongitudeRef?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.gps_measure_mode" -values ($metadata.Values.exif?.GpsMeasureMode?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.gps_status" -values ($metadata.Values.exif?.GpsStatus?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.light_source" -values ($metadata.Values.exif?.LightSource?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.make" -values ($metadata.Values.exif?.Make?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.metering_mode" -values ($metadata.Values.exif?.MeteringMode?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.model" -values ($metadata.Values.exif?.Model?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.orientation" -values ($metadata.Values.exif?.Orientation?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.saturation" -values ($metadata.Values.exif?.Saturation?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.scene_capture_type" -values ($metadata.Values.exif?.SceneCaptureType?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.scene_type" -values ($metadata.Values.exif?.SceneType?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.sensing_method" -values ($metadata.Values.exif?.SensingMethod?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.sharpness" -values ($metadata.Values.exif?.Sharpness?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.af_area_mode" -values ($metadata.Values.exif?.AFAreaMode?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.af_point" -values ($metadata.Values.exif?.AFPoint?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.active_d_lighting" -values ($metadata.Values.exif?.'ActiveD-Lighting'?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.colorspace" -values ($metadata.Values.exif?.Colorspace?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.flash_color_filter" -values ($metadata.Values.exif?.FlashColorFilter?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.flash_mode" -values ($metadata.Values.exif?.FlashMode?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.flash_setting" -values ($metadata.Values.exif?.FlashSetting?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.flash_type" -values ($metadata.Values.exif?.FlashType?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.focus_mode" -values ($metadata.Values.exif?.FocusMode?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.high_iso_noise_reduction" -values ($metadata.Values.exif?.HighIsoNoiseReduction?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.hue_adjustment" -values ($metadata.Values.exif?.HueAdjustment?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.noise_reduction" -values ($metadata.Values.exif?.NoiseReduction?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.picture_control_name" -values ($metadata.Values.exif?.PictureControlName?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.vibration_reduction" -values ($metadata.Values.exif?.VibrationReduction?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.vignette_control" -values ($metadata.Values.exif?.VignetteControl?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.vr_mode" -values ($metadata.Values.exif?.VRMode?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.white_balance" -values ($metadata.Values.exif?.WhiteBalance?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.auto_focus" -values ($metadata.Values.exif?.AutoFocus?.val | Select-Object -Unique)
    WriteSqlLookup -writer $writer -table "photo.lens" -values ($metadata.Values.exif?.LensId?.val | Select-Object -Unique)
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
            "sm_height" = $photo.sm.height
            "sm_width" = $photo.sm.width
            "sm_size" = $photo.sm.size
            "sm_path" = SqlString -val (BuildUrl -category $category -file $photo.sm.path)
            "md_height" = $photo.md.height
            "md_width" = $photo.md.width
            "md_size" = $photo.md.size
            "md_path" = SqlString -val (BuildUrl -category $category -file $photo.md.path)
            "lg_height" = $photo.lg.height
            "lg_width" = $photo.lg.width
            "lg_size" = $photo.lg.size
            "lg_path" = SqlString -val (BuildUrl -category $category -file $photo.lg.path)
            "prt_height" = $photo.lg.height
            "prt_width" = $photo.lg.width
            "prt_size" = 0 # seeing we are referencing lg, specify 0b so we don't double count what is actually on disk
            "prt_path" = SqlString -val (BuildUrl -category $category -file $photo.lg.path)  # just use lg for prt until we remove
            "src_height" = $photo.src.height
            "src_width" = $photo.src.width
            "src_size" = $photo.src.size
            "src_path" = SqlString -val (BuildUrl -category $category -file $photo.src.path)
            # exif
            "bits_per_sample" = SqlNumber -val ($photo.exif?.BitsPerSample?.num ?? $photo.exif?.BitsPerSample?.val)
            "compression_id" = SqlLookupId -table "photo.compression" -value $photo.exif?.Compression?.val
            "contrast_id" = SqlLookupId -table "photo.contrast" -value $photo.exif?.Contrast?.val
            "create_date" = SqlTimestamp -dt $photo.exif?.CreateDate?.val
            "digital_zoom_ratio" = SqlNumber -val ($photo.exif?.DigitalZoomRatio?.num ?? $photo.exif?.DigitalZoomRatio?.val)
            "exposure_compensation" = SqlString -val $photo.exif?.ExposureCompensation?.val
            "exposure_mode_id" = SqlLookupId -table "photo.exposure_mode" -value $photo.exif?.ExposureMode?.val
            "exposure_program_id" = SqlLookupId -table "photo.exposure_program" -value $photo.exif?.ExposureProgram?.val
            "exposure_time" = SqlString -val $photo.exif?.ExposureTime?.val
            "f_number" = SqlNumber -val ($photo.exif?.FNumber?.num ?? $photo.exif?.FNumber?.val)
            "flash_id" = SqlLookupId -table "photo.flash" -value $photo.exif?.Flash?.val
            "focal_length" = SqlNumber -val ($photo.exif?.FocalLength?.num ?? $photo.exif?.FocalLength?.val)
            "focal_length_in_35_mm_format" = SqlNumber -val ($photo.exif?.FocalLengthIn35mmFormat?.num ?? $photo.exif?.FocalLengthIn35mmFormat?.val)
            "gain_control_id" = SqlLookupId -table "photo.gain_control" -value $photo.exif?.GainControl?.val
            "gps_altitude" = SqlNumber -val ($photo.exif?.GpsAltitude?.num ?? $photo.exif?.GpsAltitude?.val)
            "gps_altitude_ref_id" = SqlLookupId -table "photo.gps_altitude_ref" -value $photo.exif?.GpsAltitudeRef?.val
            "gps_date_time_stamp" = SqlTimestamp -dt $photo.exif?.GpsDateStamp?.val
            "gps_direction" = SqlNumber -val ($photo.exif?.GpsImgDirection?.num ?? $photo.exif?.GpsImgDirection?.val)
            "gps_direction_ref_id" = SqlLookupId -table "photo.gps_direction_ref" -value $photo.exif?.GpsImgDirectionRef?.val
            "gps_latitude" = SqlNumber -val ($photo.exif?.GpsLatitude?.num ?? $photo.exif?.GpsLatitude?.val)
            "gps_latitude_ref_id" = SqlLookupId -table "photo.gps_latitude_ref" -value $photo.exif?.GpsLatitudeRef?.val
            "gps_longitude" = SqlNumber -val ($photo.exif?.GpsLongitude?.num ?? $photo.exif?.GpsLongitude?.val)
            "gps_longitude_ref_id" = SqlLookupId -table "photo.gps_longitude_ref" -value $photo.exif?.GpsLongitudeRef?.val
            "gps_measure_mode_id" = SqlLookupId -table "photo.gps_measure_mode" -value $photo.exif?.GpsMeasureMode?.val
            "gps_satellites" = SqlString -val $photo.exif?.GpsSatellites?.val
            "gps_status_id" = SqlLookupId -table "photo.gps_status" -value $photo.exif?.GpsStatus?.val
            "gps_version_id" = SqlString -val $photo.exif?.GpsVersionId?.val
            "iso" = SqlNumber -val ($photo.exif?.Iso?.num ?? $photo.exif?.Iso?.val)
            "light_source_id" = SqlLookupId -table "photo.light_source" -value $photo.exif?.LightSource?.val
            "make_id" = SqlLookupId -table "photo.make" -value $photo.exif?.Make?.val
            "metering_mode_id" = SqlLookupId -table "photo.metering_mode" -value $photo.exif?.MeteringMode?.val
            "model_id" = SqlLookupId -table "photo.model" -value $photo.exif?.Model?.val
            "orientation_id" = SqlLookupId -table "photo.orientation" -value $photo.exif?.Orientation?.val
            "saturation_id" = SqlLookupId -table "photo.saturation" -value $photo.exif?.Saturation?.val
            "scene_capture_type_id" = SqlLookupId -table "photo.scene_capture_type" -value $photo.exif?.SceneCaptureType?.val
            "scene_type_id" = SqlLookupId -table "photo.scene_type" -value $photo.exif?.SceneType?.val
            "sensing_method_id" = SqlLookupId -table "photo.sensing_method" -value $photo.exif?.SensingMethod?.val
            "sharpness_id" = SqlLookupId -table "photo.sharpness" -value $photo.exif?.Sharpness?.val
            # nikon
            "af_area_mode_id" = SqlLookupId -table "photo.af_area_mode" -value $photo.exif?.AFAreaMode?.val
            "af_point_id" = SqlLookupId -table "photo.af_point" -value $photo.exif?.AFPoint?.val
            "active_d_lighting_id" = SqlLookupId -table "photo.active_d_lighting" -value $photo.exif?.'ActiveD-Lighting'?.val
            "colorspace_id" = SqlLookupId -table "photo.colorspace" -value $photo.exif?.Colorspace?.val
            "exposure_difference" = SqlNumber -val ($photo.exif?.ExposureDifference?.num ?? $photo.exif?.ExposureDifference?.val)
            "flash_color_filter_id" = SqlLookupId -table "photo.flash_color_filter" -value $photo.exif?.FlashColorFilter?.val
            "flash_compensation" = SqlString -val $photo.exif?.FlashCompensation?.val
            "flash_control_mode" = SqlNumber -val ($photo.exif?.FlashControlMode?.num ?? $photo.exif?.FlashControlMode?.val)
            "flash_exposure_compensation" = SqlString -val $photo.exif?.FlashExposureComp?.val
            "flash_focal_length" = SqlNumber -val ($photo.exif?.FlashFocalLength?.num ?? $photo.exif?.FlashFocalLength?.val)
            "flash_mode_id" = SqlLookupId -table "photo.flash_mode" -value $photo.exif?.FlashMode?.val
            "flash_setting_id" = SqlLookupId -table "photo.flash_setting" -value $photo.exif?.FlashSetting?.val
            "flash_type_id" = SqlLookupId -table "photo.flash_type" -value $photo.exif?.FlashType?.val
            "focus_distance" = SqlNumber -val ($photo.exif?.FocusDistance?.num ?? $photo.exif?.FocusDistance?.val)
            "focus_mode_id" = SqlLookupId -table "photo.focus_mode" -value $photo.exif?.FocusMode?.val
            "focus_position" = SqlNumber -val ($photo.exif?.FocusPosition?.num ?? $photo.exif?.FocusPosition?.val)
            "high_iso_noise_reduction_id" = SqlLookupId -table "photo.high_iso_noise_reduction" -value $photo.exif?.HighIsoNoiseReduction?.val
            "hue_adjustment_id" = SqlLookupId -table "photo.hue_adjustment" -value $photo.exif?.HueAdjustment?.val
            "noise_reduction_id" = SqlLookupId -table "photo.noise_reduction" -value $photo.exif?.NoiseReduction?.val
            "picture_control_name_id" = SqlLookupId -table "photo.picture_control_name" -value $photo.exif?.PictureControlName?.val
            "primary_af_point" = SqlString -val $photo.exif?.PrimaryAFPoint?.val
            "vibration_reduction_id" = SqlLookupId -table "photo.vibration_reduction" -value $photo.exif?.VibrationReduction?.val
            "vignette_control_id" = SqlLookupId -table "photo.vignette_control" -value $photo.exif?.VignetteControl?.val
            "vr_mode_id" = SqlLookupId -table "photo.vr_mode" -value $photo.exif?.VRMode?.val
            "white_balance_id" = SqlLookupId -table "photo.white_balance" -value $photo.exif?.WhiteBalance?.val
            # composite
            "aperture" = SqlNumber -val ($photo.exif?.Aperture?.num ?? $photo.exif?.Aperture?.val)
            "auto_focus_id" = SqlLookupId -table "photo.auto_focus" -value $photo.exif?.AutoFocus?.val
            "depth_of_field" = SqlString -val $photo.exif?.DOF?.val
            "field_of_view" = SqlString -val $photo.exif?.FOV?.val
            "hyperfocal_distance" = SqlNumber -val ($photo.exif?.HyperfocalDistance?.num ?? $photo.exif?.HyperfocalDistance?.val)
            "lens_id" = SqlLookupId -table "photo.lens" -value $photo.exif?.LensId?.val
            "light_value" = SqlNumber -val ($photo.exif?.LightValue?.num ?? $photo.exif?.LightValue?.val)
            "scale_factor_35_efl" = SqlNumber -val ($photo.exif?.ScaleFactor35Efl?.num ?? $photo.exif?.ScaleFactor35Efl?.val)
            "shutter_speed" = SqlString -val $photo.exif?.ShutterSpeed?.val
        }

        $colNames = @()
        $colValues = @()

        # hashtables make no guarantees about ordering, so build lists to enforce this
        foreach($key in $items.Keys) {
            $colNames += $key
            $colValues += $items[$key]   # "$($items[$key])    -- $key"
        }

        $writer.WriteLine(@"
INSERT INTO photo.photo
(
    $([string]::Join("`n    , ", $colNames))
)
VALUES
(
    $([string]::Join("`n    , ", $colValues))
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
    $(SqlString -val $categorySpec.name),
    $($categorySpec.year),
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

    foreach ($role in $categorySpec.allowedRoles) {
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

function StartDevPod {
    param(
        [Parameter(Mandatory = $true)] [hashtable] $cfg
    )

    systemctl --user start "$($cfg.devPodSystemdService)"

    # give some time for services to start
    sleep 3s
}

function BuildConfig {
    $cfg = @{
        sshRemoteHost = "tifa"
        sshUsername = "svc_www_maw"
        dirAssetRoot = "/srv/www/website_assets"
        postgresImage = "docker.io/postgres:16-alpine"
        awsProfile = "mawpower"
        dev = @{
            pod = "dev-maw-pod"
            postgresEnvFile = "/home/mmorano/maw_dev/podman-env/maw-postgres.env"
        }
        prod = @{
            pod = "prod-maw-pod"
            postgresEnvFile = "/home/svc_www_maw/maw_prod/podman-env/maw-postgres.env"
        }
    }

    $cfg.devPodSystemdService = "pod-$($cfg.dev.pod)"

    return $cfg
}

function PromptPath {
    while($true) {
        $path = Read-Host -Prompt "Enter path to photos"

        if(Test-Path $path) {
            return Get-Item $path
        } else {
            Write-Host -ForegroundColor Red "Path [$path] does not exist!"
        }
    }
}

function PromptYear {
    while($true) {
        $year = -1
        $yrStr = Read-Host -Prompt "Enter year"

        if([int]::TryParse($yrStr, [ref]$year)) {
            return $year
        } else {
            Write-Host "Please enter the year as a number"
        }
    }
}

function PromptString {
    param(
        [Parameter(Mandatory = $true)] [string] $prompt,
        [Parameter(Mandatory = $false)] [string] $def = $null
    )

    while($true) {
        $val = Read-Host -Prompt $prompt

        if($val) {
            return $val
        } elseif($def) {
            return $def
        }
    }
}

function GetCategoryDetails {
    param(
        [Parameter(Mandatory = $true)] [string] $photoDir,
        [Parameter(Mandatory = $true)] [hashtable] $cfg
    )

    if(Test-Path $photoDir) {
        $dir = Get-Item $photoDir
    } else {
        Write-Host -ForegroundColor Red "Path [$photoDir] does not exist!"
        Exit
    }

    #$dir = GetDir
    $year = PromptYear
    $name = PromptString -prompt "Enter category name"
    $roles = (PromptString -def "friend admin" -prompt "Enter role names (space delimited).  Defaults to: [admin friend]").Split(" ")
    $sqlFile = Join-Path $dir.FullName "photocategory.sql"
    $sizeSpecs = BuildSizeSpecs -dir $dir.FullName

    $spec = @{
        rootDir = $dir.FullName
        year = $year
        name = $name
        allowedRoles = $roles
        sqlFile = $sqlFile
        sizeSpecs = $sizeSpecs

        deploySpec = @{
            yearDir = Join-Path $cfg.dirAssetRoot "images" $year
            destDir = Join-Path $cfg.dirAssetRoot "images" $year $dir.Name
            srcDir = Join-Path $cfg.dirAssetRoot "images" $year $dir.Name "src"
            awsBackupPath = "s3://mikeandwan-us-photos/${year}/$($dir.Name)"
        }
    }

    $spec.srcDir = Join-Path $spec.rootDir "src"

    return $spec
}

function VerifyDirectoryNotUsed {
    param(
        [Parameter(Mandatory = $true)] [hashtable] $categorySpec
    )

    if(Test-Path -PathType Container $categorySpec.deploySpec.destDir) {
        Write-Host -ForegroundColor Red ''
        Write-Host -ForegroundColor Red '****'
        Write-Host -ForegroundColor Red '  This directory already exists in the destination.'
        Write-Host -ForegroundColor Red '  If you are trying to add images to a previously published directory, you must do this manually.'
        Write-Host -ForegroundColor Red '  Otherwise, please give the directory a unique name and then try again.'
        Write-Host -ForegroundColor Red '****'

        Exit
    }
}

function ProcessPhotos {
    param(
        [Parameter(Mandatory = $true)] [hashtable] $categorySpec
    )

    $timer = [Diagnostics.Stopwatch]::StartNew()

    # new process assumes all RAW files will be converted to dng first (via DxO PureRaw 4)
    # category's src dir will contain original NEF and pp3s at end of process, though may delete dngs
    CleanPriorAttempts -categorySpec $categorySpec
    CorrectIntermediateFilenames -categorySpec $categorySpec

    # given how we moved the rt args, this should not be needed
    # CleanSidecarPp3Files -categorySpec $categorySpec

    MoveSourceFilesWithDng -categorySpec $categorySpec
    ResizePhotos -categorySpec $categorySpec
    MoveSourceFiles -categorySpec $categorySpec
    $metadata = GetMetadata -categorySpec $categorySpec
    WriteSql -categorySpec $categorySpec -metadata $metadata

    $timer.Stop()
    return $timer.Elapsed
}

function BuildDeployScript {
    param(
        [Parameter(Mandatory = $true)] [hashtable] $categorySpec,
        [Parameter(Mandatory = $true)] [hashtable] $config
    )

    return @"
#!/bin/bash
ssh -t "$($config.sshUsername)@$($config.sshRemoteHost)" "
    echo \"These commands will be run on: \`$( uname -n )\"

    if [ ! -d '$($categorySpec.deploySpec.yearDir)' ]; then
        sudo mkdir '$($categorySpec.deploySpec.yearDir)'
    fi

    sudo mv '$([System.IO.Path]::GetFilename($categorySpec.deploySpec.destDir))' '$($categorySpec.deploySpec.yearDir)'
    sudo chown -R root:root '$($categorySpec.deploySpec.destDir)'
    sudo chmod -R go-w '$($categorySpec.deploySpec.destDir)'
    sudo restorecon -R '$($categorySpec.deploySpec.destDir)'

    podman run -it --rm \
        --pod '$($config.prod.pod)' \
        --env-file '$($config.prod.postgresEnvFile)' \
        --volume $($categorySpec.deploySpec.destDir):/sql:ro \
        --security-opt label=disable \
        $($config.postgresImage) \
            psql \
                -h localhost \
                -U postgres \
                -d maw_website \
                -f '/sql/$([System.IO.Path]::GetFilename($categorySpec.sqlFile))'

    sudo rm '$( Join-Path $categorySpec.deploySpec.destDir ([System.IO.Path]::GetFilename($categorySpec.sqlFile)) )'
"
"@
}

function ExecuteDeployOnRemote {
    param(
        [Parameter(Mandatory = $true)] [hashtable] $categorySpec,
        [Parameter(Mandatory = $true)] [hashtable] $config
    )

    try {
        $file = New-TemporaryFile
        rm $file.FullName
        $file = "$($file.FullName).sh"

        $script = BuildDeployScript `
            -categorySpec $categorySpec `
            -config $config

        #Write-Host $script
        $script > $file
        chmod u+x $file

        # hmm, cant figure out how to run the script remotely, so let's tell the user to do it manually YUCK!
        Write-Host -ForegroundColor Yellow "Please run the following to complete the deploy: .$file"
    } finally {
        #rm $file
    }
}

function Deploy {
    param(
        [Parameter(Mandatory = $true)] [hashtable] $categorySpec,
        [Parameter(Mandatory = $true)] [hashtable] $config
    )

    Write-Host -ForegroundColor Blue "Deploying..."

    $timer = [Diagnostics.Stopwatch]::StartNew()

    # -- local deploly
    StartDevPod -cfg $config
    ZipPp3s -categorySpec $categorySpec
    MoveProcessedImagesToLocalDestination -categorySpec $categorySpec -config $config
    ApplySqlToLocalDatabase -categorySpec $categorySpec -config $config

    # -- remote deploy
    CopyImagesAndSqlToRemoteDestination -categorySpec $categorySpec -config $config
    ExecuteDeployOnRemote -categorySpec $categorySpec -config $config

    $timer.Stop()
    return $timer.Elapsed
}

function Backup {
    param(
        [Parameter(Mandatory = $true)] [hashtable] $categorySpec,
        [Parameter(Mandatory = $true)] [hashtable] $config
    )

    Write-Host -ForegroundColor Blue "Executing Backup..."

    $timer = [Diagnostics.Stopwatch]::StartNew()

    # check to see if we currently logged in to aws
    aws sts get-caller-identity --profile "$($config.awsProfile)" 2>&1 | out-null

    if(-not $?) {
        Write-Host -ForegroundColor Cyan "You will be prompted to authenticate with AWS to backup files to S3 Glacier"
        Write-Host -ForegroundColor Yellow "Note: this is also buggy from pwsh, it is not printing the verification code"
        aws sso login --profile "$($config.awsProfile)"
    }

    aws s3 sync `
        "$($categorySpec.deploySpec.srcDir)" "$($categorySpec.deploySpec.awsBackupPath)" `
        --storage-class "DEEP_ARCHIVE" `
        --profile "$($config.awsProfile)" `
        --quiet

    $timer.Stop()
    return $timer.Elapsed
}

function Main {
    param(
        [Parameter(Mandatory = $false)] [string] $photoDir = $null
    )

    $config = BuildConfig
    $categorySpec = GetCategoryDetails -photoDir $photoDir -cfg $config

    VerifyDirectoryNotUsed -categorySpec $categorySpec

    $resizeDuration = ProcessPhotos -categorySpec $categorySpec

    $doContinue = Read-Host "Would you like to backup and deploy at this time? [y|N]"

    if($doContinue -ne "y") {
        Exit
    }

    # note: we no longer get aws hashtree ids from storing in s3 glacier deep archive
    #       so we might as well push sooner than later so we can verify the images on
    #       the site while the backup runs
    $deployDuration = Deploy -categorySpec $categorySpec -config $config
    $backupDuration = Backup -categorySpec $categorySpec -config $config

    # DeleteDngFiles -dir $dir

    PrintStats `
        -sizeSpecs $categorySpec.sizeSpecs `
        -resizeDuration $resizeDuration `
        -backupDuration $backupDuration `
        -deployDuration $deployDuration

    Write-Host("")
    Write-Host -ForegroundColor Yellow "Completed!"
}

Main -photoDir $photoDir