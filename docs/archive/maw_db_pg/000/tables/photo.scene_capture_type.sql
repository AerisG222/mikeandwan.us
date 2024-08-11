CREATE TABLE IF NOT EXISTS photo.scene_capture_type (
    id INTEGER NOT NULL,
    name VARCHAR(50) NOT NULL,
    CONSTRAINT pk_photo_scene_capture_type PRIMARY KEY (id),
    CONSTRAINT uq_photo_scene_capture_type_name UNIQUE (name)
);

GRANT SELECT
   ON photo.scene_capture_type
   TO website;
