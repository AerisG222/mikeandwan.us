DROP FUNCTION IF EXISTS photo.get_comments(INTEGER);

CREATE OR REPLACE FUNCTION photo.get_comments
(
    _photo_id INTEGER,
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
      FROM photo.comment c
     INNER JOIN photo.photo p ON c.photo_id = p.id
     INNER JOIN photo.category_role cr ON p.category_id = cr.category_id
     INNER JOIN maw.role r ON cr.role_id = r.id
     INNER JOIN maw.user u
             ON c.user_id = u.id
     WHERE c.photo_id = _photo_id
       AND r.name = ANY(_roles)
     ORDER BY entry_date DESC;

$$;

GRANT EXECUTE
   ON FUNCTION photo.get_comments
   TO website;
