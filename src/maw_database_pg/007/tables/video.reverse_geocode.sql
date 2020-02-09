CREATE TABLE IF NOT EXISTS video.reverse_geocode (
    video_id SMALLINT NOT NULL,
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

    CONSTRAINT pk_video_reverse_geocode PRIMARY KEY (video_id),

    CONSTRAINT fk_reverse_geocode_video FOREIGN KEY (video_id) REFERENCES video.video(id)
);

GRANT SELECT, INSERT
   ON video.reverse_geocode
   TO website;
