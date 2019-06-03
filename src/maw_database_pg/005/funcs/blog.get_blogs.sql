CREATE OR REPLACE FUNCTION blog.get_blogs
()
RETURNS TABLE
(
    id SMALLINT,
    title VARCHAR(50),
    copyright VARCHAR(50),
    description VARCHAR(250),
    last_post_date TIMESTAMP
)
LANGUAGE SQL
AS $$

    SELECT id,
           title,
           copyright,
           description,
           (SELECT MAX(publish_date)
              FROM blog.post
             WHERE blog_id = b.id
           ) AS last_post_date
      FROM blog.blog b;

$$;

GRANT EXECUTE
   ON FUNCTION blog.get_blogs
   TO website;
