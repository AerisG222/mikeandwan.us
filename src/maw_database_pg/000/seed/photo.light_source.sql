DO
$$
BEGIN

    IF NOT EXISTS (SELECT 1 FROM photo.light_source) THEN
        
        INSERT INTO photo.light_source (id, name) VALUES (0, 'Unknown');
        INSERT INTO photo.light_source (id, name) VALUES (1, 'Daylight');
        INSERT INTO photo.light_source (id, name) VALUES (2, 'Fluorescent');
        INSERT INTO photo.light_source (id, name) VALUES (3, 'Tungsten (Incandescent)'); 	
        INSERT INTO photo.light_source (id, name) VALUES (4, 'Flash');
        INSERT INTO photo.light_source (id, name) VALUES (9, 'Fine Weather'); 	
        INSERT INTO photo.light_source (id, name) VALUES (10, 'Cloudy');
        INSERT INTO photo.light_source (id, name) VALUES (11, 'Shade');
        INSERT INTO photo.light_source (id, name) VALUES (12, 'Daylight Fluorescent'); 	
        INSERT INTO photo.light_source (id, name) VALUES (13, 'Day White Fluorescent'); 	
        INSERT INTO photo.light_source (id, name) VALUES (14, 'Cool White Fluorescent'); 	
        INSERT INTO photo.light_source (id, name) VALUES (15, 'White Fluorescent'); 	
        INSERT INTO photo.light_source (id, name) VALUES (16, 'Warm White Fluorescent'); 	
        INSERT INTO photo.light_source (id, name) VALUES (17, 'Standard Light A'); 	
        INSERT INTO photo.light_source (id, name) VALUES (18, 'Standard Light B'); 	 	 
        INSERT INTO photo.light_source (id, name) VALUES (19, 'Standard Light C'); 	 	 
        INSERT INTO photo.light_source (id, name) VALUES (20, 'D55');
        INSERT INTO photo.light_source (id, name) VALUES (21, 'D65');
        INSERT INTO photo.light_source (id, name) VALUES (22, 'D75');
        INSERT INTO photo.light_source (id, name) VALUES (23, 'D50');
        INSERT INTO photo.light_source (id, name) VALUES (24, 'ISO Studio Tungsten');
        INSERT INTO photo.light_source (id, name) VALUES (255, 'Other');

    END IF;

END
$$
