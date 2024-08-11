CREATE OR REPLACE FUNCTION maw.get_users_in_role
(
    _role_name VARCHAR(30)
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
LANGUAGE PLPGSQL
AS $$
DECLARE
    _role_id SMALLINT;
BEGIN

    SELECT r.id INTO _role_id
      FROM maw.role r
     WHERE r.name = _role_name;

    RETURN QUERY
    SELECT u.id,
           u.username,
           u.hashed_password,
           u.security_stamp,
           u.password_last_set_on,
           u.first_name,
           u.last_name,
           u.email,
           u.enable_github_auth AS is_github_auth_enabled,
           u.enable_google_auth AS is_google_auth_enabled,
           u.enable_microsoft_auth AS is_microsoft_auth_enabled,
           u.enable_twitter_auth AS is_twitter_auth_enabled
      FROM maw.user u
     WHERE u.id IN (SELECT r.user_id
                      FROM maw.user_role r
                     WHERE r.role_id = _role_id
                   )
     ORDER BY u.username;

END;
$$;

GRANT EXECUTE
   ON FUNCTION maw.get_users_in_role
   TO website;
