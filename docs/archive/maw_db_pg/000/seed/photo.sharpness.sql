DO
$$
BEGIN

    IF NOT EXISTS (SELECT 1 FROM photo.sharpness) THEN

        INSERT INTO photo.sharpness (id, name) VALUES (0, 'Normal');
        INSERT INTO photo.sharpness (id, name) VALUES (1, 'Soft');
        INSERT INTO photo.sharpness (id, name) VALUES (2, 'Hard');
        
    END IF;

END
$$
