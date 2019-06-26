CREATE OR REPLACE FUNCTION maw.add_role
(
    _name VARCHAR(30),
    _description VARCHAR(255)
)
RETURNS SMALLINT
LANGUAGE SQL
AS $$

    INSERT INTO maw.role
         (
             name,
             description
         )
    VALUES
         (
             _name,
             _description
         )
    RETURNING id;

$$;

GRANT EXECUTE
   ON FUNCTION maw.add_role
   TO website;
