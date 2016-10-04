CREATE TABLE IF NOT EXISTS photo.auto_focus (
    id SMALLINT NOT NULL,
    name VARCHAR(10) NOT NULL,
    CONSTRAINT pk_photo_auto_focus PRIMARY KEY (id),
    CONSTRAINT uq_photo_auto_focus_name UNIQUE (name)
);

GRANT SELECT
   ON photo.auto_focus
   TO website;
