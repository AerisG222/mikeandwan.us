CREATE OR REPLACE FUNCTION video.get_source_gps
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

    SELECT v.gps_latitude AS latitude,
           v.gps_longitude AS longitude
      FROM video.video v
     WHERE v.id = _video_id;

$$;

GRANT EXECUTE
   ON FUNCTION video.get_source_gps
   TO website;
