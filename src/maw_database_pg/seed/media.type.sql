DO
$$
BEGIN

    IF NOT EXISTS (SELECT 1 FROM media.type) THEN
        INSERT INTO media.type (id, code, name) VALUES (1, 'P', 'Photo');
        INSERT INTO media.type (id, code, name) VALUES (2, 'V', 'Video');
    END IF;

END
$$
