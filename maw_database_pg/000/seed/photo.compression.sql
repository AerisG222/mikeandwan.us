DO
$$
BEGIN

    IF NOT EXISTS (SELECT 1 FROM photo.compression) THEN
        
        INSERT INTO photo.compression (id, name) VALUES (1, 'Uncompressed');
        INSERT INTO photo.compression (id, name) VALUES (2, 'CCITT 1D');
        INSERT INTO photo.compression (id, name) VALUES (3, 'T4/Group 3 Fax');
        INSERT INTO photo.compression (id, name) VALUES (4, 'T6/Group 4 Fax');
        INSERT INTO photo.compression (id, name) VALUES (5, 'LZW');
        INSERT INTO photo.compression (id, name) VALUES (6, 'JPEG (old-style)');
        INSERT INTO photo.compression (id, name) VALUES (7, 'JPEG');
        INSERT INTO photo.compression (id, name) VALUES (8, 'Adobe Deflate');
        INSERT INTO photo.compression (id, name) VALUES (9, 'JBIG B&W');
        INSERT INTO photo.compression (id, name) VALUES (10, 'JBIG Color');
        INSERT INTO photo.compression (id, name) VALUES (99, 'JPEG');
        INSERT INTO photo.compression (id, name) VALUES (262, 'Kodak 262');
        INSERT INTO photo.compression (id, name) VALUES (32766, 'Next');
        INSERT INTO photo.compression (id, name) VALUES (32767, 'Sony ARW Compressed');
        INSERT INTO photo.compression (id, name) VALUES (32769, 'Packed RAW');
        INSERT INTO photo.compression (id, name) VALUES (32770, 'Samsung SRW Compressed');
        INSERT INTO photo.compression (id, name) VALUES (32771, 'CCIRLEW');
        INSERT INTO photo.compression (id, name) VALUES (32772, 'Samsung SRW Compressed 2');
        INSERT INTO photo.compression (id, name) VALUES (32773, 'PackBits');
        INSERT INTO photo.compression (id, name) VALUES (32809, 'Thunderscan');
        INSERT INTO photo.compression (id, name) VALUES (32867, 'Kodak KDC Compressed');
        INSERT INTO photo.compression (id, name) VALUES (32895, 'IT8CTPAD');
        INSERT INTO photo.compression (id, name) VALUES (32896, 'IT8LW');
        INSERT INTO photo.compression (id, name) VALUES (32897, 'IT8MP');
        INSERT INTO photo.compression (id, name) VALUES (32898, 'IT8BL');
        INSERT INTO photo.compression (id, name) VALUES (32908, 'PixarFilm');
        INSERT INTO photo.compression (id, name) VALUES (32909, 'PixarLog');
        INSERT INTO photo.compression (id, name) VALUES (32946, 'Deflate');
        INSERT INTO photo.compression (id, name) VALUES (32947, 'DCS');
        INSERT INTO photo.compression (id, name) VALUES (34661, 'JBIG');
        INSERT INTO photo.compression (id, name) VALUES (34676, 'SGILog');
        INSERT INTO photo.compression (id, name) VALUES (34677, 'SGILog24');
        INSERT INTO photo.compression (id, name) VALUES (34712, 'JPEG 2000');
        INSERT INTO photo.compression (id, name) VALUES (34713, 'Nikon NEF Compressed');
        INSERT INTO photo.compression (id, name) VALUES (34715, 'JBIG2 TIFF FX');
        INSERT INTO photo.compression (id, name) VALUES (34718, 'Microsoft Document Imaging (MDI) Binary Level Codec');
        INSERT INTO photo.compression (id, name) VALUES (34719, 'Microsoft Document Imaging (MDI) Progressive Transform Codec');
        INSERT INTO photo.compression (id, name) VALUES (34720, 'Microsoft Document Imaging (MDI) Vector');
        INSERT INTO photo.compression (id, name) VALUES (34892, 'Lossy JPEG');
        INSERT INTO photo.compression (id, name) VALUES (65000, 'Kodak DCR Compressed');
        INSERT INTO photo.compression (id, name) VALUES (65535, 'Pentax PEF Compressed');

    END IF;

END
$$
