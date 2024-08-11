DROP FUNCTION IF EXISTS photo.get_gps(INTEGER);

CREATE OR REPLACE FUNCTION photo.get_gps
(
    _photo_id INTEGER,
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
           p.gps_latitude AS source_latitude,
           p.gps_longitude AS source_longitude,
           pgo.latitude AS override_latitude,
           pgo.longitude AS override_longitude
      FROM photo.photo p
     INNER JOIN photo.category_role cr ON p.category_id = cr.category_id
     INNER JOIN maw.role r ON cr.role_id = r.id
      LEFT OUTER JOIN photo.gps_override pgo ON p.id = pgo.photo_id
     WHERE p.id = _photo_id
       AND r.name = ANY(_roles);

$$;

GRANT EXECUTE
   ON FUNCTION photo.get_gps
   TO website;
