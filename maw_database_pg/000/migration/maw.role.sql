select concat('insert into maw.role (id, name, description) values (', id, ', ''', LOWER(name), ''', ', COALESCE(CONCAT('''', description, ''''), 'NULL'), ');') from role;

select concat('alter sequence maw.role_id_seq restart with ', max(id) + 1, ';') from role;
