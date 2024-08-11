CREATE TABLE IF NOT EXISTS maw.login_activity_type (
    id SMALLINT NOT NULL,
    name VARCHAR(30) NOT NULL,
    CONSTRAINT pk_maw_login_activity_type PRIMARY KEY (id),
    CONSTRAINT uq_maw_login_activity_type_name UNIQUE (name)
);

GRANT SELECT
   ON maw.login_activity_type
   TO website;
