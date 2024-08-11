ALTER TABLE photo.photo
    -- processing info
    DROP COLUMN raw_conversion_mode_id,
    DROP COLUMN sigmoidal_contrast_adjustment,
    DROP COLUMN saturation_adjustment,
    DROP COLUMN compression_quality;
