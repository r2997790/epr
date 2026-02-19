# Distribution ASN Module - Quick Start Guide

## 🚀 Getting Started in 3 Steps

### Step 1: Run the Application
```bash
cd src\EPR.Web
dotnet run
```

### Step 2: Navigate to Distribution
- Open your browser to `http://localhost:5000` (or the displayed port)
- Click **Distribution** in the top navigation menu

### Step 3: Upload Sample ASN
1. Click the **"Upload ASN"** button
2. Navigate to: `src\EPR.Web\wwwroot\sample-data\example_ASN.xml`
3. Click **"Upload & Process"**
4. ✅ Done! Your first ASN shipment is now in the system

## 📊 What You'll See

### List View
```
┌─────────────────────────────────────────────────────────────────────────┐
│  Distribution - ASN Management                    [Upload ASN] [Refresh] │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                           │
│  ASN Number    │ Shipper        │ Ship Date  │ Pallets │ Destinations  │
│  ─────────────────────────────────────────────────────────────────────  │
│  ASN20260203001│ Acme Foods Ltd │ 03/02/2026 │ 3      │ 🗺️ DC North   │
│                │ (GS1_XML)      │            │        │ 🗺️ DC South   │
│                                                                           │
│  [Click any row to view full details]                                    │
└─────────────────────────────────────────────────────────────────────────┘
```

### Detail View
- Complete shipment information with shipper/receiver cards
- Transport details (carrier, vehicle, dates)
- Individual pallet cards showing:
  - SSCC identifier
  - Destination address
  - Line items with products, GTINs, quantities, batch numbers, expiry dates

## 🎨 Visual Features

### Status Badges
- 🟡 **PENDING** - Awaiting shipment
- 🔵 **IN_TRANSIT** - On the way
- 🟢 **DELIVERED** - Arrived at destination
- 🔴 **CANCELLED** - Shipment cancelled

### Destination Badges
- 📍 Show multiple destinations in a shipment
- Display city and country code
- Click to see full address details

### Interactive Elements
- **Clickable Rows** - Click anywhere on a row to see details
- **Back Navigation** - Easy return to list view
- **Refresh** - Update list without page reload
- **Delete** - Remove ASN shipments (with confirmation)

## 📁 Sample Data Details

The included example ASN (`example_ASN.xml`) contains:

### Shipment Overview
- **From:** Acme Foods Ltd, Manchester, UK
- **To:** MegaMart Supermarkets
- **Date:** February 3, 2026
- **Carrier:** Acme Logistics

### Pallet 1 → DC North (Leeds)
- SSCC: 376141410000000001
- 120 cases Baked Beans
- 60 cases Tomato Soup

### Pallet 2 → DC North (Leeds)
- SSCC: 376141410000000002
- 100 cases Chicken Soup

### Pallet 3 → DC South (Southampton)
- SSCC: 376141410000000003
- 80 cases Baked Beans
- 40 cases Mushroom Soup

## 🔍 Key Features to Try

1. **Multi-Destination Tracking**
   - Notice how pallets 1 & 2 go to Leeds, pallet 3 to Southampton
   - Destinations shown as badges in list view

2. **Product Traceability**
   - Each line item has GTIN, batch number, and expiry date
   - Click detail view to see full traceability data

3. **Visual Organization**
   - Color-coded status
   - Icon-based navigation
   - Card-based layout for easy scanning

4. **Editable Data**
   - Detail view shows all data in organized format
   - Future: Edit status, dates, and transport info inline

## 🌐 Supported Formats

| Format | Description | Status |
|--------|-------------|--------|
| **GS1 XML** | Modern XML-based ASN | ✅ Implemented |
| **EDI 856** | ANSI X12 format | 🚧 Coming Soon |
| **DESADV** | UN/EDIFACT format | 🚧 Coming Soon |

## 💡 Pro Tips

1. **Upload Multiple Files** - Upload different ASN files to see the list grow
2. **Check Destinations** - Notice how multi-destination shipments are visualized
3. **Explore Details** - Click through to see the full hierarchy: Shipment → Pallets → Line Items
4. **Status Tracking** - Future feature: Update status as shipments progress

## 🎯 Use Cases

### EPR Compliance
- Track where products are distributed by region/country
- Report packaging materials by jurisdiction
- Maintain audit trail for compliance

### Supply Chain Visibility
- See real-time distribution data
- Track multi-destination shipments
- Monitor delivery schedules

### Product Recall Management
- Identify which batches went where
- Track by batch number and expiry date
- Quick destination lookup

## 📞 Need Help?

- **Full Documentation:** See `DISTRIBUTION_ASN_GUIDE.md`
- **ASN Format Reference:** See `asn/QUICK_REFERENCE.md`
- **Sample Data Info:** See `src/EPR.Web/wwwroot/sample-data/README.md`

## ✨ What's Next?

After testing with the sample data, you can:
1. Upload your own GS1 XML ASN files
2. Explore the detail view for each shipment
3. Track products across multiple destinations
4. Use the data for EPR reporting and compliance

---

**Ready to go!** Just run the app and click Distribution to start exploring. 🚀
