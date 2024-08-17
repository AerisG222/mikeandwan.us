DO
$$
BEGIN
   IF NOT EXISTS (SELECT 1
                    FROM pg_catalog.pg_user
                   WHERE usename = 'svc_www_maw') THEN

      CREATE ROLE svc_www_maw LOGIN INHERIT;
      
   END IF;

   GRANT idsrv TO svc_www_maw;
END
$$;
