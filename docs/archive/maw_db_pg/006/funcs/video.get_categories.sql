CREATE OR REPLACE FUNCTION video.get_categories
(
    _allow_private BOOLEAN,
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
    teaser_image_sq_size INTEGER
)
LANGUAGE SQL
AS $$

    SELECT id,
           year,
           name,
           create_date,
           gps_latitude AS latitude,
           gps_longitude AS longitude,
           video_count,
           total_duration,
           total_size_thumb,
           total_size_thumb_sq,
           total_size_scaled,
           total_size_full,
           total_size_raw,
           COALESCE(total_size_thumb, 0) +
               COALESCE(total_size_thumb_sq, 0) +
               COALESCE(total_size_scaled, 0) +
               COALESCE(total_size_full, 0) +
               COALESCE(total_size_raw, 0) AS total_size,
           teaser_image_path,
           teaser_image_width,
           teaser_image_height,
           teaser_image_size,
           teaser_image_sq_path,
           teaser_image_sq_width,
           teaser_image_sq_height,
           teaser_image_sq_size
      FROM video.category
     WHERE (_allow_private OR is_private = FALSE)
       AND (_year IS NULL OR year = _year)
       AND (_id IS NULL OR id = _id)
     ORDER BY id;

$$;

GRANT EXECUTE
   ON FUNCTION video.get_categories
   TO website;
