DROP FUNCTION IF EXISTS photo.get_photo_metadata(BOOLEAN, INTEGER);

CREATE OR REPLACE FUNCTION photo.get_photo_metadata
(
    _roles TEXT[],
    _id INTEGER DEFAULT NULL
)
RETURNS TABLE
(
    bits_per_sample SMALLINT,
    compression VARCHAR(80),
    contrast VARCHAR(10),
    create_date TIMESTAMP,
    digital_zoom_ratio REAL,
    exposure_compensation VARCHAR(10),
    exposure_mode VARCHAR(50),
    exposure_program VARCHAR(50),
    exposure_time VARCHAR(10),
    f_number NUMERIC(3, 1),
    flash VARCHAR(60),
    focal_length REAL,
    focal_length_in_35_mm_format REAL,
    gain_control VARCHAR(20),
    gps_altitude REAL,
    gps_alititude_ref VARCHAR(20),
    gps_date_stamp TIMESTAMP,
    gps_direction REAL,
    gps_direction_ref VARCHAR(20),
    gps_latitude REAL,
    gps_latitude_ref VARCHAR(10),
    gps_longitude REAL,
    gps_longitude_ref VARCHAR(10),
    gps_measure_mode VARCHAR(30),
    gps_satellites VARCHAR(150),
    gps_status VARCHAR(20),
    gps_version_id VARCHAR(16),
    iso INTEGER,
    light_source VARCHAR(30),
    make VARCHAR(50),
    metering_mode VARCHAR(50),
    model VARCHAR(50),
    orientation VARCHAR(50),
    saturation VARCHAR(50),
    scene_capture_type VARCHAR(50),
    scene_type VARCHAR(50),
    sensing_method VARCHAR(50),
    sharpness VARCHAR(50),
    auto_focus_area_mode VARCHAR(10),
    auto_focus_point VARCHAR(30),
    active_d_lighting VARCHAR(20),
    colorspace VARCHAR(15),
    exposure_difference REAL,
    flash_color_filter VARCHAR(10),
    flash_compensation VARCHAR(10),
    flash_control_mode SMALLINT,
    flash_exposure_compensation VARCHAR(10),
    flash_focal_length SMALLINT,
    flash_mode VARCHAR(30),
    flash_setting VARCHAR(50),
    flash_type VARCHAR(60),
    focus_distance REAL,
    focus_mode VARCHAR(50),
    focus_position SMALLINT,
    high_iso_noise_reduction VARCHAR(20),
    hue_adjustment VARCHAR(50),
    noise_reduction VARCHAR(50),
    picture_control_name VARCHAR(50),
    primary_af_point VARCHAR(20),
    vr_mode VARCHAR(20),
    vibration_reduction VARCHAR(10),
    vignette_control VARCHAR(10),
    white_balance VARCHAR(50),
    aperture NUMERIC(3, 1),
    auto_focus VARCHAR(10),
    depth_of_field VARCHAR(50),
    field_of_view VARCHAR(50),
    hyperfocal_distance REAL,
    lens_id VARCHAR(100),
    light_value REAL,
    scale_factor_35_efl REAL,
    shutter_speed VARCHAR(50)
)
LANGUAGE SQL
AS $$

    SELECT p.bits_per_sample,
           cm.name AS compression,
           cn.name AS contrast,
           create_date,
           digital_zoom_ratio,
           exposure_compensation,
           em.name AS exposure_mode,
           ep.name AS exposure_program,
           exposure_time,
           f_number,
           f.name AS flash,
           focal_length,
           focal_length_in_35_mm_format,
           gc.name AS gain_control,
           gps_altitude,
           gpsar.name AS gps_alititude_ref,
           gps_date_time_stamp AS gps_date_stamp,
           gps_direction,
           gpsdr.name AS gps_direction_ref,
           gps_latitude,
           gpslatr.name AS gps_latitude_ref,
           gps_longitude,
           gpslngr.name AS gps_longitude_ref,
           gpsmm.name AS gps_measure_mode,
           gps_satellites,
           gpss.name AS gps_status,
           gps_version_id,
           iso,
           ls.name AS light_source,
           m.name AS make,
           mm.name AS metering_mode,
           md.name AS model,
           o.name AS orientation,
           s.name AS saturation,
           sct.name AS scene_capture_type,
           st.name AS scene_type,
           sm.name AS sensing_method,
           sh.name AS sharpness,
           afam.name AS auto_focus_area_mode,
           afp.name AS auto_focus_point,
           adl.name AS active_d_lighting,
           cs.name AS colorspace,
           exposure_difference,
           fcf.name AS flash_color_filter,
           flash_compensation,
           flash_control_mode,
           flash_exposure_compensation,
           flash_focal_length,
           fm.name AS flash_mode,
           fs.name AS flash_setting,
           ft.name AS flash_type,
           focus_distance,
           foc.name AS focus_mode,
           focus_position,
           hinr.name AS high_iso_noise_reduction,
           ha.name AS hue_adjustment,
           nr.name AS noise_reduction,
           pcn.name AS picture_control_name,
           primary_af_point,
           vrm.name AS vr_mode,
           vr.name AS vibration_reduction,
           vc.name AS vignette_control,
           wb.name AS white_balance,
           aperture,
           af.name AS auto_focus,
           depth_of_field,
           field_of_view,
           hyperfocal_distance,
           l.name AS lens_id,
           light_value,
           scale_factor_35_efl,
           shutter_speed
      FROM photo.photo p
     INNER JOIN photo.category_role cr ON p.category_id = cr.category_id
     INNER JOIN maw.role r ON cr.role_id = r.id
      LEFT OUTER JOIN photo.active_d_lighting adl ON adl.id = p.active_d_lighting_id
      LEFT OUTER JOIN photo.af_area_mode afam ON afam.id = p.af_area_mode_id
      LEFT OUTER JOIN photo.af_point afp ON afp.id = p.af_point_id
      LEFT OUTER JOIN photo.auto_focus af ON af.id = p.auto_focus_id
      LEFT OUTER JOIN photo.colorspace cs ON cs.id = p.colorspace_id
      LEFT OUTER JOIN photo.compression cm ON cm.id = p.compression_id
      LEFT OUTER JOIN photo.contrast cn ON cn.id = p.contrast_id
      LEFT OUTER JOIN photo.exposure_mode em ON em.id = p.exposure_mode_id
      LEFT OUTER JOIN photo.exposure_program ep ON ep.id = p.exposure_program_id
      LEFT OUTER JOIN photo.flash f ON f.id = p.flash_id
      LEFT OUTER JOIN photo.flash_color_filter fcf ON fcf.id = p.flash_color_filter_id
      LEFT OUTER JOIN photo.flash_mode fm ON fm.id = p.flash_mode_id
      LEFT OUTER JOIN photo.flash_setting fs ON fs.id = p.flash_setting_id
      LEFT OUTER JOIN photo.flash_type ft ON ft.id = p.flash_type_id
      LEFT OUTER JOIN photo.focus_mode foc ON foc.id = p.focus_mode_id
      LEFT OUTER JOIN photo.gain_control gc ON gc.id = p.gain_control_id
      LEFT OUTER JOIN photo.gps_altitude_ref gpsar ON gpsar.id = p.gps_altitude_ref_id
      LEFT OUTER JOIN photo.gps_direction_ref gpsdr ON gpsdr.id = p.gps_direction_ref_id
      LEFT OUTER JOIN photo.gps_latitude_ref gpslatr ON gpslatr.id = p.gps_latitude_ref_id
      LEFT OUTER JOIN photo.gps_longitude_ref gpslngr ON gpslngr.id = p.gps_longitude_ref_id
      LEFT OUTER JOIN photo.gps_measure_mode gpsmm ON gpsmm.id = p.gps_measure_mode_id
      LEFT OUTER JOIN photo.gps_status gpss ON gpss.id = p.gps_status_id
      LEFT OUTER JOIN photo.high_iso_noise_reduction hinr ON hinr.id = p.high_iso_noise_reduction_id
      LEFT OUTER JOIN photo.hue_adjustment ha ON ha.id = p.hue_adjustment_id
      LEFT OUTER JOIN photo.lens l ON l.id = p.lens_id
      LEFT OUTER JOIN photo.light_source ls ON ls.id = p.light_source_id
      LEFT OUTER JOIN photo.make m ON m.id = p.make_id
      LEFT OUTER JOIN photo.metering_mode mm ON mm.id = p.metering_mode_id
      LEFT OUTER JOIN photo.model md ON md.id = p.model_id
      LEFT OUTER JOIN photo.noise_reduction nr ON nr.id = p.noise_reduction_id
      LEFT OUTER JOIN photo.orientation o ON o.id = p.orientation_id
      LEFT OUTER JOIN photo.picture_control_name pcn ON pcn.id = p.picture_control_name_id
      LEFT OUTER JOIN photo.saturation s ON s.id = p.saturation_id
      LEFT OUTER JOIN photo.scene_capture_type sct ON sct.id = p.scene_capture_type_id
      LEFT OUTER JOIN photo.scene_type st ON st.id = p.scene_type_id
      LEFT OUTER JOIN photo.sensing_method sm ON sm.id = p.sensing_method_id
      LEFT OUTER JOIN photo.sharpness sh ON sh.id = p.sharpness_id
      LEFT OUTER JOIN photo.vibration_reduction vr ON vr.id = p.vibration_reduction_id
      LEFT OUTER JOIN photo.vignette_control vc ON vc.id = p.vignette_control_id
      LEFT OUTER JOIN photo.vr_mode vrm ON vrm.id = p.vr_mode_id
      LEFT OUTER JOIN photo.white_balance wb ON wb.id = p.white_balance_id
     WHERE r.name = ANY(_roles)
       AND p.id = _id;

$$;

GRANT EXECUTE
   ON FUNCTION photo.get_photo_metadata
   TO website;
