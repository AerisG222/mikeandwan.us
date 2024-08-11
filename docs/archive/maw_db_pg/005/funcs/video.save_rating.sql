CREATE OR REPLACE FUNCTION video.save_rating
(
    _video_id SMALLINT,
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
          FROM video.rating
         WHERE video_id = _video_id
           AND user_id = _user_id;

    ELSE

        INSERT
          INTO video.rating
             (
                video_id,
                user_id,
                score
             )
        VALUES
             (
                _video_id,
                _user_id,
                _score
            )
        ON CONFLICT (video_id, user_id)
        DO UPDATE
           SET score = _score;

    END IF;

    GET DIAGNOSTICS _rowcount = ROW_COUNT;

    RETURN _rowcount;

END;
$$;

GRANT EXECUTE
   ON FUNCTION video.save_rating
   TO website;
