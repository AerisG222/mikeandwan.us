select concat('insert into maw.user_role (user_id, role_id) values (', user_id, ', ', role_id, ');') from user_role;