CREATE TABLE IF NOT EXISTS photo.high_iso_noise_reduction (
    id SMALLSERIAL,
    name VARCHAR(20) NOT NULL,
    CONSTRAINT pk_photo_high_iso_noise_reduction PRIMARY KEY (id),
    CONSTRAINT uq_photo_high_iso_noise_reduction_name UNIQUE (name)
);

GRANT SELECT
   ON photo.high_iso_noise_reduction
   TO website;
