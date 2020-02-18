CREATE OR REPLACE FUNCTION photo.get_gps
(
    _photo_id INTEGER
)
RETURNS TABLE
(
    source_latitude REAL,
    source_longitude REAL,
    override_latitude REAL,
    override_longitude REAL
)
LANGUAGE SQL
AS $$

    SELECT p.gps_latitude AS source_latitude,
           p.gps_longitude AS source_longitude,
           pgo.latitude AS override_latitude,
           pgo.longitude AS override_longitude
      FROM photo.photo p
      LEFT OUTER JOIN photo.gps_override pgo
              ON p.id = pgo.photo_id
     WHERE p.id = _photo_id;

$$;

GRANT EXECUTE
   ON FUNCTION photo.get_gps
   TO website;
