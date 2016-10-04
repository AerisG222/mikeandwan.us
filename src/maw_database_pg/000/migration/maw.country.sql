select concat('insert into maw.country (id, code, name) values (', id, ', ''', UPPER(code), ''', ''', name, ''');') from country;

select concat('alter sequence maw.country_id_seq restart with ', max(id) + 1, ';') from country;
