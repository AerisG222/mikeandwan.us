CREATE OR REPLACE FUNCTION video.set_category_teaser
(
    _category_id SMALLINT,
    _video_id INTEGER
)
RETURNS TABLE
(
    rowcount BIGINT
)
LANGUAGE PLPGSQL
AS $$
DECLARE
    _category_id_for_video SMALLINT;
    _rowcount BIGINT;
BEGIN

    SELECT category_id INTO _category_id_for_video
      FROM video.video
     WHERE video.id = _video_id;

    _rowcount := 0;

    IF _category_id = _category_id_for_video THEN

        UPDATE video.category c
           SET teaser_image_width = v.thumb_width,
               teaser_image_height = v.thumb_height,
               teaser_image_path = v.thumb_path,
               teaser_image_size = v.thumb_size,
               teaser_image_sq_width = v.thumb_sq_width,
               teaser_image_sq_height = v.thumb_sq_height,
               teaser_image_sq_path = v.thumb_sq_path,
               teaser_image_sq_size = v.thumb_sq_size
          FROM video.video v
         WHERE c.id = v.category_id
           AND v.id = _video_id;

        GET DIAGNOSTICS _rowcount = ROW_COUNT;

    END IF;

    RETURN QUERY
    SELECT _rowcount;

END;
$$;

GRANT EXECUTE
   ON FUNCTION video.set_category_teaser
   TO website;
