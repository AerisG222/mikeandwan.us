CREATE TABLE IF NOT EXISTS photo.compression (
    id INTEGER NOT NULL,
    name VARCHAR(80) NOT NULL,
    CONSTRAINT pk_photo_compression PRIMARY KEY (id)
);

GRANT SELECT
   ON photo.compression
   TO website;
