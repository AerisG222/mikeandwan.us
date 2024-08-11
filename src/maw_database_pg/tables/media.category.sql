CREATE TABLE IF NOT EXISTS media.category (
    id UUID,
    published TIMESTAMP NOT NULL,
    name TEXT NOT NULL,

    -- qqvg (160x120)
    teaser_qqvg_height SMALLINT NOT NULL,
    teaser_qqvg_width SMALLINT NOT NULL,
    teaser_qqvg_path TEXT NOT NULL,
    teaser_qqvg_bytes SMALLINT NOT NULL,

    -- qvg (320x240)
    teaser_qvg_height SMALLINT NOT NULL,
    teaser_qvg_width SMALLINT NOT NULL,
    teaser_qvg_path TEXT NOT NULL,
    teaser_qvg_bytes SMALLINT NOT NULL,

    -- add video / mjpeg teasers?
    -- add tags?

    -- legacy fields to support migrations / updates - to be removed in future
    legacy_id SMALLINT NOT NULL,
    legacy_type CHAR(1) NOT NULL,  -- p = photo, v = video

    CONSTRAINT pk_media_category PRIMARY KEY (id)
);

DO
$$
BEGIN
    IF NOT EXISTS (SELECT 1
                     FROM pg_catalog.pg_indexes
                    WHERE schemaname = 'media'
                      AND tablename = 'category'
                      AND indexname = 'ix_media_category_published') THEN

        CREATE INDEX ix_media_category_published
            ON media.category(published);

    END IF;
END
$$;

GRANT SELECT
   ON media.category
   TO website;
