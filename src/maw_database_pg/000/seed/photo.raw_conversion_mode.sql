DO
$$
BEGIN

    IF NOT EXISTS (SELECT 1 FROM photo.raw_conversion_mode) THEN

        INSERT INTO photo.raw_conversion_mode (id, name) VALUES (0, 'None');
        INSERT INTO photo.raw_conversion_mode (id, name) VALUES (1, 'Brightening');
        INSERT INTO photo.raw_conversion_mode (id, name) VALUES (2, 'Non-Brightening');

    END IF;

END
$$
