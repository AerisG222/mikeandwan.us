ALTER TABLE maw.user
        ADD COLUMN enable_github_auth BOOLEAN NOT NULL DEFAULT FALSE,
        ADD COLUMN enable_google_auth BOOLEAN NOT NULL DEFAULT FALSE,
        ADD COLUMN enable_microsoft_auth BOOLEAN NOT NULL DEFAULT FALSE,
        ADD COLUMN enable_twitter_auth BOOLEAN NOT NULL DEFAULT FALSE;
