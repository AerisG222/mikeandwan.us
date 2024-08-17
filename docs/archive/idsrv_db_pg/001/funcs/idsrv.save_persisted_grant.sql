CREATE OR REPLACE FUNCTION idsrv.save_persisted_grant
(
    _key VARCHAR(200),
    _type VARCHAR(50),
    _subject_id VARCHAR(200),
    _client_id VARCHAR(200),
    _creation_time TIMESTAMPTZ,
    _expiration TIMESTAMPTZ,
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
             client_id,
             creation_time,
             expiration,
             data
         )
     VALUES
         (
             _key,
             _type,
             _subject_id,
             _client_id,
             _creation_time,
             _expiration,
             _data
         );

    GET DIAGNOSTICS _rowcount = ROW_COUNT;

    RETURN _rowcount;

END;
$$;

GRANT EXECUTE
   ON FUNCTION idsrv.save_persisted_grant
   TO idsrv;
