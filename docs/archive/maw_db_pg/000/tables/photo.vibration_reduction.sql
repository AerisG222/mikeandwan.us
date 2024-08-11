CREATE TABLE IF NOT EXISTS photo.vibration_reduction (
    id SMALLSERIAL,
    name VARCHAR(10) NOT NULL,
    CONSTRAINT pk_photo_vibration_reduction PRIMARY KEY (id),
    CONSTRAINT uq_photo_vibration_reduction_name UNIQUE (name)
);

GRANT SELECT
   ON photo.vibration_reduction
   TO website;
