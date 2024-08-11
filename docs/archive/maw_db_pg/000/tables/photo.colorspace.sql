CREATE TABLE IF NOT EXISTS photo.colorspace (
    id SMALLSERIAL,
    name VARCHAR(15) NOT NULL,
    CONSTRAINT pk_photo_colorspace PRIMARY KEY (id),
    CONSTRAINT uq_photo_colorspace_name UNIQUE (name)
);

GRANT SELECT
   ON photo.colorspace
   TO website;
