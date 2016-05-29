CREATE TABLE IF NOT EXISTS maw.state (
    id SMALLSERIAL,
    code CHAR(2) NOT NULL,
    name VARCHAR(30) NOT NULL,
    CONSTRAINT pk_maw_state PRIMARY KEY (id),
    CONSTRAINT uq_maw_state_code UNIQUE (code),
    CONSTRAINT uq_maw_state_name UNIQUE (name)
);

GRANT SELECT
   ON maw.state
   TO website;
