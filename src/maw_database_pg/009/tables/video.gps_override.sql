CREATE TABLE IF NOT EXISTS video.gps_override (
    video_id INTEGER NOT NULL,
    latitude REAL NOT NULL,
    longitude REAL NOT NULL,
    user_id SMALLINT NOT NULL,
    updated_time TIMESTAMP NOT NULL,
    has_been_reverse_geocoded BOOLEAN NOT NULL,

    CONSTRAINT pk_video_gps_override PRIMARY KEY (video_id),

    CONSTRAINT fk_gps_override_video FOREIGN KEY (video_id) REFERENCES video.video(id),
    CONSTRAINT fk_gps_override_video_user FOREIGN KEY (user_id) REFERENCES maw.user(id)
);

GRANT SELECT, INSERT, UPDATE, DELETE
   ON video.gps_override
   TO website;
