DO
$$
BEGIN

    IF NOT EXISTS (SELECT 1 FROM photo.orientation) THEN

        INSERT INTO photo.orientation (id, name) VALUES (1, 'Horizontal (normal)');
        INSERT INTO photo.orientation (id, name) VALUES (2, 'Mirror horizontal');
        INSERT INTO photo.orientation (id, name) VALUES (3, 'Rotate 180');
        INSERT INTO photo.orientation (id, name) VALUES (4, 'Mirror vertical');
        INSERT INTO photo.orientation (id, name) VALUES (5, 'Mirror horizontal and rotate 270 CW');
        INSERT INTO photo.orientation (id, name) VALUES (6, 'Rotate 90 CW');
        INSERT INTO photo.orientation (id, name) VALUES (7, 'Mirror horizontal and rotate 90 CW');
        INSERT INTO photo.orientation (id, name) VALUES (8, 'Rotate 270 CW');

    END IF;

END
$$
