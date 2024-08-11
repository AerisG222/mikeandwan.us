CREATE TABLE IF NOT EXISTS photo.gps_measure_mode (
    id VARCHAR(2) NOT NULL,
    name VARCHAR(30) NOT NULL,
    CONSTRAINT pk_photo_gps_measure_mode PRIMARY KEY (id),
    CONSTRAINT uq_photo_gps_measure_mode_name UNIQUE (name)
);

GRANT SELECT
   ON photo.gps_measure_mode
   TO website;
