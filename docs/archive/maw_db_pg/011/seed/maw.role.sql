DO
$$
BEGIN

    IF NOT EXISTS (SELECT 1 FROM maw.role WHERE name = 'demo') THEN

        INSERT INTO maw.role(name, description) VALUES ('demo', 'Role used to demonstrate app functionality with access to test data');

    END IF;

END
$$
