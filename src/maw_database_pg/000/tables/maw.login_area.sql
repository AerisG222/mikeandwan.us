CREATE TABLE IF NOT EXISTS maw.login_area (
    id SMALLINT NOT NULL,
    name VARCHAR(30) NOT NULL,
    CONSTRAINT pk_maw_login_area PRIMARY KEY (id),
    CONSTRAINT uq_maw_login_area_name UNIQUE (name)
);

GRANT SELECT
   ON maw.login_area
   TO website;
