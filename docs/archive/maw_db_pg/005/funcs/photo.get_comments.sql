CREATE OR REPLACE FUNCTION photo.get_comments
(
    _photo_id INTEGER
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
      FROM photo.comment c
     INNER JOIN maw.user u
             ON c.user_id = u.id
     WHERE c.photo_id = _photo_id
     ORDER BY entry_date DESC;

$$;

GRANT EXECUTE
   ON FUNCTION photo.get_comments
   TO website;
