# ASN Import Specification Package

## Contents

This package contains comprehensive documentation and examples for implementing an Advanced Shipment Notice (ASN) data import system.

### Files Included

1. **ASN_Import_Specification.docx**
   - Complete technical specification document
   - Format descriptions for EDI 856, DESADV, and GS1 XML
   - Cross-format data mapping tables
   - Database schema recommendations
   - Implementation requirements
   - Testing guidelines

2. **example_856_multi_destination.edi**
   - EDI 856 (ANSI X12) format example
   - Demonstrates multi-destination shipment (3 pallets to 2 locations)
   - Includes batch numbers and expiry dates
   - Real-world structure with proper hierarchical levels

3. **example_DESADV_multi_destination.edi**
   - DESADV (UN/EDIFACT) format example
   - Same shipment as EDI 856 for comparison
   - Shows European/international format structure
   - Includes all required segments and qualifiers

4. **example_GS1_XML_multi_destination.xml**
   - GS1 XML ASN format example
   - Same shipment data in modern XML format
   - Clean, hierarchical structure
   - Includes namespaces and schema references

## Example Data Structure

All three example files represent the same shipment:

**Shipper:** Acme Foods Ltd (GLN: 0614141000001)  
**Receiver:** MegaMart Supermarkets  
**ASN Number:** ASN20260203001  
**Ship Date:** 2026-02-03  

**Pallets:**
- **Pallet 1** (SSCC: 376141410000000001) → MegaMart DC North
  - 120 cases Baked Beans (GTIN: 05012345678900)
  - 60 cases Tomato Soup (GTIN: 05012345678917)

- **Pallet 2** (SSCC: 376141410000000002) → MegaMart DC North
  - 100 cases Chicken Soup (GTIN: 05012345678924)

- **Pallet 3** (SSCC: 376141410000000003) → MegaMart DC South
  - 80 cases Baked Beans (GTIN: 05012345678900)
  - 40 cases Mushroom Soup (GTIN: 05012345678931)

## Key Features Demonstrated

### Multi-Destination Tracking
The examples show how to track products going to different warehouse locations within the same shipment - critical for EPR compliance and supply chain visibility.

### Complete Product Traceability
Each line item includes:
- GTIN (product identifier)
- Batch/lot number
- Best before date
- Quantity

### GS1 Standards Compliance
All identifiers use proper GS1 formats:
- GLN (13 digits)
- GTIN (14 digits)
- SSCC (18 digits)

## Implementation Notes

### Format Detection
Your parser should detect the format automatically:
- **EDI 856**: Look for `ISA` and `BSN` segments
- **DESADV**: Look for `UNB`, `UNH`, and message type `DESADV`
- **GS1 XML**: Parse as XML, check for `despatchAdvice` root element

### Testing Approach
1. Parse each example file independently
2. Normalize the data from all three formats
3. Verify the normalized output is identical across formats
4. Test edge cases (missing optional fields, different date formats)

### Common Parsing Challenges

**EDI 856:**
- Hierarchical navigation (HL segments)
- Qualifier-based field interpretation
- Variable date formats

**DESADV:**
- Different separator characters (+ : ')
- Sequential hierarchical structure (CPS)
- Composite data elements

**GS1 XML:**
- Namespace handling
- Optional vs required elements
- ISO 8601 date format

## Database Integration

The specification document includes a recommended 3-table schema:
1. `asn_shipments` - Header information
2. `asn_pallets` - Pallet/container level
3. `asn_line_items` - Product line items

This normalized structure supports:
- Multi-destination tracking
- EPR compliance reporting
- Supply chain analytics
- Traceability queries

## Use Cases

### Primary Use Case: EPR Compliance
Track where products are ultimately delivered for Extended Producer Responsibility reporting by region/country.

### Secondary Use Cases:
- Supply chain visibility dashboards
- Inventory management integration
- Product recall management
- Delivery performance tracking

## Support

For questions or issues with the specification:
1. Review the main specification document
2. Test against all three example files
3. Validate your parsing logic produces consistent normalized output

## Version Information

- **Specification Version:** 1.0
- **Date:** February 2026
- **Status:** Final for Development

## License

This specification and examples are provided for development purposes. 
GS1 standards referenced are property of GS1 Global.
