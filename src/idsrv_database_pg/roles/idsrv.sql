DO
$$
BEGIN
   IF NOT EXISTS (SELECT 1
                    FROM pg_catalog.pg_roles
                   WHERE rolname = 'idsrv') THEN

      CREATE ROLE idsrv;
      
   END IF;
END
$$;
