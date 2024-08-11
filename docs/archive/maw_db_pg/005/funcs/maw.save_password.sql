CREATE OR REPLACE FUNCTION maw.save_password
(
    _username VARCHAR(30),
    _hashed_password VARCHAR(2000)
)
RETURNS BIGINT
LANGUAGE PLPGSQL
AS $$
DECLARE
    _rowcount BIGINT;
BEGIN

    UPDATE maw.user
       SET hashed_password = _hashed_password
     WHERE username = _username;

    GET DIAGNOSTICS _rowcount = ROW_COUNT;

    RETURN _rowcount;

END;
$$;

GRANT EXECUTE
   ON FUNCTION maw.save_password
   TO website;
