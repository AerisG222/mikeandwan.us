DO
$$
BEGIN

    IF NOT EXISTS (SELECT 1 FROM media.type) THEN
        INSERT INTO media.type (id, code, name) VALUES (uuid_generate_v7(), 'P', 'Photo');
        INSERT INTO media.type (id, code, name) VALUES (uuid_generate_v7(), 'V', 'Video');
    END IF;

END
$$
