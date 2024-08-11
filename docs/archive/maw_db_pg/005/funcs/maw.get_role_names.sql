CREATE OR REPLACE FUNCTION maw.get_role_names
()
RETURNS TABLE (
    name VARCHAR(30)
)
LANGUAGE SQL
AS $$

    SELECT name
      FROM maw.role
     ORDER BY name;

$$;

GRANT EXECUTE
   ON FUNCTION maw.get_role_names
   TO website;
