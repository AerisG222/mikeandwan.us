CREATE TABLE IF NOT EXISTS photo.gps_altitude_ref (
    id SMALLINT NOT NULL,
    name VARCHAR(20) NOT NULL,
    CONSTRAINT pk_photo_gps_altitude_ref PRIMARY KEY (id),
    CONSTRAINT uq_photo_gps_altitude_ref_name UNIQUE (name)
);

GRANT SELECT
   ON photo.gps_altitude_ref
   TO website;
