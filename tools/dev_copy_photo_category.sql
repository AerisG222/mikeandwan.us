insert into photo.category (year, name) values (2023, 'Test Category');

update photo.category
   set teaser_photo_width = c.teaser_photo_width,
        teaser_photo_height = c.teaser_photo_height,
        teaser_photo_path = c.teaser_photo_path,
        teaser_photo_sq_height = c.teaser_photo_sq_height,
        teaser_photo_sq_width = c.teaser_photo_sq_width,
        teaser_photo_sq_path = c.teaser_photo_sq_path
   from (select * from photo.category) as c
   where photo.category.id = 1698 and c.id = 1645;

insert into photo.photo (category_id, xs_height, xs_width, xs_path, sm_height, sm_width, sm_path, md_height, md_width, md_path, lg_height, lg_width, lg_path, prt_height, prt_width, prt_path, src_height, src_width, src_path)
       select currval('photo.category_id_seq'), xs_height, xs_width, xs_path, sm_height, sm_width, sm_path, md_height, md_width, md_path, lg_height, lg_width, lg_path, prt_height, prt_width, prt_path, src_height, src_width, src_path
       from photo.photo
       where category_id = 1645;

insert into photo.category_role (category_id, role_id) values (currval('photo.category_id_seq'), 1);
insert into photo.category_role (category_id, role_id) values (currval('photo.category_id_seq'), 2);
