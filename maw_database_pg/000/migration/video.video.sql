select concat('insert into video.video (
    id,
    category_id,
    is_private,
    duration,
    thumb_height,
    thumb_width,
    thumb_path,
    scaled_height,
    scaled_width,
    scaled_path,
    full_height,
    full_width,
    full_path,
    raw_path
)
values (',
    id, ', ',
    video_category_id, ', ',
    'CAST(', is_private, ' AS BOOLEAN), ',
    duration, ', ',
    thumb_height, ', ',
    thumb_width, ', ',
    '''', thumb_path, ''', ',
    scaled_height, ', ',
    scaled_width, ', ',
    '''', scaled_path, ''', ',
    full_height, ', ',
    full_width, ', ',
    '''', full_path, ''', ',
    '''', raw_path, ''''
');') from video;

select concat('alter sequence video.video_id_seq restart with ', max(id) + 1, ';') from video;
