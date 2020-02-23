CREATE OR REPLACE FUNCTION photo.set_category_teaser
(
    _category_id SMALLINT,
    _photo_id INTEGER
)
RETURNS TABLE
(
    rowcount BIGINT
)
LANGUAGE PLPGSQL
AS $$
DECLARE
    _category_id_for_photo SMALLINT;
    _rowcount BIGINT;
BEGIN

    SELECT category_id INTO _category_id_for_photo
      FROM photo.photo
     WHERE photo.id = _photo_id;

    _rowcount := 0;

    IF _category_id = _category_id_for_photo THEN

        UPDATE photo.category c
           SET teaser_photo_width = p.xs_width,
               teaser_photo_height = p.xs_height,
               teaser_photo_path = p.xs_path,
               teaser_photo_size = p.xs_size,
               teaser_photo_sq_width = p.xs_sq_width,
               teaser_photo_sq_height = p.xs_sq_height,
               teaser_photo_sq_path = p.xs_sq_path,
               teaser_photo_sq_size = p.xs_sq_size
          FROM photo.photo p
         WHERE c.id = p.category_id
           AND p.id = _photo_id;

        GET DIAGNOSTICS _rowcount = ROW_COUNT;

    END IF;

    RETURN QUERY
    SELECT _rowcount;

END;
$$;

GRANT EXECUTE
   ON FUNCTION photo.set_category_teaser
   TO website;
