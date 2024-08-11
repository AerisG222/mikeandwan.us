DROP FUNCTION IF EXISTS video.get_comments(SMALLINT);

CREATE OR REPLACE FUNCTION video.get_comments
(
    _video_id SMALLINT,
    _roles TEXT[]
)
RETURNS TABLE
(
    entry_date TIMESTAMP,
    comment_text TEXT,
    username VARCHAR(30)
)
LANGUAGE SQL
AS $$

    SELECT DISTINCT
           entry_date,
           message AS comment_text,
           u.username
      FROM video.comment c
     INNER JOIN video.video v ON c.video_id = v.id
     INNER JOIN video.category_role cr ON v.category_id = cr.category_id
     INNER JOIN maw.role r ON cr.role_id = r.id
     INNER JOIN maw.user u ON c.user_id = u.id
     WHERE c.video_id = _video_id
       AND r.name = ANY(_roles)
     ORDER BY entry_date DESC;

$$;

GRANT EXECUTE
   ON FUNCTION video.get_comments
   TO website;
