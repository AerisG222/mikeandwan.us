CREATE OR REPLACE FUNCTION blog.get_posts
(
    _blog_id SMALLINT DEFAULT NULL,
    _id SMALLINT DEFAULT NULL,
    _post_count SMALLINT DEFAULT NULL
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
     WHERE (_blog_id IS NULL OR blog_id = _blog_id)
       AND (_id IS NULL OR _id = id)
     ORDER BY publish_date DESC
     LIMIT _post_count;

$$;

GRANT EXECUTE
   ON FUNCTION blog.get_posts
   TO website;
