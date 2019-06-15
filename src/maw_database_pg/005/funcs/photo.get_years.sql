CREATE OR REPLACE FUNCTION photo.get_years
(
    _allow_private BOOLEAN
)
RETURNS TABLE
(
    year SMALLINT
)
LANGUAGE SQL
AS $$

    SELECT DISTINCT year
      FROM photo.category
     WHERE (_allow_private OR is_private = FALSE)
     ORDER BY year DESC;

$$;

GRANT EXECUTE
   ON FUNCTION photo.get_years
   TO website;
