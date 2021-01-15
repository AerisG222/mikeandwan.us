CREATE OR REPLACE FUNCTION idsrv.save_signing_key
(
    _id TEXT,
    _version INT,
    _created TIMESTAMPTZ,
    _algorithm TEXT,
    _is_x509_certificate BOOLEAN,
    _data_protected BOOLEAN,
    _data TEXT
)
RETURNS BIGINT
LANGUAGE PLPGSQL
AS $$
DECLARE
    _rowcount BIGINT;
BEGIN

    INSERT INTO idsrv.signing_key
         (
             id,
             version,
             created,
             algorithm,
             is_x509_certificate,
             data_protected,
             data
         )
     VALUES
         (
             _id,
             _version,
             _created,
             _algorithm,
             _is_x509_certificate,
             _data_protected,
             _data
         );

    GET DIAGNOSTICS _rowcount = ROW_COUNT;

    RETURN _rowcount;

END;
$$;

GRANT EXECUTE
   ON FUNCTION idsrv.save_signing_key
   TO idsrv;
