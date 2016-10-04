DO
$$
BEGIN

    IF NOT EXISTS (SELECT 1 FROM photo.gain_control) THEN
        
        INSERT INTO photo.gain_control (id, name) VALUES (0, 'None');
        INSERT INTO photo.gain_control (id, name) VALUES (1, 'Low gain up');
        INSERT INTO photo.gain_control (id, name) VALUES (2, 'High gain up');
        INSERT INTO photo.gain_control (id, name) VALUES (3, 'Low gain down');
        INSERT INTO photo.gain_control (id, name) VALUES (4, 'High gain down');
        INSERT INTO photo.gain_control (id, name) VALUES (256, 'Unknown');

    END IF;

END
$$
