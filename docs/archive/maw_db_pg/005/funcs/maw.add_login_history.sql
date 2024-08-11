CREATE OR REPLACE FUNCTION maw.add_login_history
(
    _login_activity_type_id SMALLINT,
    _login_area_id SMALLINT,
    _attempt_time TIMESTAMP,
    _username VARCHAR(30) DEFAULT NULL,
    _email VARCHAR(255) DEFAULT NULL
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
     WHERE (_username IS NULL OR username = _username)
       AND (_email IS NULL OR email = _email);

    INSERT INTO maw.login_history
         (
             user_id,
             username,
             login_activity_type_id,
             login_area_id,
             attempt_time
         )
     VALUES
         (
             _user_id,
             COALESCE(_username, _email),
             _login_activity_type_id,
             _login_area_id,
             _attempt_time
         );

    GET DIAGNOSTICS _rowcount = ROW_COUNT;

    RETURN _rowcount;

END;
$$;

GRANT EXECUTE
   ON FUNCTION maw.add_login_history
   TO website;
