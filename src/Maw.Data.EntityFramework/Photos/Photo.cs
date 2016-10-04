using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("photo", Schema = "photo")]
    public partial class Photo
    {
        public Photo()
        {
            Comment = new HashSet<Comment>();
            Rating = new HashSet<Rating>();
        }

        [Column("id")]
        public int Id { get; set; }
        [Column("category_id")]
        public short CategoryId { get; set; }
        [Column("is_private")]
        public bool IsPrivate { get; set; }
        [Column("xs_height")]
        public short XsHeight { get; set; }
        [Column("xs_width")]
        public short XsWidth { get; set; }
        [Required]
        [Column("xs_path", TypeName = "varchar")]
        [MaxLength(255)]
        public string XsPath { get; set; }
        [Column("sm_height")]
        public short SmHeight { get; set; }
        [Column("sm_width")]
        public short SmWidth { get; set; }
        [Required]
        [Column("sm_path", TypeName = "varchar")]
        [MaxLength(255)]
        public string SmPath { get; set; }
        [Column("md_height")]
        public short MdHeight { get; set; }
        [Column("md_width")]
        public short MdWidth { get; set; }
        [Required]
        [Column("md_path", TypeName = "varchar")]
        [MaxLength(255)]
        public string MdPath { get; set; }
        [Column("lg_height")]
        public short LgHeight { get; set; }
        [Column("lg_width")]
        public short LgWidth { get; set; }
        [Required]
        [Column("lg_path", TypeName = "varchar")]
        [MaxLength(255)]
        public string LgPath { get; set; }
        [Column("prt_height")]
        public short PrtHeight { get; set; }
        [Column("prt_width")]
        public short PrtWidth { get; set; }
        [Required]
        [Column("prt_path", TypeName = "varchar")]
        [MaxLength(255)]
        public string PrtPath { get; set; }
        [Column("src_height")]
        public short SrcHeight { get; set; }
        [Column("src_width")]
        public short SrcWidth { get; set; }
        [Required]
        [Column("src_path", TypeName = "varchar")]
        [MaxLength(255)]
        public string SrcPath { get; set; }
        [Column("bits_per_sample")]
        public short? BitsPerSample { get; set; }
        [Column("compression_id")]
        public int? CompressionId { get; set; }
        [Column("contrast_id")]
        public int? ContrastId { get; set; }
        [Column("create_date")]
        public DateTime? CreateDate { get; set; }
        [Column("digital_zoom_ratio")]
        public float? DigitalZoomRatio { get; set; }
        [Column("exposure_compensation", TypeName = "varchar")]
        [MaxLength(10)]
        public string ExposureCompensation { get; set; }
        [Column("exposure_mode_id")]
        public int? ExposureModeId { get; set; }
        [Column("exposure_program_id")]
        public int? ExposureProgramId { get; set; }
        [Column("exposure_time", TypeName = "varchar")]
        [MaxLength(10)]
        public string ExposureTime { get; set; }
        [Column("f_number")]
        public decimal? FNumber { get; set; }
        [Column("flash_id")]
        public int? FlashId { get; set; }
        [Column("focal_length")]
        public float? FocalLength { get; set; }
        [Column("focal_length_in_35_mm_format")]
        public float? FocalLengthIn35MmFormat { get; set; }
        [Column("gain_control_id")]
        public int? GainControlId { get; set; }
        [Column("gps_altitude")]
        public float? GpsAltitude { get; set; }
        [Column("gps_altitude_ref_id")]
        public short? GpsAltitudeRefId { get; set; }
        [Column("gps_date_time_stamp")]
        public DateTime? GpsDateTimeStamp { get; set; }
        [Column("gps_direction")]
        public float? GpsDirection { get; set; }
        [Column("gps_direction_ref_id", TypeName = "varchar")]
        [MaxLength(2)]
        public string GpsDirectionRefId { get; set; }
        [Column("gps_latitude")]
        public float? GpsLatitude { get; set; }
        [Column("gps_latitude_ref_id", TypeName = "varchar")]
        [MaxLength(2)]
        public string GpsLatitudeRefId { get; set; }
        [Column("gps_longitude")]
        public float? GpsLongitude { get; set; }
        [Column("gps_longitude_ref_id", TypeName = "varchar")]
        [MaxLength(2)]
        public string GpsLongitudeRefId { get; set; }
        [Column("gps_measure_mode_id", TypeName = "varchar")]
        [MaxLength(2)]
        public string GpsMeasureModeId { get; set; }
        [Column("gps_satellites", TypeName = "varchar")]
        [MaxLength(150)]
        public string GpsSatellites { get; set; }
        [Column("gps_status_id", TypeName = "varchar")]
        [MaxLength(2)]
        public string GpsStatusId { get; set; }
        [Column("gps_version_id", TypeName = "varchar")]
        [MaxLength(16)]
        public string GpsVersionId { get; set; }
        [Column("iso")]
        public int? Iso { get; set; }
        [Column("light_source_id")]
        public int? LightSourceId { get; set; }
        [Column("make_id")]
        public short? MakeId { get; set; }
        [Column("metering_mode_id")]
        public int? MeteringModeId { get; set; }
        [Column("model_id")]
        public short? ModelId { get; set; }
        [Column("orientation_id")]
        public int? OrientationId { get; set; }
        [Column("saturation_id")]
        public short? SaturationId { get; set; }
        [Column("scene_capture_type_id")]
        public int? SceneCaptureTypeId { get; set; }
        [Column("scene_type_id")]
        public int? SceneTypeId { get; set; }
        [Column("sensing_method_id")]
        public int? SensingMethodId { get; set; }
        [Column("sharpness_id")]
        public int? SharpnessId { get; set; }
        [Column("af_area_mode_id")]
        public short? AfAreaModeId { get; set; }
        [Column("af_point_id")]
        public short? AfPointId { get; set; }
        [Column("active_d_lighting_id")]
        public short? ActiveDLightingId { get; set; }
        [Column("colorspace_id")]
        public short? ColorspaceId { get; set; }
        [Column("exposure_difference")]
        public float? ExposureDifference { get; set; }
        [Column("flash_color_filter_id")]
        public short? FlashColorFilterId { get; set; }
        [Column("flash_compensation", TypeName = "varchar")]
        [MaxLength(10)]
        public string FlashCompensation { get; set; }
        [Column("flash_control_mode")]
        public short? FlashControlMode { get; set; }
        [Column("flash_exposure_compensation", TypeName = "varchar")]
        [MaxLength(10)]
        public string FlashExposureCompensation { get; set; }
        [Column("flash_focal_length")]
        public short? FlashFocalLength { get; set; }
        [Column("flash_mode_id")]
        public short? FlashModeId { get; set; }
        [Column("flash_setting_id")]
        public short? FlashSettingId { get; set; }
        [Column("flash_type_id")]
        public short? FlashTypeId { get; set; }
        [Column("focus_distance")]
        public float? FocusDistance { get; set; }
        [Column("focus_mode_id")]
        public short? FocusModeId { get; set; }
        [Column("focus_position")]
        public short? FocusPosition { get; set; }
        [Column("high_iso_noise_reduction_id")]
        public short? HighIsoNoiseReductionId { get; set; }
        [Column("hue_adjustment_id")]
        public short? HueAdjustmentId { get; set; }
        [Column("noise_reduction_id")]
        public short? NoiseReductionId { get; set; }
        [Column("picture_control_name_id")]
        public short? PictureControlNameId { get; set; }
        [Column("primary_af_point", TypeName = "varchar")]
        [MaxLength(20)]
        public string PrimaryAfPoint { get; set; }
        [Column("vibration_reduction_id")]
        public short? VibrationReductionId { get; set; }
        [Column("vignette_control_id")]
        public short? VignetteControlId { get; set; }
        [Column("vr_mode_id")]
        public short? VrModeId { get; set; }
        [Column("white_balance_id")]
        public short? WhiteBalanceId { get; set; }
        [Column("aperture")]
        public decimal? Aperture { get; set; }
        [Column("auto_focus_id")]
        public short? AutoFocusId { get; set; }
        [Column("depth_of_field", TypeName = "varchar")]
        [MaxLength(50)]
        public string DepthOfField { get; set; }
        [Column("field_of_view", TypeName = "varchar")]
        [MaxLength(50)]
        public string FieldOfView { get; set; }
        [Column("hyperfocal_distance")]
        public float? HyperfocalDistance { get; set; }
        [Column("lens_id")]
        public short? LensId { get; set; }
        [Column("light_value")]
        public float? LightValue { get; set; }
        [Column("scale_factor_35_efl")]
        public float? ScaleFactor35Efl { get; set; }
        [Column("shutter_speed", TypeName = "varchar")]
        [MaxLength(50)]
        public string ShutterSpeed { get; set; }
        [Column("raw_conversion_mode_id")]
        public short? RawConversionModeId { get; set; }
        [Column("sigmoidal_contrast_adjustment")]
        public float? SigmoidalContrastAdjustment { get; set; }
        [Column("saturation_adjustment")]
        public float? SaturationAdjustment { get; set; }
        [Column("compression_quality")]
        public short? CompressionQuality { get; set; }
        [Column("aws_glacier_vault_id")]
        public short? AwsGlacierVaultId { get; set; }
        [Column("aws_archive_id", TypeName = "bpchar")]
        [MaxLength(138)]
        public string AwsArchiveId { get; set; }
        [Column("aws_treehash", TypeName = "bpchar")]
        [MaxLength(64)]
        public string AwsTreehash { get; set; }

        [InverseProperty("Photo")]
        public virtual ICollection<Comment> Comment { get; set; }
        [InverseProperty("Photo")]
        public virtual ICollection<Rating> Rating { get; set; }
        [ForeignKey("ActiveDLightingId")]
        [InverseProperty("Photo")]
        public virtual ActiveDLighting ActiveDLighting { get; set; }
        [ForeignKey("AfAreaModeId")]
        [InverseProperty("Photo")]
        public virtual AfAreaMode AfAreaMode { get; set; }
        [ForeignKey("AfPointId")]
        [InverseProperty("Photo")]
        public virtual AfPoint AfPoint { get; set; }
        [ForeignKey("AutoFocusId")]
        [InverseProperty("Photo")]
        public virtual AutoFocus AutoFocus { get; set; }
        [ForeignKey("CategoryId")]
        [InverseProperty("Photo")]
        public virtual Category Category { get; set; }
        [ForeignKey("ColorspaceId")]
        [InverseProperty("Photo")]
        public virtual Colorspace Colorspace { get; set; }
        [ForeignKey("CompressionId")]
        [InverseProperty("Photo")]
        public virtual Compression Compression { get; set; }
        [ForeignKey("ContrastId")]
        [InverseProperty("Photo")]
        public virtual Contrast Contrast { get; set; }
        [ForeignKey("ExposureModeId")]
        [InverseProperty("Photo")]
        public virtual ExposureMode ExposureMode { get; set; }
        [ForeignKey("ExposureProgramId")]
        [InverseProperty("Photo")]
        public virtual ExposureProgram ExposureProgram { get; set; }
        [ForeignKey("FlashColorFilterId")]
        [InverseProperty("Photo")]
        public virtual FlashColorFilter FlashColorFilter { get; set; }
        [ForeignKey("FlashId")]
        [InverseProperty("Photo")]
        public virtual Flash Flash { get; set; }
        [ForeignKey("FlashModeId")]
        [InverseProperty("Photo")]
        public virtual FlashMode FlashMode { get; set; }
        [ForeignKey("FlashSettingId")]
        [InverseProperty("Photo")]
        public virtual FlashSetting FlashSetting { get; set; }
        [ForeignKey("FlashTypeId")]
        [InverseProperty("Photo")]
        public virtual FlashType FlashType { get; set; }
        [ForeignKey("FocusModeId")]
        [InverseProperty("Photo")]
        public virtual FocusMode FocusMode { get; set; }
        [ForeignKey("GainControlId")]
        [InverseProperty("Photo")]
        public virtual GainControl GainControl { get; set; }
        [ForeignKey("GpsAltitudeRefId")]
        [InverseProperty("Photo")]
        public virtual GpsAltitudeRef GpsAltitudeRef { get; set; }
        [ForeignKey("GpsDirectionRefId")]
        [InverseProperty("Photo")]
        public virtual GpsDirectionRef GpsDirectionRef { get; set; }
        [ForeignKey("GpsLatitudeRefId")]
        [InverseProperty("Photo")]
        public virtual GpsLatitudeRef GpsLatitudeRef { get; set; }
        [ForeignKey("GpsLongitudeRefId")]
        [InverseProperty("Photo")]
        public virtual GpsLongitudeRef GpsLongitudeRef { get; set; }
        [ForeignKey("GpsMeasureModeId")]
        [InverseProperty("Photo")]
        public virtual GpsMeasureMode GpsMeasureMode { get; set; }
        [ForeignKey("GpsStatusId")]
        [InverseProperty("Photo")]
        public virtual GpsStatus GpsStatus { get; set; }
        [ForeignKey("HighIsoNoiseReductionId")]
        [InverseProperty("Photo")]
        public virtual HighIsoNoiseReduction HighIsoNoiseReduction { get; set; }
        [ForeignKey("HueAdjustmentId")]
        [InverseProperty("Photo")]
        public virtual HueAdjustment HueAdjustment { get; set; }
        [ForeignKey("LensId")]
        [InverseProperty("Photo")]
        public virtual Lens Lens { get; set; }
        [ForeignKey("LightSourceId")]
        [InverseProperty("Photo")]
        public virtual LightSource LightSource { get; set; }
        [ForeignKey("MakeId")]
        [InverseProperty("Photo")]
        public virtual Make Make { get; set; }
        [ForeignKey("MeteringModeId")]
        [InverseProperty("Photo")]
        public virtual MeteringMode MeteringMode { get; set; }
        [ForeignKey("ModelId")]
        [InverseProperty("Photo")]
        public virtual Model Model { get; set; }
        [ForeignKey("NoiseReductionId")]
        [InverseProperty("Photo")]
        public virtual NoiseReduction NoiseReduction { get; set; }
        [ForeignKey("OrientationId")]
        [InverseProperty("Photo")]
        public virtual Orientation Orientation { get; set; }
        [ForeignKey("PictureControlNameId")]
        [InverseProperty("Photo")]
        public virtual PictureControlName PictureControlName { get; set; }
        [ForeignKey("RawConversionModeId")]
        [InverseProperty("Photo")]
        public virtual RawConversionMode RawConversionMode { get; set; }
        [ForeignKey("SaturationId")]
        [InverseProperty("Photo")]
        public virtual Saturation Saturation { get; set; }
        [ForeignKey("SceneCaptureTypeId")]
        [InverseProperty("Photo")]
        public virtual SceneCaptureType SceneCaptureType { get; set; }
        [ForeignKey("SceneTypeId")]
        [InverseProperty("Photo")]
        public virtual SceneType SceneType { get; set; }
        [ForeignKey("SensingMethodId")]
        [InverseProperty("Photo")]
        public virtual SensingMethod SensingMethod { get; set; }
        [ForeignKey("SharpnessId")]
        [InverseProperty("Photo")]
        public virtual Sharpness Sharpness { get; set; }
        [ForeignKey("VibrationReductionId")]
        [InverseProperty("Photo")]
        public virtual VibrationReduction VibrationReduction { get; set; }
        [ForeignKey("VignetteControlId")]
        [InverseProperty("Photo")]
        public virtual VignetteControl VignetteControl { get; set; }
        [ForeignKey("VrModeId")]
        [InverseProperty("Photo")]
        public virtual VrMode VrMode { get; set; }
        [ForeignKey("WhiteBalanceId")]
        [InverseProperty("Photo")]
        public virtual WhiteBalance WhiteBalance { get; set; }
    }
}
