# 🚀 QUICK TEST GUIDE - Distribution Page

## ⚡ 3-Minute Test

### Step 1: Start App (30 seconds)
```powershell
cd src\EPR.Web
dotnet run
```

Wait for: `Now listening on: http://localhost:5290`

### Step 2: Open Browser (10 seconds)
- **Incognito**: `Ctrl+Shift+N`
- Go to: `http://localhost:5290/Distribution`

### Step 3: Upload File (60 seconds)
1. Click **"Upload ASN"** (blue button, top right)
2. Click **"Choose File"**
3. Select: `src\EPR.Web\wwwroot\sample-data\example_GS1_XML_multi_destination.xml`
4. Click **"Upload & Process"**
5. Wait for alert: **"Success: ASN ASN20260203001 imported successfully"**
6. Click **OK**

### Step 4: Verify (60 seconds)
Page refreshes automatically, you should see:

```
┌─────────────────────────────────────────────────────────────────┐
│ Distribution - ASN Management              [Upload ASN][Refresh]│
├─────────────────────────────────────────────────────────────────┤
│ ASN Number │ Shipper      │ Receiver    │ Ship Date │ Carrier  │
├────────────┼──────────────┼─────────────┼───────────┼──────────┤
│ ASN2026... │ Acme Ware... │ UK Retail...│ Feb 3,... │ Express..│
│            │ Format:      │             │ Est. Del: │          │
│            │ GS1_XML      │             │ Feb 5,... │          │
│            │              │             │           │ [👁️] [🗑️]│
└────────────┴──────────────┴─────────────┴───────────┴──────────┘
```

✅ **SUCCESS!** If you see the table with data, everything works!

---

## 🐛 Troubleshooting

### Problem: "Loading ASN shipments..." won't stop
**Solution 1:** Wait 10 seconds (automatic timeout)
**Solution 2:** Click **"Stop Loading (Click Me!)"** button below the spinner
**Solution 3:** Press **F12** → Console → Type:
```javascript
document.getElementById('loadingIndicator').style.display='none';
document.getElementById('shipmentsContainer').style.display='block';
```

### Problem: Upload button doesn't work
**Solution 1:** Press **F12** → Console → Type:
```javascript
window.openUploadModal()
```
**Solution 2:** Click **"Test Modal"** in debug panel (top right)
**Solution 3:** Hard refresh: `Ctrl+Shift+R`

### Problem: Modal submit button doesn't work
**Solution:** The button has inline JavaScript, so it should always work. If not:
- Check browser console (F12) for errors
- Try another browser
- Clear cache and refresh

### Problem: No data shows after upload
**Solution 1:** Click **"Refresh"** button (gray, top right)
**Solution 2:** Check browser console (F12) for errors. You should see:
```
✅ Loaded 1 shipments
✅ Load complete!
```
**Solution 3:** Verify database:
```powershell
cd src\EPR.Web
sqlite3 epr.db "SELECT * FROM AsnShipments;"
```

### Problem: Database error: "no such table"
**Solution:** The app should auto-create tables on startup. Look for:
```
✓ ASN tables already exist.
```
Or:
```
Creating ASN tables...
✓ ASN tables created
```

If missing, restart the app.

---

## 🔍 What to Check in Console (F12)

### ✅ Good Console Output:
```
ASN Management initialized
🔄 Loading ASN shipments...
📡 Fetching from /Distribution/GetAsnShipments...
📥 Response status: 200
📦 API result: {success: true, data: Array(1)}
✅ Loaded 1 shipments
📋 Rendering shipments list, count: 1
✓ Shipments container visible
✅ Displaying 1 shipments
✓ Table visible
✅ Load complete!
```

### ❌ Bad Console Output:
```
❌ Error loading shipments: ...
Failed to load shipments: ...
```

If you see errors, check:
1. Is the backend running? (Check terminal)
2. Is the URL correct? (Should be `localhost:5290`)
3. Any CORS errors? (Shouldn't happen for same-origin)

---

## 📊 Expected Results

### After Upload:
- ✅ Alert box with success message
- ✅ Modal closes automatically
- ✅ Page reloads
- ✅ Data appears in table

### Table Should Show:
- ASN Number: `ASN20260203001`
- Shipper: `Acme Warehouse Distribution`
- Receiver: `UK Retailers Central Hub`
- Format: `GS1_XML`
- Status: **Pending** (green badge)
- **3 pallets** with **5 items** total

### Click Row → Detail View Shows:
- Full shipper address
- Receiver details
- Transport info (carrier, vehicle)
- **3 pallet cards**, each with:
  - SSCC number
  - Destination
  - Line items (products)
  - Quantities, GTINs, batch numbers

---

## 🎯 Success Checklist

After testing, verify these all work:

- [ ] Page loads without errors
- [ ] Upload button opens modal
- [ ] File can be selected
- [ ] Submit button processes file
- [ ] Success alert appears
- [ ] Page reloads automatically
- [ ] Data appears in table
- [ ] Can click row to see details
- [ ] Can click "Back to List"
- [ ] Can delete shipment (trash icon)
- [ ] No red errors in console

**If all checked, you're done! 🎉**

---

## 📝 Test Data Details

The example file contains:
- **1 Shipment** (ASN20260203001)
- **3 Pallets** with different destinations:
  1. Tesco London - 2 items
  2. Sainsbury's Manchester - 2 items
  3. Asda Birmingham - 1 item
- **5 Total Line Items** (products)
- Various GTINs, batch numbers, and best-before dates

Perfect for testing multi-destination logistics!

---

## 💡 Pro Tips

1. **Use Incognito** - Avoids cache issues
2. **Keep Console Open** - See what's happening
3. **Debug Panel** - Use "Test Modal" for quick access
4. **Network Tab** - Check API calls (F12 → Network)
5. **Multiple Uploads** - Try uploading the same file twice (should fail with "already exists")

---

## 🚀 Ready to Test!

**Just run these commands and follow the steps above:**

```powershell
# Start app
cd src\EPR.Web
dotnet run

# Open browser (manual)
# Navigate to: http://localhost:5290/Distribution

# Upload file
# Select: src\EPR.Web\wwwroot\sample-data\example_GS1_XML_multi_destination.xml

# ✅ Done!
```

**The whole test takes less than 3 minutes!** ⏱️
