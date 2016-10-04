DO
$$
BEGIN

    IF NOT EXISTS (SELECT 1 FROM photo.auto_focus) THEN

        INSERT INTO photo.auto_focus (id, name) VALUES (0, 'Off');
        INSERT INTO photo.auto_focus (id, name) VALUES (1, 'On');

    END IF;

END
$$
