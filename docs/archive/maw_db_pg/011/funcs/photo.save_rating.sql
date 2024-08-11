DROP FUNCTION IF EXISTS photo.save_rating(INTEGER, VARCHAR(30), SMALLINT);

CREATE OR REPLACE FUNCTION photo.save_rating
(
    _photo_id INTEGER,
    _username VARCHAR(30),
    _score SMALLINT,
    _roles TEXT[]
)
RETURNS BIGINT
LANGUAGE PLPGSQL
AS $$
DECLARE
    _authorized_photo_id INTEGER;
    _user_id SMALLINT;
    _rowcount BIGINT;
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

    IF _score IS NULL OR _score <= 0 THEN

        DELETE
          FROM photo.rating
         WHERE photo_id = _authorized_photo_id
           AND user_id = _user_id;

    ELSE

        INSERT
          INTO photo.rating
             (
                photo_id,
                user_id,
                score
             )
        VALUES
             (
                _authorized_photo_id,
                _user_id,
                _score
            )
        ON CONFLICT (photo_id, user_id)
        DO UPDATE
           SET score = _score;

    END IF;

    GET DIAGNOSTICS _rowcount = ROW_COUNT;

    RETURN _rowcount;

END;
$$;

GRANT EXECUTE
   ON FUNCTION photo.save_rating
   TO website;
