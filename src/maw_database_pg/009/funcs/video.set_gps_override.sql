CREATE OR REPLACE FUNCTION video.set_gps_override
(
    _video_id INTEGER,
    _latitude REAL,
    _longitude REAL,
    _username VARCHAR(30),
    _update_date TIMESTAMP
)
RETURNS BOOLEAN
LANGUAGE PLPGSQL
AS $$
DECLARE
    _user_id SMALLINT;
BEGIN

    DELETE FROM video.point_of_interest poi
     WHERE poi.video_id = _video_id
       AND poi.is_override = TRUE;

    DELETE FROM video.reverse_geocode rg
     WHERE rg.video_id = _video_id
       AND rg.is_override = TRUE;

    DELETE FROM video.gps_override pgo
     WHERE pgo.video_id = _video_id;

    IF _latitude IS NOT NULL AND _longitude IS NOT NULL THEN

        SELECT id INTO _user_id
          FROM maw.user
         WHERE username = _username;

        INSERT INTO video.gps_override
             (
                 video_id,
                 latitude,
                 longitude,
                 user_id,
                 updated_time,
                 has_been_reverse_geocoded
             )
        VALUES
             (
                 _video_id,
                 _latitude,
                 _longitude,
                 _user_id,
                 _update_date,
                 FALSE
             );

    END IF;

    RETURN TRUE;

END;
$$;

GRANT EXECUTE
   ON FUNCTION video.set_gps_override
   TO website;
