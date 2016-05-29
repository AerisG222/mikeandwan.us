DO
$$
BEGIN

    IF NOT EXISTS (SELECT 1 FROM photo.metering_mode) THEN

        INSERT INTO photo.metering_mode (id, name) VALUES (0, 'Unknown');
        INSERT INTO photo.metering_mode (id, name) VALUES (1, 'Average');
        INSERT INTO photo.metering_mode (id, name) VALUES (2, 'Center-weighted average');
        INSERT INTO photo.metering_mode (id, name) VALUES (3, 'Spot');
        INSERT INTO photo.metering_mode (id, name) VALUES (4, 'Multi-spot');
        INSERT INTO photo.metering_mode (id, name) VALUES (5, 'Multi-segment');
        INSERT INTO photo.metering_mode (id, name) VALUES (6, 'Partial');
        INSERT INTO photo.metering_mode (id, name) VALUES (255, 'Other');

    END IF;

END
$$
