CREATE OR REPLACE FUNCTION idsrv.get_signing_keys
()
RETURNS TABLE
(
    id TEXT,
    version INT,
    created TIMESTAMPTZ,
    algorithm TEXT,
    is_x509_certificate BOOLEAN,
    data_protected BOOLEAN,
    data TEXT
)
LANGUAGE SQL
AS $$

    SELECT id,
           version,
           created,
           algorithm,
           is_x509_certificate,
           data_protected,
           data
      FROM idsrv.signing_key;

$$;

GRANT EXECUTE
   ON FUNCTION idsrv.get_signing_keys
   TO idsrv;
