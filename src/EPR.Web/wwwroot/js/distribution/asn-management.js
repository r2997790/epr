// ASN Management JavaScript
(function () {
    'use strict';

    let currentShipments = [];
    let currentShipment = null;
    let isLoading = false;
    let loadTimeout = null;
    let retryCount = 0;
    const MAX_RETRIES = 3;
    const LOAD_TIMEOUT_MS = 15000; // 15 second timeout
    var _modalRestoreParents = {};

    // Run init when Distribution content is present (full page load or when injected into a tab)
    function tryInitDistributionPage() {
        const listView = document.getElementById('listView');
        if (!listView) return;
        if (listView.dataset.asnInited === 'true') return;
        listView.dataset.asnInited = 'true';
        console.log('ASN Management initializing (Distribution page detected)');
        resetPageState();
        initializeEventListeners();
        document.addEventListener('visibilitychange', function onVisibility() {
            if (document.hidden) return;
            if (!document.getElementById('listView')) return;
            if (isLoading === false && currentShipments.length === 0) {
                console.log('Page became visible, reloading data...');
                loadShipments();
            }
        });
        loadShipments();
    }

    document.addEventListener('DOMContentLoaded', function () {
        tryInitDistributionPage();
        // When Distribution is loaded in a browser tab, content is injected later; watch for #listView
        const observer = new MutationObserver(function (mutations) {
            for (let i = 0; i < mutations.length; i++) {
                const added = mutations[i].addedNodes;
                for (let j = 0; j < added.length; j++) {
                    const node = added[j];
                    if (node.nodeType !== 1) continue;
                    const hasListView = node.id === 'listView' || (node.querySelector && node.querySelector('#listView'));
                    if (hasListView) {
                        setTimeout(tryInitDistributionPage, 50);
                        return;
                    }
                }
            }
        });
        observer.observe(document.body, { childList: true, subtree: true });
    });
    
    function resetPageState() {
        // Clear any stuck loading states
        const loadingIndicator = document.getElementById('loadingIndicator');
        const shipmentsContainer = document.getElementById('shipmentsContainer');
        const tbody = document.getElementById('shipmentsTableBody');
        
        // Reset flags
        isLoading = false;
        if (loadTimeout) {
            clearTimeout(loadTimeout);
            loadTimeout = null;
        }
        
        // Ensure containers are in correct initial state
        if (loadingIndicator) {
            loadingIndicator.style.display = 'block';
        }
        if (shipmentsContainer) {
            shipmentsContainer.style.display = 'none';
        }
        if (tbody) {
            tbody.innerHTML = '';
        }
    }

    function initializeEventListeners() {
        // Upload button
        const btnUpload = document.getElementById('btnUploadAsn');
        if (btnUpload) {
            console.log('Upload button found, attaching event listener');
            
            // Remove any existing listeners first
            btnUpload.replaceWith(btnUpload.cloneNode(true));
            const newBtn = document.getElementById('btnUploadAsn');
            
            newBtn.addEventListener('click', function (e) {
                e.preventDefault();
                e.stopPropagation();
                console.log('Upload button clicked!');
                
                // Check if Bootstrap is available
                if (typeof bootstrap === 'undefined') {
                    console.error('Bootstrap is not loaded!');
                    alert('Bootstrap is not loaded. Please refresh the page.');
                    return;
                }
                
                const modalEl = document.getElementById('uploadModal');
                if (!modalEl) {
                    console.error('Upload modal element not found!');
                    alert('Modal element not found. Please refresh the page.');
                    return;
                }
                
                try {
                    console.log('Opening modal...');
                    const uploadModal = new bootstrap.Modal(modalEl);
                    uploadModal.show();
                    console.log('Modal opened successfully');
                } catch (error) {
                    console.error('Error opening modal:', error);
                    alert('Error opening modal: ' + error.message);
                }
            });
            
            console.log('Event listener attached successfully');
        } else {
            console.error('Upload button not found!');
        }

        // Process upload
        const btnProcess = document.getElementById('btnProcessUpload');
        if (btnProcess) {
            btnProcess.addEventListener('click', processUpload);
        }

        // Refresh button
        const btnRefresh = document.getElementById('btnRefresh');
        if (btnRefresh) {
            btnRefresh.addEventListener('click', loadShipments);
        }

        // Create Dummy Data button
        const btnCreateDummyData = document.getElementById('btnCreateDummyData');
        if (btnCreateDummyData) {
            btnCreateDummyData.addEventListener('click', async function() {
                if (!confirm('This will create sample ASN, Product, and Packaging data. Continue?')) {
                    return;
                }

                btnCreateDummyData.disabled = true;
                btnCreateDummyData.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Creating...';

                try {
                    const response = await fetch('/Distribution/CreateDummyData', {
                        method: 'POST',
                        credentials: 'include',
                        headers: {
                            'Content-Type': 'application/json',
                            'X-Requested-With': 'XMLHttpRequest'
                        }
                    });

                    const responseText = await response.text();
                    const contentType = response.headers.get('content-type') || '';
                    const isHtml = !response.ok || contentType.includes('text/html') || responseText.trim().startsWith('<!') || responseText.trim().startsWith('<html');
                    if (isHtml) {
                        if (responseText.includes('login') || responseText.includes('Login') || responseText.includes('Account/Login')) {
                            alert('Session expired. Please log in again and try Create Sample Data.');
                        } else {
                            alert('Session expired or server error. Please log in again and try Create Sample Data.');
                        }
                        return;
                    }
                    let result;
                    try {
                        result = JSON.parse(responseText);
                    } catch (parseErr) {
                        alert('Error creating sample data: Invalid response from server. Please try logging in again.');
                        return;
                    }

                    if (result.success) {
                        alert('Sample data created successfully! Refreshing page...');
                        location.reload();
                    } else {
                        alert('Error creating sample data: ' + (result.message || 'Unknown error'));
                    }
                } catch (error) {
                    console.error('Error creating dummy data:', error);
                    alert('Error creating sample data: ' + error.message);
                } finally {
                    btnCreateDummyData.disabled = false;
                    btnCreateDummyData.innerHTML = '<i class="bi bi-database-add"></i> Create Sample Data';
                }
            });
        }

        // Create New ASN button
        const btnCreateAsn = document.getElementById('btnCreateAsn');
        if (btnCreateAsn) {
            btnCreateAsn.addEventListener('click', function(e) {
                e.preventDefault();
                e.stopPropagation();
                
                if (typeof bootstrap === 'undefined') {
                    alert('Bootstrap is not loaded. Please refresh the page.');
                    return;
                }
                
                const modalEl = document.getElementById('createAsnModal');
                if (!modalEl) {
                    alert('Create ASN modal not found. Please refresh the page.');
                    return;
                }
                if (typeof moveModalToBody === 'function') moveModalToBody('createAsnModal');
                try {
                    const modal = new bootstrap.Modal(modalEl, {
                        backdrop: true,
                        keyboard: true,
                        focus: true
                    });
                    modal.show();
                    
                    // Ensure modal is on top
                    setTimeout(() => {
                        const modalElement = document.querySelector('#createAsnModal');
                        if (modalElement) {
                            modalElement.style.zIndex = '10500';
                        }
                        const backdrop = document.querySelector('.modal-backdrop');
                        if (backdrop) {
                            backdrop.style.zIndex = '10499';
                        }
                    }, 10);
                } catch (error) {
                    console.error('Error opening create ASN modal:', error);
                    alert('Error opening modal: ' + error.message);
                }
            });
        }

        // Back to list
        const btnBack = document.getElementById('btnBackToList');
        if (btnBack) {
            btnBack.addEventListener('click', function(e) { e.preventDefault(); showListView(); });
        }

        // Sync view with URL on browser back/forward
        window.addEventListener('popstate', function(e) {
            var base = getDistributionBasePath();
            var pathname = window.location.pathname.replace(/\/$/, '');
            var match = pathname.match(new RegExp('^' + base.replace(/[.*+?^${}()|[\]\\]/g, '\\$&') + '\\/(\\d+)\\/?$', 'i'));
            if (match) {
                var id = parseInt(match[1], 10);
                if (!isNaN(id)) showDetailView(id);
            } else {
                showListView();
            }
        });

        // Move modals to body when shown so they are editable (not clipped by tab content)
        ['uploadModal', 'createAsnModal', 'editPalletModal', 'editLineItemModal'].forEach(function(modalId) {
            var modalEl = document.getElementById(modalId);
            if (modalEl) {
                modalEl.addEventListener('hidden.bs.modal', function() {
                    if (_modalRestoreParents[modalId]) {
                        _modalRestoreParents[modalId].appendChild(modalEl);
                        delete _modalRestoreParents[modalId];
                    }
                });
            }
        });

        // Save Edit Pallet
        const btnSaveEditPallet = document.getElementById('btnSaveEditPallet');
        if (btnSaveEditPallet) {
            btnSaveEditPallet.addEventListener('click', saveEditPallet);
        }

        // Save Edit Line Item
        const btnSaveEditLineItem = document.getElementById('btnSaveEditLineItem');
        if (btnSaveEditLineItem) {
            btnSaveEditLineItem.addEventListener('click', saveEditLineItem);
        }
    }

    async function saveEditPallet() {
        var shipmentId = parseInt(document.getElementById('editPalletShipmentId').value, 10);
        var palletId = parseInt(document.getElementById('editPalletId').value, 10);
        var destName = document.getElementById('editPalletDestName').value.trim();
        if (!destName && palletId > 0) destName = 'Not specified';
        if (palletId === 0 && !destName) {
            alert('Destination name is required.');
            return;
        }
        var payload = {
            Id: shipmentId,
            Pallets: [{
                Id: palletId,
                Sscc: document.getElementById('editPalletSscc').value.trim() || null,
                PackageTypeCode: document.getElementById('editPalletPackageType').value.trim() || null,
                GrossWeight: parseFloat(document.getElementById('editPalletGrossWeight').value) || null,
                DestinationName: destName || null,
                DestinationAddress: document.getElementById('editPalletDestAddress').value.trim() || null,
                DestinationCity: document.getElementById('editPalletDestCity').value.trim() || null,
                DestinationPostalCode: document.getElementById('editPalletDestPostalCode').value.trim() || null,
                DestinationCountryCode: document.getElementById('editPalletDestCountryCode').value.trim() || null,
                DestinationGln: document.getElementById('editPalletDestGln').value.trim() || null,
                SequenceNumber: 1,
                IsSimulated: document.getElementById('editPalletIsSimulated').checked
            }]
        };
        try {
            var response = await fetch('/Distribution/UpdateAsnShipment', {
                method: 'POST',
                credentials: 'include',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            });
            var result = await response.json();
            if (result.success) {
                if (typeof bootstrap !== 'undefined' && bootstrap.Modal) {
                    var modalEl = document.getElementById('editPalletModal');
                    var m = bootstrap.Modal.getInstance(modalEl);
                    if (m) m.hide();
                }
                showDetailView(shipmentId);
            } else {
                alert('Failed to save: ' + (result.message || 'Unknown error'));
            }
        } catch (err) {
            console.error(err);
            alert('Error saving pallet: ' + err.message);
        }
    }

    async function saveEditLineItem() {
        var shipmentId = parseInt(document.getElementById('editLineItemShipmentId').value, 10);
        var palletId = parseInt(document.getElementById('editLineItemPalletId').value, 10);
        var lineItemId = parseInt(document.getElementById('editLineItemId').value, 10);
        var gtin = document.getElementById('editLineItemGtin').value.trim();
        var description = document.getElementById('editLineItemDescription').value.trim();
        var qty = parseFloat(document.getElementById('editLineItemQuantity').value);
        if (!gtin || !description) {
            alert('GTIN and Description are required.');
            return;
        }
        if (isNaN(qty) || qty <= 0) {
            alert('Quantity must be a positive number.');
            return;
        }
        var bestBeforeVal = document.getElementById('editLineItemBestBeforeDate').value;
        var bestBefore = bestBeforeVal ? new Date(bestBeforeVal + 'T12:00:00Z') : null;
        var payload = {
            Id: shipmentId,
            Pallets: [{
                Id: palletId,
                LineItems: [{
                    Id: lineItemId,
                    LineNumber: parseInt(document.getElementById('editLineItemLineNumber').value, 10) || 1,
                    Gtin: gtin,
                    Description: description,
                    Quantity: qty,
                    UnitOfMeasure: document.getElementById('editLineItemUnitOfMeasure').value.trim() || 'EA',
                    BatchNumber: document.getElementById('editLineItemBatchNumber').value.trim() || null,
                    BestBeforeDate: bestBefore ? bestBefore.toISOString() : null,
                    PoLineReference: document.getElementById('editLineItemPoLineRef').value.trim() || null,
                    SupplierArticleNumber: document.getElementById('editLineItemSupplierArticle').value.trim() || null,
                    NetWeight: null,
                    IsSimulated: document.getElementById('editLineItemIsSimulated').checked
                }]
            }]
        };
        try {
            var response = await fetch('/Distribution/UpdateAsnShipment', {
                method: 'POST',
                credentials: 'include',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            });
            var result = await response.json();
            if (result.success) {
                if (typeof bootstrap !== 'undefined' && bootstrap.Modal) {
                    var modalEl = document.getElementById('editLineItemModal');
                    var m = bootstrap.Modal.getInstance(modalEl);
                    if (m) m.hide();
                }
                showDetailView(shipmentId);
            } else {
                alert('Failed to save: ' + (result.message || 'Unknown error'));
            }
        } catch (err) {
            console.error(err);
            alert('Error saving line item: ' + err.message);
        }
    }

    async function loadShipments() {
        // Prevent multiple simultaneous loads
        if (isLoading) {
            console.log('⚠️ Load already in progress, skipping...');
            return;
        }

        // Clear any existing timeout
        if (loadTimeout) {
            clearTimeout(loadTimeout);
            loadTimeout = null;
        }

        isLoading = true;
        retryCount = 0;
        console.log('🔄 Loading ASN shipments...');
        showLoading(true);

        // Set timeout to prevent infinite loading
        loadTimeout = setTimeout(() => {
            if (isLoading) {
                console.warn('⚠️ Load timeout reached, clearing loading state');
                isLoading = false;
                showLoading(false);
                
                // Show error message
                const tbody = document.getElementById('shipmentsTableBody');
                if (tbody && tbody.children.length === 0) {
                    tbody.innerHTML = `
                        <tr>
                            <td colspan="9" class="text-center">
                                <div class="alert alert-warning">
                                    <i class="bi bi-exclamation-triangle"></i> 
                                    Request timed out. 
                                    <button class="btn btn-sm btn-primary ms-2" onclick="location.reload()">
                                        <i class="bi bi-arrow-clockwise"></i> Retry
                                    </button>
                                </div>
                            </td>
                        </tr>
                    `;
                }
            }
        }, LOAD_TIMEOUT_MS);

        try {
            console.log('📡 Fetching from /Distribution/GetAsnShipments...');
            
            // Create abort controller for timeout
            const controller = new AbortController();
            const timeoutId = setTimeout(() => controller.abort(), LOAD_TIMEOUT_MS - 1000);
            
            const response = await fetch('/Distribution/GetAsnShipments', {
                signal: controller.signal,
                credentials: 'include',
                headers: {
                    'Accept': 'application/json',
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });
            
            clearTimeout(timeoutId);
            clearTimeout(loadTimeout);
            loadTimeout = null;
            
            console.log('📥 Response status:', response.status);
            console.log('📥 Response Content-Type:', response.headers.get('content-type'));

            // Read body once (stream can only be read once)
            const responseText = await response.text();
            console.log('📦 Response text (first 200 chars):', responseText.substring(0, 200));

            // Check if we got redirected to login page
            const contentType = response.headers.get('content-type') || '';
            if (contentType.includes('text/html') && (responseText.includes('login') || responseText.includes('Login') || responseText.includes('Account/Login'))) {
                console.error('❌ Got HTML response (likely login page redirect)');
                isLoading = false;
                showLoading(false);
                showError('Session expired. Please <a href="/Account/Login">login again</a>.', true);
                return;
            }

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            
            let result;
            try {
                result = JSON.parse(responseText);
                console.log('📦 API result:', result);
            } catch (parseError) {
                console.error('❌ Failed to parse JSON response:', parseError);
                console.error('❌ Response was:', responseText.substring(0, 500));
                
                // Check if it's HTML (login page)
                if (responseText.trim().startsWith('<!DOCTYPE') || responseText.trim().startsWith('<html')) {
                    isLoading = false;
                    showLoading(false);
                    showError('Session expired. Please <a href="/Account/Login">login again</a>.', true);
                    return;
                }
                
                isLoading = false;
                showLoading(false);
                showError('Failed to parse server response. The server may have returned an error page.');
                return;
            }

            // Clear timeout cleanup
            if (loadTimeout) {
                clearTimeout(loadTimeout);
                loadTimeout = null;
            }
            
            isLoading = false;
            
            if (result.success) {
                currentShipments = result.data || [];
                const requiresDataset = result.requiresDataset === true;
                console.log('✅ Loaded ' + currentShipments.length + ' shipments' + (requiresDataset ? ' (dataset required)' : ''));
                console.log('🎨 Rendering shipments...');

                // Render data FIRST
                renderShipmentsList(currentShipments, requiresDataset);
                
                // Then hide loading indicator AFTER data is rendered
                console.log('⏹️ Hiding loading indicator after rendering...');
                showLoading(false);
                console.log('✅ Load complete!');
                retryCount = 0; // Reset retry count on success
                tryOpenInitialAsnFromUrl();
            } else {
                console.error('❌ API returned error:', result.message);
                isLoading = false;
                showLoading(false);
                showError('Failed to load shipments: ' + (result.message || 'Unknown error'));
            }
        } catch (error) {
            clearTimeout(loadTimeout);
            loadTimeout = null;
            isLoading = false;
            
            console.error('❌ Error loading shipments:', error);
            
            // Handle abort (timeout)
            if (error.name === 'AbortError') {
                showLoading(false);
                showError('Request timed out. Please check your connection and try again.');
                return;
            }
            
            // Retry logic for network errors
            if (retryCount < MAX_RETRIES && (error.message.includes('Failed to fetch') || error.message.includes('NetworkError'))) {
                retryCount++;
                console.log(`🔄 Retrying... (${retryCount}/${MAX_RETRIES})`);
                showLoading(false);
                setTimeout(() => {
                    loadShipments();
                }, 1000 * retryCount); // Exponential backoff
                return;
            }
            
            showLoading(false);
            showError('Error loading shipments: ' + error.message);
        }
    }

    function showLoading(show) {
        const loadingIndicator = document.getElementById('loadingIndicator');
        const shipmentsContainer = document.getElementById('shipmentsContainer');
        
        console.log('showLoading called with:', show);
        
        if (loadingIndicator) {
            if (show) {
                loadingIndicator.style.setProperty('display', 'block', 'important');
                console.log('✓ Loading indicator shown');
            } else {
                loadingIndicator.style.setProperty('display', 'none', 'important');
                console.log('✓ Loading indicator hidden');
            }
        } else {
            console.error('❌ Loading indicator element not found');
        }
        
        if (shipmentsContainer) {
            if (show) {
                shipmentsContainer.style.setProperty('display', 'none', 'important');
            } else {
                shipmentsContainer.style.setProperty('display', 'block', 'important');
                console.log('✓ Shipments container shown');
            }
        } else {
            console.error('❌ Shipments container element not found');
        }
    }

    function renderShipmentsList(shipments, requiresDataset) {
        console.log('📋 Rendering shipments list, count:', shipments ? shipments.length : 0, 'requiresDataset:', !!requiresDataset);

        const tbody = document.getElementById('shipmentsTableBody');
        const shipmentsContainer = document.getElementById('shipmentsContainer');
        const noShipmentsEl = document.getElementById('noShipments');
        const tableResponsive = document.querySelector('#shipmentsContainer .table-responsive');
        const loadingIndicator = document.getElementById('loadingIndicator');
        
        if (!tbody) {
            console.error('❌ shipmentsTableBody element not found!');
            return;
        }
        
        // CRITICAL: Force hide loading indicator with !important via inline style
        if (loadingIndicator) {
            loadingIndicator.style.setProperty('display', 'none', 'important');
            console.log('✓ Loading indicator hidden');
        }
        
        // ALWAYS make sure the container is visible
        if (shipmentsContainer) {
            shipmentsContainer.style.setProperty('display', 'block', 'important');
            console.log('✓ Shipments container visible');
        }
        
        // Clear existing content
        tbody.innerHTML = '';

        if (!shipments || shipments.length === 0) {
            console.log('📭 No shipments to display');
            if (noShipmentsEl) {
                const msgEl = noShipmentsEl.querySelector('p');
                if (msgEl) {
                    msgEl.innerHTML = requiresDataset
                        ? 'Please select a dataset (e.g. <a href="/">Electronics</a>, <a href="/">Alcoholic Beverages</a>, or <a href="/">Confectionary</a>) from the home page to view ASN shipments.'
                        : 'No ASN shipments found. Upload an ASN file or click Create Sample Data to get started.';
                }
                noShipmentsEl.style.display = 'block';
                console.log('✓ Showing "no shipments" message');
            }
            if (tableResponsive) {
                tableResponsive.style.display = 'none';
            }
            return;
        }

        console.log('✅ Displaying ' + shipments.length + ' shipments');
        if (noShipmentsEl) {
            noShipmentsEl.style.display = 'none';
            console.log('✓ Hiding "no shipments" message');
        }
        if (tableResponsive) {
            tableResponsive.style.display = 'block';
            console.log('✓ Table visible');
        }

        shipments.forEach(shipment => {
            const row = document.createElement('tr');
            row.style.cursor = 'pointer';
            row.addEventListener('click', () => showDetailView(shipment.id));

            // Format dates
            const shipDate = new Date(shipment.shipDate).toLocaleDateString();
            const deliveryDate = shipment.deliveryDate 
                ? new Date(shipment.deliveryDate).toLocaleDateString() 
                : 'N/A';

            // Carrier/Vehicle info
            const carrierInfo = shipment.vehicleRegistration 
                ? `${shipment.carrierName || 'Unknown'} (${shipment.vehicleRegistration})`
                : (shipment.carrierName || 'N/A');

            // Destinations
            const destinations = shipment.destinations && shipment.destinations.length > 0
                ? shipment.destinations.map(d => 
                    `<span class="destination-badge">
                        <i class="bi bi-geo-alt"></i> ${escapeHtml(d.destinationName)} - ${escapeHtml(d.destinationCity || '')}, ${escapeHtml(d.destinationCountryCode || '')}
                    </span>`
                  ).join('')
                : '<span class="text-muted">No destinations</span>';

            // Chain indicator
            let chainIndicator = '<span class="text-muted">-</span>';
            if (shipment.isChained) {
                let chainHtml = '';
                if (shipment.parentAsnId) {
                    chainHtml += `<i class="bi bi-arrow-down-circle text-primary" title="Has parent: ${escapeHtml(shipment.parentAsnNumber || '')}"></i> `;
                }
                if (shipment.childAsnCount > 0) {
                    chainHtml += `<i class="bi bi-arrow-up-circle text-success" title="Has ${shipment.childAsnCount} child(ren)"></i>`;
                }
                chainIndicator = `
                    <div class="d-flex align-items-center gap-1">
                        ${chainHtml}
                        <span class="badge bg-info">${shipment.parentAsnId ? '↓' : ''}${shipment.childAsnCount > 0 ? '↑' : ''}</span>
                    </div>
                `;
            }

            // Status
            const statusClass = `status-${shipment.status.toLowerCase().replace('_', '-')}`;
            const statusBadge = `<span class="status-badge ${statusClass}">${shipment.status}</span>`;

            // Simulated indicator
            const simulatedBadge = shipment.isSimulated 
                ? '<span class="badge bg-warning text-dark ms-1" title="Simulated ASN"><i class="bi bi-lightning-charge"></i> Simulated</span>'
                : '';

            row.innerHTML = `
                <td><strong>${escapeHtml(shipment.asnNumber)}</strong>${simulatedBadge}</td>
                <td>
                    <div>${escapeHtml(shipment.shipperName)}</div>
                    <small class="text-muted">Format: ${shipment.sourceFormat}</small>
                </td>
                <td>${escapeHtml(shipment.receiverName)}</td>
                <td>
                    <div>${shipDate}</div>
                    <small class="text-muted">Est. Delivery: ${deliveryDate}</small>
                </td>
                <td>${escapeHtml(carrierInfo)}</td>
                <td>
                    <div><i class="bi bi-box-seam"></i> ${shipment.palletCount} pallets</div>
                    <small class="text-muted">${shipment.totalItems} items</small>
                </td>
                <td>${destinations}</td>
                <td class="text-center">${chainIndicator}</td>
                <td>${statusBadge}</td>
                <td>
                    <button class="btn btn-sm btn-outline-primary" onclick="event.stopPropagation(); viewShipmentDetails(${shipment.id})">
                        <i class="bi bi-eye"></i>
                    </button>
                    <button class="btn btn-sm btn-outline-danger" onclick="event.stopPropagation(); deleteShipment(${shipment.id}, '${escapeHtml(shipment.asnNumber)}')">
                        <i class="bi bi-trash"></i>
                    </button>
                </td>
            `;
            tbody.appendChild(row);
        });
    }

    async function showDetailView(shipmentId) {
        console.log('Loading shipment details for ID:', shipmentId);

        if (!shipmentId || shipmentId <= 0) {
            console.error('Invalid shipment ID:', shipmentId);
            showError('Invalid shipment ID');
            return;
        }

        try {
            const url = `/Distribution/GetAsnShipment?id=${shipmentId}`;
            console.log('Fetching from:', url);
            
            const response = await fetch(url, {
                method: 'GET',
                credentials: 'include',
                headers: {
                    'Accept': 'application/json',
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });
            
            console.log('Response status:', response.status);
            console.log('Response Content-Type:', response.headers.get('content-type'));

            const responseText = await response.text();
            if (!response.ok) {
                console.error('HTTP error response:', responseText);
                throw new Error(`HTTP error! status: ${response.status} - ${responseText.substring(0, 200)}`);
            }
            const contentType = response.headers.get('content-type') || '';
            if (contentType.includes('text/html') && (responseText.includes('login') || responseText.includes('Login'))) {
                showError('Session expired. Please <a href="/Account/Login">login again</a>.', true);
                return;
            }
            let result;
            try {
                result = JSON.parse(responseText);
            } catch (e) {
                console.error('Failed to parse GetAsnShipment response:', e);
                showError('Invalid response from server.');
                return;
            }
            console.log('API result:', result);

            if (result.success && result.data) {
                currentShipment = result.data;
                console.log('Shipment data loaded successfully:', {
                    id: result.data.id,
                    asnNumber: result.data.asnNumber,
                    palletCount: result.data.pallets ? result.data.pallets.length : 0
                });
                
                // Ensure pallets array exists
                if (!result.data.pallets) {
                    result.data.pallets = [];
                    console.warn('No pallets array found, initializing empty array');
                }
                
                // Render the detail view
                renderShipmentDetails(result.data);
                
                // Populate raw data tab
                const rawDataEl = document.getElementById('rawDataContent');
                if (rawDataEl) {
                    if (result.data.rawData) {
                        // Format raw data with syntax highlighting
                        rawDataEl.textContent = formatRawData(result.data.rawData, result.data.sourceFormat);
                    } else {
                        rawDataEl.textContent = 'No raw data available for this shipment.';
                    }
                }
                
                // Setup Map View tab
                const mapTab = document.getElementById('map-tab');
                if (mapTab) {
                    mapTab.addEventListener('shown.bs.tab', function (event) {
                        if (typeof window.renderAsnMapView === 'function') {
                            window.renderAsnMapView(result.data);
                        }
                    }, { once: false });
                }
                
                // Setup Visual View tab
                const visualTab = document.getElementById('visual-tab');
                if (visualTab) {
                    visualTab.addEventListener('shown.bs.tab', function (event) {
                        if (typeof window.renderAsnVisualView === 'function') {
                            window.renderAsnVisualView(result.data);
                        }
                    }, { once: false });
                }
                
                // Check if ASN is part of a chain and show/hide chain tab
                const chainTabItem = document.getElementById('chain-tab-item');
                if (chainTabItem) {
                    if (result.data.isChained) {
                        chainTabItem.style.display = 'block';
                        // Load chain data when tab is clicked
                        const chainTab = document.getElementById('chain-tab');
                        if (chainTab) {
                            chainTab.addEventListener('shown.bs.tab', function (event) {
                                loadChainVisualization(shipmentId);
                            }, { once: true }); // Only load once
                    }
                    } else {
                        chainTabItem.style.display = 'none';
                    }
                }
                
                // Switch to detail view
                const listView = document.getElementById('listView');
                const detailView = document.getElementById('detailView');
                
                if (listView) {
                    listView.style.display = 'none';
                }
                if (detailView) {
                    detailView.style.display = 'block';
                }

                var base = getDistributionBasePath();
                var currentPath = window.location.pathname.replace(/\/$/, '');
                var expectedPath = base + '/' + shipmentId;
                if (window.history && window.history.pushState && currentPath !== expectedPath) {
                    window.history.pushState({ asnId: shipmentId }, '', expectedPath);
                }
                
                console.log('Detail view displayed successfully');
            } else {
                const message = result.message || 'Unknown error loading shipment details';
                console.error('API returned error:', message);
                showError('Failed to load shipment details: ' + message);
            }
        } catch (error) {
            console.error('Error loading shipment details:', error);
            console.error('Error stack:', error.stack);
            
            // Provide more specific error messages
            let errorMessage = 'Error loading shipment details: ';
            if (error.name === 'TypeError' && error.message.includes('fetch')) {
                errorMessage += 'Network error. Please check your connection and try again.';
            } else if (error.message) {
                errorMessage += error.message;
            } else {
                errorMessage += 'Unknown error occurred.';
            }
            
            showError(errorMessage);
        }
    }
    
    function formatRawData(rawData, format) {
        if (!rawData) return 'No raw data available';
        
        // Add basic formatting based on format
        if (format === 'EDI_856' || format === 'DESADV') {
            // Add line breaks after segment terminators
            const segmentTerminator = format === 'EDI_856' ? '~' : '\'';
            return rawData.split(segmentTerminator).join(segmentTerminator + '\n');
        } else if (format === 'GS1_XML') {
            // XML is already formatted, just return it
            return rawData;
        }
        
        return rawData;
    }
    
    async function loadChainVisualization(shipmentId) {
        const chainContainer = document.getElementById('chainVisualization');
        if (!chainContainer) return;
        
        try {
            const response = await fetch(`/Distribution/GetAsnChain?id=${shipmentId}`);
            const result = await response.json();
            
            if (result.success && result.data) {
                renderChainDiagram(result.data, chainContainer);
            } else {
                chainContainer.innerHTML = '<div class="alert alert-warning">Unable to load chain data</div>';
            }
        } catch (error) {
            console.error('Error loading chain:', error);
            chainContainer.innerHTML = `<div class="alert alert-danger">Error loading chain: ${error.message}</div>`;
        }
    }
    
    function renderChainDiagram(chainData, container) {
        if (!chainData.chainNodes || chainData.chainNodes.length === 0) {
            container.innerHTML = '<div class="alert alert-info">This ASN is not part of a chain</div>';
            return;
        }
        
        // Generate Mermaid diagram syntax
        let mermaidSyntax = 'graph LR\n';
        
        // Sort nodes by level
        const sortedNodes = [...chainData.chainNodes].sort((a, b) => a.level - b.level);
        
        // Create node definitions
        sortedNodes.forEach(node => {
            const nodeId = `ASN${node.id}`;
            const label = `${node.asnNumber}<br/>${node.shipperName} to ${node.receiverName}`;
            const style = node.isCurrent ? `${nodeId}[["${label}"]]` : `${nodeId}["${label}"]`;
            mermaidSyntax += `    ${style}\n`;
        });
        
        // Create connections
        for (let i = 0; i < sortedNodes.length - 1; i++) {
            const current = sortedNodes[i];
            const next = sortedNodes[i + 1];
            mermaidSyntax += `    ASN${current.id} -->|"${current.receiverGln}"| ASN${next.id}\n`;
        }
        
        // Apply styling to current node
        const currentNode = sortedNodes.find(n => n.isCurrent);
        if (currentNode) {
            mermaidSyntax += `    class ASN${currentNode.id} currentNode\n`;
            mermaidSyntax += `    classDef currentNode fill:#e7f3ff,stroke:#0066cc,stroke-width:3px\n`;
        }
        
        // Create container for diagram
        container.innerHTML = `
            <div class="mb-3">
                <h6>ASN Chain Flow</h6>
                <p class="text-muted small">Click on any node to view that ASN</p>
            </div>
            <div class="mermaid-container p-3 border rounded" style="background-color: #fff;">
                <div class="mermaid">${mermaidSyntax}</div>
            </div>
            <div class="mt-3">
                <h6>Chain Details</h6>
                <ul class="list-group">
                    ${sortedNodes.map(node => `
                        <li class="list-group-item ${node.isCurrent ? 'active' : ''}">
                            <div class="d-flex justify-content-between align-items-center">
                                <div>
                                    <strong>${escapeHtml(node.asnNumber)}</strong> 
                                    ${node.isCurrent ? '<span class="badge bg-primary ms-2">Current</span>' : ''}
                                    <br>
                                    <small>${escapeHtml(node.shipperName)} → ${escapeHtml(node.receiverName)}</small>
                                    <br>
                                    <small class="text-muted">Ship Date: ${new Date(node.shipDate).toLocaleDateString()}</small>
                                </div>
                                ${!node.isCurrent ? `
                                    <button class="btn btn-sm btn-outline-primary" onclick="viewShipmentDetails(${node.id})">
                                        <i class="bi bi-eye"></i> View
                                    </button>
                                ` : ''}
                            </div>
                        </li>
                    `).join('')}
                </ul>
            </div>
        `;
        
        // Render Mermaid diagram
        try {
            mermaid.init(undefined, container.querySelector('.mermaid'));
        } catch (error) {
            console.error('Mermaid rendering error:', error);
        }
    }

    function getDistributionBasePath() {
        const p = window.location.pathname;
        const match = p.match(/^(\/distribution)(?:\/\d+)?\/?$/i);
        return match ? match[1] : '/distribution';
    }

    function tryOpenInitialAsnFromUrl() {
        const base = getDistributionBasePath();
        const pathname = window.location.pathname;
        const match = pathname.match(new RegExp('^' + base.replace(/[.*+?^${}()|[\]\\]/g, '\\$&') + '\\/(\\d+)\\/?$', 'i'));
        let asnId = match ? parseInt(match[1], 10) : null;
        if (!asnId) {
            const page = document.querySelector('.distribution-page');
            const dataId = page && page.getAttribute('data-initial-asn-id');
            if (dataId) asnId = parseInt(dataId, 10);
        }
        if (asnId && !isNaN(asnId)) {
            showDetailView(asnId);
        }
    }

    function showListView() {
        document.getElementById('detailView').style.display = 'none';
        document.getElementById('listView').style.display = 'block';
        currentShipment = null;
        const base = getDistributionBasePath();
        if (window.history && window.history.replaceState && window.location.pathname !== base) {
            window.history.replaceState(null, '', base);
        }
    }

    function renderShipmentDetails(shipment) {
        const container = document.getElementById('shipmentDetails');
        if (!container) return;
        
        // Format dates
        const shipDate = new Date(shipment.shipDate).toLocaleDateString('en-GB', { 
            year: 'numeric', month: 'long', day: 'numeric' 
        });
        const deliveryDate = shipment.deliveryDate 
            ? new Date(shipment.deliveryDate).toLocaleDateString('en-GB', { 
                year: 'numeric', month: 'long', day: 'numeric' 
              })
            : 'Not specified';

        const statusClass = `status-${shipment.status.toLowerCase().replace('_', '-')}`;

        // Check for missing destinations
        const missingDestWarnings = [];
        if (shipment.pallets && shipment.pallets.length > 0) {
            shipment.pallets.forEach((pallet, index) => {
                if (!pallet.destinationGln && !pallet.destinationName) {
                    missingDestWarnings.push({
                        type: 'pallet',
                        palletNumber: index + 1,
                        sscc: pallet.sscc,
                        message: `Pallet ${index + 1}${pallet.sscc ? ` (SSCC: ${pallet.sscc})` : ''} has no destination information`
                    });
                } else if (!pallet.destinationCity && !pallet.destinationAddress) {
                    missingDestWarnings.push({
                        type: 'pallet',
                        palletNumber: index + 1,
                        sscc: pallet.sscc,
                        message: `Pallet ${index + 1}${pallet.sscc ? ` (SSCC: ${pallet.sscc})` : ''} has destination name but missing address/city`
                    });
                }

                // Check line items
                if (pallet.lineItems && pallet.lineItems.length > 0) {
                    pallet.lineItems.forEach((item) => {
                        if (!pallet.destinationGln && !pallet.destinationName) {
                            missingDestWarnings.push({
                                type: 'lineItem',
                                palletNumber: index + 1,
                                lineNumber: item.lineNumber,
                                gtin: item.gtin,
                                description: item.description,
                                message: `Line Item ${item.lineNumber} (${item.description || item.gtin}) on Pallet ${index + 1} has no destination`
                            });
                        }
                    });
                }
            });
        }

        let html = `
            <!-- Missing Destination Warnings -->
            ${missingDestWarnings.length > 0 ? `
            <div class="alert alert-warning mb-4">
                <h6><i class="bi bi-exclamation-triangle"></i> Missing Destination Information</h6>
                <p class="mb-2">The following items are missing destination information:</p>
                <ul class="mb-0">
                    ${missingDestWarnings.map(w => `<li>${escapeHtml(w.message)}</li>`).join('')}
                </ul>
            </div>
            ` : ''}
            
            <!-- Header Info -->
            <div class="row mb-4">
                <div class="col-md-6">
                    <h3>${escapeHtml(shipment.asnNumber)}</h3>
                    <div class="mb-2"><span class="status-badge ${statusClass}">${shipment.status}</span></div>
                    <div class="mb-1"><strong>Source Format:</strong> ${shipment.sourceFormat}</div>
                    <div><strong>Imported:</strong> ${new Date(shipment.importedAt).toLocaleString()}</div>
                </div>
                <div class="col-md-6 text-end">
                    <div class="mb-2"><strong>Ship Date:</strong> ${shipDate}</div>
                    <div class="mb-2"><strong>Est. Delivery:</strong> ${deliveryDate}</div>
                    <div class="mb-2"><strong>Total Pallets:</strong> ${shipment.totalPackages || shipment.pallets.length}</div>
                    <div><strong>Total Weight:</strong> ${shipment.totalWeight ? shipment.totalWeight.toFixed(2) + ' kg' : 'N/A'}</div>
                </div>
            </div>

            <!-- Shipper & Receiver -->
            <div class="row mb-4">
                <div class="col-md-6">
                    <div class="card">
                        <div class="card-header bg-light distribution-section-header">
                            <i class="bi bi-building"></i> <strong>Shipper</strong>
                        </div>
                        <div class="card-body">
                            <div><strong>${escapeHtml(shipment.shipperName)}</strong></div>
                            <div class="text-muted">GLN: ${escapeHtml(shipment.shipperGln)}</div>
                            ${shipment.shipperAddress ? `<div class="mt-2">${escapeHtml(shipment.shipperAddress)}</div>` : ''}
                            ${shipment.shipperCity ? `<div>${escapeHtml(shipment.shipperCity)}, ${escapeHtml(shipment.shipperPostalCode || '')}</div>` : ''}
                            ${shipment.shipperCountryCode ? `<div>${escapeHtml(shipment.shipperCountryCode)}</div>` : ''}
                        </div>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="card">
                        <div class="card-header bg-light distribution-section-header">
                            <i class="bi bi-shop"></i> <strong>Receiver</strong>
                        </div>
                        <div class="card-body">
                            <div><strong>${escapeHtml(shipment.receiverName)}</strong></div>
                            <div class="text-muted">GLN: ${escapeHtml(shipment.receiverGln)}</div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Transport Info -->
            <div class="card mb-4">
                <div class="card-header bg-light distribution-section-header">
                    <i class="bi bi-truck"></i> <strong>Transport Information</strong>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-4">
                            <strong>Carrier:</strong> ${escapeHtml(shipment.carrierName || 'Not specified')}
                        </div>
                        <div class="col-md-4">
                            <strong>Transport Mode:</strong> ${escapeHtml(shipment.transportMode || 'Not specified')}
                        </div>
                        <div class="col-md-4">
                            <strong>Vehicle Reg:</strong> ${escapeHtml(shipment.vehicleRegistration || 'Not specified')}
                        </div>
                    </div>
                    ${shipment.poReference ? `<div class="mt-2"><strong>PO Reference:</strong> ${escapeHtml(shipment.poReference)}</div>` : ''}
                </div>
            </div>

            <!-- Pallets & Line Items -->
            <div class="d-flex justify-content-between align-items-center mb-3">
                <h5 class="mb-0">
                    <i class="bi bi-box-seam"></i> Pallets & Contents 
                    <span class="badge bg-secondary">${shipment.pallets.length} Pallet(s)</span>
                </h5>
                <button type="button" class="btn btn-sm btn-outline-success align-self-center" onclick="addPallet(${shipment.id})" title="Add New Pallet">
                    <i class="bi bi-plus-circle"></i> Add Pallet
                </button>
            </div>
        `;

        // Check if this is a multi-destination shipment
        const uniqueDestinations = [...new Set(shipment.pallets.map(p => p.destinationGln))].filter(Boolean);
        if (uniqueDestinations.length > 1) {
            html += `
                <div class="alert alert-info mb-3">
                    <i class="bi bi-info-circle"></i> <strong>Multi-Destination Shipment:</strong> 
                    This shipment contains pallets going to ${uniqueDestinations.length} different destinations.
                </div>
            `;
        }

        // Render each pallet - compact single-row layout
        shipment.pallets.forEach((pallet, index) => {
            // Calculate total items on this pallet
            const totalItemsOnPallet = pallet.lineItems.reduce((sum, item) => sum + (item.quantity || 0), 0);
            const destParts = [pallet.destinationName || 'Not specified'];
            if (pallet.destinationAddress) destParts.push(pallet.destinationAddress);
            const cityLine = [pallet.destinationCity, pallet.destinationPostalCode, pallet.destinationCountryCode].filter(Boolean).join(', ');
            if (cityLine) destParts.push(cityLine);
            const destOneLine = destParts.map(function(p) { return escapeHtml(p); }).join(' \u2013 ');
            const destWithGln = pallet.destinationGln ? destOneLine + ' \u00B7 GLN: ' + escapeHtml(pallet.destinationGln) : destOneLine;
            
            html += `
                <div class="pallet-card pallet-card-compact ${pallet.isSimulated ? 'border-warning' : ''}" style="${pallet.isSimulated ? 'border-width: 2px; background-color: #fffbf0;' : ''}">
                    <div class="row align-items-center flex-nowrap pallet-row-first">
                        <div class="col-auto text-start">
                            <i class="bi bi-box"></i> <strong>Pallet ${index + 1} of ${shipment.pallets.length}</strong>
                            ${pallet.isSimulated ? '<span class="badge bg-warning text-dark ms-1"><i class="bi bi-lightning-charge"></i> Simulated</span>' : ''}
                        </div>
                        <div class="col text-nowrap">
                            <strong>SSCC:</strong> <code class="pallet-sscc">${escapeHtml(pallet.sscc)}</code>
                        </div>
                        <div class="col text-nowrap pallet-col-attrs">
                            <i class="bi bi-tag"></i> ${pallet.packageTypeCode || 'PLT'} &nbsp;|&nbsp;
                            <i class="bi bi-box-seam"></i> Weight: ${pallet.grossWeight ? pallet.grossWeight.toFixed(2) + ' kg' : 'N/A'} &nbsp;|&nbsp;
                            <i class="bi bi-boxes"></i> ${totalItemsOnPallet} Units
                        </div>
                        <div class="col-auto text-end">
                            <button type="button" class="btn btn-sm btn-link text-secondary p-0 border-0" onclick="editPallet(${shipment.id}, ${pallet.id})" title="Edit Pallet"><i class="bi bi-pencil"></i></button>
                        </div>
                    </div>
                    <div class="pallet-row pallet-row-dest">
                        <span class="pallet-dest-block"><strong><i class="bi bi-geo-alt-fill"></i> Destination:</strong> ${escapeHtml(destWithGln)}</span>
                    </div>

                    <div class="pallet-line-items mt-2 pt-2 border-top">
                        <strong><i class="bi bi-list-check"></i> Line Items (${pallet.lineItems.length}):</strong>
                        <button class="btn btn-sm btn-outline-success ms-2" onclick="addLineItem(${shipment.id}, ${pallet.id})" title="Add Line Item"><i class="bi bi-plus-circle"></i> Add Item</button>
                        <div class="mt-2">
            `;

            pallet.lineItems.forEach(item => {
                const bestBefore = item.bestBeforeDate 
                    ? new Date(item.bestBeforeDate).toLocaleDateString() 
                    : 'N/A';
                const productLink = (item.productId && item.productId > 0)
                    ? `<a href="/Products/Detail/${item.productId}" class="product-detail-link">${escapeHtml(item.description)}</a>`
                    : `<strong>${escapeHtml(item.description)}</strong>`;
                const gtinDisplay = item.productId && item.productId > 0
                    ? `<a href="/Products/Detail/${item.productId}" class="product-detail-link small text-muted">GTIN: ${escapeHtml(item.gtin)}</a>`
                    : `<small class="text-muted">GTIN: ${escapeHtml(item.gtin)}</small>`;

                html += `
                    <div class="line-item-row ${item.isSimulated ? 'border-warning' : ''}" style="${item.isSimulated ? 'border-left: 3px solid #ffc107; background-color: #fffbf0;' : ''}">
                        <div class="row align-items-center">
                            <div class="col-md-1 text-center">
                                <strong>${item.lineNumber}</strong>
                                ${item.isSimulated ? '<br><span class="badge bg-warning text-dark" style="font-size: 0.6rem;"><i class="bi bi-lightning-charge"></i></span>' : ''}
                            </div>
                            <div class="col-md-3">
                                <div>${productLink}</div>
                                <div>${gtinDisplay}</div>
                            </div>
                            <div class="col-md-2">
                                <strong>${item.quantity} ${item.unitOfMeasure}</strong>
                            </div>
                            <div class="col-md-3">
                                ${item.batchNumber ? `<div><i class="bi bi-tag"></i> Batch: ${escapeHtml(item.batchNumber)}</div>` : ''}
                                ${item.bestBeforeDate ? `<div><i class="bi bi-calendar"></i> Best Before: ${bestBefore}</div>` : ''}
                            </div>
                            <div class="col-md-3 line-item-actions">
                                ${item.poLineReference ? `<small>PO Line: ${escapeHtml(item.poLineReference)}</small>` : ''}
                                ${item.supplierArticleNumber ? `<br><small>Article: ${escapeHtml(item.supplierArticleNumber)}</small>` : ''}
                                <button type="button" class="btn btn-sm btn-link text-secondary p-0 border-0 mt-1" onclick="event.stopPropagation(); editLineItem(${shipment.id}, ${pallet.id}, ${item.id})" title="Edit Line Item"><i class="bi bi-pencil"></i></button>
                            </div>
                        </div>
                    </div>
                `;
            });

            html += `
                        </div>
                    </div>
                </div>
            `;
        });

        container.innerHTML = html;
    }

    async function processUpload() {
        const fileInput = document.getElementById('asnFileInput');
        const files = Array.from(fileInput.files);

        if (files.length === 0) {
            showUploadStatus('Please select at least one file', 'danger');
            return;
        }

        const btnProcess = document.getElementById('btnProcessUpload');
        const btnCancel = document.getElementById('btnCancelUpload');
        const progressDiv = document.getElementById('uploadProgress');
        const progressBar = document.getElementById('uploadProgressBar');
        const progressText = document.getElementById('uploadProgressText');
        const fileList = document.getElementById('uploadFileList');
        const statusDiv = document.getElementById('uploadStatus');

        // Hide status and show progress
        statusDiv.style.display = 'none';
        progressDiv.style.display = 'block';
        btnProcess.disabled = true;
        btnCancel.disabled = true;

        let successful = 0;
        let failed = 0;
        const results = [];

        // Create file status list
        fileList.innerHTML = files.map((f, i) => `
            <div id="file-${i}" class="mb-1">
                <i class="bi bi-clock text-muted"></i> 
                <span class="file-name">${escapeHtml(f.name)}</span>
                <span class="file-status text-muted">Waiting...</span>
            </div>
        `).join('');

        for (let i = 0; i < files.length; i++) {
            const file = files[i];
            const fileStatusEl = document.getElementById(`file-${i}`);
            const iconEl = fileStatusEl.querySelector('i');
            const statusSpan = fileStatusEl.querySelector('.file-status');

            try {
                // Update progress
                const percentComplete = Math.round((i / files.length) * 100);
                progressBar.style.width = percentComplete + '%';
                progressBar.textContent = percentComplete + '%';
                progressText.textContent = `Processing ${i + 1} of ${files.length}: ${file.name}`;

                // Update file status
                iconEl.className = 'bi bi-hourglass-split text-primary';
                statusSpan.textContent = 'Uploading...';
                statusSpan.className = 'file-status text-primary';

                const formData = new FormData();
                formData.append('file', file);

                const response = await fetch('/Distribution/UploadAsn', {
                    method: 'POST',
                    credentials: 'include',
                    body: formData
                });

                const result = await response.json();

                if (result.success) {
                    successful++;
                    iconEl.className = 'bi bi-check-circle-fill text-success';
                    statusSpan.textContent = 'Success';
                    statusSpan.className = 'file-status text-success';
                    results.push({ file: file.name, success: true, message: result.message });
                } else {
                    failed++;
                    iconEl.className = 'bi bi-x-circle-fill text-danger';
                    statusSpan.textContent = result.message || 'Failed';
                    statusSpan.className = 'file-status text-danger';
                    results.push({ file: file.name, success: false, message: result.message });
                }
            } catch (error) {
                failed++;
                iconEl.className = 'bi bi-x-circle-fill text-danger';
                statusSpan.textContent = 'Error: ' + error.message;
                statusSpan.className = 'file-status text-danger';
                results.push({ file: file.name, success: false, message: error.message });
            }
        }

        // Complete progress
        progressBar.style.width = '100%';
        progressBar.textContent = '100%';
        progressBar.classList.remove('progress-bar-animated');
        
        if (failed === 0) {
            progressBar.classList.remove('progress-bar-striped');
            progressBar.classList.add('bg-success');
            progressText.textContent = `Complete! ${successful} file(s) uploaded successfully.`;
        } else if (successful === 0) {
            progressBar.classList.remove('progress-bar-striped');
            progressBar.classList.add('bg-danger');
            progressText.textContent = `Failed! ${failed} file(s) failed to upload.`;
        } else {
            progressBar.classList.remove('progress-bar-striped');
            progressBar.classList.add('bg-warning');
            progressText.textContent = `Partial success: ${successful} succeeded, ${failed} failed.`;
        }

        // Show summary
        showUploadStatus(
            `Upload complete: ${successful} successful, ${failed} failed`,
            failed === 0 ? 'success' : (successful === 0 ? 'danger' : 'warning')
        );

        // Re-enable buttons
        btnCancel.disabled = false;
        
        // Auto-close and reload if all succeeded
        if (failed === 0) {
            setTimeout(() => {
                bootstrap.Modal.getInstance(document.getElementById('uploadModal')).hide();
                fileInput.value = '';
                progressDiv.style.display = 'none';
                loadShipments();
            }, 2000);
        } else {
            btnProcess.disabled = false;
            btnProcess.innerHTML = '<i class="bi bi-check-circle"></i> Retry Failed';
        }
    }

    function showUploadStatus(message, type) {
        const statusDiv = document.getElementById('uploadStatus');
        if (!statusDiv) return;
        
        statusDiv.className = `alert alert-${type}`;
        statusDiv.textContent = message;
        statusDiv.style.display = 'block';

        if (type === 'success') {
            setTimeout(() => {
                statusDiv.style.display = 'none';
            }, 3000);
        }
    }

    function showError(message, allowHtml = false) {
        console.error('Error:', message);
        
        // FORCE clear loading state
        isLoading = false;
        if (loadTimeout) {
            clearTimeout(loadTimeout);
            loadTimeout = null;
        }
        
        // FORCE hide loading indicator
        const loadingIndicator = document.getElementById('loadingIndicator');
        if (loadingIndicator) {
            loadingIndicator.style.display = 'none';
        }
        
        // Show error in table or container
        const shipmentsContainer = document.getElementById('shipmentsContainer');
        const tbody = document.getElementById('shipmentsTableBody');
        
        if (shipmentsContainer) {
            shipmentsContainer.style.display = 'block';
        }
        
        if (tbody) {
            const messageHtml = allowHtml ? message : escapeHtml(message);
            tbody.innerHTML = `
                <tr>
                    <td colspan="9" class="text-center">
                        <div class="alert alert-danger" role="alert">
                            <h5><i class="bi bi-exclamation-triangle"></i> Error</h5>
                            <p>${messageHtml}</p>
                            <button class="btn btn-sm btn-primary" onclick="location.reload()">
                                <i class="bi bi-arrow-clockwise"></i> Reload Page
                            </button>
                        </div>
                    </td>
                </tr>
            `;
        } else if (shipmentsContainer) {
            const messageHtml = allowHtml ? message : escapeHtml(message);
            shipmentsContainer.innerHTML = `
                <div class="alert alert-danger" role="alert">
                    <h5><i class="bi bi-exclamation-triangle"></i> Error</h5>
                    <p>${messageHtml}</p>
                    <button class="btn btn-sm btn-primary" onclick="location.reload()">
                        <i class="bi bi-arrow-clockwise"></i> Reload Page
                    </button>
                </div>
            `;
        } else {
            alert(message);
        }
    }

    function escapeHtml(text) {
        if (text === null || text === undefined) return '';
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    // Global function to open upload modal (for testing/backup)
    window.openUploadModal = function() {
        console.log('openUploadModal called');
        const modalEl = document.getElementById('uploadModal');
        if (!modalEl) {
            alert('Modal element not found!');
            return;
        }
        if (typeof bootstrap === 'undefined') {
            alert('Bootstrap not loaded!');
            return;
        }
        try {
            const uploadModal = new bootstrap.Modal(modalEl);
            uploadModal.show();
            console.log('Modal opened via global function');
        } catch (error) {
            console.error('Error opening modal:', error);
            alert('Error: ' + error.message);
        }
    };
    
    // Create New ASN functionality
    function initializeCreateAsn() {
        const btnSaveCreateAsn = document.getElementById('btnSaveCreateAsn');
        if (btnSaveCreateAsn) {
            btnSaveCreateAsn.addEventListener('click', async function() {
                const form = document.getElementById('createAsnForm');
                if (!form.checkValidity()) {
                    form.reportValidity();
                    return;
                }

                const isSimulated = document.querySelector('input[name="asnType"]:checked').value === 'simulated';
                const asnData = {
                    AsnNumber: document.getElementById('createAsnNumber').value,
                    ShipDate: document.getElementById('createShipDate').value,
                    ShipperGln: document.getElementById('createShipperGln').value || null,
                    ShipperName: document.getElementById('createShipperName').value || null,
                    ShipperAddress: document.getElementById('createShipperAddress').value || null,
                    ShipperCity: document.getElementById('createShipperCity').value || null,
                    ShipperCountryCode: document.getElementById('createShipperCountry').value || null,
                    ReceiverGln: document.getElementById('createReceiverGln').value || null,
                    ReceiverName: document.getElementById('createReceiverName').value || null,
                    IsSimulated: isSimulated,
                    Status: 'PENDING'
                };

                try {
                    const response = await fetch('/Distribution/CreateAsnShipment', {
                        method: 'POST',
                        credentials: 'include',
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify(asnData)
                    });

                    const result = await response.json();

                    if (result.success) {
                        const modal = bootstrap.Modal.getInstance(document.getElementById('createAsnModal'));
                        if (modal) {
                            modal.hide();
                        }
                        form.reset();
                        // Reset the form date to today
                        const shipDateInput = document.getElementById('createShipDate');
                        if (shipDateInput) {
                            const today = new Date().toISOString().split('T')[0];
                            shipDateInput.value = today;
                        }
                        
                        // Reload the shipments list immediately
                        console.log('ASN created successfully, reloading list...');
                        await loadShipments();
                        
                        // Show success message and optionally open detail view
                        if (result.data && result.data.Id) {
                            setTimeout(() => {
                                showDetailView(result.data.Id);
                            }, 500);
                        } else {
                            alert('ASN created successfully!');
                        }
                    } else {
                        alert('Failed to create ASN: ' + (result.message || 'Unknown error'));
                        console.error('Create ASN error:', result);
                    }
                } catch (error) {
                    console.error('Create ASN error:', error);
                    alert('Error creating ASN: ' + error.message);
                } finally {
                    btn.disabled = false;
                    btn.innerHTML = originalText;
                }
            });
        }

        // Set default ship date to today
        const shipDateInput = document.getElementById('createShipDate');
        if (shipDateInput) {
            const today = new Date().toISOString().split('T')[0];
            shipDateInput.value = today;
        }
    }

    // Initialize create ASN on page load
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initializeCreateAsn);
    } else {
        initializeCreateAsn();
    }

    // Make functions globally accessible
    window.viewShipmentDetails = showDetailView;
    
    window.deleteShipment = async function(id, asnNumber) {
        if (!confirm(`Are you sure you want to delete ASN ${asnNumber}?`)) {
            return;
        }

        try {
            const response = await fetch('/Distribution/DeleteAsnShipment', {
                method: 'POST',
                credentials: 'include',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ id: id })
            });

            const result = await response.json();

            if (result.success) {
                alert('ASN deleted successfully');
                loadShipments();
            } else {
                alert('Failed to delete ASN: ' + result.message);
            }
        } catch (error) {
            console.error('Delete error:', error);
            alert('Error deleting ASN: ' + error.message);
        }
    };

    // Edit functions - these will be implemented with modals
    window.editShipment = function(shipmentId) {
        // Load shipment and show edit modal
        fetch(`/Distribution/GetAsnShipment?id=${shipmentId}`)
            .then(r => r.json())
            .then(result => {
                if (result.success) {
                    showEditShipmentModal(result.data);
                } else {
                    alert('Failed to load shipment: ' + result.message);
                }
            })
            .catch(error => {
                console.error('Error loading shipment:', error);
                alert('Error loading shipment: ' + error.message);
            });
    };

    window.editPallet = function(shipmentId, palletId) {
        fetch(`/Distribution/GetAsnShipment?id=${shipmentId}`)
            .then(r => r.json())
            .then(result => {
                if (result.success) {
                    const pallet = result.data.pallets.find(p => p.id === palletId);
                    if (pallet) {
                        showEditPalletModal(shipmentId, pallet);
                    } else {
                        alert('Pallet not found');
                    }
                } else {
                    alert('Failed to load shipment: ' + result.message);
                }
            })
            .catch(error => {
                console.error('Error loading shipment:', error);
                alert('Error loading shipment: ' + error.message);
            });
    };

    window.editLineItem = function(shipmentId, palletId, lineItemId) {
        fetch(`/Distribution/GetAsnShipment?id=${shipmentId}`)
            .then(r => r.json())
            .then(result => {
                if (result.success) {
                    const pallet = result.data.pallets.find(p => p.id === palletId);
                    if (pallet) {
                        const lineItem = pallet.lineItems.find(li => li.id === lineItemId);
                        if (lineItem) {
                            showEditLineItemModal(shipmentId, palletId, lineItem);
                        } else {
                            alert('Line item not found');
                        }
                    } else {
                        alert('Pallet not found');
                    }
                } else {
                    alert('Failed to load shipment: ' + result.message);
                }
            })
            .catch(error => {
                console.error('Error loading shipment:', error);
                alert('Error loading shipment: ' + error.message);
            });
    };

    window.addPallet = function(shipmentId) {
        showEditPalletModal(shipmentId, null);
    };

    window.addLineItem = function(shipmentId, palletId) {
        showEditLineItemModal(shipmentId, palletId, null);
    };

    function showEditShipmentModal(shipment) {
        // Simple implementation - show alert with instructions
        const isSimulated = confirm(`Edit ASN ${shipment.asnNumber}\n\n` +
            `Current Status: ${shipment.status}\n` +
            `Simulated: ${shipment.isSimulated ? 'Yes' : 'No'}\n\n` +
            `Would you like to mark this as Simulated?\n\n` +
            `(Full edit functionality coming soon - use API for now)`);
        
        if (isSimulated !== null) {
            updateShipmentSimulated(shipment.id, isSimulated);
        }
    }

    function moveModalToBody(modalId) {
        var modalEl = document.getElementById(modalId);
        if (modalEl && modalEl.parentNode !== document.body) {
            _modalRestoreParents[modalId] = modalEl.parentNode;
            document.body.appendChild(modalEl);
        }
    }

    function showEditPalletModal(shipmentId, pallet) {
        var titleEl = document.querySelector('#editPalletModal .modal-title');
        if (titleEl) titleEl.innerHTML = pallet ? '<i class="bi bi-pencil"></i> Edit Pallet' : '<i class="bi bi-plus-circle"></i> Add Pallet';
        document.getElementById('editPalletShipmentId').value = shipmentId;
        document.getElementById('editPalletId').value = pallet ? pallet.id : '0';
        document.getElementById('editPalletSscc').value = pallet ? (pallet.sscc || '') : '';
        document.getElementById('editPalletPackageType').value = pallet ? (pallet.packageTypeCode || 'PLT') : 'PLT';
        document.getElementById('editPalletGrossWeight').value = pallet && pallet.grossWeight != null ? pallet.grossWeight : '';
        document.getElementById('editPalletDestName').value = pallet ? (pallet.destinationName || '') : '';
        document.getElementById('editPalletDestAddress').value = pallet ? (pallet.destinationAddress || '') : '';
        document.getElementById('editPalletDestCity').value = pallet ? (pallet.destinationCity || '') : '';
        document.getElementById('editPalletDestPostalCode').value = pallet ? (pallet.destinationPostalCode || '') : '';
        document.getElementById('editPalletDestCountryCode').value = pallet ? (pallet.destinationCountryCode || '') : '';
        document.getElementById('editPalletDestGln').value = pallet ? (pallet.destinationGln || '') : '';
        document.getElementById('editPalletIsSimulated').checked = pallet ? !!pallet.isSimulated : false;
        moveModalToBody('editPalletModal');
        var modalEl = document.getElementById('editPalletModal');
        var modal = typeof bootstrap !== 'undefined' && bootstrap.Modal ? new bootstrap.Modal(modalEl) : null;
        if (modal) modal.show();
    }

    function showEditLineItemModal(shipmentId, palletId, lineItem) {
        var titleEl = document.querySelector('#editLineItemModal .modal-title');
        if (titleEl) titleEl.innerHTML = lineItem ? '<i class=\"bi bi-pencil\"></i> Edit Line Item' : '<i class=\"bi bi-plus-circle\"></i> Add Line Item';
        document.getElementById('editLineItemShipmentId').value = shipmentId;
        document.getElementById('editLineItemPalletId').value = palletId;
        document.getElementById('editLineItemId').value = lineItem ? lineItem.id : '0';
        document.getElementById('editLineItemGtin').value = lineItem ? (lineItem.gtin || '') : '';
        document.getElementById('editLineItemDescription').value = lineItem ? (lineItem.description || '') : '';
        document.getElementById('editLineItemQuantity').value = lineItem && lineItem.quantity != null ? lineItem.quantity : '';
        document.getElementById('editLineItemUnitOfMeasure').value = lineItem ? (lineItem.unitOfMeasure || 'EA') : 'EA';
        document.getElementById('editLineItemLineNumber').value = lineItem ? (lineItem.lineNumber || 1) : 1;
        document.getElementById('editLineItemBatchNumber').value = lineItem ? (lineItem.batchNumber || '') : '';
        var bestBefore = '';
        if (lineItem && lineItem.bestBeforeDate) {
            try {
                var d = new Date(lineItem.bestBeforeDate);
                bestBefore = d.toISOString().slice(0, 10);
            } catch (e) {}
        }
        document.getElementById('editLineItemBestBeforeDate').value = bestBefore;
        document.getElementById('editLineItemPoLineRef').value = lineItem ? (lineItem.poLineReference || '') : '';
        document.getElementById('editLineItemSupplierArticle').value = lineItem ? (lineItem.supplierArticleNumber || '') : '';
        document.getElementById('editLineItemIsSimulated').checked = lineItem ? !!lineItem.isSimulated : false;
        moveModalToBody('editLineItemModal');
        var modalEl = document.getElementById('editLineItemModal');
        var modal = typeof bootstrap !== 'undefined' && bootstrap.Modal ? new bootstrap.Modal(modalEl) : null;
        if (modal) modal.show();
    }

    window.moveDistributionModalToBody = moveModalToBody;

    async function updateShipmentSimulated(shipmentId, isSimulated) {
        try {
            const response = await fetch('/Distribution/UpdateAsnShipment', {
                method: 'POST',
                credentials: 'include',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ Id: shipmentId, IsSimulated: isSimulated })
            });
            const result = await response.json();
            if (result.success) {
                alert('ASN updated successfully');
                showDetailView(shipmentId);
            } else {
                alert('Failed to update: ' + result.message);
            }
        } catch (error) {
            console.error('Update error:', error);
            alert('Error updating: ' + error.message);
        }
    }

    async function updatePalletSimulated(shipmentId, palletId, isSimulated) {
        try {
            const response = await fetch('/Distribution/UpdateAsnShipment', {
                method: 'POST',
                credentials: 'include',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    Id: shipmentId,
                    Pallets: [{ Id: palletId, IsSimulated: isSimulated }]
                })
            });
            const result = await response.json();
            if (result.success) {
                alert('Pallet updated successfully');
                showDetailView(shipmentId);
            } else {
                alert('Failed to update: ' + result.message);
            }
        } catch (error) {
            console.error('Update error:', error);
            alert('Error updating: ' + error.message);
        }
    }

    async function updateLineItemSimulated(shipmentId, palletId, lineItemId, isSimulated) {
        try {
            const response = await fetch('/Distribution/UpdateAsnShipment', {
                method: 'POST',
                credentials: 'include',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    Id: shipmentId,
                    Pallets: [{
                        Id: palletId,
                        LineItems: [{ Id: lineItemId, IsSimulated: isSimulated }]
                    }]
                })
            });
            const result = await response.json();
            if (result.success) {
                alert('Line item updated successfully');
                showDetailView(shipmentId);
            } else {
                alert('Failed to update: ' + result.message);
            }
        } catch (error) {
            console.error('Update error:', error);
            alert('Error updating: ' + error.message);
        }
    }

    async function addNewPallet(shipmentId, isSimulated) {
        const sscc = prompt('Enter SSCC (or leave blank to auto-generate):');
        const destinationName = prompt('Enter destination name:');
        const destinationGln = prompt('Enter destination GLN (optional):');
        
        if (!destinationName) {
            alert('Destination name is required');
            return;
        }

        try {
            const response = await fetch('/Distribution/UpdateAsnShipment', {
                method: 'POST',
                credentials: 'include',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    Id: shipmentId,
                    Pallets: [{
                        Id: 0,
                        Sscc: sscc || null,
                        DestinationName: destinationName,
                        DestinationGln: destinationGln || null,
                        SequenceNumber: 1,
                        IsSimulated: isSimulated
                    }]
                })
            });
            const result = await response.json();
            if (result.success) {
                alert('Pallet added successfully');
                showDetailView(shipmentId);
            } else {
                alert('Failed to add pallet: ' + result.message);
            }
        } catch (error) {
            console.error('Add pallet error:', error);
            alert('Error adding pallet: ' + error.message);
        }
    }

    async function addNewLineItem(shipmentId, palletId, isSimulated) {
        const gtin = prompt('Enter GTIN:');
        const description = prompt('Enter description:');
        const quantity = prompt('Enter quantity:');
        
        if (!gtin || !description || !quantity) {
            alert('GTIN, description, and quantity are required');
            return;
        }

        try {
            const response = await fetch('/Distribution/UpdateAsnShipment', {
                method: 'POST',
                credentials: 'include',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    Id: shipmentId,
                    Pallets: [{
                        Id: palletId,
                        LineItems: [{
                            Id: 0,
                            LineNumber: 1,
                            Gtin: gtin,
                            Description: description,
                            Quantity: parseFloat(quantity),
                            UnitOfMeasure: 'PCE',
                            IsSimulated: isSimulated
                        }]
                    }]
                })
            });
            const result = await response.json();
            if (result.success) {
                alert('Line item added successfully');
                showDetailView(shipmentId);
            } else {
                alert('Failed to add line item: ' + result.message);
            }
        } catch (error) {
            console.error('Add line item error:', error);
            alert('Error adding line item: ' + error.message);
        }
    }

})();
