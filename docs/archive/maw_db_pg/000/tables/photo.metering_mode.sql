CREATE TABLE IF NOT EXISTS photo.metering_mode (
    id INTEGER NOT NULL,
    name VARCHAR(50) NOT NULL,
    CONSTRAINT pk_photo_metering_mode PRIMARY KEY (id),
    CONSTRAINT uq_photo_metering_mode_name UNIQUE (name)
);

GRANT SELECT
   ON photo.metering_mode
   TO website;
