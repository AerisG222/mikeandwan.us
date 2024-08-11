-- todo: remove
ALTER TABLE media.media DROP CONSTRAINT fk_media_media_category;
ALTER TABLE media.media DROP CONSTRAINT fk_media_media_type;

ALTER TABLE media.category_role DROP CONSTRAINT fk_media_category_role_category;

DROP TABLE media.media;
DROP TABLE media.category;
