CREATE TABLE IF NOT EXISTS photo.orientation (
    id INTEGER NOT NULL,
    name VARCHAR(50) NOT NULL,
    CONSTRAINT pk_photo_orientation PRIMARY KEY (id),
    CONSTRAINT uq_photo_orientation_name UNIQUE (name)
);

GRANT SELECT
   ON photo.orientation
   TO website;
