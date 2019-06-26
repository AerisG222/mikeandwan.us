CREATE OR REPLACE FUNCTION idsrv.get_persisted_grants
(
    _key VARCHAR(200) DEFAULT NULL,
    _subject_id VARCHAR(200) DEFAULT NULL
)
RETURNS TABLE
(
    key VARCHAR(200),
    type VARCHAR(50),
    subject_id VARCHAR(200),
    client_id VARCHAR(200),
    creation_time TIMESTAMPTZ,
    expiration TIMESTAMPTZ,
    data VARCHAR(50000)
)
LANGUAGE SQL
AS $$

    SELECT key,
           type,
           subject_id,
           client_id,
           creation_time,
           expiration,
           data
      FROM idsrv.persisted_grant
     WHERE (_key IS NULL OR key = _key)
       AND (_subject_id IS NULL OR subject_id = _subject_id)
     ORDER BY creation_time;

$$;

GRANT EXECUTE
   ON FUNCTION idsrv.get_persisted_grants
   TO idsrv;
