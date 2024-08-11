CREATE TABLE IF NOT EXISTS photo.gps_override (
    photo_id INTEGER NOT NULL,
    latitude REAL NOT NULL,
    longitude REAL NOT NULL,
    user_id SMALLINT NOT NULL,
    updated_time TIMESTAMP NOT NULL,
    has_been_reverse_geocoded BOOLEAN NOT NULL,

    CONSTRAINT pk_photo_gps_override PRIMARY KEY (photo_id),

    CONSTRAINT fk_gps_override_photo FOREIGN KEY (photo_id) REFERENCES photo.photo(id),
    CONSTRAINT fk_gps_override_photo_user FOREIGN KEY (user_id) REFERENCES maw.user(id)
);

GRANT SELECT, INSERT, UPDATE, DELETE
   ON photo.gps_override
   TO website;
