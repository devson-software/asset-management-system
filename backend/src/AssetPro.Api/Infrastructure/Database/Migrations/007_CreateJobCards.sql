CREATE TABLE JobCards (
    Id                  UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    TenantId            UNIQUEIDENTIFIER NOT NULL REFERENCES Tenants(Id),
    JobNumber           NVARCHAR(30)     NOT NULL,
    AssetId             UNIQUEIDENTIFIER NOT NULL REFERENCES Assets(Id),
    ServiceEventId      UNIQUEIDENTIFIER NULL REFERENCES ServiceEvents(Id),
    TechnicianId        UNIQUEIDENTIFIER NOT NULL,
    Date                DATE             NOT NULL,
    WorkType            NVARCHAR(50)     NOT NULL DEFAULT 'Maintenance',
    CheckInTime         NVARCHAR(20)     NULL,
    CheckOutTime        NVARCHAR(20)     NULL,
    FaultFound          BIT              NOT NULL DEFAULT 0,
    FaultReported       NVARCHAR(MAX)    NULL,
    RootCause           NVARCHAR(MAX)    NULL,
    Remedy              NVARCHAR(MAX)    NULL,
    Comments            NVARCHAR(MAX)    NULL,
    ReadingsJson        NVARCHAR(MAX)    NULL,
    ChecklistJson       NVARCHAR(MAX)    NULL,
    SignatureUrl        NVARCHAR(500)    NULL,
    SignedBy            NVARCHAR(50)     NULL,
    CreatedAt           DATETIME2(7)     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy           UNIQUEIDENTIFIER NOT NULL,
    UpdatedAt           DATETIME2(7)     NULL,
    UpdatedBy           UNIQUEIDENTIFIER NULL,
    IsDeleted           BIT              NOT NULL DEFAULT 0,
    DeletedAt           DATETIME2(7)     NULL,
    DeletedBy           UNIQUEIDENTIFIER NULL
);

CREATE UNIQUE NONCLUSTERED INDEX UX_JobCards_TenantJobNumber
    ON JobCards(TenantId, JobNumber) WHERE IsDeleted = 0;

CREATE NONCLUSTERED INDEX IX_JobCards_TenantAsset
    ON JobCards(TenantId, AssetId) WHERE IsDeleted = 0;
