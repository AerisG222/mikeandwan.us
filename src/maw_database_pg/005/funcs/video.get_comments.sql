CREATE OR REPLACE FUNCTION video.get_comments
(
    _video_id SMALLINT
)
RETURNS TABLE
(
    entry_date TIMESTAMP,
    comment_text TEXT,
    username VARCHAR(30)
)
LANGUAGE SQL
AS $$

    SELECT entry_date,
           message AS comment_text,
           u.username
      FROM video.comment c
     INNER JOIN maw.user u
             ON c.user_id = u.id
     WHERE c.video_id = _video_id
     ORDER BY entry_date DESC;

$$;

GRANT EXECUTE
   ON FUNCTION video.get_comments
   TO website;
