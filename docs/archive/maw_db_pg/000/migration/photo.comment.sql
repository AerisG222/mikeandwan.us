select concat('insert into photo.comment (photo_id, user_id, entry_date, message) values (', 
    photo_id, ', ',
    user_id, ', ',
    '''', entry_date, ''', ',
    '''', REPLACE(comment, '''', ''''''), '''', 
    ');')
from photo_comment;
