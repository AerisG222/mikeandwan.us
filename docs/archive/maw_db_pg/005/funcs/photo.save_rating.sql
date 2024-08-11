CREATE OR REPLACE FUNCTION photo.save_rating
(
    _photo_id INTEGER,
    _username VARCHAR(30),
    _score SMALLINT
)
RETURNS BIGINT
LANGUAGE PLPGSQL
AS $$
DECLARE
    _user_id SMALLINT;
    _rowcount BIGINT;
BEGIN

    SELECT id INTO _user_id
      FROM maw.user
     WHERE username = _username;

    IF _score IS NULL OR _score <= 0 THEN

        DELETE
          FROM photo.rating
         WHERE photo_id = _photo_id
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
                _photo_id,
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
