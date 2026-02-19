# Nested ASN Examples Guide

This document explains all 6 new nested/connected ASN examples and their relationships.

## Overview

These examples demonstrate various types of supply chain nesting and multi-destination scenarios:

1. **EDI 856 Multi-Destination** - Single shipment with multiple pallets going to different destinations
2. **DESADV Chain** - EDIFACT format showing redistribution
3. **Complex Nesting (GS1 XML)** - Multi-pallet shipment with 3 different destinations
4. **Electronics Chain Start** - Manufacturer to distribution hub
5. **Hub to Store A** - Distribution hub to retail store (child of #4)
6. **Hub to Store B** - Distribution hub to another retail store (child of #4)

## Chain Relationships

### Chain 1: Electronics Distribution
```
Tech Manufacturing Corp (GLN: 1234567890010)
         |
         | ASN-NEST-004: 50 Smartphones + 100 Earbuds
         | Ship Date: 2026-02-08 10:00
         ↓
Electronics Distribution Network (GLN: 1234567890011)
         |
         |-- ASN-NEST-005: 25 Smartphones + 50 Earbuds → Tech Store A (GLN: 1234567890012)
         |   Ship Date: 2026-02-09 11:00
         |
         └-- ASN-NEST-006: 25 Smartphones + 50 Earbuds → Tech Store B (GLN: 1234567890013)
             Ship Date: 2026-02-09 11:30
```

**GLN Matching:**
- ASN-NEST-004 **ReceiverGLN** (1234567890011) = ASN-NEST-005 **ShipperGLN** (1234567890011)
- ASN-NEST-004 **ReceiverGLN** (1234567890011) = ASN-NEST-006 **ShipperGLN** (1234567890011)

## File Details

### 1. nested_example_1_edi856_multi_dest.edi
**Format:** EDI 856 (ANSI X12)  
**Purpose:** Demonstrates multi-destination shipment in EDI format  
**Shipper:** ACME MANUFACTURING (GLN: 1234567890001)  
**Receiver:** REGIONAL DISTRIBUTION CENTER (GLN: 1234567890005)  
**Pallets:** 2 pallets
- Pallet 1 (SSCC: 376123456789012350) → NORTHERN RETAIL STORE (GLN: 1234567890006)
- Pallet 2 (SSCC: 376123456789012351) → SOUTHERN RETAIL STORE (GLN: 1234567890007)

**Products:**
- Premium Coffee Beans 1kg (GTIN: 05012345678900) - 500 EA
- Batch: BATCH-2026-001

**Key Features:**
- Multiple HL segments for hierarchical structure
- Multiple N1 segments for different destinations
- MAN segments for SSCC identification

---

### 2. nested_example_2_desadv_chain.edi
**Format:** DESADV (EDIFACT)  
**Purpose:** Shows redistribution from hub to regional DC  
**Shipper:** CENTRAL DISTRIBUTION HUB (GLN: 1234567890002)  
**Receiver:** REGIONAL DC NORTH (GLN: 1234567890003)  
**Pallets:** 2 pallets

**Products:**
- Premium Coffee Beans 1kg (GTIN: 05012345678900) - 250 EA
- Organic Tea Selection Box (GTIN: 05012345678917) - 150 EA
- Gourmet Chocolate Bars (GTIN: 05012345678924) - 100 EA

**Key Features:**
- CPS (Consignment Packing Sequence) segments
- PAC (Package) segments
- GIN (SSCC) segments
- Multiple LIN (Line Item) segments

**Note:** This connects to the original chain examples (chain_example_1, 2, 3) as the hub redistributes goods.

---

### 3. nested_example_3_complex_nesting.xml
**Format:** GS1 XML  
**Purpose:** Complex multi-destination shipment with 3 pallets  
**Shipper:** Global Food Distributors (GLN: 1234567890008)  
**Receiver:** Metro Retail Chain HQ (GLN: 1234567890009)  
**Pallets:** 3 pallets going to different stores

**Pallet 1 (SSCC: 376123456789012370):**
- Premium Olive Oil 500ml (GTIN: 05012345678931) - 200 EA
- Batch: BATCH-OIL-001
- Best Before: 2026-10-15

**Pallet 2 (SSCC: 376123456789012371):**
- Premium Olive Oil 500ml - 150 EA
- Artisan Pasta 500g (GTIN: 05012345678948) - 100 EA
- Batch: BATCH-PASTA-001
- Best Before: 2027-01-20

**Pallet 3 (SSCC: 376123456789012372):**
- Artisan Pasta 500g - 100 EA

**Key Features:**
- Multiple logistic units with different destinations
- Mixed products on pallets
- Different batch numbers and expiry dates

---

### 4. nested_example_4_electronics_chain.xml
**Format:** GS1 XML  
**Purpose:** Start of electronics distribution chain  
**Shipper:** Tech Manufacturing Corp (GLN: 1234567890010)  
**Receiver:** Electronics Distribution Network (GLN: 1234567890011)  
**Pallets:** 1 pallet

**Products:**
- Smartphone Model X Pro (GTIN: 05012345678955) - 50 EA
- Wireless Earbuds Pro (GTIN: 05012345678962) - 100 EA

**Key Features:**
- Serial/batch tracking for electronics
- High-value items
- Single pallet shipment

**Chain Connection:** This is the parent shipment for ASN-NEST-005 and ASN-NEST-006

---

### 5. nested_example_5_hub_to_store_a.xml
**Format:** GS1 XML  
**Purpose:** Distribution hub to retail store (part of electronics chain)  
**Shipper:** Electronics Distribution Network (GLN: 1234567890011) ← **Matches receiver from ASN-NEST-004**  
**Receiver:** Tech Store A - Manchester (GLN: 1234567890012)  
**Pallets:** 1 pallet

**Products:**
- Smartphone Model X Pro - 25 EA (half of original shipment)
- Wireless Earbuds Pro - 50 EA (half of original shipment)

**Key Features:**
- Partial redistribution from hub
- Same batch numbers as parent shipment
- Local delivery to retail store

**Chain Connection:** Child of ASN-NEST-004

---

### 6. nested_example_6_hub_to_store_b.xml
**Format:** GS1 XML  
**Purpose:** Distribution hub to another retail store (part of electronics chain)  
**Shipper:** Electronics Distribution Network (GLN: 1234567890011) ← **Matches receiver from ASN-NEST-004**  
**Receiver:** Tech Store B - Birmingham (GLN: 1234567890013)  
**Pallets:** 1 pallet

**Products:**
- Smartphone Model X Pro - 25 EA (remaining half)
- Wireless Earbuds Pro - 50 EA (remaining half)

**Key Features:**
- Completes the redistribution from hub
- Same batch numbers as parent shipment
- Different destination than ASN-NEST-005

**Chain Connection:** Child of ASN-NEST-004 (sibling of ASN-NEST-005)

## Complete Chain Visualization

```
┌─────────────────────────────────────────┐
│   Tech Manufacturing Corp              │
│   GLN: 1234567890010                   │
│   50 Smartphones + 100 Earbuds         │
└──────────────┬──────────────────────────┘
               │ ASN-NEST-004
               ↓
┌─────────────────────────────────────────┐
│   Electronics Distribution Network      │
│   GLN: 1234567890011                    │
└──────┬──────────────────────┬───────────┘
       │ ASN-NEST-005         │ ASN-NEST-006
       ↓                      ↓
┌──────────────┐      ┌──────────────┐
│ Tech Store A │      │ Tech Store B │
│ Manchester   │      │ Birmingham   │
│ GLN: ...0012 │      │ GLN: ...0013 │
│ 25 + 50      │      │ 25 + 50      │
└──────────────┘      └──────────────┘
```

## Testing Instructions

### Test Multi-Destination Display

1. Upload `nested_example_1_edi856_multi_dest.edi`
2. Click on the row
3. **Verify:** Alert shows "Multi-Destination Shipment: This shipment contains pallets going to 2 different destinations"
4. **Verify:** Two pallets displayed with different destinations

### Test Complex Nesting

1. Upload `nested_example_3_complex_nesting.xml`
2. Click on the row
3. **Verify:** Alert shows "Multi-Destination Shipment: This shipment contains pallets going to 3 different destinations"
4. **Verify:** Three pallets displayed
5. **Verify:** Pallet 2 has 2 different products

### Test Chain Detection (When Enabled)

1. Upload `nested_example_4_electronics_chain.xml` first
2. Then upload `nested_example_5_hub_to_store_a.xml`
3. Then upload `nested_example_6_hub_to_store_b.xml`
4. **Verify:** Chain indicators appear (↑↓) in list view
5. **Verify:** Chain visualization tab appears in detail view
6. **Verify:** Flowchart shows Tech Manufacturing → Hub → (Store A, Store B)

## Upload Order Recommendations

### For Chain Testing:
1. **First:** Upload parent shipments (ASN-NEST-004, chain_example_1)
2. **Then:** Upload child shipments (ASN-NEST-005, ASN-NEST-006, chain_example_2, chain_example_3)

### For Multi-Destination Testing:
- Upload any of: `nested_example_1_edi856_multi_dest.edi` or `nested_example_3_complex_nesting.xml`

### For Format Testing:
- **EDI 856:** `nested_example_1_edi856_multi_dest.edi`
- **DESADV:** `nested_example_2_desadv_chain.edi`
- **GS1 XML:** `nested_example_3_complex_nesting.xml`, `nested_example_4_electronics_chain.xml`, etc.

## GLN Reference Table

| GLN | Organization | Role |
|-----|-------------|------|
| 1234567890001 | ACME MANUFACTURING | Manufacturer |
| 1234567890002 | CENTRAL DISTRIBUTION HUB | Hub |
| 1234567890003 | REGIONAL DC NORTH | Regional DC |
| 1234567890004 | REGIONAL DC SOUTH | Regional DC |
| 1234567890005 | REGIONAL DISTRIBUTION CENTER | Regional DC |
| 1234567890006 | NORTHERN RETAIL STORE | Retail Store |
| 1234567890007 | SOUTHERN RETAIL STORE | Retail Store |
| 1234567890008 | Global Food Distributors | Distributor |
| 1234567890009 | Metro Retail Chain HQ | Retail HQ |
| 1234567890010 | Tech Manufacturing Corp | Manufacturer |
| 1234567890011 | Electronics Distribution Network | Hub |
| 1234567890012 | Tech Store A - Manchester | Retail Store |
| 1234567890013 | Tech Store B - Birmingham | Retail Store |

## Summary

These 6 examples provide comprehensive coverage of:
- ✅ Multiple ASN formats (EDI 856, DESADV, GS1 XML)
- ✅ Multi-destination shipments
- ✅ Supply chain nesting/chaining
- ✅ Different product types (food, electronics)
- ✅ Various batch tracking scenarios
- ✅ Complex pallet configurations

Use these files to thoroughly test the distribution page functionality!
