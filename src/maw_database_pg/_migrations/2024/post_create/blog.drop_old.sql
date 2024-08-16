DO
$$
DECLARE
    old_count INTEGER;
    new_count INTEGER;
BEGIN

    IF EXISTS (SELECT 1
                 FROM information_schema.columns
                WHERE table_schema = 'blog'
                  AND table_name = 'old_blog') THEN

        SELECT COUNT(1) INTO old_count FROM blog.old_blog;
        SELECT COUNT(1) INTO new_count FROM blog.blog;

        IF old_count = new_count THEN
            DROP TABLE blog.old_blog;
        END IF;

    END IF;


    IF EXISTS (SELECT 1
                 FROM information_schema.columns
                WHERE table_schema = 'blog'
                  AND table_name = 'old_post') THEN

        SELECT COUNT(1) INTO old_count FROM blog.old_post;
        SELECT COUNT(1) INTO new_count FROM blog.post;

        IF old_count = new_count THEN
            DROP TABLE blog.old_post;
        END IF;

    END IF;

END
$$;
