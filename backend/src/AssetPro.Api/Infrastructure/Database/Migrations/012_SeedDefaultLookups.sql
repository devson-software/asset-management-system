-- Seed system-wide default lookups (TenantId = NULL)
-- These are available to all tenants. Tenants can add their own entries.

DECLARE @SystemUser UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000000';

-- CommissioningType
INSERT INTO Lookups (TenantId, Category, Code, DisplayName, SortOrder, CreatedBy) VALUES
    (NULL, 'CommissioningType', 'pump',      'Pump',      1, @SystemUser),
    (NULL, 'CommissioningType', 'fan',       'Fan',       2, @SystemUser),
    (NULL, 'CommissioningType', 'dx_split',  'DX Split',  3, @SystemUser);

-- ServiceSchedule
INSERT INTO Lookups (TenantId, Category, Code, DisplayName, SortOrder, CreatedBy) VALUES
    (NULL, 'ServiceSchedule', 'monthly',      'Monthly',       1, @SystemUser),
    (NULL, 'ServiceSchedule', 'quarterly',    'Quarterly',     2, @SystemUser),
    (NULL, 'ServiceSchedule', 'bi_annually',  'Bi-Annually',   3, @SystemUser),
    (NULL, 'ServiceSchedule', 'annually',     'Annually',      4, @SystemUser);

-- RefrigerantType
INSERT INTO Lookups (TenantId, Category, Code, DisplayName, SortOrder, CreatedBy) VALUES
    (NULL, 'RefrigerantType', 'r410a',  'R410A',  1, @SystemUser),
    (NULL, 'RefrigerantType', 'r32',    'R32',    2, @SystemUser),
    (NULL, 'RefrigerantType', 'r22',    'R22',    3, @SystemUser),
    (NULL, 'RefrigerantType', 'r407c',  'R407C',  4, @SystemUser),
    (NULL, 'RefrigerantType', 'r134a',  'R134a',  5, @SystemUser);

-- AssetStatus
INSERT INTO Lookups (TenantId, Category, Code, DisplayName, SortOrder, CreatedBy) VALUES
    (NULL, 'AssetStatus', 'registered',       'Registered',       1, @SystemUser),
    (NULL, 'AssetStatus', 'commissioned',     'Commissioned',     2, @SystemUser),
    (NULL, 'AssetStatus', 'active',           'Active',           3, @SystemUser),
    (NULL, 'AssetStatus', 'decommissioned',   'Decommissioned',   4, @SystemUser);

-- ServiceEventStatus
INSERT INTO Lookups (TenantId, Category, Code, DisplayName, SortOrder, CreatedBy) VALUES
    (NULL, 'ServiceEventStatus', 'scheduled',    'Scheduled',     1, @SystemUser),
    (NULL, 'ServiceEventStatus', 'in_progress',  'In Progress',   2, @SystemUser),
    (NULL, 'ServiceEventStatus', 'completed',    'Completed',     3, @SystemUser),
    (NULL, 'ServiceEventStatus', 'cancelled',    'Cancelled',     4, @SystemUser);

-- CommissioningStatus
INSERT INTO Lookups (TenantId, Category, Code, DisplayName, SortOrder, CreatedBy) VALUES
    (NULL, 'CommissioningStatus', 'in_progress',  'In Progress',  1, @SystemUser),
    (NULL, 'CommissioningStatus', 'completed',    'Completed',    2, @SystemUser),
    (NULL, 'CommissioningStatus', 'failed',       'Failed',       3, @SystemUser);

-- WorkType
INSERT INTO Lookups (TenantId, Category, Code, DisplayName, SortOrder, CreatedBy) VALUES
    (NULL, 'WorkType', 'maintenance',       'Maintenance',        1, @SystemUser),
    (NULL, 'WorkType', 'breakdown',         'Breakdown Callout',  2, @SystemUser),
    (NULL, 'WorkType', 'installation',      'Installation',       3, @SystemUser),
    (NULL, 'WorkType', 'inspection',        'Inspection',         4, @SystemUser),
    (NULL, 'WorkType', 'commissioning',     'Commissioning',      5, @SystemUser);

-- ServiceDuration
INSERT INTO Lookups (TenantId, Category, Code, DisplayName, SortOrder, CreatedBy) VALUES
    (NULL, 'ServiceDuration', '30min',     '30 Minutes',  1, @SystemUser),
    (NULL, 'ServiceDuration', '1hour',     '1 Hour',      2, @SystemUser),
    (NULL, 'ServiceDuration', '2hours',    '2 Hours',     3, @SystemUser),
    (NULL, 'ServiceDuration', 'half_day',  'Half Day',    4, @SystemUser),
    (NULL, 'ServiceDuration', 'full_day',  'Full Day',    5, @SystemUser);

-- TeamMemberRole
INSERT INTO Lookups (TenantId, Category, Code, DisplayName, SortOrder, CreatedBy) VALUES
    (NULL, 'TeamMemberRole', 'leader',     'Leader',     1, @SystemUser),
    (NULL, 'TeamMemberRole', 'assistant',  'Assistant',  2, @SystemUser);

-- Seed default PlantHierarchy (system-wide, TenantId = NULL)
-- Level 0 = Category, Level 1 = UnitType (child)

DECLARE @AirConId UNIQUEIDENTIFIER = NEWID();
DECLARE @VentId   UNIQUEIDENTIFIER = NEWID();
DECLARE @RefrigId UNIQUEIDENTIFIER = NEWID();

INSERT INTO PlantHierarchy (Id, TenantId, ParentId, Name, Level, SortOrder, CreatedBy) VALUES
    (@AirConId, NULL, NULL, 'Air Conditioning', 0, 1, @SystemUser),
    (@VentId,   NULL, NULL, 'Ventilation',      0, 2, @SystemUser),
    (@RefrigId, NULL, NULL, 'Refrigeration',    0, 3, @SystemUser);

-- Air Conditioning children
INSERT INTO PlantHierarchy (TenantId, ParentId, Name, Level, SortOrder, CreatedBy) VALUES
    (NULL, @AirConId, 'Split System',    1, 1, @SystemUser),
    (NULL, @AirConId, 'Cassette',        1, 2, @SystemUser),
    (NULL, @AirConId, 'Ducted',          1, 3, @SystemUser),
    (NULL, @AirConId, 'VRV / VRF',       1, 4, @SystemUser),
    (NULL, @AirConId, 'Package Unit',    1, 5, @SystemUser),
    (NULL, @AirConId, 'Chiller',         1, 6, @SystemUser);

-- Ventilation children
INSERT INTO PlantHierarchy (TenantId, ParentId, Name, Level, SortOrder, CreatedBy) VALUES
    (NULL, @VentId, 'Supply Fan',   1, 1, @SystemUser),
    (NULL, @VentId, 'Extract Fan',  1, 2, @SystemUser),
    (NULL, @VentId, 'AHU',          1, 3, @SystemUser);

-- Refrigeration children
INSERT INTO PlantHierarchy (TenantId, ParentId, Name, Level, SortOrder, CreatedBy) VALUES
    (NULL, @RefrigId, 'Walk-in Cooler',   1, 1, @SystemUser),
    (NULL, @RefrigId, 'Walk-in Freezer',  1, 2, @SystemUser),
    (NULL, @RefrigId, 'Display Fridge',   1, 3, @SystemUser);
