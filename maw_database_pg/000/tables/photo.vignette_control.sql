CREATE TABLE IF NOT EXISTS photo.vignette_control (
    id SMALLSERIAL,
    name VARCHAR(10) NOT NULL,
    CONSTRAINT pk_photo_vignette_control PRIMARY KEY (id),
    CONSTRAINT uq_photo_vignette_control_name UNIQUE (name)
);

GRANT SELECT
   ON photo.vignette_control
   TO website;
