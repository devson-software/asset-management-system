CREATE TABLE CommissioningRecords (
    Id                  UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    TenantId            UNIQUEIDENTIFIER NOT NULL REFERENCES Tenants(Id),
    AssetId             UNIQUEIDENTIFIER NULL REFERENCES Assets(Id),
    UnitRef             NVARCHAR(50)     NOT NULL,
    CommissioningType   NVARCHAR(50)     NOT NULL,
    Status              NVARCHAR(50)     NOT NULL DEFAULT 'in_progress',
    FormDataJson        NVARCHAR(MAX)    NULL,
    CreatedAt           DATETIME2(7)     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy           UNIQUEIDENTIFIER NOT NULL,
    UpdatedAt           DATETIME2(7)     NULL,
    UpdatedBy           UNIQUEIDENTIFIER NULL,
    IsDeleted           BIT              NOT NULL DEFAULT 0,
    DeletedAt           DATETIME2(7)     NULL,
    DeletedBy           UNIQUEIDENTIFIER NULL
);

CREATE NONCLUSTERED INDEX IX_CommissioningRecords_TenantAsset
    ON CommissioningRecords(TenantId, AssetId) WHERE IsDeleted = 0;
