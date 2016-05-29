CREATE TABLE IF NOT EXISTS photo.vr_mode (
    id SMALLSERIAL,
    name VARCHAR(20) NOT NULL,
    CONSTRAINT pk_photo_vr_mode PRIMARY KEY (id),
    CONSTRAINT uq_photo_vr_mode_name UNIQUE (name)
);

GRANT SELECT
   ON photo.vr_mode
   TO website;
