select concat('insert into blog.blog (id, title, copyright, description) values (', id, ', ''', title, ''', ''', copyright, ''', ''', description, ''');') from blog;

select concat('alter sequence blog.blog_id_seq restart with ', max(id) + 1, ';') from blog;
