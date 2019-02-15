ALTER TABLE photo.category
    ADD COLUMN create_date TIMESTAMP,
    ADD COLUMN gps_latitude REAL,
    ADD COLUMN gps_longitude REAL,
    ADD COLUMN photo_count INT,
    ADD COLUMN total_size_xs INT,
    ADD COLUMN total_size_xs_sq INT,
    ADD COLUMN total_size_sm INT,
    ADD COLUMN total_size_md INT,
    ADD COLUMN total_size_lg INT,
    ADD COLUMN total_size_prt INT,
    ADD COLUMN total_size_src INT,
    ADD COLUMN teaser_photo_size INT,

    -- square teaser
    ADD COLUMN teaser_photo_sq_height SMALLINT,
    ADD COLUMN teaser_photo_sq_width SMALLINT,
    ADD COLUMN teaser_photo_sq_path VARCHAR(255),
    ADD COLUMN teaser_photo_sq_size INT;
