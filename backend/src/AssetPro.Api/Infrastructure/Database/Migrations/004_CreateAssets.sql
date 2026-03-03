CREATE TABLE Assets (
    Id                  UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    TenantId            UNIQUEIDENTIFIER NOT NULL REFERENCES Tenants(Id),
    ProjectId           UNIQUEIDENTIFIER NOT NULL REFERENCES Projects(Id),
    UnitRef             NVARCHAR(50)     NOT NULL,
    PlantCategoryId     UNIQUEIDENTIFIER NULL,
    UnitTypeId          UNIQUEIDENTIFIER NULL,
    Manufacturer        NVARCHAR(200)    NOT NULL,
    IndoorModel         NVARCHAR(200)    NOT NULL,
    SerialNumber        NVARCHAR(100)    NOT NULL,
    OutdoorModel        NVARCHAR(200)    NULL,
    OutdoorSerial       NVARCHAR(100)    NULL,
    InstallationDate    DATE             NULL,
    RefrigerantType     NVARCHAR(50)     NOT NULL DEFAULT 'r410a',
    RefrigerantKg       DECIMAL(8,2)     NULL,
    ServiceSchedule     NVARCHAR(50)     NOT NULL DEFAULT 'monthly',
    ServiceDuration     NVARCHAR(50)     NULL,
    VendorArea          NVARCHAR(200)    NULL,
    VendorLocation      NVARCHAR(200)    NULL,
    VendorAddress       NVARCHAR(500)    NULL,
    Status              NVARCHAR(50)     NOT NULL DEFAULT 'registered',
    NameplatePhotoUrl   NVARCHAR(500)    NULL,
    QrCodeUrl           NVARCHAR(500)    NULL,
    CreatedAt           DATETIME2(7)     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy           UNIQUEIDENTIFIER NOT NULL,
    UpdatedAt           DATETIME2(7)     NULL,
    UpdatedBy           UNIQUEIDENTIFIER NULL,
    IsDeleted           BIT              NOT NULL DEFAULT 0,
    DeletedAt           DATETIME2(7)     NULL,
    DeletedBy           UNIQUEIDENTIFIER NULL
);

CREATE NONCLUSTERED INDEX IX_Assets_TenantProject
    ON Assets(TenantId, ProjectId) WHERE IsDeleted = 0;

CREATE UNIQUE NONCLUSTERED INDEX UX_Assets_TenantUnitRef
    ON Assets(TenantId, UnitRef) WHERE IsDeleted = 0;
