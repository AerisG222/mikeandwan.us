CREATE TABLE IF NOT EXISTS photo.reverse_geocode (
    photo_id INTEGER NOT NULL,
    formatted_address TEXT,
    administrative_area_level_1 TEXT,
    administrative_area_level_2 TEXT,
    administrative_area_level_3 TEXT,
    country TEXT,
    locality TEXT,
    neighborhood TEXT,
    sub_locality_level_1 TEXT,
    sub_locality_level_2 TEXT,
    postal_code TEXT,
    postal_code_suffix TEXT,
    premise TEXT,
    route TEXT,
    street_number TEXT,
    sub_premise TEXT,

    CONSTRAINT pk_photo_reverse_geocode PRIMARY KEY (photo_id),

    CONSTRAINT fk_reverse_geocode_photo FOREIGN KEY (photo_id) REFERENCES photo.photo(id)
);

GRANT SELECT, INSERT
   ON photo.reverse_geocode
   TO website;
