(function() {
    'use strict';
        const init = window.PACKAGING_INIT || {};
        let currentType = init.type || 'supply-chain-matrix';
        let currentPage = init.page ?? 1;
        let currentPageSize = init.pageSize ?? 25;
        let currentSortBy = init.sortBy || 'name';
        let currentSortDir = init.sortDir || 'asc';
        let currentFilter = init.filter || '';
        let selectedId = null;
        let selectedSupplierId = null;
        let selectedSupplierName = null;
        let isHorizontalLayout = false;
        let navigationHistory = [];
        let dataCache = {};
        const CACHE_TTL_MS = 5 * 60 * 1000;
        const visualEditorUrl = init.visualEditorUrl || '/VisualEditor?embedded=1';

        // Initialize (run immediately if doc already loaded, e.g. when opened in browser tab)
        let matrixPage = 1;
        let matrixPageSize = 50;
        let matrixFilter = '';
        let matrixGroupBy = 'group';
        let matrixSortBy = 'material';
        let matrixSortDir = 'asc';
        let matrixLayerFilter = { primary: true, secondary: true, tertiary: true };
        let matrixData = null;
        let matrixOptionsCache = null;

        function initPackaging() {
            if (currentType === 'supply-chain-matrix') {
                loadMatrixData();
                loadData('packaging-items');
            } else if (currentType === 'packaging-flow') {
                loadPackagingFlowData();
                setupPackagingFlowEvents();
            } else if (currentType !== 'visual-editor' && currentType !== 'packaging-taxonomy') {
                updateFilterPlaceholder();
                updateFilterBadge();
                loadData();
            } else {
                const frame = document.getElementById('visualEditorFrame');
                if (frame) {
                    const needsLoad = !frame.src || frame.src === 'about:blank' || !frame.src.includes('VisualEditor');
                    if (needsLoad) frame.src = visualEditorUrl;
                }
            }
            setupEventListeners();
            setupSupplierModals();
            setupAddModals();
            setupTabSwitching();
            setupDetailNavigation();
            setupEditDeleteButtons();
            setupMatrixEventListeners();
        }
        window.switchPackagingTab = switchTab;
        window.navigateToRecord = navigateToRecord;
        window.selectRecord = selectRecord;
        window.tableRowKeydown = tableRowKeydown;
        window.sortTable = sortTable;
        window.changePage = changePage;

        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', initPackaging);
        } else {
            try { initPackaging(); } catch (e) {
                console.error('[Packaging Management] Init error:', e);
            }
        }

        function setupTabSwitching() {
            const tabsEl = document.getElementById('packagingManagementTabs');
            if (!tabsEl) return;
            tabsEl.addEventListener('click', function(e) {
                const link = e.target.closest('a[data-type]');
                if (!link) return;
                e.preventDefault();
                e.stopPropagation();
                const type = link.getAttribute('data-type');
                if (type === currentType) return;
                switchTab(type);
            });
            window.switchPackagingTab = switchTab;
        }

        function switchTab(type) {
            currentType = type;
            currentPage = 1;
            try {
                const url = window.location.pathname + '?type=' + encodeURIComponent(type);
                window.history.replaceState({ type: type }, '', url);
            } catch (e) { /* same-origin only */ }
            selectedId = null;
            selectedSupplierId = null;
            navigationHistory = [];
            showEmptyDetailState();

            document.querySelectorAll('#packagingManagementTabs .nav-link').forEach(l => {
                l.classList.toggle('active', l.getAttribute('data-type') === type);
            });

            const visualContainer = document.getElementById('visualEditorContainer');
            const splitContainer = document.getElementById('splitContainer');
            const headerButtons = document.getElementById('packagingHeaderButtons');
            const addSupplierBtn = document.getElementById('addSupplierBtn');
            const addRawMaterialBtn = document.getElementById('addRawMaterialBtn');
            const addPackagingItemBtn = document.getElementById('addPackagingItemBtn');
            const addPackagingGroupBtn = document.getElementById('addPackagingGroupBtn');
            const addProductBtn = document.getElementById('addProductBtn');

            const taxonomyContainer = document.getElementById('taxonomyTreeContainer');
            const tablePane = document.getElementById('tablePane');
            const detailPane = document.getElementById('detailPane');

            const matrixContainer = document.getElementById('matrixContainer');
            const exportCsvBtn = document.getElementById('exportMatrixCsvBtn');
            const addMatrixRowBtn = document.getElementById('addMatrixRowBtn');

            if (type === 'visual-editor') {
                visualContainer.classList.remove('d-none');
                splitContainer.classList.add('d-none');
                if (matrixContainer) matrixContainer.classList.add('d-none');
                const flowContainer = document.getElementById('packagingFlowContainer');
                if (flowContainer) flowContainer.classList.add('d-none');
                if (headerButtons) headerButtons.classList.add('d-none');
                const frame = document.getElementById('visualEditorFrame');
                if (frame) {
                    const needsLoad = !frame.src || frame.src === 'about:blank' || !frame.src.includes('VisualEditor');
                    if (needsLoad) frame.src = visualEditorUrl;
                }
            } else if (type === 'supply-chain-matrix') {
                visualContainer.classList.add('d-none');
                splitContainer.classList.remove('d-none');
                if (matrixContainer) matrixContainer.classList.remove('d-none');
                const flowContainer = document.getElementById('packagingFlowContainer');
                if (flowContainer) flowContainer.classList.add('d-none');
                if (headerButtons) headerButtons.classList.remove('d-none');
                if (addSupplierBtn) addSupplierBtn.classList.add('d-none');
                if (addRawMaterialBtn) addRawMaterialBtn.classList.add('d-none');
                if (addPackagingItemBtn) addPackagingItemBtn.classList.add('d-none');
                if (addPackagingGroupBtn) addPackagingGroupBtn.classList.add('d-none');
                if (addProductBtn) addProductBtn.classList.add('d-none');
                if (exportCsvBtn) exportCsvBtn.classList.remove('d-none');
                if (addMatrixRowBtn) addMatrixRowBtn.classList.remove('d-none');
                if (taxonomyContainer) taxonomyContainer.classList.add('d-none');
                if (tablePane) tablePane.classList.remove('d-none');
                if (detailPane) detailPane.classList.remove('d-none');
                const resizeHandle = document.getElementById('splitResizeHandle');
                if (resizeHandle) resizeHandle.classList.remove('d-none');
                document.getElementById('tablePaneTitle').textContent = 'Packaging Items';
                document.getElementById('detailPaneTitle').textContent = 'Packaging Item';
                updateFilterPlaceholder();
                updateFilterBadge();
                loadMatrixData();
                loadData('packaging-items');
            } else if (type === 'packaging-flow') {
                visualContainer.classList.add('d-none');
                splitContainer.classList.add('d-none');
                if (matrixContainer) matrixContainer.classList.add('d-none');
                const flowContainer = document.getElementById('packagingFlowContainer');
                if (flowContainer) flowContainer.classList.remove('d-none');
                if (headerButtons) headerButtons.classList.remove('d-none');
                if (addSupplierBtn) addSupplierBtn.classList.add('d-none');
                if (addRawMaterialBtn) addRawMaterialBtn.classList.add('d-none');
                if (addPackagingItemBtn) addPackagingItemBtn.classList.add('d-none');
                if (addPackagingGroupBtn) addPackagingGroupBtn.classList.add('d-none');
                if (addProductBtn) addProductBtn.classList.add('d-none');
                if (exportCsvBtn) exportCsvBtn.classList.add('d-none');
                if (addMatrixRowBtn) addMatrixRowBtn.classList.add('d-none');
                loadPackagingFlowData();
                setupPackagingFlowEvents();
            } else if (type === 'packaging-taxonomy') {
                visualContainer.classList.add('d-none');
                splitContainer.classList.remove('d-none');
                if (matrixContainer) matrixContainer.classList.add('d-none');
                const flowContainer = document.getElementById('packagingFlowContainer');
                if (flowContainer) flowContainer.classList.add('d-none');
                if (headerButtons) headerButtons.classList.remove('d-none');
                if (exportCsvBtn) exportCsvBtn.classList.add('d-none');
                if (addMatrixRowBtn) addMatrixRowBtn.classList.add('d-none');
                if (taxonomyContainer) taxonomyContainer.classList.remove('d-none');
                if (tablePane) tablePane.classList.add('d-none');
                if (detailPane) detailPane.classList.add('d-none');
                const resizeHandle = document.getElementById('splitResizeHandle');
                if (resizeHandle) resizeHandle.classList.add('d-none');
                loadTaxonomyTree();
            } else {
                visualContainer.classList.add('d-none');
                splitContainer.classList.remove('d-none');
                if (matrixContainer) matrixContainer.classList.add('d-none');
                const flowContainer = document.getElementById('packagingFlowContainer');
                if (flowContainer) flowContainer.classList.add('d-none');
                if (headerButtons) headerButtons.classList.remove('d-none');
                if (exportCsvBtn) exportCsvBtn.classList.add('d-none');
                if (addMatrixRowBtn) addMatrixRowBtn.classList.add('d-none');
                if (taxonomyContainer) taxonomyContainer.classList.add('d-none');
                if (tablePane) tablePane.classList.remove('d-none');
                if (detailPane) detailPane.classList.remove('d-none');
                const resizeHandle = document.getElementById('splitResizeHandle');
                if (resizeHandle) resizeHandle.classList.remove('d-none');
                if (addSupplierBtn) addSupplierBtn.classList.toggle('d-none', type !== 'suppliers');
                if (addRawMaterialBtn) addRawMaterialBtn.classList.toggle('d-none', type !== 'raw-materials');
                if (addPackagingItemBtn) addPackagingItemBtn.classList.toggle('d-none', type !== 'packaging-items');
                if (addPackagingGroupBtn) addPackagingGroupBtn.classList.toggle('d-none', type !== 'packaging-groups');
                addProductBtn.classList.toggle('d-none', type !== 'suppliers');
                addProductBtn.disabled = true;

                updateFilterPlaceholder();
                updateFilterBadge();

                const titles = { 'suppliers': 'Supplier List', 'raw-materials': 'Summary Table', 'packaging-items': 'Summary Table', 'packaging-groups': 'Summary Table' };
                const detailTitles = { 'suppliers': 'Packaging Supplier', 'raw-materials': 'Raw Material', 'packaging-items': 'Packaging Item', 'packaging-groups': 'Packaging Group' };
                document.getElementById('tablePaneTitle').textContent = titles[type] || 'Summary Table';
                document.getElementById('detailPaneTitle').textContent = detailTitles[type] || 'Record Details';

                const cacheKey = type + '-' + currentPage + '-' + currentFilter;
                if (dataCache[cacheKey] && (Date.now() - dataCache[cacheKey].ts) < CACHE_TTL_MS) {
                    renderTable(dataCache[cacheKey].data);
                    renderPagination(dataCache[cacheKey].data);
                    document.getElementById('recordCount').textContent = dataCache[cacheKey].data.totalCount || 0;
                } else {
                    loadData();
                }
            }
        }

        const FILTER_PLACEHOLDERS = {
            'suppliers': 'Filter by name, city, country...',
            'raw-materials': 'Filter by name, code...',
            'packaging-items': 'Filter by name, taxonomy code...',
            'packaging-groups': 'Filter by name, pack ID...',
            'supply-chain-matrix': 'Filter by name, taxonomy code...'
        };
        const EMPTY_DETAIL_MESSAGES = {
            'suppliers': { main: 'Select a supplier to view details.', hint: 'Add your first packaging supplier to start building your supply chain.' },
            'raw-materials': { main: 'Select a raw material to view details.', hint: 'Define raw materials (e.g. PET, HDPE) that your packaging items are made from.' },
            'packaging-items': { main: 'Select a packaging item to view details.', hint: 'Add packaging items (e.g. bottles, caps) and link them to raw materials.' },
            'packaging-groups': { main: 'Select a packaging group to view details.', hint: 'Group packaging items into packs (e.g. bottle + cap + label).' }
        };
        const EMPTY_TABLE_CTAS = {
            'suppliers': { msg: 'No suppliers yet.', cta: 'Add your first packaging supplier', btnId: 'addSupplierBtn' },
            'raw-materials': { msg: 'No raw materials defined.', cta: 'Add your first raw material', btnId: 'addRawMaterialBtn' },
            'packaging-items': { msg: 'No packaging items yet.', cta: 'Add your first packaging item', btnId: 'addPackagingItemBtn' },
            'packaging-groups': { msg: 'No packaging groups yet.', cta: 'Add your first packaging group', btnId: 'addPackagingGroupBtn' }
        };
        function updateFilterPlaceholder() {
            const input = document.getElementById('filterInput');
            if (input) input.placeholder = FILTER_PLACEHOLDERS[currentType] || 'Filter...';
        }
        function updateFilterBadge() {
            const badge = document.getElementById('filterBadge');
            const badgeText = document.getElementById('filterBadgeText');
            if (badge && badgeText) {
                if (currentFilter && currentFilter.trim()) {
                    badge.classList.remove('d-none');
                    badge.classList.add('d-flex');
                    badgeText.textContent = "'" + (currentFilter.length > 20 ? currentFilter.substring(0, 17) + '...' : currentFilter) + "'";
                } else {
                    badge.classList.add('d-none');
                    badge.classList.remove('d-flex');
                }
            }
        }
        function showEmptyDetailState() {
            const container = document.getElementById('detailContent');
            const msg = EMPTY_DETAIL_MESSAGES[currentType];
            if (container && msg) {
                container.innerHTML = '<div class="packaging-detail-empty"><p class="mb-2">' + escapeHtml(msg.main) + '</p><small class="text-muted">' + escapeHtml(msg.hint) + '</small></div>';
                container.classList.add('text-center');
            } else if (container) {
                container.innerHTML = '<div class="packaging-detail-empty text-center"><p class="mb-0">Select a record from the table to view details</p></div>';
            }
            document.getElementById('editRecordBtn')?.classList.add('d-none');
            document.getElementById('deleteRecordBtn')?.classList.add('d-none');
            document.getElementById('viewProductsBtn')?.classList.add('d-none');
        }
        function setupEventListeners() {
            // Table / Card view toggle
            const tableViewBtn = document.getElementById('tableViewBtn');
            const cardViewBtn = document.getElementById('cardViewBtn');
            if (tableViewBtn && cardViewBtn) {
                tableViewBtn.addEventListener('click', function() {
                    isCardView = false;
                    tableViewBtn.classList.add('active');
                    tableViewBtn.setAttribute('aria-pressed', 'true');
                    cardViewBtn.classList.remove('active');
                    cardViewBtn.setAttribute('aria-pressed', 'false');
                    const cacheKey = currentType + '-' + currentPage + '-' + currentFilter;
                    if (dataCache[cacheKey]) renderTable(dataCache[cacheKey].data);
                    else loadData();
                });
                cardViewBtn.addEventListener('click', function() {
                    isCardView = true;
                    cardViewBtn.classList.add('active');
                    cardViewBtn.setAttribute('aria-pressed', 'true');
                    tableViewBtn.classList.remove('active');
                    tableViewBtn.setAttribute('aria-pressed', 'false');
                    const cacheKey = currentType + '-' + currentPage + '-' + currentFilter;
                    if (dataCache[cacheKey]) renderTable(dataCache[cacheKey].data);
                    else loadData();
                });
            }
            
            // Mobile back to list (responsive)
            const mobileBackBtn = document.getElementById('mobileBackToListBtn');
            const splitContainer = document.getElementById('splitContainer');
            if (mobileBackBtn && splitContainer) {
                mobileBackBtn.addEventListener('click', function() {
                    splitContainer.classList.remove('packaging-mobile-detail-open');
                });
            }
            
            // Keyboard navigation for table
            const tablePane = document.getElementById('tablePane');
            if (tablePane) {
                tablePane.addEventListener('keydown', function(e) {
                    if (e.target.closest('input, select, textarea, button')) return;
                    const rows = Array.from(document.querySelectorAll('#tableBody tr:not([style*="display: none"])')).filter(r => !r.querySelector('td[colspan="100"]'));
                    if (rows.length === 0) return;
                    const idx = rows.findIndex(r => r.classList.contains('selected'));
                    let nextIdx = -1;
                    if (e.key === 'ArrowDown' && idx < rows.length - 1) nextIdx = idx + 1;
                    else if (e.key === 'ArrowUp' && idx > 0) nextIdx = idx - 1;
                    else if (e.key === 'Home') nextIdx = 0;
                    else if (e.key === 'End') nextIdx = rows.length - 1;
                    if (nextIdx >= 0) {
                        e.preventDefault();
                        const row = rows[nextIdx];
                        const id = row.getAttribute('data-id') || row.getAttribute('onclick')?.match(/selectRecord\((\d+)/)?.[1];
                        const typeMatch = row.getAttribute('onclick')?.match(/selectRecord\(\d+,\s*'([^']+)'/);
                        const typeOverride = typeMatch ? typeMatch[1] : null;
                        if (id) selectRecord(parseInt(id, 10), typeOverride);
                        row.setAttribute('tabindex', '0');
                        row.focus();
                    }
                });
            }
            
            // Filter input
            const filterInput = document.getElementById('filterInput');
            if (filterInput) {
                let filterTimeout;
                filterInput.addEventListener('input', function() {
                    clearTimeout(filterTimeout);
                    filterTimeout = setTimeout(() => {
                        currentFilter = this.value;
                        currentPage = 1;
                        updateFilterBadge();
                        loadData();
                    }, 300);
                });
            }

            // Clear filter
            const clearFilterBtn = document.getElementById('clearFilterBtn');
            const clearFilterBadgeBtn = document.getElementById('clearFilterBadgeBtn');
            function doClearFilter() {
                const fi = document.getElementById('filterInput');
                if (fi) fi.value = '';
                currentFilter = '';
                currentPage = 1;
                updateFilterBadge();
                loadData();
            }
            if (clearFilterBtn) clearFilterBtn.addEventListener('click', doClearFilter);
            if (clearFilterBadgeBtn) clearFilterBadgeBtn.addEventListener('click', doClearFilter);

            // Toggle layout
            const toggleLayoutBtn = document.getElementById('toggleLayoutBtn');
            if (toggleLayoutBtn) toggleLayoutBtn.addEventListener('click', function() {
                isHorizontalLayout = !isHorizontalLayout;
                const container = document.getElementById('splitContainer');
                const icon = document.getElementById('layoutIcon');
                
                if (isHorizontalLayout) {
                    container.classList.add('split-horizontal');
                    icon.className = 'bi bi-arrows-collapse';
                    toggleLayoutBtn.title = 'Switch to stacked layout';
                } else {
                    container.classList.remove('split-horizontal');
                    icon.className = 'bi bi-arrows-expand';
                    toggleLayoutBtn.title = 'Switch to side-by-side layout';
                }
                try { localStorage.setItem('packagingLayoutHorizontal', isHorizontalLayout ? '1' : '0'); } catch (e) {}
            });

            // Detail pane collapse toggle
            const detailPaneToggleBtn = document.getElementById('detailPaneToggleBtn');
            const detailPaneToggleIcon = document.getElementById('detailPaneToggleIcon');
            const detailPaneEl = document.getElementById('detailPane');
            if (detailPaneToggleBtn && detailPaneEl) {
                const collapsed = localStorage.getItem('packagingDetailCollapsed') === '1';
                if (collapsed) {
                    detailPaneEl.classList.add('collapsed');
                    if (detailPaneToggleIcon) detailPaneToggleIcon.className = 'bi bi-chevron-up';
                    detailPaneToggleBtn.title = 'Expand detail pane';
                }
                detailPaneToggleBtn.addEventListener('click', function() {
                    detailPaneEl.classList.toggle('collapsed');
                    const isCollapsed = detailPaneEl.classList.contains('collapsed');
                    if (detailPaneToggleIcon) {
                        detailPaneToggleIcon.className = isCollapsed ? 'bi bi-chevron-up' : 'bi bi-chevron-down';
                    }
                    detailPaneToggleBtn.title = isCollapsed ? 'Expand detail pane' : 'Collapse detail pane';
                    try { localStorage.setItem('packagingDetailCollapsed', isCollapsed ? '1' : '0'); } catch (e) {}
                });
            }

            // Taxonomy expand/collapse (run once; container populated when tree loads)
            const taxonomyExpandAll = document.getElementById('taxonomyExpandAll');
            const taxonomyCollapseAll = document.getElementById('taxonomyCollapseAll');
            const taxonomyTreeContent = document.getElementById('taxonomyTreeContent');
            if (taxonomyExpandAll && taxonomyTreeContent) {
                taxonomyExpandAll.addEventListener('click', function() {
                    taxonomyTreeContent.querySelectorAll('.taxonomy-tree ul').forEach(ul => ul.classList.remove('d-none'));
                    taxonomyTreeContent.querySelectorAll('.tree-toggle:not(.empty)').forEach(t => t.textContent = 'â–¼');
                    taxonomyTreeContent.querySelectorAll('.tree-node.folder .tree-icon').forEach(i => i.className = 'bi bi-folder2-open tree-icon');
                });
            }
            if (taxonomyCollapseAll && taxonomyTreeContent) {
                taxonomyCollapseAll.addEventListener('click', function() {
                    taxonomyTreeContent.querySelectorAll('.taxonomy-tree ul').forEach(ul => ul.classList.add('d-none'));
                    taxonomyTreeContent.querySelectorAll('.tree-toggle:not(.empty)').forEach(t => t.textContent = 'â–¶');
                    taxonomyTreeContent.querySelectorAll('.tree-node.folder .tree-icon').forEach(i => i.className = 'bi bi-folder2 tree-icon');
                });
            }
            const taxonomyTreeSearch = document.getElementById('taxonomyTreeSearch');
            if (taxonomyTreeSearch && taxonomyTreeContent) {
                taxonomyTreeSearch.addEventListener('input', function() {
                    const q = (this.value || '').toLowerCase().trim();
                    taxonomyTreeContent.querySelectorAll('.taxonomy-tree .tree-node').forEach(n => n.style.display = '');
                    taxonomyTreeContent.querySelectorAll('.taxonomy-tree li').forEach(li => li.style.display = '');
                    if (!q) return;
                    const allNodes = taxonomyTreeContent.querySelectorAll('.tree-node[data-nav-id]');
                    const matchingIds = new Set();
                    allNodes.forEach(n => {
                        const label = (n.querySelector('.tree-label')?.textContent || '').toLowerCase();
                        if (label.includes(q)) {
                            let el = n.closest('li');
                            while (el) {
                                const node = el.querySelector(':scope > .tree-node');
                                if (node) matchingIds.add(node);
                                el = el.parentElement?.closest('li');
                            }
                        }
                    });
                    allNodes.forEach(n => {
                        if (!matchingIds.has(n)) {
                            const li = n.closest('li');
                            if (li && !matchingIds.has(li.querySelector(':scope > .tree-node'))) li.style.display = 'none';
                        }
                    });
                });
            }

            // Resize handle
            const resizeHandle = document.getElementById('splitResizeHandle');
            if (resizeHandle && detailPaneEl) {
                let isResizing = false;
                resizeHandle.addEventListener('mousedown', function(e) {
                    e.preventDefault();
                    isResizing = true;
                    document.body.style.cursor = 'row-resize';
                    document.body.style.userSelect = 'none';
                });
                document.addEventListener('mousemove', function(e) {
                    if (!isResizing) return;
                    const container = document.getElementById('splitContainer');
                    if (!container) return;
                    const rect = container.getBoundingClientRect();
                    const containerHeight = rect.height;
                    const fromBottom = rect.bottom - e.clientY;
                    const pct = Math.max(20, Math.min(80, (fromBottom / containerHeight) * 100));
                    detailPaneEl.style.flex = '0 0 ' + pct + '%';
                    detailPaneEl.style.minHeight = '200px';
                    try { localStorage.setItem('packagingDetailHeight', String(pct)); } catch (e) {}
                });
                document.addEventListener('mouseup', function() {
                    if (isResizing) {
                        isResizing = false;
                        document.body.style.cursor = '';
                        document.body.style.userSelect = '';
                    }
                });
                try {
                    const saved = localStorage.getItem('packagingDetailHeight');
                    if (saved) {
                        const pct = parseFloat(saved);
                        if (pct >= 20 && pct <= 80) detailPaneEl.style.flex = '0 0 ' + pct + '%';
                    }
                } catch (e) {}
            }
        }

        function loadData(overrideType) {
            if (currentType === 'visual-editor') return;
            if (currentType === 'supply-chain-matrix' && !overrideType) { loadData('packaging-items'); return; }
            if (currentType === 'packaging-taxonomy') { loadTaxonomyTree(); return; }
            if (currentType === 'packaging-flow') { loadPackagingFlowData(); return; }
            const effectiveType = overrideType || currentType;
            const url = `/api/packaging-management/list/${effectiveType}?page=${currentPage}&pageSize=${currentPageSize}&sortBy=${currentSortBy}&sortDir=${currentSortDir}&filter=${encodeURIComponent(currentFilter)}`;
            
            console.log('[Packaging Management] Loading data from:', url);
            
            // Show loading skeletons
            const tbody = document.getElementById('tableBody');
            const cardView = document.getElementById('cardViewContainer');
            const dataTable = document.getElementById('dataTable');
            if (tbody) {
                const colCount = Math.max(1, document.querySelectorAll('#tableHead tr th').length) || 4;
                let skeletonRows = '';
                for (let i = 0; i < 5; i++) {
                    skeletonRows += '<tr><td colspan="' + colCount + '"><div class="d-flex gap-3"><div class="skeleton skeleton-text" style="width:20%;"></div><div class="skeleton skeleton-text" style="width:15%;"></div><div class="skeleton skeleton-text" style="width:30%;"></div></div></td></tr>';
                }
                tbody.innerHTML = skeletonRows;
            }
            if (cardView) cardView.classList.add('d-none');
            if (dataTable) dataTable.classList.remove('d-none');
            
            fetch(url)
                .then(response => {
                    console.log('[Packaging Management] Response status:', response.status);
                    if (!response.ok) {
                        throw new Error(`HTTP error! status: ${response.status}`);
                    }
                    return response.json();
                })
                .then(data => {
                    console.log('[Packaging Management] Data received:', data);
                    if (data.error) {
                        console.error('Error:', data.error);
                        if (tbody) {
                            tbody.innerHTML = `<tr><td colspan="100%" class="text-center text-danger">Error: ${data.error}</td></tr>`;
                        }
                        return;
                    }
                    
                    if (!data.items) {
                        console.warn('[Packaging Management] No items property in response:', data);
                        if (tbody) {
                            tbody.innerHTML = '<tr><td colspan="100%" class="text-center text-warning">No data structure found</td></tr>';
                        }
                        return;
                    }
                    
                    renderTable(data, effectiveType);
                    renderPagination(data);
                    const recordCountEl = document.getElementById('recordCount');
                    if (recordCountEl) {
                        recordCountEl.textContent = data.totalCount || 0;
                    }
                    const cacheKey = effectiveType + '-' + currentPage + '-' + currentFilter;
                    dataCache[cacheKey] = { data: data, ts: Date.now() };
                })
                .catch(error => {
                    console.error('[Packaging Management] Error loading data:', error);
                    if (tbody) {
                        tbody.innerHTML = `<tr><td colspan="100%" class="text-center text-danger">Error loading data: ${error.message}</td></tr>`;
                    }
                });
        }

        function loadTaxonomyTree() {
            const container = document.getElementById('taxonomyTreeContent');
            if (!container) return;
            container.innerHTML = '<div class="py-4"><div class="skeleton skeleton-text short mb-2"></div><div class="skeleton skeleton-text medium mb-2"></div><div class="skeleton skeleton-text long mb-2"></div><div class="skeleton skeleton-text short" style="width:60%;"></div></div>';
            fetch('/api/packaging-management/taxonomy-tree')
                .then(r => r.json())
                .then(data => {
                    if (data.error) {
                        container.innerHTML = '<div class="alert alert-danger">' + escapeHtml(data.error) + '</div>';
                        return;
                    }
                    container.innerHTML = renderTaxonomyTree(data.tree || [], data.orphanItems || [], data.standaloneGroups || [], data.allPackagingItems || [], data.allPackagingGroups || []);
                    container.querySelectorAll('.tree-node').forEach(el => {
                        el.addEventListener('click', function(e) {
                            const li = this.closest('li');
                            const ul = li?.querySelector(':scope > ul');
                            const hasChildren = this.getAttribute('data-has-children') === 'true';
                            const doToggle = e.target.closest('.tree-toggle') || (hasChildren && this.classList.contains('folder'));
                            if (doToggle && ul) {
                                e.stopPropagation();
                                ul.classList.toggle('d-none');
                                const toggle = this.querySelector('.tree-toggle');
                                if (toggle && !toggle.classList.contains('empty')) toggle.textContent = ul.classList.contains('d-none') ? 'â–¶' : 'â–¼';
                                const icon = this.querySelector('.tree-icon');
                                if (icon && this.classList.contains('folder')) icon.className = 'bi tree-icon ' + (ul.classList.contains('d-none') ? 'bi-folder2' : 'bi-folder2-open');
                                return;
                            }
                            const id = parseInt(this.getAttribute('data-nav-id'), 10);
                            const type = this.getAttribute('data-nav-type');
                            if (id && type && window.navigateToRecord) navigateToRecord(id, type, true);
                        });
                    });
                })
                .catch(err => {
                    container.innerHTML = '<div class="alert alert-danger">Error loading taxonomy: ' + escapeHtml(err.message) + '</div>';
                });
        }

        function getFlowPackagingLayerParam() {
            const layers = [];
            if (document.getElementById('flowFilterPrimary')?.checked) layers.push('Primary');
            if (document.getElementById('flowFilterSecondary')?.checked) layers.push('Secondary');
            if (document.getElementById('flowFilterTertiary')?.checked) layers.push('Tertiary');
            return layers.length === 3 ? '' : layers.join(',');
        }

        function loadPackagingFlowData() {
            const filter = (document.getElementById('flowFilterInput')?.value || '').trim();
            const packagingLayer = getFlowPackagingLayerParam();
            const baseParams = 'page=1&pageSize=100&sortBy=name&sortDir=asc';
            const filterParam = filter ? '&filter=' + encodeURIComponent(filter) : '';
            const layerParam = packagingLayer ? '&packagingLayer=' + encodeURIComponent(packagingLayer) : '';

            const setLoading = (bodyId) => {
                const tbody = document.getElementById(bodyId);
                if (tbody) tbody.innerHTML = '<tr><td colspan="3" class="text-center text-muted py-3"><span class="spinner-border spinner-border-sm me-1"></span>Loading...</td></tr>';
            };
            setLoading('flowBodyRawMaterials');
            setLoading('flowBodyPackagingItems');
            setLoading('flowBodyPackagingGroups');

            Promise.all([
                fetch('/api/packaging-management/list/raw-materials?' + baseParams + filterParam + layerParam).then(r => r.json()),
                fetch('/api/packaging-management/list/packaging-items?' + baseParams + filterParam + layerParam).then(r => r.json()),
                fetch('/api/packaging-management/list/packaging-groups?' + baseParams + filterParam + layerParam).then(r => r.json())
            ]).then(([rmData, piData, pgData]) => {
                renderFlowColumn('flowBodyRawMaterials', rmData, 'raw-materials', (item) => {
                    const productsBtn = '<button type="button" class="btn btn-sm btn-link p-0 text-secondary flow-products-btn" title="View products" data-id="' + item.id + '" data-type="raw-materials"><i class="bi bi-link-45deg"></i></button>';
                    return '<tr class="flow-row" data-id="' + item.id + '" data-type="raw-materials"><td>' + escapeHtml(item.name || '-') + '</td><td>' + escapeHtml(item.code || '-') + '</td><td>' + productsBtn + '</td></tr>';
                });
                renderFlowColumn('flowBodyPackagingItems', piData, 'packaging-items', (item) => {
                    const productsBtn = '<button type="button" class="btn btn-sm btn-link p-0 text-secondary flow-products-btn" title="View products" data-id="' + item.id + '" data-type="packaging-items"><i class="bi bi-link-45deg"></i></button>';
                    return '<tr class="flow-row" data-id="' + item.id + '" data-type="packaging-items"><td>' + escapeHtml(item.name || '-') + '</td><td>' + escapeHtml(item.taxonomyCode || '-') + '</td><td>' + productsBtn + '</td></tr>';
                });
                renderFlowColumn('flowBodyPackagingGroups', pgData, 'packaging-groups', (item) => {
                    const productsBtn = '<button type="button" class="btn btn-sm btn-link p-0 text-secondary flow-products-btn" title="View products" data-id="' + item.id + '" data-type="packaging-groups"><i class="bi bi-link-45deg"></i></button>';
                    return '<tr class="flow-row" data-id="' + item.id + '" data-type="packaging-groups"><td>' + escapeHtml(item.name || '-') + '</td><td>' + escapeHtml(item.packagingLayer || '-') + '</td><td>' + productsBtn + '</td></tr>';
                });
            }).catch(err => {
                console.error('[Packaging Flow] Error:', err);
                ['flowBodyRawMaterials', 'flowBodyPackagingItems', 'flowBodyPackagingGroups'].forEach(bodyId => {
                    const tbody = document.getElementById(bodyId);
                    if (tbody) tbody.innerHTML = '<tr><td colspan="3" class="text-center text-danger py-3">Error loading data</td></tr>';
                });
            });
        }

        function renderFlowColumn(bodyId, data, type, rowRenderer) {
            const tbody = document.getElementById(bodyId);
            if (!tbody) return;
            if (data.error) {
                tbody.innerHTML = '<tr><td colspan="3" class="text-center text-danger py-3">' + escapeHtml(data.error) + '</td></tr>';
                return;
            }
            const items = data.items || [];
            if (items.length === 0) {
                tbody.innerHTML = '<tr><td colspan="3" class="text-center text-muted py-3">No records</td></tr>';
                return;
            }
            tbody.innerHTML = items.map(rowRenderer).join('');
            if (!tbody.dataset.flowDelegated) {
                tbody.dataset.flowDelegated = '1';
                tbody.addEventListener('click', function(e) {
                    const btn = e.target.closest('.flow-products-btn');
                    if (btn) {
                        e.stopPropagation();
                        const id = parseInt(btn.getAttribute('data-id'), 10);
                        const t = btn.getAttribute('data-type');
                        if (id && t && window.showProductAssociations) {
                            if (t === 'raw-materials') window.showProductAssociations(id, null, null, 'rawMaterial');
                            else if (t === 'packaging-items') window.showProductAssociations(null, id, null, 'packagingItem');
                            else if (t === 'packaging-groups') window.showProductAssociations(null, null, id, 'packagingGroup');
                        }
                        return;
                    }
                    const row = e.target.closest('.flow-row');
                    if (row) {
                        const id = parseInt(row.getAttribute('data-id'), 10);
                        const t = row.getAttribute('data-type');
                        if (id && t && window.switchPackagingTab) {
                            switchPackagingTab(t);
                            setTimeout(() => selectRecord(id, t), 50);
                        }
                    }
                });
            }
        }

        function setupPackagingFlowEvents() {
            const flowFilterInput = document.getElementById('flowFilterInput');
            if (flowFilterInput && !flowFilterInput.dataset.flowSetup) {
                flowFilterInput.dataset.flowSetup = '1';
                let to;
                flowFilterInput.addEventListener('input', function() {
                    clearTimeout(to);
                    to = setTimeout(loadPackagingFlowData, 300);
                });
            }
            document.querySelectorAll('#flowFilterPrimary, #flowFilterSecondary, #flowFilterTertiary').forEach(cb => {
                if (cb && !cb.dataset.flowSetup) {
                    cb.dataset.flowSetup = '1';
                    cb.addEventListener('change', loadPackagingFlowData);
                }
            });
            document.querySelectorAll('.flow-column-toggle').forEach(btn => {
                if (btn.dataset.flowSetup) return;
                btn.dataset.flowSetup = '1';
                const col = btn.closest('.packaging-flow-column');
                if (!col) return;
                const key = col.getAttribute('data-column');
                let stored = {};
                try {
                    stored = JSON.parse(sessionStorage.getItem('packaging-flow-columns') || '{}');
                } catch (e) {}
                if (stored[key] === false) {
                    col.classList.add('collapsed');
                    const icon = btn.querySelector('i');
                    if (icon) icon.className = 'bi bi-chevron-right';
                    btn.title = 'Expand column';
                }
                btn.addEventListener('click', function() {
                    col.classList.toggle('collapsed');
                    stored[key] = !col.classList.contains('collapsed');
                    const icon = btn.querySelector('i');
                    if (icon) {
                        icon.className = col.classList.contains('collapsed') ? 'bi bi-chevron-right' : 'bi bi-chevron-left';
                    }
                    btn.title = col.classList.contains('collapsed') ? 'Expand column' : 'Collapse column';
                    try { sessionStorage.setItem('packaging-flow-columns', JSON.stringify(stored)); } catch (e) {}
                });
            });
        }

        function renderTaxonomyTree(tree, orphanItems, standaloneGroups, allPackagingItems, allPackagingGroups) {
            function countNodes(node) {
                let n = 1;
                (node.children || []).forEach(c => n += countNodes(c));
                (node.packagingItems || []).forEach(() => n++);
                (node.packagingGroups || []).forEach(() => n++);
                return n;
            }
            function renderNode(node, depth, isExpanded) {
                const childList = node.children || [];
                const itemList = node.packagingItems || [];
                const groupList = node.packagingGroups || [];
                const hasChildren = childList.length > 0 || itemList.length > 0 || groupList.length > 0;
                const isFolder = node.type === 'raw-material' || node.type === 'folder';
                const nodeClass = node.type === 'raw-material' || node.type === 'folder' ? 'raw-material folder' : node.type === 'packaging-item' ? 'packaging-item' : 'packaging-group';
                const icon = isFolder ? (isExpanded ? 'bi-folder2-open' : 'bi-folder2') : node.type === 'packaging-item' ? 'bi-box-seam' : 'bi-collection';
                const badge = node.code ? ` (${escapeHtml(node.code)})` : node.taxonomyCode ? ` (${escapeHtml(node.taxonomyCode)})` : node.packId ? ` (${escapeHtml(node.packId)})` : '';
                const weightStr = node.weight ? ' ' + node.weight + 'g' : '';
                const expanded = isExpanded !== false && hasChildren;
                let html = '<li><div class="tree-node ' + nodeClass + '" data-nav-id="' + (node.id || '') + '" data-nav-type="' + (node.type || '') + '" data-has-children="' + hasChildren + '" role="button" tabindex="0">';
                html += '<span class="tree-toggle' + (hasChildren ? '' : ' empty') + '">' + (hasChildren ? (expanded ? 'â–¼' : 'â–¶') : '') + '</span>';
                html += '<i class="bi ' + icon + ' tree-icon"></i>';
                html += '<span class="tree-label">' + escapeHtml(node.name || '') + weightStr + '</span>';
                if (badge) html += '<span class="tree-badge">' + badge + '</span>';
                html += '</div>';
                if (hasChildren && expanded) {
                    html += '<ul>';
                    childList.forEach(c => html += renderNode(c, depth + 1, true));
                    itemList.forEach(pi => html += renderNode(pi, depth + 1, false));
                    groupList.forEach(g => html += renderNode(g, depth + 1, false));
                    html += '</ul>';
                } else if (hasChildren) {
                    html += '<ul class="d-none">';
                    childList.forEach(c => html += renderNode(c, depth + 1, false));
                    itemList.forEach(pi => html += renderNode(pi, depth + 1, false));
                    groupList.forEach(g => html += renderNode(g, depth + 1, false));
                    html += '</ul>';
                }
                html += '</li>';
                return html;
            }
            let html = '<ul class="taxonomy-tree">';
            html += '<li><div class="tree-node folder" data-has-children="true" role="button" tabindex="0">';
            html += '<span class="tree-toggle">â–¼</span><i class="bi bi-folder2-open tree-icon"></i><span class="tree-label">Packaging Hierarchy</span></div>';
            html += '<ul>';
            const rawCount = tree.reduce((acc, n) => acc + countNodes(n), 0);
            html += '<li><div class="tree-node folder" data-has-children="true" role="button" tabindex="0">';
            html += '<span class="tree-toggle">â–¼</span><i class="bi bi-folder2-open tree-icon"></i><span class="tree-label">Raw materials</span><span class="tree-badge">(' + rawCount + ')</span></div><ul>';
            tree.forEach(n => html += renderNode(n, 0, true));
            html += '</ul></li>';
            if (orphanItems && orphanItems.length > 0) {
                html += '<li><div class="tree-node folder" data-has-children="true" role="button" tabindex="0">';
                html += '<span class="tree-toggle">â–¼</span><i class="bi bi-folder2-open tree-icon"></i><span class="tree-label">Packaging items (no raw material)</span><span class="tree-badge">(' + orphanItems.length + ')</span></div><ul>';
                orphanItems.forEach(n => html += renderNode(n, 0, false));
                html += '</ul></li>';
            }
            if (standaloneGroups && standaloneGroups.length > 0) {
                html += '<li><div class="tree-node folder" data-has-children="true" role="button" tabindex="0">';
                html += '<span class="tree-toggle">â–¼</span><i class="bi bi-folder2-open tree-icon"></i><span class="tree-label">Standalone packaging groups</span></div><ul>';
                standaloneGroups.forEach(g => html += renderNode(g, 0, false));
                html += '</ul></li>';
            }
            if (allPackagingItems && allPackagingItems.length > 0) {
                html += '<li><div class="tree-node folder" data-has-children="true" role="button" tabindex="0">';
                html += '<span class="tree-toggle">â–¶</span><i class="bi bi-folder2 tree-icon"></i><span class="tree-label">All packaging items</span><span class="tree-badge">(' + allPackagingItems.length + ')</span></div><ul class="d-none">';
                allPackagingItems.forEach(n => html += renderNode(n, 0, false));
                html += '</ul></li>';
            }
            if (allPackagingGroups && allPackagingGroups.length > 0) {
                html += '<li><div class="tree-node folder" data-has-children="true" role="button" tabindex="0">';
                html += '<span class="tree-toggle">â–¶</span><i class="bi bi-folder2 tree-icon"></i><span class="tree-label">All packaging groups</span><span class="tree-badge">(' + allPackagingGroups.length + ')</span></div><ul class="d-none">';
                allPackagingGroups.forEach(g => html += renderNode(g, 0, false));
                html += '</ul></li>';
            }
            html += '</ul></li></ul>';
            return html;
        }

        let isCardView = false;
        function renderTable(data, effectiveType) {
            const typeForRender = effectiveType || currentType;
            const tbody = document.getElementById('tableBody');
            const thead = document.getElementById('tableHead');
            const cardView = document.getElementById('cardViewContainer');
            const dataTable = document.getElementById('dataTable');
            
            if (!data.items || data.items.length === 0) {
                const emptyCta = EMPTY_TABLE_CTAS[typeForRender];
                if (emptyCta) {
                    const btn = document.getElementById(emptyCta.btnId);
                    const btnHtml = btn ? '<button type="button" class="btn btn-primary mt-3" onclick="document.getElementById(\'' + emptyCta.btnId + '\').click()"><i class="bi bi-plus-lg"></i> ' + escapeHtml(emptyCta.cta) + '</button>' : '';
                    tbody.innerHTML = '<tr><td colspan="100%" class="text-center py-5"><p class="text-muted mb-0">' + escapeHtml(emptyCta.msg) + '</p>' + btnHtml + '</td></tr>';
                } else {
                    tbody.innerHTML = '<tr><td colspan="100%" class="text-center text-muted py-4">No records found</td></tr>';
                }
                thead.innerHTML = '<tr></tr>';
                if (cardView) cardView.classList.add('d-none');
                if (dataTable) dataTable.classList.remove('d-none');
                return;
            }

            // Determine headers based on type
            let headers = [];
            if (typeForRender === 'raw-materials') {
                headers = [
                    { key: 'name', label: 'Name', sortable: true },
                    { key: 'code', label: 'Code', sortable: true },
                    { key: 'description', label: 'Description', sortable: true },
                    { key: 'packagingItems', label: 'Used in Packaging Items', sortable: false }
                ];
            } else if (typeForRender === 'packaging-items') {
                headers = [
                    { key: 'name', label: 'Name', sortable: true },
                    { key: 'taxonomyCode', label: 'Taxonomy Code', sortable: true },
                    { key: 'materialTaxonomyName', label: 'Raw Materials', sortable: true },
                    { key: 'weight', label: 'Weight (g)', sortable: true },
                    { key: 'supplyChain', label: 'Supply Chain', sortable: false },
                    { key: 'groups', label: 'Packaging Groups', sortable: false }
                ];
            } else if (typeForRender === 'packaging-groups') {
                headers = [
                    { key: 'name', label: 'Name', sortable: true },
                    { key: 'packId', label: 'Pack ID', sortable: true },
                    { key: 'packagingLayer', label: 'Layer', sortable: true },
                    { key: 'style', label: 'Style', sortable: false },
                    { key: 'totalPackWeight', label: 'Weight (g)', sortable: true },
                    { key: 'items', label: 'Items & Supply Chain', sortable: false }
                ];
            } else if (typeForRender === 'suppliers') {
                headers = [
                    { key: 'name', label: 'Name', sortable: true },
                    { key: 'city', label: 'City', sortable: true },
                    { key: 'state', label: 'State', sortable: true },
                    { key: 'country', label: 'Country', sortable: true },
                    { key: 'email', label: 'Email', sortable: true },
                    { key: 'contactCount', label: 'Contacts', sortable: false },
                    { key: 'productCount', label: 'Packaging Products', sortable: false }
                ];
            }

            // Render headers
            thead.innerHTML = '<tr role="row">' + headers.map((h, i) => {
                let html = `<th role="columnheader"`;
                if (h.sortable) {
                    html += ` class="sortable ${currentSortBy === h.key ? (currentSortDir === 'asc' ? 'sort-asc' : 'sort-desc') : ''}"`;
                    html += ` onclick="sortTable('${h.key}')"`;
                    html += ` onkeydown="if(event.key==='Enter'||event.key===' ') { event.preventDefault(); sortTable('${h.key}'); }"`;
                    const sortState = currentSortBy === h.key ? (currentSortDir === 'asc' ? 'ascending' : 'descending') : 'none';
                    html += ` aria-sort="${sortState}" tabindex="0"`;
                }
                html += `>${h.label}</th>`;
                return html;
            }).join('') + '</tr>';

            // Render rows
            tbody.innerHTML = data.items.map(item => {
                // Determine if this is a child row
                const isChild = item.isChild === true;
                const depth = item.depth || 0;
                const indentPx = depth * 2; // 2rem per level
                
                // Determine the type for detail loading - child rows in raw-materials are always raw-materials
                const detailType = (typeForRender === 'raw-materials' && isChild) ? 'raw-materials' : typeForRender;
                
                let row = '<tr onclick="selectRecord(' + item.id + ', \'' + detailType + '\')" onkeydown="tableRowKeydown(event, ' + item.id + ', \'' + detailType + '\')" data-id="' + item.id + '" role="row" tabindex="0"';
                if (selectedId === item.id) {
                    row += ' class="selected" aria-selected="true"';
                }
                if (isChild) {
                    row += ' class="child-row" style="background-color: #f8f9fa;"';
                }
                row += '>';
                
                headers.forEach(header => {
                    let cellContent = '';
                    
                    // For child rows, indent based on depth
                    if (header.key === 'name' && isChild) {
                        const indentSymbol = 'â†³ ';
                        const indent = indentSymbol.repeat(Math.min(depth, 5)); // Up to 5 levels
                        cellContent = `<span style="padding-left: ${indentPx}rem;">${indent}${escapeHtml(item.name || '-')}</span>`;
                    }
                    else if (header.key === 'packagingItems' && item.packagingItems && Array.isArray(item.packagingItems)) {
                        // Show packaging items as badges (parents for raw materials)
                        if (item.packagingItems.length === 0) {
                            cellContent = '<span class="text-muted">-</span>';
                        } else {
                            cellContent = item.packagingItems.map(p => 
                                `<span class="badge bg-info me-1">${escapeHtml(p.name)}</span>`
                            ).join('');
                        }
                    } else if (header.key === 'groups' && item.groups && Array.isArray(item.groups)) {
                        // Show groups as badges (parents for packaging items)
                        if (item.groups.length === 0) {
                            cellContent = '<span class="text-muted">-</span>';
                        } else {
                            cellContent = item.groups.map(g => 
                                `<span class="badge bg-info me-1">${escapeHtml(g.name)}</span>`
                            ).join('');
                        }
                    } else if (header.key === 'supplyChain' && item.supplyChain && Array.isArray(item.supplyChain)) {
                        // Show supply chain: Supplier â†’ supplied by (lineal)
                        if (item.supplyChain.length === 0) {
                            cellContent = '<span class="text-muted">-</span>';
                        } else {
                            cellContent = item.supplyChain.map(sc => {
                                let chain = escapeHtml(sc.supplierName || sc.supplier?.name || '');
                                if (sc.suppliedBy) chain += ' â† ' + escapeHtml(sc.suppliedBy);
                                return `<span class="badge bg-secondary me-1" title="Supply chain">${chain}</span>`;
                            }).join('');
                        }
                    } else if (header.key === 'items' && item.items && Array.isArray(item.items)) {
                        // Show items with raw materials and supply chain (children for packaging groups)
                        if (item.items.length === 0) {
                            cellContent = '<span class="text-muted">-</span>';
                        } else {
                            cellContent = item.items.map(i => {
                                let html = `<span class="badge bg-primary me-1">${escapeHtml(i.name)}${i.weight ? ' (' + i.weight + 'g)' : ''}</span>`;
                                if (i.rawMaterials && i.rawMaterials.length > 0) {
                                    html += ' <small class="text-muted">â† ' + i.rawMaterials.join(', ') + '</small>';
                                }
                                if (i.supplyChain && i.supplyChain.length > 0) {
                                    const chainStr = i.supplyChain.map(sc => {
                                        const sup = sc.supplier;
                                        let s = (sup && sup.name) || sc.supplierName || '';
                                        const suppliedBy = (sup && sup.suppliedBy) || sc.suppliedBy;
                                        if (suppliedBy) s += ' â† ' + (suppliedBy.name || suppliedBy);
                                        return s;
                                    }).join('; ');
                                    html += ' <small class="text-secondary" title="Supply chain">[' + escapeHtml(chainStr) + ']</small>';
                                }
                                return html;
                            }).join('<br>');
                        }
                    } else if (header.key === 'contactCount' || header.key === 'productCount') {
                        cellContent = item[header.key] != null ? item[header.key] : '-';
                    } else {
                        // Regular value
                        let value = item[header.key];
                        if (value === null || value === undefined) value = '-';
                        if (header.key === 'weight' || header.key === 'totalPackWeight') {
                            value = value ? value + 'g' : '-';
                        }
                        // For child rows, indent non-name columns too based on depth
                        if (isChild && header.key !== 'name') {
                            cellContent = `<span style="padding-left: ${indentPx}rem;">${escapeHtml(value)}</span>`;
                        } else {
                            const str = String(value);
                            const escaped = escapeHtml(str);
                            if (str.length > 35 && !isChild) {
                                cellContent = '<span class="text-truncate-cell d-inline-block" style="max-width:180px;" title="' + escaped.replace(/"/g, '&quot;') + '">' + escaped + '</span>';
                            } else {
                                cellContent = escaped;
                            }
                        }
                    }
                    
                    row += `<td role="gridcell">${cellContent}</td>`;
                });
                
                row += '</tr>';
                return row;
            }).join('');
            
            if (isCardView && cardView && dataTable) {
                const headers = getTableHeaders();
                const cardsHtml = data.items.map(item => {
                    const detailType = (currentType === 'raw-materials' && item.isChild) ? 'raw-materials' : currentType;
                    const title = item.name || item.displayName || ('#' + item.id);
                    const meta = headers.filter(h => h.key !== 'name').slice(0, 3).map(h => {
                        let v = item[h.key];
                        if (h.key === 'weight' || h.key === 'totalPackWeight') v = v ? v + 'g' : '-';
                        if (v === null || v === undefined) v = '-';
                        if (typeof v === 'object') return '';
                        return (String(v).length > 40 ? String(v).substring(0, 37) + '...' : String(v));
                    }).filter(Boolean).join(' Â· ');
                    const sel = selectedId === item.id ? ' selected' : '';
                    const safeTitle = (title || '').replace(/"/g, '&quot;').substring(0, 50);
return '<div class="table-card' + sel + '" onclick="selectRecord(' + item.id + ', \'' + detailType + '\')" onkeydown="if(event.key===\'Enter\'||event.key===\' \'){event.preventDefault();selectRecord(' + item.id + ',\'' + detailType + '\');}" role="button" tabindex="0" data-id="' + item.id + '" aria-label="View ' + safeTitle + '"><div class="table-card-title">' + escapeHtml(title) + '</div><div class="table-card-meta">' + escapeHtml(meta) + '</div></div>';
                }).join('');
                cardView.innerHTML = cardsHtml;
                cardView.classList.remove('d-none');
                dataTable.classList.add('d-none');
            } else if (cardView && dataTable) {
                cardView.classList.add('d-none');
                dataTable.classList.remove('d-none');
            }
        }
        
        function getTableHeaders() {
            if (currentType === 'raw-materials') return [{ key: 'name' }, { key: 'code' }, { key: 'description' }, { key: 'packagingItems' }];
            if (currentType === 'packaging-items') return [{ key: 'name' }, { key: 'taxonomyCode' }, { key: 'materialTaxonomyName' }, { key: 'weight' }];
            if (currentType === 'packaging-groups') return [{ key: 'name' }, { key: 'packId' }, { key: 'packagingLayer' }, { key: 'totalPackWeight' }];
            if (currentType === 'suppliers') return [{ key: 'name' }, { key: 'city' }, { key: 'country' }, { key: 'email' }];
            return [];
        }

        function renderPagination(data) {
            const pagination = document.getElementById('pagination');
            const totalPages = data.totalPages || 1;
            
            if (totalPages <= 1) {
                pagination.innerHTML = '';
                return;
            }

            let html = '';
            
            // Previous button
            html += `<li class="page-item ${currentPage === 1 ? 'disabled' : ''}">`;
            html += `<a class="page-link" href="#" onclick="changePage(${currentPage - 1}); return false;">Previous</a>`;
            html += `</li>`;

            // Page numbers
            const startPage = Math.max(1, currentPage - 2);
            const endPage = Math.min(totalPages, currentPage + 2);
            
            if (startPage > 1) {
                html += `<li class="page-item"><a class="page-link" href="#" onclick="changePage(1); return false;">1</a></li>`;
                if (startPage > 2) {
                    html += `<li class="page-item disabled"><span class="page-link">...</span></li>`;
                }
            }
            
            for (let i = startPage; i <= endPage; i++) {
                html += `<li class="page-item ${i === currentPage ? 'active' : ''}">`;
                html += `<a class="page-link" href="#" onclick="changePage(${i}); return false;">${i}</a>`;
                html += `</li>`;
            }
            
            if (endPage < totalPages) {
                if (endPage < totalPages - 1) {
                    html += `<li class="page-item disabled"><span class="page-link">...</span></li>`;
                }
                html += `<li class="page-item"><a class="page-link" href="#" onclick="changePage(${totalPages}); return false;">${totalPages}</a></li>`;
            }

            // Next button
            html += `<li class="page-item ${currentPage === totalPages ? 'disabled' : ''}">`;
            html += `<a class="page-link" href="#" onclick="changePage(${currentPage + 1}); return false;">Next</a>`;
            html += `</li>`;

            pagination.innerHTML = html;
        }

        function changePage(page) {
            if (page < 1) return;
            currentPage = page;
            loadData();
        }

        function sortTable(column) {
            if (currentSortBy === column) {
                currentSortDir = currentSortDir === 'asc' ? 'desc' : 'asc';
            } else {
                currentSortBy = column;
                currentSortDir = 'asc';
            }
            currentPage = 1;
            loadData();
        }

        function tableRowKeydown(e, id, typeOverride) {
            if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                selectRecord(id, typeOverride);
                return;
            }
            if (e.key === 'ArrowDown' || e.key === 'ArrowUp') {
                e.preventDefault();
                const rows = Array.from(document.querySelectorAll('#tableBody tr[data-id]'));
                const idx = rows.findIndex(r => parseInt(r.dataset.id, 10) === id);
                if (idx < 0) return;
                const nextIdx = e.key === 'ArrowDown' ? Math.min(idx + 1, rows.length - 1) : Math.max(idx - 1, 0);
                if (nextIdx !== idx) rows[nextIdx].focus();
            }
        }
        function selectRecord(id, typeOverride) {
            navigationHistory = [];
            selectedId = id;
            const detailType = typeOverride || currentType;
            if (window.innerWidth < 992) {
                const splitContainer = document.getElementById('splitContainer');
                if (splitContainer) splitContainer.classList.add('packaging-mobile-detail-open');
            }
            if (currentType === 'suppliers') {
                selectedSupplierId = id;
                selectedSupplierName = null; // Will be set from detail data
                const addProductBtn = document.getElementById('addProductBtn');
                if (addProductBtn) addProductBtn.disabled = false;
            }
            
            // Update table row selection
            document.querySelectorAll('#tableBody tr').forEach(tr => {
                tr.classList.remove('selected');
            });
            if (event && event.currentTarget) {
                event.currentTarget.classList.add('selected');
            }
            
            // Load detail
            loadDetail(id, detailType);
        }

        function loadDetail(id, typeOverride) {
            const detailType = typeOverride || currentType;
            const url = `/api/packaging-management/${detailType}/${id}`;
            
            console.log('[Packaging Management] Loading detail:', url);
            
            fetch(url)
                .then(response => {
                    console.log('[Packaging Management] Detail response status:', response.status);
                    if (!response.ok) {
                        throw new Error(`HTTP error! status: ${response.status}`);
                    }
                    return response.json();
                })
                .then(data => {
                    console.log('[Packaging Management] Detail data received:', data);
                    if (data.error) {
                        document.getElementById('detailContent').innerHTML = `<div class="text-danger">Error: ${data.error}</div>`;
                        return;
                    }
                    
                    renderDetail(data);
                })
                .catch(error => {
                    console.error('Error loading detail:', error);
                    document.getElementById('detailContent').innerHTML = '<div class="text-danger">Error loading details</div>';
                });
        }

        function renderDetail(data) {
            const container = document.getElementById('detailContent');
            
            // Determine type from data - if it has a 'level' property, it's a raw material taxonomy
            let detailType = currentType;
            if (data.level !== undefined) {
                detailType = 'raw-materials';
            }
            if (data.contacts !== undefined && Array.isArray(data.contacts)) {
                detailType = 'suppliers';
            }
            
            document.getElementById('editRecordBtn')?.classList.remove('d-none');
            document.getElementById('deleteRecordBtn')?.classList.remove('d-none');
            document.getElementById('editRecordBtn')?.setAttribute('data-edit-id', String(data.id));
            document.getElementById('deleteRecordBtn')?.setAttribute('data-delete-id', String(data.id));

            const viewProductsBtn = document.getElementById('viewProductsBtn');
            if (viewProductsBtn) {
                if (detailType === 'raw-materials' || detailType === 'packaging-items' || detailType === 'packaging-groups') {
                    viewProductsBtn.classList.remove('d-none');
                    viewProductsBtn.setAttribute('data-detail-type', detailType);
                    viewProductsBtn.setAttribute('data-detail-id', String(data.id));
                } else {
                    viewProductsBtn.classList.add('d-none');
                }
            }

            const typeLabels = { 'suppliers': 'Supplier', 'raw-materials': 'Raw Material', 'packaging-items': 'Packaging Item', 'packaging-groups': 'Packaging Group' };
            const recordName = data.name || data.displayName || ('#' + data.id);
            const titleEl = document.getElementById('detailPaneTitle');
            if (titleEl) titleEl.textContent = (typeLabels[detailType] || 'Record') + ' \u00BB ' + recordName;

            if (detailType === 'raw-materials') {
                container.innerHTML = renderRawMaterialDetail(data);
            } else if (detailType === 'packaging-items') {
                container.innerHTML = renderPackagingItemDetail(data);
            } else if (detailType === 'packaging-groups') {
                container.innerHTML = renderPackagingGroupDetail(data);
            } else             if (detailType === 'suppliers') {
                selectedSupplierName = data.name;
                container.innerHTML = renderSupplierDetail(data);
            } else {
                container.innerHTML = '<div class="text-warning">Unknown record type</div>';
            }
            updateDetailBackButton();
        }

        function renderRawMaterialDetail(data) {
            let html = '<div class="detail-section">';
            html += '<h6>Basic Information</h6>';
            html += `<div class="detail-item"><span class="detail-label">Name:</span>${escapeHtml(data.name)}</div>`;
            html += `<div class="detail-item"><span class="detail-label">Code:</span>${escapeHtml(data.code)}</div>`;
            html += `<div class="detail-item"><span class="detail-label">Description:</span>${escapeHtml(data.description || '-')}</div>`;
            html += `<div class="detail-item"><span class="detail-label">Level:</span>${data.level}</div>`;
            html += `</div>`;

            if (data.parent) {
                html += '<div class="detail-section">';
                html += '<h6>Parent</h6>';
                html += navBadge(data.parent.id, 'raw-materials', escapeHtml(data.parent.name) + ' (' + escapeHtml(data.parent.code) + ')');
                html += `</div>`;
            }

            if (data.children && data.children.length > 0) {
                html += '<div class="detail-section">';
                html += '<h6>Children (' + data.children.length + ')</h6>';
                html += data.children.map(c => navBadge(c.id, 'raw-materials', escapeHtml(c.name) + ' (' + escapeHtml(c.code) + ')')).join('');
                html += `</div>`;
            }

            if (data.packagingItems && data.packagingItems.length > 0) {
                html += '<div class="detail-section">';
                html += '<h6>Used in Packaging Items (' + data.packagingItems.length + ')</h6>';
                html += data.packagingItems.map(item => navBadge(item.id, 'packaging-items', escapeHtml(item.name) + ' (' + escapeHtml(item.taxonomyCode) + ')')).join('');
                html += `</div>`;
            }

            if (data.supplyChain && data.supplyChain.length > 0) {
                html += '<div class="detail-section">';
                html += '<h6>Supply Chain (Suppliers of this Raw Material)</h6>';
                html += '<div class="supply-chain-lineal">';
                data.supplyChain.forEach(sc => {
                    html += '<div class="supply-chain-item">' + renderSupplyChainFlow(sc) + '</div>';
                });
                html += '</div></div>';
            }

            return html;
        }

        function renderPackagingItemDetail(data) {
            let html = '<div class="detail-section">';
            html += '<h6>Basic Information</h6>';
            html += `<div class="detail-item"><span class="detail-label">Name:</span>${escapeHtml(data.name)}</div>`;
            html += `<div class="detail-item"><span class="detail-label">Taxonomy Code:</span>${escapeHtml(data.taxonomyCode)}</div>`;
            html += `<div class="detail-item"><span class="detail-label">Weight:</span>${data.weight ? data.weight + 'g' : '-'}</div>`;
            html += `</div>`;

            if (data.rawMaterials && data.rawMaterials.length > 0) {
                html += '<div class="detail-section">';
                html += '<h6>Raw Materials (' + data.rawMaterials.length + ')</h6>';
                html += data.rawMaterials.map(rm => navBadge(rm.id, 'raw-materials', escapeHtml(rm.name) + ' (' + escapeHtml(rm.code) + ')')).join('');
                html += `</div>`;
            } else if (data.materialTaxonomy) {
                html += '<div class="detail-section">';
                html += '<h6>Raw Material</h6>';
                html += navBadge(data.materialTaxonomy.id, 'raw-materials', escapeHtml(data.materialTaxonomy.name) + ' (' + escapeHtml(data.materialTaxonomy.code) + ')');
                html += `</div>`;
            }

            if (data.supplyChain && data.supplyChain.length > 0) {
                html += '<div class="detail-section">';
                html += '<h6>Supply Chain (Suppliers of this Packaging Item)</h6>';
                html += '<div class="supply-chain-lineal">';
                data.supplyChain.forEach(sc => {
                    html += '<div class="supply-chain-item">' + renderSupplyChainFlow(sc) + '</div>';
                });
                html += '</div></div>';
            }

            if (data.packagingGroups && data.packagingGroups.length > 0) {
                html += '<div class="detail-section">';
                html += '<h6>Used in Packaging Groups (' + data.packagingGroups.length + ')</h6>';
                html += data.packagingGroups.map(g => navBadge(g.id, 'packaging-groups', escapeHtml(g.name) + ' (' + escapeHtml(g.packId) + ')')).join('');
                html += `</div>`;
            }

            return html;
        }

        function renderPackagingGroupDetail(data) {
            let html = '<div class="detail-section">';
            html += '<h6>Basic Information</h6>';
            html += `<div class="detail-item"><span class="detail-label">Name:</span>${escapeHtml(data.name)}</div>`;
            html += `<div class="detail-item"><span class="detail-label">Pack ID:</span>${escapeHtml(data.packId)}</div>`;
            html += `<div class="detail-item"><span class="detail-label">Packaging Layer:</span>${escapeHtml(data.packagingLayer || '-')}</div>`;
            html += `<div class="detail-item"><span class="detail-label">Style:</span>${escapeHtml(data.style || '-')}</div>`;
            html += `<div class="detail-item"><span class="detail-label">Shape:</span>${escapeHtml(data.shape || '-')}</div>`;
            html += `<div class="detail-item"><span class="detail-label">Size:</span>${escapeHtml(data.size || '-')}</div>`;
            html += `<div class="detail-item"><span class="detail-label">Volume Dimensions:</span>${escapeHtml(data.volumeDimensions || '-')}</div>`;
            html += `<div class="detail-item"><span class="detail-label">Colours Available:</span>${escapeHtml(data.coloursAvailable || '-')}</div>`;
            html += `<div class="detail-item"><span class="detail-label">Recycled Content:</span>${escapeHtml(data.recycledContent || '-')}</div>`;
            html += `<div class="detail-item"><span class="detail-label">Total Pack Weight:</span>${data.totalPackWeight ? data.totalPackWeight + 'g' : '-'}</div>`;
            html += `<div class="detail-item"><span class="detail-label">Weight Basis:</span>${escapeHtml(data.weightBasis || '-')}</div>`;
            html += `<div class="detail-item"><span class="detail-label">Notes:</span>${escapeHtml(data.notes || '-')}</div>`;
            html += `<div class="detail-item"><span class="detail-label">Example Reference:</span>${escapeHtml(data.exampleReference || '-')}</div>`;
            html += `<div class="detail-item"><span class="detail-label">Source:</span>${escapeHtml(data.source || '-')}</div>`;
            if (data.url) {
                html += `<div class="detail-item"><span class="detail-label">URL:</span><a href="${escapeHtml(data.url)}" target="_blank">${escapeHtml(data.url)}</a></div>`;
            }
            html += `</div>`;

            if (data.items && data.items.length > 0) {
                html += '<div class="detail-section">';
                html += '<h6>Packaging Items (' + data.items.length + ')</h6>';
                html += '<div class="table-responsive"><table class="table table-sm">';
                html += '<thead><tr><th>Name</th><th>Taxonomy Code</th><th>Raw Materials</th><th>Weight</th><th>Supply Chain</th></tr></thead>';
                html += '<tbody>';
                html += data.items.map(item => {
                    const rawMats = item.rawMaterials && item.rawMaterials.length > 0
                        ? item.rawMaterials.map(m => typeof m === 'object' ? m.name : m).join(', ')
                        : (item.materialTaxonomy ? item.materialTaxonomy.name : '-');
                    const chainStr = item.supplyChain && item.supplyChain.length > 0
                        ? item.supplyChain.map(sc => {
                            const s = sc.supplier?.name || '';
                            return sc.supplier?.suppliedBy ? s + ' â† ' + sc.supplier.suppliedBy.name : s;
                        }).join('; ')
                        : '-';
                    const nameCell = item.id ? `<span class="relationship-badge" data-nav-id="${item.id}" data-nav-type="packaging-items" role="button" tabindex="0">${escapeHtml(item.name)}</span>` : escapeHtml(item.name);
                    return `<tr>
                        <td>${nameCell}</td>
                        <td>${escapeHtml(item.taxonomyCode)}</td>
                        <td>${escapeHtml(rawMats)}</td>
                        <td>${item.weight ? item.weight + 'g' : '-'}</td>
                        <td><small class="supply-chain-cell">${escapeHtml(chainStr)}</small></td>
                    </tr>`;
                }).join('');
                html += '</tbody></table></div>';
                html += `</div>`;
            }

            return html;
        }

        function renderSupplierDetail(data) {
            let html = '<div class="supplier-detail-card text-start">';
            html += '<div class="row g-2">';
            html += '<div class="col-md-6"><div class="detail-item"><span class="detail-label">Name:</span> ' + escapeHtml(data.name) + '</div></div>';
            html += '<div class="col-md-6"><div class="detail-item"><span class="detail-label">Phone:</span> ' + escapeHtml(data.phone || '-') + '</div></div>';
            html += '<div class="col-12"><div class="detail-item"><span class="detail-label">Address:</span> ' + escapeHtml(data.address || '-');
            if (data.city || data.state || data.country) {
                const parts = [data.city, data.state, data.country].filter(Boolean);
                if (parts.length) html += ', ' + escapeHtml(parts.join(', '));
            }
            html += '</div></div>';
            html += '<div class="col-md-6"><div class="detail-item"><span class="detail-label">Email:</span> ' + (data.email ? '<a href="mailto:' + escapeHtml(data.email) + '">' + escapeHtml(data.email) + '</a>' : '-') + '</div></div>';
            if (data.website) {
                html += '<div class="col-md-6"><div class="detail-item"><span class="detail-label">Website:</span> <a href="' + escapeHtml(data.website) + '" target="_blank" rel="noopener">' + escapeHtml(data.website) + '</a></div></div>';
            }
            if (data.contacts && data.contacts.length > 0) {
                html += '<div class="col-12 mt-2"><div class="detail-item"><span class="detail-label">Contact Persons:</span></div>';
                data.contacts.forEach(c => {
                    html += '<div class="supplier-contact-card text-start">';
                    html += '<div class="fw-semibold text-dark">' + escapeHtml(c.name);
                    if (c.title) html += ' <span class="text-muted fw-normal">â€” ' + escapeHtml(c.title) + '</span>';
                    html += '</div>';
                    html += '<div class="small text-secondary mt-1">';
                    if (c.phone) html += '<i class="bi bi-telephone me-1"></i>' + escapeHtml(c.phone);
                    if (c.phone && c.email) html += ' &nbsp;|&nbsp; ';
                    if (c.email) html += '<i class="bi bi-envelope me-1"></i><a href="mailto:' + escapeHtml(c.email) + '">' + escapeHtml(c.email) + '</a>';
                    html += '</div></div>';
                });
                html += '</div>';
            }
            html += '</div></div>';

            html += '<div class="supplier-detail-card text-start">';
            html += '<div class="supplier-detail-header"><i class="bi bi-box-seam me-2"></i>Packaging Products</div>';
            if (data.products && data.products.length > 0) {
                html += '<div class="supplier-products-nested">';
                html += '<div class="table-responsive"><table class="table table-sm table-hover mb-0">';
                html += '<thead><tr><th>Name</th><th>Description</th><th>Code</th><th>Taxonomy</th></tr></thead><tbody>';
                data.products.forEach(p => {
                    html += '<tr><td>' + escapeHtml(p.name) + '</td><td>' + escapeHtml(p.description || '-') + '</td><td>' + escapeHtml(p.productCode || '-') + '</td><td>' + escapeHtml(p.taxonomyCode || '-') + '</td></tr>';
                });
                html += '</tbody></table></div></div>';
            } else {
                html += '<p class="text-muted mb-0">No packaging products added yet. Click "Add Packaging" to add products.</p>';
            }
            html += '</div>';

            return html;
        }

        function escapeHtml(text) {
            if (text === null || text === undefined) return '';
            const div = document.createElement('div');
            div.textContent = text;
            return div.innerHTML;
        }

        function renderSupplyChainFlow(sc) {
            const product = escapeHtml(sc.productName || '');
            const supplierName = sc.supplier?.name || sc.supplierName || '';
            const supplierId = sc.supplier?.id;
            const upstream = sc.supplier?.suppliedBy;
            const upstreamName = upstream?.name || '';
            const upstreamId = upstream?.id;
            let html = '<div class="supply-chain-flow" title="' + escapeHtml(product + ' from ' + supplierName + (upstreamName ? ' (supplied by ' + upstreamName + ')' : '')) + '">';
            html += '<span class="supply-chain-step product">' + product + '</span>';
            html += '<span class="supply-chain-arrow">â†’</span>';
            html += supplierId ? navBadge(supplierId, 'suppliers', supplierName) : '<span class="supply-chain-step supplier">' + escapeHtml(supplierName) + '</span>';
            if (upstreamName) {
                html += '<span class="supply-chain-arrow">â†’</span>';
                html += upstreamId ? navBadge(upstreamId, 'suppliers', upstreamName) : '<span class="supply-chain-step upstream">' + escapeHtml(upstreamName) + '</span>';
            }
            html += '</div>';
            return html;
        }
        function navBadge(id, type, label) {
            const icons = { 'suppliers': 'bi-truck', 'raw-materials': 'bi-circle', 'packaging-items': 'bi-box-seam', 'packaging-groups': 'bi-collection' };
            const icon = icons[type] ? '<i class="bi ' + icons[type] + ' me-1"></i>' : '';
            return `<span class="relationship-badge" data-nav-id="${id}" data-nav-type="${type}" role="button" tabindex="0">${icon}${label}</span>`;
        }

        function navigateToRecord(id, type, fromBadge) {
            if (fromBadge && selectedId && currentType) {
                const typeLabels = { 'suppliers': 'Supplier', 'raw-materials': 'Raw Material', 'packaging-items': 'Packaging Item', 'packaging-groups': 'Packaging Group' };
                navigationHistory.push({ id: selectedId, type: currentType, name: document.getElementById('detailPaneTitle')?.textContent || typeLabels[currentType] });
            }
            if (type !== currentType) switchTab(type);
            selectedId = id;
            loadDetail(id, type);
            dataCache = {};
            loadData();
            updateDetailBackButton();
        }
        function updateDetailBackButton() {
            const backBtn = document.getElementById('detailBackBtn');
            if (backBtn) {
                if (navigationHistory.length > 0) {
                    backBtn.classList.remove('d-none');
                    const prev = navigationHistory[navigationHistory.length - 1];
                    backBtn.title = 'Back to ' + (prev.name || 'previous');
                } else {
                    backBtn.classList.add('d-none');
                }
            }
        }
        function goBackInDetail() {
            if (navigationHistory.length === 0) return;
            const prev = navigationHistory.pop();
            navigateToRecord(prev.id, prev.type, false);
        }

        function setupDetailNavigation() {
            const container = document.getElementById('detailContent');
            if (!container) return;
            container.addEventListener('click', function(e) {
                const badge = e.target.closest('.relationship-badge[data-nav-id]');
                if (badge) {
                    e.preventDefault();
                    const id = parseInt(badge.getAttribute('data-nav-id'), 10);
                    const type = badge.getAttribute('data-nav-type');
                    if (id && type) navigateToRecord(id, type, true);
                }
            });
            container.addEventListener('keydown', function(e) {
                const badge = e.target.closest('.relationship-badge[data-nav-id]');
                if (badge && (e.key === 'Enter' || e.key === ' ')) {
                    e.preventDefault();
                    const id = parseInt(badge.getAttribute('data-nav-id'), 10);
                    const type = badge.getAttribute('data-nav-type');
                    if (id && type) navigateToRecord(id, type, true);
                }
            });
            document.getElementById('detailBackBtn')?.addEventListener('click', goBackInDetail);
        }

        function setupEditDeleteButtons() {
            const editBtn = document.getElementById('editRecordBtn');
            const deleteBtn = document.getElementById('deleteRecordBtn');
            const deleteModal = document.getElementById('deleteConfirmModal');
            const deleteConfirmBtn = document.getElementById('deleteConfirmBtn');
            const deleteConfirmMessage = document.getElementById('deleteConfirmMessage');

            if (deleteBtn && deleteModal && deleteConfirmBtn) {
                deleteBtn.addEventListener('click', function() {
                    const id = this.getAttribute('data-delete-id');
                    if (!id) return;
                    const typeLabels = { 'suppliers': 'supplier', 'raw-materials': 'raw material', 'packaging-items': 'packaging item', 'packaging-groups': 'packaging group' };
                    deleteConfirmMessage.textContent = 'Are you sure you want to delete this ' + (typeLabels[currentType] || 'record') + '?';
                    deleteConfirmBtn.dataset.deleteId = id;
                    const modal = new bootstrap.Modal(deleteModal);
                    modal.show();
                });
                deleteConfirmBtn.addEventListener('click', async function() {
                    const id = this.dataset.deleteId;
                    if (!id) return;
                    try {
                        const url = '/api/packaging-management/' + currentType + '/' + id;
                        const r = await fetch(url, { method: 'DELETE' });
                        const data = await r.json();
                        if (data.success) {
                            bootstrap.Modal.getInstance(deleteModal).hide();
                            selectedId = null;
                            selectedSupplierId = null;
                            dataCache = {};
                            showEmptyDetailState();
                            loadData();
                            showToast('Record deleted successfully');
                        } else {
                            showToast('Error: ' + (data.error || 'Failed to delete'), 'danger');
                        }
                    } catch (e) {
                        showToast('Error: ' + e.message, 'danger');
                    }
                });
            }
            if (editBtn) {
                editBtn.addEventListener('click', function() {
                    const id = this.getAttribute('data-edit-id');
                    if (!id) return;
                    openEditModal(parseInt(id, 10));
                });
            }
            const viewProductsBtn = document.getElementById('viewProductsBtn');
            if (viewProductsBtn) {
                viewProductsBtn.addEventListener('click', function() {
                    const type = this.getAttribute('data-detail-type');
                    const id = parseInt(this.getAttribute('data-detail-id'), 10);
                    if (!id || !type || !window.showProductAssociations) return;
                    if (type === 'raw-materials') window.showProductAssociations(id, null, null, 'rawMaterial');
                    else if (type === 'packaging-items') window.showProductAssociations(null, id, null, 'packagingItem');
                    else if (type === 'packaging-groups') window.showProductAssociations(null, null, id, 'packagingGroup');
                });
            }
        }
        function openEditModal(id) {
            const url = '/api/packaging-management/' + currentType + '/' + id;
            fetch(url).then(r => r.json()).then(data => {
                if (data.error) { showToast('Error loading record', 'danger'); return; }
                if (currentType === 'suppliers') {
                    document.getElementById('supplierName').value = data.name || '';
                    document.getElementById('supplierAddress').value = data.address || '';
                    document.getElementById('supplierCity').value = data.city || '';
                    document.getElementById('supplierState').value = data.state || '';
                    document.getElementById('supplierCountry').value = data.country || '';
                    document.getElementById('supplierPhone').value = data.phone || '';
                    document.getElementById('supplierEmail').value = data.email || '';
                    document.getElementById('supplierWebsite').value = data.website || '';
                    const container = document.getElementById('contactRowsContainer');
                    container.innerHTML = '';
                    (data.contacts || []).forEach(c => {
                        const row = document.createElement('div');
                        row.className = 'contact-row border rounded p-2 mb-2 bg-light d-flex align-items-center';
                        row.innerHTML = '<div class="row g-2 flex-grow-1"><div class="col-md-4"><input type="text" class="form-control form-control-sm contact-name" placeholder="Name" value="' + escapeHtml(c.name || '') + '" aria-label="Contact name"></div><div class="col-md-3"><input type="text" class="form-control form-control-sm contact-title" placeholder="Title" value="' + escapeHtml(c.title || '') + '" aria-label="Contact title"></div><div class="col-md-3"><input type="text" class="form-control form-control-sm contact-phone" placeholder="Phone" value="' + escapeHtml(c.phone || '') + '" aria-label="Contact phone"></div><div class="col-md-2"><input type="email" class="form-control form-control-sm contact-email" placeholder="Email" value="' + escapeHtml(c.email || '') + '" aria-label="Contact email"></div></div><button type="button" class="btn btn-outline-danger btn-sm ms-2 remove-contact-btn" title="Remove contact" aria-label="Remove contact"><i class="bi bi-x-lg"></i></button>';
                        container.appendChild(row);
                    });
                    if (container.children.length === 0) {
                        const row = document.createElement('div');
                        row.className = 'contact-row border rounded p-2 mb-2 bg-light d-flex align-items-center';
                        row.innerHTML = '<div class="row g-2 flex-grow-1"><div class="col-md-4"><input type="text" class="form-control form-control-sm contact-name" placeholder="Name" aria-label="Contact name"></div><div class="col-md-3"><input type="text" class="form-control form-control-sm contact-title" placeholder="Title" aria-label="Contact title"></div><div class="col-md-3"><input type="text" class="form-control form-control-sm contact-phone" placeholder="Phone" aria-label="Contact phone"></div><div class="col-md-2"><input type="email" class="form-control form-control-sm contact-email" placeholder="Email" aria-label="Contact email"></div></div><button type="button" class="btn btn-outline-danger btn-sm ms-2 remove-contact-btn" title="Remove contact" aria-label="Remove contact"><i class="bi bi-x-lg"></i></button>';
                        container.appendChild(row);
                    }
                    document.getElementById('addSupplierModal').querySelector('.modal-title').textContent = 'Edit Packaging Supplier';
                    document.getElementById('saveSupplierBtn').dataset.editId = id;
                    showSupplierModalStep(1);
                    new bootstrap.Modal(document.getElementById('addSupplierModal')).show();
                } else if (currentType === 'raw-materials') {
                    document.getElementById('rawMaterialName').value = data.name || '';
                    document.getElementById('rawMaterialCode').value = data.code || '';
                    document.getElementById('rawMaterialLevel').value = String(data.level || 1);
                    document.getElementById('rawMaterialDescription').value = data.description || '';
                    const parentSelect = document.getElementById('rawMaterialParent');
                    parentSelect.innerHTML = '<option value="">â€” None (top level) â€”</option>';
                    fetch('/api/packaging-management/options/raw-materials').then(r => r.json()).then(opts => {
                        opts.filter(t => t.id !== id).forEach(t => {
                            const opt = document.createElement('option');
                            opt.value = t.id;
                            opt.textContent = t.name + ' (' + t.code + ')';
                            if (data.parent && data.parent.id === t.id) opt.selected = true;
                            parentSelect.appendChild(opt);
                        });
                        parentSelect.value = data.parent ? String(data.parent.id) : '';
                        document.getElementById('addRawMaterialModal').querySelector('.modal-title').textContent = 'Edit Raw Material';
                        document.getElementById('saveRawMaterialBtn').dataset.editId = id;
                        new bootstrap.Modal(document.getElementById('addRawMaterialModal')).show();
                    });
                } else if (currentType === 'packaging-items') {
                    document.getElementById('packagingItemName').value = data.name || '';
                    document.getElementById('packagingItemTaxonomyCode').value = data.taxonomyCode || '';
                    document.getElementById('packagingItemWeight').value = data.weight || '';
                    const rmSelect = document.getElementById('packagingItemRawMaterials');
                    const supSelect = document.getElementById('packagingItemSuppliers');
                    rmSelect.innerHTML = '';
                    supSelect.innerHTML = '';
                    const rawMatIds = (data.rawMaterials || []).map(rm => rm.id);
                    const chainProductIds = (data.supplyChain || []).map(sc => sc.productId).filter(Boolean);
                    Promise.all([
                        fetch('/api/packaging-management/options/raw-materials').then(r => r.json()),
                        fetch('/api/packaging-management/options/supplier-products').then(r => r.json())
                    ]).then(([rms, sups]) => {
                        rms.forEach(t => {
                            const opt = document.createElement('option');
                            opt.value = t.id;
                            opt.textContent = t.name + ' (' + t.code + ')';
                            if (rawMatIds.includes(t.id)) opt.selected = true;
                            rmSelect.appendChild(opt);
                        });
                        sups.forEach(p => {
                            const opt = document.createElement('option');
                            opt.value = p.id;
                            opt.textContent = p.name + ' â€” ' + p.supplierName;
                            if (chainProductIds.includes(p.id)) opt.selected = true;
                            supSelect.appendChild(opt);
                        });
                        document.getElementById('addPackagingItemModal').querySelector('.modal-title').textContent = 'Edit Packaging Item';
                        document.getElementById('savePackagingItemBtn').dataset.editId = id;
                        new bootstrap.Modal(document.getElementById('addPackagingItemModal')).show();
                    });
                } else if (currentType === 'packaging-groups') {
                    document.getElementById('packagingGroupName').value = data.name || '';
                    document.getElementById('packagingGroupPackId').value = data.packId || '';
                    document.getElementById('packagingGroupLayer').value = data.packagingLayer || '';
                    const itemsSelect = document.getElementById('packagingGroupItems');
                    itemsSelect.innerHTML = '';
                    const itemIds = (data.items || []).map(i => i.id).filter(Boolean);
                    fetch('/api/packaging-management/options/packaging-items').then(r => r.json()).then(opts => {
                        opts.forEach(l => {
                            const opt = document.createElement('option');
                            opt.value = l.id;
                            opt.textContent = l.name + ' (' + l.taxonomyCode + ')';
                            if (itemIds.includes(l.id)) opt.selected = true;
                            itemsSelect.appendChild(opt);
                        });
                        document.getElementById('addPackagingGroupModal').querySelector('.modal-title').textContent = 'Edit Packaging Group';
                        document.getElementById('savePackagingGroupBtn').dataset.editId = id;
                        new bootstrap.Modal(document.getElementById('addPackagingGroupModal')).show();
                    });
                }
            }).catch(e => showToast('Error: ' + e.message, 'danger'));
        }
        function showToast(message, type) {
            type = type || 'success';
            const t = document.getElementById('packagingToast') || (function() {
                const div = document.createElement('div');
                div.id = 'packagingToast';
                div.className = 'position-fixed bottom-0 end-0 p-3';
                div.style.zIndex = '9999';
                div.innerHTML = '<div class="toast align-items-center text-bg-' + type + ' border-0" role="alert"><div class="d-flex"><div class="toast-body"></div><button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button></div></div>';
                document.body.appendChild(div);
                return div;
            })();
            const toastEl = t.querySelector('.toast');
            toastEl.className = 'toast align-items-center text-bg-' + type + ' border-0';
            toastEl.querySelector('.toast-body').textContent = message;
            const toast = new bootstrap.Toast(toastEl);
            toast.show();
        }

        function setupAddModals() {
            const addRawMaterialBtn = document.getElementById('addRawMaterialBtn');
            const addPackagingItemBtn = document.getElementById('addPackagingItemBtn');
            const addPackagingGroupBtn = document.getElementById('addPackagingGroupBtn');
            const addRawMaterialModal = document.getElementById('addRawMaterialModal');
            const addPackagingItemModal = document.getElementById('addPackagingItemModal');
            const addPackagingGroupModal = document.getElementById('addPackagingGroupModal');

            if (addRawMaterialBtn && addRawMaterialModal) {
                addRawMaterialBtn.addEventListener('click', async function() {
                    document.getElementById('addRawMaterialModal').querySelector('.modal-title').textContent = 'Add Raw Material';
                    delete document.getElementById('saveRawMaterialBtn')?.dataset.editId;
                    document.getElementById('rawMaterialName').value = '';
                    document.getElementById('rawMaterialCode').value = '';
                    document.getElementById('rawMaterialLevel').value = '1';
                    document.getElementById('rawMaterialParent').value = '';
                    document.getElementById('rawMaterialDescription').value = '';
                    const parentSelect = document.getElementById('rawMaterialParent');
                    parentSelect.innerHTML = '<option value="">â€” None (top level) â€”</option>';
                    try {
                        const r = await fetch('/api/packaging-management/options/raw-materials');
                        const opts = await r.json();
                        opts.forEach(t => {
                            const opt = document.createElement('option');
                            opt.value = t.id;
                            opt.textContent = t.name + ' (' + t.code + ')';
                            parentSelect.appendChild(opt);
                        });
                    } catch (e) { console.error(e); }
                    const modal = new bootstrap.Modal(addRawMaterialModal);
                    modal.show();
                });
            }
            document.getElementById('saveRawMaterialBtn')?.addEventListener('click', async function() {
                const name = document.getElementById('rawMaterialName').value?.trim();
                const code = document.getElementById('rawMaterialCode').value?.trim();
                if (!name || !code) { showToast('Name and Code are required', 'danger'); return; }
                const level = parseInt(document.getElementById('rawMaterialLevel').value, 10) || 1;
                const parentVal = document.getElementById('rawMaterialParent').value;
                const parentId = parentVal ? parseInt(parentVal, 10) : null;
                const editId = this.dataset.editId;
                const url = editId ? '/api/packaging-management/raw-materials/' + editId : '/api/packaging-management/raw-materials';
                try {
                    const r = await fetch(url, {
                        method: editId ? 'PUT' : 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify({
                            name: name,
                            code: code,
                            description: document.getElementById('rawMaterialDescription').value?.trim() || null,
                            level: level,
                            parentTaxonomyId: parentId
                        })
                    });
                    const data = await r.json();
                    if (data.error) { showToast('Error: ' + data.error, 'danger'); return; }
                    bootstrap.Modal.getInstance(addRawMaterialModal).hide();
                    delete this.dataset.editId;
                    dataCache = {};
                    loadData();
                    const id = data.id || (editId ? parseInt(editId, 10) : null);
                    if (id) { selectedId = id; loadDetail(id, 'raw-materials'); }
                    showToast('Raw material saved successfully');
                } catch (e) { showToast('Error: ' + e.message, 'danger'); }
            });

            if (addPackagingItemBtn && addPackagingItemModal) {
                addPackagingItemBtn.addEventListener('click', async function() {
                    document.getElementById('addPackagingItemModal').querySelector('.modal-title').textContent = 'Add Packaging Item';
                    delete document.getElementById('savePackagingItemBtn')?.dataset.editId;
                    document.getElementById('packagingItemName').value = '';
                    document.getElementById('packagingItemTaxonomyCode').value = '';
                    document.getElementById('packagingItemWeight').value = '';
                    const rmSelect = document.getElementById('packagingItemRawMaterials');
                    const supSelect = document.getElementById('packagingItemSuppliers');
                    rmSelect.innerHTML = '';
                    supSelect.innerHTML = '';
                    try {
                        const [rmRes, supRes] = await Promise.all([
                            fetch('/api/packaging-management/options/raw-materials'),
                            fetch('/api/packaging-management/options/supplier-products')
                        ]);
                        const rms = await rmRes.json();
                        const sups = await supRes.json();
                        rms.forEach(t => {
                            const opt = document.createElement('option');
                            opt.value = t.id;
                            opt.textContent = t.name + ' (' + t.code + ')';
                            rmSelect.appendChild(opt);
                        });
                        sups.forEach(p => {
                            const opt = document.createElement('option');
                            opt.value = p.id;
                            opt.textContent = p.name + ' â€” ' + p.supplierName;
                            supSelect.appendChild(opt);
                        });
                    } catch (e) { console.error(e); }
                    const modal = new bootstrap.Modal(addPackagingItemModal);
                    modal.show();
                });
            }
            document.getElementById('savePackagingItemBtn')?.addEventListener('click', async function() {
                const name = document.getElementById('packagingItemName').value?.trim();
                const taxonomyCode = document.getElementById('packagingItemTaxonomyCode').value?.trim();
                if (!name || !taxonomyCode) { showToast('Name and Taxonomy Code are required', 'danger'); return; }
                const weightVal = document.getElementById('packagingItemWeight').value;
                const weight = weightVal ? parseFloat(weightVal) : null;
                const rmSelected = Array.from(document.getElementById('packagingItemRawMaterials').selectedOptions).map(o => parseInt(o.value, 10));
                const supSelected = Array.from(document.getElementById('packagingItemSuppliers').selectedOptions).map(o => parseInt(o.value, 10));
                const editId = this.dataset.editId;
                const url = editId ? '/api/packaging-management/packaging-items/' + editId : '/api/packaging-management/packaging-items';
                try {
                    const r = await fetch(url, {
                        method: editId ? 'PUT' : 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify({
                            name: name,
                            taxonomyCode: taxonomyCode,
                            weight: weight,
                            materialTaxonomyIds: rmSelected,
                            packagingSupplierProductIds: supSelected
                        })
                    });
                    const data = await r.json();
                    if (data.error) { showToast('Error: ' + data.error, 'danger'); return; }
                    bootstrap.Modal.getInstance(addPackagingItemModal).hide();
                    delete this.dataset.editId;
                    dataCache = {};
                    loadData();
                    const id = data.id || (editId ? parseInt(editId, 10) : null);
                    if (id) { selectedId = id; loadDetail(id, 'packaging-items'); }
                    showToast('Packaging item saved successfully');
                } catch (e) { showToast('Error: ' + e.message, 'danger'); }
            });

            if (addPackagingGroupBtn && addPackagingGroupModal) {
                addPackagingGroupBtn.addEventListener('click', async function() {
                    document.getElementById('addPackagingGroupModal').querySelector('.modal-title').textContent = 'Add Packaging Group';
                    delete document.getElementById('savePackagingGroupBtn')?.dataset.editId;
                    document.getElementById('packagingGroupName').value = '';
                    document.getElementById('packagingGroupPackId').value = '';
                    document.getElementById('packagingGroupLayer').value = '';
                    const itemsSelect = document.getElementById('packagingGroupItems');
                    itemsSelect.innerHTML = '';
                    try {
                        const r = await fetch('/api/packaging-management/options/packaging-items');
                        const opts = await r.json();
                        opts.forEach(l => {
                            const opt = document.createElement('option');
                            opt.value = l.id;
                            opt.textContent = l.name + ' (' + l.taxonomyCode + ')';
                            itemsSelect.appendChild(opt);
                        });
                    } catch (e) { console.error(e); }
                    const modal = new bootstrap.Modal(addPackagingGroupModal);
                    modal.show();
                });
            }
            document.getElementById('savePackagingGroupBtn')?.addEventListener('click', async function() {
                const name = document.getElementById('packagingGroupName').value?.trim();
                const packId = document.getElementById('packagingGroupPackId').value?.trim();
                if (!name || !packId) { showToast('Name and Pack ID are required', 'danger'); return; }
                const layer = document.getElementById('packagingGroupLayer').value?.trim() || null;
                const itemsSelected = Array.from(document.getElementById('packagingGroupItems').selectedOptions).map(o => parseInt(o.value, 10));
                const editId = this.dataset.editId;
                const url = editId ? '/api/packaging-management/packaging-groups/' + editId : '/api/packaging-management/packaging-groups';
                try {
                    const r = await fetch(url, {
                        method: editId ? 'PUT' : 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify({
                            name: name,
                            packId: packId,
                            packagingLayer: layer,
                            packagingLibraryIds: itemsSelected
                        })
                    });
                    const data = await r.json();
                    if (data.error) { showToast('Error: ' + data.error, 'danger'); return; }
                    bootstrap.Modal.getInstance(addPackagingGroupModal).hide();
                    delete this.dataset.editId;
                    dataCache = {};
                    loadData();
                    const id = data.id || (editId ? parseInt(editId, 10) : null);
                    if (id) { selectedId = id; loadDetail(id, 'packaging-groups'); }
                    showToast('Packaging group saved successfully');
                } catch (e) { showToast('Error: ' + e.message, 'danger'); }
            });
        }

        let supplierModalStep = 1;
        function showSupplierModalStep(step) {
            supplierModalStep = step;
            const s1 = document.getElementById('supplierStep1');
            const s2 = document.getElementById('supplierStep2');
            const prevBtn = document.getElementById('supplierStepPrevBtn');
            const nextBtn = document.getElementById('supplierStepNextBtn');
            const saveBtn = document.getElementById('saveSupplierBtn');
            document.querySelectorAll('.supplier-step-badge').forEach(b => {
                b.classList.toggle('active', parseInt(b.dataset.step, 10) === step);
                b.removeAttribute('aria-current');
                if (parseInt(b.dataset.step, 10) === step) b.setAttribute('aria-current', 'step');
            });
            if (s1) s1.classList.toggle('d-none', step !== 1);
            if (s2) s2.classList.toggle('d-none', step !== 2);
            if (prevBtn) prevBtn.style.display = step === 1 ? 'none' : 'inline-block';
            if (nextBtn) nextBtn.style.display = step === 2 ? 'none' : 'inline-block';
            if (saveBtn) saveBtn.style.display = step === 2 ? 'inline-block' : 'none';
        }
        function setupSupplierModals() {
            const addSupplierBtn = document.getElementById('addSupplierBtn');
            const saveProductBtn = document.getElementById('saveProductBtn');
            const addProductBtn = document.getElementById('addProductBtn');
            const saveSupplierBtn = document.getElementById('saveSupplierBtn');
            const addSupplierModal = document.getElementById('addSupplierModal');
            const addProductModal = document.getElementById('addProductModal');

            document.getElementById('supplierStepNextBtn')?.addEventListener('click', function() {
                const name = document.getElementById('supplierName').value?.trim();
                if (!name) { showToast('Please enter a supplier name.', 'danger'); return; }
                showSupplierModalStep(2);
            });
            document.getElementById('supplierStepPrevBtn')?.addEventListener('click', function() {
                showSupplierModalStep(1);
            });

            addSupplierModal?.addEventListener('show.bs.modal', function() {
                showSupplierModalStep(1);
            });

            if (addSupplierBtn) {
                addSupplierBtn.addEventListener('click', function() {
                    document.getElementById('addSupplierModal').querySelector('.modal-title').textContent = 'Add Packaging Supplier';
                    delete document.getElementById('saveSupplierBtn').dataset.editId;
                    document.getElementById('supplierName').value = '';
                    document.getElementById('supplierAddress').value = '';
                    document.getElementById('supplierCity').value = '';
                    document.getElementById('supplierState').value = '';
                    document.getElementById('supplierCountry').value = '';
                    document.getElementById('supplierPhone').value = '';
                    document.getElementById('supplierEmail').value = '';
                    document.getElementById('supplierWebsite').value = '';
                    const container = document.getElementById('contactRowsContainer');
                    container.innerHTML = '<div class="contact-row border rounded p-2 mb-2 bg-light d-flex align-items-center"><div class="row g-2 flex-grow-1"><div class="col-md-4"><input type="text" class="form-control form-control-sm contact-name" placeholder="Name" aria-label="Contact name"></div><div class="col-md-3"><input type="text" class="form-control form-control-sm contact-title" placeholder="Title" aria-label="Contact title"></div><div class="col-md-3"><input type="text" class="form-control form-control-sm contact-phone" placeholder="Phone" aria-label="Contact phone"></div><div class="col-md-2"><input type="email" class="form-control form-control-sm contact-email" placeholder="Email" aria-label="Contact email"></div></div><button type="button" class="btn btn-outline-danger btn-sm ms-2 remove-contact-btn" title="Remove contact" aria-label="Remove contact"><i class="bi bi-x-lg"></i></button></div>';
                    if (addSupplierModal && typeof bootstrap !== 'undefined') {
                        new bootstrap.Modal(addSupplierModal).show();
                    }
                });
            }

            const addContactRowBtn = document.getElementById('addContactRowBtn');
            if (addContactRowBtn) {
                addContactRowBtn.addEventListener('click', function() {
                    const container = document.getElementById('contactRowsContainer');
                    const row = document.createElement('div');
                    row.className = 'contact-row border rounded p-2 mb-2 bg-light d-flex align-items-center';
                    row.innerHTML = '<div class="row g-2 flex-grow-1"><div class="col-md-4"><input type="text" class="form-control form-control-sm contact-name" placeholder="Name" aria-label="Contact name"></div><div class="col-md-3"><input type="text" class="form-control form-control-sm contact-title" placeholder="Title" aria-label="Contact title"></div><div class="col-md-3"><input type="text" class="form-control form-control-sm contact-phone" placeholder="Phone" aria-label="Contact phone"></div><div class="col-md-2"><input type="email" class="form-control form-control-sm contact-email" placeholder="Email" aria-label="Contact email"></div></div><button type="button" class="btn btn-outline-danger btn-sm ms-2 remove-contact-btn" title="Remove contact" aria-label="Remove contact"><i class="bi bi-x-lg"></i></button>';
                    container.appendChild(row);
                });
            }
            document.getElementById('contactRowsContainer')?.addEventListener('click', function(e) {
                if (e.target.closest('.remove-contact-btn')) e.target.closest('.contact-row')?.remove();
            });

            if (addProductBtn) {
                addProductBtn.addEventListener('click', function() {
                    if (!selectedSupplierId) return;
                    document.getElementById('addProductSupplierName').textContent = 'Adding product for: ' + (selectedSupplierName || 'Supplier');
                    document.getElementById('productName').value = '';
                    document.getElementById('productDescription').value = '';
                    document.getElementById('productCode').value = '';
                    document.getElementById('productTaxonomyCode').value = '';
                    if (addProductModal && typeof bootstrap !== 'undefined') {
                        new bootstrap.Modal(addProductModal).show();
                    }
                });
            }

            if (saveSupplierBtn) {
                saveSupplierBtn.addEventListener('click', async function() {
                    const name = document.getElementById('supplierName').value.trim();
                    if (!name) {
                        showToast('Please enter a supplier name.', 'danger');
                        return;
                    }
                    const editId = this.dataset.editId;
                    const url = editId ? '/api/packaging-management/suppliers/' + editId : '/api/packaging-management/suppliers';
                    const method = editId ? 'PUT' : 'POST';
                    try {
                        const contacts = [];
                        document.querySelectorAll('.contact-row').forEach(row => {
                            const cName = row.querySelector('.contact-name')?.value?.trim();
                            if (cName) {
                                contacts.push({
                                    name: cName,
                                    title: row.querySelector('.contact-title')?.value?.trim() || null,
                                    phone: row.querySelector('.contact-phone')?.value?.trim() || null,
                                    email: row.querySelector('.contact-email')?.value?.trim() || null
                                });
                            }
                        });
                        const res = await fetch(url, {
                            method: method,
                            headers: { 'Content-Type': 'application/json' },
                            body: JSON.stringify({
                                name: name,
                                address: document.getElementById('supplierAddress').value.trim() || null,
                                city: document.getElementById('supplierCity').value.trim() || null,
                                state: document.getElementById('supplierState').value.trim() || null,
                                country: document.getElementById('supplierCountry').value.trim() || null,
                                phone: document.getElementById('supplierPhone').value.trim() || null,
                                email: document.getElementById('supplierEmail').value.trim() || null,
                                website: document.getElementById('supplierWebsite').value.trim() || null,
                                contacts: contacts
                            })
                        });
                        const data = await res.json();
                        if (data.success) {
                            if (addSupplierModal && typeof bootstrap !== 'undefined') {
                                const modal = bootstrap.Modal.getInstance(addSupplierModal);
                                if (modal) modal.hide();
                            }
                            delete this.dataset.editId;
                            dataCache = {};
                            loadData();
                            if (editId) loadDetail(parseInt(editId, 10), 'suppliers');
                            showToast('Supplier saved successfully');
                        } else {
                            showToast('Error: ' + (data.error || 'Failed to save'), 'danger');
                        }
                    } catch (e) {
                        showToast('Error: ' + e.message, 'danger');
                    }
                });
            }

            if (saveProductBtn) {
                saveProductBtn.addEventListener('click', async function() {
                    if (!selectedSupplierId) return;
                    const name = document.getElementById('productName').value.trim();
                    if (!name) {
                        showToast('Please enter a product name.', 'danger');
                        return;
                    }
                    try {
                        const res = await fetch('/api/packaging-management/suppliers/' + selectedSupplierId + '/products', {
                            method: 'POST',
                            headers: { 'Content-Type': 'application/json' },
                            body: JSON.stringify({
                                name: name,
                                description: document.getElementById('productDescription').value.trim() || null,
                                productCode: document.getElementById('productCode').value.trim() || null,
                                taxonomyCode: document.getElementById('productTaxonomyCode').value.trim() || null
                            })
                        });
                        const data = await res.json();
                        if (data.success) {
                            if (addProductModal && typeof bootstrap !== 'undefined') {
                                const modal = bootstrap.Modal.getInstance(addProductModal);
                                if (modal) modal.hide();
                            }
                            loadDetail(selectedSupplierId, 'suppliers');
                            loadData();
                        } else {
                            showToast('Error: ' + (data.error || 'Failed to save'), 'danger');
                        }
                    } catch (e) {
                        showToast('Error: ' + e.message, 'danger');
                    }
                });
            }
        }

        // ============================================================
        // SUPPLY CHAIN MATRIX
        // ============================================================

        function setupMatrixEventListeners() {
            const filterInput = document.getElementById('matrixFilterInput');
            if (filterInput) {
                let timeout;
                filterInput.addEventListener('input', function() {
                    clearTimeout(timeout);
                    timeout = setTimeout(() => {
                        matrixFilter = this.value;
                        matrixPage = 1;
                        loadMatrixData();
                    }, 300);
                });
            }
            ['matrixFilterPrimary', 'matrixFilterSecondary', 'matrixFilterTertiary'].forEach((id, idx) => {
                const el = document.getElementById(id);
                const key = ['primary', 'secondary', 'tertiary'][idx];
                if (el) {
                    el.addEventListener('change', function() {
                        matrixLayerFilter[key] = this.checked;
                        matrixPage = 1;
                        loadMatrixData();
                    });
                }
            });
            const groupBySelect = document.getElementById('matrixGroupBy');
            if (groupBySelect) {
                groupBySelect.addEventListener('change', function() {
                    matrixGroupBy = this.value;
                    matrixSortBy = this.value;
                    matrixPage = 1;
                    loadMatrixData();
                });
            }
            const exportBtn = document.getElementById('exportMatrixCsvBtn');
            if (exportBtn) {
                exportBtn.addEventListener('click', exportMatrixCsv);
            }
        }

        async function ensureMatrixOptions() {
            if (matrixOptionsCache) return matrixOptionsCache;
            try {
                const [rmRes, piRes, spRes, pgRes, sRes] = await Promise.all([
                    fetch('/api/packaging-management/options/raw-materials').then(r => r.json()),
                    fetch('/api/packaging-management/options/packaging-items').then(r => r.json()),
                    fetch('/api/packaging-management/options/supplier-products').then(r => r.json()),
                    fetch('/api/packaging-management/options/packaging-groups').then(r => r.json()),
                    fetch('/api/packaging-management/options/suppliers').then(r => r.json())
                ]);
                matrixOptionsCache = { rawMaterials: rmRes, packagingItems: piRes, supplierProducts: spRes, packagingGroups: pgRes, suppliers: sRes };
                return matrixOptionsCache;
            } catch (e) {
                console.error('[Matrix] Failed to load options:', e);
                return { rawMaterials: [], packagingItems: [], supplierProducts: [], packagingGroups: [], suppliers: [] };
            }
        }

        function loadMatrixData() {
            let layers = [];
            if (matrixLayerFilter.primary) layers.push('Primary');
            if (matrixLayerFilter.secondary) layers.push('Secondary');
            if (matrixLayerFilter.tertiary) layers.push('Tertiary');
            if (layers.length === 0) layers = ['Primary', 'Secondary', 'Tertiary'];
            const packagingLayer = layers.join(',');
            const url = `/api/packaging-management/supply-chain-matrix?page=${matrixPage}&pageSize=${matrixPageSize}&groupBy=${matrixGroupBy}&filter=${encodeURIComponent(matrixFilter)}&sortBy=${matrixSortBy}&sortDir=${matrixSortDir}&packagingLayer=${encodeURIComponent(packagingLayer)}`;
            const tbody = document.getElementById('matrixTableBody');
            if (tbody) tbody.innerHTML = '<tr><td colspan="8" class="text-center py-4"><div class="spinner-border spinner-border-sm text-primary" role="status"></div> Loading...</td></tr>';

            fetch(url)
                .then(r => r.json())
                .then(data => {
                    if (data.error) {
                        if (tbody) tbody.innerHTML = '<tr><td colspan="8" class="text-center text-danger py-4">Error: ' + escapeHtml(data.error) + '</td></tr>';
                        return;
                    }
                    matrixData = data;
                    renderMatrix(data);
                    renderMatrixPagination(data);
                    const countEl = document.getElementById('matrixRecordCount');
                    if (countEl) countEl.textContent = data.totalCount || 0;
                })
                .catch(err => {
                    if (tbody) tbody.innerHTML = '<tr><td colspan="8" class="text-center text-danger py-4">Failed to load matrix data.</td></tr>';
                    console.error('[Matrix] Load error:', err);
                });
        }

        function renderMatrix(data) {
            const tbody = document.getElementById('matrixTableBody');
            if (!tbody) return;
            if (!data.rows || data.rows.length === 0) {
                tbody.innerHTML = '<tr><td colspan="8" class="text-center text-muted py-5">No supply chain data found. Add packaging groups, items, and suppliers to populate this view.</td></tr>';
                return;
            }

            const rows = data.rows;
            let html = '';
            let prevMaterialId = null;
            let prevItemId = null;
            let prevGroupId = null;
            let groupIndex = 0;

            for (let i = 0; i < rows.length; i++) {
                const row = rows[i];
                const isNewMaterial = row.rawMaterialId !== prevMaterialId || row.packagingItemId !== prevItemId || row.packagingGroupId !== prevGroupId;
                const isNewItem = row.packagingItemId !== prevItemId || row.packagingGroupId !== prevGroupId;
                const isNewGroup = row.packagingGroupId !== prevGroupId;
                if (isNewGroup) { groupIndex++; }

                const groupClass = groupIndex % 2 === 0 ? 'matrix-group-even' : 'matrix-group-odd';
                const showMaterial = isNewMaterial;
                const showItem = isNewItem;
                const showGroup = isNewGroup;

                html += '<tr class="' + groupClass + '" data-row-idx="' + i + '">';

                // Raw Material column
                if (showMaterial) {
                    if (row.rawMaterialId) {
                        html += '<td class="matrix-cell-first">';
                        html += '<span class="matrix-nav-link" onclick="window.navigateToRecord(' + row.rawMaterialId + ', \'raw-materials\')" title="View material">' + escapeHtml(row.rawMaterialName) + '</span>';
                        if (row.rawMaterialCode) html += ' <span class="matrix-cell-secondary">[' + escapeHtml(row.rawMaterialCode) + ']</span>';
                        html += ' <button class="matrix-plus-btn" title="Add material" onclick="matrixInlineAdd(\'material\', ' + row.packagingItemId + ', this)"><i class="bi bi-plus-square"></i></button>';
                        html += '</td>';
                    } else {
                        html += '<td class="matrix-cell-missing"><span class="matrix-assign-link" onclick="matrixInlineAdd(\'material\', ' + row.packagingItemId + ', this)"><i class="bi bi-plus-circle-fill"></i> Assign material</span></td>';
                    }
                } else {
                    html += '<td class="matrix-cell-merged"></td>';
                }

                // Raw Material Supplier(s) column (immediately follows Raw Material)
                if (showMaterial) {
                    html += '<td>';
                    const rmSuppliers = row.rawMaterialSuppliers || (row.rawMaterialSupplierId ? [{ id: row.rawMaterialSupplierId, name: row.rawMaterialSupplierName, productName: row.rawMaterialSupplierProductName }] : []);
                    if (rmSuppliers.length > 0) {
                        rmSuppliers.forEach(s => {
                            html += '<span class="badge bg-info bg-opacity-10 text-info me-1"><span class="matrix-nav-link" onclick="window.navigateToRecord(' + s.id + ', \'suppliers\')">' + escapeHtml(s.name || '') + '</span>';
                            if (s.productName) html += ' <span class="matrix-cell-secondary">(' + escapeHtml(s.productName) + ')</span>';
                            html += '</span>';
                        });
                    } else {
                        html += '<span class="matrix-cell-missing"><span class="matrix-assign-link" onclick="matrixInlineAdd(\'material-supplier\', ' + (row.rawMaterialId || 0) + ', this)"><i class="bi bi-plus-circle-fill"></i> Assign</span></span>';
                    }
                    html += '</td>';
                } else {
                    html += '<td class="matrix-cell-merged"></td>';
                }

                // Packaging Item column
                if (showItem) {
                    html += '<td class="matrix-cell-first">';
                    html += '<span class="matrix-nav-link" onclick="window.navigateToRecord(' + row.packagingItemId + ', \'packaging-items\')" title="View item">' + escapeHtml(row.packagingItemName) + '</span>';
                    if (row.packagingItemWeight) html += ' <span class="matrix-cell-secondary">(' + row.packagingItemWeight + 'g)</span>';
                    html += '</td>';
                } else {
                    html += '<td class="matrix-cell-merged"></td>';
                }

                // Packaging Item Supplier(s) column (immediately follows Packaging Item)
                if (showItem) {
                    html += '<td>';
                    const itemSuppliers = row.packagingItemSuppliers || (row.supplierId ? [{ id: row.supplierId, name: row.supplierName, productName: row.supplierProductName }] : []);
                    if (itemSuppliers.length > 0) {
                        itemSuppliers.forEach(s => {
                            html += '<span class="badge bg-primary bg-opacity-10 text-primary me-1"><span class="matrix-nav-link" onclick="window.navigateToRecord(' + s.id + ', \'suppliers\')">' + escapeHtml(s.name || '') + '</span>';
                            if (s.productName) html += ' <span class="matrix-cell-secondary">(' + escapeHtml(s.productName) + ')</span>';
                            html += '</span>';
                        });
                        html += ' <button class="matrix-plus-btn" title="Add supplier" onclick="matrixInlineAdd(\'supplier\', ' + row.packagingItemId + ', this)"><i class="bi bi-plus-square"></i></button>';
                    } else {
                        html += '<span class="matrix-cell-missing"><span class="matrix-assign-link" onclick="matrixInlineAdd(\'supplier\', ' + row.packagingItemId + ', this)"><i class="bi bi-person-plus-fill"></i> Assign supplier</span></span>';
                    }
                    html += '</td>';
                } else {
                    html += '<td class="matrix-cell-merged"></td>';
                }

                // Packaging Group column
                if (showGroup) {
                    html += '<td class="matrix-cell-first">';
                    if (row.packagingGroupId) {
                        html += '<span class="matrix-nav-link" onclick="window.navigateToRecord(' + row.packagingGroupId + ', \'packaging-groups\')" title="View group">' + escapeHtml(row.packagingGroupName) + '</span>';
                        if (row.packagingGroupLayer) html += ' <span class="matrix-cell-secondary">(' + escapeHtml(row.packagingGroupLayer) + ')</span>';
                    } else {
                        html += '<span class="matrix-cell-secondary fst-italic">Ungrouped</span>';
                    }
                    html += '</td>';
                } else {
                    html += '<td class="matrix-cell-merged"></td>';
                }

                // Packaging Group Supplier(s) column (immediately follows Packaging Group)
                if (showGroup) {
                    html += '<td>';
                    const groupSuppliers = row.packagingGroupSuppliers || [];
                    if (row.packagingGroupId) {
                        if (groupSuppliers.length > 0) {
                            groupSuppliers.forEach(s => {
                                html += '<span class="badge bg-success bg-opacity-10 text-success me-1"><span class="matrix-nav-link" onclick="window.navigateToRecord(' + s.id + ', \'suppliers\')">' + escapeHtml(s.name || '') + '</span>';
                                if (s.productName) html += ' <span class="matrix-cell-secondary">(' + escapeHtml(s.productName) + ')</span>';
                                html += '</span>';
                            });
                            html += ' <button class="matrix-plus-btn" title="Add supplier" onclick="matrixInlineAdd(\'group-supplier\', ' + row.packagingGroupId + ', this)"><i class="bi bi-plus-square"></i></button>';
                        } else {
                            html += '<span class="matrix-cell-missing"><span class="matrix-assign-link" onclick="matrixInlineAdd(\'group-supplier\', ' + row.packagingGroupId + ', this)"><i class="bi bi-plus-circle-fill"></i> Assign</span></span>';
                        }
                    } else {
                        html += '<span class="matrix-cell-secondary">-</span>';
                    }
                    html += '</td>';
                } else {
                    html += '<td class="matrix-cell-merged"></td>';
                }

                // Upstream Supplier column
                if (row.upstreamSupplierId) {
                    html += '<td>';
                    html += '<span class="matrix-nav-link" onclick="window.navigateToRecord(' + row.upstreamSupplierId + ', \'suppliers\')" title="View upstream supplier">' + escapeHtml(row.upstreamSupplierName) + '</span>';
                    html += '</td>';
                } else {
                    html += '<td><span class="matrix-cell-secondary">-</span></td>';
                }

                // Actions column
                html += '<td class="text-nowrap">';
                html += '<button class="matrix-action-btn me-1" title="View product associations" onclick="window.showProductAssociations(' + (row.packagingItemId || 0) + ', ' + (row.supplierProductId || 0) + ', ' + (row.supplierId || 0) + ')"><i class="bi bi-diagram-3"></i><span class="matrix-action-label">Products</span></button>';
                if (row.rawMaterialId && row.packagingItemId) {
                    html += '<button class="matrix-action-btn danger me-1" title="Unlink material from item" onclick="matrixUnlink(\'material\', ' + row.packagingItemId + ', ' + row.rawMaterialId + ')"><i class="bi bi-x-circle"></i><span class="matrix-action-label">Unlink mat</span></button>';
                }
                if (row.supplierProductId && row.packagingItemId) {
                    html += '<button class="matrix-action-btn danger" title="Unlink supplier from item" onclick="matrixUnlink(\'supplier\', ' + row.packagingItemId + ', ' + row.supplierProductId + ')"><i class="bi bi-x-circle"></i><span class="matrix-action-label">Unlink sup</span></button>';
                }
                html += '</td>';

                html += '</tr>';

                prevMaterialId = row.rawMaterialId;
                prevItemId = row.packagingItemId;
                prevGroupId = row.packagingGroupId;
            }

            tbody.innerHTML = html;
        }

        function renderMatrixPagination(data) {
            const pagination = document.getElementById('matrixPagination');
            if (!pagination) return;
            const totalPages = data.totalPages || 1;
            if (totalPages <= 1) { pagination.innerHTML = ''; return; }
            let html = '';
            html += '<li class="page-item ' + (matrixPage === 1 ? 'disabled' : '') + '"><a class="page-link" href="#" onclick="window.changeMatrixPage(' + (matrixPage - 1) + '); return false;">Prev</a></li>';
            const start = Math.max(1, matrixPage - 2);
            const end = Math.min(totalPages, matrixPage + 2);
            if (start > 1) {
                html += '<li class="page-item"><a class="page-link" href="#" onclick="window.changeMatrixPage(1); return false;">1</a></li>';
                if (start > 2) html += '<li class="page-item disabled"><span class="page-link">...</span></li>';
            }
            for (let i = start; i <= end; i++) {
                html += '<li class="page-item ' + (i === matrixPage ? 'active' : '') + '"><a class="page-link" href="#" onclick="window.changeMatrixPage(' + i + '); return false;">' + i + '</a></li>';
            }
            if (end < totalPages) {
                if (end < totalPages - 1) html += '<li class="page-item disabled"><span class="page-link">...</span></li>';
                html += '<li class="page-item"><a class="page-link" href="#" onclick="window.changeMatrixPage(' + totalPages + '); return false;">' + totalPages + '</a></li>';
            }
            html += '<li class="page-item ' + (matrixPage === totalPages ? 'disabled' : '') + '"><a class="page-link" href="#" onclick="window.changeMatrixPage(' + (matrixPage + 1) + '); return false;">Next</a></li>';
            pagination.innerHTML = html;
        }

        window.changeMatrixPage = function(page) {
            if (page < 1) return;
            matrixPage = page;
            loadMatrixData();
        };

        window.matrixInlineAdd = async function(linkType, entityId, el) {
            const td = el.closest('td');
            if (!td) return;
            const options = await ensureMatrixOptions();
            let items = [];
            if (linkType === 'material') {
                items = options.rawMaterials.map(m => ({ id: m.id, label: (m.code ? m.code + ' - ' : '') + m.name }));
            } else if (linkType === 'supplier') {
                items = options.supplierProducts.map(sp => ({ id: sp.id, label: sp.name + (sp.supplierName ? ' (' + sp.supplierName + ')' : '') }));
            } else if (linkType === 'material-supplier' || linkType === 'group-supplier') {
                items = options.supplierProducts.map(sp => ({ id: sp.id, label: sp.name + (sp.supplierName ? ' (' + sp.supplierName + ')' : '') }));
            }
            if (items.length === 0) {
                showToast('No options available. Create ' + (linkType === 'material' ? 'raw materials' : 'supplier products') + ' first.', 'warning');
                return;
            }
            let selectHtml = '<select class="matrix-inline-select" autofocus><option value="">-- Select --</option>';
            items.forEach(opt => {
                selectHtml += '<option value="' + opt.id + '">' + escapeHtml(opt.label) + '</option>';
            });
            selectHtml += '</select>';
            td.innerHTML = selectHtml;
            const sel = td.querySelector('select');
            if (sel) {
                sel.focus();
                sel.addEventListener('change', async function() {
                    const val = parseInt(this.value, 10);
                    if (!val) { loadMatrixData(); return; }
                    try {
                        let url, body;
                        if (linkType === 'material') {
                            url = '/api/packaging-management/packaging-items/' + entityId + '/raw-materials';
                            body = JSON.stringify({ materialTaxonomyId: val });
                        } else if (linkType === 'material-supplier') {
                            url = '/api/packaging-management/raw-materials/' + entityId + '/suppliers';
                            body = JSON.stringify({ packagingSupplierProductId: val });
                        } else if (linkType === 'group-supplier') {
                            url = '/api/packaging-management/packaging-groups/' + entityId + '/suppliers';
                            body = JSON.stringify({ packagingSupplierProductId: val });
                        } else {
                            url = '/api/packaging-management/packaging-items/' + entityId + '/suppliers';
                            body = JSON.stringify({ packagingSupplierProductId: val });
                        }
                        const res = await fetch(url, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: body });
                        const result = await res.json();
                        if (result.success) {
                            td.classList.add('matrix-flash-success');
                            matrixOptionsCache = null;
                            dataCache = {};
                            setTimeout(() => loadMatrixData(), 400);
                        } else {
                            showToast('Error: ' + (result.error || 'Failed to assign'), 'danger');
                            loadMatrixData();
                        }
                    } catch (e) {
                        showToast('Error: ' + e.message, 'danger');
                        loadMatrixData();
                    }
                });
                sel.addEventListener('blur', function() {
                    if (!this.value) loadMatrixData();
                });
            }
        };

        // Unlink: remove a relationship
        window.matrixUnlink = async function(linkType, packagingItemId, linkedId) {
            if (!confirm('Remove this ' + linkType + ' link?')) return;
            try {
                let url;
                if (linkType === 'material') {
                    url = '/api/packaging-management/packaging-items/' + packagingItemId + '/raw-materials/' + linkedId;
                } else {
                    url = '/api/packaging-management/packaging-items/' + packagingItemId + '/suppliers/' + linkedId;
                }
                const res = await fetch(url, { method: 'DELETE' });
                const result = await res.json();
                if (result.success) {
                    showToast('Link removed successfully.');
                    matrixOptionsCache = null;
                    dataCache = {};
                    loadMatrixData();
                } else {
                    showToast('Error: ' + (result.error || 'Failed to remove link'), 'danger');
                }
            } catch (e) {
                showToast('Error: ' + e.message, 'danger');
            }
        };

        // CSV Export
        function exportMatrixCsv() {
            if (!matrixData || !matrixData.rows || matrixData.rows.length === 0) {
                showToast('No data to export.', 'warning');
                return;
            }
            const headers = ['Packaging Group', 'Pack ID', 'Layer', 'Packaging Item', 'Taxonomy Code', 'Weight (g)', 'Raw Material', 'Material Code', 'Material Supplier', 'Supplier', 'Supplier Product', 'Upstream Supplier'];
            const csvRows = [headers.join(',')];
            matrixData.rows.forEach(row => {
                csvRows.push([
                    csvEscape(row.packagingGroupName),
                    csvEscape(row.packagingGroupPackId),
                    csvEscape(row.packagingGroupLayer),
                    csvEscape(row.packagingItemName),
                    csvEscape(row.packagingItemTaxonomyCode),
                    row.packagingItemWeight != null ? row.packagingItemWeight : '',
                    csvEscape(row.rawMaterialName),
                    csvEscape(row.rawMaterialCode),
                    csvEscape(row.rawMaterialSupplierName),
                    csvEscape(row.supplierName),
                    csvEscape(row.supplierProductName),
                    csvEscape(row.upstreamSupplierName)
                ].join(','));
            });
            const blob = new Blob([csvRows.join('\n')], { type: 'text/csv;charset=utf-8;' });
            const url = URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = 'supply-chain-matrix-' + new Date().toISOString().slice(0, 10) + '.csv';
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
            URL.revokeObjectURL(url);
            showToast('CSV exported successfully.');
        }

        function csvEscape(val) {
            if (val == null) return '';
            const str = String(val);
            if (str.includes(',') || str.includes('"') || str.includes('\n')) {
                return '"' + str.replace(/"/g, '""') + '"';
            }
            return str;
        }

        // === Add Matrix Row Modal ===
        window.openAddMatrixRowModal = async function() {
            const options = await ensureMatrixOptions();
            const groupSel = document.getElementById('matrixRowGroup');
            const itemSel = document.getElementById('matrixRowItem');
            const matSel = document.getElementById('matrixRowMaterials');
            const spSel = document.getElementById('matrixRowSupplierProducts');
            const alert = document.getElementById('addMatrixRowAlert');
            if (alert) { alert.classList.add('d-none'); alert.textContent = ''; }

            if (groupSel) {
                groupSel.innerHTML = '<option value="">(None / Ungrouped)</option><option value="__new__">-- Create New Group --</option>';
                (options.packagingGroups || []).forEach(g => {
                    groupSel.innerHTML += '<option value="' + g.id + '">' + escapeHtml(g.name) + (g.packId ? ' [' + escapeHtml(g.packId) + ']' : '') + '</option>';
                });
            }
            if (itemSel) {
                itemSel.innerHTML = '<option value="">-- Select Item --</option><option value="__new__">-- Create New Item --</option>';
                (options.packagingItems || []).forEach(p => {
                    itemSel.innerHTML += '<option value="' + p.id + '">' + escapeHtml(p.name) + '</option>';
                });
            }
            if (matSel) {
                matSel.innerHTML = '';
                (options.rawMaterials || []).forEach(m => {
                    matSel.innerHTML += '<option value="' + m.id + '">' + (m.code ? escapeHtml(m.code) + ' - ' : '') + escapeHtml(m.name) + '</option>';
                });
            }
            if (spSel) {
                spSel.innerHTML = '';
                (options.supplierProducts || []).forEach(sp => {
                    spSel.innerHTML += '<option value="' + sp.id + '">' + escapeHtml(sp.name) + (sp.supplierName ? ' (' + escapeHtml(sp.supplierName) + ')' : '') + '</option>';
                });
            }

            document.getElementById('matrixRowNewGroupFields')?.classList.add('d-none');
            document.getElementById('matrixRowNewItemFields')?.classList.add('d-none');

            groupSel?.addEventListener('change', function() {
                document.getElementById('matrixRowNewGroupFields')?.classList.toggle('d-none', this.value !== '__new__');
            });
            itemSel?.addEventListener('change', function() {
                document.getElementById('matrixRowNewItemFields')?.classList.toggle('d-none', this.value !== '__new__');
            });

            const modal = new bootstrap.Modal(document.getElementById('addMatrixRowModal'));
            modal.show();
        };

        window.saveMatrixRow = async function() {
            const spinner = document.getElementById('saveMatrixRowSpinner');
            const alert = document.getElementById('addMatrixRowAlert');
            const btn = document.getElementById('saveMatrixRowBtn');
            if (spinner) spinner.classList.remove('d-none');
            if (btn) btn.disabled = true;
            if (alert) { alert.classList.add('d-none'); alert.textContent = ''; }

            const groupVal = document.getElementById('matrixRowGroup')?.value;
            const itemVal = document.getElementById('matrixRowItem')?.value;

            const body = {};

            if (groupVal === '__new__') {
                body.newGroupName = document.getElementById('matrixRowNewGroupName')?.value?.trim();
                body.newGroupPackId = document.getElementById('matrixRowNewGroupPackId')?.value?.trim();
                body.newGroupLayer = document.getElementById('matrixRowNewGroupLayer')?.value?.trim();
                if (!body.newGroupName) {
                    showAlert(alert, 'Group name is required when creating a new group.', 'danger');
                    if (spinner) spinner.classList.add('d-none');
                    if (btn) btn.disabled = false;
                    return;
                }
            } else if (groupVal) {
                body.packagingGroupId = parseInt(groupVal, 10);
            }

            if (itemVal === '__new__') {
                body.newItemName = document.getElementById('matrixRowNewItemName')?.value?.trim();
                body.newItemTaxonomyCode = document.getElementById('matrixRowNewItemCode')?.value?.trim();
                const wt = document.getElementById('matrixRowNewItemWeight')?.value;
                if (wt) body.newItemWeight = parseFloat(wt);
                if (!body.newItemName) {
                    showAlert(alert, 'Item name is required when creating a new item.', 'danger');
                    if (spinner) spinner.classList.add('d-none');
                    if (btn) btn.disabled = false;
                    return;
                }
            } else if (itemVal) {
                body.packagingItemId = parseInt(itemVal, 10);
            } else {
                showAlert(alert, 'A packaging item is required.', 'danger');
                if (spinner) spinner.classList.add('d-none');
                if (btn) btn.disabled = false;
                return;
            }

            const matSel = document.getElementById('matrixRowMaterials');
            if (matSel) {
                body.materialTaxonomyIds = Array.from(matSel.selectedOptions).map(o => parseInt(o.value, 10));
            }

            const spSel = document.getElementById('matrixRowSupplierProducts');
            if (spSel) {
                body.supplierProductIds = Array.from(spSel.selectedOptions).map(o => parseInt(o.value, 10));
            }

            try {
                const res = await fetch('/api/packaging-management/supply-chain-matrix/add-row', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(body)
                });
                const result = await res.json();
                if (result.success) {
                    bootstrap.Modal.getInstance(document.getElementById('addMatrixRowModal'))?.hide();
                    matrixOptionsCache = null;
                    dataCache = {};
                    showToast('Row added successfully.');
                    loadMatrixData();
                } else {
                    showAlert(alert, result.error || 'Failed to add row.', 'danger');
                }
            } catch (e) {
                showAlert(alert, 'Error: ' + e.message, 'danger');
            } finally {
                if (spinner) spinner.classList.add('d-none');
                if (btn) btn.disabled = false;
            }
        };

        function showAlert(el, msg, type) {
            if (!el) return;
            el.className = 'alert alert-' + type + ' mb-3';
            el.textContent = msg;
            el.classList.remove('d-none');
        }

        // === Product Associations Modal ===
        // Signature: showProductAssociations(rawMaterialId, packagingItemId, packagingGroupId, source)
        //    OR for matrix: showProductAssociations(packagingItemId, supplierProductId, supplierId)
        window.showProductAssociations = async function(a, b, c, d) {
            let rawMaterialId, packagingItemId, packagingGroupId, supplierProductId, supplierId;
            if (typeof d === 'string') {
                rawMaterialId = a && a > 0 ? a : null;
                packagingItemId = b && b > 0 ? b : null;
                packagingGroupId = c && c > 0 ? c : null;
                supplierProductId = null;
                supplierId = null;
            } else {
                rawMaterialId = null;
                packagingItemId = a && a > 0 ? a : null;
                supplierProductId = b && b > 0 ? b : null;
                supplierId = c && c > 0 ? c : null;
                packagingGroupId = null;
            }

            const modal = new bootstrap.Modal(document.getElementById('productAssociationsModal'));
            const loading = document.getElementById('productAssocLoading');
            const empty = document.getElementById('productAssocEmpty');
            const table = document.getElementById('productAssocTable');
            const tbody = document.getElementById('productAssocTableBody');
            const context = document.getElementById('productAssocContext');

            if (loading) loading.classList.remove('d-none');
            if (empty) empty.classList.add('d-none');
            if (table) table.classList.add('d-none');
            if (tbody) tbody.innerHTML = '';
            if (context) context.textContent = 'Showing products associated with this supply chain path.';

            modal.show();

            try {
                const params = new URLSearchParams();
                if (rawMaterialId) params.append('rawMaterialId', rawMaterialId);
                if (packagingItemId) params.append('packagingItemId', packagingItemId);
                if (packagingGroupId) params.append('packagingGroupId', packagingGroupId);
                if (supplierProductId) params.append('supplierProductId', supplierProductId);
                if (supplierId) params.append('supplierId', supplierId);

                const res = await fetch('/api/packaging-management/supply-chain-matrix/product-associations?' + params.toString());
                const data = await res.json();

                if (loading) loading.classList.add('d-none');

                if (!data.products || data.products.length === 0) {
                    if (empty) empty.classList.remove('d-none');
                    return;
                }

                let html = '';
                data.products.forEach(p => {
                    html += '<tr>';
                    html += '<td>' + escapeHtml(p.name) + '</td>';
                    html += '<td>' + escapeHtml(p.sku || '-') + '</td>';
                    html += '<td>' + escapeHtml(p.brand || '-') + '</td>';
                    html += '<td>' + escapeHtml(p.category || '-') + '</td>';
                    html += '</tr>';
                });
                if (tbody) tbody.innerHTML = html;
                if (table) table.classList.remove('d-none');
                if (context) context.textContent = 'Found ' + data.products.length + ' associated product(s).';
            } catch (e) {
                if (loading) loading.classList.add('d-none');
                if (empty) { empty.classList.remove('d-none'); empty.querySelector('p').textContent = 'Error loading product associations.'; }
                console.error('[Matrix] Product associations error:', e);
            }
        };
})();