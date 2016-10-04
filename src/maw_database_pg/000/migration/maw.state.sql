select concat('insert into maw.state (id, code, name) values (', id, ', ''', UPPER(code), ''', ''', name, ''');') from state;

select concat('alter sequence maw.state_id_seq restart with ', max(id) + 1, ';') from state;
