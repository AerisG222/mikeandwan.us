CREATE TABLE IF NOT EXISTS video.video (
    id SMALLSERIAL,
    category_id SMALLINT NOT NULL,
    is_private BOOLEAN NOT NULL,
    duration SMALLINT NOT NULL,
    thumb_height SMALLINT NOT NULL,
    thumb_width SMALLINT NOT NULL,
    thumb_path VARCHAR(255) NOT NULL,
    scaled_height SMALLINT NOT NULL,
    scaled_width SMALLINT NOT NULL,
    scaled_path VARCHAR(255) NOT NULL,
    full_height SMALLINT NOT NULL,
    full_width SMALLINT NOT NULL,
    full_path VARCHAR(255) NOT NULL,
    raw_path VARCHAR(255) NOT NULL,
    CONSTRAINT pk_video_video PRIMARY KEY (id),
    CONSTRAINT fk_video_category FOREIGN KEY (category_id) REFERENCES video.category(id)
);

DO
$$
BEGIN
    IF NOT EXISTS (SELECT 1
                     FROM pg_catalog.pg_indexes
                    WHERE schemaname = 'video'
                      AND tablename = 'video'
                      AND indexname = 'ix_video_video_category_id_is_private') THEN

        CREATE INDEX ix_video_video_category_id_is_private
            ON video.video(category_id, is_private);
      
    END IF;
END
$$;

GRANT SELECT
   ON video.video
   TO website;
