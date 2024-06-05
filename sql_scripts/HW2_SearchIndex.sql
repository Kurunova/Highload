-- create B-tree index
CREATE INDEX IF NOT EXISTS idx_users_firstname ON Users (FirstName);
CREATE INDEX IF NOT EXISTS idx_users_lastname ON Users (LastName);

-- delete B-tree index
DROP INDEX IF EXISTS idx_users_firstname;
DROP INDEX IF EXISTS idx_users_lastname;

-- create GIN index
CREATE EXTENSION IF NOT EXISTS pg_trgm;
CREATE INDEX IF NOT EXISTS idx_users_firstname_trgm ON Users USING GIN (FirstName gin_trgm_ops);
CREATE INDEX IF NOT EXISTS idx_users_lastname_trgm ON Users USING GIN (LastName gin_trgm_ops);

-- delete GIN index
DROP INDEX IF EXISTS idx_users_firstname_trgm;
DROP INDEX IF EXISTS idx_users_lastname_trgm;
DROP EXTENSION IF EXISTS pg_trgm;

-- update index statistic 


-- create GiST index
CREATE EXTENSION IF NOT EXISTS pg_trgm;
CREATE INDEX IF NOT EXISTS idx_users_firstname_gist ON Users USING GiST (FirstName gist_trgm_ops);
CREATE INDEX IF NOT EXISTS idx_users_lastname_gist ON Users USING GiST (LastName gist_trgm_ops);

-- delete GiST index
DROP INDEX IF EXISTS idx_users_firstname_gist;
DROP INDEX IF EXISTS idx_users_lastname_gist;
IF NOT EXISTS (SELECT 1 FROM pg_indexes WHERE indexdef LIKE '%gist_trgm_ops%') THEN
	DROP EXTENSION IF EXISTS pg_trgm;
END IF;

-- update index statistic 