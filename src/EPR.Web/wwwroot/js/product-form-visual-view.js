// Product Form Visual View - Based on Visual Editor
// This is a copy/adaptation of the Visual Editor for use in the Product Form page
(function() {
    'use strict';

    let visualEditorInstance = null;
    let isSyncing = false; // Prevent sync loops

    function onVisualViewShown() {
        console.log('[Product Visual View] Visual View shown');
        setTimeout(() => {
            const visualViewPane = document.getElementById('visualView');
            if (visualViewPane && visualViewPane.style.display !== 'none') {
                if (!visualEditorInstance) {
                    console.log('[Product Visual View] Initializing Visual Editor...');
                    initializeVisualView();
                } else {
                    console.log('[Product Visual View] Visual Editor already initialized, syncing...');
                    syncFormToVisual();
                }
            }
        }, 300);
    }

    function onFormViewShown() {
        if (visualEditorInstance) {
            syncVisualToForm();
        }
    }

    document.addEventListener('DOMContentLoaded', function() {
        document.addEventListener('visualViewShown', onVisualViewShown);

        // Setup GS1 Import button
        const btnImportGS1 = document.getElementById('btnImportGS1');
        if (btnImportGS1) {
            btnImportGS1.addEventListener('click', handleGS1Import);
        }

        // If Visual View is already visible on load (e.g. hash), init
        const visualViewPane = document.getElementById('visualView');
        if (visualViewPane && visualViewPane.style.display === 'block') {
            document.body.classList.add('visual-view-active');
            setTimeout(() => {
                if (!visualEditorInstance) {
                    initializeVisualView();
                }
            }, 500);
        }
    });

    function initializeVisualView() {
        console.log('[Product Visual View] Initializing...');
        
        // Check if Visual Editor is available
        let initRetryCount = 0;
        const MAX_RETRIES = 100; // Increased retries
        
        function tryInit() {
            if (initRetryCount >= MAX_RETRIES) {
                console.error('[Product Visual View] Max retries reached. Visual Editor may not be loaded.');
                console.error('[Product Visual View] EPRVisualEditorApp available:', typeof window.EPRVisualEditorApp !== 'undefined');
                const editor = document.getElementById('epr-visual-editor');
                const canvas = document.getElementById('eprCanvas');
                const nodesLayer = document.getElementById('eprNodesLayer');
                const connectionsLayer = document.getElementById('eprConnectionsLayer');
                console.error('[Product Visual View] Elements found:', {
                    editor: !!editor,
                    canvas: !!canvas,
                    nodesLayer: !!nodesLayer,
                    connectionsLayer: !!connectionsLayer
                });
                return;
            }

            if (typeof window.EPRVisualEditorApp === 'undefined') {
                console.log('[Product Visual View] Waiting for EPRVisualEditorApp...', initRetryCount);
                initRetryCount++;
                setTimeout(tryInit, 200);
                return;
            }

            // Check if Visual View is visible (display block or has active/show class)
            const visualViewPane = document.getElementById('visualView');
            const isVisible = visualViewPane && (
                visualViewPane.style.display === 'block' ||
                window.getComputedStyle(visualViewPane).display === 'block' ||
                visualViewPane.classList.contains('active') ||
                visualViewPane.classList.contains('show')
            );
            if (!visualViewPane || !isVisible) {
                console.log('[Product Visual View] Visual View not visible, waiting...', {
                    hasPane: !!visualViewPane,
                    display: visualViewPane?.style?.display,
                    computedDisplay: visualViewPane ? window.getComputedStyle(visualViewPane).display : null
                });
                initRetryCount++;
                setTimeout(tryInit, 200);
                return;
            }
            
            const editor = document.getElementById('epr-visual-editor');
            const canvas = document.getElementById('eprCanvas');
            const nodesLayer = document.getElementById('eprNodesLayer');
            const connectionsLayer = document.getElementById('eprConnectionsLayer');
            
            if (!editor || !canvas || !nodesLayer || !connectionsLayer) {
                console.warn('[Product Visual View] Elements not found, retrying...', {
                    editor: !!editor,
                    canvas: !!canvas,
                    nodesLayer: !!nodesLayer,
                    connectionsLayer: !!connectionsLayer,
                    visualViewActive: visualViewPane?.classList.contains('active')
                });
                initRetryCount++;
                setTimeout(tryInit, 200);
                return;
            }
            
            // Check if elements are visible (not hidden by tab)
            // For tabs, elements might have 0 dimensions even when active, so be more lenient
            const editorRect = editor.getBoundingClientRect();
            const computedStyle = window.getComputedStyle(editor);
            const isHidden = computedStyle.display === 'none' || computedStyle.visibility === 'hidden';
            
            if (isHidden && (editorRect.width === 0 || editorRect.height === 0)) {
                console.warn('[Product Visual View] Editor element appears hidden, retrying...', {
                    display: computedStyle.display,
                    visibility: computedStyle.visibility,
                    width: editorRect.width,
                    height: editorRect.height
                });
                initRetryCount++;
                setTimeout(tryInit, 200);
                return;
            }
            
            // If tab is active but element still has 0 dimensions, wait a bit more for CSS transitions
            if (editorRect.width === 0 || editorRect.height === 0) {
                console.warn('[Product Visual View] Editor element has 0 dimensions, waiting for CSS transition...', {
                    width: editorRect.width,
                    height: editorRect.height,
                    retryCount: initRetryCount
                });
                if (initRetryCount < 10) { // Allow a few retries for CSS transitions
                    initRetryCount++;
                    setTimeout(tryInit, 300);
                    return;
                }
                // After 10 retries, proceed anyway - the Visual Editor might handle it
                console.warn('[Product Visual View] Proceeding despite 0 dimensions - Visual Editor may handle it');
            }
            
            console.log('[Product Visual View] All required elements found and visible, creating Visual Editor...');

            try {
                console.log('[Product Visual View] Creating EPRVisualEditorApp instance...');
                console.log('[Product Visual View] Elements before creation:', {
                    editor: !!editor,
                    canvas: !!canvas,
                    nodesLayer: !!nodesLayer,
                    connectionsLayer: !!connectionsLayer,
                    editorDisplay: window.getComputedStyle(editor).display,
                    editorVisibility: window.getComputedStyle(editor).visibility,
                    editorRect: editor.getBoundingClientRect()
                });
                
                // Verify EPRVisualEditorApp is available
                if (typeof window.EPRVisualEditorApp === 'undefined') {
                    throw new Error('EPRVisualEditorApp is not defined. visual-editor-complete.js may not be loaded.');
                }
                
                visualEditorInstance = new window.EPRVisualEditorApp();
                console.log('[Product Visual View] Visual Editor created successfully');
                console.log('[Product Visual View] Instance:', visualEditorInstance);
                
                if (!visualEditorInstance) {
                    throw new Error('EPRVisualEditorApp constructor returned null/undefined');
                }
                
                // Visual Editor initializes automatically in constructor
                // Wait for all components to be ready
                setTimeout(() => {
                    if (visualEditorInstance && visualEditorInstance.canvasManager) {
                        console.log('[Product Visual View] Visual Editor initialized');
                        console.log('[Product Visual View] Canvas Manager:', visualEditorInstance.canvasManager);
                        console.log('[Product Visual View] Actions Toolbar:', visualEditorInstance.actionsToolbar);
                        console.log('[Product Visual View] Types Toolbar:', visualEditorInstance.typesToolbar);
                        
                        // Setup toolbar dragging and event listeners
                        const setupToolbarDragging = () => {
                            if (visualEditorInstance && visualEditorInstance.actionsToolbar) {
                                try {
                                    // Setup event listeners first (includes minimize button handlers)
                                    if (visualEditorInstance.actionsToolbar.setupEventListeners) {
                                        visualEditorInstance.actionsToolbar.setupEventListeners();
                                        console.log('[Product Visual View] Toolbar event listeners setup complete');
                                    }
                                    
                                    // Then setup dragging
                                    if (visualEditorInstance.actionsToolbar.setupToolbarDragging) {
                                        visualEditorInstance.actionsToolbar.setupToolbarDragging();
                                        console.log('[Product Visual View] Actions toolbar dragging setup complete');
                                    }
                                    
                                    // Setup Types toolbar dragging if available
                                    if (visualEditorInstance.typesToolbar && visualEditorInstance.typesToolbar.setupToolbarDragging) {
                                        visualEditorInstance.typesToolbar.setupToolbarDragging();
                                        console.log('[Product Visual View] Types toolbar dragging setup complete');
                                    }
                                    
                                    // Manually ensure ALL toolbars in product-form-visual-editor are set up
                                    const visualEditor = document.getElementById('epr-visual-editor');
                                    if (visualEditor) {
                                        const toolbars = visualEditor.querySelectorAll('.epr-floating-toolbar, .epr-types-panel, .epr-types-toolbar, [class*="epr-types"], [class*="epr-actions"]');
                                        console.log('[Product Visual View] Found toolbars in visual editor:', toolbars.length);
                                        toolbars.forEach(toolbar => {
                                            const header = toolbar.querySelector('.epr-toolbar-header');
                                            if (header) {
                                                // Ensure header has proper cursor and user-select
                                                header.style.cursor = 'move';
                                                header.style.userSelect = 'none';
                                                header.style.webkitUserSelect = 'none';
                                                header.style.mozUserSelect = 'none';
                                                header.style.msUserSelect = 'none';
                                                
                                                // Ensure toolbar is positioned absolutely
                                                if (toolbar.style.position !== 'absolute' && toolbar.style.position !== 'fixed') {
                                                    toolbar.style.position = 'absolute';
                                                }
                                                
                                                // Re-attach minimize button handler if needed
                                                const minimizeBtn = header.querySelector('.epr-btn-close-toolbar, .bi-dash, .bi-plus-lg');
                                                if (minimizeBtn) {
                                                    // Clone to remove old listeners
                                                    const newMinimizeBtn = minimizeBtn.cloneNode(true);
                                                    minimizeBtn.parentNode.replaceChild(newMinimizeBtn, minimizeBtn);
                                                    newMinimizeBtn.addEventListener('click', (e) => {
                                                        e.stopPropagation();
                                                        e.preventDefault();
                                                        const wasMinimized = toolbar.classList.contains('minimized');
                                                        toolbar.classList.toggle('minimized');
                                                        const icon = newMinimizeBtn.querySelector('i');
                                                        if (icon) {
                                                            if (toolbar.classList.contains('minimized')) {
                                                                icon.className = 'bi bi-plus-lg';
                                                                newMinimizeBtn.title = 'Expand';
                                                            } else {
                                                                icon.className = 'bi bi-dash';
                                                                newMinimizeBtn.title = 'Minimize';
                                                            }
                                                        }
                                                    });
                                                }
                                                
                                                console.log('[Product Visual View] Toolbar header configured for:', toolbar.id || toolbar.className);
                                            } else {
                                                console.warn('[Product Visual View] No header found for toolbar:', toolbar.id || toolbar.className);
                                            }
                                        });
                                    }
                                } catch (err) {
                                    console.error('[Product Visual View] Error setting up toolbar dragging:', err);
                                    console.error('[Product Visual View] Error stack:', err.stack);
                                }
                            } else {
                                console.warn('[Product Visual View] Actions toolbar not available yet');
                            }
                        };
                        
                        // Setup immediately
                        setupToolbarDragging();
                        
                        // Retry setup after delays to catch any toolbars that weren't ready
                        setTimeout(setupToolbarDragging, 500);
                        setTimeout(setupToolbarDragging, 1000);
                        setTimeout(setupToolbarDragging, 2000);
                        setTimeout(setupToolbarDragging, 3000);
                        
                        // Setup custom parameters panel for product form
                        try {
                            setupProductFormParametersPanel();
                        } catch (err) {
                            console.error('[Product Visual View] Error setting up parameters panel:', err);
                        }
                        
                        // Setup sync handlers
                        try {
                            setupSyncHandlers();
                        } catch (err) {
                            console.error('[Product Visual View] Error setting up sync handlers:', err);
                        }
                        
                        // Load form data into visual view (will create nodes if form has data)
                        try {
                            syncFormToVisual();
                            console.log('[Product Visual View] Form data synced to visual view');
                        } catch (err) {
                            console.error('[Product Visual View] Error syncing form to visual:', err);
                            console.error('[Product Visual View] Error stack:', err.stack);
                        }
                    } else {
                        console.warn('[Product Visual View] Visual Editor not fully initialized, retrying...');
                        console.warn('[Product Visual View] visualEditorInstance:', visualEditorInstance);
                        console.warn('[Product Visual View] canvasManager:', visualEditorInstance?.canvasManager);
                        if (initRetryCount < MAX_RETRIES) {
                            initRetryCount++;
                            setTimeout(tryInit, 500);
                        }
                    }
                }, 2000); // Increased delay to ensure everything is ready
            } catch (error) {
                console.error('[Product Visual View] Error creating Visual Editor:', error);
                console.error('[Product Visual View] Error name:', error.name);
                console.error('[Product Visual View] Error message:', error.message);
                console.error('[Product Visual View] Error stack:', error.stack);
                
                // If it's a "Canvas elements not found" error, retry
                if (error.message && error.message.includes('Canvas elements not found')) {
                    console.warn('[Product Visual View] Canvas elements not found, retrying...');
                    initRetryCount++;
                    if (initRetryCount < MAX_RETRIES) {
                        setTimeout(tryInit, 500);
                    } else {
                        console.error('[Product Visual View] Max retries reached after canvas elements error');
                        // Show user-friendly error message
                        const visualViewPane = document.getElementById('visualView');
                        if (visualViewPane) {
                            visualViewPane.innerHTML = `
                                <div class="alert alert-danger m-4" role="alert">
                                    <h4 class="alert-heading">Visual Editor Initialization Failed</h4>
                                    <p>Unable to find required canvas elements. Please refresh the page and try again.</p>
                                    <hr>
                                    <p class="mb-0"><small>Error: ${error.message}</small></p>
                                </div>
                            `;
                        }
                    }
                } else {
                    // For other errors, still retry but log more details
                    console.error('[Product Visual View] Unexpected error, retrying...');
                    initRetryCount++;
                    if (initRetryCount < MAX_RETRIES) {
                        setTimeout(tryInit, 1000);
                    } else {
                        console.error('[Product Visual View] Max retries reached after unexpected error');
                        // Show user-friendly error message
                        const visualViewPane = document.getElementById('visualView');
                        if (visualViewPane) {
                            visualViewPane.innerHTML = `
                                <div class="alert alert-danger m-4" role="alert">
                                    <h4 class="alert-heading">Visual Editor Initialization Failed</h4>
                                    <p>Unable to initialize the Visual Editor. Please refresh the page and try again.</p>
                                    <hr>
                                    <p class="mb-0"><small>Error: ${error.message || 'Unknown error'}</small></p>
                                </div>
                            `;
                        }
                    }
                }
            }
        }
        
        tryInit();
    }

    function setupProductFormParametersPanel() {
        if (!visualEditorInstance || !visualEditorInstance.canvasManager) return;

        // Override the parameters panel to show form data
        const canvasManager = visualEditorInstance.canvasManager;
        const parametersPanel = visualEditorInstance.parametersPanel;
        
        if (parametersPanel) {
            // Override the loadNodeParameters method to include form data
            const originalLoadNodeParameters = parametersPanel.loadNodeParameters.bind(parametersPanel);
            
            parametersPanel.loadNodeParameters = function(node) {
                // Call original to show standard parameters
                originalLoadNodeParameters(node);
                
                // Then enhance with form-specific data
                if (node) {
                    enhanceParametersWithFormData(node);
                }
            };
        }

        // Also hook into node selection to sync form data
        const originalSelectNode = canvasManager.selectNode.bind(canvasManager);
        
        canvasManager.selectNode = function(nodeId) {
            originalSelectNode(nodeId);
            const node = this.nodes.get(nodeId);
            if (node && parametersPanel) {
                // Parameters panel will be updated by the override above
                enhanceParametersWithFormData(node);
            }
        };
    }

    function enhanceParametersWithFormData(node) {
        // Enhance the standard parameters panel with form data
        const paramsContent = document.getElementById('eprParametersContent');
        if (!paramsContent) return;

        // Get form data, but prioritize node data
        const nodeName = node.name || '';
        const formProductName = document.getElementById('productName')?.value || '';
        const productName = nodeName || formProductName; // Use node name first, fallback to form
        const gtin = node.parameters?.gtin || document.getElementById('gtin')?.value || '';
        const brand = node.parameters?.brand || document.getElementById('brand')?.value || '';
        const productWeight = node.parameters?.productWeight || document.getElementById('productWeight')?.value || '';
        const productCategory = node.parameters?.productCategory || document.getElementById('productCategory')?.value || '';

        // Add form-specific fields to the parameters panel
        let formDataHtml = '';
        
        if (node.type === 'product') {
            formDataHtml = `
                <div class="mt-3 pt-3 border-top form-data-section">
                    <h6 class="small text-muted mb-2">Form Data</h6>
                    <div class="mb-2">
                        <label class="form-label small">Product Name</label>
                        <input type="text" class="form-control form-control-sm epr-parameter-input" 
                               id="paramProductName" value="${escapeHtml(productName)}" 
                               data-param-key="productName" data-form-field="productName">
                    </div>
                    <div class="mb-2">
                        <label class="form-label small">GTIN</label>
                        <input type="text" class="form-control form-control-sm epr-parameter-input" 
                               id="paramGtin" value="${escapeHtml(gtin)}" 
                               data-param-key="gtin" data-form-field="gtin">
                    </div>
                    <div class="mb-2">
                        <label class="form-label small">Brand</label>
                        <input type="text" class="form-control form-control-sm epr-parameter-input" 
                               id="paramBrand" value="${escapeHtml(brand)}" 
                               data-param-key="brand" data-form-field="brand">
                    </div>
                    <div class="mb-2">
                        <label class="form-label small">Product Weight</label>
                        <input type="number" class="form-control form-control-sm epr-parameter-input" 
                               id="paramProductWeight" value="${escapeHtml(productWeight)}" 
                               data-param-key="productWeight" data-form-field="productWeight">
                    </div>
                    <div class="mb-2">
                        <label class="form-label small">Product Category</label>
                        <select class="form-select form-select-sm epr-parameter-input" 
                                id="paramProductCategory" data-param-key="productCategory" data-form-field="productCategory">
                            <option value="">Select category...</option>
                            <option value="Berries" ${productCategory === 'Berries' ? 'selected' : ''}>Berries</option>
                            <option value="Citrus" ${productCategory === 'Citrus' ? 'selected' : ''}>Citrus</option>
                            <option value="Leafy Greens" ${productCategory === 'Leafy Greens' ? 'selected' : ''}>Leafy Greens</option>
                            <option value="Root Vegetables" ${productCategory === 'Root Vegetables' ? 'selected' : ''}>Root Vegetables</option>
                            <option value="Stone Fruit" ${productCategory === 'Stone Fruit' ? 'selected' : ''}>Stone Fruit</option>
                            <option value="Tropical Fruit" ${productCategory === 'Tropical Fruit' ? 'selected' : ''}>Tropical Fruit</option>
                            <option value="Other" ${productCategory === 'Other' ? 'selected' : ''}>Other</option>
                        </select>
                    </div>
                </div>
            `;
        } else if (node.type === 'packaging') {
            const components = getPackagingComponents();
            const component = components.find(c => c.materialType === node.name) || components[0] || {};
            
            formDataHtml = `
                <div class="mt-3 pt-3 border-top">
                    <h6 class="small text-muted mb-2">Form Data</h6>
                    <div class="mb-2">
                        <label class="form-label small">Material Type</label>
                        <input type="text" class="form-control form-control-sm" 
                               value="${escapeHtml(component.materialType || node.name || '')}" readonly>
                    </div>
                    <div class="mb-2">
                        <label class="form-label small">Weight (g)</label>
                        <input type="number" class="form-control form-control-sm epr-parameter-input" 
                               id="paramPackagingWeight" value="${component.weight || ''}" step="0.01"
                               data-param-key="weight">
                    </div>
                </div>
            `;
        }

        // Append form data to parameters content
        if (formDataHtml) {
            const existingFormData = paramsContent.querySelector('.form-data-section');
            if (existingFormData) {
                existingFormData.remove();
            }
            paramsContent.insertAdjacentHTML('beforeend', formDataHtml.replace('mt-3 pt-3 border-top', 'form-data-section mt-3 pt-3 border-top'));
            
            // Setup event listeners
            setupParameterChangeListeners(node);
        }
    }

    function getProductFormFields(node) {
        const productName = document.getElementById('productName')?.value || node.name || '';
        const gtin = document.getElementById('gtin')?.value || '';
        const brand = document.getElementById('brand')?.value || '';
        const productWeight = document.getElementById('productWeight')?.value || '';
        const productCategory = document.getElementById('productCategory')?.value || '';
        
        return `
            <div class="mb-3">
                <label class="form-label">Product Name</label>
                <input type="text" class="form-control" id="paramProductName" value="${escapeHtml(productName)}" data-form-field="productName">
            </div>
            <div class="mb-3">
                <label class="form-label">GTIN</label>
                <input type="text" class="form-control" id="paramGtin" value="${escapeHtml(gtin)}" data-form-field="gtin">
            </div>
            <div class="mb-3">
                <label class="form-label">Brand</label>
                <input type="text" class="form-control" id="paramBrand" value="${escapeHtml(brand)}" data-form-field="brand">
            </div>
            <div class="mb-3">
                <label class="form-label">Product Weight</label>
                <input type="number" class="form-control" id="paramProductWeight" value="${escapeHtml(productWeight)}" data-form-field="productWeight">
            </div>
            <div class="mb-3">
                <label class="form-label">Product Category</label>
                <select class="form-select" id="paramProductCategory" data-form-field="productCategory">
                    <option value="">Select category...</option>
                    <option value="Berries" ${productCategory === 'Berries' ? 'selected' : ''}>Berries</option>
                    <option value="Citrus" ${productCategory === 'Citrus' ? 'selected' : ''}>Citrus</option>
                    <option value="Leafy Greens" ${productCategory === 'Leafy Greens' ? 'selected' : ''}>Leafy Greens</option>
                    <option value="Root Vegetables" ${productCategory === 'Root Vegetables' ? 'selected' : ''}>Root Vegetables</option>
                    <option value="Stone Fruit" ${productCategory === 'Stone Fruit' ? 'selected' : ''}>Stone Fruit</option>
                    <option value="Tropical Fruit" ${productCategory === 'Tropical Fruit' ? 'selected' : ''}>Tropical Fruit</option>
                    <option value="Other" ${productCategory === 'Other' ? 'selected' : ''}>Other</option>
                </select>
            </div>
        `;
    }

    function getPackagingFormFields(node) {
        // Get packaging data from form components
        const components = getPackagingComponents();
        const component = components.find(c => c.materialType === node.name) || components[0] || {};
        
        return `
            <div class="mb-3">
                <label class="form-label">Material Type</label>
                <input type="text" class="form-control" id="paramMaterialType" value="${escapeHtml(component.materialType || node.name || '')}" readonly>
            </div>
            <div class="mb-3">
                <label class="form-label">Weight (g)</label>
                <input type="number" class="form-control" id="paramPackagingWeight" value="${component.weight || ''}" step="0.01">
            </div>
        `;
    }

    function getRawMaterialFormFields(node) {
        return `
            <div class="mb-3">
                <label class="form-label">Material Name</label>
                <input type="text" class="form-control" id="paramRawMaterialName" value="${escapeHtml(node.name || '')}">
            </div>
        `;
    }

    function setupParameterChangeListeners(node) {
        // Remove old listeners to prevent duplicates
        const inputs = document.querySelectorAll('#eprParametersContent .form-data-section input, #eprParametersContent .form-data-section select');
        inputs.forEach(input => {
            // Clone node to remove all event listeners
            const newInput = input.cloneNode(true);
            input.parentNode.replaceChild(newInput, input);
        });
        
        // Re-attach listeners to new elements
        const newInputs = document.querySelectorAll('#eprParametersContent .form-data-section input, #eprParametersContent .form-data-section select');
        newInputs.forEach(input => {
            input.addEventListener('change', function() {
                if (isSyncing) return; // Prevent sync loops
                
                isSyncing = true;
                
                const formField = this.dataset.formField;
                const paramKey = this.dataset.paramKey || this.id.replace('param', '').toLowerCase();
                
                // Update form field
                if (formField) {
                    const formInput = document.getElementById(formField);
                    if (formInput) {
                        formInput.value = this.value;
                        formInput.dispatchEvent(new Event('change', { bubbles: true }));
                    }
                }
                
                // Update node in visual editor
                if (visualEditorInstance && visualEditorInstance.canvasManager && node) {
                    const canvasManager = visualEditorInstance.canvasManager;
                    const nodeId = node.id;
                    const updatedNode = canvasManager.nodes.get(nodeId);
                    if (updatedNode) {
                        // Special handling for productName - update node.name
                        if (paramKey === 'productname' || paramKey === 'productName' || formField === 'productName') {
                            updatedNode.name = this.value || 'Product';
                            // Re-render the node to show new name
                            canvasManager.renderNode(updatedNode);
                        }
                        
                        // Special handling for packaging weight - update description and form
                        if (paramKey === 'weight' && updatedNode.type === 'packaging') {
                            updatedNode.description = `${this.value}g`;
                            // Update form component
                            const materialType = this.dataset.materialType || updatedNode.name;
                            if (window.packagingComponents && materialType) {
                                const comp = window.packagingComponents.find(c => c.materialType === materialType);
                                if (comp) {
                                    comp.weight = parseFloat(this.value) || 0;
                                    window.packagingComponents = window.packagingComponents; // Trigger update
                                    if (window.updatePackagingComponentsList) {
                                        window.updatePackagingComponentsList();
                                    }
                                    if (window.updateTotalPackagingWeight) {
                                        window.updateTotalPackagingWeight();
                                    }
                                }
                            }
                        }
                        
                        // Store in parameters
                        if (!updatedNode.parameters) {
                            updatedNode.parameters = {};
                        }
                        updatedNode.parameters[paramKey] = this.value;
                        
                        // Update node
                        canvasManager.updateNode(nodeId, updatedNode);
                    }
                }
                
                isSyncing = false;
            });
            
            input.addEventListener('input', function() {
                // Real-time sync for text inputs (especially product name)
                if (isSyncing) return;
                
                const formField = this.dataset.formField;
                const paramKey = this.dataset.paramKey || this.id.replace('param', '').toLowerCase();
                
                if (formField && this.type === 'text') {
                    isSyncing = true;
                    const formInput = document.getElementById(formField);
                    if (formInput) {
                        formInput.value = this.value;
                    }
                    
                    // Update node name in real-time for product name
                    if (visualEditorInstance && visualEditorInstance.canvasManager && node) {
                        if (paramKey === 'productname' || paramKey === 'productName' || formField === 'productName') {
                            const canvasManager = visualEditorInstance.canvasManager;
                            const nodeId = node.id;
                            const updatedNode = canvasManager.nodes.get(nodeId);
                            if (updatedNode) {
                                updatedNode.name = this.value || 'Product';
                                canvasManager.renderNode(updatedNode);
                            }
                        }
                    }
                    
                    isSyncing = false;
                }
            });
        });
    }

    function setupToolbarButtons() {
        // Add node buttons
        document.querySelectorAll('[data-node-type]').forEach(btn => {
            btn.addEventListener('click', function() {
                const nodeType = this.dataset.nodeType;
                if (visualEditorInstance && visualEditorInstance.canvasManager) {
                    const canvasManager = visualEditorInstance.canvasManager;
                    const x = 200 + (canvasManager.nodes.size * 200);
                    const y = 200;
                    
                    canvasManager.addNode({
                        type: nodeType,
                        name: nodeType === 'product' ? 'Product' : nodeType === 'packaging' ? 'Packaging' : 'Raw Material',
                        x: x,
                        y: y
                    });
                }
            });
        });

        // Tool buttons
        const selectTool = document.getElementById('productVisualSelectTool');
        const panTool = document.getElementById('productVisualPanTool');
        const gridToggle = document.getElementById('productVisualGridToggle');
        const deleteBtn = document.getElementById('productVisualDelete');
        const fitToViewBtn = document.getElementById('productVisualFitToView');

        if (selectTool) {
            selectTool.addEventListener('click', function() {
                if (visualEditorInstance && visualEditorInstance.canvasManager) {
                    visualEditorInstance.canvasManager.setToolMode('select');
                    selectTool.classList.add('active');
                    if (panTool) panTool.classList.remove('active');
                }
            });
        }

        if (panTool) {
            panTool.addEventListener('click', function() {
                if (visualEditorInstance && visualEditorInstance.canvasManager) {
                    visualEditorInstance.canvasManager.setToolMode('pan');
                    panTool.classList.add('active');
                    if (selectTool) selectTool.classList.remove('active');
                }
            });
        }

        if (gridToggle) {
            gridToggle.addEventListener('click', function() {
                if (visualEditorInstance && visualEditorInstance.canvasManager) {
                    visualEditorInstance.canvasManager.toggleGrid();
                    this.classList.toggle('active');
                }
            });
        }

        if (deleteBtn) {
            deleteBtn.addEventListener('click', function() {
                if (visualEditorInstance && visualEditorInstance.canvasManager) {
                    const canvasManager = visualEditorInstance.canvasManager;
                    if (canvasManager.selectedNode) {
                        canvasManager.deleteNode(canvasManager.selectedNode);
                    }
                }
            });
        }

        if (fitToViewBtn) {
            fitToViewBtn.addEventListener('click', function() {
                if (visualEditorInstance && visualEditorInstance.canvasManager) {
                    visualEditorInstance.canvasManager.fitToView();
                }
            });
        }
    }

    function escapeHtml(text) {
        if (text === null || text === undefined) return '';
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    function setupSyncHandlers() {
        if (!visualEditorInstance || !visualEditorInstance.canvasManager) return;

        // Sync form changes to visual view
        const form = document.getElementById('productForm');
        if (form) {
            form.addEventListener('input', function(e) {
                if (!isSyncing) {
                    syncFormToVisual();
                }
            });

            form.addEventListener('change', function(e) {
                if (!isSyncing) {
                    syncFormToVisual();
                }
            });
        }

        // Sync visual view changes to form
        if (visualEditorInstance.canvasManager) {
            const canvasManager = visualEditorInstance.canvasManager;
            
            // Listen for node updates via MutationObserver
            const nodesLayer = document.getElementById('eprNodesLayer');
            if (nodesLayer) {
                const observer = new MutationObserver(function(mutations) {
                    if (!isSyncing) {
                        setTimeout(() => {
                            syncVisualToForm();
                        }, 100);
                    }
                });

                observer.observe(nodesLayer, {
                    childList: true,
                    subtree: true,
                    attributes: true,
                    attributeFilter: ['data-node-id', 'style']
                });
            }
            
            // Also hook into node update events if available
            if (canvasManager.onNodeUpdate) {
                const originalOnNodeUpdate = canvasManager.onNodeUpdate.bind(canvasManager);
                canvasManager.onNodeUpdate = function(nodeId, node) {
                    originalOnNodeUpdate(nodeId, node);
                    if (!isSyncing) {
                        setTimeout(() => {
                            syncVisualToForm();
                        }, 100);
                    }
                };
            }
        }
    }

    function syncFormToVisual() {
        if (!visualEditorInstance || !visualEditorInstance.canvasManager) return;
        
        isSyncing = true;
        try {
            const canvasManager = visualEditorInstance.canvasManager;
            
            // Get form data
            const productName = document.getElementById('productName')?.value || '';
            const gtin = document.getElementById('gtin')?.value || '';
            const brand = document.getElementById('brand')?.value || '';
            const packagingLevel = document.getElementById('packagingLevel')?.value || '';
            const packagingType = document.getElementById('packagingType')?.value || '';
            
            // Find or create product node
            let productNode = null;
            for (const [id, node] of canvasManager.nodes) {
                if (node.type === 'product') {
                    productNode = node;
                    break;
                }
            }

            if (!productNode && (productName || true)) { // Always create a product node if none exists
                // Create product node
                try {
                    productNode = canvasManager.addNode({
                        type: 'product',
                        name: productName || 'Product',
                        x: 200,
                        y: 200
                    });
                    console.log('[Product Visual View] Product node created:', productNode);
                    
                    // Hide placeholder if nodes exist
                    if (canvasManager.updatePlaceholder) {
                        canvasManager.updatePlaceholder();
                    } else {
                        const placeholder = document.getElementById('eprCanvasPlaceholder');
                        if (placeholder && canvasManager.nodes && canvasManager.nodes.size > 0) {
                            placeholder.classList.add('hidden');
                        }
                    }
                } catch (err) {
                    console.error('[Product Visual View] Error creating product node:', err);
                }
            }

            if (productNode) {
                // Update product node with form data
                const newName = productName || 'Product';
                if (productNode.name !== newName) {
                    productNode.name = newName;
                    // Re-render to show new name
                    try {
                        canvasManager.renderNode(productNode);
                        console.log('[Product Visual View] Product node rendered');
                        
                        // Ensure placeholder is hidden
                        const placeholder = document.getElementById('eprCanvasPlaceholder');
                        if (placeholder) {
                            placeholder.classList.add('hidden');
                        }
                    } catch (err) {
                        console.error('[Product Visual View] Error rendering product node:', err);
                    }
                }
                
                // Store form data in parameters
                if (!productNode.parameters) {
                    productNode.parameters = {};
                }
                productNode.parameters.gtin = gtin;
                productNode.parameters.brand = brand;
                productNode.parameters.productWeight = document.getElementById('productWeight')?.value || '';
                productNode.parameters.productCategory = document.getElementById('productCategory')?.value || '';
                
                productNode.description = `${brand} - ${gtin}`;
                canvasManager.updateNode(productNode.id, productNode);
            }

            // Handle packaging components
            const packagingComponents = getPackagingComponents();
            const existingPackagingNodes = [];
            for (const [id, node] of canvasManager.nodes) {
                if (node.type === 'packaging') {
                    existingPackagingNodes.push(node);
                }
            }
            
            // Remove packaging nodes that are no longer in the form
            existingPackagingNodes.forEach(node => {
                const stillExists = packagingComponents.some(comp => comp.materialType === node.name);
                if (!stillExists) {
                    canvasManager.deleteNode(node.id);
                }
            });
            
            // Create or update packaging nodes
            if (packagingComponents && packagingComponents.length > 0) {
                packagingComponents.forEach((component, index) => {
                    let packagingNode = null;
                    for (const [id, node] of canvasManager.nodes) {
                        if (node.type === 'packaging' && node.name === component.materialType) {
                            packagingNode = node;
                            break;
                        }
                    }

                    if (!packagingNode) {
                        try {
                            packagingNode = canvasManager.addNode({
                                type: 'packaging',
                                name: component.materialType || 'Packaging',
                                x: 400 + (index * 200),
                                y: 200
                            });
                            console.log('[Product Visual View] Packaging node created:', packagingNode);
                            
                            // Hide placeholder if nodes exist
                            if (canvasManager.updatePlaceholder) {
                                canvasManager.updatePlaceholder();
                            } else {
                                const placeholder = document.getElementById('eprCanvasPlaceholder');
                                if (placeholder && canvasManager.nodes && canvasManager.nodes.size > 0) {
                                    placeholder.classList.add('hidden');
                                }
                            }
                        } catch (err) {
                            console.error('[Product Visual View] Error creating packaging node:', err);
                            continue; // Skip this component if node creation fails
                        }
                    }

                    // Update packaging node with component data
                    if (packagingNode) {
                        packagingNode.description = `${component.weight}g`;
                        if (!packagingNode.parameters) {
                            packagingNode.parameters = {};
                        }
                        packagingNode.parameters.weight = component.weight;
                        packagingNode.parameters.height = component.height;
                        packagingNode.parameters.width = component.width;
                        packagingNode.parameters.depth = component.depth;
                        packagingNode.parameters.volumeCapacity = component.volumeCapacity;
                        packagingNode.parameters.volumeUnit = component.volumeUnit;
                        
                        try {
                            canvasManager.renderNode(packagingNode);
                            canvasManager.updateNode(packagingNode.id, packagingNode);
                            console.log('[Product Visual View] Packaging node updated:', packagingNode.id);
                            
                            // Ensure placeholder is hidden
                            if (canvasManager.updatePlaceholder) {
                                canvasManager.updatePlaceholder();
                            } else {
                                const placeholder = document.getElementById('eprCanvasPlaceholder');
                                if (placeholder && canvasManager.nodes && canvasManager.nodes.size > 0) {
                                    placeholder.classList.add('hidden');
                                }
                            }
                        } catch (err) {
                            console.error('[Product Visual View] Error updating packaging node:', err);
                        }
                    }

                    // Connect to product if exists
                    if (productNode) {
                        const existingConn = canvasManager.connections.find(c => 
                            c.from === packagingNode.id && c.to === productNode.id
                        );
                        if (!existingConn) {
                            canvasManager.addConnection(packagingNode.id, productNode.id);
                        }
                    }
                });
            }

            try {
                canvasManager.renderConnections();
                canvasManager.updateConnections();
                console.log('[Product Visual View] Connections updated');
                
                // Ensure placeholder is hidden if nodes exist
                if (canvasManager.updatePlaceholder) {
                    canvasManager.updatePlaceholder();
                } else {
                    const placeholder = document.getElementById('eprCanvasPlaceholder');
                    if (placeholder && canvasManager.nodes && canvasManager.nodes.size > 0) {
                        placeholder.classList.add('hidden');
                    }
                }
            } catch (err) {
                console.error('[Product Visual View] Error updating connections:', err);
            }
        } catch (err) {
            console.error('[Product Visual View] Error in syncFormToVisual:', err);
        } finally {
            isSyncing = false;
        }
    }

    function syncVisualToForm() {
        if (!visualEditorInstance || !visualEditorInstance.canvasManager) return;
        
        isSyncing = true;
        try {
            const canvasManager = visualEditorInstance.canvasManager;
            
            // Find product node
            let productNode = null;
            for (const [id, node] of canvasManager.nodes) {
                if (node.type === 'product') {
                    productNode = node;
                    break;
                }
            }

            if (productNode) {
                // Update form fields from product node
                const productNameField = document.getElementById('productName');
                if (productNameField && productNode.name) {
                    productNameField.value = productNode.name;
                    productNameField.dispatchEvent(new Event('change', { bubbles: true }));
                }

                // Get data from parameters first, then fallback to description
                const gtin = productNode.parameters?.gtin || '';
                const brand = productNode.parameters?.brand || '';
                const productWeight = productNode.parameters?.productWeight || '';
                const productCategory = productNode.parameters?.productCategory || '';
                
                // Extract from description if parameters not available
                if (!gtin || !brand) {
                    if (productNode.description) {
                        const parts = productNode.description.split(' - ');
                        if (parts.length >= 2) {
                            if (!brand) {
                                const brandField = document.getElementById('brand');
                                if (brandField) brandField.value = parts[0];
                            }
                            if (!gtin) {
                                const gtinField = document.getElementById('gtin');
                                if (gtinField) gtinField.value = parts[1];
                            }
                        }
                    }
                } else {
                    // Use parameters
                    const brandField = document.getElementById('brand');
                    const gtinField = document.getElementById('gtin');
                    const weightField = document.getElementById('productWeight');
                    const categoryField = document.getElementById('productCategory');
                    
                    if (brandField && brand) brandField.value = brand;
                    if (gtinField && gtin) gtinField.value = gtin;
                    if (weightField && productWeight) weightField.value = productWeight;
                    if (categoryField && productCategory) categoryField.value = productCategory;
                }
            }

            // Update packaging components from visual nodes
            const packagingNodes = [];
            for (const [id, node] of canvasManager.nodes) {
                if (node.type === 'packaging') {
                    packagingNodes.push(node);
                }
            }

            // Update packaging components list
            if (packagingNodes.length > 0) {
                updatePackagingComponentsFromVisual(packagingNodes);
            }
        } finally {
            isSyncing = false;
        }
    }

    function getPackagingComponents() {
        // Get packaging components from form - check if window.packagingComponents exists (from product-form.js)
        if (window.packagingComponents && Array.isArray(window.packagingComponents)) {
            return window.packagingComponents;
        }
        
        // Fallback: parse from table
        const components = [];
        const componentsList = document.getElementById('packagingComponentsList');
        if (componentsList) {
            const rows = componentsList.querySelectorAll('tbody tr');
            rows.forEach(row => {
                const cells = row.querySelectorAll('td');
                if (cells.length >= 2) {
                    const materialType = cells[0]?.textContent?.trim() || '';
                    const weightText = cells[1]?.textContent?.trim() || '0';
                    const weight = parseFloat(weightText) || 0;
                    if (materialType) {
                        components.push({ materialType, weight });
                    }
                }
            });
        }
        return components;
    }

    function updatePackagingComponentsFromVisual(nodes) {
        // Update packaging components list in form from visual nodes
        if (!window.packagingComponents) {
            window.packagingComponents = [];
        }
        
        // Clear existing components
        const newComponents = [];
        
        // Add components from visual nodes
        nodes.forEach(node => {
            const materialType = node.name || '';
            // Extract weight from description (format: "XXXg") or parameters
            let weight = 0;
            if (node.description) {
                const match = node.description.match(/(\d+(?:\.\d+)?)\s*g/i);
                if (match) {
                    weight = parseFloat(match[1]);
                }
            }
            if (!weight && node.parameters?.weight) {
                weight = parseFloat(node.parameters.weight);
            }
            
            if (materialType) {
                newComponents.push({
                    materialType: materialType,
                    weight: weight || 0,
                    height: node.parameters?.height || 0,
                    width: node.parameters?.width || 0,
                    depth: node.parameters?.depth || 0,
                    volumeCapacity: node.parameters?.volumeCapacity || null,
                    volumeUnit: node.parameters?.volumeUnit || 'ml'
                });
            }
        });
        
        // Update global reference
        window.packagingComponents = newComponents;
        
        // Update the form display
        if (window.updatePackagingComponentsList) {
            window.updatePackagingComponentsList();
        }
        
        // Update total weight
        if (window.updateTotalPackagingWeight) {
            window.updateTotalPackagingWeight();
        }
    }

    function handleGS1Import() {
        // Create file input
        const input = document.createElement('input');
        input.type = 'file';
        input.accept = '.xml,.json';
        input.onchange = function(e) {
            const file = e.target.files[0];
            if (!file) return;

            const reader = new FileReader();
            reader.onload = function(event) {
                try {
                    const content = event.target.result;
                    let data;
                    
                    if (file.name.endsWith('.xml')) {
                        // Parse XML
                        const parser = new DOMParser();
                        const xmlDoc = parser.parseFromString(content, 'text/xml');
                        data = parseGS1XML(xmlDoc);
                    } else if (file.name.endsWith('.json')) {
                        // Parse JSON
                        data = JSON.parse(content);
                    } else {
                        alert('Unsupported file format. Please use XML or JSON.');
                        return;
                    }

                    // Populate form with GS1 data
                    populateFormFromGS1(data);
                    
                    // Sync to visual view after a short delay
                    setTimeout(() => {
                        if (visualEditorInstance) {
                            syncFormToVisual();
                        } else {
                            // If visual view not initialized, try to initialize it
                            const visualViewTab = document.getElementById('visualViewTab');
                            if (visualViewTab) {
                                // Switch to visual view tab to trigger initialization
                                const tab = new bootstrap.Tab(visualViewTab);
                                tab.show();
                            }
                        }
                    }, 300);
                } catch (error) {
                    console.error('[Product Visual View] Error importing GS1:', error);
                    alert('Error importing GS1 data: ' + error.message);
                }
            };
            reader.readAsText(file);
        };
        input.click();
    }

    function parseGS1XML(xmlDoc) {
        // Parse GS1 XML format (GS1 Data Standard XML)
        const data = {};
        
        // Extract product information - try multiple possible element names
        const productNameEl = xmlDoc.querySelector('ProductName, productName, name, Name, gs1:ProductName, Product');
        const productName = productNameEl?.textContent || productNameEl?.getAttribute('name') || '';
        
        const gtinEl = xmlDoc.querySelector('GTIN, gtin, GlobalTradeItemNumber, gs1:GTIN, TradeItemIdentification');
        const gtin = gtinEl?.textContent || gtinEl?.getAttribute('gtin') || '';
        
        const brandEl = xmlDoc.querySelector('Brand, brand, BrandOwner, gs1:Brand, BrandName');
        const brand = brandEl?.textContent || brandEl?.getAttribute('name') || '';
        
        data.productName = productName.trim();
        data.gtin = gtin.trim().replace(/\D/g, ''); // Remove non-digits
        data.brand = brand.trim();
        
        // Extract additional product fields
        const weightEl = xmlDoc.querySelector('NetContent, netContent, Weight, weight, gs1:NetContent');
        if (weightEl) {
            const weight = parseFloat(weightEl.textContent || weightEl.getAttribute('value') || '0');
            const unit = weightEl.getAttribute('unit') || 'g';
            data.productWeight = weight;
            data.productWeightUnit = unit;
        }
        
        const categoryEl = xmlDoc.querySelector('ProductCategory, productCategory, Category, category, gs1:ProductCategory');
        if (categoryEl) {
            data.productCategory = categoryEl.textContent || categoryEl.getAttribute('value') || '';
        }
        
        const countryEl = xmlDoc.querySelector('CountryOfOrigin, countryOfOrigin, Origin, origin, gs1:CountryOfOrigin');
        if (countryEl) {
            data.countryOfOrigin = countryEl.textContent || countryEl.getAttribute('code') || '';
        }
        
        // Extract packaging information
        const packagingElements = xmlDoc.querySelectorAll('Packaging, packaging, Package, package, gs1:Packaging, PackagingComponent');
        if (packagingElements.length > 0) {
            data.packaging = [];
            packagingElements.forEach(pkg => {
                const materialTypeEl = pkg.querySelector('MaterialType, materialType, Material, material, gs1:MaterialType');
                const materialType = materialTypeEl?.textContent || materialTypeEl?.getAttribute('type') || pkg.getAttribute('type') || '';
                
                const weightEl = pkg.querySelector('Weight, weight, gs1:Weight, NetWeight');
                const weight = parseFloat(weightEl?.textContent || weightEl?.getAttribute('value') || pkg.getAttribute('weight') || '0');
                
                if (materialType || weight > 0) {
                    data.packaging.push({ 
                        materialType: materialType.trim(), 
                        weight: weight 
                    });
                }
            });
        }
        
        return data;
    }

    function populateFormFromGS1(data) {
        // Populate form fields from GS1 data
        if (data.productName) {
            const field = document.getElementById('productName');
            if (field) field.value = data.productName;
        }

        if (data.gtin) {
            const field = document.getElementById('gtin');
            if (field) field.value = data.gtin;
        }

        if (data.brand) {
            const field = document.getElementById('brand');
            if (field) field.value = data.brand;
        }

        // Populate packaging components
        if (data.packaging && data.packaging.length > 0) {
            // Clear existing components
            const componentsList = document.getElementById('packagingComponentsList');
            if (componentsList) {
                componentsList.innerHTML = '';
            }

            // Add new components using the form's component modal
            if (data.packaging && data.packaging.length > 0) {
                // Open modal for first component
                const btnAddComponent = document.getElementById('btnAddComponent');
                if (btnAddComponent) {
                    btnAddComponent.click();
                    
                    // Wait for modal to open, then populate and save components one by one
                    setTimeout(() => {
                        let componentIndex = 0;
                        
                        function addNextComponent() {
                            if (componentIndex >= data.packaging.length) {
                                // Close modal if still open
                                const modalEl = document.getElementById('componentModal');
                                if (modalEl) {
                                    const modal = bootstrap.Modal.getInstance(modalEl);
                                    if (modal) modal.hide();
                                }
                                return;
                            }
                            
                            const component = data.packaging[componentIndex];
                            const materialTypeField = document.getElementById('componentMaterialType');
                            const weightField = document.getElementById('componentWeight');
                            
                            if (materialTypeField && weightField) {
                                materialTypeField.value = component.materialType || '';
                                weightField.value = component.weight || '';
                                
                                // Trigger the save component button
                                const saveBtn = document.getElementById('btnSaveComponent');
                                if (saveBtn) {
                                    saveBtn.click();
                                    
                                    // After saving, open modal again for next component
                                    componentIndex++;
                                    if (componentIndex < data.packaging.length) {
                                        setTimeout(() => {
                                            btnAddComponent.click();
                                            setTimeout(addNextComponent, 300);
                                        }, 500);
                                    } else {
                                        // All components added, close modal
                                        setTimeout(() => {
                                            const modalEl = document.getElementById('componentModal');
                                            if (modalEl) {
                                                const modal = bootstrap.Modal.getInstance(modalEl);
                                                if (modal) modal.hide();
                                            }
                                        }, 500);
                                    }
                                }
                            }
                        }
                        
                        addNextComponent();
                    }, 300);
                }
            }
            
            // Also populate other product fields
            if (data.productWeight) {
                const weightField = document.getElementById('productWeight');
                const weightUnitField = document.getElementById('productWeightUnit');
                if (weightField) weightField.value = data.productWeight;
                if (weightUnitField && data.productWeightUnit) weightUnitField.value = data.productWeightUnit;
            }
            
            if (data.productCategory) {
                const categoryField = document.getElementById('productCategory');
                if (categoryField) {
                    categoryField.value = data.productCategory;
                    // Trigger change event to update sub-categories
                    categoryField.dispatchEvent(new Event('change'));
                }
            }
            
            if (data.countryOfOrigin) {
                const countryField = document.getElementById('countryOfOrigin');
                if (countryField) countryField.value = data.countryOfOrigin;
            }
        }
    }

    // Export functions for external use
    window.ProductFormVisualView = {
        syncFormToVisual: syncFormToVisual,
        syncVisualToForm: syncVisualToForm,
        getVisualEditorInstance: function() { return visualEditorInstance; }
    };
    
    // Also expose for easier access (lowercase for consistency)
    window.productFormVisualView = window.ProductFormVisualView;
    
    // Direct access
    window.syncFormToVisual = syncFormToVisual;
})();
