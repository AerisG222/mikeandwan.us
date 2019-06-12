CREATE OR REPLACE FUNCTION video.get_ratings
(
    _video_id SMALLINT,
    _username VARCHAR(30)
)
RETURNS TABLE
(
    average_rating NUMERIC,
    user_rating SMALLINT
)
LANGUAGE SQL
AS $$

    SELECT (SELECT AVG(score)
              FROM video.rating
             WHERE video_id = _video_id
           ) AS average_rating,
           (SELECT score
              FROM video.rating
             WHERE video_id = _video_id
               AND user_id = (SELECT id
                                FROM maw.user
                               WHERE username = _username
                             )
           ) AS user_rating;

$$;

GRANT EXECUTE
   ON FUNCTION video.get_ratings
   TO website;
