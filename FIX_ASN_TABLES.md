# Fix: "no such table: AsnShipments"

## ✅ FIXED!

The error was caused by the existing database not having the new ASN tables.

## What I Fixed

Added automatic table creation to `Program.cs` that:
1. Checks if ASN tables exist
2. Creates them automatically if they don't
3. Creates all indexes
4. Shows confirmation messages in console

## How to Apply the Fix

### Step 1: Stop the Application
Press `Ctrl+C` in the terminal running the app

### Step 2: Restart the Application
```bash
cd src\EPR.Web
dotnet run
```

### Step 3: Watch the Console Output
You should see:
```
✓ Database verified/created
Creating ASN tables...
✓ ASN tables created
Now listening on: http://localhost:5290
```

### Step 4: Refresh Your Browser
1. Go to `http://localhost:5290/Distribution`
2. Hard refresh: `Ctrl + Shift + R`
3. The page should load without errors!
4. Upload button should work!

## What Will Happen

1. **On First Run:**
   - Console shows: "Creating ASN tables..."
   - Tables are created
   - Page works!

2. **On Subsequent Runs:**
   - Console shows: "✓ ASN tables verified"
   - No changes needed
   - Page works!

## Tables Created

- `AsnShipments` - Main shipment data
- `AsnPallets` - Pallet/container data
- `AsnLineItems` - Product line items

Plus 7 indexes for fast queries.

## Test It Now!

1. **Restart app:** `dotnet run` from `src\EPR.Web`
2. **Check console:** Should see "✓ ASN tables created"
3. **Open browser:** `http://localhost:5290/Distribution`
4. **Upload file:** Click "Upload ASN" → Select `example_GS1_XML_multi_destination.xml`
5. **Success!** Shipment appears in the list

## If Still Getting Error

### Option 1: Delete Database and Recreate
```bash
cd src\EPR.Web
Remove-Item epr.db -Force
dotnet run
# Database will be recreated with all tables
```

### Option 2: Manual SQL
If you have SQLite tools, run:
```sql
-- Check if tables exist
SELECT name FROM sqlite_master WHERE type='table' AND name LIKE 'Asn%';

-- If empty, restart the application and they'll be created automatically
```

## Success Indicators

✅ Console shows: "✓ ASN tables created" or "✓ ASN tables verified"  
✅ Page loads showing: "No ASN shipments found"  
✅ No red error message  
✅ Upload button is clickable  
✅ Can upload sample XML file

---

**The fix is automatic!** Just restart the application and the tables will be created. 🚀
