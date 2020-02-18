ALTER TABLE video.reverse_geocode
  ADD COLUMN is_override BOOLEAN NOT NULL DEFAULT FALSE;

ALTER TABLE video.reverse_geocode
 DROP CONSTRAINT pk_video_reverse_geocode;

ALTER TABLE video.reverse_geocode
  ADD CONSTRAINT pk_video_reverse_geocode
         PRIMARY KEY (video_id, is_override);
