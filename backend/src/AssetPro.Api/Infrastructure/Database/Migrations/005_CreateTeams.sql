CREATE TABLE app.Teams (
    Id                  UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    TenantId            UNIQUEIDENTIFIER NOT NULL REFERENCES dbo.Tenants(Id),
    Name                NVARCHAR(100)    NOT NULL,
    Color               NVARCHAR(10)     NULL,
    CreatedAt           DATETIME2(7)     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy           UNIQUEIDENTIFIER NOT NULL,
    UpdatedAt           DATETIME2(7)     NULL,
    UpdatedBy           UNIQUEIDENTIFIER NULL,
    IsDeleted           BIT              NOT NULL DEFAULT 0,
    DeletedAt           DATETIME2(7)     NULL,
    DeletedBy           UNIQUEIDENTIFIER NULL
);

CREATE NONCLUSTERED INDEX IX_Teams_TenantId
    ON app.Teams(TenantId) WHERE IsDeleted = 0;

CREATE TABLE app.TeamMembers (
    Id                  UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    TenantId            UNIQUEIDENTIFIER NOT NULL REFERENCES dbo.Tenants(Id),
    TeamId              UNIQUEIDENTIFIER NOT NULL REFERENCES app.Teams(Id),
    UserId              UNIQUEIDENTIFIER NOT NULL,
    Role                NVARCHAR(50)     NOT NULL DEFAULT 'assistant',
    CreatedAt           DATETIME2(7)     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy           UNIQUEIDENTIFIER NOT NULL,
    UpdatedAt           DATETIME2(7)     NULL,
    UpdatedBy           UNIQUEIDENTIFIER NULL,
    IsDeleted           BIT              NOT NULL DEFAULT 0,
    DeletedAt           DATETIME2(7)     NULL,
    DeletedBy           UNIQUEIDENTIFIER NULL
);

CREATE NONCLUSTERED INDEX IX_TeamMembers_TenantTeam
    ON app.TeamMembers(TenantId, TeamId) WHERE IsDeleted = 0;
