CREATE TABLE IF NOT EXISTS media.type (
    id UUID NOT NULL,
    code VARCHAR(2) NOT NULL,
    name TEXT NOT NULL,

    CONSTRAINT pk_media_type PRIMARY KEY (id)
);

GRANT SELECT
   ON media.type
   TO website;
