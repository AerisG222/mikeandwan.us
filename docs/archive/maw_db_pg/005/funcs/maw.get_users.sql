CREATE OR REPLACE FUNCTION maw.get_users
(
    _id SMALLINT DEFAULT NULL,
    _username VARCHAR(30) DEFAULT NULL,
    _email VARCHAR(255) DEFAULT NULL
)
RETURNS TABLE (
    id SMALLINT,
    username VARCHAR(30),
    hashed_password VARCHAR(2000),
    security_stamp VARCHAR(2000),
    password_last_set_on TIMESTAMP,
    first_name VARCHAR(30),
    last_name VARCHAR(30),
    email VARCHAR(255),
    is_github_auth_enabled BOOLEAN,
    is_google_auth_enabled BOOLEAN,
    is_microsoft_auth_enabled BOOLEAN,
    is_twitter_auth_enabled BOOLEAN
)
LANGUAGE SQL
AS $$

    SELECT id,
           username,
           hashed_password,
           security_stamp,
           password_last_set_on,
           first_name,
           last_name,
           email,
           enable_github_auth AS is_github_auth_enabled,
           enable_google_auth AS is_google_auth_enabled,
           enable_microsoft_auth AS is_microsoft_auth_enabled,
           enable_twitter_auth AS is_twitter_auth_enabled
      FROM maw.user
     WHERE (_id IS NULL OR id = _id)
       AND (_username IS NULL OR username = _username)
       AND (_email IS NULL OR email = _email)
     ORDER BY username;

$$;

GRANT EXECUTE
   ON FUNCTION maw.get_users
   TO website;
