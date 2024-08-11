CREATE TABLE IF NOT EXISTS media.category_role (
    category_id UUID NOT NULL,
    role_id SMALLINT NOT NULL,

    CONSTRAINT pk_media_category_role PRIMARY KEY (category_id, role_id),

    CONSTRAINT fk_media_category_role_category FOREIGN KEY (category_id) REFERENCES media.category(id),
    CONSTRAINT fk_media_category_role_role FOREIGN KEY (role_id) REFERENCES maw.role(id)
);

GRANT SELECT, INSERT, DELETE
   ON media.category_role
   TO website;
