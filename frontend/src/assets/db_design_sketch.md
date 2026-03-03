# HVAC Asset Pro - Comprehensive Database Architecture

This document outlines the professional-grade database architecture for the HVAC management tool, specifically rechecked against the system's current functional requirements (Commissioning, Technical Data Sheets, and ASHRAE 180 Maintenance).

## Detailed Entity Relationship Diagram (ERD)

```mermaid
erDiagram
    %% --- LOOKUP TABLES ---
    LU_EQUIPMENT_TYPES {
        int id PK
        string name "Chiller, AHU, FCU, Pump, Fan, DX Split"
        string category "Mechanical, Electrical, HVAC"
    }
    LU_REFRIGERANTS {
        int id PK
        string name "R410A, R32, R134a, R407C, R404A"
        float gwp_value "Global Warming Potential"
    }
    LU_SERVICE_FREQUENCIES {
        int id PK
        string name "Monthly, Quarterly, Semi-Annual, Annual"
        int months_interval
    }
    LU_ROLES {
        int id PK
        string name "Admin, Field Engineer, Client Manager"
    }
    LU_OWNERSHIP_TYPES {
        int id PK
        string name "Landlord, Tenant, Owner"
    }

    %% --- CORE ENTITIES ---
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
        string contact_person
        string email
        string mobile
        string billing_address
    }

    PROJECTS {
        uuid id PK
        uuid customer_id FK
        string site_name
        string site_address
        string region
        string gps_coordinates
    }

    ASSETS {
        uuid id PK
        uuid project_id FK
        int equipment_type_id FK
        string unit_ref "e.g. Ac1.01"
        string manufacturer
        string indoor_model
        string indoor_serial
        string outdoor_model
        string outdoor_serial
        int refrigerant_id FK
        float factory_charge_kg
        datetime installation_date
        datetime warranty_expiry
        %% ASHRAE / Tech Data Fields
        string system_served
        string fan_duty "Airflow/ESP"
        float cooling_kw
        float heating_kw
        float mca "Max Circuit Amps"
        float mfa "Max Fuse Amps"
        string power_supply_indoor
        string power_supply_outdoor
        string liquid_pipe_size
        string gas_pipe_size
        %% Financials
        int ownership_type_id FK
        string service_responsibility "Owner/Contractor"
        float asset_value
        float depreciation_rate_pct
    }

    %% --- WORKFLOW & PERFORMANCE ---
    COMMISSIONING_RECORDS {
        uuid id PK
        uuid asset_id FK
        uuid engineer_id FK
        datetime record_date
        string witness_name
        json electrical_measurements "L1-L2, L2-L3, Measured Amps"
        json performance_measurements "Suction, Discharge, Airflow, etc"
        json functional_checks_json "Checklist pass/fail"
        text overall_comments
        boolean is_finalized
    }

    SERVICE_SCHEDULE {
        uuid id PK
        uuid asset_id FK
        int frequency_id FK
        datetime next_due_date
        string status "Pending, Overdue, Completed"
    }

    JOB_CARDS {
        uuid id PK
        uuid asset_id FK
        uuid schedule_id FK "Optional if breakdown"
        uuid technician_id FK
        datetime check_in_time
        datetime check_out_time
        string exit_qr_token "Proof of presence"
        boolean fault_discovered
        text fault_description
        text work_performed
        string client_signature_path
        json measurements_json "Current performance data"
        json checklist_results_json "Maintenance tasks status"
    }

    ATTACHMENTS {
        uuid id PK
        uuid linked_id FK "JobCard ID, Asset ID, etc"
        string file_type "Image, PDF, Document"
        string storage_url
        datetime uploaded_at
    }

    %% --- RELATIONSHIPS ---
    LU_ROLES ||--o{ USERS : "defines"
    LU_EQUIPMENT_TYPES ||--o{ ASSETS : "categorizes"
    LU_REFRIGERANTS ||--o{ ASSETS : "charges"
    LU_OWNERSHIP_TYPES ||--o{ ASSETS : "classifies"
    LU_SERVICE_FREQUENCIES ||--o{ SERVICE_SCHEDULE : "calculates"
    
    CUSTOMERS ||--o{ PROJECTS : "owns"
    PROJECTS ||--o{ ASSETS : "houses"
    
    USERS ||--o{ JOB_CARDS : "executes"
    USERS ||--o{ COMMISSIONING_RECORDS : "authorizes"
    
    ASSETS ||--o{ COMMISSIONING_RECORDS : "verified_by"
    ASSETS ||--o{ SERVICE_SCHEDULE : "planned_for"
    ASSETS ||--o{ JOB_CARDS : "maintained_by"
    
    SERVICE_SCHEDULE ||--o{ JOB_CARDS : "generates"
    JOB_CARDS ||--o{ ATTACHMENTS : "contains_photos"
    ASSETS ||--o{ ATTACHMENTS : "manuals_photos"
```

## System Alignment Summary

This detailed design is specifically tailored to the features already present in the UI:

1.  **Technical Data Sheet Support**: The `ASSETS` table now includes all 20+ specialized fields found in the "Technical Data Sheet" stepper, including electrical ratings (MCA/MFA), piping sizes, and financial depreciation.
2.  **ASHRAE 180 Compliance**: The `TASK_LOGS` logic is integrated into `JOB_CARDS` via `checklist_results_json`, allowing the system to store specific results for Mechanical, Electrical, and Performance tasks as defined in the ASHRAE Axial Fan maintenance image.
3.  **Commissioning Performance**: The `COMMISSIONING_RECORDS` table is structured to handle the specialized data found in the Commissioning Form (L1-L2-L3 voltages, suction/discharge pressures, and calculated flow rates).
4.  **Verification Loop**: Proof-of-presence is enforced via `check_in_time`, `check_out_time`, and the `exit_qr_token` in the `JOB_CARDS` table.
5.  **Multi-Asset Photos**: The `ATTACHMENTS` table allows technicians to upload "Fault Pictures" as seen in the Service Entry form, keeping the main data tables lightweight.
6.  **Normalization & Scalability**: By using UUIDs and comprehensive Lookup tables, the system is ready to scale from a single site to a global enterprise asset registry.
