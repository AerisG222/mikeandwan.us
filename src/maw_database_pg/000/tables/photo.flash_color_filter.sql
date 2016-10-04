CREATE TABLE IF NOT EXISTS photo.flash_color_filter (
    id SMALLSERIAL,
    name VARCHAR(10) NOT NULL,
    CONSTRAINT pk_photo_flash_color_filter PRIMARY KEY (id),
    CONSTRAINT uq_photo_flash_color_filter_name UNIQUE (name)
);

GRANT SELECT
   ON photo.flash_color_filter
   TO website;
