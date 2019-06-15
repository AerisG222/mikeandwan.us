CREATE OR REPLACE FUNCTION photo.get_photos
(
    _allow_private BOOLEAN,
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

    SELECT id,
           category_id,
           create_date,
           gps_latitude AS latitude,
           gps_longitude AS longitude,
           xs_path,
           xs_height,
           xs_width,
           xs_size,
           xs_sq_path,
           xs_sq_height,
           xs_sq_width,
           xs_sq_size,
           sm_path,
           sm_height,
           sm_width,
           sm_size,
           md_path,
           md_height,
           md_width,
           md_size,
           lg_path,
           lg_height,
           lg_width,
           lg_size,
           prt_path,
           prt_height,
           prt_width,
           prt_size,
           src_path,
           src_height,
           src_width,
           src_size
      FROM photo.photo
     WHERE (_allow_private OR is_private = FALSE)
       AND (_category_id IS NULL OR category_id = _category_id)
       AND (_id IS NULL OR id = _id);

$$;

GRANT EXECUTE
   ON FUNCTION photo.get_photos
   TO website;
