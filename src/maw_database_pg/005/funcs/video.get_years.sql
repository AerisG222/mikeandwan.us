CREATE OR REPLACE FUNCTION video.get_years
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
      FROM video.category
     WHERE (_allow_private OR is_private = FALSE)
     ORDER BY year DESC;

$$;

GRANT EXECUTE
   ON FUNCTION video.get_years
   TO website;
