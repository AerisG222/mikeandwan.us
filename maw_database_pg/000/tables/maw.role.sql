CREATE TABLE IF NOT EXISTS maw.role (
    id SMALLSERIAL,
    name VARCHAR(30) NOT NULL,
    description VARCHAR(255),
    CONSTRAINT pk_maw_role PRIMARY KEY (id),
    CONSTRAINT uq_maw_role_name UNIQUE (name)
);

GRANT SELECT, INSERT, UPDATE, DELETE
   ON maw.role
   TO website;

GRANT USAGE 
   ON SEQUENCE maw.role_id_seq 
   TO website;
