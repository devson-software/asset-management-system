CREATE TABLE ref.UnitLibrary (
    Id                  UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    TenantId            UNIQUEIDENTIFIER NULL REFERENCES dbo.Tenants(Id),
    Manufacturer        NVARCHAR(200)    NOT NULL,
    UnitTypeId          UNIQUEIDENTIFIER NULL,
    IndoorModel         NVARCHAR(200)    NOT NULL,
    OutdoorModel        NVARCHAR(200)    NULL,
    RefrigerantType     NVARCHAR(50)     NOT NULL,
    RefrigerantCharge   DECIMAL(8,2)     NULL,
    ServiceDuration     NVARCHAR(50)     NULL,
    CreatedAt           DATETIME2(7)     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy           UNIQUEIDENTIFIER NOT NULL,
    UpdatedAt           DATETIME2(7)     NULL,
    UpdatedBy           UNIQUEIDENTIFIER NULL,
    IsDeleted           BIT              NOT NULL DEFAULT 0,
    DeletedAt           DATETIME2(7)     NULL,
    DeletedBy           UNIQUEIDENTIFIER NULL
);

CREATE NONCLUSTERED INDEX IX_UnitLibrary_TenantId
    ON ref.UnitLibrary(TenantId) WHERE IsDeleted = 0;
