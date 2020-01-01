CREATE TABLE IF NOT EXISTS photo.point_of_interest (
    photo_id INTEGER NOT NULL,
    poi_type TEXT NOT NULL,
    poi_name TEXT NOT NULL,

    CONSTRAINT pk_photo_point_of_interest PRIMARY KEY (photo_id, poi_type),

    CONSTRAINT fk_point_of_interest_photo FOREIGN KEY (photo_id) REFERENCES photo.photo(id)
);

GRANT SELECT
   ON photo.point_of_interest
   TO website;
