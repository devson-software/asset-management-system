# HVAC Asset Pro - Detailed Database Design Sketch

This document outlines the professional-grade database architecture for the Asset Management System, including detailed lookups and technical specifications required for CIBSE and ASHRAE compliance.

## Detailed Entity Relationship Diagram (ERD)

```mermaid
erDiagram
    %% Lookup Tables
    LU_ROLES {
        int id PK
        string role_name "Admin, Tech, Manager, Client"
    }
    LU_EQUIPMENT_TYPES {
        int id PK
        string name "Chiller, AHU, FCU, Pump, Fan"
        string code "CHLR, AHU, FCU, PMP, FAN"
    }
    LU_REFRIGERANTS {
        int id PK
        string name "R410A, R32, R134a, R407C"
    }
    LU_SERVICE_FREQUENCIES {
        int id PK
        string name "Monthly, Quarterly, Bi-Annual, Annual"
        int months_interval
    }
    LU_JOB_STATUSES {
        int id PK
        string name "Scheduled, In-Progress, Completed, Review, Cancelled"
    }

    %% Main Entities
    USERS {
        uuid id PK
        string username
        string email
        string password_hash
        int role_id FK
        datetime last_login
    }

    CUSTOMERS {
        uuid id PK
        string name
        string vat_number
        string account_code
        string contact_name
        string email
        string phone
        string billing_address
    }

    PROJECTS {
        uuid id PK
        uuid customer_id FK
        string site_name
        string site_address
        string region "GP, KZN, WC, etc."
        string gps_coordinates
    }

    ASSETS {
        uuid id PK
        uuid project_id FK
        int equipment_type_id FK
        string unit_ref "P-01, AC-01"
        string indoor_model
        string outdoor_model
        string serial_number
        int refrigerant_id FK
        float charge_kg
        datetime installation_date
        datetime warranty_expiry
        string barcode_id
    }

    %% Technical / Workflow Entities
    COMMISSIONING_RECORDS {
        uuid id PK
        uuid asset_id FK
        datetime date
        uuid engineer_id FK
        string witness_name
        json design_specs "Design Flow, Design Power"
        json measured_specs "Actual Flow, Actual Power"
        json electrical_data "L1, L2, L3 Amps/Volts"
        boolean status_finalized
    }

    SERVICE_SCHEDULE {
        uuid id PK
        uuid asset_id FK
        int frequency_id FK
        datetime next_service_due
        int current_status_id FK
    }

    JOB_CARDS {
        uuid id PK
        uuid asset_id FK
        uuid schedule_id FK
        uuid technician_id FK
        datetime check_in_time
        datetime check_out_time
        string exit_qr_token
        boolean spares_used
        json measurements "Pressures, Temps, Currents"
        json checklist_json "Standard/Specialized tasks"
        text tech_notes
        string client_signature_path
    }

    %% Relationships
    LU_ROLES ||--o{ USERS : "assigned_to"
    LU_EQUIPMENT_TYPES ||--o{ ASSETS : "categorizes"
    LU_REFRIGERANTS ||--o{ ASSETS : "used_in"
    LU_SERVICE_FREQUENCIES ||--o{ SERVICE_SCHEDULE : "defines"
    LU_JOB_STATUSES ||--o{ SERVICE_SCHEDULE : "tracks"
    
    USERS ||--o{ JOB_CARDS : "performs"
    CUSTOMERS ||--o{ PROJECTS : "owns"
    PROJECTS ||--o{ ASSETS : "located_at"
    
    ASSETS ||--o{ COMMISSIONING_RECORDS : "certified_by"
    ASSETS ||--o{ SERVICE_SCHEDULE : "planned_for"
    ASSETS ||--o{ JOB_CARDS : "serviced_by"
    
    SERVICE_SCHEDULE ||--o{ JOB_CARDS : "triggers"
```

## Key Design Enhancements

1.  **Lookup Integrity**: By moving types (Refrigerants, Equipment Types, Frequencies) into dedicated lookup tables, we ensure data consistency and allow for easy global updates or translations.
2.  **Granular Assets**: The `ASSETS` table now includes barcode tracking, warranty dates, and specific refrigerant charge fields, making it a true "Digital Twin" of the physical unit.
3.  **Audit-Ready Job Cards**: `JOB_CARDS` now track precise `check_in_time` and `check_out_time`, combined with the `exit_qr_token` for undeniable proof of service duration and location.
4.  **Service Lifecycle**: The `SERVICE_SCHEDULE` acts as a bridge between the Asset and the Job Card, allowing the system to automate future maintenance dates based on the `LU_SERVICE_FREQUENCIES`.
5.  **Multi-Dimensional Measurements**: Both Commissioning and Job Cards use `json` types for measurements, providing the flexibility to store high-precision data (e.g., individual phase currents or multi-stage pressure readings) without schema bloat.
6.  **Regional Management**: Projects include a `region` and `gps_coordinates` to assist in technician dispatching and regional reporting.
