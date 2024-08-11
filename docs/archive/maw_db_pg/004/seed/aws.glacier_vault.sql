DO
$$
BEGIN

    IF NOT EXISTS (SELECT 1 FROM aws.glacier_vault WHERE vault_name = 'videos') THEN

        INSERT INTO aws.glacier_vault (region, vault_name) VALUES ('us-east-1', 'videos');

    END IF;

END
$$
