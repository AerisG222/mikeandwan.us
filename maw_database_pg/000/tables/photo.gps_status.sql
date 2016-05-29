CREATE TABLE IF NOT EXISTS photo.gps_status (
    id VARCHAR(2) NOT NULL,
    name VARCHAR(20) NOT NULL,
    CONSTRAINT pk_photo_gps_status PRIMARY KEY (id),
    CONSTRAINT uq_photo_gps_status_name UNIQUE (name)
);

GRANT SELECT
   ON photo.gps_status
   TO website;
