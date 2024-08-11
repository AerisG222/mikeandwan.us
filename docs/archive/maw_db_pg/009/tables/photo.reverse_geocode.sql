ALTER TABLE photo.reverse_geocode
  ADD COLUMN is_override BOOLEAN NOT NULL DEFAULT FALSE;

ALTER TABLE photo.reverse_geocode
 DROP CONSTRAINT pk_photo_reverse_geocode;

ALTER TABLE photo.reverse_geocode
  ADD CONSTRAINT pk_photo_reverse_geocode
         PRIMARY KEY (photo_id, is_override);

GRANT DELETE
   ON photo.reverse_geocode
   TO website;
