/**
 * Text Editor — Step-by-step text-first interface for supply chain data.
 * Reads/writes the same localStorage key as the Visual Editor (embedded instance),
 * so both interfaces stay in sync automatically.
 */
(function () {
    'use strict';

    // ─── Constants ────────────────────────────────────────────────────────────
    const STORAGE_KEY = 'eprSavedProjects-epr-instance-packaging-management';

    const STEPS = [
        { type: 'raw-material',    label: 'Raw Materials',     icon: 'bi-circle',       color: '#6f42c1', bg: '#f3f0ff', desc: 'Define what your packaging materials are made from.' },
        { type: 'packaging',       label: 'Packaging',         icon: 'bi-box-seam',     color: '#0d6efd', bg: '#e7f3ff', desc: 'Define the packaging items used in your supply chain.' },
        { type: 'packaging-group', label: 'Groups',            icon: 'bi-collection',   color: '#198754', bg: '#e9f7ef', desc: 'Group packaging items by layer (Primary, Secondary, etc.).' },
        { type: 'product',         label: 'Products',          icon: 'bi-bag',          color: '#fd7e14', bg: '#fff3e9', desc: 'Define products that use this packaging.' },
        { type: 'distribution',    label: 'Distribution',      icon: 'bi-geo-alt',      color: '#dc3545', bg: '#fdf0f0', desc: 'Define where products are distributed.' },
        { type: 'review',          label: 'Review',            icon: 'bi-check-circle', color: '#20c997', bg: '#e9faf5', desc: 'Review your complete supply chain.' }
    ];

    // Connection rules: which type connects to which (from → to)
    const CONNECTION_MAP = {
        'raw-material':    'packaging',
        'packaging':       'packaging-group',
        'packaging-group': 'product',
        'product':         'distribution'
    };

    // Reverse lookup: to ← from
    const REVERSE_CONNECTION_MAP = {};
    Object.entries(CONNECTION_MAP).forEach(([from, to]) => { REVERSE_CONNECTION_MAP[to] = from; });

    // Fields per node type
    const NODE_FIELDS = {
        'raw-material': [
            { key: 'name',        label: 'Name',         type: 'text',   required: true,  placeholder: 'e.g. PET Plastic' },
            { key: 'description', label: 'Description',  type: 'text',   placeholder: 'Brief description' },
            { key: 'weight',      label: 'Weight (g)',   type: 'number', placeholder: '0' },
            { key: 'notes',       label: 'Notes',        type: 'textarea' }
        ],
        'packaging': [
            { key: 'name',        label: 'Name',         type: 'text',   required: true, placeholder: 'e.g. 500ml Bottle' },
            { key: 'description', label: 'Description',  type: 'text',   placeholder: 'Brief description' },
            { key: 'height',      label: 'Height (mm)',  type: 'number', placeholder: '0' },
            { key: 'weight',      label: 'Weight (g)',   type: 'number', placeholder: '0' },
            { key: 'depth',       label: 'Depth (mm)',   type: 'number', placeholder: '0' },
            { key: 'notes',       label: 'Notes',        type: 'textarea' }
        ],
        'packaging-group': [
            { key: 'name',         label: 'Name',          type: 'text',   required: true, placeholder: 'e.g. Primary Pack' },
            { key: 'layer',        label: 'Layer',         type: 'select', options: ['Primary', 'Secondary', 'Tertiary', 'Quaternary'] },
            { key: 'holdsQuantity',label: 'Holds Quantity',type: 'number', placeholder: '1' }
        ],
        'product': [
            { key: 'name',        label: 'Name',         type: 'text',   required: true, placeholder: 'e.g. Sparkling Water 500ml' },
            { key: 'sku',         label: 'SKU',          type: 'text',   placeholder: 'e.g. SW-500' },
            { key: 'description', label: 'Description',  type: 'text',   placeholder: 'Brief description' },
            { key: 'weight',      label: 'Weight (g)',   type: 'number', placeholder: '0' },
            { key: 'quantity',    label: 'Quantity',     type: 'number', placeholder: '0' }
        ],
        'distribution': [
            { key: 'name',        label: 'Name / Location', type: 'text',   required: true, placeholder: 'e.g. London – Tesco' },
            { key: 'companyName', label: 'Company',         type: 'text',   placeholder: 'e.g. Tesco PLC' },
            { key: 'siteName',    label: 'Site Name',       type: 'text',   placeholder: 'e.g. Tesco Kensington' },
            { key: 'city',        label: 'City',            type: 'text',   placeholder: 'e.g. London' },
            { key: 'country',     label: 'Country',         type: 'text',   placeholder: 'e.g. UK' },
            { key: 'type',        label: 'Type',            type: 'select', options: ['Store', 'Warehouse', 'Hub', 'Distribution Centre'] }
        ]
    };

    // Auto-layout X positions by node type
    const LAYOUT_X = {
        'raw-material': 80, 'packaging': 350, 'packaging-group': 620,
        'product': 890, 'distribution': 1160
    };

    // ─── State ────────────────────────────────────────────────────────────────
    let allProjects = [];
    let currentProject = null;
    let currentStep = 0;
    let expandedCardId = null;
    let addingInStep = false;
    let initialized = false;

    // ─── Entry point ──────────────────────────────────────────────────────────
    function init() {
        if (!document.getElementById('textEditorContainer')) return;
        loadFromStorage();
        if (!initialized) {
            bindProjectBarEvents();
            document.getElementById('teTreeRefreshBtn')?.addEventListener('click', () => {
                renderTree();
            });
            initialized = true;
        }
        renderAll();
    }

    // ─── Storage ──────────────────────────────────────────────────────────────
    function loadFromStorage() {
        try {
            const raw = localStorage.getItem(STORAGE_KEY);
            allProjects = raw ? JSON.parse(raw) : [];
            if (!Array.isArray(allProjects)) allProjects = [];
        } catch (_) { allProjects = []; }

        if (allProjects.length === 0) {
            const blank = createBlankProject('My Supply Chain');
            allProjects.push(blank);
            saveToStorage();
        }
        if (!currentProject) {
            currentProject = allProjects[0];
        } else {
            // Re-sync reference after reload
            const idx = allProjects.findIndex(p => p.projectName === currentProject.projectName);
            currentProject = idx >= 0 ? allProjects[idx] : allProjects[0];
        }
    }

    function saveToStorage() {
        try {
            localStorage.setItem(STORAGE_KEY, JSON.stringify(allProjects));
            flashSyncBadge();
        } catch (_) {}
    }

    function flashSyncBadge() {
        const badge = document.getElementById('teSyncBadge');
        if (!badge) return;
        badge.style.background = '#d1fae5';
        badge.style.borderColor = '#6ee7b7';
        badge.innerHTML = '<i class="bi bi-check-circle-fill me-1" style="color:#059669"></i>Saved';
        setTimeout(() => {
            badge.style.background = '#f0f4ff';
            badge.style.borderColor = '';
            badge.innerHTML = '<i class="bi bi-arrow-repeat me-1"></i>Synced with Visual Editor';
        }, 1800);
    }

    function createBlankProject(name) {
        return { projectName: name, nodes: [], connections: [], savedAt: new Date().toISOString() };
    }

    // ─── Project helpers ──────────────────────────────────────────────────────
    function getNodesOfType(type) {
        if (!currentProject) return [];
        return (currentProject.nodes || []).filter(n => n.type === type);
    }

    function getNodeById(id) {
        return (currentProject?.nodes || []).find(n => n.id === id);
    }

    function getConnectedTo(nodeId) {
        return (currentProject?.connections || [])
            .filter(c => c.from === nodeId)
            .map(c => getNodeById(c.to))
            .filter(Boolean);
    }

    function getConnectedFrom(nodeId) {
        return (currentProject?.connections || [])
            .filter(c => c.to === nodeId)
            .map(c => getNodeById(c.from))
            .filter(Boolean);
    }

    function connectionExists(fromId, toId) {
        return (currentProject?.connections || []).some(c => c.from === fromId && c.to === toId);
    }

    function generateNodeId() {
        return `node-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
    }

    function assignCoordinates(type) {
        const x = LAYOUT_X[type] || 100;
        const peers = getNodesOfType(type);
        const y = 80 + peers.length * 140;
        return { x, y };
    }

    // ─── CRUD ─────────────────────────────────────────────────────────────────
    function addNode(type, params) {
        const { x, y } = assignCoordinates(type);
        const node = {
            id: generateNodeId(),
            type,
            x, y,
            name: params.name || 'Untitled',
            icon: STEPS.find(s => s.type === type)?.icon || 'bi-square',
            locked: false,
            parameters: { ...params }
        };
        // Store name at top level too (matches VE format)
        currentProject.nodes.push(node);
        currentProject.savedAt = new Date().toISOString();
        saveToStorage();
        return node;
    }

    function updateNode(id, params) {
        const node = getNodeById(id);
        if (!node) return;
        node.name = params.name || node.name;
        Object.assign(node.parameters, params);
        node.parameters.name = params.name || node.name;
        currentProject.savedAt = new Date().toISOString();
        saveToStorage();
    }

    function deleteNode(id) {
        if (!currentProject) return;
        currentProject.nodes = currentProject.nodes.filter(n => n.id !== id);
        currentProject.connections = currentProject.connections.filter(
            c => c.from !== id && c.to !== id
        );
        currentProject.savedAt = new Date().toISOString();
        saveToStorage();
    }

    function addConnection(fromId, toId) {
        if (!currentProject || connectionExists(fromId, toId)) return;
        currentProject.connections.push({ from: fromId, to: toId, fromPort: 'right', toPort: 'left' });
        currentProject.savedAt = new Date().toISOString();
        saveToStorage();
    }

    function removeConnection(fromId, toId) {
        if (!currentProject) return;
        currentProject.connections = currentProject.connections.filter(
            c => !(c.from === fromId && c.to === toId)
        );
        currentProject.savedAt = new Date().toISOString();
        saveToStorage();
    }

    // ─── Render all ───────────────────────────────────────────────────────────
    function renderAll() {
        renderProjectBar();
        renderStepper();
        renderTree();
        renderStep();
    }

    // ─── Project bar ──────────────────────────────────────────────────────────
    function renderProjectBar() {
        const sel = document.getElementById('teProjectSelect');
        if (!sel) return;
        sel.innerHTML = '';
        allProjects.forEach((p, i) => {
            const opt = document.createElement('option');
            opt.value = i;
            opt.textContent = p.projectName;
            if (p === currentProject) opt.selected = true;
            sel.appendChild(opt);
        });
        const hasPrj = allProjects.length > 0;
        const delBtn = document.getElementById('teDeleteProjectBtn');
        const renameBtn = document.getElementById('teRenameProjectBtn');
        if (delBtn) delBtn.disabled = !hasPrj;
        if (renameBtn) renameBtn.disabled = !hasPrj;
    }

    function bindProjectBarEvents() {
        document.getElementById('teProjectSelect')?.addEventListener('change', e => {
            const idx = parseInt(e.target.value, 10);
            if (!isNaN(idx) && allProjects[idx]) {
                currentProject = allProjects[idx];
                expandedCardId = null;
                addingInStep = false;
                renderAll();
            }
        });

        document.getElementById('teNewProjectBtn')?.addEventListener('click', () => {
            const name = prompt('Project name:', 'My Supply Chain');
            if (!name?.trim()) return;
            const p = createBlankProject(name.trim());
            allProjects.push(p);
            currentProject = p;
            saveToStorage();
            expandedCardId = null;
            addingInStep = false;
            currentStep = 0;
            renderAll();
        });

        document.getElementById('teRenameProjectBtn')?.addEventListener('click', () => {
            if (!currentProject) return;
            const name = prompt('Rename project:', currentProject.projectName);
            if (!name?.trim()) return;
            currentProject.projectName = name.trim();
            saveToStorage();
            renderProjectBar();
        });

        document.getElementById('teDeleteProjectBtn')?.addEventListener('click', () => {
            if (!currentProject) return;
            if (!confirm(`Delete "${currentProject.projectName}"? This cannot be undone.`)) return;
            allProjects = allProjects.filter(p => p !== currentProject);
            if (allProjects.length === 0) {
                allProjects.push(createBlankProject('My Supply Chain'));
            }
            currentProject = allProjects[0];
            saveToStorage();
            expandedCardId = null;
            addingInStep = false;
            currentStep = 0;
            renderAll();
        });
    }

    // ─── Stepper ──────────────────────────────────────────────────────────────
    function renderStepper() {
        const bar = document.getElementById('teStepperBar');
        if (!bar) return;
        bar.innerHTML = '';
        STEPS.forEach((step, i) => {
            if (i > 0) {
                const arrow = document.createElement('span');
                arrow.className = 'te-step-arrow';
                arrow.innerHTML = '<i class="bi bi-chevron-right"></i>';
                bar.appendChild(arrow);
            }
            const pill = document.createElement('button');
            pill.type = 'button';
            pill.className = 'te-step-pill' + (i === currentStep ? ' active' : '');
            const hasItems = i < 5 && getNodesOfType(step.type).length > 0;
            if (hasItems && i !== currentStep) pill.classList.add('done');
            pill.innerHTML = `<span class="te-step-num">${hasItems && i !== currentStep ? '<i class="bi bi-check" style="font-size:0.7rem"></i>' : i + 1}</span>${step.label}`;
            pill.dataset.step = i;
            pill.addEventListener('click', () => goToStep(i));
            bar.appendChild(pill);
        });
    }

    function goToStep(idx) {
        currentStep = Math.max(0, Math.min(STEPS.length - 1, idx));
        expandedCardId = null;
        addingInStep = false;
        renderStepper();
        renderStep();
    }

    // ─── Tree ─────────────────────────────────────────────────────────────────
    function renderTree() {
        const container = document.getElementById('teTreeContent');
        if (!container) return;
        if (!currentProject || !currentProject.nodes || currentProject.nodes.length === 0) {
            container.innerHTML = '<div class="text-muted text-center py-4" style="font-size:0.8rem;">No nodes yet.<br>Start by adding raw materials.</div>';
            return;
        }

        const allNodes = currentProject.nodes;
        const html = [];

        // Group by type in step order
        const typeOrder = ['raw-material', 'packaging', 'packaging-group', 'product', 'distribution'];
        typeOrder.forEach(type => {
            const nodes = allNodes.filter(n => n.type === type);
            if (nodes.length === 0) return;
            const step = STEPS.find(s => s.type === type);
            html.push(`<div class="te-tree-section-label">${step?.label || type}</div>`);
            nodes.forEach(node => {
                const connectedTo = getConnectedTo(node.id);
                const isActive = expandedCardId === node.id;
                html.push(`<div class="te-tree-item${isActive ? ' te-active' : ''}" data-node-id="${node.id}" title="${esc(node.name)}">
                    <i class="bi ${step?.icon || 'bi-dot'} te-tree-icon" style="color:${step?.color || '#6c757d'}; font-size:0.8rem;"></i>
                    <span style="flex:1; min-width:0; overflow:hidden; text-overflow:ellipsis; white-space:nowrap;">${esc(node.name)}</span>
                    ${connectedTo.length > 0 ? `<i class="bi bi-arrow-right-short" style="color:#adb5bd; font-size:0.75rem; flex-shrink:0;"></i>` : ''}
                </div>`);
                if (connectedTo.length > 0) {
                    html.push('<div class="te-tree-indent">');
                    connectedTo.forEach(child => {
                        const childStep = STEPS.find(s => s.type === child.type);
                        html.push(`<div class="te-tree-item${expandedCardId === child.id ? ' te-active' : ''}" data-node-id="${child.id}" title="${esc(child.name)}">
                            <i class="bi ${childStep?.icon || 'bi-dot'} te-tree-icon" style="color:${childStep?.color || '#6c757d'}; font-size:0.8rem;"></i>
                            <span style="flex:1; min-width:0; overflow:hidden; text-overflow:ellipsis; white-space:nowrap;">${esc(child.name)}</span>
                        </div>`);
                    });
                    html.push('</div>');
                }
            });
        });

        container.innerHTML = html.join('');
        container.querySelectorAll('.te-tree-item[data-node-id]').forEach(el => {
            el.addEventListener('click', () => {
                const nodeId = el.dataset.nodeId;
                const node = getNodeById(nodeId);
                if (!node) return;
                const stepIdx = STEPS.findIndex(s => s.type === node.type);
                if (stepIdx >= 0) {
                    expandedCardId = nodeId;
                    addingInStep = false;
                    goToStep(stepIdx);
                }
            });
        });
    }

    // ─── Step rendering ───────────────────────────────────────────────────────
    function renderStep() {
        const step = STEPS[currentStep];
        renderStepHeader(step);
        renderStepBody(step);
        renderStepFooter();
    }

    function renderStepHeader(step) {
        const el = document.getElementById('teStepHeader');
        if (!el) return;
        el.innerHTML = `<div class="d-flex align-items-center gap-3">
            <div class="te-step-header-icon" style="background:${step.bg}; color:${step.color};">
                <i class="bi ${step.icon}"></i>
            </div>
            <div>
                <h5 class="mb-0" style="font-weight:600;">Step ${currentStep + 1} · ${step.label}</h5>
                <small class="text-muted">${step.desc}</small>
            </div>
            ${currentStep < 5 ? `<span class="ms-auto badge bg-light text-muted border" style="font-size:0.8rem;">${getNodesOfType(step.type).length} item${getNodesOfType(step.type).length !== 1 ? 's' : ''}</span>` : ''}
        </div>`;
    }

    function renderStepBody(step) {
        const el = document.getElementById('teStepBody');
        if (!el) return;
        if (step.type === 'review') {
            el.innerHTML = renderReview();
            return;
        }
        const nodes = getNodesOfType(step.type);
        const html = [];

        nodes.forEach(node => {
            const isExpanded = expandedCardId === node.id;
            html.push(renderNodeCard(node, step, isExpanded));
        });

        // Add button / form
        if (addingInStep) {
            html.push(renderAddForm(step));
        } else {
            html.push(`<button type="button" class="te-add-card" id="teAddNodeBtn">
                <i class="bi bi-plus-circle" style="font-size:1rem;"></i>
                <span>Add ${step.label.replace(/s$/, '')}…</span>
            </button>`);
        }

        if (nodes.length === 0 && !addingInStep) {
            el.innerHTML = `<div class="te-empty-state">
                <span class="te-empty-icon"><i class="bi ${step.icon}" style="color:${step.color};"></i></span>
                <div style="font-size:0.95rem; font-weight:500; color:#6c757d;">No ${step.label.toLowerCase()} yet</div>
                <small class="text-muted d-block mt-1 mb-3">${step.desc}</small>
            </div>` + html.join('');
        } else {
            el.innerHTML = html.join('');
        }

        // Bind events
        el.querySelectorAll('.te-node-card[data-node-id]').forEach(card => {
            const nodeId = card.dataset.nodeId;
            card.querySelector('.te-card-header')?.addEventListener('click', (e) => {
                if (e.target.closest('button')) return;
                expandedCardId = expandedCardId === nodeId ? null : nodeId;
                addingInStep = false;
                renderStep();
                renderTree();
            });
            card.querySelector('.te-delete-node-btn')?.addEventListener('click', (e) => {
                e.stopPropagation();
                const node = getNodeById(nodeId);
                if (!confirm(`Delete "${node?.name}"? All connections to this item will also be removed.`)) return;
                if (expandedCardId === nodeId) expandedCardId = null;
                deleteNode(nodeId);
                renderAll();
            });
            card.querySelector('.te-save-node-btn')?.addEventListener('click', (e) => {
                e.stopPropagation();
                saveNodeEdit(card, nodeId);
            });
            card.querySelectorAll('.te-pill-remove').forEach(btn => {
                btn.addEventListener('click', (e) => {
                    e.stopPropagation();
                    const fromId = btn.dataset.from;
                    const toId = btn.dataset.to;
                    removeConnection(fromId, toId);
                    renderStep();
                    renderTree();
                });
            });
            card.querySelectorAll('.te-conn-add-select').forEach(sel => {
                sel.addEventListener('change', (e) => {
                    const targetId = e.target.value;
                    if (!targetId) return;
                    e.target.value = '';
                    const direction = e.target.dataset.direction;
                    if (direction === 'to') {
                        addConnection(nodeId, targetId);
                    } else {
                        addConnection(targetId, nodeId);
                    }
                    renderStep();
                    renderTree();
                });
            });
        });

        el.querySelector('#teAddNodeBtn')?.addEventListener('click', () => {
            addingInStep = true;
            expandedCardId = null;
            renderStep();
            // Focus first input
            setTimeout(() => el.querySelector('.te-add-form input, .te-add-form select')?.focus(), 50);
        });

        el.querySelector('#teCancelAddBtn')?.addEventListener('click', () => {
            addingInStep = false;
            renderStep();
        });

        el.querySelector('#teSaveAddBtn')?.addEventListener('click', () => {
            const form = el.querySelector('.te-add-form');
            if (!form) return;
            const params = collectFormData(form, step.type);
            if (!params) return;
            const node = addNode(step.type, params);
            addingInStep = false;
            expandedCardId = node.id;
            renderAll();
        });
    }

    // ─── Node card ────────────────────────────────────────────────────────────
    function renderNodeCard(node, step, isExpanded) {
        const meta = buildNodeMeta(node, step.type);
        const upstreamType = REVERSE_CONNECTION_MAP[step.type];
        const downstreamType = CONNECTION_MAP[step.type];
        const connFrom = getConnectedFrom(node.id);
        const connTo = getConnectedTo(node.id);

        let connHtml = '';
        if (isExpanded) {
            // Upstream connections
            if (upstreamType) {
                const upstreamNodes = getNodesOfType(upstreamType);
                const upLabel = STEPS.find(s => s.type === upstreamType)?.label || upstreamType;
                const upStep = STEPS.find(s => s.type === upstreamType);
                const usedFromIds = connFrom.map(n => n.id);
                connHtml += `<div class="te-conn-section">
                    <div class="te-conn-label"><i class="bi bi-arrow-left-short"></i> From ${upLabel}</div>
                    <div class="te-conn-pills">
                        ${connFrom.map(n => `<span class="te-conn-pill">
                            <i class="bi ${upStep?.icon || 'bi-dot'}" style="color:${upStep?.color};"></i>
                            ${esc(n.name)}
                            <span class="te-pill-remove" data-from="${n.id}" data-to="${node.id}" title="Remove connection">×</span>
                        </span>`).join('')}
                        ${upstreamNodes.filter(n => !usedFromIds.includes(n.id)).length > 0 ? `
                        <select class="te-conn-add-select" data-direction="from" aria-label="Add from ${upLabel}">
                            <option value="">+ Link ${upLabel.replace(/s$/, '')}…</option>
                            ${upstreamNodes.filter(n => !usedFromIds.includes(n.id)).map(n =>
                                `<option value="${n.id}">${esc(n.name)}</option>`
                            ).join('')}
                        </select>` : ''}
                    </div>
                </div>`;
            }
            // Downstream connections
            if (downstreamType) {
                const downstreamNodes = getNodesOfType(downstreamType);
                const downLabel = STEPS.find(s => s.type === downstreamType)?.label || downstreamType;
                const downStep = STEPS.find(s => s.type === downstreamType);
                const usedToIds = connTo.map(n => n.id);
                connHtml += `<div class="te-conn-section">
                    <div class="te-conn-label"><i class="bi bi-arrow-right-short"></i> To ${downLabel}</div>
                    <div class="te-conn-pills">
                        ${connTo.map(n => `<span class="te-conn-pill">
                            <i class="bi ${downStep?.icon || 'bi-dot'}" style="color:${downStep?.color};"></i>
                            ${esc(n.name)}
                            <span class="te-pill-remove" data-from="${node.id}" data-to="${n.id}" title="Remove connection">×</span>
                        </span>`).join('')}
                        ${downstreamNodes.filter(n => !usedToIds.includes(n.id)).length > 0 ? `
                        <select class="te-conn-add-select" data-direction="to" aria-label="Add to ${downLabel}">
                            <option value="">+ Link ${downLabel.replace(/s$/, '')}…</option>
                            ${downstreamNodes.filter(n => !usedToIds.includes(n.id)).map(n =>
                                `<option value="${n.id}">${esc(n.name)}</option>`
                            ).join('')}
                        </select>` : ''}
                    </div>
                </div>`;
            }
        } else {
            // Collapsed: show connection summary as pills
            const allConn = [...connFrom, ...connTo];
            if (allConn.length > 0) {
                connHtml = `<div class="te-conn-pills mt-2">
                    ${connFrom.map(n => {
                        const s = STEPS.find(st => st.type === n.type);
                        return `<span class="te-conn-pill" style="background:#f3f0ff;color:${s?.color || '#6c757d'};border-color:#d8cff8;">
                            <i class="bi ${s?.icon || 'bi-dot'}"></i>${esc(n.name)}
                        </span>`;
                    }).join('')}
                    ${connTo.map(n => {
                        const s = STEPS.find(st => st.type === n.type);
                        return `<span class="te-conn-pill" style="background:${s?.bg || '#f0f4ff'};color:${s?.color || '#0d6efd'};border-color:#c8d9ff;">
                            <i class="bi ${s?.icon || 'bi-dot'}"></i>${esc(n.name)}
                        </span>`;
                    }).join('')}
                </div>`;
            }
        }

        const editFormHtml = isExpanded ? renderEditForm(node, step.type) : '';

        return `<div class="te-node-card${isExpanded ? ' te-card-expanded' : ''}" data-node-id="${node.id}">
            <div class="te-card-header">
                <div class="te-card-icon" style="background:${step.bg}; color:${step.color};">
                    <i class="bi ${step.icon}"></i>
                </div>
                <div style="flex:1; min-width:0;">
                    <div style="font-weight:600; font-size:0.9rem; white-space:nowrap; overflow:hidden; text-overflow:ellipsis;">${esc(node.name)}</div>
                    <div class="te-card-meta">${meta}</div>
                </div>
                <div class="d-flex gap-1 ms-2 flex-shrink-0">
                    <button type="button" class="btn btn-sm btn-light border-0 te-delete-node-btn" title="Delete" style="color:#dc3545; padding:0.2rem 0.4rem;">
                        <i class="bi bi-trash" style="font-size:0.8rem;"></i>
                    </button>
                    <button type="button" class="btn btn-sm btn-light border-0" title="${isExpanded ? 'Collapse' : 'Edit'}" style="padding:0.2rem 0.4rem;">
                        <i class="bi bi-chevron-${isExpanded ? 'up' : 'down'}" style="font-size:0.8rem;"></i>
                    </button>
                </div>
            </div>
            ${connHtml && !isExpanded ? connHtml : ''}
            ${isExpanded ? `<div class="te-card-body">${editFormHtml}${connHtml}</div>` : ''}
        </div>`;
    }

    function buildNodeMeta(node, type) {
        const p = node.parameters || {};
        const parts = [];
        if (type === 'raw-material' && p.weight) parts.push(`<span class="badge bg-light text-muted border">${p.weight}g</span>`);
        if (type === 'packaging') {
            if (p.height) parts.push(`<span class="badge bg-light text-muted border">${p.height}mm tall</span>`);
            if (p.weight) parts.push(`<span class="badge bg-light text-muted border">${p.weight}g</span>`);
        }
        if (type === 'packaging-group' && p.layer) {
            const layerColors = { Primary: '#6f42c1', Secondary: '#0d6efd', Tertiary: '#198754', Quaternary: '#fd7e14' };
            const c = layerColors[p.layer] || '#6c757d';
            parts.push(`<span class="badge" style="background:${c}20;color:${c};border:1px solid ${c}40;">${p.layer}</span>`);
        }
        if (type === 'product') {
            if (p.sku) parts.push(`<span class="badge bg-light text-muted border">${esc(p.sku)}</span>`);
            if (p.weight) parts.push(`<span class="badge bg-light text-muted border">${p.weight}g</span>`);
        }
        if (type === 'distribution') {
            if (p.city) parts.push(`<span class="badge bg-light text-muted border"><i class="bi bi-geo-alt me-1"></i>${esc(p.city)}</span>`);
            if (p.type) parts.push(`<span class="badge bg-light text-muted border">${esc(p.type)}</span>`);
        }
        if (p.description && !parts.length) {
            parts.push(`<span class="text-muted" style="font-size:0.78rem;">${esc(p.description.substring(0, 60))}</span>`);
        }
        return parts.join('');
    }

    // ─── Edit form ────────────────────────────────────────────────────────────
    function renderEditForm(node, type) {
        const fields = NODE_FIELDS[type] || [];
        const p = node.parameters || {};
        const rows = [];
        const textFields = fields.filter(f => f.type !== 'textarea');
        for (let i = 0; i < textFields.length; i += 2) {
            const f1 = textFields[i];
            const f2 = textFields[i + 1];
            rows.push(`<div class="te-form-row${!f2 ? ' te-single' : ''}">
                ${renderFormField(f1, p[f1.key] ?? '', 'te-edit-field')}
                ${f2 ? renderFormField(f2, p[f2.key] ?? '', 'te-edit-field') : ''}
            </div>`);
        }
        const textareas = fields.filter(f => f.type === 'textarea');
        textareas.forEach(f => {
            rows.push(`<div class="te-form-row te-single">${renderFormField(f, p[f.key] ?? '', 'te-edit-field')}</div>`);
        });
        return `<div class="te-edit-form mb-3" data-node-id="${node.id}">
            ${rows.join('')}
            <div class="d-flex gap-2 mt-3">
                <button type="button" class="btn btn-sm btn-primary te-save-node-btn">Save changes</button>
            </div>
        </div>`;
    }

    function renderFormField(field, value, cls) {
        const attrs = `class="form-control form-control-sm ${cls}" data-field="${field.key}" ${field.required ? 'required' : ''}`;
        let input;
        if (field.type === 'select') {
            const opts = field.options.map(o => `<option value="${o}"${o === value ? ' selected' : ''}>${o}</option>`).join('');
            input = `<select ${attrs}>${opts}</select>`;
        } else if (field.type === 'textarea') {
            input = `<textarea ${attrs} rows="2" placeholder="${field.placeholder || ''}">${esc(String(value))}</textarea>`;
        } else {
            input = `<input type="${field.type}" ${attrs} value="${esc(String(value))}" placeholder="${field.placeholder || ''}">`;
        }
        return `<div><label class="form-label form-label-sm mb-1" style="font-size:0.78rem;font-weight:600;color:#495057;">${field.label}${field.required ? ' <span class="text-danger">*</span>' : ''}</label>${input}</div>`;
    }

    function collectFormData(form, type) {
        const fields = NODE_FIELDS[type] || [];
        const params = {};
        let valid = true;
        fields.forEach(field => {
            const el = form.querySelector(`[data-field="${field.key}"]`);
            if (!el) return;
            const val = el.value.trim();
            if (field.required && !val) {
                el.classList.add('is-invalid');
                valid = false;
            } else {
                el.classList.remove('is-invalid');
                params[field.key] = field.type === 'number' ? (val ? parseFloat(val) : null) : val;
            }
        });
        if (!valid) return null;
        return params;
    }

    function saveNodeEdit(card, nodeId) {
        const form = card.querySelector('.te-edit-form');
        if (!form) return;
        const node = getNodeById(nodeId);
        if (!node) return;
        const params = collectFormData(form, node.type);
        if (!params) return;
        updateNode(nodeId, params);
        renderAll();
    }

    // ─── Add form ─────────────────────────────────────────────────────────────
    function renderAddForm(step) {
        if (step.type === 'review') return '';
        const fields = NODE_FIELDS[step.type] || [];
        const rows = [];
        const textFields = fields.filter(f => f.type !== 'textarea');
        for (let i = 0; i < textFields.length; i += 2) {
            const f1 = textFields[i];
            const f2 = textFields[i + 1];
            rows.push(`<div class="te-form-row${!f2 ? ' te-single' : ''}">
                ${renderFormField(f1, f1.type === 'select' ? (f1.options?.[0] || '') : '', 'te-add-field')}
                ${f2 ? renderFormField(f2, f2.type === 'select' ? (f2.options?.[0] || '') : '', 'te-add-field') : ''}
            </div>`);
        }
        const textareas = fields.filter(f => f.type === 'textarea');
        textareas.forEach(f => {
            rows.push(`<div class="te-form-row te-single">${renderFormField(f, '', 'te-add-field')}</div>`);
        });
        return `<div class="te-node-card te-card-expanded te-add-form" style="border-style:dashed;">
            <div class="d-flex align-items-center gap-2 mb-3">
                <div class="te-card-icon" style="background:${step.bg};color:${step.color};"><i class="bi ${step.icon}"></i></div>
                <span style="font-weight:600;">New ${step.label.replace(/s$/, '')}</span>
            </div>
            ${rows.join('')}
            <div class="d-flex gap-2 mt-3">
                <button type="button" class="btn btn-sm btn-primary" id="teSaveAddBtn">Add ${step.label.replace(/s$/, '')}</button>
                <button type="button" class="btn btn-sm btn-outline-secondary" id="teCancelAddBtn">Cancel</button>
            </div>
        </div>`;
    }

    // ─── Step footer ──────────────────────────────────────────────────────────
    function renderStepFooter() {
        const el = document.getElementById('teStepFooter');
        if (!el) return;
        const isFirst = currentStep === 0;
        const isLast = currentStep === STEPS.length - 1;
        el.innerHTML = `
            <button type="button" class="btn btn-sm btn-outline-secondary${isFirst ? ' invisible' : ''}" id="teBackBtn">
                <i class="bi bi-arrow-left me-1"></i> Back
            </button>
            <span class="text-muted" style="font-size:0.8rem;">${currentStep + 1} / ${STEPS.length}</span>
            <button type="button" class="btn btn-sm btn-${isLast ? 'success' : 'primary'}" id="teNextBtn">
                ${isLast ? '<i class="bi bi-check2 me-1"></i> Done' : 'Next <i class="bi bi-arrow-right ms-1"></i>'}
            </button>`;
        document.getElementById('teBackBtn')?.addEventListener('click', () => goToStep(currentStep - 1));
        document.getElementById('teNextBtn')?.addEventListener('click', () => {
            if (isLast) {
                goToStep(0);
            } else {
                goToStep(currentStep + 1);
            }
        });
    }

    // ─── Review ───────────────────────────────────────────────────────────────
    function renderReview() {
        if (!currentProject || !currentProject.nodes || currentProject.nodes.length === 0) {
            return `<div class="te-empty-state">
                <span class="te-empty-icon"><i class="bi bi-check-circle" style="color:#20c997;"></i></span>
                <div style="font-size:0.95rem;font-weight:500;color:#6c757d;">Nothing to review yet</div>
                <small class="text-muted">Add nodes in the previous steps to build your supply chain.</small>
            </div>`;
        }

        const chains = buildChains();
        if (chains.length === 0) {
            // Show flat node list if no connections
            const html = ['<div class="mb-3"><p class="text-muted small">No connections defined yet. The supply chain nodes are listed below.</p></div>'];
            const typeOrder = ['raw-material', 'packaging', 'packaging-group', 'product', 'distribution'];
            typeOrder.forEach(type => {
                const nodes = getNodesOfType(type);
                if (!nodes.length) return;
                const step = STEPS.find(s => s.type === type);
                html.push(`<div class="mb-3"><div class="te-conn-label mb-2">${step?.label}</div>`);
                nodes.forEach(node => {
                    html.push(`<div class="te-review-chain">
                        <span class="te-review-chain-node" style="background:${step?.bg};color:${step?.color};">
                            <i class="bi ${step?.icon}"></i> ${esc(node.name)}
                        </span>
                    </div>`);
                });
                html.push('</div>');
            });
            return html.join('');
        }

        const html = ['<div class="mb-1"><p class="text-muted small mb-3">Each row shows a complete path through your supply chain, from raw material to distribution.</p></div>'];
        chains.forEach(chain => {
            const pills = chain.map(node => {
                const step = STEPS.find(s => s.type === node.type);
                return `<span class="te-review-chain-node" style="background:${step?.bg || '#f0f4ff'};color:${step?.color || '#0d6efd'};">
                    <i class="bi ${step?.icon || 'bi-dot'}"></i> ${esc(node.name)}
                </span>`;
            }).join('<span class="te-review-arrow"><i class="bi bi-arrow-right"></i></span>');
            html.push(`<div class="te-review-chain">${pills}</div>`);
        });
        return html.join('');
    }

    function buildChains() {
        // Find all end-nodes (nodes with no outgoing connections or last in ORDER)
        const allNodes = currentProject?.nodes || [];
        const connections = currentProject?.connections || [];
        if (!connections.length) return [];

        // Find source nodes (not pointed to by any connection → "roots")
        const pointedTo = new Set(connections.map(c => c.to));
        const sources = allNodes.filter(n => !pointedTo.has(n.id));

        const chains = [];
        function dfs(node, path) {
            const next = connections.filter(c => c.from === node.id).map(c => allNodes.find(n => n.id === c.to)).filter(Boolean);
            if (next.length === 0) {
                if (path.length > 1) chains.push([...path]);
            } else {
                next.forEach(child => dfs(child, [...path, child]));
            }
        }
        sources.forEach(s => dfs(s, [s]));
        return chains.slice(0, 50); // Cap for performance
    }

    // ─── Utilities ────────────────────────────────────────────────────────────
    function esc(str) {
        return String(str ?? '')
            .replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;').replace(/'/g, '&#39;');
    }

    // ─── Public API ───────────────────────────────────────────────────────────
    window.initTextEditor = function () {
        loadFromStorage();
        renderAll();
    };

    window.refreshTextEditor = function () {
        loadFromStorage();
        const container = document.getElementById('textEditorContainer');
        if (container && !container.classList.contains('d-none')) {
            renderAll();
        }
    };

})();
