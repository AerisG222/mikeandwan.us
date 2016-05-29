CREATE TABLE IF NOT EXISTS photo.lens (
    id SMALLSERIAL,
    name VARCHAR(100) NOT NULL,
    CONSTRAINT pk_photo_lens PRIMARY KEY (id),
    CONSTRAINT uq_photo_lens_name UNIQUE (name)
);

GRANT SELECT
   ON photo.lens
   TO website;
