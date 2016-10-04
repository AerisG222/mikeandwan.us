CREATE TABLE IF NOT EXISTS photo.model (
    id SMALLSERIAL,
    name VARCHAR(50) NOT NULL,
    CONSTRAINT pk_photo_model PRIMARY KEY (id),
    CONSTRAINT uq_photo_model_name UNIQUE (name)
);

GRANT SELECT
   ON photo.model
   TO website;
