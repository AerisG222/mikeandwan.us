CREATE OR REPLACE FUNCTION maw.get_roles_for_user
(
    _username VARCHAR(30)
)
RETURNS TABLE (
    id SMALLINT,
    name VARCHAR(30),
    description VARCHAR(255)
)
LANGUAGE SQL
AS $$

    SELECT r.id,
           r.name,
           r.description
      FROM maw.role r
     INNER JOIN maw.user_role ur
             ON ur.role_id = r.id
     INNER JOIN maw.user u
             ON u.id = ur.user_id
     WHERE u.username = _username
     ORDER BY r.name;

$$;

GRANT EXECUTE
   ON FUNCTION maw.get_roles_for_user
   TO website;
