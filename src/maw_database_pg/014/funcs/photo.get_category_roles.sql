DROP FUNCTION IF EXISTS photo.get_category_roles();

CREATE OR REPLACE FUNCTION photo.get_category_roles
()
RETURNS TABLE
(
    id SMALLINT,
    roles TEXT[]
)
LANGUAGE SQL
AS $$

    SELECT c.id,
           array_agg(r.name) AS roles
      FROM photo.category c
     INNER JOIN photo.category_role cr ON c.id = cr.category_id
     INNER JOIN maw.role r ON cr.role_id = r.id
     GROUP BY c.id
     ORDER BY c.id;

$$;

GRANT EXECUTE
   ON FUNCTION photo.get_category_roles
   TO website;
