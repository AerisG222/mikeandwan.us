DO
$$
BEGIN

    IF NOT EXISTS (SELECT 1 FROM aws.glacier_vault) THEN

        INSERT INTO aws.glacier_vault (region, vault_name) VALUES ('us-east-1', 'photos');

    END IF;

END
$$
