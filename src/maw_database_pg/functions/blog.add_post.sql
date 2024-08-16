CREATE OR REPLACE FUNCTION blog.add_post
(
    _blog_id UUID,
    _title TEXT,
    _content TEXT,
    _published TIMESTAMP
)
RETURNS UUID
LANGUAGE SQL
AS $$

    INSERT INTO blog.post
         (
           blog_id,
           title,
           content,
           published
         )
    VALUES
         (
             _blog_id,
             _title,
             _content,
             _published
         )
    RETURNING id;

$$;

GRANT EXECUTE
   ON FUNCTION blog.add_post
   TO website;
