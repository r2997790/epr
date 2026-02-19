# Import Data Persistence Analysis

## Questions Answered

### 1. Is data being read? ✅ YES
- The import functions successfully read Excel data
- Fields are being parsed correctly
- Categories and items are being identified

### 2. Is data being written to the database? ⚠️ NEEDS VERIFICATION

**Current Implementation:**
- `ImportMaterialInputs2` calls `SaveMaterialInput` for each row
- `SaveMaterialInput` checks `if (_context.ChangeTracker.HasChanges())` before calling `SaveChanges()`
- Controller calls `_context.SaveChanges()` after import
- **POTENTIAL ISSUE**: If `SaveMaterialInput` already saved, the controller's `SaveChanges()` might have nothing to save

**What Was Fixed:**
- Added verification query after import to check if data exists in database
- Added logging to track when SaveChanges is called
- Added check for `HasChanges()` before calling SaveChanges

**How to Verify:**
1. Check server logs for: `"ImportMaterialInputs2: Verification - Found X MaterialInputs in database"`
2. Query database directly:
   ```sql
   SELECT COUNT(*) FROM MaterialInputs 
   WHERE AssessmentCode = 'ASMT-006' AND LifecycleStageId = [stage_id];
   ```

### 3. Are tables connected to the database? ✅ YES

**How Tables Load Data:**
- `loadMaterialInputs()` function calls: `GetMaterialInputs` endpoint
- Endpoint queries: `_context.MaterialInputs.Where(mi => mi.AssessmentCode == code && mi.LifecycleStageId == stageId)`
- Data is returned as JSON and rendered in the table

**Connection Verified:**
- Tables use `fetch()` to call controller endpoints
- Endpoints query database using Entity Framework
- Data flows: Database → Controller → JSON → JavaScript → Table Rendering

### 4. Do tabs refresh with updated data? ⚠️ FIXED

**Previous Issue:**
- Refresh functions were called but might use wrong `code`/`lcStage` values
- Functions might not be accessible in the correct scope

**What Was Fixed:**
- Changed refresh to use `switchLifecycleStage(code, stage)` which reloads entire stage
- Added proper code and stage parameters to refresh calls
- Increased delay to 1000ms to ensure database writes complete
- Added logging to track refresh calls

**How Refresh Works Now:**
1. Import completes and returns `assessmentCode` and `lifecycleStage`
2. Gets current active tab's stage
3. Calls `switchLifecycleStage(code, stage)` which:
   - Updates URL
   - Fetches new HTML via AJAX
   - Reloads all tabs with fresh data from database

## Root Cause Analysis

### Most Likely Issues:

1. **Data Not Persisting**
   - `SaveMaterialInput` might not be calling `SaveChanges()` if ChangeTracker is cleared
   - Solution: Added explicit SaveChanges check and verification query

2. **Wrong Scope for Refresh**
   - `loadMaterialInputs()` uses closure variables `code` and `lcStage` from `_MaterialInputsTab.cshtml`
   - These might not match imported data
   - Solution: Use `switchLifecycleStage()` which reloads entire page section

3. **Timing Issue**
   - Database writes might not complete before refresh
   - Solution: Increased delay to 1000ms and added verification

## Debugging Steps

### Step 1: Verify Data in Database
```sql
-- Check Material Inputs
SELECT * FROM MaterialInputs 
WHERE AssessmentCode = 'ASMT-006' 
ORDER BY LifecycleStageId, CategorySortOrder, InputSortOrder;

-- Check Input Categories
SELECT * FROM InputCategories 
WHERE AssessmentCode = 'ASMT-006';

-- Check Lifecycle Stages
SELECT * FROM AssessmentLifecycleStages 
WHERE AssessmentCode = 'ASMT-006';
```

### Step 2: Check Server Logs
Look for these log entries:
- `"ImportMaterialInputs2: Saved X changes to database"`
- `"ImportMaterialInputs2: Verification - Found X MaterialInputs"`
- `"MaterialInputServiceDb.SaveMaterialInput: SaveChanges completed"`

### Step 3: Check Browser Console
Look for:
- `"[Import 2] Refreshing tables for code: X, stage: Y"`
- `"[Import 2] Calling switchLifecycleStage(...)"`
- `"[Material Inputs] Loading material inputs for code: X, lcStage: Y"`
- `"[Material Inputs] Data received, success: true, inputs count: X"`

### Step 4: Verify Endpoint Response
Check Network tab for `GetMaterialInputs` request:
- Status should be 200
- Response should contain `{"success": true, "inputs": [...]}`
- `inputs` array should have imported items

## Additional Fixes Applied

1. **Enhanced Logging**
   - Added detailed logging at each step
   - Logs when data is saved
   - Logs verification queries

2. **Better Refresh Logic**
   - Uses `switchLifecycleStage()` instead of individual load functions
   - Ensures correct code and stage are used
   - Reloads entire stage section, not just individual tabs

3. **Database Verification**
   - Queries database after import to verify data exists
   - Logs count of records found

4. **Error Handling**
   - Better error messages
   - Debug logs persist in modal
   - Console logging for all steps

## Next Steps if Still Not Working

1. **Check Database Directly**
   - Run SQL queries to see if data exists
   - If data exists but tables don't show it → refresh issue
   - If data doesn't exist → save issue

2. **Check Service Implementation**
   - Verify `MaterialInputServiceDb` is being used (not in-memory service)
   - Check if `SaveChanges()` is actually being called
   - Verify foreign key relationships are correct

3. **Check Front-End**
   - Verify `switchLifecycleStage` function exists and works
   - Check if AJAX request for stage reload succeeds
   - Verify table rendering function receives data

4. **Test Original Import**
   - Compare behavior with original "Import from Excel"
   - If original works but Import 2 doesn't → check differences
   - If neither works → check database/service configuration

