DO
$$
BEGIN

    IF EXISTS (SELECT 1
                 FROM information_schema.columns
                WHERE table_schema = 'blog'
                  AND table_name = 'post'
                  AND column_name = 'id'
                  AND data_type = 'smallint') THEN

        ALTER TABLE blog.post
         DROP CONSTRAINT IF EXISTS pk_blog_post;

        DROP INDEX IF EXISTS  ix_blog_post_blog_id_published;

        ALTER TABLE blog.post
             RENAME TO old_post;

        ALTER TABLE blog.old_post
          ADD COLUMN newid UUID;

        UPDATE blog.old_post
           SET newid = uuid_generate_v7()
         WHERE id IN
             (
               SELECT id
                 FROM blog.old_post
                ORDER BY id
                  FOR UPDATE
             );

    END IF;

END
$$;
