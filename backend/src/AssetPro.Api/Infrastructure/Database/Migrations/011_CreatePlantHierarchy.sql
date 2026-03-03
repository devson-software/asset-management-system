-- Drop the old PlantHierarchy if it was created by a previous migration
IF OBJECT_ID('dbo.PlantHierarchy', 'U') IS NOT NULL
    DROP TABLE dbo.PlantHierarchy;

CREATE TABLE PlantHierarchy (
    Id              UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    TenantId        UNIQUEIDENTIFIER NULL REFERENCES Tenants(Id),
    ParentId        UNIQUEIDENTIFIER NULL REFERENCES PlantHierarchy(Id),
    Name            NVARCHAR(200)    NOT NULL,
    Level           INT              NOT NULL DEFAULT 0,
    SortOrder       INT              NOT NULL DEFAULT 0,
    IsActive        BIT              NOT NULL DEFAULT 1,
    CreatedAt       DATETIME2(7)     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy       UNIQUEIDENTIFIER NOT NULL,
    UpdatedAt       DATETIME2(7)     NULL,
    UpdatedBy       UNIQUEIDENTIFIER NULL,
    IsDeleted       BIT              NOT NULL DEFAULT 0,
    DeletedAt       DATETIME2(7)     NULL,
    DeletedBy       UNIQUEIDENTIFIER NULL
);

CREATE NONCLUSTERED INDEX IX_PlantHierarchy_TenantParent
    ON PlantHierarchy(TenantId, ParentId) WHERE IsDeleted = 0 AND IsActive = 1;

CREATE NONCLUSTERED INDEX IX_PlantHierarchy_TenantLevel
    ON PlantHierarchy(TenantId, Level) WHERE IsDeleted = 0 AND IsActive = 1;
