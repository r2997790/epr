# Sample ASN Data

## Test File

This directory contains sample ASN (Advanced Shipping Notice) files for testing the Distribution module.

### example_ASN.xml

This is a GS1 XML ASN format example containing:

- **Shipper:** Acme Foods Ltd (GLN: 0614141000001)
- **Receiver:** MegaMart Supermarkets
- **ASN Number:** ASN20260203001
- **Ship Date:** February 3, 2026

#### Contents:
- **3 Pallets** with Serial Shipping Container Codes (SSCC)
- **2 Destinations:**
  - MegaMart DC North (Leeds) - Pallets 1 & 2
  - MegaMart DC South (Southampton) - Pallet 3
- **5 Product Line Items** with GTINs, batch numbers, and expiry dates

## How to Use

1. Navigate to **Distribution** in the main menu
2. Click **"Upload ASN"**
3. Select the `example_ASN.xml` file
4. Click **"Upload & Process"**

The system will:
- Parse the GS1 XML format
- Extract shipment, pallet, and line item data
- Store it in the database
- Display it in the list view

## Supported Formats

The system supports three ASN formats:

1. **GS1 XML** (.xml) - Modern XML-based format (currently implemented)
2. **EDI 856** (.edi) - ANSI X12 format (coming soon)
3. **DESADV** (.edi) - UN/EDIFACT format (coming soon)

## Data Structure

### Shipment Level
- ASN Number, Ship/Delivery Dates
- Shipper and Receiver information (GLN, name, address)
- Carrier and transport details

### Pallet Level (Despatch Unit)
- SSCC (18-digit identifier)
- Destination GLN and address
- Package type and weight

### Line Item Level
- GTIN (14-digit product identifier)
- Quantity and unit of measure
- Batch number and best before date
- Product description

## EPR Compliance

This ASN data supports Extended Producer Responsibility (EPR) compliance by:
- Tracking product distribution by destination
- Recording quantities sent to different regions/countries
- Maintaining traceability with batch numbers and expiry dates
- Enabling reporting on packaging material flows by geography
