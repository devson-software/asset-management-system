CREATE TABLE Tenants (
    Id                  UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    CompanyName         NVARCHAR(300)    NOT NULL,
    Subdomain           NVARCHAR(100)    NOT NULL,
    ContactEmail        NVARCHAR(256)    NOT NULL,
    ContactPhone        NVARCHAR(50)     NULL,
    LogoUrl             NVARCHAR(500)    NULL,
    SubscriptionPlan    NVARCHAR(50)     NOT NULL DEFAULT 'Trial',
    SubscriptionStatus  NVARCHAR(30)     NOT NULL DEFAULT 'Active',
    SubscriptionExpiry  DATETIME2(7)     NULL,
    CreatedAt           DATETIME2(7)     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy           UNIQUEIDENTIFIER NOT NULL,
    UpdatedAt           DATETIME2(7)     NULL,
    UpdatedBy           UNIQUEIDENTIFIER NULL,
    IsDeleted           BIT              NOT NULL DEFAULT 0,
    DeletedAt           DATETIME2(7)     NULL,
    DeletedBy           UNIQUEIDENTIFIER NULL
);

CREATE UNIQUE NONCLUSTERED INDEX UX_Tenants_Subdomain
    ON Tenants(Subdomain) WHERE IsDeleted = 0;
