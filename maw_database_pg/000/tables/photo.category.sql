CREATE TABLE IF NOT EXISTS photo.category (
    id SMALLSERIAL,
    year SMALLINT NOT NULL,
    is_private BOOLEAN NOT NULL,
    name VARCHAR(50) NOT NULL,
    teaser_photo_width SMALLINT,
    teaser_photo_height SMALLINT,
    teaser_photo_path VARCHAR(255),
    CONSTRAINT pk_photo_category PRIMARY KEY (id)
);

DO
$$
BEGIN
    IF NOT EXISTS (SELECT 1
                     FROM pg_catalog.pg_indexes
                    WHERE schemaname = 'photo'
                      AND tablename = 'category'
                      AND indexname = 'ix_photo_category_year_is_private') THEN

        CREATE INDEX ix_photo_category_year_is_private
            ON photo.category(year, is_private);
      
    END IF;
END
$$;

GRANT SELECT
   ON photo.category
   TO website;
