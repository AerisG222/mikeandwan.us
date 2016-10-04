CREATE TABLE IF NOT EXISTS photo.gps_longitude_ref (
    id VARCHAR(2) NOT NULL,
    name VARCHAR(10) NOT NULL,
    CONSTRAINT pk_photo_gps_longitude_ref PRIMARY KEY (id),
    CONSTRAINT uq_photo_gps_longitude_ref_name UNIQUE (name)
);

GRANT SELECT
   ON photo.gps_longitude_ref
   TO website;
