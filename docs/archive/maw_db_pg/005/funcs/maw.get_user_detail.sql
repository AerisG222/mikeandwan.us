CREATE OR REPLACE FUNCTION maw.get_user_detail
()
RETURNS TABLE (
    id SMALLINT,
    username VARCHAR(30),
    first_name VARCHAR(30),
    last_name VARCHAR(30),
    email VARCHAR(255),
    last_login_date TIMESTAMP
)
LANGUAGE SQL
AS $$

    SELECT id,
           username,
           first_name,
           last_name,
           email,
           (SELECT MAX(attempt_time)
              FROM maw.login_history lh
             WHERE lh.user_id = u.id
           ) AS last_login_date
      FROM maw.user u
     ORDER BY u.username;

$$;

GRANT EXECUTE
   ON FUNCTION maw.get_user_detail
   TO website;
