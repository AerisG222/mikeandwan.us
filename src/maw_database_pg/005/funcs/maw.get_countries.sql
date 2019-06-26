CREATE OR REPLACE FUNCTION maw.get_countries
()
RETURNS TABLE (
    id SMALLINT,
    code CHAR(2),
    name VARCHAR(30)
)
LANGUAGE SQL
AS $$

    SELECT id,
           code,
           name
	  FROM maw.country;

$$;

GRANT EXECUTE
   ON FUNCTION maw.get_countries
   TO website;
