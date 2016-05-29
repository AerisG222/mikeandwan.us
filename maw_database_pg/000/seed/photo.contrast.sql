DO
$$
BEGIN

    IF NOT EXISTS (SELECT 1 FROM photo.contrast) THEN
        
        INSERT INTO photo.contrast (id, name) VALUES (0, 'Normal');
        INSERT INTO photo.contrast (id, name) VALUES (1, 'Low');
        INSERT INTO photo.contrast (id, name) VALUES (2, 'High');
        
    END IF;

END
$$
