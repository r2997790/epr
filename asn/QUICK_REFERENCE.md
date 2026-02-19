# ASN Parser Quick Reference

## Data Element Extraction Cheat Sheet

| Data Element | EDI 856 | DESADV | GS1 XML |
|--------------|---------|--------|---------|
| **ASN Number** | `BSN02` | `BGM02` | `entityIdentification` |
| **Ship Date** | `BSN03` or `DTM*011` | `DTM+137` | `despatchDateTime` |
| **Delivery Date** | `DTM*067` | `DTM+11` | `estimatedDeliveryDateTime` |
| **Shipper GLN** | `N1*SF` with `N103=92`, get `N104` | `NAD+SU`, 2nd element | `shipper/gln` |
| **Receiver GLN** | `N1*BY` with `N103=92`, get `N104` | `NAD+BY`, 2nd element | `receiver/gln` |
| **Destination GLN** | `N1*ST` (under `HL*P`), get `N104` | `NAD+DP`, 2nd element | `finalShipTo/gln` |
| **SSCC (Pallet)** | `MAN*GM`, get `MAN02` | `GIN+BJ`, 2nd element | `logisticUnitIdentification/sscc` |
| **GTIN (Product)** | `LIN` with `LIN03=SK`, get `LIN04` | `LIN`, 3rd element with `:EN` | `tradeItemIdentification/gtin` |
| **Quantity** | `SN102` | `QTY+12`, 2nd element | `quantityDespatched` |
| **Batch Number** | `REF*LT`, get `REF02` | `RFF+BT`, 2nd element | `transactionalItemData/batchNumber` |
| **Best Before** | `DTM*036`, get `DTM02` | `DTM+361`, 2nd element | `transactionalItemData/itemExpirationDate` |
| **Description** | `PID` segment, 5th element | `IMD` segment, after `:::` | `descriptionShort` |
| **Weight** | `MEA*PD*G`, 3rd element | `MEA+AAE+AAD`, after `+KGM:` | `grossWeight` |

## Format Identification

```python
def detect_format(file_content):
    if file_content.startswith('ISA'):
        return 'EDI_856'
    elif file_content.startswith('UNB') or file_content.startswith('UNH'):
        return 'DESADV'
    elif '<?xml' in file_content and 'despatchAdvice' in file_content:
        return 'GS1_XML'
    else:
        raise ValueError("Unknown ASN format")
```

## EDI 856 Hierarchical Levels

| HL Code | Meaning | Contains |
|---------|---------|----------|
| S | Shipment | Overall shipment info |
| O | Order | Purchase order reference |
| P | Pack | Pallet/container (SSCC) |
| I | Item | Product line items (GTIN) |

**Navigation Example:**
```
HL*7*1*P     # Pallet 7 is child of Shipment 1
HL*8*7*I     # Item 8 is child of Pallet 7
```

## DESADV Key Qualifiers

| Qualifier | Segment | Meaning |
|-----------|---------|---------|
| `SU` | NAD | Supplier/Shipper |
| `BY` | NAD | Buyer/Receiver |
| `DP` | NAD | Delivery Party/Destination |
| `BJ` | GIN | SSCC identifier |
| `EN` | LIN | GTIN (EAN) |
| `BT` | RFF | Batch/Lot number |
| `12` | QTY | Despatch quantity |
| `361` | DTM | Best before date |
| `137` | DTM | Ship date |

## GS1 XML Namespaces

```xml
xmlns:sh="http://www.gs1.org/ecom/ecom_despatch_advice/2-0"
```

**Key XPath expressions:**
```
/sh:despatchAdviceMessage/despatchAdvice/despatchAdviceIdentification/entityIdentification
/sh:despatchAdviceMessage/despatchAdvice/shipper/gln
/sh:despatchAdviceMessage/despatchAdvice/despatchUnit/logisticUnitIdentification/sscc
/sh:despatchAdviceMessage/despatchAdvice/despatchUnit/finalShipTo/gln
/sh:despatchAdviceMessage/despatchAdvice/despatchUnit/despatchUnitItemDetail/tradeItemIdentification/gtin
```

## Date Format Conversion

| Format | Example | Convert To |
|--------|---------|-----------|
| YYYYMMDD | 20260203 | 2026-02-03 |
| YYMMDD | 260203 | 2026-02-03 |
| YYYYMMDD:HHMMSS | 20260203:083000 | 2026-02-03T08:30:00Z |
| ISO 8601 | 2026-02-03T08:30:00Z | (already correct) |

**Python conversion:**
```python
from datetime import datetime

def normalize_date(date_str, format_type):
    if format_type == 'YYYYMMDD':
        return datetime.strptime(date_str, '%Y%m%d').date().isoformat()
    elif format_type == 'YYMMDD':
        return datetime.strptime(date_str, '%y%m%d').date().isoformat()
    elif ':' in date_str:  # EDIFACT format
        return datetime.strptime(date_str, '%Y%m%d:%H%M%S').isoformat() + 'Z'
    else:
        return date_str  # Already ISO format
```

## Check Digit Validation

### GLN/GTIN Check Digit (13 or 14 digits)
```python
def validate_gtin(gtin):
    if len(gtin) not in [13, 14]:
        return False
    
    digits = [int(d) for d in gtin]
    check = digits[-1]
    
    # Calculate expected check digit
    odd_sum = sum(digits[-2::-2])
    even_sum = sum(digits[-3::-2])
    total = (odd_sum * 3) + even_sum
    calculated = (10 - (total % 10)) % 10
    
    return check == calculated
```

### SSCC Check Digit (18 digits)
```python
def validate_sscc(sscc):
    if len(sscc) != 18:
        return False
    
    digits = [int(d) for d in sscc]
    check = digits[-1]
    
    # Calculate expected check digit (same algorithm as GTIN)
    odd_sum = sum(digits[-2::-2])
    even_sum = sum(digits[-3::-2])
    total = (odd_sum * 3) + even_sum
    calculated = (10 - (total % 10)) % 10
    
    return check == calculated
```

## Common Parsing Errors

### EDI 856
- **Error:** Missing qualifier check
  - **Fix:** Always check `N103=92` before extracting GLN from `N104`
  
- **Error:** Wrong hierarchical level
  - **Fix:** Track HL parent-child relationships, don't assume order

### DESADV
- **Error:** Incorrect separator
  - **Fix:** Use `+` for elements, `:` for composites, `'` for segment end

- **Error:** Missing composite extraction
  - **Fix:** `NAD+SU+0614141000001::9` - GLN is in 2nd position, qualifier in 4th

### GS1 XML
- **Error:** Namespace ignored
  - **Fix:** Use namespace-aware XML parser

- **Error:** Optional fields assumed present
  - **Fix:** Check for null/missing elements before accessing

## Validation Checklist

- [ ] ASN number is unique (or handle duplicates per config)
- [ ] GLN is 13 digits with valid check digit
- [ ] GTIN is 14 digits with valid check digit
- [ ] SSCC is 18 digits with valid check digit
- [ ] All dates converted to ISO 8601
- [ ] Quantities are positive numbers
- [ ] Each pallet has at least one line item
- [ ] Destination GLN exists for every pallet
- [ ] Shipper GLN matches expected manufacturer

## Testing Checklist

- [ ] Parse EDI 856 example successfully
- [ ] Parse DESADV example successfully
- [ ] Parse GS1 XML example successfully
- [ ] All three produce identical normalized output
- [ ] Handle missing optional fields (batch, expiry)
- [ ] Reject malformed files with clear errors
- [ ] Process 100+ pallet shipments
- [ ] Complete parsing in < 30 seconds

## Performance Tips

1. **Stream large files** - Don't load entire file into memory
2. **Index by SSCC** - Create lookup tables for fast pallet queries
3. **Batch database inserts** - Use transactions for multi-row inserts
4. **Cache GLN lookups** - Avoid repeated party/location queries
5. **Validate incrementally** - Don't wait until end to check format

## SQL Query Examples

**Get all products going to a specific location:**
```sql
SELECT 
    li.gtin,
    li.quantity,
    li.batch_number,
    li.best_before_date
FROM asn_line_items li
JOIN asn_pallets p ON li.pallet_id = p.id
WHERE p.destination_gln = '0745632100001';
```

**EPR reporting by region:**
```sql
SELECT 
    l.country_code,
    l.city,
    SUM(li.quantity) as total_units
FROM asn_line_items li
JOIN asn_pallets p ON li.pallet_id = p.id
JOIN locations l ON p.destination_gln = l.gln
WHERE li.gtin = '05012345678900'
GROUP BY l.country_code, l.city;
```

**Batch recall query:**
```sql
SELECT 
    s.asn_number,
    p.sscc,
    p.destination_gln,
    li.gtin,
    li.batch_number,
    li.quantity
FROM asn_line_items li
JOIN asn_pallets p ON li.pallet_id = p.id
JOIN asn_shipments s ON p.shipment_id = s.id
WHERE li.batch_number = 'BATCH2026034';
```

---

**Need more help?** Refer to the full specification document for detailed explanations.
