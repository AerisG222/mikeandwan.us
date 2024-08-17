DROP FUNCTION IF EXISTS idsrv.get_persisted_grants;

CREATE OR REPLACE FUNCTION idsrv.get_persisted_grants
(
    _subject_id VARCHAR(200) DEFAULT NULL,
    _session_id TEXT DEFAULT NULL,
    _client_id VARCHAR(200) DEFAULT NULL,
    _type VARCHAR(50) DEFAULT NULL
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
     WHERE (_subject_id IS NULL OR subject_id = _subject_id)
       AND (_session_id IS NULL OR session_id = _session_id)
       AND (_client_id IS NULL OR client_id = _client_id)
       AND (_type IS NULL OR type = _type)
     ORDER BY creation_time;

$$;

GRANT EXECUTE
   ON FUNCTION idsrv.get_persisted_grants
   TO idsrv;
