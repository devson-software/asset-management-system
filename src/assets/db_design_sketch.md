# HVAC Asset Pro - Database Design Sketch

This document outlines the proposed database architecture for the Asset Management System. It follows a relational structure optimized for equipment lifecycle tracking and dynamic technical data logging.

## Entity Relationship Diagram (ERD)

```mermaid
erDiagram
    USERS {
        string id PK
        string username
        string password_hash
        string role "Admin, Technician"
    }

    CUSTOMERS {
        string id PK
        string name
        string contact_name
        string email
        string mobile
        string address
        string vat_number
    }

    PROJECTS {
        string id PK
        string customer_id FK
        string name
        string site_address
        string vendor_location
    }

    ASSETS {
        string id PK
        string project_id FK
        string unit_ref "Unique ID on unit"
        string indoor_model
        string outdoor_model
        string serial_number
        string refrigerant_type
        float refrigerant_kg
        string status "Registered, Commissioned, Faulty"
    }

    COMMISSIONING_RECORDS {
        string id PK
        string asset_id FK
        datetime date
        string equipment_type "Pump, Fan, DX Split"
        json electrical_measurements "Voltage, Amps"
        json performance_data "Suction, Discharge, Flow"
        json functional_checks "Checklist results"
        string engineer_name
        string witness_name
    }

    SERVICE_SCHEDULE {
        string id PK
        string asset_id FK
        datetime scheduled_date
        string service_type "Monthly, Quarterly, Annual"
        string status "Pending, Completed, Cancelled"
    }

    JOB_CARDS {
        string id PK
        string asset_id FK
        string schedule_id FK
        datetime completed_date
        string technician_id FK
        boolean fault_found
        text comments
        string exit_qr_token "Verified timestamp"
        json checklist_results
    }

    CUSTOMERS ||--o{ PROJECTS : "has"
    PROJECTS ||--o{ ASSETS : "contains"
    ASSETS ||--o{ COMMISSIONING_RECORDS : "documented_by"
    ASSETS ||--o{ SERVICE_SCHEDULE : "scheduled_for"
    ASSETS ||--o{ JOB_CARDS : "maintained_by"
    USERS ||--o{ JOB_CARDS : "performs"
```

## Key Design Principles

1.  **Hierarchical Core**: The system follows a standard **Client > Site (Project) > Asset** hierarchy. This ensures scalability for clients with multiple geographic locations.
2.  **Asset-Centric History**: Every maintenance action (Commissioning, Scheduling, Job Cards) is linked directly to the `ASSETS` table. This enables a complete audit trail for each unit.
3.  **Dynamic Schemas (JSON)**: `COMMISSIONING_RECORDS` and `JOB_CARDS` utilize JSON columns to handle polymorphic technical data. This allows the system to store Pump measurements and Fan measurements in the same table without rigid schemas.
4.  **Verification Loop**: The `exit_qr_token` in `JOB_CARDS` provides proof-of-presence, ensuring technicians were physically at the asset during the service.
5.  **Analytics Ready**: Storing measurements as structured data allows for future predictive maintenance analytics and equipment performance reporting.
