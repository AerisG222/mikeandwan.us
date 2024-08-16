DO
$$
BEGIN

    IF EXISTS (SELECT 1
                 FROM information_schema.columns
                WHERE table_schema = 'blog'
                  AND table_name = 'blog'
                  AND column_name = 'id'
                  AND data_type = 'smallint') THEN

        ALTER TABLE blog.blog
         DROP CONSTRAINT IF EXISTS pk_blog_blog;

        ALTER TABLE blog.blog
         DROP CONSTRAINT IF EXISTS uq_blog_blog$title;

        ALTER TABLE blog.blog
             RENAME TO old_blog;

        ALTER TABLE blog.old_blog
          ADD COLUMN newid UUID;

        UPDATE blog.old_blog
           SET newid = uuid_generate_v7()
         WHERE id IN
             (
               SELECT id
                 FROM blog.old_blog
                ORDER BY id
                  FOR UPDATE
             );

    END IF;

END
$$;
