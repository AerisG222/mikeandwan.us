DROP FUNCTION IF EXISTS photo.get_photos(BOOLEAN, SMALLINT, INTEGER);

CREATE OR REPLACE FUNCTION photo.get_photos
(
    _roles TEXT[],
    _category_id SMALLINT DEFAULT NULL,
    _id INTEGER DEFAULT NULL
)
RETURNS TABLE
(
    id INTEGER,
    category_id SMALLINT,
    create_date TIMESTAMP,
    latitude REAL,
    longitude REAL,
    xs_path VARCHAR(255),
    xs_height SMALLINT,
    xs_width SMALLINT,
    xs_size INTEGER,
    xs_sq_path VARCHAR(255),
    xs_sq_height SMALLINT,
    xs_sq_width SMALLINT,
    xs_sq_size INTEGER,
    sm_path VARCHAR(255),
    sm_height SMALLINT,
    sm_width SMALLINT,
    sm_size INTEGER,
    md_path VARCHAR(255),
    md_height SMALLINT,
    md_width SMALLINT,
    md_size INTEGER,
    lg_path VARCHAR(255),
    lg_height SMALLINT,
    lg_width SMALLINT,
    lg_size INTEGER,
    prt_path VARCHAR(255),
    prt_height SMALLINT,
    prt_width SMALLINT,
    prt_size INTEGER,
    src_path VARCHAR(255),
    src_height SMALLINT,
    src_width SMALLINT,
    src_size INTEGER
)
LANGUAGE SQL
AS $$

    SELECT DISTINCT
           p.id,
           p.category_id,
           p.create_date,
           COALESCE(pgo.latitude, p.gps_latitude) AS latitude,
           COALESCE(pgo.longitude, p.gps_longitude) AS longitude,
           p.xs_path,
           p.xs_height,
           p.xs_width,
           p.xs_size,
           p.xs_sq_path,
           p.xs_sq_height,
           p.xs_sq_width,
           p.xs_sq_size,
           p.sm_path,
           p.sm_height,
           p.sm_width,
           p.sm_size,
           p.md_path,
           p.md_height,
           p.md_width,
           p.md_size,
           p.lg_path,
           p.lg_height,
           p.lg_width,
           p.lg_size,
           p.prt_path,
           p.prt_height,
           p.prt_width,
           p.prt_size,
           p.src_path,
           p.src_height,
           p.src_width,
           p.src_size
      FROM photo.photo p
     INNER JOIN photo.category_role cr ON p.category_id = cr.category_id
     INNER JOIN maw.role r ON cr.role_id = r.id
      LEFT OUTER JOIN photo.gps_override pgo ON p.id = pgo.photo_id
     WHERE r.name = ANY(_roles)
       AND (_category_id IS NULL OR p.category_id = _category_id)
       AND (_id IS NULL OR p.id = _id)
     ORDER BY p.lg_path;

$$;

GRANT EXECUTE
   ON FUNCTION photo.get_photos
   TO website;
