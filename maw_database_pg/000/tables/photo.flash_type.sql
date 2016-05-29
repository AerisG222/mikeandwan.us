CREATE TABLE IF NOT EXISTS photo.flash_type (
    id SMALLSERIAL,
    name VARCHAR(50) NOT NULL,
    CONSTRAINT pk_photo_flash_type PRIMARY KEY (id),
    CONSTRAINT uq_photo_flash_type_name UNIQUE (name)
);

GRANT SELECT
   ON photo.flash_type
   TO website;
