CREATE TABLE IF NOT EXISTS photo.category_role (
    category_id SMALLINT NOT NULL,
    role_id SMALLINT NOT NULL,

    CONSTRAINT pk_photo_category_role PRIMARY KEY (category_id, role_id),

    CONSTRAINT fk_photo_category_role_category FOREIGN KEY (category_id) REFERENCES photo.category(id),
    CONSTRAINT fk_photo_category_role_role FOREIGN KEY (role_id) REFERENCES maw.role(id)
);

GRANT SELECT, INSERT, DELETE
   ON photo.category_role
   TO website;
