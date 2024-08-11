DROP FUNCTION photo.get_categories(BOOLEAN, SMALLINT, SMALLINT, SMALLINT);

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
    teaser_photo_sq_size INTEGER,
    is_missing_gps_data BOOLEAN
)
LANGUAGE SQL
AS $$

    WITH missing_gps_categories
    AS
    (
        SELECT c.id
          FROM photo.category c
         INNER JOIN photo.photo p
                 ON c.id = p.category_id
          LEFT OUTER JOIN photo.gps_override o
                  ON p.id = o.photo_id
         WHERE COALESCE(p.gps_latitude, o.latitude) IS NULL
            OR COALESCE(p.gps_longitude, o.longitude) IS NULL
         GROUP BY c.id
        HAVING COUNT(1) > 0
    )
    SELECT c.id,
           c.year,
           c.name,
           c.create_date,
           c.gps_latitude AS latitude,
           c.gps_longitude AS longitude,
           c.photo_count,
           c.total_size_xs,
           c.total_size_xs_sq,
           c.total_size_sm,
           c.total_size_md,
           c.total_size_lg,
           c.total_size_prt,
           c.total_size_src,
           COALESCE(c.total_size_xs, 0) +
               COALESCE(c.total_size_xs_sq, 0) +
               COALESCE(c.total_size_sm, 0) +
               COALESCE(c.total_size_md, 0) +
               COALESCE(c.total_size_lg, 0) +
               COALESCE(c.total_size_prt, 0) +
               COALESCE(c.total_size_src, 0) AS total_size,
           c.teaser_photo_path,
           c.teaser_photo_height,
           c.teaser_photo_width,
           c.teaser_photo_size,
           c.teaser_photo_sq_path,
           c.teaser_photo_sq_height,
           c.teaser_photo_sq_width,
           c.teaser_photo_sq_size,
           CASE WHEN m.id IS NULL
                THEN FALSE
                ELSE TRUE
                 END AS is_missing_gps_data
      FROM photo.category c
      LEFT OUTER JOIN missing_gps_categories m
              ON m.id = c.id
     WHERE (_allow_private OR c.is_private = FALSE)
       AND (_year IS NULL OR c.year = _year)
       AND (_id IS NULL OR c.id = _id)
       AND (_since_id IS NULL OR c.id > _since_id)
     ORDER BY c.id;

$$;

GRANT EXECUTE
   ON FUNCTION photo.get_categories
   TO website;
