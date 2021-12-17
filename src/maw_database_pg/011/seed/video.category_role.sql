DO
$$
BEGIN

    IF NOT EXISTS (SELECT 1 FROM video.category_role) THEN

        INSERT INTO video.category_role
        SELECT vc.id AS category_id,
            r.id AS role_id
        FROM video.category vc
        CROSS JOIN maw.role r
        WHERE r.name = 'admin'
            OR (r.name = 'friend' and vc.is_private = FALSE);

    END IF;

END
$$
