CREATE OR REPLACE FUNCTION maw.update_user
(
    _username VARCHAR(30),
    _first_name VARCHAR(30),
    _last_name VARCHAR(30),
    _email VARCHAR(255),
    _hashed_password VARCHAR(2000),
    _security_stamp VARCHAR(2000),
    _enable_github_auth BOOLEAN,
    _enable_google_auth BOOLEAN,
    _enable_microsoft_auth BOOLEAN,
    _enable_twitter_auth BOOLEAN
)
RETURNS BIGINT
LANGUAGE PLPGSQL
AS $$
DECLARE
    _rowcount BIGINT;
BEGIN

    UPDATE maw.user
       SET first_name = _first_name,
           last_name = _last_name,
           email = _email,
           hashed_password = _hashed_password,
           security_stamp = _security_stamp,
           enable_github_auth = _enable_github_auth,
           enable_google_auth = _enable_google_auth,
           enable_microsoft_auth = _enable_microsoft_auth,
           enable_twitter_auth = _enable_twitter_auth
     WHERE username = _username;

    GET DIAGNOSTICS _rowcount = ROW_COUNT;

    RETURN _rowcount;

END;
$$;

GRANT EXECUTE
   ON FUNCTION maw.update_user
   TO website;
