CREATE TABLE IF NOT EXISTS video.comment (
    id SERIAL,
    video_id INTEGER NOT NULL,
    user_id SMALLINT NOT NULL,
    entry_date TIMESTAMP NOT NULL,
    message TEXT NOT NULL,
    CONSTRAINT pk_video_comment PRIMARY KEY (id),
    CONSTRAINT fk_video_comment_video FOREIGN KEY (video_id) REFERENCES video.video(id),
    CONSTRAINT fk_video_comment_user FOREIGN KEY (user_id) REFERENCES maw.user(id)
);

DO
$$
BEGIN
    IF NOT EXISTS (SELECT 1
                     FROM pg_catalog.pg_indexes
                    WHERE schemaname = 'video'
                      AND tablename = 'comment'
                      AND indexname = 'ix_video_comment_video_id') THEN

        CREATE INDEX ix_video_comment_video_id
            ON video.comment(video_id);

    END IF;
END
$$;

DO
$$
BEGIN
    IF NOT EXISTS (SELECT 1
                     FROM pg_catalog.pg_indexes
                    WHERE schemaname = 'video'
                      AND tablename = 'comment'
                      AND indexname = 'ix_video_comment_user_id') THEN

        CREATE INDEX ix_video_comment_user_id
            ON video.comment(user_id);

    END IF;
END
$$;

GRANT SELECT, INSERT
   ON video.comment
   TO website;

GRANT USAGE
   ON SEQUENCE video.comment_id_seq
   TO website;
