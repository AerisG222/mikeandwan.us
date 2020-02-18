CREATE OR REPLACE FUNCTION video.get_gps
(
    _video_id INTEGER
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

    SELECT v.gps_latitude AS source_latitude,
           v.gps_longitude AS source_longitude,
           pgo.latitude AS override_latitude,
           pgo.longitude AS override_longitude
      FROM video.video v
      LEFT OUTER JOIN video.gps_override pgo
              ON v.id = pgo.video_id
     WHERE v.id = _video_id;

$$;

GRANT EXECUTE
   ON FUNCTION video.get_gps
   TO website;
