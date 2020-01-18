DO
$$
BEGIN
   IF NOT EXISTS (SELECT 1
                    FROM pg_catalog.pg_user
                   WHERE usename = 'svc_solr') THEN

      CREATE ROLE svc_solr WITH PASSWORD 'Welcome123' LOGIN INHERIT;

      GRANT readonly TO svc_solr;

   END IF;
END
$$;
