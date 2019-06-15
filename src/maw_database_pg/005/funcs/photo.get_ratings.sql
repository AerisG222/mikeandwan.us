CREATE OR REPLACE FUNCTION photo.get_ratings
(
    _photo_id INTEGER,
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
              FROM photo.rating
             WHERE photo_id = _photo_id
           ) AS average_rating,
           (SELECT score
              FROM photo.rating
             WHERE photo_id = _photo_id
               AND user_id = (SELECT id
                                FROM maw.user
                               WHERE username = _username
                             )
           ) AS user_rating;

$$;

GRANT EXECUTE
   ON FUNCTION photo.get_ratings
   TO website;
