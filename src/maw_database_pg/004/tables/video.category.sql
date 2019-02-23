ALTER TABLE video.category
    ADD COLUMN create_date TIMESTAMP,
    ADD COLUMN gps_latitude REAL,
    ADD COLUMN gps_longitude REAL,
    ADD COLUMN gps_latitude_ref_id VARCHAR(2),
    ADD COLUMN gps_longitude_ref_id VARCHAR(2),
    ADD COLUMN video_count INT,
    ADD COLUMN total_duration INT,
    ADD COLUMN total_size_thumb BIGINT,
    ADD COLUMN total_size_thumb_sq BIGINT,
    ADD COLUMN total_size_scaled BIGINT,
    ADD COLUMN total_size_full BIGINT,
    ADD COLUMN total_size_raw BIGINT,
    ADD COLUMN teaser_image_size INT,

    -- square teaser
    ADD COLUMN teaser_image_sq_height SMALLINT,
    ADD COLUMN teaser_image_sq_width SMALLINT,
    ADD COLUMN teaser_image_sq_path VARCHAR(255),
    ADD COLUMN teaser_image_sq_size INT;


ALTER TABLE video.category
  ADD CONSTRAINT fk_video_video_category_gps_latitude_ref FOREIGN KEY (gps_latitude_ref_id) REFERENCES photo.gps_latitude_ref(id);

ALTER TABLE video.category
  ADD CONSTRAINT fk_video_video_category_gps_longitude_ref FOREIGN KEY (gps_longitude_ref_id) REFERENCES photo.gps_longitude_ref(id);
