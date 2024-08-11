CREATE TABLE IF NOT EXISTS maw.country (
    id SMALLSERIAL,
    code CHAR(2) NOT NULL,
    name VARCHAR(30) NOT NULL,
    CONSTRAINT pk_maw_country PRIMARY KEY (id),
    CONSTRAINT uq_maw_country_code UNIQUE (code),
    CONSTRAINT uq_maw_country_name UNIQUE (name)
);

GRANT SELECT
   ON maw.country
   TO website;
