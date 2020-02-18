CREATE OR REPLACE FUNCTION photo.get_gps_override
(
    _photo_id INTEGER
)
RETURNS TABLE
(
    latitude REAL,
    longitude REAL
)
LANGUAGE SQL
AS $$

    SELECT pgo.latitude,
           pgo.longitude
      FROM photo.gps_override pgo
     WHERE pgo.photo_id = _photo_id;

$$;

GRANT EXECUTE
   ON FUNCTION photo.get_gps_override
   TO website;
