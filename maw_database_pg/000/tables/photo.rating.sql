CREATE TABLE IF NOT EXISTS photo.rating (
    photo_id INTEGER NOT NULL,
    user_id SMALLINT NOT NULL,
    score SMALLINT NOT NULL,
    CONSTRAINT pk_photo_rating PRIMARY KEY (photo_id, user_id),
    CONSTRAINT fk_photo_rating_photo FOREIGN KEY (photo_id) REFERENCES photo.photo(id),
    CONSTRAINT fk_photo_rating_user FOREIGN KEY (user_id) REFERENCES maw.user(id)
);

DO
$$
BEGIN
    IF NOT EXISTS (SELECT 1
                     FROM pg_catalog.pg_indexes
                    WHERE schemaname = 'photo'
                      AND tablename = 'rating'
                      AND indexname = 'ix_photo_rating_photo_id') THEN

        CREATE INDEX ix_photo_rating_photo_id
            ON photo.rating(photo_id);
      
    END IF;
END
$$;

DO
$$
BEGIN
    IF NOT EXISTS (SELECT 1
                     FROM pg_catalog.pg_indexes
                    WHERE schemaname = 'photo'
                      AND tablename = 'rating'
                      AND indexname = 'ix_photo_rating_user_id') THEN

        CREATE INDEX ix_photo_rating_user_id
            ON photo.rating(user_id);
      
    END IF;
END
$$;

GRANT SELECT, INSERT, UPDATE, DELETE
   ON photo.rating
   TO website;
