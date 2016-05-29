select concat('insert into video.category (id, year, is_private, name, teaser_image_width, teaser_image_height, teaser_image_path) values (', 
    id, ', ',
    year, ', ',
    'CAST(', is_private, ' AS BOOLEAN), ',
    '''', REPLACE(name, '''', ''''''), ''', ',
    teaser_image_width, ', ',
    teaser_image_height, ', ',
    '''', teaser_image_path, '''', 
    ');')
from video_category;

select concat('alter sequence video.category_id_seq restart with ', max(id) + 1, ';') from video_category;
