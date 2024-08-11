CREATE TABLE IF NOT EXISTS photo.exposure_mode (
    id INTEGER NOT NULL,
    name VARCHAR(50) NOT NULL,
    CONSTRAINT pk_photo_exposure_mode PRIMARY KEY (id),
    CONSTRAINT uq_photo_exposure_mode_name UNIQUE (name)
);

GRANT SELECT
   ON photo.exposure_mode
   TO website;
