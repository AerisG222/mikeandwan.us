CREATE OR REPLACE FUNCTION photo.save_comment
(
    _username VARCHAR(30),
    _photo_id INTEGER,
    _message TEXT,
    _entry_date TIMESTAMP
)
RETURNS INTEGER
LANGUAGE PLPGSQL
AS $$
DECLARE
    _user_id SMALLINT;
    _comment_id INTEGER;
BEGIN

    SELECT id INTO _user_id
      FROM maw.user
     WHERE username = _username;

    INSERT INTO photo.comment
         (
           user_id,
           photo_id,
           message,
           entry_date
         )
    VALUES
         (
           _user_id,
           _photo_id,
           _message,
           _entry_date
         )
    RETURNING id INTO _comment_id;

    RETURN _comment_id;

END;
$$;

GRANT EXECUTE
   ON FUNCTION photo.save_comment
   TO website;
