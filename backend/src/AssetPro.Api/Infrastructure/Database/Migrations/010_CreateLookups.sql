CREATE TABLE ref.Lookups (
    Id              UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    TenantId        UNIQUEIDENTIFIER NULL REFERENCES dbo.Tenants(Id),
    Category        NVARCHAR(50)     NOT NULL,
    Code            NVARCHAR(50)     NOT NULL,
    DisplayName     NVARCHAR(200)    NOT NULL,
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

CREATE UNIQUE NONCLUSTERED INDEX UX_Lookups_TenantCategoryCode
    ON Lookups(TenantId, Category, Code) WHERE IsDeleted = 0;

CREATE NONCLUSTERED INDEX IX_Lookups_TenantCategory
    ON Lookups(TenantId, Category) WHERE IsDeleted = 0 AND IsActive = 1;
