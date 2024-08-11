DO
$$
BEGIN

    IF NOT EXISTS (SELECT 1 FROM photo.category_role) THEN

        INSERT INTO photo.category_role
        SELECT pc.id AS category_id,
            r.id AS role_id
        FROM photo.category pc
        CROSS JOIN maw.role r
        WHERE r.name = 'admin'
            OR (r.name = 'friend' and pc.is_private = FALSE);

    END IF;

END
$$