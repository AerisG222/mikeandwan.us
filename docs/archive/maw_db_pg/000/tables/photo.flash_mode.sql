CREATE TABLE IF NOT EXISTS photo.flash_mode (
    id SMALLSERIAL,
    name VARCHAR(30) NOT NULL,
    CONSTRAINT pk_photo_flash_mode PRIMARY KEY (id),
    CONSTRAINT uq_photo_flash_mode_name UNIQUE (name)
);

GRANT SELECT
   ON photo.flash_mode
   TO website;
