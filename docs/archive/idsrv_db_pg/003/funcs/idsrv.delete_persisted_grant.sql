DROP FUNCTION IF EXISTS idsrv.delete_persisted_grant;

CREATE OR REPLACE FUNCTION idsrv.delete_persisted_grant
(
    _key VARCHAR(200)
)
RETURNS BIGINT
LANGUAGE PLPGSQL
AS $$
DECLARE
    _rowcount BIGINT;
BEGIN

    DELETE
      FROM idsrv.persisted_grant
     WHERE key = _key;

    GET DIAGNOSTICS _rowcount = ROW_COUNT;

    RETURN _rowcount;

END;
$$;

GRANT EXECUTE
   ON FUNCTION idsrv.delete_persisted_grant
   TO idsrv;
