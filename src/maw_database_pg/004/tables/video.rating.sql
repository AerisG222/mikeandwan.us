CREATE TABLE IF NOT EXISTS video.rating (
    video_id INTEGER NOT NULL,
    user_id SMALLINT NOT NULL,
    score SMALLINT NOT NULL,
    CONSTRAINT pk_video_rating PRIMARY KEY (video_id, user_id),
    CONSTRAINT fk_video_rating_video FOREIGN KEY (video_id) REFERENCES video.video(id),
    CONSTRAINT fk_video_rating_user FOREIGN KEY (user_id) REFERENCES maw.user(id)
);

DO
$$
BEGIN
    IF NOT EXISTS (SELECT 1
                     FROM pg_catalog.pg_indexes
                    WHERE schemaname = 'video'
                      AND tablename = 'rating'
                      AND indexname = 'ix_video_rating_video_id') THEN

        CREATE INDEX ix_video_rating_video_id
            ON video.rating(video_id);

    END IF;
END
$$;

DO
$$
BEGIN
    IF NOT EXISTS (SELECT 1
                     FROM pg_catalog.pg_indexes
                    WHERE schemaname = 'video'
                      AND tablename = 'rating'
                      AND indexname = 'ix_video_rating_user_id') THEN

        CREATE INDEX ix_video_rating_user_id
            ON video.rating(user_id);

    END IF;
END
$$;

GRANT SELECT, INSERT, UPDATE, DELETE
   ON video.rating
   TO website;
