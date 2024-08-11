DROP FUNCTION IF EXISTS video.save_rating;

CREATE OR REPLACE FUNCTION video.save_rating
(
    _video_id SMALLINT,
    _username VARCHAR(30),
    _score SMALLINT,
    _roles TEXT[]
)
RETURNS BIGINT
LANGUAGE PLPGSQL
AS $$
DECLARE
    _authorized_video_id INTEGER;
    _user_id SMALLINT;
    _rowcount BIGINT;
BEGIN

    SELECT MAX(v.id) INTO _authorized_video_id
      FROM video.video v
     INNER JOIN video.category_role cr ON v.category_id = cr.category_id
     INNER JOIN maw.role r ON cr.role_id = r.id
     WHERE v.id = _video_id
       AND r.name = ANY(_roles);

    SELECT id INTO _user_id
      FROM maw.user
     WHERE username = _username;

    IF _score IS NULL OR _score <= 0 THEN

        DELETE
          FROM video.rating
         WHERE video_id = _authorized_video_id
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
                _authorized_video_id,
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
