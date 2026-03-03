CREATE TABLE Customers (
    Id                  UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    TenantId            UNIQUEIDENTIFIER NOT NULL REFERENCES Tenants(Id),
    Name                NVARCHAR(300)    NOT NULL,
    ContactName         NVARCHAR(200)    NOT NULL,
    Email               NVARCHAR(256)    NOT NULL,
    Mobile              NVARCHAR(50)     NOT NULL,
    Telephone           NVARCHAR(50)     NULL,
    Address             NVARCHAR(500)    NOT NULL,
    BillingAddress      NVARCHAR(500)    NULL,
    VatNumber           NVARCHAR(50)     NULL,
    PictureUrl          NVARCHAR(500)    NULL,
    CreatedAt           DATETIME2(7)     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy           UNIQUEIDENTIFIER NOT NULL,
    UpdatedAt           DATETIME2(7)     NULL,
    UpdatedBy           UNIQUEIDENTIFIER NULL,
    IsDeleted           BIT              NOT NULL DEFAULT 0,
    DeletedAt           DATETIME2(7)     NULL,
    DeletedBy           UNIQUEIDENTIFIER NULL
);

CREATE NONCLUSTERED INDEX IX_Customers_TenantId
    ON Customers(TenantId) WHERE IsDeleted = 0;
