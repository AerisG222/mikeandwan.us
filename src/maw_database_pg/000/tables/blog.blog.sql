CREATE TABLE IF NOT EXISTS blog.blog (
    id SMALLSERIAL,
    title VARCHAR(50) NOT NULL,
    copyright VARCHAR(50) NOT NULL,
    description VARCHAR(250) NOT NULL,
    CONSTRAINT pk_blog_blog PRIMARY KEY (id),
    CONSTRAINT uq_blog_blog$title UNIQUE (title)
);

GRANT SELECT
   ON blog.blog
   TO website;
