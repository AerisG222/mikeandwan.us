DO
$$
BEGIN
    IF NOT EXISTS (SELECT 1
                     FROM pg_catalog.pg_roles
                    WHERE rolname = 'website') THEN

        CREATE ROLE website;

    END IF;
END
$$;
