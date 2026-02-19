# Excel Import Debugging Guide

## Issues Fixed

1. **Enhanced Error Handling**: Added comprehensive try-catch blocks and error logging
2. **Debug Panels**: Added visible debug panels in modals that persist during import
3. **Response Validation**: Check response status and content-type before parsing JSON
4. **Element Validation**: Verify all DOM elements exist before using them
5. **Server-Side Logging**: Added detailed logging on server-side methods
6. **CSRF Token**: Added `[IgnoreAntiforgeryToken]` to Import 2 and Import 3 methods

## How to Debug Import Issues

### Client-Side Debugging

1. **Open Browser Console** (F12)
   - All import functions now log detailed information
   - Look for messages prefixed with `[Import 2]` or `[Import 3]`

2. **Check Debug Panels**
   - Click "Toggle Debug Log" button in the import modal
   - Debug panel shows last 30 log entries
   - Includes timestamps and detailed error messages

3. **Check Network Tab**
   - Open Network tab in browser DevTools
   - Look for requests to `/AssessmentNavigator/ResourceManagement/ImportFromExcel2` or `ImportFromExcel3`
   - Check:
     - Request status (200 = success, 4xx/5xx = error)
     - Request payload (FormData)
     - Response body (should be JSON)

4. **Common Issues to Check**:
   - **404 Error**: Endpoint not found - check route registration
   - **500 Error**: Server error - check server logs
   - **Network Error**: CORS or connection issue
   - **Empty Response**: Server crashed or returned non-JSON

### Server-Side Debugging

1. **Check Application Logs**
   - Look for log entries starting with `=== ImportFromExcel2 START ===` or `=== ImportFromExcel3 START ===`
   - Check for exceptions or errors

2. **Verify Database Connection**
   - Ensure database is accessible
   - Check that assessment code exists
   - Verify lifecycle stage exists or can be created

3. **Check File Processing**
   - Verify Excel file is being read correctly
   - Check EPPlus license context is set
   - Verify worksheet detection logic

### Additional Debugging Steps

1. **Test with Simple File**
   - Create a minimal Excel file with just headers
   - Test if basic import works

2. **Check Browser Compatibility**
   - Test in different browsers
   - Check if fetch API is supported

3. **Verify JavaScript Execution**
   - Add `console.log('Function called')` at start of functions
   - Verify functions are being called when buttons are clicked

4. **Check Modal Display**
   - Verify modals are showing correctly
   - Check if file input is accessible
   - Verify buttons are clickable

5. **Test Original Import**
   - Test the original "Import from Excel" function
   - Compare behavior with Import 2 and Import 3

## What Was Changed

### JavaScript Functions (`Index.cshtml`)

- `importExcelFile2()`: Enhanced with comprehensive error handling and debug logging
- `importExcelFile3()`: Enhanced with comprehensive error handling and debug logging
- Added `updateDebugPanel()` helper function
- Added debug panels to modals
- Added "Toggle Debug Log" buttons

### Controller Methods (`ResourceManagementController.cs`)

- Added `[IgnoreAntiforgeryToken]` attribute to `ImportFromExcel2` and `ImportFromExcel3`
- Added detailed server-side logging
- Enhanced error messages

## Next Steps if Still Not Working

1. **Check Server Logs**: Look for exceptions or errors in application logs
2. **Test Endpoints Directly**: Use Postman or curl to test endpoints
3. **Verify Routes**: Ensure routes are registered correctly
4. **Check Dependencies**: Verify EPPlus and other NuGet packages are installed
5. **Database Issues**: Check if database operations are completing successfully
6. **File Format**: Verify Excel file format matches expected structure

## Debugging Commands

### Browser Console Commands

```javascript
// Check if functions exist
typeof importExcelFile2
typeof importExcelFile3

// Check stored data
window.importExcelData2
window.importExcelData3

// Manually trigger import (after selecting file)
importExcelFile2('ASMT-006', 'Primary Production')
importExcelFile3('ASMT-006', 'Primary Production')
```

### Server-Side Logging

Check application logs for:
- `=== ImportFromExcel2 START ===`
- `=== ImportFromExcel3 START ===`
- Any exceptions or stack traces

