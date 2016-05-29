CREATE TABLE IF NOT EXISTS blog.post (
    id SMALLSERIAL,
    blog_id SMALLINT NOT NULL,
    title VARCHAR(100) NOT NULL,
    description TEXT NOT NULL,
    publish_date TIMESTAMP NOT NULL,
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
                      AND indexname = 'ix_blog_post_blog_id_publish_date') THEN

        CREATE INDEX ix_blog_post_blog_id_publish_date
            ON blog.post(blog_id, publish_date);
      
    END IF;
END
$$;

GRANT SELECT, INSERT, UPDATE, DELETE 
   ON blog.post
   TO website;

GRANT USAGE 
   ON SEQUENCE blog.post_id_seq 
   TO website;
   