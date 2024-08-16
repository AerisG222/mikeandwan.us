DO
$$
BEGIN

    IF EXISTS (SELECT 1
                 FROM information_schema.columns
                WHERE table_schema = 'blog'
                  AND table_name = 'old_post') THEN

        INSERT INTO blog.post (
               id,
               blog_id,
               title,
               content,
               published
        )
        SELECT newid,
               (SELECT newid FROM blog.old_blog b where b.id = blog_id),
               title,
               description,
               publish_date
          FROM blog.old_post
         ORDER BY id;

    END IF;

END
$$;
