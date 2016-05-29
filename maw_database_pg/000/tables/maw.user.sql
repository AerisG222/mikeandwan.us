CREATE TABLE IF NOT EXISTS maw.user (
    id SMALLSERIAL,
    username VARCHAR(30) NOT NULL,
    salt VARCHAR(50),
    hashed_password VARCHAR(2000),
    security_stamp VARCHAR(2000),
    password_last_set_on TIMESTAMP NOT NULL,
    first_name VARCHAR(30),
    last_name VARCHAR(30),
    email VARCHAR(255),
    website VARCHAR(255),
    date_of_birth DATE,
    company_name VARCHAR(50),
    position VARCHAR(50),
    work_email VARCHAR(255),
    address_1 VARCHAR(100),
    address_2 VARCHAR(100),
    city VARCHAR(100),
    state_id SMALLINT,
    postal_code VARCHAR(20),
    country_id SMALLINT,
    home_phone VARCHAR(50),
    mobile_phone VARCHAR(50),
    work_phone VARCHAR(50),
    CONSTRAINT pk_maw_user PRIMARY KEY (id),
    CONSTRAINT uq_maw_user_username UNIQUE (username),
    CONSTRAINT uq_maw_user_email UNIQUE (email),
    CONSTRAINT fk_maw_user_country FOREIGN KEY (country_id) REFERENCES maw.country(id),
    CONSTRAINT fk_maw_user_state FOREIGN KEY (state_id) REFERENCES maw.state(id)
);

GRANT SELECT, INSERT, UPDATE, DELETE
   ON maw.user
   TO website;

GRANT USAGE 
   ON SEQUENCE maw.user_id_seq 
   TO website;
