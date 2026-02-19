# ASN Chain Examples

This folder contains example ASN files that demonstrate supply chain nesting/chaining.

## Chain Structure

These three ASN files form a distribution chain:

```
Acme Manufacturing (GLN: 1234567890001)
         |
         | ASN-CHAIN-001: 500 Coffee + 300 Tea
         | Ship Date: 2026-02-05 08:00
         ↓
Central Distribution Hub (GLN: 1234567890002)
         |
         |-- ASN-CHAIN-002: 250 Coffee + 150 Tea → Regional DC North (GLN: 1234567890003)
         |   Ship Date: 2026-02-06 15:00
         |
         └-- ASN-CHAIN-003: 250 Coffee + 150 Tea → Regional DC South (GLN: 1234567890004)
             Ship Date: 2026-02-06 15:30
```

## Key Points

1. **ASN-CHAIN-001**: Manufacturer ships 500 coffee units and 300 tea units to the Central Hub
2. **ASN-CHAIN-002**: Hub redistributes 250 coffee and 150 tea to Northern DC
3. **ASN-CHAIN-003**: Hub redistributes remaining 250 coffee and 150 tea to Southern DC

## GLN Matching (for Auto-Chain Detection)

- ASN-001 **ReceiverGLN** (1234567890002) = ASN-002 **ShipperGLN** (1234567890002)
- ASN-001 **ReceiverGLN** (1234567890002) = ASN-003 **ShipperGLN** (1234567890002)

This matching enables the system to automatically detect that ASN-002 and ASN-003 are child shipments of ASN-001.

## How to Test

1. Upload `chain_example_1_manufacturer_to_hub.xml` first
2. Then upload `chain_example_2_hub_to_north_dc.xml`
3. Finally upload `chain_example_3_hub_to_south_dc.xml`

When the chain detection feature is enabled, the system will automatically:
- Link ASN-002 as a child of ASN-001
- Link ASN-003 as a child of ASN-001
- Display chain visualization showing the full distribution flow

## Products in Chain

- **Premium Coffee Beans 1kg**
  - GTIN: 05012345678900
  - Batch: BATCH-2026-001
  - Best Before: 2026-08-05
  - Total: 500 units → 250 North + 250 South

- **Organic Tea Selection Box**
  - GTIN: 05012345678917
  - Batch: BATCH-2026-002
  - Best Before: 2026-12-31
  - Total: 300 units → 150 North + 150 South

## Upload Order

**Important**: Upload the files in the correct chronological order to properly demonstrate the chain:
1. Manufacturer → Hub (earliest)
2. Hub → North DC (next day)
3. Hub → South DC (same day as North, 30 minutes later)
