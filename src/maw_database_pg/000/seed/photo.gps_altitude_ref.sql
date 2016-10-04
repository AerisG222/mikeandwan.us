DO
$$
BEGIN

    IF NOT EXISTS (SELECT 1 FROM photo.gps_altitude_ref) THEN
        
        INSERT INTO photo.gps_altitude_ref (id, name) VALUES (0, 'Above Sea Level');
        INSERT INTO photo.gps_altitude_ref (id, name) VALUES (1, 'Below Sea Level');

    END IF;

END
$$
