DROP FUNCTION IF EXISTS video.get_videos(BOOLEAN, SMALLINT, SMALLINT);

CREATE OR REPLACE FUNCTION video.get_videos
(
    _roles TEXT[],
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
    scaled_size BIGINT,
    full_path VARCHAR(255),
    full_height SMALLINT,
    full_width SMALLINT,
    full_size BIGINT,
    raw_path VARCHAR(255),
    raw_height SMALLINT,
    raw_width SMALLINT,
    raw_size BIGINT
)
LANGUAGE SQL
AS $$

    SELECT DISTINCT
           v.id,
           v.category_id,
           v.duration,
           v.create_date,
           COALESCE(vgo.latitude, v.gps_latitude) AS latitude,
           COALESCE(vgo.longitude, v.gps_longitude) AS longitude,
           v.thumb_path,
           v.thumb_height,
           v.thumb_width,
           v.thumb_size,
           v.thumb_sq_path,
           v.thumb_sq_height,
           v.thumb_sq_width,
           v.thumb_sq_size,
           v.scaled_path,
           v.scaled_height,
           v.scaled_width,
           v.scaled_size,
           v.full_path,
           v.full_height,
           v.full_width,
           v.full_size,
           v.raw_path,
           v.raw_height,
           v.raw_width,
           v.raw_size
      FROM video.video v
     INNER JOIN video.category_role cr ON v.category_id = cr.category_id
     INNER JOIN maw.role r ON cr.role_id = r.id
      LEFT OUTER JOIN video.gps_override vgo ON v.id = vgo.video_id
     WHERE r.name = ANY(_roles)
       AND (_category_id IS NULL OR v.category_id = _category_id)
       AND (_id IS NULL OR v.id = _id);

$$;

GRANT EXECUTE
   ON FUNCTION video.get_videos
   TO website;
