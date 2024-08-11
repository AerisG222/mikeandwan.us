DROP FUNCTION IF EXISTS photo.get_years(BOOLEAN);

CREATE OR REPLACE FUNCTION photo.get_years
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
      FROM photo.category c
     INNER JOIN photo.category_role cr ON c.id = cr.category_id
     INNER JOIN maw.role r ON cr.role_id = r.id
     WHERE r.name = ANY(_roles)
     ORDER BY year DESC;

$$;

GRANT EXECUTE
   ON FUNCTION photo.get_years
   TO website;
