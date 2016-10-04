CREATE TABLE IF NOT EXISTS photo.saturation (
    id SMALLSERIAL,
    name VARCHAR(50) NOT NULL,
    CONSTRAINT pk_photo_saturation PRIMARY KEY (id),
    CONSTRAINT uq_photo_saturation_name UNIQUE (name)
);

GRANT SELECT
   ON photo.saturation
   TO website;
