DO
$$
BEGIN

    IF NOT EXISTS (SELECT 1 FROM photo.scene_capture_type) THEN

        INSERT INTO photo.scene_capture_type (id, name) VALUES (0, 'Standard');
        INSERT INTO photo.scene_capture_type (id, name) VALUES (1, 'Landscape');
        INSERT INTO photo.scene_capture_type (id, name) VALUES (2, 'Portrait');
        INSERT INTO photo.scene_capture_type (id, name) VALUES (3, 'Night');

    END IF;

END
$$