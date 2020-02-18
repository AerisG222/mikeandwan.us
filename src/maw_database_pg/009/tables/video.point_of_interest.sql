ALTER TABLE video.point_of_interest
  ADD COLUMN is_override BOOLEAN NOT NULL DEFAULT FALSE;

ALTER TABLE video.point_of_interest
 DROP CONSTRAINT pk_video_point_of_interest;

ALTER TABLE video.point_of_interest
  ADD CONSTRAINT pk_video_point_of_interest
         PRIMARY KEY (video_id, poi_type, is_override);
