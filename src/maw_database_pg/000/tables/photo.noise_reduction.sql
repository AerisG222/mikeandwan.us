 CREATE TABLE IF NOT EXISTS photo.noise_reduction (
    id SMALLSERIAL,
    name VARCHAR(50) NOT NULL,
    CONSTRAINT pk_photo_noise_reduction PRIMARY KEY (id),
    CONSTRAINT uq_photo_noise_reduction_name UNIQUE (name)
);

GRANT SELECT
   ON photo.noise_reduction
   TO website;
