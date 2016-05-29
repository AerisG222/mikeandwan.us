CREATE TABLE IF NOT EXISTS photo.make (
    id SMALLSERIAL,
    name VARCHAR(50) NOT NULL,
    CONSTRAINT pk_photo_make PRIMARY KEY (id),
    CONSTRAINT uq_photo_make_name UNIQUE (name)
);

GRANT SELECT
   ON photo.make
   TO website;
