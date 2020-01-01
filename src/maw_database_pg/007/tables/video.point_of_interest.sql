CREATE TABLE IF NOT EXISTS video.point_of_interest (
    video_id SMALLINT NOT NULL,
    poi_type TEXT NOT NULL,
    poi_name TEXT NOT NULL,

    CONSTRAINT pk_video_point_of_interest PRIMARY KEY (video_id, poi_type),

    CONSTRAINT fk_point_of_interest_video FOREIGN KEY (video_id) REFERENCES video.video(id)
);

GRANT SELECT, INSERT
   ON video.point_of_interest
   TO website;
