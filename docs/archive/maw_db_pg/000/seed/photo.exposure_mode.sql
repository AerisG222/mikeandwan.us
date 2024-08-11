DO
$$
BEGIN

    IF NOT EXISTS (SELECT 1 FROM photo.exposure_mode) THEN
        
        INSERT INTO photo.exposure_mode (id, name) VALUES (0, 'Auto');
        INSERT INTO photo.exposure_mode (id, name) VALUES (1, 'Manual');
        INSERT INTO photo.exposure_mode (id, name) VALUES (2, 'Auto Bracket');
        
    END IF;

END
$$
