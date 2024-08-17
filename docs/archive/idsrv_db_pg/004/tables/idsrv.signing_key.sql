CREATE TABLE IF NOT EXISTS idsrv.signing_key (
    id TEXT NOT NULL,
    version INT NOT NULL,
    created TIMESTAMPTZ NOT NULL,
    algorithm TEXT NOT NULL,
    is_x509_certificate BOOLEAN NOT NULL,
    data_protected BOOLEAN NOT NULL,
    data TEXT NOT NULL,
    CONSTRAINT pk_idsrv_signing_key PRIMARY KEY (id)
);

GRANT SELECT, INSERT, UPDATE, DELETE
   ON idsrv.signing_key
   TO idsrv;
