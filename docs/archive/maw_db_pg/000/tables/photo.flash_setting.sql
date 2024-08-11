CREATE TABLE IF NOT EXISTS photo.flash_setting (
    id SMALLSERIAL,
    name VARCHAR(50) NOT NULL,
    CONSTRAINT pk_photo_flash_setting PRIMARY KEY (id),
    CONSTRAINT uq_photo_flash_setting_name UNIQUE (name)
);

GRANT SELECT
   ON photo.flash_setting
   TO website;
