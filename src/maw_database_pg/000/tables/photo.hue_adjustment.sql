CREATE TABLE IF NOT EXISTS photo.hue_adjustment (
    id SMALLSERIAL,
    name VARCHAR(50) NOT NULL,
    CONSTRAINT pk_photo_hue_adjustment PRIMARY KEY (id),
    CONSTRAINT uq_photo_hue_adjustment_name UNIQUE (name)
);

GRANT SELECT
   ON photo.hue_adjustment
   TO website;
