CREATE TABLE IF NOT EXISTS photo.flash (
    id INTEGER NOT NULL,
    name VARCHAR(60) NOT NULL,
    CONSTRAINT pk_photo_flash PRIMARY KEY (id),
    CONSTRAINT uq_photo_flash_name UNIQUE (name)
);

GRANT SELECT
   ON photo.flash
   TO website;
