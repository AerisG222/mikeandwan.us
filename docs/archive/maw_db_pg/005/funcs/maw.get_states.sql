CREATE OR REPLACE FUNCTION maw.get_states
(
    _code CHAR(2) DEFAULT NULL
)
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
	  FROM maw.state
     WHERE (_code IS NULL OR code = _code)
     ORDER BY name;

$$;

GRANT EXECUTE
   ON FUNCTION maw.get_states
   TO website;
