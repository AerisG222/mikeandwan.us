CREATE OR REPLACE FUNCTION blog.get_blogs
()
RETURNS TABLE
(
    id UUID,
    title TEXT,
    copyright TEXT,
    description TEXT,
    last_post_date TIMESTAMP
)
LANGUAGE SQL
AS $$

    SELECT id,
           title,
           copyright,
           description,
           (SELECT MAX(published)
              FROM blog.post
             WHERE blog_id = b.id
           ) AS last_post_date
      FROM blog.blog b;

$$;

GRANT EXECUTE
   ON FUNCTION blog.get_blogs
   TO website;
