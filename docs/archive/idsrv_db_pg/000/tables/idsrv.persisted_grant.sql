CREATE TABLE IF NOT EXISTS idsrv.persisted_grant (
    key VARCHAR(200) NOT NULL,
    type VARCHAR(50) NOT NULL,
    subject_id VARCHAR(200),
    client_id VARCHAR(200) NOT NULL,
    creation_time TIMESTAMPTZ NOT NULL,
    expiration TIMESTAMPTZ,
    data VARCHAR(50000) NOT NULL,
    CONSTRAINT pk_idsrv_persisted_grant PRIMARY KEY (key)
);

DO
$$
BEGIN
    IF NOT EXISTS (SELECT 1
                     FROM pg_catalog.pg_indexes
                    WHERE schemaname = 'idsrv'
                      AND tablename = 'persisted_grant'
                      AND indexname = 'ix_persisted_grant_subject_client_type') THEN

        CREATE INDEX ix_persisted_grant_subject_client_type
            ON idsrv.persisted_grant(subject_id, client_id, type);
      
    END IF;
END
$$;

GRANT SELECT, INSERT, UPDATE, DELETE
   ON idsrv.persisted_grant
   TO idsrv;
