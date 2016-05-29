select concat('insert into maw.login_history (
    id,
    user_id,
    username,
    attempt_time,
    login_activity_type_id,
    login_area_id
)
values (',
    id, ', ', 
    COALESCE(user_id, 'NULL'), ', ', 
    COALESCE(CONCAT('''', REPLACE(LOWER(username), '''', ''''''), ''''), 'NULL'), ', ', 
    '''', attempt_time, ''', ', 
    login_activity_type_id, ', ', 
    login_area_id,
');') from login_history;

select concat('alter sequence maw.login_history_id_seq restart with ', max(id) + 1, ';') from login_history;
