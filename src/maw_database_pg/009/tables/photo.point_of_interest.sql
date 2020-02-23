ALTER TABLE photo.point_of_interest
  ADD COLUMN is_override BOOLEAN NOT NULL DEFAULT FALSE;

ALTER TABLE photo.point_of_interest
 DROP CONSTRAINT pk_photo_point_of_interest;

ALTER TABLE photo.point_of_interest
  ADD CONSTRAINT pk_photo_point_of_interest
         PRIMARY KEY (photo_id, poi_type, is_override);

GRANT DELETE
   ON photo.point_of_interest
   TO website;
