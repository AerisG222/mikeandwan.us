CREATE TABLE IF NOT EXISTS photo.af_point (
    id SMALLSERIAL,
    name VARCHAR(20) NOT NULL,
    CONSTRAINT pk_photo_af_point PRIMARY KEY (id),
    CONSTRAINT uq_photo_af_point_name UNIQUE (name)
);

GRANT SELECT
   ON photo.af_point
   TO website;
