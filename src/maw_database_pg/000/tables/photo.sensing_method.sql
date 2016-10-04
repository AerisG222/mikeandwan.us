CREATE TABLE IF NOT EXISTS photo.sensing_method (
    id INTEGER NOT NULL,
    name VARCHAR(50) NOT NULL,
    CONSTRAINT pk_photo_sensing_method PRIMARY KEY (id),
    CONSTRAINT uq_photo_sensing_method_name UNIQUE (name)
);

GRANT SELECT
   ON photo.sensing_method
   TO website;
