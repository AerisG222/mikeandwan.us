CREATE TABLE IF NOT EXISTS aws.glacier_vault (
    id SMALLSERIAL,
    region VARCHAR(20) NOT NULL,
    vault_name VARCHAR(50) NOT NULL,
    CONSTRAINT pk_aws_glacier_vault PRIMARY KEY (id),
    CONSTRAINT uq_aws_glacier_vault$region$vault_name UNIQUE (region, vault_name)
);

GRANT SELECT
   ON aws.glacier_vault
   TO website;
