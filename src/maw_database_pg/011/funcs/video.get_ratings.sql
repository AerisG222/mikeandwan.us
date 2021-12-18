DROP FUNCTION IF EXISTS video.get_ratings(SMALLINT, VARCHAR(30));

CREATE OR REPLACE FUNCTION video.get_ratings
(
    _video_id SMALLINT,
    _username VARCHAR(30),
    _roles TEXT[]
)
RETURNS TABLE
(
    average_rating NUMERIC,
    user_rating SMALLINT
)
LANGUAGE SQL
AS $$

    WITH authorized_video_id
    AS
    (
        SELECT DISTINCT v.id
          FROM video.video v
         INNER JOIN video.category_role cr ON v.category_id = cr.category_id
         INNER JOIN maw.role r ON cr.role_id = r.id
         WHERE v.id = _video_id
           AND r.name = ANY(_roles)
    )
    SELECT (
               SELECT AVG(r.score)
                 FROM video.rating r
                INNER JOIN authorized_video_id a ON a.id = r.video_id
           ) AS average_rating,
           (
               SELECT r.score
                 FROM video.rating r
                INNER JOIN authorized_video_id a ON a.id = r.video_id
                WHERE r.user_id = (SELECT id
                                     FROM maw.user
                                    WHERE username = _username
                                  )
           ) AS user_rating;

$$;

GRANT EXECUTE
   ON FUNCTION video.get_ratings
   TO website;
