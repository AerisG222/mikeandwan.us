CREATE OR REPLACE FUNCTION blog.get_post
(
    _id SMALLINT
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
     WHERE id = _id;

$$;

GRANT EXECUTE
   ON FUNCTION blog.get_post
   TO website;
