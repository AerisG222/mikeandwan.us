DROP FUNCTION IF EXISTS photo.get_ratings(INTEGER, VARCHAR(30));

CREATE OR REPLACE FUNCTION photo.get_ratings
(
    _photo_id INTEGER,
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

    WITH authorized_photo_id
    AS
    (
        SELECT DISTINCT p.id
          FROM photo.photo p
         INNER JOIN photo.category_role cr ON p.category_id = cr.category_id
         INNER JOIN maw.role r ON cr.role_id = r.id
         WHERE p.id = _photo_id
           AND r.name = ANY(_roles)
    )
    SELECT (
               SELECT AVG(r.score)
                 FROM photo.rating r
                INNER JOIN authorized_photo_id a ON a.id = r.photo_id
           ) AS average_rating,
           (
               SELECT r.score
                 FROM photo.rating r
                INNER JOIN authorized_photo_id a ON a.id = r.photo_id
                WHERE r.user_id = (SELECT id
                                     FROM maw.user
                                    WHERE username = _username
                                  )
           ) AS user_rating;

$$;

GRANT EXECUTE
   ON FUNCTION photo.get_ratings
   TO website;
