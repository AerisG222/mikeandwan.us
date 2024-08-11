CREATE TABLE IF NOT EXISTS photo.exposure_program (
    id INTEGER NOT NULL,
    name VARCHAR(50) NOT NULL,
    CONSTRAINT pk_photo_exposure_program PRIMARY KEY (id),
    CONSTRAINT uq_photo_exposure_program_name UNIQUE (name)
);

GRANT SELECT
   ON photo.exposure_program
   TO website;
