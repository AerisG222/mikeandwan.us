DO
$$
BEGIN

    IF NOT EXISTS (SELECT 1 FROM photo.flash) THEN
        
        INSERT INTO photo.flash (id, name) VALUES (0, 'No Flash');
        INSERT INTO photo.flash (id, name) VALUES (1, 'Fired');
        INSERT INTO photo.flash (id, name) VALUES (5, 'Fired, Return not detected');
        INSERT INTO photo.flash (id, name) VALUES (7, 'Fired, Return detected');
        INSERT INTO photo.flash (id, name) VALUES (8, 'On, Did not fire');
        INSERT INTO photo.flash (id, name) VALUES (9, 'On, Fired');
        INSERT INTO photo.flash (id, name) VALUES (13, 'On, Return not detected');
        INSERT INTO photo.flash (id, name) VALUES (15, 'On, Return detected');
        INSERT INTO photo.flash (id, name) VALUES (16, 'Off, Did not fire');
        INSERT INTO photo.flash (id, name) VALUES (20, 'Off, Did not fire, Return not detected');
        INSERT INTO photo.flash (id, name) VALUES (24, 'Auto, Did not fire');
        INSERT INTO photo.flash (id, name) VALUES (25, 'Auto, Fired');
        INSERT INTO photo.flash (id, name) VALUES (29, 'Auto, Fired, Return not detected');
        INSERT INTO photo.flash (id, name) VALUES (31, 'Auto, Fired, Return detected');
        INSERT INTO photo.flash (id, name) VALUES (32, 'No flash function');
        INSERT INTO photo.flash (id, name) VALUES (48, 'Off, No flash function');
        INSERT INTO photo.flash (id, name) VALUES (65, 'Fired, Red-eye reduction');
        INSERT INTO photo.flash (id, name) VALUES (69, 'Fired, Red-eye reduction, Return not detected');
        INSERT INTO photo.flash (id, name) VALUES (71, 'Fired, Red-eye reduction, Return detected');
        INSERT INTO photo.flash (id, name) VALUES (73, 'On, Red-eye reduction');
        INSERT INTO photo.flash (id, name) VALUES (77, 'On, Red-eye reduction, Return not detected');
        INSERT INTO photo.flash (id, name) VALUES (79, 'On, Red-eye reduction, Return detected');
        INSERT INTO photo.flash (id, name) VALUES (80, 'Off, Red-eye reduction');
        INSERT INTO photo.flash (id, name) VALUES (88, 'Auto, Did not fire, Red-eye reduction');
        INSERT INTO photo.flash (id, name) VALUES (89, 'Auto, Fired, Red-eye reduction');
        INSERT INTO photo.flash (id, name) VALUES (93, 'Auto, Fired, Red-eye reduction, Return not detected');
        INSERT INTO photo.flash (id, name) VALUES (95, 'Auto, Fired, Red-eye reduction, Return detected');
        INSERT INTO photo.flash (id, name) VALUES (12338, 'Unknown');

    END IF;

END
$$
