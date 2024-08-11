CREATE TABLE IF NOT EXISTS photo.light_source (
    id INTEGER NOT NULL,
    name VARCHAR(30) NOT NULL,
    CONSTRAINT pk_photo_light_source PRIMARY KEY (id),
    CONSTRAINT uq_photo_light_source_name UNIQUE (name)
);

GRANT SELECT
   ON photo.light_source
   TO website;
