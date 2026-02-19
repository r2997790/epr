# Excel Import Diagnostic - Why Values Don't Appear in Tabs

## Root Causes Identified

### 1. **Column Index Mismatch** ⚠️ CRITICAL
**Problem**: The field mappings use Excel column numbers (1-indexed), but there may be confusion between:
- Column index stored in mappings (e.g., `col = 5` for column E)
- How EPPlus reads cells (`worksheet.Cells[row, col]`)

**Location**: `ResourceManagementController.cs` - `ImportFoodDestinations`, `ImportMaterialInputs`, etc.

**Fix Needed**: Ensure column indices from mappings are used correctly with EPPlus (which uses 1-indexed columns).

### 2. **Material Matching Failure** ⚠️ CRITICAL  
**Problem**: Destination imports require matching MaterialInput rows. If materials aren't found:
- Destination values are skipped (`continue` statement)
- No error is shown to user
- Data appears to import but nothing is saved

**Location**: `ResourceManagementController.cs` lines 1355-1359

**Fix Needed**: 
- Ensure Material Inputs are imported BEFORE destinations
- Improve material matching logic
- Add better error logging

### 3. **Field Mapping Not Applied Correctly**
**Problem**: Field mappings might not be passed correctly or column indices might be wrong.

**Location**: `ResourceManagementController.cs` lines 1371-1373

**Fix Needed**: Verify mappings are correctly extracted and applied.

### 4. **Data Persistence Issue**
**Problem**: Services use in-memory static dictionaries. Data lost on:
- Application restart
- Key mismatches (assessment code, lcStage)
- Different service instances

**Location**: `DestinationService.cs`, `MaterialInputService.cs`

**Fix Needed**: Add persistence layer or ensure data survives page reloads.

### 5. **Frontend Retrieval Issue**
**Problem**: Frontend merges data from two endpoints. If IDs don't match, merge fails silently.

**Location**: `_DestinationsTab.cshtml` lines 147-152

**Fix Needed**: Improve ID matching logic.

## Recommended Fixes

1. **Add comprehensive logging** to trace the entire import flow
2. **Fix column index handling** to ensure 1-indexed columns are used consistently
3. **Improve material matching** with better fallback logic
4. **Add validation** to ensure materials exist before importing destinations
5. **Fix data retrieval** to handle ID mismatches





