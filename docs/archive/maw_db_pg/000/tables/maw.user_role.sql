CREATE TABLE IF NOT EXISTS maw.user_role (
    user_id SMALLINT NOT NULL,
    role_id SMALLINT NOT NULL,
    CONSTRAINT pk_maw_user_role PRIMARY KEY (user_id, role_id),
    CONSTRAINT fk_maw_user_role_role FOREIGN KEY (role_id) REFERENCES maw.role(id),
    CONSTRAINT fk_maw_user_role_user FOREIGN KEY (user_id) REFERENCES maw.user(id)
);

GRANT SELECT, INSERT, DELETE
   ON maw.user_role
   TO website;
