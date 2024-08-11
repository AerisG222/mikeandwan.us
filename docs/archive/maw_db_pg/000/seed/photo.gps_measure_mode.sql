DO
$$
BEGIN

    IF NOT EXISTS (SELECT 1 FROM photo.gps_measure_mode) THEN
        
        INSERT INTO photo.gps_measure_mode (id, name) VALUES ('2', '2-Dimensional Measurement');
        INSERT INTO photo.gps_measure_mode (id, name) VALUES ('3', '3-Dimensional Measurement');

    END IF;

END
$$
