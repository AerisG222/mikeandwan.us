CREATE OR REPLACE FUNCTION idsrv.delete_persisted_grants
(
    _key VARCHAR(200) DEFAULT NULL,
    _subject_id VARCHAR(200) DEFAULT NULL,
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
     WHERE (_key IS NULL OR key = _key)
       AND (_subject_id IS NULL OR subject_id = _subject_id)
       AND (_client_id IS NULL OR client_id = _client_id)
       AND (_type IS NULL OR type = _type);

    GET DIAGNOSTICS _rowcount = ROW_COUNT;

    RETURN _rowcount;

END;
$$;

GRANT EXECUTE
   ON FUNCTION idsrv.delete_persisted_grants
   TO idsrv;
