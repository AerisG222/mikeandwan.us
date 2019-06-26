CREATE OR REPLACE FUNCTION maw.add_user
(
    _username VARCHAR(30),
    _hashed_password VARCHAR(2000),
    _first_name VARCHAR(30),
    _last_name VARCHAR(30),
    _email VARCHAR(255),
    _security_stamp VARCHAR(2000),
    _password_last_set_on TIMESTAMP
)
RETURNS SMALLINT
LANGUAGE PLPGSQL
AS $$
DECLARE
    _user_id SMALLINT;
BEGIN

    INSERT INTO maw.user
        (
            username,
            hashed_password,
            first_name,
            last_name,
            email,
            security_stamp,
            password_last_set_on
        )
    VALUES
        (
            _username,
            _hashed_password,
            _first_name,
            _last_name,
            _email,
            _security_stamp,
            _password_last_set_on
        )
    RETURNING id INTO _user_id;

    RETURN _user_id;

END;
$$;

GRANT EXECUTE
   ON FUNCTION maw.add_user
   TO website;
