DROP FUNCTION IF EXISTS video.get_years(BOOLEAN);

CREATE OR REPLACE FUNCTION video.get_years
(
    _roles TEXT[]
)
RETURNS TABLE
(
    year SMALLINT
)
LANGUAGE SQL
AS $$

    SELECT DISTINCT c.year
      FROM video.category c
     INNER JOIN video.category_role cr ON c.id = cr.category_id
     INNER JOIN maw.role r ON cr.role_id = r.id
     WHERE r.name = ANY(_roles)
     ORDER BY year DESC;

$$;

GRANT EXECUTE
   ON FUNCTION video.get_years
   TO website;
