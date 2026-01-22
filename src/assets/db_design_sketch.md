# HVAC Asset Pro - Detailed Database Design Sketch (ASHRAE 180 Compliant)

This document outlines the professional-grade database architecture for the Asset Management System, incorporating detailed **ASHRAE 180** standards for Axial Flow Fans and other equipment.

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
    LU_TASK_CATEGORIES {
        int id PK
        string name "Mechanical, Electrical, Performance"
    }

    %% Main Entities
    USERS {
        uuid id PK
        string username
        int role_id FK
    }

    CUSTOMERS {
        uuid id PK
        string name
        string contact_name
    }

    PROJECTS {
        uuid id PK
        uuid customer_id FK
        string site_name
        string site_address
    }

    ASSETS {
        uuid id PK
        uuid project_id FK
        int equipment_type_id FK
        string unit_ref "P-01, AC-01"
        string manufacturer
        string model
        string serial_number
        string system_served "System served by asset"
        string fan_duty "Airflow / ESP"
        float motor_rating_kw "kW"
        string drive_type "Direct, Belt"
        int refrigerant_id FK
        float charge_kg
    }

    %% ASHRAE 180 Checklist Templates
    CHECKLIST_TEMPLATES {
        uuid id PK
        int equipment_type_id FK
        string name "Axial Fan Quarterly, etc."
    }

    TEMPLATE_TASKS {
        uuid id PK
        uuid template_id FK
        int category_id FK
        string task_description
        string method_detail
        string acceptance_criteria
        int default_frequency_id FK
    }

    %% Workflow / Execution
    SERVICE_SCHEDULE {
        uuid id PK
        uuid asset_id FK
        int frequency_id FK
        datetime next_service_due
    }

    JOB_CARDS {
        uuid id PK
        uuid asset_id FK
        uuid schedule_id FK
        uuid technician_id FK
        datetime completion_date
        string overall_condition
        text actions_required
        string client_rep_name
        string signature_path
    }

    TASK_LOGS {
        uuid id PK
        uuid job_card_id FK
        uuid task_id FK "Points to TEMPLATE_TASKS"
        string result "Pass, Fail, N/A"
        string measured_value "For performance checks"
        string units "m3/s, Pa, RPM, dBA"
        text comments
    }

    %% Relationships
    LU_ROLES ||--o{ USERS : "assigned_to"
    LU_EQUIPMENT_TYPES ||--o{ ASSETS : "categorizes"
    LU_REFRIGERANTS ||--o{ ASSETS : "used_in"
    LU_EQUIPMENT_TYPES ||--o{ CHECKLIST_TEMPLATES : "defines_standards"
    
    CHECKLIST_TEMPLATES ||--o{ TEMPLATE_TASKS : "contains"
    LU_TASK_CATEGORIES ||--o{ TEMPLATE_TASKS : "groups"
    
    ASSETS ||--o{ SERVICE_SCHEDULE : "planned_for"
    SERVICE_SCHEDULE ||--o{ JOB_CARDS : "triggers"
    
    JOB_CARDS ||--o{ TASK_LOGS : "records_results"
    TEMPLATE_TASKS ||--o{ TASK_LOGS : "defines_results"
```

## ASHRAE 180 Specific Integration

Based on the **Axial Flow Fan** requirements, the design now handles:

1.  **Detailed Asset Specifications**:
    *   Fields for `system_served`, `fan_duty`, `motor_rating_kw`, and `drive_type` are added to the `ASSETS` table to match the image requirements.
2.  **Structured Task Hierarchy**:
    *   Tasks are split into categories (Mechanical, Electrical, Performance) via `LU_TASK_CATEGORIES`.
    *   `TEMPLATE_TASKS` stores the `Method/Detail` and `Acceptance Criteria` directly, ensuring technicians always know the standard they are testing against.
3.  **Performance Verification (ASHRAE 180 Section)**:
    *   `TASK_LOGS` handles the "Measured Value" and "Units" for critical ASHRAE checks like Airflow (m3/s), External Static Pressure (Pa), Fan Speed (RPM), and Noise/Vibration.
4.  **Compliance Audit Trail**:
    *   By linking `TASK_LOGS` back to `TEMPLATE_TASKS`, the system can generate a PDF report that looks exactly like the Excel spreadsheet provided, including the specific acceptance criteria and frequency for every single line item.
5.  **Sign-off & Actions**:
    *   The `JOB_CARDS` table captures the `Overall Condition`, `Actions Required`, and both the Technician and `Client Rep` names for full accountability.
