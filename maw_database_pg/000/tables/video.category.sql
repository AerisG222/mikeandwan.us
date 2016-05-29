CREATE TABLE IF NOT EXISTS video.category (
    id SMALLSERIAL,
    year SMALLINT NOT NULL,
    is_private BOOLEAN NOT NULL,
    name VARCHAR(50) NOT NULL,
    teaser_image_path VARCHAR(255),
    teaser_image_width SMALLINT,
    teaser_image_height SMALLINT,
    CONSTRAINT pk_video_category PRIMARY KEY (id)
);

DO
$$
BEGIN
    IF NOT EXISTS (SELECT 1
                     FROM pg_catalog.pg_indexes
                    WHERE schemaname = 'video'
                      AND tablename = 'category'
                      AND indexname = 'ix_video_category_year_is_private') THEN

        CREATE INDEX ix_video_category_year_is_private
            ON video.category(year, is_private);
      
    END IF;
END
$$;

GRANT SELECT
   ON video.category
   TO website;
