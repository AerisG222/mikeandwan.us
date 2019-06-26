CREATE OR REPLACE FUNCTION maw.save_security_stamp
(
    _username VARCHAR(30),
    _security_stamp VARCHAR(2000)
)
RETURNS BIGINT
LANGUAGE PLPGSQL
AS $$
DECLARE
    _rowcount BIGINT;
BEGIN

    UPDATE maw.user
       SET security_stamp = _security_stamp
     WHERE username = _username;

    GET DIAGNOSTICS _rowcount = ROW_COUNT;

    RETURN _rowcount;

END;
$$;

GRANT EXECUTE
   ON FUNCTION maw.save_security_stamp
   TO website;
