CREATE OR REPLACE FUNCTION maw.remove_user
(
    _username VARCHAR(30)
)
RETURNS BIGINT
LANGUAGE PLPGSQL
AS $$
DECLARE
    _user_id SMALLINT;
    _rowcount BIGINT;
BEGIN

    SELECT id INTO _user_id
      FROM maw.user
     WHERE username = _username;

    DELETE
      FROM maw.user_role
     WHERE user_id = _user_id;

    UPDATE maw.login_history
       SET user_id = NULL
     WHERE user_id = _user_id;

    DELETE
      FROM maw.user
	 WHERE username = _username;

    GET DIAGNOSTICS _rowcount = ROW_COUNT;

    RETURN _rowcount;

END;
$$;

GRANT EXECUTE
   ON FUNCTION maw.remove_user
   TO website;
