DROP FUNCTION IF EXISTS video.get_category_roles();

CREATE OR REPLACE FUNCTION video.get_category_roles
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
      FROM video.category c
     INNER JOIN video.category_role cr ON c.id = cr.category_id
     INNER JOIN maw.role r ON cr.role_id = r.id
     GROUP BY c.id
     ORDER BY c.id;

$$;

GRANT EXECUTE
   ON FUNCTION video.get_category_roles
   TO website;
