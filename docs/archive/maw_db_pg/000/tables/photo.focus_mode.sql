 CREATE TABLE IF NOT EXISTS photo.focus_mode (
    id SMALLSERIAL,
    name VARCHAR(50) NOT NULL,
    CONSTRAINT pk_photo_focus_mode PRIMARY KEY (id),
    CONSTRAINT uq_photo_focus_mode_name UNIQUE (name)
);

GRANT SELECT
   ON photo.focus_mode
   TO website;
