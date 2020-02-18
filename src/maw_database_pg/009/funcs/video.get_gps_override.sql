CREATE OR REPLACE FUNCTION video.get_gps_override
(
    _video_id INTEGER
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
      FROM video.gps_override pgo
     WHERE pgo.video_id = _video_id;

$$;

GRANT EXECUTE
   ON FUNCTION video.get_gps_override
   TO website;
