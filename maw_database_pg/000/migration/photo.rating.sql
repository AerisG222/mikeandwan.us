select concat('insert into photo.rating (photo_id, user_id, score) values (', 
    photo_id, ', ',
    user_id, ', ',
    rating,
    ');')
from photo_rating;
