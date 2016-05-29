CREATE TABLE IF NOT EXISTS maw.login_history (
    id SERIAL,
    user_id SMALLINT,
    username VARCHAR(30) NOT NULL,
    attempt_time TIMESTAMP NOT NULL,
    login_activity_type_id SMALLINT NOT NULL,
    login_area_id SMALLINT NOT NULL,
    CONSTRAINT pk_maw_login_history PRIMARY KEY (id),
    CONSTRAINT fk_maw_login_history_login_activity_type FOREIGN KEY (login_activity_type_id) REFERENCES maw.login_activity_type(id),
    CONSTRAINT fk_maw_login_history_login_area FOREIGN KEY (login_area_id) REFERENCES maw.login_area(id),
    CONSTRAINT fk_maw_login_history_user FOREIGN KEY (user_id) REFERENCES maw.user(id)
);

DO
$$
BEGIN
    IF NOT EXISTS (SELECT 1
                     FROM pg_catalog.pg_indexes
                    WHERE schemaname = 'maw'
                      AND tablename = 'login_history'
                      AND indexname = 'ix_maw_login_history_username_attempt_time') THEN

        CREATE INDEX ix_maw_login_history_username_attempt_time
            ON maw.login_history(username, attempt_time);
      
    END IF;
END
$$;

DO
$$
BEGIN
    IF NOT EXISTS (SELECT 1
                     FROM pg_catalog.pg_indexes
                    WHERE schemaname = 'maw'
                      AND tablename = 'login_history'
                      AND indexname = 'ix_maw_login_history_user_id_attempt_time') THEN

        CREATE INDEX ix_maw_login_history_user_id_attempt_time
            ON maw.login_history(user_id, attempt_time);
      
    END IF;
END
$$;

GRANT SELECT, INSERT
   ON maw.login_history
   TO website;
   
GRANT USAGE 
   ON SEQUENCE maw.login_history_id_seq 
   TO website;
