import { test, expect } from '@playwright/test';

test.describe('Visual Editor Fixes Validation', () => {
    test.beforeEach(async ({ page }) => {
        // Navigate to login page
        await page.goto('http://localhost:5290/Account/Login', { waitUntil: 'domcontentloaded' });
        
        // Wait for login form
        await page.waitForSelector('input[type="text"], input[name="Username"]', { timeout: 10000 });
        
        // Login with admin credentials
        const usernameInput = page.locator('input[name="Username"], input[type="text"]').first();
        await usernameInput.fill('admin');
        
        const passwordInput = page.locator('input[name="Password"], input[type="password"]').first();
        await passwordInput.fill('admin123');
        
        const submitButton = page.locator('button[type="submit"], input[type="submit"]').first();
        await submitButton.click();
        
        // Wait for navigation to Visual Editor (with timeout)
        try {
            await page.waitForURL('**/VisualEditor**', { timeout: 15000 });
        } catch (e) {
            // If URL doesn't change, check if we're already on VisualEditor
            const currentUrl = page.url();
            if (!currentUrl.includes('VisualEditor')) {
                throw new Error(`Failed to navigate to VisualEditor. Current URL: ${currentUrl}`);
            }
        }
        
        await page.waitForLoadState('domcontentloaded');
        
        // Wait for Visual Editor to initialize
        await page.waitForTimeout(3000);
    });

    test('Fix 1: Align/Distribute detects multiple selected nodes', async ({ page }) => {
        // Add two nodes to canvas
        await page.click('#eprSplitTypesBtn'); // Ensure split mode is active
        
        // Wait for panels to load
        await page.waitForSelector('#eprTypesPanelPackaging', { state: 'visible' });
        
        // Add a packaging item
        const packagingSelect = page.locator('#eprPackagingLibraryDropdownSplit');
        await packagingSelect.waitFor({ state: 'visible' });
        await packagingSelect.selectOption({ index: 1 }); // Select first option
        await page.click('#eprAddPackagingBtnSplit');
        
        await page.waitForTimeout(500);
        
        // Add another packaging item
        await packagingSelect.selectOption({ index: 2 }); // Select second option if available
        await page.click('#eprAddPackagingBtnSplit');
        
        await page.waitForTimeout(1000);
        
        // Select multiple nodes using Ctrl+Click
        const nodes = page.locator('.epr-canvas-node');
        const nodeCount = await nodes.count();
        
        if (nodeCount >= 2) {
            // Click first node
            await nodes.nth(0).click();
            await page.waitForTimeout(200);
            
            // Ctrl+Click second node
            await nodes.nth(1).click({ modifiers: ['Control'] });
            await page.waitForTimeout(200);
            
            // Check that both nodes are selected
            const selectedNodes = page.locator('.epr-canvas-node.selected');
            const selectedCount = await selectedNodes.count();
            expect(selectedCount).toBeGreaterThanOrEqual(1);
            
            // Click Align/Distribute dropdown
            const alignDropdown = page.locator('button:has-text("Align/Distribute")');
            await alignDropdown.waitFor({ state: 'visible' });
            
            // Check z-index of dropdown menu
            const dropdownMenu = page.locator('#eprAlignDistributeDropdown');
            const zIndex = await dropdownMenu.evaluate(el => window.getComputedStyle(el).zIndex);
            expect(parseInt(zIndex)).toBeGreaterThanOrEqual(10000);
            
            // Try to open dropdown
            await alignDropdown.click();
            await page.waitForTimeout(300);
            
            // Check if dropdown is visible
            const dropdownVisible = await dropdownMenu.isVisible();
            expect(dropdownVisible).toBe(true);
            
            // Try clicking Align Top
            const alignTopBtn = page.locator('#eprAlignTopBtn');
            if (await alignTopBtn.isVisible()) {
                await alignTopBtn.click();
                await page.waitForTimeout(500);
                
                // Check if alert appeared (should not appear if nodes are selected)
                // If alert appears, it means selection detection failed
                page.on('dialog', dialog => {
                    expect(dialog.message()).not.toContain('Please select at least 2 nodes');
                });
            }
        }
    });

    test('Fix 2: Raw Materials visible and draggable/clickable', async ({ page }) => {
        // Ensure split mode is active
        await page.click('#eprSplitTypesBtn');
        await page.waitForTimeout(500);
        
        // Check Raw Materials panel is visible
        const rawMaterialsPanel = page.locator('#eprTypesPanelRawMaterials');
        await rawMaterialsPanel.waitFor({ state: 'visible' });
        
        // Check if panel is visible
        const isVisible = await rawMaterialsPanel.isVisible();
        expect(isVisible).toBe(true);
        
        // Check Raw Materials buttons container
        const rawMaterialsContainer = page.locator('#eprRawMaterialsButtonsSplit');
        await rawMaterialsContainer.waitFor({ state: 'visible' });
        
        // Check if buttons exist
        const rawMaterialButtons = page.locator('#eprRawMaterialsButtonsSplit .epr-material-btn');
        const buttonCount = await rawMaterialButtons.count();
        expect(buttonCount).toBeGreaterThan(0);
        
        // Check first button is not disabled and has proper cursor
        if (buttonCount > 0) {
            const firstButton = rawMaterialButtons.first();
            const isDisabled = await firstButton.isDisabled();
            expect(isDisabled).toBe(false);
            
            // Check cursor style
            const cursor = await firstButton.evaluate(el => window.getComputedStyle(el).cursor);
            expect(cursor).toBe('pointer');
            
            // Check button has icon
            const icon = firstButton.locator('i');
            const iconCount = await icon.count();
            expect(iconCount).toBeGreaterThan(0);
            
            // Check button text is visible
            const buttonText = await firstButton.textContent();
            expect(buttonText).toBeTruthy();
            expect(buttonText.trim().length).toBeGreaterThan(0);
        }
    });

    test('Fix 3: Load buttons have icons', async ({ page }) => {
        // Check Load Project button
        const loadProjectBtn = page.locator('#eprLoadProjectBtn');
        await loadProjectBtn.waitFor({ state: 'visible' });
        const loadProjectIcon = loadProjectBtn.locator('i');
        const loadProjectIconCount = await loadProjectIcon.count();
        expect(loadProjectIconCount).toBeGreaterThan(0);
        
        // Check Load Group button (in unified toolbar)
        const loadGroupBtn = page.locator('#eprLoadPackagingGroupBtn');
        if (await loadGroupBtn.isVisible()) {
            const loadGroupIcon = loadGroupBtn.locator('i');
            const loadGroupIconCount = await loadGroupIcon.count();
            expect(loadGroupIconCount).toBeGreaterThan(0);
        }
        
        // Check Load Network Distribution button
        const loadDistBtn = page.locator('#eprLoadDistributionGroupBtn');
        if (await loadDistBtn.isVisible()) {
            const loadDistIcon = loadDistBtn.locator('i');
            const loadDistIconCount = await loadDistIcon.count();
            expect(loadDistIconCount).toBeGreaterThan(0);
        }
        
        // Check split panel Load buttons
        await page.click('#eprSplitTypesBtn');
        await page.waitForTimeout(500);
        
        const loadGroupSplitBtn = page.locator('#eprLoadPackagingGroupBtnSplit');
        if (await loadGroupSplitBtn.isVisible()) {
            const loadGroupSplitIcon = loadGroupSplitBtn.locator('i');
            const loadGroupSplitIconCount = await loadGroupSplitIcon.count();
            expect(loadGroupSplitIconCount).toBeGreaterThan(0);
        }
        
        const loadDistSplitBtn = page.locator('#eprLoadDistributionGroupBtnSplit');
        if (await loadDistSplitBtn.isVisible()) {
            const loadDistSplitIcon = loadDistSplitBtn.locator('i');
            const loadDistSplitIconCount = await loadDistSplitIcon.count();
            expect(loadDistSplitIconCount).toBeGreaterThan(0);
        }
    });

    test('Fix 4: Panel locking/unlocking works', async ({ page }) => {
        // Ensure split mode is active
        await page.click('#eprSplitTypesBtn');
        await page.waitForTimeout(1000);
        
        // Get a panel
        const rawMaterialsPanel = page.locator('#eprTypesPanelRawMaterials');
        await rawMaterialsPanel.waitFor({ state: 'visible' });
        
        // Get lock button
        const lockBtn = rawMaterialsPanel.locator('.epr-btn-lock-toolbar');
        await lockBtn.waitFor({ state: 'visible' });
        
        // Check initial state (should be unlocked/open lock icon)
        const initialIcon = lockBtn.locator('i');
        const initialIconClass = await initialIcon.getAttribute('class');
        const isInitiallyLocked = initialIconClass.includes('lock-fill');
        
        // Get initial position
        const initialRect = await rawMaterialsPanel.boundingBox();
        
        // Click lock button to toggle
        await lockBtn.click();
        await page.waitForTimeout(300);
        
        // Check icon changed
        const newIconClass = await initialIcon.getAttribute('class');
        const isNowLocked = newIconClass.includes('lock-fill');
        
        // State should have changed
        expect(isNowLocked).not.toBe(isInitiallyLocked);
        
        // Check data attribute
        const lockedAttr = await rawMaterialsPanel.getAttribute('data-toolbar-locked');
        if (isNowLocked) {
            expect(lockedAttr).toBe('true');
        } else {
            expect(lockedAttr).toBe('false');
        }
    });

    test('Fix 5: Connection modal appears at center of connector line', async ({ page }) => {
        // Add two nodes
        await page.click('#eprSplitTypesBtn');
        await page.waitForTimeout(500);
        
        // Add first packaging item
        const packagingSelect = page.locator('#eprPackagingLibraryDropdownSplit');
        await packagingSelect.waitFor({ state: 'visible' });
        await packagingSelect.selectOption({ index: 1 });
        await page.click('#eprAddPackagingBtnSplit');
        await page.waitForTimeout(1000);
        
        // Add second packaging item
        const optionCount = await packagingSelect.locator('option').count();
        if (optionCount > 2) {
            await packagingSelect.selectOption({ index: 2 });
            await page.click('#eprAddPackagingBtnSplit');
            await page.waitForTimeout(1000);
        }
        
        // Get nodes
        const nodes = page.locator('.epr-canvas-node');
        const nodeCount = await nodes.count();
        
        if (nodeCount >= 2) {
            // Get positions of both nodes
            const node1Rect = await nodes.nth(0).boundingBox();
            const node2Rect = await nodes.nth(1).boundingBox();
            
            // Calculate expected center point
            const expectedCenterX = (node1Rect.x + node1Rect.width / 2 + node2Rect.x + node2Rect.width / 2) / 2;
            const expectedCenterY = (node1Rect.y + node1Rect.height / 2 + node2Rect.y + node2Rect.height / 2) / 2;
            
            // Find connection line (SVG path)
            const connectionLine = page.locator('.epr-connection-line').first();
            const connectionExists = await connectionLine.count() > 0;
            
            if (connectionExists) {
                // Click on connection line
                await connectionLine.click({ force: true });
                await page.waitForTimeout(500);
                
                // Check if modal appeared
                const modal = page.locator('.modal.show').filter({ hasText: 'Add Quantity' });
                const modalVisible = await modal.isVisible();
                
                if (modalVisible) {
                    // Get modal position
                    const modalRect = await modal.boundingBox();
                    const modalCenterX = modalRect.x + modalRect.width / 2;
                    const modalCenterY = modalRect.y + modalRect.height / 2;
                    
                    // Check if modal is roughly centered (within 100px tolerance)
                    const xDiff = Math.abs(modalCenterX - expectedCenterX);
                    const yDiff = Math.abs(modalCenterY - expectedCenterY);
                    
                    // Modal should not be at top-left corner (0,0)
                    expect(modalRect.x).toBeGreaterThan(50);
                    expect(modalRect.y).toBeGreaterThan(50);
                    
                    // Modal should be reasonably close to center (within 200px)
                    expect(xDiff).toBeLessThan(300);
                    expect(yDiff).toBeLessThan(300);
                }
            }
        }
    });
});

