CREATE TABLE IF NOT EXISTS photo.gps_latitude_ref (
    id VARCHAR(2) NOT NULL,
    name VARCHAR(10) NOT NULL,
    CONSTRAINT pk_photo_gps_latitude_ref PRIMARY KEY (id),
    CONSTRAINT uq_photo_gps_latitude_ref_name UNIQUE (name)
);

GRANT SELECT
   ON photo.gps_latitude_ref
   TO website;
