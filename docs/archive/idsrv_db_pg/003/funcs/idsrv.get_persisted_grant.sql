DROP FUNCTION IF EXISTS idsrv.get_persisted_grant;

CREATE OR REPLACE FUNCTION idsrv.get_persisted_grant
(
    _key VARCHAR(200)
)
RETURNS TABLE
(
    key VARCHAR(200),
    type VARCHAR(50),
    subject_id VARCHAR(200),
    session_id TEXT,
    client_id VARCHAR(200),
    description TEXT,
    creation_time TIMESTAMPTZ,
    expiration TIMESTAMPTZ,
    consumed_time TIMESTAMPTZ,
    data VARCHAR(50000)
)
LANGUAGE SQL
AS $$

    SELECT key,
           type,
           subject_id,
           session_id,
           client_id,
           description,
           creation_time,
           expiration,
           consumed_time,
           data
      FROM idsrv.persisted_grant
     WHERE key = _key;

$$;

GRANT EXECUTE
   ON FUNCTION idsrv.get_persisted_grant
   TO idsrv;
