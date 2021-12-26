DO
$$
DECLARE
    _cat_cursor REFCURSOR;
    _photo_cursor REFCURSOR;
    _cat_id SMALLINT;
    _photo_id INTEGER;
BEGIN

    IF NOT EXISTS (SELECT 1 FROM photo.category_role WHERE role_id = 3) THEN
        CREATE TEMP TABLE tmp_photo_category
            AS SELECT *
                 FROM photo.category
                WHERE id IN (1546, 1500, 1477, 1447, 1400);

        CREATE TEMP TABLE tmp_photo
            AS SELECT *
                 FROM photo.photo
                WHERE category_id IN (SELECT id FROM tmp_photo_category);

        OPEN _photo_cursor NO SCROLL FOR SELECT id FROM tmp_photo;
        LOOP
            FETCH _photo_cursor INTO _photo_id;
            EXIT WHEN NOT FOUND;

            RAISE NOTICE 'photoid: %', _photo_id;

            UPDATE tmp_photo
               SET id = (SELECT NEXTVAL('photo.photo_id_seq')),
                   gps_altitude = NULL,
                   gps_altitude_ref_id = NULL,
                   gps_date_time_stamp = NULL,
                   gps_direction = NULL,
                   gps_direction_ref_id = NULL,
                   gps_latitude = NULL,
                   gps_latitude_ref_id = NULL,
                   gps_longitude = NULL,
                   gps_longitude_ref_id = NULL,
                   gps_measure_mode_id = NULL,
                   gps_satellites = NULL,
                   gps_status_id = NULL,
                   gps_version_id = NULL
             WHERE id = _photo_id;
        END LOOP;
        CLOSE _photo_cursor;

        OPEN _cat_cursor NO SCROLL FOR SELECT id FROM tmp_photo_category;
        LOOP
            FETCH _cat_cursor INTO _cat_id;
            EXIT WHEN NOT FOUND;

            RAISE NOTICE 'catid: %', _cat_id;

            UPDATE tmp_photo_category
               SET id = (SELECT NEXTVAL('photo.category_id_seq'))
             WHERE id = _cat_id;

            UPDATE tmp_photo
               SET category_id = (SELECT CURRVAL('photo.category_id_seq'))
             WHERE category_id = _cat_id;
        END LOOP;
        CLOSE _cat_cursor;

        INSERT INTO photo.category
        SELECT *
          FROM tmp_photo_category;

        INSERT INTO photo.photo
        SELECT *
          FROM tmp_photo;

        INSERT INTO photo.category_role (category_id, role_id)
        SELECT id, 3
          FROM tmp_photo_category;
    END IF;

END
$$
