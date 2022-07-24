using StackExchange.Redis;
using Maw.Domain.Models.Photos;

namespace Maw.Cache.Photos;

class PhotoDetailSerializer
    : BaseSerializer<Detail>
{
    const string KEY_BITS_PER_SAMPLE = "bits-per-sample";
    const string KEY_COMPRESSION = "compression";
    const string KEY_CONTRAST = "contrast";
    const string KEY_CREATE_DATE = "create-date";
    const string KEY_DIGITAL_ZOOM_RATIO = "digital-zoom-ratio";
    const string KEY_EXPOSURE_COMPENSATION = "exposure-compensation";
    const string KEY_EXPOSURE_MODE = "exposure-mode";
    const string KEY_EXPOSURE_PROGRAM = "exposure-program";
    const string KEY_EXPOSURE_TIME = "exposure-time";
    const string KEY_F_NUMBER = "f-number";
    const string KEY_FLASH = "flash";
    const string KEY_FOCAL_LENGTH = "focal-length";
    const string KEY_FOCAL_LENGTH_IN_35MM_FORMAT = "focal-length-in-35mm-format";
    const string KEY_GAIN_CONTROL = "gain-control";
    const string KEY_GPS_ALTITUDE = "gps-altitude";
    const string KEY_GPS_ALTITUDE_REF = "gps-altitude-ref";
    const string KEY_GPS_DATE_STAMP = "gps-date-stamp";
    const string KEY_GPS_DIRECTION = "gps-direction";
    const string KEY_GPS_DIRECTION_REF = "gps-direction-ref";
    const string KEY_GPS_LATITUDE = "gps-latitude";
    const string KEY_GPS_LATITUDE_REF = "gps-latitude-ref";
    const string KEY_GPS_LONGITUDE = "gps-longitude";
    const string KEY_GPS_LONGITUDE_REF = "gps-longitude-ref";
    const string KEY_GPS_MEASURE_MODE = "gps-measure-mode";
    const string KEY_GPS_SATELLITES = "gps-satellites";
    const string KEY_GPS_STATUS = "gps-status";
    const string KEY_GPS_VERSION_ID = "gps-version-id";
    const string KEY_ISO = "iso";
    const string KEY_LIGHT_SOURCE = "light-source";
    const string KEY_MAKE = "make";
    const string KEY_METERING_MODE = "metering-mode";
    const string KEY_MODEL = "model";
    const string KEY_ORIENTATION = "orientation";
    const string KEY_SATURATION = "saturation";
    const string KEY_SCENE_CAPTURE_TYPE = "scene-capture-type";
    const string KEY_SCENE_TYPE = "scene-type";
    const string KEY_SENSING_METHOD = "sensing-method";
    const string KEY_SHARPNESS = "sharpness";

    // nikon
    const string KEY_AUTO_FOCUS_AREA_MODE = "auto-focus-area-mode";
    const string KEY_AUTO_FOCUS_POINT = "auto-focus-point";
    const string KEY_ACTIVE_D_LIGHTING = "active-d-lighting";
    const string KEY_COLORSPACE = "colorspace";
    const string KEY_EXPOSURE_DIFFERENCE = "exposure-difference";
    const string KEY_FLASH_COLOR_FILTER = "flash-color-filter";
    const string KEY_FlASH_COMPENSATION = "flash-compensation";
    const string KEY_FLASH_CONTROL_MODE = "flash-control-mode";
    const string KEY_FLASH_EXPOSURE_COMPENSATION = "flash-exposure-compensation";
    const string KEY_FLASH_FOCAL_LENGTH = "flash-focal-length";
    const string KEY_FLASH_MODE = "flash-mode";
    const string KEY_FLASH_SETTING = "flash-setting";
    const string KEY_FLASH_TYPE = "flash-type";
    const string KEY_FOCUS_DISTANCE = "focus-distance";
    const string KEY_FOCUS_MODE = "focus-mode";
    const string KEY_FOCUS_POSITION = "focus-position";
    const string KEY_HIGH_ISO_NOISE_REDUCTION = "high-iso-noise-reduction";
    const string KEY_HUE_ADJUSTMENT = "hue-adjustment";
    const string KEY_NOISE_REDUCTION = "noise-reduction";
    const string KEY_PICTURE_CONTROL_NAME = "picture-control-name";
    const string KEY_PRIMARY_AF_POINT = "primary-af-point";
    const string KEY_VR_MODE = "vr-mode";
    const string KEY_VIBRATION_REDUCTION = "vibration-reduction";
    const string KEY_VIGNETTE_CONTROL = "vignette-control";
    const string KEY_WHITE_BALANCE = "white-balance";

    // composite
    const string KEY_APERTURE = "aperture";
    const string KEY_AUTO_FOCUS = "auto-focus";
    const string KEY_DEPTH_OF_FIELD = "depth-of-field";
    const string KEY_FIELD_OF_VIEW = "field-of-view";
    const string KEY_HYPERFOCAL_DISTANCE = "hyperfocal-distance";
    const string KEY_LENS_ID = "lens-id";
    const string KEY_LIGHT_VALUE = "light-value";
    const string KEY_SCALE_FACTOR_35_EFL = "scale-factor-35-efl";
    const string KEY_SHUTTER_SPEED = "shutter-speed";

    static readonly RedisValue[] _hashFields = new RedisValue[]
    {
        KEY_BITS_PER_SAMPLE,
        KEY_COMPRESSION,
        KEY_CONTRAST,
        KEY_CREATE_DATE,
        KEY_DIGITAL_ZOOM_RATIO,
        KEY_EXPOSURE_COMPENSATION,
        KEY_EXPOSURE_MODE,
        KEY_EXPOSURE_PROGRAM,
        KEY_EXPOSURE_TIME,
        KEY_F_NUMBER,
        KEY_FLASH,
        KEY_FOCAL_LENGTH,
        KEY_FOCAL_LENGTH_IN_35MM_FORMAT,
        KEY_GAIN_CONTROL,
        KEY_GPS_ALTITUDE,
        KEY_GPS_ALTITUDE_REF,
        KEY_GPS_DATE_STAMP,
        KEY_GPS_DIRECTION,
        KEY_GPS_DIRECTION_REF,
        KEY_GPS_LATITUDE,
        KEY_GPS_LATITUDE_REF,
        KEY_GPS_LONGITUDE,
        KEY_GPS_LONGITUDE_REF,
        KEY_GPS_MEASURE_MODE,
        KEY_GPS_SATELLITES,
        KEY_GPS_STATUS,
        KEY_GPS_VERSION_ID,
        KEY_ISO,
        KEY_LIGHT_SOURCE,
        KEY_MAKE,
        KEY_METERING_MODE,
        KEY_MODEL,
        KEY_ORIENTATION,
        KEY_SATURATION,
        KEY_SCENE_CAPTURE_TYPE,
        KEY_SCENE_TYPE,
        KEY_SENSING_METHOD,
        KEY_SHARPNESS,

        KEY_AUTO_FOCUS_AREA_MODE,
        KEY_AUTO_FOCUS_POINT,
        KEY_ACTIVE_D_LIGHTING,
        KEY_COLORSPACE,
        KEY_EXPOSURE_DIFFERENCE,
        KEY_FLASH_COLOR_FILTER,
        KEY_FlASH_COMPENSATION,
        KEY_FLASH_CONTROL_MODE,
        KEY_FLASH_EXPOSURE_COMPENSATION,
        KEY_FLASH_FOCAL_LENGTH,
        KEY_FLASH_MODE,
        KEY_FLASH_SETTING,
        KEY_FLASH_TYPE,
        KEY_FOCUS_DISTANCE,
        KEY_FOCUS_MODE,
        KEY_FOCUS_POSITION,
        KEY_HIGH_ISO_NOISE_REDUCTION,
        KEY_HUE_ADJUSTMENT,
        KEY_NOISE_REDUCTION,
        KEY_PICTURE_CONTROL_NAME,
        KEY_PRIMARY_AF_POINT,
        KEY_VR_MODE,
        KEY_VIBRATION_REDUCTION,
        KEY_VIGNETTE_CONTROL,
        KEY_WHITE_BALANCE,

        KEY_APERTURE,
        KEY_AUTO_FOCUS,
        KEY_DEPTH_OF_FIELD,
        KEY_FIELD_OF_VIEW,
        KEY_HYPERFOCAL_DISTANCE,
        KEY_LENS_ID,
        KEY_LIGHT_VALUE,
        KEY_SCALE_FACTOR_35_EFL,
        KEY_SHUTTER_SPEED
    };

    static readonly RedisValue[] _sortLookup = new RedisValue[]
    {
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_BITS_PER_SAMPLE),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_COMPRESSION),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_CONTRAST),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_CREATE_DATE),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_DIGITAL_ZOOM_RATIO),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_EXPOSURE_COMPENSATION),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_EXPOSURE_MODE),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_EXPOSURE_PROGRAM),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_EXPOSURE_TIME),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_F_NUMBER),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_FLASH),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_FOCAL_LENGTH),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_FOCAL_LENGTH_IN_35MM_FORMAT),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_GAIN_CONTROL),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_GPS_ALTITUDE),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_GPS_ALTITUDE_REF),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_GPS_DATE_STAMP),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_GPS_DIRECTION),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_GPS_DIRECTION_REF),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_GPS_LATITUDE),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_GPS_LATITUDE_REF),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_GPS_LONGITUDE),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_GPS_LONGITUDE_REF),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_GPS_MEASURE_MODE),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_GPS_SATELLITES),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_GPS_STATUS),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_GPS_VERSION_ID),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_ISO),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_LIGHT_SOURCE),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_MAKE),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_METERING_MODE),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_MODEL),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_ORIENTATION),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_SATURATION),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_SCENE_CAPTURE_TYPE),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_SCENE_TYPE),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_SENSING_METHOD),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_SHARPNESS),

        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_AUTO_FOCUS_AREA_MODE),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_AUTO_FOCUS_POINT),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_ACTIVE_D_LIGHTING),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_COLORSPACE),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_EXPOSURE_DIFFERENCE),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_FLASH_COLOR_FILTER),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_FlASH_COMPENSATION),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_FLASH_CONTROL_MODE),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_FLASH_EXPOSURE_COMPENSATION),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_FLASH_FOCAL_LENGTH),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_FLASH_MODE),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_FLASH_SETTING),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_FLASH_TYPE),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_FOCUS_DISTANCE),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_FOCUS_MODE),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_FOCUS_POSITION),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_HIGH_ISO_NOISE_REDUCTION),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_HUE_ADJUSTMENT),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_NOISE_REDUCTION),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_PICTURE_CONTROL_NAME),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_PRIMARY_AF_POINT),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_VR_MODE),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_VIBRATION_REDUCTION),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_VIGNETTE_CONTROL),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_WHITE_BALANCE),

        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_APERTURE),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_AUTO_FOCUS),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_DEPTH_OF_FIELD),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_FIELD_OF_VIEW),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_HYPERFOCAL_DISTANCE),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_LENS_ID),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_LIGHT_VALUE),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_SCALE_FACTOR_35_EFL),
        GetSortExternalLookup(PhotoKeys.EXIF_HASH_KEY_PATTERN, KEY_SHUTTER_SPEED)
    };

    public override RedisValue[] HashFields { get => _hashFields; }
    public override RedisValue[] SortLookupFields { get => _sortLookup; }

    public override HashEntry[] BuildHashSet(Detail item)
    {
        var entries = new List<HashEntry>();

        if(item.BitsPerSample != null) { entries.Add(new HashEntry(KEY_BITS_PER_SAMPLE, (uint)item.BitsPerSample)); }
        if(item.Compression != null) { entries.Add(new HashEntry(KEY_COMPRESSION, item.Compression)); }
        if(item.Contrast != null) { entries.Add(new HashEntry(KEY_CONTRAST, item.Contrast)); }
        if(item.CreateDate != null) { entries.Add(new HashEntry(KEY_CREATE_DATE, SerializeDate((DateTime)item.CreateDate))); }
        if(item.DigitalZoomRatio != null) { entries.Add(new HashEntry(KEY_DIGITAL_ZOOM_RATIO, item.DigitalZoomRatio)); }
        if(item.ExposureCompensation != null) { entries.Add(new HashEntry(KEY_EXPOSURE_COMPENSATION, item.ExposureCompensation)); }
        if(item.ExposureMode != null) { entries.Add(new HashEntry(KEY_EXPOSURE_MODE, item.ExposureMode)); }
        if(item.ExposureProgram != null) { entries.Add(new HashEntry(KEY_EXPOSURE_PROGRAM, item.ExposureProgram)); }
        if(item.ExposureTime != null) { entries.Add(new HashEntry(KEY_EXPOSURE_TIME, item.ExposureTime)); }
        if(item.FNumber != null) { entries.Add(new HashEntry(KEY_F_NUMBER, item.FNumber)); }
        if(item.Flash != null) { entries.Add(new HashEntry(KEY_FLASH, item.Flash)); }
        if(item.FocalLength != null) { entries.Add(new HashEntry(KEY_FOCAL_LENGTH, item.FocalLength)); }
        if(item.FocalLengthIn35mmFormat != null) { entries.Add(new HashEntry(KEY_FOCAL_LENGTH_IN_35MM_FORMAT, item.FocalLengthIn35mmFormat)); }
        if(item.GainControl != null) { entries.Add(new HashEntry(KEY_GAIN_CONTROL, item.GainControl)); }
        if(item.GpsAltitude != null) { entries.Add(new HashEntry(KEY_GPS_ALTITUDE, item.GpsAltitude)); }
        if(item.GpsAltitudeRef != null) { entries.Add(new HashEntry(KEY_GPS_ALTITUDE_REF, item.GpsAltitudeRef)); }
        if(item.GpsDateStamp != null) { entries.Add(new HashEntry(KEY_GPS_DATE_STAMP, SerializeDate((DateTime)item.GpsDateStamp))); }
        if(item.GpsDirection != null) { entries.Add(new HashEntry(KEY_GPS_DIRECTION, item.GpsDirection)); }
        if(item.GpsDirectionRef != null) { entries.Add(new HashEntry(KEY_GPS_DIRECTION_REF, item.GpsDirectionRef)); }
        if(item.GpsLatitude != null) { entries.Add(new HashEntry(KEY_GPS_LATITUDE, item.GpsLatitude)); }
        if(item.GpsLatitudeRef != null) { entries.Add(new HashEntry(KEY_GPS_LATITUDE_REF, item.GpsLatitudeRef)); }
        if(item.GpsLongitude != null) { entries.Add(new HashEntry(KEY_GPS_LONGITUDE, item.GpsLongitude)); }
        if(item.GpsLongitudeRef != null) { entries.Add(new HashEntry(KEY_GPS_LONGITUDE_REF, item.GpsLongitudeRef)); }
        if(item.GpsMeasureMode != null) { entries.Add(new HashEntry(KEY_GPS_MEASURE_MODE, item.GpsMeasureMode)); }
        if(item.GpsSatellites != null) { entries.Add(new HashEntry(KEY_GPS_SATELLITES, item.GpsSatellites)); }
        if(item.GpsStatus != null) { entries.Add(new HashEntry(KEY_GPS_STATUS, item.GpsStatus)); }
        if(item.GpsVersionId != null) { entries.Add(new HashEntry(KEY_GPS_VERSION_ID, item.GpsVersionId)); }
        if(item.Iso != null) { entries.Add(new HashEntry(KEY_ISO, item.Iso)); }
        if(item.LightSource != null) { entries.Add(new HashEntry(KEY_LIGHT_SOURCE, item.LightSource)); }
        if(item.Make != null) { entries.Add(new HashEntry(KEY_MAKE, item.Make)); }
        if(item.MeteringMode != null) { entries.Add(new HashEntry(KEY_METERING_MODE, item.MeteringMode)); }
        if(item.Model != null) { entries.Add(new HashEntry(KEY_MODEL, item.Model)); }
        if(item.Orientation != null) { entries.Add(new HashEntry(KEY_ORIENTATION, item.Orientation)); }
        if(item.Saturation != null) { entries.Add(new HashEntry(KEY_SATURATION, item.Saturation)); }
        if(item.SceneCaptureType != null) { entries.Add(new HashEntry(KEY_SCENE_CAPTURE_TYPE, item.SceneCaptureType)); }
        if(item.SceneType != null) { entries.Add(new HashEntry(KEY_SCENE_TYPE, item.SceneType)); }
        if(item.SensingMethod != null) { entries.Add(new HashEntry(KEY_SENSING_METHOD, item.SensingMethod)); }
        if(item.Sharpness != null) { entries.Add(new HashEntry(KEY_SHARPNESS, item.Sharpness)); }

        if(item.AutoFocusAreaMode != null) { entries.Add(new HashEntry(KEY_AUTO_FOCUS_AREA_MODE, item.AutoFocusAreaMode)); }
        if(item.AutoFocusPoint != null) { entries.Add(new HashEntry(KEY_AUTO_FOCUS_POINT, item.AutoFocusPoint)); }
        if(item.ActiveDLighting != null) { entries.Add(new HashEntry(KEY_ACTIVE_D_LIGHTING, item.ActiveDLighting)); }
        if(item.Colorspace != null) { entries.Add(new HashEntry(KEY_COLORSPACE, item.Colorspace)); }
        if(item.ExposureDifference != null) { entries.Add(new HashEntry(KEY_EXPOSURE_DIFFERENCE, item.ExposureDifference)); }
        if(item.FlashColorFilter != null) { entries.Add(new HashEntry(KEY_FLASH_COLOR_FILTER, item.FlashColorFilter)); }
        if(item.FlashCompensation != null) { entries.Add(new HashEntry(KEY_FlASH_COMPENSATION, item.FlashCompensation)); }
        if(item.FlashControlMode != null) { entries.Add(new HashEntry(KEY_FLASH_CONTROL_MODE, item.FlashControlMode)); }
        if(item.FlashExposureCompensation != null) { entries.Add(new HashEntry(KEY_FLASH_EXPOSURE_COMPENSATION, item.FlashExposureCompensation)); }
        if(item.FlashFocalLength != null) { entries.Add(new HashEntry(KEY_FLASH_FOCAL_LENGTH, item.FlashFocalLength)); }
        if(item.FlashMode != null) { entries.Add(new HashEntry(KEY_FLASH_MODE, item.FlashMode)); }
        if(item.FlashSetting != null) { entries.Add(new HashEntry(KEY_FLASH_SETTING, item.FlashSetting)); }
        if(item.FlashType != null) { entries.Add(new HashEntry(KEY_FLASH_TYPE, item.FlashType)); }
        if(item.FocusDistance != null) { entries.Add(new HashEntry(KEY_FOCUS_DISTANCE, item.FocusDistance)); }
        if(item.FocusMode != null) { entries.Add(new HashEntry(KEY_FOCUS_MODE, item.FocusMode)); }
        if(item.FocusPosition != null) { entries.Add(new HashEntry(KEY_FOCUS_POSITION, item.FocusPosition)); }
        if(item.HighIsoNoiseReduction != null) { entries.Add(new HashEntry(KEY_HIGH_ISO_NOISE_REDUCTION, item.HighIsoNoiseReduction)); }
        if(item.HueAdjustment != null) { entries.Add(new HashEntry(KEY_HUE_ADJUSTMENT, item.HueAdjustment)); }
        if(item.NoiseReduction != null) { entries.Add(new HashEntry(KEY_NOISE_REDUCTION, item.NoiseReduction)); }
        if(item.PictureControlName != null) { entries.Add(new HashEntry(KEY_PICTURE_CONTROL_NAME, item.PictureControlName)); }
        if(item.PrimaryAFPoint != null) { entries.Add(new HashEntry(KEY_PRIMARY_AF_POINT, item.PrimaryAFPoint)); }
        if(item.VRMode != null) { entries.Add(new HashEntry(KEY_VR_MODE, item.VRMode)); }
        if(item.VibrationReduction != null) { entries.Add(new HashEntry(KEY_VIBRATION_REDUCTION, item.VibrationReduction)); }
        if(item.VignetteControl != null) { entries.Add(new HashEntry(KEY_VIGNETTE_CONTROL, item.VignetteControl)); }
        if(item.WhiteBalance != null) { entries.Add(new HashEntry(KEY_WHITE_BALANCE, item.WhiteBalance)); }

        if(item.Aperture != null) { entries.Add(new HashEntry(KEY_APERTURE, item.Aperture)); }
        if(item.AutoFocus != null) { entries.Add(new HashEntry(KEY_AUTO_FOCUS, item.AutoFocus)); }
        if(item.DepthOfField != null) { entries.Add(new HashEntry(KEY_DEPTH_OF_FIELD, item.DepthOfField)); }
        if(item.FieldOfView != null) { entries.Add(new HashEntry(KEY_FIELD_OF_VIEW, item.FieldOfView)); }
        if(item.HyperfocalDistance != null) { entries.Add(new HashEntry(KEY_HYPERFOCAL_DISTANCE, item.HyperfocalDistance)); }
        if(item.LensId != null) { entries.Add(new HashEntry(KEY_LENS_ID, item.LensId)); }
        if(item.LightValue != null) { entries.Add(new HashEntry(KEY_LIGHT_VALUE, item.LightValue)); }
        if(item.ScaleFactor35Efl != null) { entries.Add(new HashEntry(KEY_SCALE_FACTOR_35_EFL, item.ScaleFactor35Efl)); }
        if(item.ShutterSpeed != null) { entries.Add(new HashEntry(KEY_SHUTTER_SPEED, item.ShutterSpeed)); }

        return entries.ToArray();
    }

    protected override Detail ParseSingleInternal(ReadOnlySpan<RedisValue> values)
    {
        int i = 0;

        return new Detail
        {
            BitsPerSample = (ushort?)(int?)values[i++],
            Compression = values[i++],
            Contrast = values[i++],
            CreateDate = values[i++] == RedisValue.Null ? null : DeserializeDate(values[i - 1]!),
            DigitalZoomRatio = (double?)values[i++],
            ExposureCompensation = values[i++],
            ExposureMode = values[i++],
            ExposureProgram = values[i++],
            ExposureTime = values[i++],
            FNumber = (double?)values[i++],
            Flash = values[i++],
            FocalLength = (double?)values[i++],
            FocalLengthIn35mmFormat = (double?)values[i++],
            GainControl = values[i++],
            GpsAltitude = (double?)values[i++],
            GpsAltitudeRef = values[i++],
            GpsDateStamp = values[i++] == RedisValue.Null ? null : DeserializeDate(values[i - 1]!),
            GpsDirection = (double?)values[i++],
            GpsDirectionRef = values[i++],
            GpsLatitude = (double?)values[i++],
            GpsLatitudeRef = values[i++],
            GpsLongitude = (double?)values[i++],
            GpsLongitudeRef = values[i++],
            GpsMeasureMode = values[i++],
            GpsSatellites = values[i++],
            GpsStatus = values[i++],
            GpsVersionId = values[i++],
            Iso = (int?)values[i++],
            LightSource = values[i++],
            Make = values[i++],
            MeteringMode = values[i++],
            Model = values[i++],
            Orientation = values[i++],
            Saturation = values[i++],
            SceneCaptureType = values[i++],
            SceneType = values[i++],
            SensingMethod = values[i++],
            Sharpness = values[i++],

            AutoFocusAreaMode = values[i++],
            AutoFocusPoint = values[i++],
            ActiveDLighting = values[i++],
            Colorspace = values[i++],
            ExposureDifference = (double?)values[i++],
            FlashColorFilter = values[i++],
            FlashCompensation = values[i++],
            FlashControlMode = (short?)values[i++],
            FlashExposureCompensation = values[i++],
            FlashFocalLength = (double?)values[i++],
            FlashMode = values[i++],
            FlashSetting = values[i++],
            FlashType = values[i++],
            FocusDistance = (double?)values[i++],
            FocusMode = values[i++],
            FocusPosition = (int?)values[i++],
            HighIsoNoiseReduction = values[i++],
            HueAdjustment = values[i++],
            NoiseReduction = values[i++],
            PictureControlName = values[i++],
            PrimaryAFPoint = values[i++],
            VRMode = values[i++],
            VibrationReduction = values[i++],
            VignetteControl = values[i++],
            WhiteBalance = values[i++],

            Aperture = (double?)values[i++],
            AutoFocus = values[i++],
            DepthOfField = values[i++],
            FieldOfView = values[i++],
            HyperfocalDistance = (double?)values[i++],
            LensId = values[i++],
            LightValue = (double?)values[i++],
            ScaleFactor35Efl = (double?)values[i++],
            ShutterSpeed = values[i++]
        };
    }
}
