DO
$$
BEGIN

    IF EXISTS (SELECT 1
                 FROM information_schema.columns
                WHERE table_schema = 'blog'
                  AND table_name = 'old_blog') THEN

        INSERT INTO blog.blog (
               id,
               title,
               copyright,
               description
        )
        SELECT newid,
               title,
               copyright,
               description
          FROM blog.old_blog
         ORDER BY id;

    END IF;

END
$$;
