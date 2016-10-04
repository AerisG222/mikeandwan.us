DO
$$
BEGIN

    IF NOT EXISTS (SELECT 1 FROM photo.scene_type) THEN

        INSERT INTO photo.scene_type (id, name) VALUES (1, 'Directly photographed');

    END IF;

END
$$
