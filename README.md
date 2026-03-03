# HVAC Asset Pro

A professional multi-tenant HVAC management SaaS for tracking HVAC units, service schedules, and maintenance records.

## Repository Structure

```
├── frontend/    # Vue 3 / Quasar SPA
├── backend/     # .NET 9 Web API (Vertical Slice Architecture)
└── README.md
```

## Features

- **Customer Registration**: Manage clients and their logos.
- **Project & Unit Tracking**: Detailed asset specifications and technical drawings.
- **Technical Data Sheets**: Deep technical specs for indoor/outdoor units.
- **Service Management**:
  - Dynamic checklists based on service frequency (Monthly to Annual).
  - Automatic next service scheduling.
  - Job card generation with electronic signatures and fault photos.
- **Cloud Storage**: Secure job card history and reporting.
- **Asset Register**: Full inventory overview with Excel export and QR label printing.

## Frontend Setup

```bash
cd frontend
npm install
npx quasar dev
```

## Backend Setup

```bash
cd backend/src/AssetPro.Api
dotnet run
```

## Branches

- **main** — Wireframe / client demo (frontend only)
- **develop** — Active development (monorepo with backend)
