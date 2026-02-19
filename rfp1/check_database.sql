-- SQLite commands to check database contents
-- Run these commands in SQLite to verify data is being saved

-- Check Material Inputs
SELECT COUNT(*) as MaterialInputCount FROM MaterialInputs WHERE AssessmentCode = 'ASMT-001';

-- Check Destinations
SELECT COUNT(*) as DestinationCount FROM Destinations WHERE AssessmentCode = 'ASMT-001';

-- Check Material Outputs
SELECT COUNT(*) as OutputCount FROM MaterialOutputs WHERE AssessmentCode = 'ASMT-001';

-- Check Business Costs
SELECT COUNT(*) as BusinessCostCount FROM BusinessCosts WHERE AssessmentCode = 'ASMT-001';

-- Check specific lifecycle stage data
SELECT 'MaterialInputs' as TableName, COUNT(*) as Count FROM MaterialInputs WHERE AssessmentCode = 'ASMT-001' AND LifecycleStageId IN (SELECT Id FROM AssessmentLifecycleStages WHERE AssessmentCode = 'ASMT-001')
UNION ALL
SELECT 'Destinations', COUNT(*) FROM Destinations WHERE AssessmentCode = 'ASMT-001' AND LifecycleStageId IN (SELECT Id FROM AssessmentLifecycleStages WHERE AssessmentCode = 'ASMT-001')
UNION ALL
SELECT 'MaterialOutputs', COUNT(*) FROM MaterialOutputs WHERE AssessmentCode = 'ASMT-001' AND LifecycleStageId IN (SELECT Id FROM AssessmentLifecycleStages WHERE AssessmentCode = 'ASMT-001')
UNION ALL
SELECT 'BusinessCosts', COUNT(*) FROM BusinessCosts WHERE AssessmentCode = 'ASMT-001' AND LifecycleStageId IN (SELECT Id FROM AssessmentLifecycleStages WHERE AssessmentCode = 'ASMT-001');

-- Check Activity Logs for imports
SELECT Timestamp, UserId, ActivityType, Details FROM ActivityLogs WHERE ActivityType = 'ImportFromExcel' ORDER BY Timestamp DESC LIMIT 10;






