ALTER TABLE video.video
    ADD COLUMN create_date TIMESTAMP,
    ADD COLUMN gps_latitude REAL,
    ADD COLUMN gps_longitude REAL,
    ADD COLUMN gps_latitude_ref_id VARCHAR(2),
    ADD COLUMN gps_longitude_ref_id VARCHAR(2),

    -- add size to existing videos
    ADD COLUMN thumb_size INT,
    ADD COLUMN scaled_size BIGINT,
    ADD COLUMN full_size BIGINT,
    ADD COLUMN raw_height SMALLINT,
    ADD COLUMN raw_width SMALLINT,
    ADD COLUMN raw_size BIGINT,

    -- square thumbnails
    ADD COLUMN thumb_sq_height SMALLINT,
    ADD COLUMN thumb_sq_width SMALLINT,
    ADD COLUMN thumb_sq_path VARCHAR(255),
    ADD COLUMN thumb_sq_size INT,

    -- backup info
    ADD COLUMN aws_glacier_vault_id SMALLINT,
    ADD COLUMN aws_archive_id CHAR(138),
    ADD COLUMN aws_treehash CHAR(64);


ALTER TABLE video.video
  ADD CONSTRAINT fk_video_video_gps_latitude_ref FOREIGN KEY (gps_latitude_ref_id) REFERENCES photo.gps_latitude_ref(id);

ALTER TABLE video.video
  ADD CONSTRAINT fk_video_video_gps_longitude_ref FOREIGN KEY (gps_longitude_ref_id) REFERENCES photo.gps_longitude_ref(id);

ALTER TABLE video.video
  ADD CONSTRAINT fk_video_video_aws_glacier_vault FOREIGN KEY (aws_glacier_vault_id) REFERENCES aws.glacier_vault(id);
