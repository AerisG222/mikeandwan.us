CREATE TABLE IF NOT EXISTS blog.post (
    id UUID NOT NULL,
    blog_id UUID NOT NULL,
    title TEXT NOT NULL,
    content TEXT NOT NULL,
    published TIMESTAMP NOT NULL,
    CONSTRAINT pk_blog_post PRIMARY KEY (id),
    CONSTRAINT fk_blog_post_blog FOREIGN KEY (blog_id) REFERENCES blog.blog(id)
);

DO
$$
BEGIN
    IF NOT EXISTS (SELECT 1
                     FROM pg_catalog.pg_indexes
                    WHERE schemaname = 'blog'
                      AND tablename = 'post'
                      AND indexname = 'ix_blog_post_blog_id_published') THEN

        CREATE INDEX ix_blog_post_blog_id_published
            ON blog.post(blog_id, published);

    END IF;
END
$$;

GRANT SELECT, INSERT, UPDATE, DELETE
   ON blog.post
   TO website;
