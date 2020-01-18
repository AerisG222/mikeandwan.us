DO
$$
BEGIN
   IF NOT EXISTS (SELECT 1
                    FROM pg_catalog.pg_roles
                   WHERE rolname = 'readonly') THEN

      CREATE ROLE readonly;

      GRANT USAGE ON SCHEMA photo TO readonly;
      GRANT USAGE ON SCHEMA video TO readonly;

      GRANT SELECT ON ALL TABLES IN SCHEMA photo TO readonly;
      GRANT SELECT ON ALL TABLES IN SCHEMA video TO readonly;

      ALTER DEFAULT PRIVILEGES IN SCHEMA photo
              GRANT SELECT ON TABLES TO readonly;

      ALTER DEFAULT PRIVILEGES IN SCHEMA video
              GRANT SELECT ON TABLES TO readonly;

   END IF;
END
$$;
