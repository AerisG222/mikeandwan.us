CREATE OR REPLACE FUNCTION maw.get_usernames
()
RETURNS TABLE (
    username VARCHAR(30)
)
LANGUAGE SQL
AS $$

    SELECT username
      FROM maw.user
     ORDER BY username;

$$;

GRANT EXECUTE
   ON FUNCTION maw.get_usernames
   TO website;
