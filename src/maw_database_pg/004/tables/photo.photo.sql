ALTER TABLE photo.photo
    -- add size to existing images
    ADD COLUMN xs_size INT,
    ADD COLUMN sm_size INT,
    ADD COLUMN md_size INT,
    ADD COLUMN lg_size INT,
    ADD COLUMN prt_size INT,
    ADD COLUMN src_size INT,

    -- square thumbnails
    ADD COLUMN xs_sq_height SMALLINT,
    ADD COLUMN xs_sq_width SMALLINT,
    ADD COLUMN xs_sq_path VARCHAR(255),
    ADD COLUMN xs_sq_size INT;
