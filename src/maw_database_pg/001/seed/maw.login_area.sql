DO
$$
BEGIN

    IF NOT EXISTS (SELECT 1 FROM maw.login_area WHERE id = 3) THEN

        INSERT INTO maw.login_area (id, name) VALUES (3, 'GitHub');
        INSERT INTO maw.login_area (id, name) VALUES (4, 'Google');
        INSERT INTO maw.login_area (id, name) VALUES (5, 'Microsoft');
        INSERT INTO maw.login_area (id, name) VALUES (6, 'Twitter');

    END IF;

END
$$
