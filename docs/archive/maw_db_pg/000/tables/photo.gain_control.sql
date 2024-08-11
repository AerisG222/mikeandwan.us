CREATE TABLE IF NOT EXISTS photo.gain_control (
    id INTEGER NOT NULL,
    name VARCHAR(20) NOT NULL,
    CONSTRAINT pk_photo_gain_control PRIMARY KEY (id),
    CONSTRAINT uq_photo_gain_control_name UNIQUE (name)
);

GRANT SELECT
   ON photo.gain_control
   TO website;
