-- create B-tree index
CREATE INDEX IF NOT EXISTS idx_users_firstname_lastname ON Users (FirstName, LastName);
CREATE INDEX IF NOT EXISTS idx_users_lastname_firstname ON Users (LastName, FirstName);

-- delete B-tree index
DROP INDEX IF EXISTS idx_users_firstname_lastname;
DROP INDEX IF EXISTS idx_users_lastname_firstname;

-- create GIN index
CREATE EXTENSION IF NOT EXISTS pg_trgm;
CREATE INDEX IF NOT EXISTS idx_users_lastname_firstname_trgm ON Users USING GIN (FirstName gin_trgm_ops, LastName gin_trgm_ops);
CREATE INDEX IF NOT EXISTS idx_users_firstname_lastname_trgm ON Users USING GIN (LastName gin_trgm_ops, FirstName gin_trgm_ops);
CREATE INDEX IF NOT EXISTS idx_users_firstname_trgm ON Users USING GIN (FirstName gin_trgm_ops);
CREATE INDEX IF NOT EXISTS idx_users_lastname_trgm ON Users USING GIN (LastName gin_trgm_ops);

-- delete GIN index
DROP INDEX IF EXISTS idx_users_lastname_firstname_trgm;
DROP INDEX IF EXISTS idx_users_firstname_lastname_trgm;
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
DROP EXTENSION IF EXISTS pg_trgm;

-- update index statistic 