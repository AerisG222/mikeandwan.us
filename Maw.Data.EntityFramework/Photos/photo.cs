using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("photo", Schema = "photo")]
    public partial class photo
    {
        public photo()
        {
            comment = new HashSet<comment>();
            rating = new HashSet<rating>();
        }

        public int id { get; set; }
        public short? active_d_lighting_id { get; set; }
        public short? af_area_mode_id { get; set; }
        public short? af_point_id { get; set; }
        public decimal? aperture { get; set; }
        public short? auto_focus_id { get; set; }
        public short? bits_per_sample { get; set; }
        public short category_id { get; set; }
        public short? colorspace_id { get; set; }
        public short? compression_quality { get; set; }
        public int? compression_id { get; set; }
        public int? contrast_id { get; set; }
        public DateTime? create_date { get; set; }
        [MaxLength(50)]
        public string depth_of_field { get; set; }
        public float? digital_zoom_ratio { get; set; }
        [MaxLength(10)]
        public string exposure_compensation { get; set; }
        public float? exposure_difference { get; set; }
        public int? exposure_mode_id { get; set; }
        public int? exposure_program_id { get; set; }
        [MaxLength(10)]
        public string exposure_time { get; set; }
        public decimal? f_number { get; set; }
        [MaxLength(50)]
        public string field_of_view { get; set; }
        public short? flash_color_filter_id { get; set; }
        [MaxLength(10)]
        public string flash_compensation { get; set; }
        public short? flash_control_mode { get; set; }
        [MaxLength(10)]
        public string flash_exposure_compensation { get; set; }
        public short? flash_focal_length { get; set; }
        public int? flash_id { get; set; }
        public short? flash_mode_id { get; set; }
        public short? flash_setting_id { get; set; }
        public short? flash_type_id { get; set; }
        public float? focal_length { get; set; }
        public float? focal_length_in_35_mm_format { get; set; }
        public float? focus_distance { get; set; }
        public short? focus_mode_id { get; set; }
        public short? focus_position { get; set; }
        public int? gain_control_id { get; set; }
        public float? gps_altitude { get; set; }
        public short? gps_altitude_ref_id { get; set; }
        public DateTime? gps_date_time_stamp { get; set; }
        public float? gps_direction { get; set; }
        [MaxLength(2)]
        public string gps_direction_ref_id { get; set; }
        public float? gps_latitude { get; set; }
        [MaxLength(2)]
        public string gps_latitude_ref_id { get; set; }
        public float? gps_longitude { get; set; }
        [MaxLength(2)]
        public string gps_longitude_ref_id { get; set; }
        [MaxLength(2)]
        public string gps_measure_mode_id { get; set; }
        [MaxLength(150)]
        public string gps_satellites { get; set; }
        [MaxLength(2)]
        public string gps_status_id { get; set; }
        [MaxLength(16)]
        public string gps_version_id { get; set; }
        public short? high_iso_noise_reduction_id { get; set; }
        public short? hue_adjustment_id { get; set; }
        public float? hyperfocal_distance { get; set; }
        public bool is_private { get; set; }
        public int? iso { get; set; }
        public short? lens_id { get; set; }
        public short lg_height { get; set; }
        [Required]
        [MaxLength(255)]
        public string lg_path { get; set; }
        public short lg_width { get; set; }
        public int? light_source_id { get; set; }
        public float? light_value { get; set; }
        public short? make_id { get; set; }
        public short md_height { get; set; }
        [Required]
        [MaxLength(255)]
        public string md_path { get; set; }
        public short md_width { get; set; }
        public int? metering_mode_id { get; set; }
        public short? model_id { get; set; }
        public short? noise_reduction_id { get; set; }
        public int? orientation_id { get; set; }
        public short? picture_control_name_id { get; set; }
        [MaxLength(20)]
        public string primary_af_point { get; set; }
        public short prt_height { get; set; }
        [Required]
        [MaxLength(255)]
        public string prt_path { get; set; }
        public short prt_width { get; set; }
        public short? raw_conversion_mode_id { get; set; }
        public short? saturation_adjustment { get; set; }
        public short? saturation_id { get; set; }
        public float? scale_factor_35_efl { get; set; }
        public int? scene_capture_type_id { get; set; }
        public int? scene_type_id { get; set; }
        public int? sensing_method_id { get; set; }
        public int? sharpness_id { get; set; }
        public float? sigmoidal_contrast_adjustment { get; set; }
        [MaxLength(50)]
        public string shutter_speed { get; set; }
        public short sm_height { get; set; }
        [Required]
        [MaxLength(255)]
        public string sm_path { get; set; }
        public short sm_width { get; set; }
        public short src_height { get; set; }
        [Required]
        [MaxLength(255)]
        public string src_path { get; set; }
        public short src_width { get; set; }
        public short? vibration_reduction_id { get; set; }
        public short? vignette_control_id { get; set; }
        public short? vr_mode_id { get; set; }
        public short? white_balance_id { get; set; }
        public short xs_height { get; set; }
        [Required]
        [MaxLength(255)]
        public string xs_path { get; set; }
        public short xs_width { get; set; }

        [InverseProperty("photo")]
        public virtual ICollection<comment> comment { get; set; }
        [InverseProperty("photo")]
        public virtual ICollection<rating> rating { get; set; }
        [ForeignKey("active_d_lighting_id")]
        [InverseProperty("photo")]
        public virtual active_d_lighting active_d_lighting { get; set; }
        [ForeignKey("af_area_mode_id")]
        [InverseProperty("photo")]
        public virtual af_area_mode af_area_mode { get; set; }
        [ForeignKey("af_point_id")]
        [InverseProperty("photo")]
        public virtual af_point af_point { get; set; }
        [ForeignKey("auto_focus_id")]
        [InverseProperty("photo")]
        public virtual auto_focus auto_focus { get; set; }
        [ForeignKey("category_id")]
        [InverseProperty("photo")]
        public virtual category category { get; set; }
        [ForeignKey("colorspace_id")]
        [InverseProperty("photo")]
        public virtual colorspace colorspace { get; set; }
        [ForeignKey("compression_id")]
        [InverseProperty("photo")]
        public virtual compression compression { get; set; }
        [ForeignKey("contrast_id")]
        [InverseProperty("photo")]
        public virtual contrast contrast { get; set; }
        [ForeignKey("exposure_mode_id")]
        [InverseProperty("photo")]
        public virtual exposure_mode exposure_mode { get; set; }
        [ForeignKey("exposure_program_id")]
        [InverseProperty("photo")]
        public virtual exposure_program exposure_program { get; set; }
        [ForeignKey("flash_color_filter_id")]
        [InverseProperty("photo")]
        public virtual flash_color_filter flash_color_filter { get; set; }
        [ForeignKey("flash_id")]
        [InverseProperty("photo")]
        public virtual flash flash { get; set; }
        [ForeignKey("flash_mode_id")]
        [InverseProperty("photo")]
        public virtual flash_mode flash_mode { get; set; }
        [ForeignKey("flash_setting_id")]
        [InverseProperty("photo")]
        public virtual flash_setting flash_setting { get; set; }
        [ForeignKey("flash_type_id")]
        [InverseProperty("photo")]
        public virtual flash_type flash_type { get; set; }
        [ForeignKey("focus_mode_id")]
        [InverseProperty("photo")]
        public virtual focus_mode focus_mode { get; set; }
        [ForeignKey("gain_control_id")]
        [InverseProperty("photo")]
        public virtual gain_control gain_control { get; set; }
        [ForeignKey("gps_altitude_ref_id")]
        [InverseProperty("photo")]
        public virtual gps_altitude_ref gps_altitude_ref { get; set; }
        [ForeignKey("gps_direction_ref_id")]
        [InverseProperty("photo")]
        public virtual gps_direction_ref gps_direction_ref { get; set; }
        [ForeignKey("gps_latitude_ref_id")]
        [InverseProperty("photo")]
        public virtual gps_latitude_ref gps_latitude_ref { get; set; }
        [ForeignKey("gps_longitude_ref_id")]
        [InverseProperty("photo")]
        public virtual gps_longitude_ref gps_longitude_ref { get; set; }
        [ForeignKey("gps_measure_mode_id")]
        [InverseProperty("photo")]
        public virtual gps_measure_mode gps_measure_mode { get; set; }
        [ForeignKey("gps_status_id")]
        [InverseProperty("photo")]
        public virtual gps_status gps_status { get; set; }
        [ForeignKey("high_iso_noise_reduction_id")]
        [InverseProperty("photo")]
        public virtual high_iso_noise_reduction high_iso_noise_reduction { get; set; }
        [ForeignKey("hue_adjustment_id")]
        [InverseProperty("photo")]
        public virtual hue_adjustment hue_adjustment { get; set; }
        [ForeignKey("lens_id")]
        [InverseProperty("photo")]
        public virtual lens lens { get; set; }
        [ForeignKey("light_source_id")]
        [InverseProperty("photo")]
        public virtual light_source light_source { get; set; }
        [ForeignKey("make_id")]
        [InverseProperty("photo")]
        public virtual make make { get; set; }
        [ForeignKey("metering_mode_id")]
        [InverseProperty("photo")]
        public virtual metering_mode metering_mode { get; set; }
        [ForeignKey("model_id")]
        [InverseProperty("photo")]
        public virtual model model { get; set; }
        [ForeignKey("noise_reduction_id")]
        [InverseProperty("photo")]
        public virtual noise_reduction noise_reduction { get; set; }
        [ForeignKey("orientation_id")]
        [InverseProperty("photo")]
        public virtual orientation orientation { get; set; }
        [ForeignKey("picture_control_name_id")]
        [InverseProperty("photo")]
        public virtual picture_control_name picture_control_name { get; set; }
        [ForeignKey("raw_conversion_mode_id")]
        [InverseProperty("photo")]
        public virtual raw_conversion_mode raw_conversion_mode { get; set; }
        [ForeignKey("saturation_id")]
        [InverseProperty("photo")]
        public virtual saturation saturation { get; set; }
        [ForeignKey("scene_capture_type_id")]
        [InverseProperty("photo")]
        public virtual scene_capture_type scene_capture_type { get; set; }
        [ForeignKey("scene_type_id")]
        [InverseProperty("photo")]
        public virtual scene_type scene_type { get; set; }
        [ForeignKey("sensing_method_id")]
        [InverseProperty("photo")]
        public virtual sensing_method sensing_method { get; set; }
        [ForeignKey("sharpness_id")]
        [InverseProperty("photo")]
        public virtual sharpness sharpness { get; set; }
        [ForeignKey("vibration_reduction_id")]
        [InverseProperty("photo")]
        public virtual vibration_reduction vibration_reduction { get; set; }
        [ForeignKey("vignette_control_id")]
        [InverseProperty("photo")]
        public virtual vignette_control vignette_control { get; set; }
        [ForeignKey("vr_mode_id")]
        [InverseProperty("photo")]
        public virtual vr_mode vr_mode { get; set; }
        [ForeignKey("white_balance_id")]
        [InverseProperty("photo")]
        public virtual white_balance white_balance { get; set; }
    }
}
