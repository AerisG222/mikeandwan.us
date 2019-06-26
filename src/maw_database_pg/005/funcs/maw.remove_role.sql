CREATE OR REPLACE FUNCTION maw.remove_role
(
    _name VARCHAR(30)
)
RETURNS BIGINT
LANGUAGE PLPGSQL
AS $$
DECLARE
    _role_id SMALLINT;
    _rowcount BIGINT;
BEGIN

    SELECT id INTO _role_id
      FROM maw.role
     WHERE name = _name;

    DELETE
      FROM maw.user_role
	 WHERE role_id = _role_id;

    DELETE
      FROM maw.role
	 WHERE id = _role_id;

    GET DIAGNOSTICS _rowcount = ROW_COUNT;

    RETURN _rowcount;

END;
$$;

GRANT EXECUTE
   ON FUNCTION maw.remove_role
   TO website;
