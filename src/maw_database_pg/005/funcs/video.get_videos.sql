CREATE OR REPLACE FUNCTION video.get_videos
(
    _allow_private BOOLEAN,
    _category_id SMALLINT DEFAULT NULL,
    _id SMALLINT DEFAULT NULL
)
RETURNS TABLE
(
    id SMALLINT,
    category_id SMALLINT,
    duration SMALLINT,
    create_date TIMESTAMP,
    latitude REAL,
    longitude REAL,
    thumb_path VARCHAR(255),
    thumb_height SMALLINT,
    thumb_width SMALLINT,
    thumb_size INTEGER,
    thumb_sq_path VARCHAR(255),
    thumb_sq_height SMALLINT,
    thumb_sq_width SMALLINT,
    thumb_sq_size INTEGER,
    scaled_path VARCHAR(255),
    scaled_height SMALLINT,
    scaled_width SMALLINT,
    scaled_size INTEGER,
    full_path VARCHAR(255),
    full_height SMALLINT,
    full_width SMALLINT,
    full_size INTEGER,
    raw_path VARCHAR(255),
    raw_height SMALLINT,
    raw_width SMALLINT,
    raw_size INTEGER
)
LANGUAGE SQL
AS $$

    SELECT id,
           category_id,
           duration,
           create_date,
           gps_latitude AS latitude,
           gps_longitude AS longitude,
           thumb_path,
           thumb_height,
           thumb_width,
           thumb_size,
           thumb_sq_path,
           thumb_sq_height,
           thumb_sq_width,
           thumb_sq_size,
           scaled_path,
           scaled_height,
           scaled_width,
           scaled_size,
           full_path,
           full_height,
           full_width,
           full_size,
           raw_path,
           raw_height,
           raw_width,
           raw_size
      FROM video.video
     WHERE (_allow_private OR is_private = FALSE)
       AND (_category_id IS NULL OR category_id = _category_id)
       AND (_id IS NULL OR id = _id);

$$;

GRANT EXECUTE
   ON FUNCTION video.get_videos
   TO website;
