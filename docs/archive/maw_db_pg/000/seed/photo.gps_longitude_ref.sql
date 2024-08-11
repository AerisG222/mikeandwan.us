DO
$$
BEGIN

    IF NOT EXISTS (SELECT 1 FROM photo.gps_longitude_ref) THEN
        
        INSERT INTO photo.gps_longitude_ref (id, name) VALUES ('E', 'East');
        INSERT INTO photo.gps_longitude_ref (id, name) VALUES ('W', 'West');

    END IF;

END
$$
