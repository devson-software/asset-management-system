CREATE TABLE app.Projects (
    Id                  UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    TenantId            UNIQUEIDENTIFIER NOT NULL REFERENCES dbo.Tenants(Id),
    CustomerId          UNIQUEIDENTIFIER NOT NULL REFERENCES app.Customers(Id),
    Name                NVARCHAR(300)    NOT NULL,
    SiteAddress         NVARCHAR(500)    NULL,
    VendorLocation      NVARCHAR(200)    NULL,
    TimeAllocation      NVARCHAR(100)    NULL,
    PictureUrl          NVARCHAR(500)    NULL,
    CreatedAt           DATETIME2(7)     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy           UNIQUEIDENTIFIER NOT NULL,
    UpdatedAt           DATETIME2(7)     NULL,
    UpdatedBy           UNIQUEIDENTIFIER NULL,
    IsDeleted           BIT              NOT NULL DEFAULT 0,
    DeletedAt           DATETIME2(7)     NULL,
    DeletedBy           UNIQUEIDENTIFIER NULL
);

CREATE NONCLUSTERED INDEX IX_Projects_TenantCustomer
    ON app.Projects(TenantId, CustomerId) WHERE IsDeleted = 0;
