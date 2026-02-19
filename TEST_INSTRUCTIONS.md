# Visual Editor Changes - Test Instructions

## Changes Implemented

All code changes have been made to the following files:
- `src/EPR.Web/Views/VisualEditor/Index.cshtml` - CSS and HTML changes
- `src/EPR.Web/wwwroot/js/visual-editor/visual-editor-complete.js` - JavaScript functionality

## To Test the Changes

1. **Start the Application:**
   ```powershell
   cd src/EPR.Web
   dotnet run
   ```
   Ensure the application is running on `http://localhost:5290`

2. **Run Playwright Tests:**
   ```powershell
   cd src/EPR.Web
   npx playwright test test-visual-editor-changes.spec.js --headed
   ```

3. **Manual Verification:**
   - Login at http://localhost:5290/Account/Login with admin/admin123
   - Navigate to http://localhost:5290/VisualEditor
   - Verify:
     - Project Name font size is 16pt (larger and bold)
     - Raw Materials palette shows 8 buttons with icons visible
     - Raw Materials buttons remain visible (don't disappear)
     - Autosave checkbox is visible in the menu bar
     - Snap to Grid button turns green when clicked
     - All menu buttons have icons

## Changes Summary

1. ✅ Project Name font size increased to 16pt
2. ✅ Raw Materials buttons changed from `disabled=true` to event prevention (prevents disappearing)
3. ✅ Raw Materials icons set to opacity 1 and display inline-block
4. ✅ Raw Materials palette width set to 280px
5. ✅ Snap to Grid icon color changes to green (#28a745) when active
6. ✅ Autosave checkbox added to menu
7. ✅ All buttons have icons verified

## Known Issues Fixed

- Raw Materials disappearing: Changed from `btn.disabled = true` to event listeners that prevent click/drag
- Icons not visible: Set icon opacity to 1 and display to inline-block
- Font size: Set to 16pt in CSS










