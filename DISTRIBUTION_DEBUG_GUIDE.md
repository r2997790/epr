# Distribution Page - Debugging Guide

## Current Status

✅ **Fixed Issues:**
1. SQL APPLY operation error (SQLite compatibility)
2. GS1 XML parsing with namespace handling
3. JavaScript file location (`asn-management.js` now in correct folder)
4. Added Bootstrap availability check

## "share-modal.js" Error - NOT A REAL ERROR

The error message:
```
share-modal.js:1 Uncaught TypeError: Cannot read properties of null (reading 'addEventListener')
```

**This is a FALSE ERROR from browser DevTools!** There is no `share-modal.js` file in the project.

### Why This Happens:
1. Browser source maps can get confused
2. Console shows wrong file names
3. Error is actually from another script
4. Browser cache issues

### How to Fix:
1. **Hard Refresh:** Press `Ctrl + Shift + R` (Windows) or `Cmd + Shift + R` (Mac)
2. **Clear Cache:** 
   - Open DevTools (F12)
   - Right-click refresh button
   - Click "Empty Cache and Hard Reload"
3. **Disable Cache:** 
   - DevTools → Network tab → Check "Disable cache"

## Testing Steps

### Step 1: Clear Browser Cache
```
1. Open DevTools (F12)
2. Go to Application tab
3. Click "Clear storage"
4. Check all boxes
5. Click "Clear site data"
6. Close DevTools
7. Close browser completely
8. Reopen browser
```

### Step 2: Verify Files Exist
Run in PowerShell from project root:
```powershell
Test-Path "src\EPR.Web\wwwroot\js\distribution\asn-management.js"
# Should return: True

Get-ChildItem "src\EPR.Web\wwwroot\js\distribution\"
# Should show: asn-management.js
```

### Step 3: Access Distribution Page
```
1. Start application: cd src\EPR.Web; dotnet run
2. Open browser to: http://localhost:5290
3. Click "Distribution" menu
4. Check browser console (F12)
```

### Expected Console Output:
```
[Browser Tabs] Initializing browser tabs
ASN Management initialized
Loading ASN shipments...
Response status: 200
API result: {success: true, data: []}
```

### Step 4: Test Upload Button
```
1. Click "Upload ASN" button
2. Console should show: "Upload button clicked"
3. Modal should appear
```

### If Upload Button Still Doesn't Work:

#### Check 1: Is JavaScript File Loading?
Open DevTools → Network tab → Reload page
- Look for `asn-management.js` - should be status 200
- If 404: File path is wrong
- If 304: Cached version

#### Check 2: Are Elements Present?
Open DevTools → Console → Type:
```javascript
document.getElementById('btnUploadAsn')
document.getElementById('uploadModal')
document.getElementById('listView')
```
All should return HTML elements, not `null`

#### Check 3: Is Bootstrap Loaded?
Open DevTools → Console → Type:
```javascript
typeof bootstrap
```
Should return: `"object"`, not `"undefined"`

#### Check 4: Event Listeners
Open DevTools → Console → Type:
```javascript
getEventListeners(document.getElementById('btnUploadAsn'))
```
Should show click event listener

## Common Issues & Solutions

### Issue 1: "Bootstrap is not loaded"
**Cause:** Scripts loading in wrong order

**Solution:**
Check `_Layout.cshtml` has Bootstrap before custom scripts:
```html
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
<script src="~/js/site.js"></script>
@await RenderSectionAsync("Scripts", required: false)
```

### Issue 2: "Cannot read properties of null"
**Cause:** Element not found in DOM

**Solutions:**
1. Check element IDs match
2. Ensure script loads after DOM ready
3. Verify  view renders elements

### Issue 3: Modal Doesn't Appear
**Cause:** CSS or z-index issues

**Solutions:**
1. Check Bootstrap CSS is loaded
2. Inspect modal element (F12)
3. Check for `display: none !important` overrides

### Issue 4: API Returns Error
**Cause:** Database not initialized

**Solution:**
```bash
cd src\EPR.Web
dotnet run
# Database auto-creates on first run
```

## Manual Test Without Modal

If modal still doesn't work, test API directly:

### Test 1: List Shipments
Open DevTools Console:
```javascript
fetch('/Distribution/GetAsnShipments')
  .then(r => r.json())
  .then(d => console.log(d))
```

Expected: `{success: true, data: []}`

### Test 2: Upload File (Manual FormData)
```javascript
// Select file first using: <input type="file" id="test">
const file = document.getElementById('test').files[0];
const formData = new FormData();
formData.append('file', file);

fetch('/Distribution/UploadAsn', {
  method: 'POST',
  body: formData
})
.then(r => r.json())
.then(d => console.log(d))
```

## Nuclear Option: Fresh Start

If nothing works:

```powershell
# 1. Stop application
# 2. Delete database
Remove-Item "src\EPR.Web\epr.db" -Force

# 3. Clear browser completely
# Close all browser windows

# 4. Restart
cd src\EPR.Web
dotnet run

# 5. Open fresh browser window (incognito mode)
# 6. Navigate to http://localhost:5290/Distribution
```

## Verify JavaScript Loading Order

Check page source (Right-click → View Page Source):

Should see scripts in this order:
```html
1. Bootstrap JS
2. site.js  
3. browser-tabs.js (from component)
4. asn-management.js (in Scripts section)
```

## Final Checklist

- [ ] Hard refresh browser (Ctrl+Shift+R)
- [ ] `asn-management.js` file exists and loads (Network tab)
- [ ] Bootstrap is available (`typeof bootstrap === "object"`)
- [ ] Upload button exists (`getElementById('btnUploadAsn')` not null)
- [ ] Upload modal exists (`getElementById('uploadModal')` not null)
- [ ] Console shows "ASN Management initialized"
- [ ] Console shows "Loading ASN shipments..."
- [ ] No red errors in console (ignore "share-modal" phantom error)

## Getting Help

If still broken after all these steps:

1. **Screenshot browser console** (F12 → Console tab)
2. **Screenshot Network tab** showing asn-management.js request
3. **Copy exact error message** (not "share-modal" - that's fake)
4. **Note browser** (Chrome, Firefox, Edge, etc.)

---

**Most likely fix:** Hard refresh (Ctrl+Shift+R) to clear cached scripts!
