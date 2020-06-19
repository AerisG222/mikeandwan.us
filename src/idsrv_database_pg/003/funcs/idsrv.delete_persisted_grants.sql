DROP FUNCTION IF EXISTS idsrv.delete_persisted_grants(
    _key VARCHAR(200),
    _subject_id VARCHAR(200),
    _client_id VARCHAR(200),
    _type VARCHAR(50));

CREATE OR REPLACE FUNCTION idsrv.delete_persisted_grants
(
    _subject_id VARCHAR(200) DEFAULT NULL,
    _session_id TEXT DEFAULT NULL,
    _client_id VARCHAR(200) DEFAULT NULL,
    _type VARCHAR(50) DEFAULT NULL
)
RETURNS BIGINT
LANGUAGE PLPGSQL
AS $$
DECLARE
    _user_id SMALLINT;
    _rowcount BIGINT;
BEGIN

    DELETE
      FROM idsrv.persisted_grant
     WHERE (_subject_id IS NULL OR subject_id = _subject_id)
       AND (_session_id IS NULL OR session_id = _session_id)
       AND (_client_id IS NULL OR client_id = _client_id)
       AND (_type IS NULL OR type = _type);

    GET DIAGNOSTICS _rowcount = ROW_COUNT;

    RETURN _rowcount;

END;
$$;

GRANT EXECUTE
   ON FUNCTION idsrv.delete_persisted_grants
   TO idsrv;
