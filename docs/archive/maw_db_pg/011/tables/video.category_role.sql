CREATE TABLE IF NOT EXISTS video.category_role (
    category_id SMALLINT NOT NULL,
    role_id SMALLINT NOT NULL,

    CONSTRAINT pk_video_category_role PRIMARY KEY (category_id, role_id),

    CONSTRAINT fk_video_category_role_category FOREIGN KEY (category_id) REFERENCES video.category(id),
    CONSTRAINT fk_video_category_role_role FOREIGN KEY (role_id) REFERENCES maw.role(id)
);

GRANT SELECT, INSERT, DELETE
   ON video.category_role
   TO website;
