CREATE TABLE IF NOT EXISTS photo.af_area_mode (
    id SMALLSERIAL,
    name VARCHAR(40) NOT NULL,
    CONSTRAINT pk_photo_af_area_mode PRIMARY KEY (id),
    CONSTRAINT uq_photo_af_area_mode_name UNIQUE (name)
);

GRANT SELECT
   ON photo.af_area_mode
   TO website;
