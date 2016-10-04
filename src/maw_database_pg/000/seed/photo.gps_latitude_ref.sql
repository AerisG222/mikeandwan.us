DO
$$
BEGIN

    IF NOT EXISTS (SELECT 1 FROM photo.gps_latitude_ref) THEN
        
        INSERT INTO photo.gps_latitude_ref (id, name) VALUES ('N', 'North');
        INSERT INTO photo.gps_latitude_ref (id, name) VALUES ('S', 'South');

    END IF;

END
$$
