# Import Data Tracing Guide

## Overview
This guide explains how to trace imported Excel data from the analyze popup to the database to identify where data is being lost.

## Comprehensive Logging Added

### 1. MaterialInputServiceDb.SaveMaterialInput
- Logs before SaveChanges: Assessment code, stage, category, product, mass, cost
- Logs SaveChanges result: Number of entities saved
- **VERIFICATION**: Immediately queries database after SaveChanges to confirm data exists
- **FAILURE POINT**: If verification fails, data was not saved despite SaveChanges returning success

### 2. DestinationServiceDb.SaveDestinationRow
- Logs before SaveChanges: MaterialInputId, assessment code, stage, IsFood flag, destination count
- Logs SaveChanges result: Number of entities saved
- **VERIFICATION**: Immediately queries database after SaveChanges to confirm destinations exist
- **FAILURE POINT**: If verification fails, destinations were not saved

### 3. OutputServiceDb.SaveOutput
- Logs before SaveChanges: MaterialOutput ID, assessment code, stage, category, product, cost, income
- Logs SaveChanges result: Number of entities saved
- **VERIFICATION**: Immediately queries database after SaveChanges to confirm output exists
- **FAILURE POINT**: If verification fails, output was not saved

### 4. BusinessCostServiceDb.SaveBusinessCost
- Logs before SaveChanges: BusinessCost ID, assessment code, stage, category, description, cost
- Logs SaveChanges result: Number of entities saved
- **VERIFICATION**: Immediately queries database after SaveChanges to confirm cost exists
- **FAILURE POINT**: If verification fails, cost was not saved

### 5. ResourceManagementController.ImportFromExcel
- **POST-IMPORT VERIFICATION**: After all sheets are processed, queries ALL tables to verify data was saved
- Compares expected counts (from summary) vs actual counts (from database)
- Logs sample data from each table to confirm data exists

## How to Use This Logging

1. **Enable Detailed Logging**: Ensure logging level is set to `Information` or `Debug` in `appsettings.json`

2. **Import Excel File**: Use the import feature and watch the console/logs

3. **Check Logs for**:
   - `MaterialInputServiceDb.SaveMaterialInput: START` - Each MaterialInput being saved
   - `SaveChanges completed - X entities saved` - Confirms SaveChanges was called
   - `VERIFICATION SUCCESS` - Confirms data exists in database immediately after save
   - `VERIFICATION FAILED` - **CRITICAL**: Data was not saved despite SaveChanges success
   - `POST-IMPORT DATABASE VERIFICATION` - Final verification showing all tables

4. **Failure Points to Check**:
   - If `VERIFICATION FAILED` appears: DbContext transaction issue or database constraint violation
   - If `SaveChanges completed - 0 entities saved`: Entity was not added to context properly
   - If post-import verification shows 0 counts: Data was cleared after import or never saved

## Common Issues and Solutions

### Issue 1: Data Shows in Analyze but Not Saved
**Symptoms**: Analyze popup shows correct data, but tables are empty after import
**Check**: Look for `VERIFICATION FAILED` messages in logs
**Solution**: Check for DbContext scoping issues - services might be using different DbContext instances

### Issue 2: SaveChanges Returns 0 Entities Saved
**Symptoms**: `SaveChanges completed - 0 entities saved`
**Check**: Entity might not be added to context, or it's a duplicate that's being updated
**Solution**: Check duplicate detection logic - might be finding existing record but not updating it

### Issue 3: Verification Succeeds but Data Disappears
**Symptoms**: `VERIFICATION SUCCESS` but post-import verification shows 0 counts
**Check**: Page reload might be clearing data, or there's a transaction rollback
**Solution**: Check if page reload happens before transaction commits

## Database Check Commands

Run these SQLite commands to verify data:

```sql
-- Check Material Inputs
SELECT COUNT(*) FROM MaterialInputs WHERE AssessmentCode = 'ASMT-001';

-- Check Destinations  
SELECT COUNT(*) FROM Destinations WHERE AssessmentCode = 'ASMT-001';

-- Check Material Outputs
SELECT COUNT(*) FROM MaterialOutputs WHERE AssessmentCode = 'ASMT-001';

-- Check Business Costs
SELECT COUNT(*) FROM BusinessCosts WHERE AssessmentCode = 'ASMT-001';

-- Check specific lifecycle stage
SELECT COUNT(*) FROM MaterialInputs 
WHERE AssessmentCode = 'ASMT-001' 
AND LifecycleStageId IN (SELECT Id FROM AssessmentLifecycleStages WHERE AssessmentCode = 'ASMT-001' AND Title = 'Primary Production');
```

## Next Steps

1. Run an import and check the application logs
2. Look for `VERIFICATION FAILED` messages
3. Check the post-import verification counts
4. Compare expected vs actual counts to identify which table is failing
5. Check the specific service logs for that table to find the exact failure point






