DO
$$
BEGIN

    IF NOT EXISTS (SELECT 1 FROM photo.sensing_method) THEN

        INSERT INTO photo.sensing_method (id, name) VALUES (1, 'Monochrome area');
        INSERT INTO photo.sensing_method (id, name) VALUES (2, 'One-chip color area');
        INSERT INTO photo.sensing_method (id, name) VALUES (3, 'Two-chip color area');
        INSERT INTO photo.sensing_method (id, name) VALUES (4, 'Three-chip color area');
        INSERT INTO photo.sensing_method (id, name) VALUES (5, 'Color sequential area');
        INSERT INTO photo.sensing_method (id, name) VALUES (6, 'Monochrome linear');
        INSERT INTO photo.sensing_method (id, name) VALUES (7, 'Trilinear');
        INSERT INTO photo.sensing_method (id, name) VALUES (8, 'Color sequential linear');

    END IF;

END
$$
