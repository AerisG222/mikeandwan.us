CREATE OR REPLACE FUNCTION blog.add_post
(
    _blog_id SMALLINT,
    _title VARCHAR(100),
    _description TEXT,
    _publish_date TIMESTAMP
)
RETURNS SMALLINT
LANGUAGE SQL
AS $$

    INSERT INTO blog.post
         (
           blog_id,
           title,
           description,
           publish_date
         )
    VALUES
         (
             _blog_id,
             _title,
             _description,
             _publish_date
         )
    RETURNING id;

$$;

GRANT EXECUTE
   ON FUNCTION blog.add_post
   TO website;
