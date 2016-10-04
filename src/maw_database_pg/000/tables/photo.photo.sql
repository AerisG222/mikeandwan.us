CREATE TABLE IF NOT EXISTS photo.photo (
    id SERIAL,
    category_id SMALLINT NOT NULL,
    is_private BOOLEAN NOT NULL,

    -- scaled images
    xs_height SMALLINT NOT NULL,
    xs_width SMALLINT NOT NULL,
    xs_path VARCHAR(255) NOT NULL,
    sm_height SMALLINT NOT NULL,
    sm_width SMALLINT NOT NULL,
    sm_path VARCHAR(255) NOT NULL,
    md_height SMALLINT NOT NULL,
    md_width SMALLINT NOT NULL,
    md_path VARCHAR(255) NOT NULL,
    lg_height SMALLINT NOT NULL,
    lg_width SMALLINT NOT NULL,
    lg_path VARCHAR(255) NOT NULL,
    prt_height SMALLINT NOT NULL,
    prt_width SMALLINT NOT NULL,
    prt_path VARCHAR(255) NOT NULL,
    src_height SMALLINT NOT NULL,
    src_width SMALLINT NOT NULL,
    src_path VARCHAR(255) NOT NULL,

    -- exif
    bits_per_sample SMALLINT,
    compression_id INTEGER,
    contrast_id INTEGER,
    create_date TIMESTAMP,
    digital_zoom_ratio REAL,
    exposure_compensation VARCHAR(10),
    exposure_mode_id INTEGER,
    exposure_program_id INTEGER,
    exposure_time VARCHAR(10),
    f_number NUMERIC(3, 1),
    flash_id INTEGER,
    focal_length REAL,
    focal_length_in_35_mm_format REAL,
    gain_control_id INTEGER,
    gps_altitude REAL,
    gps_altitude_ref_id SMALLINT,
    gps_date_time_stamp TIMESTAMP,
    gps_direction REAL,
    gps_direction_ref_id VARCHAR(2),
    gps_latitude REAL,
    gps_latitude_ref_id VARCHAR(2),
    gps_longitude REAL,
    gps_longitude_ref_id VARCHAR(2),
    gps_measure_mode_id VARCHAR(2),
    gps_satellites VARCHAR(150),
    gps_status_id VARCHAR(2),
    gps_version_id VARCHAR(16),
    iso INTEGER,
    light_source_id INTEGER,
    make_id SMALLINT,
    metering_mode_id INTEGER,
    model_id SMALLINT,
    orientation_id INTEGER,
    saturation_id SMALLINT,
    scene_capture_type_id INTEGER,
    scene_type_id INTEGER,
    sensing_method_id INTEGER,
    sharpness_id INTEGER,

    -- nikon
    af_area_mode_id SMALLINT,
    af_point_id SMALLINT,
    active_d_lighting_id SMALLINT,
    colorspace_id SMALLINT,
    exposure_difference REAL,
    flash_color_filter_id SMALLINT,
    flash_compensation VARCHAR(10),
    flash_control_mode SMALLINT,
    flash_exposure_compensation VARCHAR(10),
    flash_focal_length SMALLINT,
    flash_mode_id SMALLINT,
    flash_setting_id  SMALLINT,
    flash_type_id SMALLINT,
    focus_distance REAL,
    focus_mode_id SMALLINT,
    focus_position SMALLINT,
    high_iso_noise_reduction_id SMALLINT,
    hue_adjustment_id SMALLINT,
    noise_reduction_id SMALLINT,
    picture_control_name_id SMALLINT,
    primary_af_point VARCHAR(20),
    vibration_reduction_id SMALLINT,
    vignette_control_id SMALLINT,
    vr_mode_id SMALLINT,
    white_balance_id SMALLINT,

    -- composite
    aperture NUMERIC(3, 1),
    auto_focus_id SMALLINT,
    depth_of_field VARCHAR(50),
    field_of_view VARCHAR(50),
    hyperfocal_distance REAL,
    lens_id SMALLINT,
    light_value REAL,
    scale_factor_35_efl REAL,
    shutter_speed VARCHAR(50),

    -- processing info
    raw_conversion_mode_id SMALLINT,
    sigmoidal_contrast_adjustment REAL,
    saturation_adjustment REAL,
    compression_quality SMALLINT,

    -- backup info
    aws_glacier_vault_id SMALLINT,
    aws_archive_id CHAR(138),
    aws_treehash CHAR(64),

    CONSTRAINT pk_photo_photo PRIMARY KEY (id),

    CONSTRAINT fk_photo_photo_category FOREIGN KEY (category_id) REFERENCES photo.category(id),

    -- exif references
    CONSTRAINT fk_photo_photo_compression FOREIGN KEY (compression_id) REFERENCES photo.compression(id),
    CONSTRAINT fk_photo_photo_contrast FOREIGN KEY (contrast_id) REFERENCES photo.contrast(id),
    CONSTRAINT fk_photo_photo_exposure_mode FOREIGN KEY (exposure_mode_id) REFERENCES photo.exposure_mode(id),
    CONSTRAINT fk_photo_photo_exposure_program FOREIGN KEY (exposure_program_id) REFERENCES photo.exposure_program(id),
    CONSTRAINT fk_photo_photo_flash FOREIGN KEY (flash_id) REFERENCES photo.flash(id),
    CONSTRAINT fk_photo_photo_gain_control FOREIGN KEY (gain_control_id) REFERENCES photo.gain_control(id),
    CONSTRAINT fk_photo_photo_gps_altitude_ref FOREIGN KEY (gps_altitude_ref_id) REFERENCES photo.gps_altitude_ref(id),
    CONSTRAINT fk_photo_photo_gps_direction_ref FOREIGN KEY (gps_direction_ref_id) REFERENCES photo.gps_direction_ref(id),
    CONSTRAINT fk_photo_photo_gps_latitude_ref FOREIGN KEY (gps_latitude_ref_id) REFERENCES photo.gps_latitude_ref(id),
    CONSTRAINT fk_photo_photo_gps_longitude_ref FOREIGN KEY (gps_longitude_ref_id) REFERENCES photo.gps_longitude_ref(id),
    CONSTRAINT fk_photo_photo_gps_measure_mode FOREIGN KEY (gps_measure_mode_id) REFERENCES photo.gps_measure_mode(id),
    CONSTRAINT fk_photo_photo_gps_status FOREIGN KEY (gps_status_id) REFERENCES photo.gps_status(id),
    CONSTRAINT fk_photo_photo_light_source FOREIGN KEY (light_source_id) REFERENCES photo.light_source(id),
    CONSTRAINT fk_photo_photo_make FOREIGN KEY (make_id) REFERENCES photo.make(id),
    CONSTRAINT fk_photo_photo_metering_mode FOREIGN KEY (metering_mode_id) REFERENCES photo.metering_mode(id),
    CONSTRAINT fk_photo_photo_model FOREIGN KEY (model_id) REFERENCES photo.model(id),
    CONSTRAINT fk_photo_photo_orientation FOREIGN KEY (orientation_id) REFERENCES photo.orientation(id),
    CONSTRAINT fk_photo_photo_saturation FOREIGN KEY (saturation_id) REFERENCES photo.saturation(id),
    CONSTRAINT fk_photo_photo_scene_capture_type FOREIGN KEY (scene_capture_type_id) REFERENCES photo.scene_capture_type(id),
    CONSTRAINT fk_photo_photo_scene_type FOREIGN KEY (scene_type_id) REFERENCES photo.scene_type(id),
    CONSTRAINT fk_photo_photo_sensing_method FOREIGN KEY (sensing_method_id) REFERENCES photo.sensing_method(id),
    CONSTRAINT fk_photo_photo_sharpness FOREIGN KEY (sharpness_id) REFERENCES photo.sharpness(id),

    -- nikon references
    CONSTRAINT fk_photo_photo_af_area_mode FOREIGN KEY (af_area_mode_id) REFERENCES photo.af_area_mode(id),
    CONSTRAINT fk_photo_photo_af_point FOREIGN KEY (af_point_id) REFERENCES photo.af_point(id),
    CONSTRAINT fk_photo_photo_active_d_lighting FOREIGN KEY (active_d_lighting_id) REFERENCES photo.active_d_lighting(id),
    CONSTRAINT fk_photo_photo_colorspace FOREIGN KEY (colorspace_id) REFERENCES photo.colorspace(id),
    CONSTRAINT fk_photo_photo_flash_color_filter FOREIGN KEY (flash_color_filter_id) REFERENCES photo.flash_color_filter(id),
    CONSTRAINT fk_photo_photo_flash_mode FOREIGN KEY (flash_mode_id) REFERENCES photo.flash_mode(id),
    CONSTRAINT fk_photo_photo_flash_setting FOREIGN KEY (flash_setting_id) REFERENCES photo.flash_setting(id),
    CONSTRAINT fk_photo_photo_flash_type FOREIGN KEY (flash_type_id) REFERENCES photo.flash_type(id),
    CONSTRAINT fk_photo_photo_focus_mode FOREIGN KEY (focus_mode_id) REFERENCES photo.focus_mode(id),
    CONSTRAINT fk_photo_photo_high_iso_noise_reduction FOREIGN KEY (high_iso_noise_reduction_id) REFERENCES photo.high_iso_noise_reduction(id),
    CONSTRAINT fk_photo_photo_hue_adjustment FOREIGN KEY (hue_adjustment_id) REFERENCES photo.hue_adjustment(id),
    CONSTRAINT fk_photo_photo_noise_reduction FOREIGN KEY (noise_reduction_id) REFERENCES photo.noise_reduction(id),
    CONSTRAINT fk_photo_photo_picture_control_name FOREIGN KEY (picture_control_name_id) REFERENCES photo.picture_control_name(id),
    CONSTRAINT fk_photo_photo_vibration_reduction FOREIGN KEY (vibration_reduction_id) REFERENCES photo.vibration_reduction(id),
    CONSTRAINT fk_photo_photo_vignette_control FOREIGN KEY (vignette_control_id) REFERENCES photo.vignette_control(id),
    CONSTRAINT fk_photo_photo_vr_mode FOREIGN KEY (vr_mode_id) REFERENCES photo.vr_mode(id),
    CONSTRAINT fk_photo_photo_white_balance FOREIGN KEY (white_balance_id) REFERENCES photo.white_balance(id),

    -- composite references
    CONSTRAINT fk_photo_photo_auto_focus FOREIGN KEY (auto_focus_id) REFERENCES photo.auto_focus(id),
    CONSTRAINT fk_photo_photo_lens FOREIGN KEY (lens_id) REFERENCES photo.lens(id),

    -- processing
    CONSTRAINT fk_photo_photo_raw_conversion_mode FOREIGN KEY (raw_conversion_mode_id) REFERENCES photo.raw_conversion_mode(id),

    -- backup
    CONSTRAINT fk_photo_aws_glacier_vault FOREIGN KEY (aws_glacier_vault_id) REFERENCES aws.glacier_vault(id)
);

DO
$$
BEGIN
    IF NOT EXISTS (SELECT 1
                     FROM pg_catalog.pg_indexes
                    WHERE schemaname = 'photo'
                      AND tablename = 'photo'
                      AND indexname = 'ix_photo_photo_category_id_is_private') THEN

        CREATE INDEX ix_photo_photo_category_id_is_private
            ON photo.photo(category_id, is_private);

    END IF;
END
$$;

GRANT SELECT
   ON photo.photo
   TO website;
