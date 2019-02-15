ALTER TABLE video.video
    ADD COLUMN create_date TIMESTAMP,
    ADD COLUMN gps_latitude REAL,
    ADD COLUMN gps_longitude REAL,

    -- add size to existing videos
    ADD COLUMN thumb_size INT,
    ADD COLUMN scaled_size INT,
    ADD COLUMN full_size INT,
    ADD COLUMN raw_height SMALLINT,
    ADD COLUMN raw_width SMALLINT,
    ADD COLUMN raw_size INT,

    -- square thumbnails
    ADD COLUMN thumb_sq_height SMALLINT,
    ADD COLUMN thumb_sq_width SMALLINT,
    ADD COLUMN thumb_sq_path VARCHAR(255),
    ADD COLUMN thumb_sq_size INT;
