select concat('insert into blog.post (id, blog_id, title, description, publish_date) values (', id, ', ', blog_id, ', ''', title, ''', ''', REPLACE(description, '''', ''''''), ''', ''', publish_date, ''');') from blog_post;

select concat('alter sequence blog.post_id_seq restart with ', max(id) + 1, ';') from blog_post;
