CREATE TABLE IF NOT EXISTS photo.active_d_lighting (
    id SMALLSERIAL,
    name VARCHAR(20) NOT NULL,
    CONSTRAINT pk_photo_active_d_lighting PRIMARY KEY (id),
    CONSTRAINT uq_photo_active_d_lighting_name UNIQUE (name)
);

GRANT SELECT
   ON photo.active_d_lighting
   TO website;
