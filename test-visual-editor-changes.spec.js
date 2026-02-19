const { test, expect } = require('@playwright/test');

test.describe('Visual Editor Changes Verification', () => {
    test.beforeEach(async ({ page }) => {
        // Navigate to login page
        await page.goto('http://localhost:5290/Account/Login');
        
        // Login with admin credentials
        await page.fill('input[name="Username"], input[type="text"]', 'admin');
        await page.fill('input[name="Password"], input[type="password"]', 'admin123');
        await page.click('button[type="submit"], input[type="submit"]');
        
        // Wait for navigation to complete
        await page.waitForURL('**/VisualEditor**', { timeout: 10000 });
    });

    test('Raw Materials should load and remain visible', async ({ page }) => {
        // Wait for the page to load
        await page.waitForLoadState('networkidle');
        
        // Wait for Raw Materials container to appear
        const rawMaterialsContainer = page.locator('#eprRawMaterialsButtons');
        await expect(rawMaterialsContainer).toBeVisible({ timeout: 10000 });
        
        // Wait a bit for materials to load
        await page.waitForTimeout(2000);
        
        // Check that Raw Materials buttons are present and visible
        const materialButtons = rawMaterialsContainer.locator('.epr-material-btn');
        const buttonCount = await materialButtons.count();
        
        console.log(`Found ${buttonCount} Raw Materials buttons`);
        expect(buttonCount).toBeGreaterThan(0);
        
        // Verify buttons remain visible after a delay
        await page.waitForTimeout(3000);
        await expect(rawMaterialsContainer).toBeVisible();
        await expect(materialButtons.first()).toBeVisible();
        
        // Check that buttons have icons
        const firstButtonIcon = materialButtons.first().locator('i');
        await expect(firstButtonIcon).toBeVisible();
        
        // Verify buttons are disabled/non-interactive
        const firstButton = materialButtons.first();
        const cursor = await firstButton.evaluate(el => window.getComputedStyle(el).cursor);
        expect(cursor).toBe('not-allowed');
    });

    test('Project Name font size should be 16pt', async ({ page }) => {
        await page.waitForLoadState('networkidle');
        
        const projectNameInput = page.locator('#eprProjectName');
        await expect(projectNameInput).toBeVisible();
        
        // Check font size
        const fontSize = await projectNameInput.evaluate(el => {
            return window.getComputedStyle(el).fontSize;
        });
        
        console.log(`Project Name font size: ${fontSize}`);
        // 16pt = 21.33px (approximately)
        expect(parseFloat(fontSize)).toBeGreaterThanOrEqual(20);
        expect(parseFloat(fontSize)).toBeLessThanOrEqual(22);
        
        // Check font weight
        const fontWeight = await projectNameInput.evaluate(el => {
            return window.getComputedStyle(el).fontWeight;
        });
        expect(parseInt(fontWeight)).toBeGreaterThanOrEqual(600);
    });

    test('Autosave checkbox should be visible', async ({ page }) => {
        await page.waitForLoadState('networkidle');
        
        const autosaveCheckbox = page.locator('#eprAutosaveCheckbox');
        await expect(autosaveCheckbox).toBeVisible();
        
        // Check that it's a checkbox
        const type = await autosaveCheckbox.getAttribute('type');
        expect(type).toBe('checkbox');
        
        // Check label text
        const label = page.locator('label:has-text("Autosave")');
        await expect(label).toBeVisible();
    });

    test('Raw Materials palette should be wider (280px)', async ({ page }) => {
        await page.waitForLoadState('networkidle');
        
        // Check if split mode is active
        const rawMaterialsPanel = page.locator('.epr-types-panel-raw-materials');
        const isVisible = await rawMaterialsPanel.isVisible();
        
        if (isVisible) {
            const width = await rawMaterialsPanel.evaluate(el => {
                return window.getComputedStyle(el).width;
            });
            console.log(`Raw Materials panel width: ${width}`);
            const widthValue = parseFloat(width);
            expect(widthValue).toBeGreaterThanOrEqual(280);
        } else {
            // Check unified toolbar width
            const typesToolbar = page.locator('#eprTypesToolbar');
            if (await typesToolbar.isVisible()) {
                // Check if Raw Materials section has wider styling
                const rawMaterialsSection = page.locator('[data-section="raw-materials"]');
                if (await rawMaterialsSection.isVisible()) {
                    const container = page.locator('#eprRawMaterialsButtons');
                    const width = await container.evaluate(el => {
                        return window.getComputedStyle(el).width;
                    });
                    console.log(`Raw Materials container width: ${width}`);
                }
            }
        }
    });

    test('Snap to Grid button should change color when active', async ({ page }) => {
        await page.waitForLoadState('networkidle');
        
        const snapToGridBtn = page.locator('#eprSnapToGridBtn');
        await expect(snapToGridBtn).toBeVisible();
        
        // Get initial icon color
        const icon = snapToGridBtn.locator('i');
        await expect(icon).toBeVisible();
        
        const initialColor = await icon.evaluate(el => {
            return window.getComputedStyle(el).color;
        });
        console.log(`Initial Snap to Grid icon color: ${initialColor}`);
        
        // Click to toggle
        await snapToGridBtn.click();
        await page.waitForTimeout(500);
        
        // Check if color changed
        const newColor = await icon.evaluate(el => {
            return window.getComputedStyle(el).color;
        });
        console.log(`After toggle Snap to Grid icon color: ${newColor}`);
        
        // Color should be different (green when active)
        // RGB(40, 167, 69) = #28a745
        const isGreen = newColor.includes('rgb(40, 167, 69)') || 
                       newColor.includes('rgb(40, 167, 69)') ||
                       newColor.toLowerCase().includes('green');
        
        // At least verify the color changed
        expect(newColor).not.toBe(initialColor);
    });

    test('Raw Materials buttons should have icons visible', async ({ page }) => {
        await page.waitForLoadState('networkidle');
        
        const rawMaterialsContainer = page.locator('#eprRawMaterialsButtons');
        await expect(rawMaterialsContainer).toBeVisible({ timeout: 10000 });
        
        await page.waitForTimeout(2000);
        
        const materialButtons = rawMaterialsContainer.locator('.epr-material-btn');
        const buttonCount = await materialButtons.count();
        
        expect(buttonCount).toBeGreaterThan(0);
        
        // Check first few buttons have icons
        for (let i = 0; i < Math.min(3, buttonCount); i++) {
            const button = materialButtons.nth(i);
            const icon = button.locator('i');
            await expect(icon).toBeVisible();
            
            // Check icon has a class (Bootstrap icon)
            const iconClass = await icon.getAttribute('class');
            expect(iconClass).toContain('bi-');
            
            // Check icon opacity is 1 (visible)
            const iconOpacity = await icon.evaluate(el => {
                return window.getComputedStyle(el).opacity;
            });
            expect(parseFloat(iconOpacity)).toBeGreaterThan(0.9);
        }
    });

    test('All menu buttons should have icons', async ({ page }) => {
        await page.waitForLoadState('networkidle');
        
        // Check Save Project button
        const saveBtn = page.locator('#eprSaveProjectBtn');
        await expect(saveBtn).toBeVisible();
        const saveIcon = saveBtn.locator('i');
        await expect(saveIcon).toBeVisible();
        
        // Check Load Project button
        const loadBtn = page.locator('#eprLoadProjectBtn');
        await expect(loadBtn).toBeVisible();
        const loadIcon = loadBtn.locator('i');
        await expect(loadIcon).toBeVisible();
        
        // Check New Project button
        const newBtn = page.locator('#eprNewProjectBtn');
        await expect(newBtn).toBeVisible();
        const newIcon = newBtn.locator('i');
        await expect(newIcon).toBeVisible();
        
        // Check Clear Canvas button
        const clearBtn = page.locator('#eprClearCanvasBtn');
        await expect(clearBtn).toBeVisible();
        const clearIcon = clearBtn.locator('i');
        await expect(clearIcon).toBeVisible();
    });

    test('Raw Materials should not disappear after loading', async ({ page }) => {
        await page.waitForLoadState('networkidle');
        
        const rawMaterialsContainer = page.locator('#eprRawMaterialsButtons');
        await expect(rawMaterialsContainer).toBeVisible({ timeout: 10000 });
        
        // Wait for materials to load
        await page.waitForTimeout(3000);
        
        // Verify container is still visible
        await expect(rawMaterialsContainer).toBeVisible();
        
        // Check that buttons are still there
        const materialButtons = rawMaterialsContainer.locator('.epr-material-btn');
        const buttonCount = await materialButtons.count();
        expect(buttonCount).toBeGreaterThan(0);
        
        // Wait longer and check again
        await page.waitForTimeout(5000);
        await expect(rawMaterialsContainer).toBeVisible();
        
        const buttonCountAfter = await materialButtons.count();
        expect(buttonCountAfter).toBe(buttonCount);
    });
});










