CREATE TABLE IF NOT EXISTS photo.contrast (
    id INTEGER NOT NULL,
    name VARCHAR(10) NOT NULL,
    CONSTRAINT pk_photo_contrast PRIMARY KEY (id),
    CONSTRAINT uq_photo_contrast_name UNIQUE (name)
);

GRANT SELECT
   ON photo.contrast
   TO website;
