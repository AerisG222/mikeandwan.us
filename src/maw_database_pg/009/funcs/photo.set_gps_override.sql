CREATE OR REPLACE FUNCTION photo.set_gps_override
(
    _photo_id INTEGER,
    _latitude REAL,
    _longitude REAL,
    _username VARCHAR(30),
    _update_date TIMESTAMP
)
RETURNS BIGINT
LANGUAGE PLPGSQL
AS $$
DECLARE
    _user_id SMALLINT;
    _rowcount BIGINT;
BEGIN

    DELETE FROM photo.point_of_interest poi
     WHERE poi.photo_id = _photo_id
       AND poi.is_override = TRUE;

    DELETE FROM photo.reverse_geocode rg
     WHERE rg.photo_id = _photo_id
       AND rg.is_override = TRUE;

    DELETE FROM photo.gps_override pgo
     WHERE pgo.photo_id = _photo_id;

    IF _latitude IS NOT NULL AND _longitude IS NOT NULL THEN

        SELECT id INTO _user_id
          FROM maw.user
         WHERE username = _username;

        INSERT INTO photo.gps_override
             (
                 photo_id,
                 latitude,
                 longitude,
                 user_id,
                 updated_time,
                 has_been_reverse_geocoded
             )
        VALUES
             (
                 _photo_id,
                 _latitude,
                 _longitude,
                 _user_id,
                 _update_date,
                 FALSE
             );

    END IF;

    GET DIAGNOSTICS _rowcount = ROW_COUNT;

    RETURN _rowcount;

END;
$$;

GRANT EXECUTE
   ON FUNCTION photo.set_gps_override
   TO website;
