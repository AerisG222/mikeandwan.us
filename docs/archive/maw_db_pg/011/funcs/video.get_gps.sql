DROP FUNCTION IF EXISTS video.get_gps(INTEGER);

CREATE OR REPLACE FUNCTION video.get_gps
(
    _video_id SMALLINT,
    _roles TEXT[]
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

    SELECT DISTINCT
           v.gps_latitude AS source_latitude,
           v.gps_longitude AS source_longitude,
           pgo.latitude AS override_latitude,
           pgo.longitude AS override_longitude
      FROM video.video v
     INNER JOIN video.category_role cr ON v.category_id = cr.category_id
     INNER JOIN maw.role r ON cr.role_id = r.id
      LEFT OUTER JOIN video.gps_override pgo ON v.id = pgo.video_id
     WHERE v.id = _video_id
       AND r.name = ANY(_roles);

$$;

GRANT EXECUTE
   ON FUNCTION video.get_gps
   TO website;
