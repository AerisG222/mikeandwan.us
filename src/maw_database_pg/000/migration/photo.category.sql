select concat('insert into photo.category (id, year, is_private, name, teaser_photo_width, teaser_photo_height, teaser_photo_path) values (', 
    id, ', ',
    year, ', ',
    'CAST(', is_private, ' AS BOOLEAN), ',
    '''', REPLACE(name, '''', ''''''), ''', ',
    teaser_photo_width, ', ',
    teaser_photo_height, ', ',
    '''', REPLACE(teaser_photo_path, '/thumbnails/', '/xs/'), '''',
    ');')
from photo_category;

select concat('alter sequence photo.category_id_seq restart with ', max(id) + 1, ';') from photo_category;
