# HVAC Asset Pro

A professional HVAC management tool built with Quasar.js for tracking HVAC units, service schedules, and maintenance records.

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

## Setup

1. Install dependencies:
   ```bash
   npm install
   ```
2. Run in development mode:
   ```bash
   quasar dev
   ```
3. Build for production:
   ```bash
   quasar build
   ```

## Workflow

1. **Login**: Access the technician portal.
2. **Register**: Add a customer and their specific project/site.
3. **Capture**: Fill in the technical data sheet for the equipment.
4. **Service**: Use the calendar to perform scheduled maintenance via QR code.
5. **Report**: Generate job cards and export the register for accounting.
