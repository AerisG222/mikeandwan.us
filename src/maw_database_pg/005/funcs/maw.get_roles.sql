CREATE OR REPLACE FUNCTION maw.get_roles
(
    _id SMALLINT DEFAULT NULL,
    _name VARCHAR(30) DEFAULT NULL
)
RETURNS TABLE (
    id SMALLINT,
    name VARCHAR(30),
    description VARCHAR(255)
)
LANGUAGE SQL
AS $$

    SELECT id,
           name,
           description
      FROM maw.role
     WHERE (_id IS NULL OR id = _id)
       AND (_name IS NULL OR name = _name)
     ORDER BY name;

$$;

GRANT EXECUTE
   ON FUNCTION maw.get_roles
   TO website;
