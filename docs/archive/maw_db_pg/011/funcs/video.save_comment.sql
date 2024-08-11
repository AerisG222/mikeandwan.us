DROP FUNCTION IF EXISTS video.save_comment(VARCHAR(30), SMALLINT, TEXT, TIMESTAMP);

CREATE OR REPLACE FUNCTION video.save_comment
(
    _username VARCHAR(30),
    _video_id SMALLINT,
    _message TEXT,
    _entry_date TIMESTAMP,
    _roles TEXT[]
)
RETURNS INTEGER
LANGUAGE PLPGSQL
AS $$
DECLARE
    _authorized_video_id INTEGER;
    _user_id SMALLINT;
    _comment_id INTEGER;
BEGIN

    SELECT MAX(v.id) INTO _authorized_video_id
      FROM video.video v
     INNER JOIN video.category_role cr ON v.category_id = cr.category_id
     INNER JOIN maw.role r ON cr.role_id = r.id
     WHERE v.id = _photo_id
       AND r.name = ANY(_roles);

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
           _authorized_video_id,
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
