SELECT CONCAT('insert into maw.user (
    id,
    username,
    salt,
    hashed_password,
    security_stamp,
    password_last_set_on,
    first_name,
    last_name,
    email,
    website,
    date_of_birth,
    company_name,
    position,
    work_email,
    address_1,
    address_2,
    city,
    state_id,
    postal_code,
    country_id,
    home_phone,
    mobile_phone,
    work_phone
)
values ( ',
    id, ', ',
    '''', LOWER(username), '''', ', ',
    COALESCE(CONCAT('''', salt, ''''), 'NULL'), ', ',
    COALESCE(CONCAT('''', hashed_password, ''''), 'NULL'), ', ',
    COALESCE(CONCAT('''', security_stamp, ''''), 'NULL'), ', ',
    COALESCE(CONCAT('''', password_last_set_on, ''''), 'NULL'), ', ',
    COALESCE(CONCAT('''', first_name, ''''), 'NULL'), ', ',
    COALESCE(CONCAT('''', last_name, ''''), 'NULL'), ', ',
    COALESCE(CONCAT('''', LOWER(email), ''''), 'NULL'), ', ',
    COALESCE(CONCAT('''', website, ''''), 'NULL'), ', ',
    COALESCE(CONCAT('''', date_of_birth, ''''), 'NULL'), ', ',
    COALESCE(CONCAT('''', company_name, ''''), 'NULL'), ', ',
    COALESCE(CONCAT('''', position, ''''), 'NULL'), ', ',
    COALESCE(CONCAT('''', LOWER(work_email), ''''), 'NULL'), ', ',
    COALESCE(CONCAT('''', address_1, ''''), 'NULL'), ', ',
    COALESCE(CONCAT('''', address_2, ''''), 'NULL'), ', ',
    COALESCE(CONCAT('''', city, ''''), 'NULL'), ', ',
    COALESCE(state_id, 'NULL'), ', ',
    COALESCE(CONCAT('''', postal_code, ''''), 'NULL'), ', ',
    COALESCE(country_id, 'NULL'), ', ',
    COALESCE(CONCAT('''', home_phone, ''''), 'NULL'), ', ',
    COALESCE(CONCAT('''', mobile_phone, ''''), 'NULL'), ', ',
    COALESCE(CONCAT('''', work_phone, ''''), 'NULL'),
');')
FROM user;

select concat('alter sequence maw.user_id_seq restart with ', max(id) + 1, ';') from user;
