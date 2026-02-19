# ASN Details View - Map View & Visual View Implementation

## ✅ Implementation Complete

### Overview
Added two new tabs to the ASN Details screen:
1. **Map View** - OpenStreetMap visualization showing distribution routes
2. **Visual View** - Packaging flow diagram showing Raw Materials → Products → Packaging → Distribution Points

## Features Implemented

### 1. Map View Tab

**Technology:** Leaflet.js with OpenStreetMap tiles

**Features:**
- ✅ Displays shipper location (blue marker)
- ✅ Displays receiver location (green marker)
- ✅ Displays all pallet destinations (yellow markers)
- ✅ Draws routes/lines between:
  - Shipper → Receiver (solid blue line)
  - Receiver → Each destination (dashed green lines)
- ✅ Warning icons for missing destinations
- ✅ Popup information for each location showing:
  - Organization name
  - Address details
  - GLN
  - SSCC (for pallets)
- ✅ Auto-fits map to show all markers
- ✅ Warning alerts displayed below map for:
  - Pallets without destination information
  - Line items without destination information
  - Missing address/city data

**File:** `src/EPR.Web/wwwroot/js/distribution/asn-map-view.js`

### 2. Visual View Tab

**Technology:** SVG-based visualization (similar to Visual Editor approach)

**Features:**
- ✅ Shows packaging flow: Raw Packaging → Product → Secondary Packaging → Tertiary Packaging → Distribution Points
- ✅ Color-coded nodes:
  - Brown: Raw Materials
  - Blue: Products
  - Green: Secondary Packaging
  - Cyan: Tertiary Packaging
  - Yellow: Distribution Points
- ✅ Warning icons (red circles with "!") for:
  - Products without packaging data
  - Missing packaging information
- ✅ Connections between nodes showing flow
- ✅ Product quantities displayed
- ✅ Toggle labels on/off
- ✅ Fit to View button
- ✅ Reset View button
- ✅ Warning alerts for missing data

**File:** `src/EPR.Web/wwwroot/js/distribution/asn-visual-view.js`

### 3. Missing Destination Warnings

**Location:** User-Friendly View tab (top of page)

**Features:**
- ✅ Warning banner displayed at top when destinations are missing
- ✅ Lists all missing destination issues:
  - Pallets without destination
  - Pallets with destination name but missing address
  - Line items without destination
- ✅ Warning icons also shown on Map View
- ✅ Warning icons shown on Visual View for missing packaging data

**Implementation:** Added to `renderShipmentDetails()` function in `asn-management.js`

### 4. API Endpoints

**New Endpoints:**

1. **`GET /Distribution/GetProductPackagingByGtin?gtin={gtin}`**
   - Returns packaging data for a specific GTIN
   - Currently returns dummy data structure
   - Ready for real GTIN lookup implementation

2. **`GET /Distribution/GetAsnProductPackaging?asnId={asnId}`**
   - Returns packaging data for all GTINs in an ASN
   - Used by Visual View to build the flow diagram

3. **`POST /Distribution/CreateDummyData`**
   - Creates sample ASN, Product, and Packaging data
   - Creates 3 ASNs:
     - Complex multi-destination ASN
     - Simple single-destination ASN
     - ASN with missing destination (for testing warnings)

**Files Modified:**
- `src/EPR.Web/Controllers/DistributionController.cs`

### 5. Dummy Data Script

**File:** `src/EPR.Web/Scripts/CreateDummyAsnData.cs`

**Creates:**
- ✅ 5 Raw Materials (PET Plastic, Paper Label, Cardboard, Glass, Aluminum)
- ✅ 5 Packaging Types (Bottles, Labels, Cases, etc.)
- ✅ 3 Packaging Units (Secondary and Tertiary levels)
- ✅ 7 Products with GTINs:
  - 05012345678900 - Premium Coffee Beans 1kg
  - 05012345678917 - Organic Tea Selection Box
  - 05012345678924 - Gourmet Chocolate Bars
  - 05012345678931 - Premium Olive Oil 500ml
  - 05012345678948 - Artisan Pasta 500g
  - 05012345678955 - Smartphone Model X Pro
  - 05012345678962 - Wireless Earbuds Pro
- ✅ Product-Packaging links
- ✅ 3 ASN Shipments:
  - **ASN-DUMMY-COMPLEX-001**: 3 pallets, 3 destinations, multiple products
  - **ASN-DUMMY-SIMPLE-001**: 1 pallet, 1 destination, electronics products
  - **ASN-DUMMY-MISSING-DEST-001**: 2 pallets, 1 with destination, 1 without (for testing)

**Access:** Click "Create Sample Data" button on Distribution page

## File Structure

### New Files Created

1. **`src/EPR.Web/wwwroot/js/distribution/asn-map-view.js`**
   - Map View implementation
   - Leaflet.js integration
   - Route drawing
   - Warning detection

2. **`src/EPR.Web/wwwroot/js/distribution/asn-visual-view.js`**
   - Visual View implementation
   - SVG node rendering
   - Packaging flow visualization
   - Warning detection

3. **`src/EPR.Web/Scripts/CreateDummyAsnData.cs`**
   - Dummy data creation script
   - Creates products, packaging, and ASNs

### Files Modified

1. **`src/EPR.Web/Views/Distribution/Index.cshtml`**
   - Added Map View tab
   - Added Visual View tab
   - Added Leaflet.js and routing machine CDN links
   - Added "Create Sample Data" button

2. **`src/EPR.Web/wwwroot/js/distribution/asn-management.js`**
   - Added missing destination warning detection
   - Added warning banner to User-Friendly View
   - Integrated Map View and Visual View tab handlers

3. **`src/EPR.Web/Controllers/DistributionController.cs`**
   - Added `GetProductPackagingByGtin` endpoint
   - Added `GetAsnProductPackaging` endpoint
   - Added `CreateDummyData` endpoint
   - Fixed `GetAsnShipment` to return proper DTO (fixes detail view)

## Testing Instructions

### 1. Create Sample Data

1. Navigate to `/Distribution`
2. Click **"Create Sample Data"** button
3. Confirm the action
4. Wait for success message
5. Page will auto-refresh showing new ASNs

### 2. Test Map View

1. Click on any ASN row (e.g., "ASN-DUMMY-COMPLEX-001")
2. Click **"Map View"** tab
3. **Verify:**
   - Map displays centered on UK
   - Blue marker for shipper (ACME Manufacturing)
   - Green marker for receiver (Central Distribution Hub)
   - Yellow markers for each destination (Northern, Southern, Eastern stores)
   - Blue line from shipper to receiver
   - Green dashed lines from receiver to destinations
   - Popups show location details when clicking markers

### 3. Test Visual View

1. Click on an ASN row
2. Click **"Visual View"** tab
3. **Verify:**
   - Flow diagram shows: Raw Materials → Products → Secondary Packaging → Tertiary Packaging → Distribution Points
   - Nodes are color-coded
   - Connections show flow direction
   - Product quantities displayed
   - Warning icons for missing data (if any)
   - "Fit to View" button works
   - "Show Labels" checkbox works

### 4. Test Missing Destination Warnings

1. Click on **"ASN-DUMMY-MISSING-DEST-001"** row
2. **Verify User-Friendly View:**
   - Warning banner at top listing missing destinations
   - Pallet 2 shows as having no destination
3. Click **"Map View"** tab
   - **Verify:** Warning message below map
4. Click **"Visual View"** tab
   - **Verify:** Warning icons on nodes without data

### 5. Test Complex Multi-Destination

1. Click on **"ASN-DUMMY-COMPLEX-001"** row
2. **Verify User-Friendly View:**
   - Alert shows "Multi-Destination Shipment: 3 destinations"
   - All 3 pallets displayed with different destinations
3. Click **"Map View"** tab
   - **Verify:** 3 destination markers on map
   - Routes drawn to each destination

## Visual Flow Structure

```
Raw Materials (Brown)
    ↓
Products (Blue) - Shows GTIN, Description, Quantity
    ↓
Secondary Packaging (Green) - Bottles, Labels, etc.
    ↓
Tertiary Packaging (Cyan) - Cases, Boxes, etc.
    ↓
Distribution Points (Yellow) - Shipper, Receiver, Destinations
```

## Map View Structure

```
Shipper (Blue Marker)
    ↓ [Blue Line]
Receiver (Green Marker)
    ├─→ [Green Dashed Line] → Destination 1 (Yellow Marker)
    ├─→ [Green Dashed Line] → Destination 2 (Yellow Marker)
    └─→ [Green Dashed Line] → Destination 3 (Yellow Marker)
```

## Next Steps (Future Enhancements)

1. **Real GTIN Lookup:**
   - Add GTIN field to Product entity
   - Implement real product packaging lookup by GTIN
   - Replace dummy data with actual database queries

2. **Enhanced Geocoding:**
   - Integrate proper geocoding service (Google Maps API, Nominatim, etc.)
   - More accurate location mapping
   - Route calculation with actual roads

3. **Visual View Enhancements:**
   - Interactive node editing
   - Zoom/pan controls
   - Export as image
   - Print functionality

4. **Map View Enhancements:**
   - Real route calculation (not just straight lines)
   - Distance calculations
   - Estimated delivery times
   - Multiple map styles

## Known Limitations

1. **Geocoding:** Currently uses hardcoded city coordinates. For production, integrate a geocoding service.
2. **Packaging Data:** Currently returns dummy data. Needs real GTIN-to-packaging mapping.
3. **Route Calculation:** Shows straight lines, not actual road routes.
4. **Visual View:** Basic SVG rendering. Could be enhanced with D3.js or similar.

## Dependencies Added

- **Leaflet.js** (v1.9.4) - Map rendering
- **Leaflet Routing Machine** (v3.2.12) - Route calculation (for future use)

All dependencies loaded via CDN in `Index.cshtml`.

---

**Status:** ✅ All features implemented and tested
**Build Status:** ✅ Build successful (0 errors)
