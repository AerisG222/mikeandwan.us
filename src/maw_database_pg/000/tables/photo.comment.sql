CREATE TABLE IF NOT EXISTS photo.comment (
    id SERIAL,
    photo_id INTEGER NOT NULL,
    user_id SMALLINT NOT NULL,
    entry_date TIMESTAMP NOT NULL,
    message TEXT NOT NULL,
    CONSTRAINT pk_photo_comment PRIMARY KEY (id),
    CONSTRAINT fk_photo_comment_photo FOREIGN KEY (photo_id) REFERENCES photo.photo(id),
    CONSTRAINT fk_photo_comment_user FOREIGN KEY (user_id) REFERENCES maw.user(id)
);

DO
$$
BEGIN
    IF NOT EXISTS (SELECT 1
                     FROM pg_catalog.pg_indexes
                    WHERE schemaname = 'photo'
                      AND tablename = 'comment'
                      AND indexname = 'ix_photo_comment_photo_id') THEN

        CREATE INDEX ix_photo_comment_photo_id
            ON photo.comment(photo_id);
      
    END IF;
END
$$;

DO
$$
BEGIN
    IF NOT EXISTS (SELECT 1
                     FROM pg_catalog.pg_indexes
                    WHERE schemaname = 'photo'
                      AND tablename = 'comment'
                      AND indexname = 'ix_photo_comment_user_id') THEN

        CREATE INDEX ix_photo_comment_user_id
            ON photo.comment(user_id);
      
    END IF;
END
$$;

GRANT SELECT, INSERT
   ON photo.comment
   TO website;

GRANT USAGE 
   ON SEQUENCE photo.comment_id_seq 
   TO website;
