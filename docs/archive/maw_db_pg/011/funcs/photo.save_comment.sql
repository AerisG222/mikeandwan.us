DROP FUNCTION IF EXISTS photo.save_comment(VARCHAR(30), INTEGER, TEXT, TIMESTAMP);

CREATE OR REPLACE FUNCTION photo.save_comment
(
    _username VARCHAR(30),
    _photo_id INTEGER,
    _message TEXT,
    _entry_date TIMESTAMP,
    _roles TEXT[]
)
RETURNS INTEGER
LANGUAGE PLPGSQL
AS $$
DECLARE
    _authorized_photo_id INTEGER;
    _user_id SMALLINT;
    _comment_id INTEGER;
BEGIN

    SELECT MAX(p.id) INTO _authorized_photo_id
      FROM photo.photo p
     INNER JOIN photo.category_role cr ON p.category_id = cr.category_id
     INNER JOIN maw.role r ON cr.role_id = r.id
     WHERE p.id = _photo_id
       AND r.name = ANY(_roles);

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
           _authorized_photo_id,
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
