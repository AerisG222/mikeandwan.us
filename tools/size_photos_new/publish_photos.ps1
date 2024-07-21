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
        [Parameter(Mandatory = $true)] [obect] $val
    )

    if ($null -eq $val || [string]::IsNullOrWhiteSpace($val))
    {
        return "NULL";
    }

    $val = "${val}".Replace("'", "''");

    return "'${val}'";
}

function SqlTimestamp {
    param(
        [Parameter(Mandatory = $true)] [DateTime] $dt
    )

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
    IF NOT EXISTS (SELECT 1 FROM ${table} WHERE name = ${val}) THEN")
           INSERT INTO ${table} $(name) VALUES (${val});")
    END IF;
"@
}

function SqlLookupId {
    param(
        [Parameter(Mandatory = $true)] [string] $table,
        [Parameter(Mandatory = $true)] [string] $value
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

    writer.WriteLine("DO");
    writer.WriteLine("$$");
    writer.WriteLine("BEGIN");
    writer.WriteLine("");
}

function WriteSqlFooter {
    param(
        [Parameter(Mandatory = $true)] [System.IO.StreamWriter] $writer
    )

    writer.WriteLine("END");
    writer.WriteLine("$$");
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

    WriteSqlLookup -writer $writer -table "photo.active_d_lighting" -values (metadata.Select(x => x.ExifData.ActiveDLighting).Distinct())
    WriteSqlLookup -writer $writer -table "photo.af_area_mode" -values (metadata.Select(x => x.ExifData.AutoFocusAreaMode).Distinct())
    WriteSqlLookup -writer $writer -table "photo.af_point" -values (metadata.Select(x => x.ExifData.AutoFocusPoint).Distinct())
    WriteSqlLookup -writer $writer -table "photo.colorspace" -values (metadata.Select(x => x.ExifData.Colorspace).Distinct())
    WriteSqlLookup -writer $writer -table "photo.flash_color_filter" -values (metadata.Select(x => x.ExifData.FlashColorFilter).Distinct())
    WriteSqlLookup -writer $writer -table "photo.flash_mode" -values (metadata.Select(x => x.ExifData.FlashMode).Distinct())
    WriteSqlLookup -writer $writer -table "photo.flash_setting" -values (metadata.Select(x => x.ExifData.FlashSetting).Distinct())
    WriteSqlLookup -writer $writer -table "photo.flash_type" -values (metadata.Select(x => x.ExifData.FlashType).Distinct())
    WriteSqlLookup -writer $writer -table "photo.focus_mode" -values (metadata.Select(x => x.ExifData.FocusMode).Distinct())
    WriteSqlLookup -writer $writer -table "photo.high_iso_noise_reduction" -values (metadata.Select(x => x.ExifData.HighIsoNoiseReduction).Distinct())
    WriteSqlLookup -writer $writer -table "photo.hue_adjustment" -values (metadata.Select(x => x.ExifData.HueAdjustment).Distinct())
    WriteSqlLookup -writer $writer -table "photo.lens" -values (metadata.Select(x => x.ExifData.LensId).Distinct())
    WriteSqlLookup -writer $writer -table "photo.make" -values (metadata.Select(x => x.ExifData.Make).Distinct())
    WriteSqlLookup -writer $writer -table "photo.model" -values (metadata.Select(x => x.ExifData.Model).Distinct())
    WriteSqlLookup -writer $writer -table "photo.noise_reduction" -values (metadata.Select(x => x.ExifData.NoiseReduction).Distinct())
    WriteSqlLookup -writer $writer -table "photo.picture_control_name" -values (metadata.Select(x => x.ExifData.PictureControlName).Distinct())
    WriteSqlLookup -writer $writer -table "photo.saturation" -values (metadata.Select(x => x.ExifData.Saturation).Distinct())
    WriteSqlLookup -writer $writer -table "photo.vibration_reduction" -values (metadata.Select(x => x.ExifData.VibrationReduction).Distinct())
    WriteSqlLookup -writer $writer -table "photo.vignette_control" -values (metadata.Select(x => x.ExifData.VignetteControl).Distinct())
    WriteSqlLookup -writer $writer -table "photo.vr_mode" -values (metadata.Select(x => x.ExifData.VRMode).Distinct())
    WriteSqlLookup -writer $writer -table "photo.white_balance" -values (metadata.Select(x => x.ExifData.WhiteBalance).Distinct())
    WriteSqlLookup -writer $writer -table "photo.gps_altitude_ref" -values (metadata.Select(x => x.ExifData.GpsAltitudeRef).Distinct())
}

function WriteSqlResult {
    param(
        [Parameter(Mandatory = $true)] [System.IO.StreamWriter] $writer,
        [Parameter(Mandatory = $true)] [hashtable] $category,
        [Parameter(Mandatory = $true)] [hashtable] $metadata
    )

    foreach ($photo in $metadata) {
        $values = @(
            "(SELECT currval('photo.category_id_seq'))"
            # scaled images
            photo.Xs.Height.ToString()
            photo.Xs.Width.ToString()
            photo.Xs.SizeInBytes.ToString()
            SqlHelper.SqlString(BuildUrl(category, photo.Xs.OutputFile))
            photo.XsSq.Height.ToString()
            photo.XsSq.Width.ToString()
            photo.XsSq.SizeInBytes.ToString()
            SqlHelper.SqlString(BuildUrl(category, photo.XsSq.OutputFile))
            photo.Sm.Height.ToString()
            photo.Sm.Width.ToString()
            photo.Sm.SizeInBytes.ToString()
            SqlHelper.SqlString(BuildUrl(category, photo.Sm.OutputFile))
            photo.Md.Height.ToString()
            photo.Md.Width.ToString()
            photo.Md.SizeInBytes.ToString()
            SqlHelper.SqlString(BuildUrl(category, photo.Md.OutputFile))
            photo.Lg.Height.ToString()
            photo.Lg.Width.ToString()
            photo.Lg.SizeInBytes.ToString()
            SqlHelper.SqlString(BuildUrl(category, photo.Lg.OutputFile))
            photo.Prt.Height.ToString()
            photo.Prt.Width.ToString()
            photo.Prt.SizeInBytes.ToString()
            SqlHelper.SqlString(BuildUrl(category, photo.Prt.OutputFile))
            photo.Src.Height.ToString()
            photo.Src.Width.ToString()
            photo.Src.SizeInBytes.ToString()
            SqlHelper.SqlString(BuildUrl(category, photo.Src.OutputFile))
            # exif
            SqlHelper.SqlNumber(photo.ExifData?.BitsPerSample)
            SqlHelper.SqlNumber(photo.ExifData?.Compression)
            SqlHelper.SqlString(photo.ExifData?.Contrast)
            SqlHelper.SqlTimestamp(photo.ExifData?.CreateDate)
            SqlHelper.SqlNumber(photo.ExifData?.DigitalZoomRatio)
            SqlHelper.SqlString(photo.ExifData?.ExposureCompensation)
            SqlHelper.SqlNumber(photo.ExifData?.ExposureMode)
            SqlHelper.SqlNumber(photo.ExifData?.ExposureProgram)
            SqlHelper.SqlString(photo.ExifData?.ExposureTime)
            SqlHelper.SqlNumber(photo.ExifData?.FNumber)
            SqlHelper.SqlNumber(photo.ExifData?.Flash)
            SqlHelper.SqlNumber(photo.ExifData?.FocalLength)
            SqlHelper.SqlNumber(photo.ExifData?.FocalLengthIn35mmFormat)
            SqlHelper.SqlNumber(photo.ExifData?.GainControl)
            SqlHelper.SqlNumber(photo.ExifData?.GpsAltitude)
            SqlHelper.SqlLookupId("photo.gps_altitude_ref", photo.ExifData?.GpsAltitudeRef)
            SqlHelper.SqlTimestamp(photo.ExifData?.GpsDateStamp)
            SqlHelper.SqlNumber(photo.ExifData?.GpsDirection)
            SqlHelper.SqlString(photo.ExifData?.GpsDirectionRef)
            SqlHelper.SqlNumber(photo.ExifData?.GpsLatitude)
            SqlHelper.SqlString(photo.ExifData?.GpsLatitudeRef)
            SqlHelper.SqlNumber(photo.ExifData?.GpsLongitude)
            SqlHelper.SqlString(photo.ExifData?.GpsLongitudeRef)
            SqlHelper.SqlString(photo.ExifData?.GpsMeasureMode)
            SqlHelper.SqlString(photo.ExifData?.GpsSatellites)
            SqlHelper.SqlString(photo.ExifData?.GpsStatus)
            SqlHelper.SqlString(photo.ExifData?.GpsVersionId)
            SqlHelper.SqlNumber(photo.ExifData?.Iso)
            SqlHelper.SqlNumber(photo.ExifData?.LightSource)
            SqlHelper.SqlLookupId("photo.make", photo.ExifData?.Make)
            SqlHelper.SqlNumber(photo.ExifData?.MeteringMode)
            SqlHelper.SqlLookupId("photo.model", photo.ExifData?.Model)
            SqlHelper.SqlNumber(photo.ExifData?.Orientation)
            SqlHelper.SqlLookupId("photo.saturation", photo.ExifData?.Saturation)
            SqlHelper.SqlNumber(photo.ExifData?.SceneCaptureType)
            SqlHelper.SqlNumber(photo.ExifData?.SceneType)
            SqlHelper.SqlNumber(photo.ExifData?.SensingMethod)
            SqlHelper.SqlNumber(photo.ExifData?.Sharpness)
            # nikon
            SqlHelper.SqlLookupId("photo.af_area_mode", photo.ExifData?.AutoFocusAreaMode)
            SqlHelper.SqlLookupId("photo.af_point", photo.ExifData?.AutoFocusPoint)
            SqlHelper.SqlLookupId("photo.active_d_lighting", photo.ExifData?.ActiveDLighting)
            SqlHelper.SqlLookupId("photo.colorspace", photo.ExifData?.Colorspace)
            SqlHelper.SqlNumber(photo.ExifData?.ExposureDifference)
            SqlHelper.SqlLookupId("photo.flash_color_filter", photo.ExifData?.FlashColorFilter)
            SqlHelper.SqlString(photo.ExifData?.FlashCompensation)
            SqlHelper.SqlNumber(photo.ExifData?.FlashControlMode)
            SqlHelper.SqlString(photo.ExifData?.FlashExposureCompensation)
            SqlHelper.SqlNumber(photo.ExifData?.FlashFocalLength)
            SqlHelper.SqlLookupId("photo.flash_mode", photo.ExifData?.FlashMode)
            SqlHelper.SqlLookupId("photo.flash_setting", photo.ExifData?.FlashSetting)
            SqlHelper.SqlLookupId("photo.flash_type", photo.ExifData?.FlashType)
            SqlHelper.SqlNumber(photo.ExifData?.FocusDistance)
            SqlHelper.SqlLookupId("photo.focus_mode", photo.ExifData?.FocusMode)
            SqlHelper.SqlNumber(photo.ExifData?.FocusPosition)
            SqlHelper.SqlLookupId("photo.high_iso_noise_reduction", photo.ExifData?.HighIsoNoiseReduction)
            SqlHelper.SqlLookupId("photo.hue_adjustment", photo.ExifData?.HueAdjustment)
            SqlHelper.SqlLookupId("photo.noise_reduction", photo.ExifData?.NoiseReduction)
            SqlHelper.SqlLookupId("photo.picture_control_name", photo.ExifData?.PictureControlName)
            SqlHelper.SqlString(photo.ExifData?.PrimaryAFPoint)
            SqlHelper.SqlLookupId("photo.vibration_reduction", photo.ExifData?.VibrationReduction)
            SqlHelper.SqlLookupId("photo.vignette_control", photo.ExifData?.VignetteControl)
            SqlHelper.SqlLookupId("photo.vr_mode", photo.ExifData?.VRMode)
            SqlHelper.SqlLookupId("photo.white_balance", photo.ExifData?.WhiteBalance)
            # composite
            SqlHelper.SqlNumber(photo.ExifData?.Aperture)
            SqlHelper.SqlNumber(photo.ExifData?.AutoFocus)
            SqlHelper.SqlString(photo.ExifData?.DepthOfField)
            SqlHelper.SqlString(photo.ExifData?.FieldOfView)
            SqlHelper.SqlNumber(photo.ExifData?.HyperfocalDistance)
            SqlHelper.SqlLookupId("photo.lens", photo.ExifData?.LensId)
            SqlHelper.SqlNumber(photo.ExifData?.LightValue)
            SqlHelper.SqlNumber(photo.ExifData?.ScaleFactor35Efl)
            SqlHelper.SqlString(photo.ExifData?.ShutterSpeed)
        )

        $writer.WriteLine("INSERT INTO photo.photo ($([string]::Join(", ", _cols)}) VALUES ($([string]::Join(", ", values)});
    }

    $writer.WriteLine()
}

function WriteSqlCategoryCreate {
    param(
        [Parameter(Mandatory = $true)] [System.IO.StreamWriter] $writer,
        [Parameter(Mandatory = $true)] [hashtable] $category,
        [Parameter(Mandatory = $true)] [hashtable] $metadata
    )

    var photo = photos.First();

    writer.WriteLine(
        $"INSERT INTO photo.category (name, year, teaser_photo_width, teaser_photo_height, teaser_photo_size, teaser_photo_path, teaser_photo_sq_width, teaser_photo_sq_height, teaser_photo_sq_size, teaser_photo_sq_path) " +
        $"  VALUES (" +
        $"    {SqlHelper.SqlString(category.Name)}, " +
        $"    {category.Year}, " +
        $"    {photo.Xs.Width}, " +
        $"    {photo.Xs.Height}, " +
        $"    {photo.Xs.SizeInBytes}, " +
        $"    {SqlHelper.SqlString(BuildUrl(category, photo.Xs.OutputFile))}, " +
        $"    {photo.XsSq.Width}, " +
        $"    {photo.XsSq.Height}, " +
        $"    {photo.XsSq.SizeInBytes}, " +
        $"    {SqlHelper.SqlString(BuildUrl(category, photo.XsSq.OutputFile))});");

    foreach (var role in category.AllowedRoles)
    {
        writer.WriteLine(
            $"INSERT INTO photo.category_role (category_id, role_id)" +
            $"  VALUES (" +
            $"    (SELECT currval('photo.category_id_seq'))," +
            $"    (SELECT id FROM maw.role WHERE name = {SqlHelper.SqlString(role)})" +
            $"  );"
        );
    }

    writer.WriteLine();
}

function WriteCategoryUpdate(StreamWriter writer)
{
    writer.WriteLine(
        "UPDATE photo.category c " +
        "   SET photo_count = (SELECT COUNT(1) FROM photo.photo WHERE category_id = c.id), " +
        "       create_date = (SELECT create_date FROM photo.photo WHERE id = (SELECT MIN(id) FROM photo.photo where category_id = c.id AND create_date IS NOT NULL)), " +
        "       gps_latitude = (SELECT gps_latitude FROM photo.photo WHERE id = (SELECT MIN(id) FROM photo.photo WHERE category_id = c.id AND gps_latitude IS NOT NULL)), " +
        "       gps_latitude_ref_id = (SELECT gps_latitude_ref_id FROM photo.photo WHERE id = (SELECT MIN(id) FROM photo.photo WHERE category_id = c.id AND gps_latitude IS NOT NULL)), " +
        "       gps_longitude = (SELECT gps_longitude FROM photo.photo WHERE id = (SELECT MIN(id) FROM photo.photo WHERE category_id = c.id AND gps_latitude IS NOT NULL)), " +
        "       gps_longitude_ref_id = (SELECT gps_longitude_ref_id FROM photo.photo WHERE id = (SELECT MIN(id) FROM photo.photo WHERE category_id = c.id AND gps_latitude IS NOT NULL)), " +
        "       total_size_xs = (SELECT SUM(xs_size) FROM photo.photo WHERE category_id = c.id), " +
        "       total_size_xs_sq = (SELECT SUM(xs_sq_size) FROM photo.photo WHERE category_id = c.id), " +
        "       total_size_sm = (SELECT SUM(sm_size) FROM photo.photo WHERE category_id = c.id), " +
        "       total_size_md = (SELECT SUM(md_size) FROM photo.photo WHERE category_id = c.id), " +
        "       total_size_lg = (SELECT SUM(lg_size) FROM photo.photo WHERE category_id = c.id), " +
        "       total_size_prt = (SELECT SUM(prt_size) FROM photo.photo WHERE category_id = c.id), " +
        "       total_size_src = (SELECT SUM(src_size) FROM photo.photo WHERE category_id = c.id), " +
        "       teaser_photo_size = (SELECT xs_size FROM photo.photo WHERE category_id = c.id AND xs_path = c.teaser_photo_path), " +
        "       teaser_photo_sq_height = (SELECT xs_sq_height FROM photo.photo WHERE category_id = c.id AND xs_path = c.teaser_photo_path), " +
        "       teaser_photo_sq_width = (SELECT xs_sq_width FROM photo.photo WHERE category_id = c.id AND xs_path = c.teaser_photo_path), " +
        "       teaser_photo_sq_path = (SELECT xs_sq_path FROM photo.photo WHERE category_id = c.id AND xs_path = c.teaser_photo_path), " +
        "       teaser_photo_sq_size = (SELECT xs_sq_size FROM photo.photo WHERE category_id = c.id AND xs_path = c.teaser_photo_path) " +
        " WHERE c.id = (SELECT currval('photo.category_id_seq'));"
    );

    writer.WriteLine();
}

function BuildUrl(CategoryInfo category, string file)
    {
        var rootParts = _opts.WebPhotoRoot.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var root = $"/{string.Join('/', rootParts)}";
        var fileParts = file.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);

        if(fileParts.Length < 3)
        {
            throw new InvalidOperationException("should have at least 3 components of the image name!");
        }

        var filename = fileParts[^1];
        var sizeDir = fileParts[^2];
        var categoryDir = fileParts[^3];

        return $"{root}/{category.Year}/{categoryDir}/{sizeDir}/{filename}";
    }

function WriteSql {
    param(
        [Parameter(Mandatory = $true)] [string] $dir,
        [Parameter(Mandatory = $true)] [int] $year,
        [Parameter(Mandatory = $true)] [string] $category,
        [Parameter(Mandatory = $true)] [string] $webImgRoot,
        [Parameter(Mandatory = $true)] [array`] $roles,
        [Parameter(Mandatory = $true)] [hashtable] $metadata,
        [Parameter(Mandatory = $true)] [string] $sqlFile
    )

    $writer = [System.IO.StreamWriter]::new($sqlFile)

    WriteSqlHeader -writer $writer
    WriteSqlCategoryCreate -writer $writer -category $category -metadata $metadata
    WriteSqlLookups -writer $writer -metadata $metadata
    WriteSqlResult -writer $writer -category $category -metadata $metadata
    WriteSqlCategoryUpdate -writer $writer
    WriteSqlFooter -writer $writer

    $writer.Flush()
    $writer.Close()
}

function Main {
    $dir = "/home/mmorano/Desktop/testing"
    $year = 2024
    $category = "Testing"
    $webImgRoot = "/images/${year}/"
    $roles = @( "friend", "admin" )
    $sqlFile = (Join-Path $dir "photocategory.sql")
    $sizeSpecs = BuildSizeSpecs -dir $dir
    $timer = [Diagnostics.Stopwatch]::StartNew()

    # new process assumes all RAW files will be converted to dng first (via DxO PureRaw 4)
    # MoveSourceFiles -dir $dir -pattern "*.NEF"
    # CorrectIntermediateFilenames -dir $dir
    # CleanSidecarPp3Files -dir $dir
    # ResizePhotos -dir $dir -sizeSpecs $sizeSpecs
    # MoveSourceFiles -dir $dir -pattern "*.jpg"
    # MoveSourceFiles -dir $dir -pattern "*.pp3"
    $metadata = GetMetadata -dir $dir

    WriteSql `
        -dir $dir `
        -year $year `
        -category $category `
        -webImgRoot $webImgRoot `
        -roles $roles `
        -metadata $metadata `
        -output $sqlFile

    $timer.Stop()

    # DeleteDngFiles -dir $dir
    # PrintStats -sizeSpecs $sizeSpecs -timer $timer
}

Main
