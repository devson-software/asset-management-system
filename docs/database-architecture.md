# Database Architecture

## Overview

AssetPro uses a multi-tenant SQL Server database with schema-based organization for logical separation, security boundaries, and maintainability.

## Schema Organization

| Schema | Purpose | Tables | Access Pattern |
|---|---|---|---|
| **dbo** | System/infrastructure | Tenants, AspNetUsers, AspNetRoles, Identity tables | System-level, rarely changed |
| **app** | Core application data | Customers, Projects, Assets, Teams, TeamMembers | Business entities, high churn |
| **ops** | Operations/maintenance | ServiceEvents, JobCards, CommissioningRecords | Field operations, service work |
| **ref** | Reference/lookup data | Lookups, PlantHierarchy, UnitLibrary | Tenant-customizable reference data |

## Naming Conventions

### Tables
- PascalCase with descriptive names
- FK columns use `{TableName}Id` (e.g. `ProjectId`, `TenantId`)
- Audit columns: `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`
- Soft delete: `IsDeleted`, `DeletedAt`, `DeletedBy`

### Schemas
- `dbo` - Default SQL Server schema for system tables
- `app` - Application/business entities
- `ops` - Operations/maintenance domain
- `ref` - Reference/lookup data

### Columns
- Primary Keys: `Id` (UNIQUEIDENTIFIER, NEWSEQUENTIALID())
- Tenant-scoped tables: `TenantId` (UNIQUEIDENTIFIER NOT NULL)
- Lookup-driven fields: Store the `Code` from `ref.Lookups` as NVARCHAR(50)
- Hierarchical references: Store GUID FK to `ref.PlantHierarchy`

## Multi-Tenancy Strategy

### Shared Database, Schema-Based Separation
- All tenants share the same database
- `TenantId` column on every tenant-scoped table
- System-wide defaults use `TenantId = NULL` in `ref.*` tables
- Tenant-specific additions use their GUID `TenantId`

### Row-Level Security
- All queries filter by `TenantId` AND `IsDeleted = 0`
- Implemented in application layer via `TenantBehavior` MediatR pipeline
- Database-level RLS can be added later for defense in depth

## Reference Data Architecture

### Hybrid Lookup System

#### 1. `ref.Lookups` - Flat Reference Data
Simple key-value pairs with category discriminator:
```sql
ref.Lookups
├── Category = 'CommissioningType' (pump, fan, dx_split, ...)
├── Category = 'ServiceSchedule' (monthly, quarterly, annual, ...)
├── Category = 'RefrigerantType' (r410a, r32, r410a_alt, ...)
├── Category = 'AssetStatus' (registered, active, decommissioned, ...)
├── Category = 'ServiceEventStatus' (scheduled, in_progress, completed, ...)
├── Category = 'WorkType' (maintenance, repair, installation, ...)
├── Category = 'ServiceDuration' (1hr, 2hr, 4hr, ...)
└── Category = 'TeamMemberRole' (assistant, technician, supervisor, ...)
```

#### 2. `ref.PlantHierarchy` - Hierarchical Data
Parent-child tree for plant categories → unit types:
```sql
ref.PlantHierarchy
├── Level 0: Air Conditioning, Ventilation, Refrigeration
└── Level 1: Split System, Cassette, Ducted, VRV/VRF, Chiller, etc.
```

### Storage Strategy
- **Lookups fields**: Store the `Code` (string) - self-describing, readable SQL
- **PlantHierarchy fields**: Store GUID FK - need JOINs for parent-child resolution
- **System defaults**: `TenantId = NULL` (visible to all tenants)
- **Tenant overrides**: `TenantId = {guid}` (only visible to that tenant)

## Audit & Soft Delete

### Audit Trail
Every table includes:
```sql
CreatedAt    DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME()
CreatedBy    UNIQUEIDENTIFIER NOT NULL
UpdatedAt    DATETIME2(7) NULL
UpdatedBy    UNIQUEIDENTIFIER NULL
```

### Soft Delete
Every table includes:
```sql
IsDeleted    BIT NOT NULL DEFAULT 0
DeletedAt    DATETIME2(7) NULL
DeletedBy    UNIQUEIDENTIFIER NULL
```

### Indexing Strategy
- Filtered indexes: `WHERE IsDeleted = 0` for active data
- Tenant-scoped indexes: Include `TenantId` for tenant isolation
- FK indexes on all foreign keys

## Migration Strategy

### Numbered Scripts
Migrations are numbered sequentially in `Infrastructure/Database/Migrations/`:
```
000_CreateSchemas.sql
001_CreateTenants.sql
002_CreateCustomers.sql
003_CreateProjects.sql
004_CreateAssets.sql
005_CreateTeams.sql
006_CreateServiceEvents.sql
007_CreateJobCards.sql
008_CreateCommissioningRecords.sql
009_CreateUnitLibrary.sql
010_CreateLookups.sql
011_CreatePlantHierarchy.sql
012_SeedDefaultLookups.sql
```

### Schema Creation
1. **000_CreateSchemas.sql** - Creates all schemas (dbo, app, ops, ref)
2. **Subsequent migrations** - Create tables in their target schemas:
```sql
CREATE TABLE app.Assets (...);
CREATE TABLE ref.Lookups (...);
CREATE TABLE ops.ServiceEvents (...);
```

## Security Considerations

### Principle of Least Privilege
- Different schemas can have different permissions
- `ref.*` schema: Read-only for most users, write for admins
- `ops.*` schema: Service technicians only
- `app.*` schema: Full CRUD for business users

### Future Enhancements
- Row-Level Security (RLS) policies per schema
- Database-level encryption for sensitive columns
- Audit triggers for compliance requirements

## Performance Considerations

### Indexing
- Clustered indexes on primary keys (default)
- Non-clustered indexes on foreign keys and query patterns
- Filtered indexes for active records (`IsDeleted = 0`)
- Composite indexes for tenant + common filters

### Query Patterns
- Always include `TenantId` in WHERE clauses
- Use `LEFT JOIN` for optional lookups
- Filter by `IsDeleted = 0` in all queries

### Connection Pooling
- Single connection factory shared across the application
- Dapper for high-performance data access
- EF Core only for ASP.NET Identity tables

## Development Guidelines

### Adding New Tables
1. Determine the correct schema (dbo/app/ops/ref)
2. Follow naming conventions
3. Include audit and soft delete columns
4. Add appropriate indexes
5. Update domain model and vertical slices
6. Add migration script with schema prefix

### Adding New Lookups
1. Add to `ref.Lookups` table via migration
2. Use lowercase `Code` values (e.g. `monthly`, not `Monthly`)
3. Set `TenantId = NULL` for system defaults
4. Update domain model defaults to use the `Code`

### Query Writing
```sql
-- Good: Schema-qualified, tenant-filtered, active-only
SELECT a.Id, a.Name FROM app.Assets a
WHERE a.TenantId = @TenantId AND a.IsDeleted = 0;

-- Bad: No schema, no tenant filter, includes deleted
SELECT Id, Name FROM Assets;
```

## Denormalization Strategy

### Intentional Denormalization

We intentionally denormalize certain data for performance and query simplicity:

#### 1. **TenantId in Reference Tables**
```sql
-- Good: Reference tables include TenantId for tenant-specific lookups
SELECT l.Code, l.DisplayName 
FROM ref.Lookups l
WHERE l.Category = 'RefrigerantType' 
  AND (l.TenantId = @TenantId OR l.TenantId IS NULL)
  AND l.IsDeleted = 0;

-- System defaults (TenantId = NULL) available to all tenants
-- Tenant overrides (TenantId = @TenantId) only visible to that tenant
```

**Why this is intentional:**
- Allows system-wide defaults with `TenantId = NULL`
- Enables tenant-specific customizations with their own `TenantId`
- Single query gets both defaults + tenant overrides
- Performance: No separate calls for system vs tenant data
- Data isolation: Each tenant only sees their customizations

#### 2. **Lookup Codes vs Display Names**
- Store **lookup codes** (e.g., `'r410a'`) in domain models for data integrity
- Resolve to **display names** (e.g., `'R410A'`) in queries for UI
- System defaults use `TenantId = NULL` for consistency

#### 3. **When to Denormalize**
- **Read-heavy operations** (asset lists, dashboards, reports)
- **Display purposes** (dropdowns, UI labels)
- **Performance-critical paths** (avoiding JOINs in hot paths)

#### 4. **When NOT to Denormalize**
- **Write operations** (always use normalized FKs)
- **Business logic** (use codes, not display names)
- **Data integrity** (FK constraints enforce relationships)

### Trade-offs

**Benefits:**
- Faster queries (fewer JOINs in common operations)
- Simpler API responses (display names included)
- Better user experience (no separate lookup calls)

**Costs:**
- Slightly more complex queries
- Need to maintain JOIN logic consistency
- Display name changes require query updates (rare)

This is a deliberate performance optimization, not accidental denormalization.

## Documentation Maintenance

- This document lives in `/docs/database-architecture.md`
- Update when adding new schemas or major tables
- Keep migration scripts in sync with documented patterns
- Review quarterly for accuracy and completeness
