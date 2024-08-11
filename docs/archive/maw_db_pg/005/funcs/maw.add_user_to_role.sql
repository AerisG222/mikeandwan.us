CREATE OR REPLACE FUNCTION maw.add_user_to_role
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

    INSERT INTO maw.user_role
         (
             user_id,
             role_id
         )
    VALUES
         (
             (SELECT id FROM maw.user WHERE username = _username),
             (SELECT id FROM maw.role WHERE name = _role_name)
         );

    GET DIAGNOSTICS _rowcount = ROW_COUNT;

    RETURN _rowcount;

END;
$$;

GRANT EXECUTE
   ON FUNCTION maw.add_user_to_role
   TO website;
