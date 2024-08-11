DROP FUNCTION IF EXISTS video.get_categories(BOOLEAN, SMALLINT, SMALLINT);

CREATE OR REPLACE FUNCTION video.get_categories
(
    _roles TEXT[],
    _year SMALLINT DEFAULT NULL,
    _id SMALLINT DEFAULT NULL
)
RETURNS TABLE
(
    id SMALLINT,
    year SMALLINT,
    name VARCHAR(50),
    create_date TIMESTAMP,
    latitude REAL,
    longitude REAL,
    video_count INTEGER,
    total_duration INTEGER,
    total_size_thumb BIGINT,
    total_size_thumb_sq BIGINT,
    total_size_scaled BIGINT,
    total_size_full BIGINT,
    total_size_raw BIGINT,
    total_size BIGINT,
    teaser_image_path VARCHAR(255),
    teaser_image_width SMALLINT,
    teaser_image_height SMALLINT,
    teaser_image_size INTEGER,
    teaser_image_sq_path VARCHAR(255),
    teaser_image_sq_width SMALLINT,
    teaser_image_sq_height SMALLINT,
    teaser_image_sq_size INTEGER,
    is_missing_gps_data BOOLEAN
)
LANGUAGE SQL
AS $$

    WITH missing_gps_categories
    AS
    (
        SELECT c.id
          FROM video.category c
         INNER JOIN video.video v
                 ON c.id = v.category_id
          LEFT OUTER JOIN video.gps_override o
                  ON v.id = o.video_id
         WHERE COALESCE(v.gps_latitude, o.latitude) IS NULL
            OR COALESCE(v.gps_longitude, o.longitude) IS NULL
         GROUP BY c.id
        HAVING COUNT(1) > 0
    )
    SELECT DISTINCT
           c.id,
           c.year,
           c.name,
           c.create_date,
           c.gps_latitude AS latitude,
           c.gps_longitude AS longitude,
           c.video_count,
           c.total_duration,
           c.total_size_thumb,
           c.total_size_thumb_sq,
           c.total_size_scaled,
           c.total_size_full,
           c.total_size_raw,
           COALESCE(c.total_size_thumb, 0) +
               COALESCE(c.total_size_thumb_sq, 0) +
               COALESCE(c.total_size_scaled, 0) +
               COALESCE(c.total_size_full, 0) +
               COALESCE(c.total_size_raw, 0) AS total_size,
           c.teaser_image_path,
           c.teaser_image_width,
           c.teaser_image_height,
           c.teaser_image_size,
           c.teaser_image_sq_path,
           c.teaser_image_sq_width,
           c.teaser_image_sq_height,
           c.teaser_image_sq_size,
           CASE WHEN m.id IS NULL
                THEN FALSE
                ELSE TRUE
                 END AS is_missing_gps_data
      FROM video.category c
      INNER JOIN video.category_role cr ON c.id = cr.category_id
     INNER JOIN maw.role r ON cr.role_id = r.id
      LEFT OUTER JOIN missing_gps_categories m
              ON m.id = c.id
     WHERE (_roles IS NULL OR r.name = ANY(_roles))
       AND (_year IS NULL OR c.year = _year)
       AND (_id IS NULL OR c.id = _id)
     ORDER BY c.id;

$$;

GRANT EXECUTE
   ON FUNCTION video.get_categories
   TO website;
