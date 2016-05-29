DO
$$
BEGIN

    IF NOT EXISTS (SELECT 1 FROM photo.gps_status) THEN
        
        INSERT INTO photo.gps_status (id, name) VALUES ('A', 'Measurement Active');
        INSERT INTO photo.gps_status (id, name) VALUES ('V', 'Measurement Void');

    END IF;

END
$$
