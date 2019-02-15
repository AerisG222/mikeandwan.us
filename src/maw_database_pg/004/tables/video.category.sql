ALTER TABLE video.category
    ADD COLUMN create_date TIMESTAMP,
    ADD COLUMN gps_latitude REAL,
    ADD COLUMN gps_longitude REAL,
    ADD COLUMN video_count INT,
    ADD COLUMN total_duration INT,
    ADD COLUMN total_size_thumb INT,
    ADD COLUMN total_size_thumb_sq INT,
    ADD COLUMN total_size_scaled INT,
    ADD COLUMN total_size_full INT,
    ADD COLUMN total_size_raw INT,
    ADD COLUMN teaser_image_size INT,

    -- square teaser
    ADD COLUMN teaser_image_sq_height SMALLINT,
    ADD COLUMN teaser_image_sq_width SMALLINT,
    ADD COLUMN teaser_image_sq_path VARCHAR(255),
    ADD COLUMN teaser_image_sq_size INT;
