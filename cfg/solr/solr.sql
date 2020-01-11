-- ALL CATEGORIES
SELECT 'photo' AS type,
       CONCAT('photo_', CAST(id AS TEXT)) AS solr_id,
       id,
       year,
       is_private,
       name,
       teaser_photo_width,
       teaser_photo_height,
       teaser_photo_path,
       teaser_photo_sq_height,
       teaser_photo_sq_width,
       teaser_photo_sq_path,
       gps_latitude,
       gps_longitude
  FROM photo.category
  
UNION

SELECT 'video' AS type,
       CONCAT('video_', CAST(id AS TEXT)) AS solr_id,
       id,
       year,
       is_private,
       name,
       teaser_image_width AS teaser_photo_width,
       teaser_image_height AS teaser_photo_height,
       teaser_image_path AS teaser_photo_path,
       teaser_image_sq_height AS teaser_photo_sq_height,
       teaser_image_sq_width AS teaser_photo_sq_width,
       teaser_image_sq_path AS teaser_photo_sq_path,
       gps_latitude,
       gps_longitude
  FROM video.category;


-- COMMENTS IN CATEGORY
SELECT 'photo' AS type,
       CONCAT('photo_', CAST(p.category_id AS TEXT)) AS solr_id,
       p.category_id,
       c.entry_date,
       c.message
  FROM photo.photo p
  LEFT OUTER JOIN photo.comment c ON p.id = c.photo_id
 WHERE c.message IS NOT NULL

UNION

SELECT 'video' AS type,
       CONCAT('video_', CAST(v.category_id AS TEXT)) AS solr_id,
       v.category_id,
       c.entry_date,
       c.message
  FROM video.video v
  LEFT OUTER JOIN video.comment c ON v.id = c.video_id
 WHERE c.message IS NOT NULL;


-- CAMERA MAKES AND MODELS
SELECT DISTINCT
       'photo' AS type,
       CONCAT('photo_', CAST(p.category_id AS TEXT)) AS solr_id,
       p.category_id,
       mk.name AS make,
       mdl.name AS model
  FROM photo.photo p
 INNER JOIN photo.category c ON p.category_id = c.id
  LEFT OUTER JOIN photo.make mk ON mk.id = p.make_id
  LEFT OUTER JOIN photo.model mdl ON mdl.id = p.model_id
 WHERE p.make_id IS NOT NULL
    OR p.model_id IS NOT NULL;


-- LOCATION DETAILS (should we break this up and pull distinct fields individually?)
SELECT DISTINCT
       'photo' AS type,
       CONCAT('photo_', CAST(p.category_id AS TEXT)) AS solr_id,
       p.category_id,
       rg.administrative_area_level_1,
       rg.administrative_area_level_2,
       rg.administrative_area_level_3,
       rg.country,
       rg.formatted_address,
       rg.locality,
       rg.neighborhood,
       rg.postal_code,
       rg.postal_code_suffix,
       rg.premise,
       rg.route,
       rg.street_number,
       rg.sub_locality_level_1,
       rg.sub_locality_level_2,
       rg.sub_premise
  FROM photo.photo p
 INNER JOIN photo.category c ON p.category_id = c.id
  LEFT OUTER JOIN photo.reverse_geocode rg ON rg.photo_id = p.id
 WHERE rg.photo_id IS NOT NULL

UNION

SELECT DISTINCT
       'video' AS type,
       CONCAT('video_', CAST(v.category_id AS TEXT)) AS solr_id,
       v.category_id,
       rg.administrative_area_level_1,
       rg.administrative_area_level_2,
       rg.administrative_area_level_3,
       rg.country,
       rg.formatted_address,
       rg.locality,
       rg.neighborhood,
       rg.postal_code,
       rg.postal_code_suffix,
       rg.premise,
       rg.route,
       rg.street_number,
       rg.sub_locality_level_1,
       rg.sub_locality_level_2,
       rg.sub_premise
  FROM video.video v
 INNER JOIN video.category c ON v.category_id = c.id
  LEFT OUTER JOIN video.reverse_geocode rg ON rg.video_id = v.id
 WHERE rg.video_id IS NOT NULL;
     

-- POINTS OF INTEREST
SELECT DISTINCT
       'photo' AS type,
       CONCAT('photo_', CAST(p.category_id AS TEXT)) AS solr_id,
       p.category_id,
       poi.poi_name,
       poi.poi_type
  FROM photo.photo p
 INNER JOIN photo.category c ON p.category_id = c.id
  LEFT OUTER JOIN photo.point_of_interest poi ON poi.photo_id = p.id
 WHERE poi.photo_id IS NOT NULL

UNION

SELECT DISTINCT
       'video' AS type,
       CONCAT('video_', CAST(v.category_id AS TEXT)) AS solr_id,
       v.category_id,
       poi.poi_name,
       poi.poi_type
  FROM video.video v
 INNER JOIN video.category c ON v.category_id = c.id
  LEFT OUTER JOIN video.point_of_interest poi ON poi.video_id = v.id
 WHERE poi.video_id IS NOT NULL;
