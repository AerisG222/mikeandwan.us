CREATE TABLE IF NOT EXISTS blog.blog (
    id UUID NOT NULL,
    title TEXT NOT NULL,
    copyright TEXT NOT NULL,
    description TEXT NOT NULL,

    CONSTRAINT pk_blog_blog PRIMARY KEY (id),
    CONSTRAINT uq_blog_blog$title UNIQUE (title)
);

GRANT SELECT
   ON blog.blog
   TO website;
