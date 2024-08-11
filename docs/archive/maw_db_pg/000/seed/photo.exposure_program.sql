DO
$$
BEGIN

    IF NOT EXISTS (SELECT 1 FROM photo.exposure_program) THEN
        
        INSERT INTO photo.exposure_program (id, name) VALUES (0, 'Not Defined');
        INSERT INTO photo.exposure_program (id, name) VALUES (1, 'Manual');
        INSERT INTO photo.exposure_program (id, name) VALUES (2, 'Program AE');
        INSERT INTO photo.exposure_program (id, name) VALUES (3, 'Aperture-priority AE');
        INSERT INTO photo.exposure_program (id, name) VALUES (4, 'Shutter speed priority AE');
        INSERT INTO photo.exposure_program (id, name) VALUES (5, 'Creative (Slow speed)');
        INSERT INTO photo.exposure_program (id, name) VALUES (6, 'Action (High speed)');
        INSERT INTO photo.exposure_program (id, name) VALUES (7, 'Portrait');
        INSERT INTO photo.exposure_program (id, name) VALUES (8, 'Landscape');
        INSERT INTO photo.exposure_program (id, name) VALUES (9, 'Bulb');

    END IF;

END
$$
