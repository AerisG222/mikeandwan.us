CREATE OR REPLACE FUNCTION maw.remove_user_from_role
(
    _username VARCHAR(30),
    _role_name VARCHAR(30)
)
RETURNS BIGINT
LANGUAGE PLPGSQL
AS $$
DECLARE
    _rowcount BIGINT;
BEGIN

    DELETE
      FROM maw.user_role
     WHERE user_id = (SELECT id FROM maw.user WHERE username = _username)
       AND role_id = (SELECT id FROM maw.role WHERE name = _role_name);

    GET DIAGNOSTICS _rowcount = ROW_COUNT;

    RETURN _rowcount;

END;
$$;

GRANT EXECUTE
   ON FUNCTION maw.remove_user_from_role
   TO website;
