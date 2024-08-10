CREATE TABLE IF NOT EXISTS media.media (
    id UUID,
    category_id UUID NOT NULL,
    created TIMESTAMP NOT NULL,
    type_id SMALLINT NOT NULL,

    -- qqvg (160x120)
    qqvg_height SMALLINT NOT NULL,
    qqvg_width SMALLINT NOT NULL,
    qqvg_path TEXT NOT NULL,
    qqvg_bytes SMALLINT NOT NULL,

    -- qvg (320x240)
    qvg_height SMALLINT,
    qvg_width SMALLINT,
    qvg_path TEXT,
    qvg_bytes SMALLINT,

    -- nhd (640x360)
    qvg_height SMALLINT,
    qvg_width SMALLINT,
    qvg_path TEXT,
    qvg_bytes SMALLINT,

    -- hd (1280x720)
    hd_height SMALLINT,
    hd_width SMALLINT,
    hd_path TEXT,
    hd_bytes SMALLINT,

    -- full_hd (1920x1080)
    full_hd_height SMALLINT,
    full_hd_width SMALLINT,
    full_hd_path TEXT,
    full_hd_bytes SMALLINT,

    -- qhd (2560x1440)
    qhd_height SMALLINT,
    qhd_width SMALLINT,
    qhd_path TEXT,
    qhd_bytes SMALLINT,

    -- 4k (3840x2160)
    4k_height SMALLINT,
    4k_width SMALLINT,
    4k_path TEXT,
    4k_bytes SMALLINT,

    -- 5k (5120x2880)
    5k_height SMALLINT,
    5k_width SMALLINT,
    5k_path TEXT,
    5k_bytes SMALLINT,

    -- 8k (5120x2880)
    8k_height SMALLINT,
    8k_width SMALLINT,
    8k_path TEXT,
    8k_bytes SMALLINT,

    -- src
    src_height SMALLINT NOT NULL,
    src_width SMALLINT NOT NULL,
    src_path TEXT NOT NULL,
    src_bytes SMALLINT NOT NULL,

    -- override gps values - source in exif
    gps_altitude REAL,
    gps_latitude REAL,
    gps_longitude REAL,

    -- metadata
    exif jsonb,

    CONSTRAINT pk_media_media PRIMARY KEY (id),

    CONSTRAINT fk_media_media_category FOREIGN KEY (category_id) REFERENCES media.category(id),
    CONSTRAINT fk_media_media_type FOREIGN KEY (type_id) REFERENCES media.type(id)
);

DO
$$
BEGIN
    IF NOT EXISTS (SELECT 1
                     FROM pg_catalog.pg_indexes
                    WHERE schemaname = 'media'
                      AND tablename = 'media'
                      AND indexname = 'ix_media_media_category_id_created') THEN

        CREATE INDEX ix_media_media_category_id_created
            ON media.media(category_id, created);

    END IF;
END
$$;

GRANT SELECT
   ON photo.photo
   TO website;
