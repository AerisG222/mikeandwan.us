DO
$$
BEGIN

    IF NOT EXISTS (SELECT 1 FROM photo.gps_direction_ref) THEN
        
        INSERT INTO photo.gps_direction_ref (id, name) VALUES ('M', 'Magnetic North');
        INSERT INTO photo.gps_direction_ref (id, name) VALUES ('T', 'True North');

    END IF;

END
$$
