CREATE OR REPLACE FUNCTION blog.get_posts
(
    _blog_id UUID DEFAULT NULL,
    _id UUID DEFAULT NULL,
    _post_count SMALLINT DEFAULT NULL
)
RETURNS TABLE
(
    id UUID,
    blog_id UUID,
    title TEXT,
    content TEXT,
    published TIMESTAMP
)
LANGUAGE SQL
AS $$

    SELECT id,
           blog_id,
           title,
           content,
           published
      FROM blog.post
     WHERE (_blog_id IS NULL OR blog_id = _blog_id)
       AND (_id IS NULL OR id = _id)
     ORDER BY published DESC
     LIMIT _post_count;

$$;

GRANT EXECUTE
   ON FUNCTION blog.get_posts
   TO website;
