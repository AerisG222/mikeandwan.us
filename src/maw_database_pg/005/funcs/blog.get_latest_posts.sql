CREATE OR REPLACE FUNCTION blog.get_latest_posts
(
    _blog_id SMALLINT,
    _post_count SMALLINT
)
RETURNS TABLE
(
    id SMALLINT,
    blog_id SMALLINT,
    title VARCHAR(100),
    description TEXT,
    publish_date TIMESTAMP
)
LANGUAGE SQL
AS $$

    SELECT id,
           blog_id,
           title,
           description,
           publish_date
      FROM blog.post
     WHERE blog_id = _blog_id
     ORDER BY publish_date DESC
     LIMIT _post_count;

$$;

GRANT EXECUTE
   ON FUNCTION blog.get_latest_posts
   TO website;
