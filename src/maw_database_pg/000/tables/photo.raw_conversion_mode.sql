CREATE TABLE IF NOT EXISTS photo.raw_conversion_mode (
    id SMALLINT NOT NULL,
    name VARCHAR(20) NOT NULL,
    CONSTRAINT pk_photo_raw_conversion_mode PRIMARY KEY (id),
    CONSTRAINT uq_photo_raw_conversion_mode_name UNIQUE (name)
);

GRANT SELECT
   ON photo.raw_conversion_mode
   TO website;
