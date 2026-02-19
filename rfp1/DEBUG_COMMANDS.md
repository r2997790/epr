# Debugging Commands for Foreign Key Constraint Issue

## Browser Console Commands (F12)

### 1. Check Assessment Exists and Get Exact Code
```javascript
// Check all assessments and find ASMT-001
fetch('/AssessmentNavigator/Home/GetAssessments')
  .then(r => r.json())
  .then(data => {
    console.log('=== ALL ASSESSMENTS ===');
    console.table(data);
    
    const asmt = data.find(a => 
      a.code === 'ASMT-001' || 
      a.code.toLowerCase() === 'asmt-001' ||
      a.code.trim() === 'ASMT-001'
    );
    
    if (asmt) {
      console.log('=== FOUND ASMT-001 ===');
      console.log('Exact code:', asmt.code);
      console.log('Code bytes:', Array.from(asmt.code).map(c => c.charCodeAt(0)));
      console.log('Requested code bytes:', Array.from('ASMT-001').map(c => c.charCodeAt(0)));
      console.log('Match:', 'ASMT-001' === asmt.code);
      console.log('Case-insensitive match:', 'ASMT-001'.toLowerCase() === asmt.code.toLowerCase());
    } else {
      console.error('ASMT-001 NOT FOUND in assessments list');
    }
  })
  .catch(err => console.error('Error:', err));
```

### 2. Test Assessment API Directly
```javascript
// Try to get assessment details
fetch('/AssessmentNavigator/Home/Index?code=ASMT-001')
  .then(r => r.text())
  .then(html => {
    console.log('Assessment page loaded:', html.length, 'bytes');
    // Check if page contains error or assessment data
    if (html.includes('not found') || html.includes('error')) {
      console.error('Assessment not found or error occurred');
    } else {
      console.log('Assessment page loaded successfully');
    }
  });
```

### 3. Monitor Import Request
```javascript
// Intercept fetch requests to see exact data being sent
const originalFetch = window.fetch;
window.fetch = function(...args) {
  if (args[0] && typeof args[0] === 'string' && args[0].includes('ImportFromExcel')) {
    console.log('=== IMPORT REQUEST DEBUG ===');
    console.log('URL:', args[0]);
    console.log('Method:', args[1]?.method || 'POST');
    
    if (args[1]?.body instanceof FormData) {
      console.log('FormData entries:');
      for (let [key, value] of args[1].body.entries()) {
        if (key === 'file') {
          console.log(`  ${key}: [File] ${value.name}, ${value.size} bytes`);
        } else {
          console.log(`  ${key}:`, value);
        }
      }
    }
    
    // Call original fetch and log response
    return originalFetch.apply(this, args)
      .then(response => {
        console.log('Response status:', response.status);
        return response.clone().json()
          .then(data => {
            console.log('Response data:', data);
            if (!data.success) {
              console.error('=== IMPORT FAILED ===');
              console.error('Error type:', data.errorType);
              console.error('Message:', data.message);
              console.error('Inner exception:', data.innerExceptionMessage);
              console.error('Assessment code:', data.assessmentCode);
              console.error('Lifecycle stage:', data.lifecycleStage);
            }
            return response;
          })
          .catch(() => response);
      });
  }
  return originalFetch.apply(this, args);
};

console.log('✅ Fetch interceptor installed. Try importing again.');
```

### 4. Check Current Page Assessment Code
```javascript
// Get assessment code from current page
const urlParams = new URLSearchParams(window.location.search);
const code = urlParams.get('code');
console.log('Current page assessment code:', code);
console.log('Code length:', code?.length);
console.log('Code bytes:', code ? Array.from(code).map(c => c.charCodeAt(0)) : 'N/A');
console.log('Has whitespace:', code ? /\s/.test(code) : 'N/A');
```

### 5. Test Database Query Simulation
```javascript
// Simulate what the server is doing
const testCode = 'ASMT-001';
const normalized = testCode.trim();
console.log('=== CODE NORMALIZATION TEST ===');
console.log('Original:', JSON.stringify(testCode));
console.log('Normalized:', JSON.stringify(normalized));
console.log('Match test:', normalized === testCode);
console.log('Case-insensitive test:', normalized.toLowerCase() === testCode.toLowerCase());
```

## Server-Side Debugging

### Check Server Logs
Look for these log messages in your terminal:
- `"Using exact assessment code from database: '{ExactCode}' (original: '{OriginalCode}')"`
- `"Assessment found with case-insensitive match"`
- `"Assessment '{Code}' not found in database"`

### Add Temporary Debug Endpoint
Add this to ResourceManagementController.cs temporarily:

```csharp
[HttpGet]
public IActionResult DebugAssessment(string code)
{
    var normalizedCode = code?.Trim() ?? string.Empty;
    var exactMatch = _context.Assessments
        .AsNoTracking()
        .FirstOrDefault(a => a.Code == normalizedCode);
    
    var caseInsensitiveMatch = _context.Assessments
        .AsNoTracking()
        .FirstOrDefault(a => a.Code.Equals(normalizedCode, StringComparison.OrdinalIgnoreCase));
    
    var allAssessments = _context.Assessments
        .AsNoTracking()
        .Select(a => new { a.Code, CodeBytes = a.Code.Select(c => (int)c).ToArray() })
        .Take(10)
        .ToList();
    
    return Json(new {
        requestedCode = code,
        normalizedCode = normalizedCode,
        exactMatch = exactMatch != null ? new { exactMatch.Code } : null,
        caseInsensitiveMatch = caseInsensitiveMatch != null ? new { caseInsensitiveMatch.Code } : null,
        allAssessments = allAssessments,
        requestedCodeBytes = normalizedCode.Select(c => (int)c).ToArray()
    });
}
```

Then call it from browser console:
```javascript
fetch('/AssessmentNavigator/ResourceManagement/DebugAssessment?code=ASMT-001')
  .then(r => r.json())
  .then(data => {
    console.log('=== SERVER DEBUG INFO ===');
    console.log(JSON.stringify(data, null, 2));
  });
```

## Database Inspection (if you have SQLite CLI)

```bash
# Connect to database
sqlite3 empauer.db

# Check assessment codes
SELECT Code, LENGTH(Code) as CodeLength, HEX(Code) as CodeHex FROM Assessments WHERE Code LIKE '%ASMT%';

# Check lifecycle stages
SELECT AssessmentCode, Title, LENGTH(AssessmentCode) as CodeLength, HEX(AssessmentCode) as CodeHex 
FROM AssessmentLifecycleStages 
WHERE AssessmentCode LIKE '%ASMT%';

# Compare codes byte-by-byte
SELECT 
    'ASMT-001' as RequestedCode,
    Code as DatabaseCode,
    CASE WHEN 'ASMT-001' = Code THEN 'EXACT MATCH' ELSE 'NO MATCH' END as MatchStatus,
    CASE WHEN LOWER('ASMT-001') = LOWER(Code) THEN 'CASE-INSENSITIVE MATCH' ELSE 'NO MATCH' END as CaseInsensitiveMatch
FROM Assessments 
WHERE Code LIKE '%ASMT%';
```




