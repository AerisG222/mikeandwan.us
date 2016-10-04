CREATE TABLE IF NOT EXISTS photo.picture_control_name (
    id SMALLSERIAL,
    name VARCHAR(50) NOT NULL,
    CONSTRAINT pk_photo_picture_control_name PRIMARY KEY (id),
    CONSTRAINT uq_photo_picture_control_name_name UNIQUE (name)
);

GRANT SELECT
   ON photo.picture_control_name
   TO website;
