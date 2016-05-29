CREATE TABLE IF NOT EXISTS photo.white_balance (
    id SMALLSERIAL,
    name VARCHAR(50) NOT NULL,
    CONSTRAINT pk_photo_white_balance PRIMARY KEY (id),
    CONSTRAINT uq_photo_white_balance_name UNIQUE (name)
);

GRANT SELECT
   ON photo.white_balance
   TO website;
