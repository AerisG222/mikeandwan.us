CREATE OR REPLACE FUNCTION idsrv.delete_signing_key
(
    _id TEXT
)
RETURNS BIGINT
LANGUAGE PLPGSQL
AS $$
DECLARE
    _rowcount BIGINT;
BEGIN

    DELETE
      FROM idsrv.signing_key
     WHERE id = _id;

    GET DIAGNOSTICS _rowcount = ROW_COUNT;

    RETURN _rowcount;

END;
$$;

GRANT EXECUTE
   ON FUNCTION idsrv.delete_signing_key
   TO idsrv;
