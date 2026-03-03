CREATE TABLE ops.ServiceEvents (
    Id                  UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    TenantId            UNIQUEIDENTIFIER NOT NULL REFERENCES dbo.Tenants(Id),
    AssetId             UNIQUEIDENTIFIER NOT NULL REFERENCES app.Assets(Id),
    TeamId              UNIQUEIDENTIFIER NULL REFERENCES app.Teams(Id),
    ServiceType         NVARCHAR(100)    NOT NULL,
    StartDate           DATE             NOT NULL,
    EndDate             DATE             NOT NULL,
    StartTime           NVARCHAR(10)     NULL,
    Duration            NVARCHAR(50)     NULL,
    Status              NVARCHAR(50)     NOT NULL DEFAULT 'scheduled',
    CreatedAt           DATETIME2(7)     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy           UNIQUEIDENTIFIER NOT NULL,
    UpdatedAt           DATETIME2(7)     NULL,
    UpdatedBy           UNIQUEIDENTIFIER NULL,
    IsDeleted           BIT              NOT NULL DEFAULT 0,
    DeletedAt           DATETIME2(7)     NULL,
    DeletedBy           UNIQUEIDENTIFIER NULL
);

CREATE NONCLUSTERED INDEX IX_ServiceEvents_TenantDate
    ON ops.ServiceEvents(TenantId, StartDate) WHERE IsDeleted = 0;
