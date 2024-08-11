CREATE OR REPLACE FUNCTION photo.get_categories
(
    _allow_private BOOLEAN,
    _year SMALLINT DEFAULT NULL,
    _id SMALLINT DEFAULT NULL,
    _since_id SMALLINT DEFAULT NULL
)
RETURNS TABLE
(
    id SMALLINT,
    year SMALLINT,
    name VARCHAR(50),
    create_date TIMESTAMP,
    latitude REAL,
    longitude REAL,
    photo_count INTEGER,
    total_size_xs BIGINT,
    total_size_xs_sq BIGINT,
    total_size_sm BIGINT,
    total_size_md BIGINT,
    total_size_lg BIGINT,
    total_size_prt BIGINT,
    total_size_src BIGINT,
    total_size BIGINT,
    teaser_photo_path VARCHAR(255),
    teaser_photo_width SMALLINT,
    teaser_photo_height SMALLINT,
    teaser_photo_size INTEGER,
    teaser_photo_sq_path VARCHAR(255),
    teaser_photo_sq_width SMALLINT,
    teaser_photo_sq_height SMALLINT,
    teaser_photo_sq_size INTEGER
)
LANGUAGE SQL
AS $$

    SELECT id,
           year,
           name,
           create_date,
           CASE WHEN gps_latitude_ref_id = 'S' THEN CAST(-1.0 * ABS(gps_latitude) AS REAL)
                ELSE ABS(gps_latitude)
                 END AS latitude,
           CASE WHEN gps_longitude_ref_id = 'W' THEN CAST(-1.0 * ABS(gps_longitude) AS REAL)
                ELSE ABS(gps_longitude)
                 END AS longitude,
           photo_count,
           total_size_xs,
           total_size_xs_sq,
           total_size_sm,
           total_size_md,
           total_size_lg,
           total_size_prt,
           total_size_src,
           COALESCE(total_size_xs, 0) +
               COALESCE(total_size_xs_sq, 0) +
               COALESCE(total_size_sm, 0) +
               COALESCE(total_size_md, 0) +
               COALESCE(total_size_lg, 0) +
               COALESCE(total_size_prt, 0) +
               COALESCE(total_size_src, 0) AS total_size,
           teaser_photo_path,
           teaser_photo_height,
           teaser_photo_width,
           teaser_photo_size,
           teaser_photo_sq_path,
           teaser_photo_sq_height,
           teaser_photo_sq_width,
           teaser_photo_sq_size
      FROM photo.category
     WHERE (_allow_private OR is_private = FALSE)
       AND (_year IS NULL OR year = _year)
       AND (_id IS NULL OR id = _id)
       AND (_since_id IS NULL OR id > _since_id)
     ORDER BY id;

$$;

GRANT EXECUTE
   ON FUNCTION photo.get_categories
   TO website;
