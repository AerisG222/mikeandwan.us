DO
$$
BEGIN

    IF EXISTS (SELECT 1
                 FROM information_schema.columns
                WHERE table_schema = 'blog'
                  AND table_name = 'blog'
                  AND column_name = 'id'
                  AND data_type = 'smallint') THEN

        ALTER TABLE blog.post
         DROP CONSTRAINT IF EXISTS fk_blog_post_blog;

    END IF;

END
$$;
