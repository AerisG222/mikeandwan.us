DROP FUNCTION IF EXISTS idsrv.save_persisted_grant(
    _key VARCHAR(200),
    _type VARCHAR(50),
    _subject_id VARCHAR(200),
    _client_id VARCHAR(200),
    _creation_time TIMESTAMPTZ,
    _expiration TIMESTAMPTZ,
    _data VARCHAR(50000));

CREATE OR REPLACE FUNCTION idsrv.save_persisted_grant
(
    _key VARCHAR(200),
    _type VARCHAR(50),
    _subject_id VARCHAR(200),
    _session_id TEXT,
    _client_id VARCHAR(200),
    _description TEXT,
    _creation_time TIMESTAMPTZ,
    _expiration TIMESTAMPTZ,
    _consumed_time TIMESTAMPTZ,
    _data VARCHAR(50000)
)
RETURNS BIGINT
LANGUAGE PLPGSQL
AS $$
DECLARE
    _rowcount BIGINT;
BEGIN

    INSERT INTO idsrv.persisted_grant
         (
             key,
             type,
             subject_id,
             session_id,
             client_id,
             description,
             creation_time,
             expiration,
             consumed_time,
             data
         )
     VALUES
         (
             _key,
             _type,
             _subject_id,
             _session_id,
             _client_id,
             _description,
             _creation_time,
             _expiration,
             _consumed_time,
             _data
         )
    ON CONFLICT (key)
    DO
    UPDATE
       SET type = EXCLUDED.type,
           subject_id = EXCLUDED.subject_id,
           session_id = EXCLUDED.session_id,
           client_id = EXCLUDED.client_id,
           description = EXCLUDED.description,
           creation_time = EXCLUDED.creation_time,
           expiration = EXCLUDED.expiration,
           consumed_time = EXCLUDED.consumed_time,
           data = EXCLUDED.data;

    GET DIAGNOSTICS _rowcount = ROW_COUNT;

    RETURN _rowcount;

END;
$$;

GRANT EXECUTE
   ON FUNCTION idsrv.save_persisted_grant
   TO idsrv;
