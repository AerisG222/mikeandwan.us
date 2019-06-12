CREATE OR REPLACE FUNCTION video.save_comment
(
    _username VARCHAR(30),
    _video_id SMALLINT,
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

    INSERT INTO video.comment
         (
           user_id,
           video_id,
           message,
           entry_date
         )
    VALUES
         (
           _user_id,
           _video_id,
           _message,
           _entry_date
         )
    RETURNING id INTO _comment_id;

    RETURN _comment_id;

END;
$$;

GRANT EXECUTE
   ON FUNCTION video.save_comment
   TO website;
