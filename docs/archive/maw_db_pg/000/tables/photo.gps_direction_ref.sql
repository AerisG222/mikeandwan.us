CREATE TABLE IF NOT EXISTS photo.gps_direction_ref (
    id VARCHAR(2) NOT NULL,
    name VARCHAR(20) NOT NULL,
    CONSTRAINT pk_photo_gps_direction_ref PRIMARY KEY (id),
    CONSTRAINT uq_photo_gps_direction_ref_name UNIQUE (name)
);

GRANT SELECT
   ON photo.gps_direction_ref
   TO website;
