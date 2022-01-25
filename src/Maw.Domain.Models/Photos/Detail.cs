namespace Maw.Domain.Models.Photos;

public class Detail
{
    // exif
    public ushort? BitsPerSample { get; set; }
    public string? Compression { get; set; }
    public string? Contrast { get; set; }
    public DateTime? CreateDate { get; set; }
    public double? DigitalZoomRatio { get; set; }
    public string? ExposureCompensation { get; set; }
    public string? ExposureMode { get; set; }
    public string? ExposureProgram { get; set; }
    public string? ExposureTime { get; set; }
    public double? FNumber { get; set; }
    public string? Flash { get; set; }
    public double? FocalLength { get; set; }
    public double? FocalLengthIn35mmFormat { get; set; }
    public string? GainControl { get; set; }
    public double? GpsAltitude { get; set; }
    public string? GpsAltitudeRef { get; set; }
    public DateTime? GpsDateStamp { get; set; }
    public double? GpsDirection { get; set; }
    public string? GpsDirectionRef { get; set; }
    public double? GpsLatitude { get; set; }
    public string? GpsLatitudeRef { get; set; }
    public double? GpsLongitude { get; set; }
    public string? GpsLongitudeRef { get; set; }
    public string? GpsMeasureMode { get; set; }
    public string? GpsSatellites { get; set; }
    public string? GpsStatus { get; set; }
    public string? GpsVersionId { get; set; }
    public int? Iso { get; set; }
    public string? LightSource { get; set; }
    public string? Make { get; set; }
    public string? MeteringMode { get; set; }
    public string? Model { get; set; }
    public string? Orientation { get; set; }
    public string? Saturation { get; set; }
    public string? SceneCaptureType { get; set; }
    public string? SceneType { get; set; }
    public string? SensingMethod { get; set; }
    public string? Sharpness { get; set; }

    // nikon - we must get these as strings, because we will have pictures that are not just for nikon
    //       - as such, we can't use the nikon lookup tables in all cases, so we have to manage these
    //       - as generic lookup tables instead
    public string? AutoFocusAreaMode { get; set; }
    public string? AutoFocusPoint { get; set; }
    public string? ActiveDLighting { get; set; }
    public string? Colorspace { get; set; }
    public double? ExposureDifference { get; set; }
    public string? FlashColorFilter { get; set; }
    public string? FlashCompensation { get; set; }
    public short? FlashControlMode { get; set; }
    public string? FlashExposureCompensation { get; set; }
    public double? FlashFocalLength { get; set; }
    public string? FlashMode { get; set; }
    public string? FlashSetting { get; set; }
    public string? FlashType { get; set; }
    public double? FocusDistance { get; set; }
    public string? FocusMode { get; set; }
    public int? FocusPosition { get; set; }
    public string? HighIsoNoiseReduction { get; set; }
    public string? HueAdjustment { get; set; }
    public string? NoiseReduction { get; set; }
    public string? PictureControlName { get; set; }
    public string? PrimaryAFPoint { get; set; }
    public string? VRMode { get; set; }
    public string? VibrationReduction { get; set; }
    public string? VignetteControl { get; set; }
    public string? WhiteBalance { get; set; }

    // composite
    public double? Aperture { get; set; }
    public string? AutoFocus { get; set; }
    public string? DepthOfField { get; set; }
    public string? FieldOfView { get; set; }
    public double? HyperfocalDistance { get; set; }
    public string? LensId { get; set; }
    public double? LightValue { get; set; }
    public double? ScaleFactor35Efl { get; set; }
    public string? ShutterSpeed { get; set; }
}
