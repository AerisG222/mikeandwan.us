CREATE TABLE IF NOT EXISTS photo.sharpness (
    id INTEGER NOT NULL,
    name VARCHAR(50) NOT NULL,
    CONSTRAINT pk_photo_sharpness PRIMARY KEY (id),
    CONSTRAINT uq_photo_sharpness_name UNIQUE (name)
);

GRANT SELECT
   ON photo.sharpness
   TO website;
