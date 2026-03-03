-- Create database schemas
-- This must run before any table creation scripts

-- System schema (already exists by default, but ensuring it's explicit)
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'dbo')
BEGIN
    EXEC('CREATE SCHEMA dbo');
END

-- Application schema - Core business entities
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'app')
BEGIN
    EXEC('CREATE SCHEMA app');
END

-- Operations schema - Maintenance and service operations
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'ops')
BEGIN
    EXEC('CREATE SCHEMA ops');
END

-- Reference schema - Lookup data and reference tables
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'ref')
BEGIN
    EXEC('CREATE SCHEMA ref');
END

PRINT 'Database schemas created successfully';
