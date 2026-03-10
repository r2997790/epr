/**
 * Text Editor — Step-by-step text-first supply chain interface.
 * Syncs with the Visual Editor via shared localStorage key.
 */
(function () {
    'use strict';

    // ─── Constants ────────────────────────────────────────────────────────────
    const STORAGE_KEY = 'eprSavedProjects-epr-instance-packaging-management';

    const STEPS = [
        {
            type: 'raw-material',
            label: 'Raw Materials',
            icon: 'bi-circle',
            color: '#6f42c1', bg: '#f3f0ff',
            desc: 'Define the raw materials your packaging is made from — e.g. PET plastic, glass, aluminium.'
        },
        {
            type: 'packaging',
            label: 'Packaging',
            icon: 'bi-box-seam',
            color: '#0d6efd', bg: '#e7f3ff',
            desc: 'Define the physical packaging items — bottles, cans, trays, labels, caps, etc.'
        },
        {
            type: 'supplier-packaging',
            label: 'Suppliers',
            icon: 'bi-shop',
            color: '#0891b2', bg: '#ecfeff',
            desc: 'Track which supplier companies provide specific packaging to your products.'
        },
        {
            type: 'packaging-group',
            label: 'Groups',
            icon: 'bi-collection',
            color: '#198754', bg: '#e9f7ef',
            desc: 'Group packaging into layers — Primary (touches product), Secondary, Tertiary, etc.'
        },
        {
            type: 'product',
            label: 'Products',
            icon: 'bi-bag',
            color: '#fd7e14', bg: '#fff3e9',
            desc: 'Define the finished products. Link them to packaging groups and supplier packaging.'
        },
        {
            type: 'distribution',
            label: 'Distribution',
            icon: 'bi-geo-alt',
            color: '#dc3545', bg: '#fdf0f0',
            desc: 'Define distribution points — stores, warehouses, hubs — where products are sent.'
        },
        {
            type: 'review',
            label: 'Review',
            icon: 'bi-check-circle',
            color: '#20c997', bg: '#e9faf5',
            desc: 'Review the complete supply chain chains from raw material through to distribution.'
        }
    ];

    // Valid connections: [from, to]
    const CONNECTION_RULES = [
        ['raw-material',    'packaging'],
        ['packaging',       'packaging-group'],
        ['supplier-packaging', 'product'],
        ['packaging-group', 'product'],
        ['product',         'distribution']
    ];

    // Downstream: what types can this type connect TO?
    const DOWNSTREAM = {};
    // Upstream: what types can connect TO this type?
    const UPSTREAM = {};
    CONNECTION_RULES.forEach(([from, to]) => {
        if (!DOWNSTREAM[from]) DOWNSTREAM[from] = [];
        DOWNSTREAM[from].push(to);
        if (!UPSTREAM[to]) UPSTREAM[to] = [];
        UPSTREAM[to].push(from);
    });

    const NODE_FIELDS = {
        'raw-material': [
            { key: 'name',        label: 'Name',         type: 'text',   required: true,  placeholder: 'e.g. PET Plastic' },
            { key: 'description', label: 'Description',  type: 'text',   placeholder: 'e.g. Recycled PET resin' },
            { key: 'weight',      label: 'Weight (g)',   type: 'number', placeholder: '0' },
            { key: 'notes',       label: 'Notes',        type: 'textarea', placeholder: 'e.g. Min 30% recycled content required' }
        ],
        'packaging': [
            { key: 'name',        label: 'Name',         type: 'text',   required: true, placeholder: 'e.g. 500ml PET Bottle' },
            { key: 'description', label: 'Description',  type: 'text',   placeholder: 'e.g. Clear lightweight bottle' },
            { key: 'height',      label: 'Height (mm)',  type: 'number', placeholder: '0' },
            { key: 'weight',      label: 'Weight (g)',   type: 'number', placeholder: '0' },
            { key: 'depth',       label: 'Depth (mm)',   type: 'number', placeholder: '0' },
            { key: 'notes',       label: 'Notes',        type: 'textarea', placeholder: 'Additional notes' }
        ],
        'supplier-packaging': [
            { key: 'name',         label: 'Name',             type: 'text', required: true, placeholder: 'e.g. BevPak – 500ml Bottle' },
            { key: 'companyName',  label: 'Supplier Company', type: 'text', placeholder: 'e.g. BevPak Ltd' },
            { key: 'description',  label: 'Description',      type: 'text', placeholder: 'e.g. Preferred approved supplier' },
            { key: 'contactEmail', label: 'Contact / Email',  type: 'text', placeholder: 'e.g. orders@supplier.com' },
            { key: 'certifications', label: 'Certifications', type: 'text', placeholder: 'e.g. ISO 9001, BRC Grade A' },
            { key: 'notes',        label: 'Notes',            type: 'textarea', placeholder: 'Lead time, MOQ, payment terms, etc.' }
        ],
        'packaging-group': [
            { key: 'name',         label: 'Name',          type: 'text',   required: true, placeholder: 'e.g. Primary Pack' },
            { key: 'layer',        label: 'Layer',         type: 'select', options: ['Primary', 'Secondary', 'Tertiary', 'Quaternary'] },
            { key: 'holdsQuantity',label: 'Holds Quantity',type: 'number', placeholder: '1' }
        ],
        'product': [
            { key: 'name',        label: 'Name',         type: 'text',   required: true, placeholder: 'e.g. Sparkling Water 500ml' },
            { key: 'sku',         label: 'SKU',          type: 'text',   placeholder: 'e.g. SW-500' },
            { key: 'description', label: 'Description',  type: 'text',   placeholder: 'e.g. Natural sparkling mineral water' },
            { key: 'weight',      label: 'Net Weight (g)',type: 'number', placeholder: '0' },
            { key: 'quantity',    label: 'Annual Qty',   type: 'number', placeholder: '0' }
        ],
        'distribution': [
            { key: 'name',        label: 'Location Name',   type: 'text',   required: true, placeholder: 'e.g. London – Central DC' },
            { key: 'companyName', label: 'Company',         type: 'text',   placeholder: 'e.g. AquaCo Ltd' },
            { key: 'siteName',    label: 'Site Name',       type: 'text',   placeholder: 'e.g. Canning Town Hub' },
            { key: 'city',        label: 'City',            type: 'text',   placeholder: 'e.g. London' },
            { key: 'country',     label: 'Country',         type: 'text',   placeholder: 'e.g. UK' },
            { key: 'type',        label: 'Type',            type: 'select', options: ['Store', 'Warehouse', 'Hub', 'Distribution Centre'] }
        ]
    };

    // Auto-layout X positions by node type
    const LAYOUT_X = {
        'raw-material': 80, 'packaging': 320, 'supplier-packaging': 480,
        'packaging-group': 620, 'product': 890, 'distribution': 1160
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
            document.getElementById('teTreeRefreshBtn')?.addEventListener('click', renderTree);
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
            allProjects.push(createExampleProject());
            saveToStorage();
        }

        if (!currentProject) {
            currentProject = allProjects[0];
        } else {
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
        badge.innerHTML = '<i class="bi bi-check-circle-fill me-1" style="color:#059669"></i>Saved';
        setTimeout(() => {
            badge.style.background = '#f0f4ff';
            badge.innerHTML = '<i class="bi bi-arrow-repeat me-1"></i>Synced with Visual Editor';
        }, 1800);
    }

    // ─── Example data ─────────────────────────────────────────────────────────
    function createExampleProject() {
        const nodes = [
            // Raw Materials
            { id: 'ex-rm-1', type: 'raw-material', x: 80,  y: 80,  name: 'rPET Plastic',       icon: 'bi-circle', locked: false, parameters: { name: 'rPET Plastic',       description: 'Recycled PET resin, min 30% PCR', weight: 25,  notes: 'Preferred: 30–50% post-consumer recycled content' } },
            { id: 'ex-rm-2', type: 'raw-material', x: 80,  y: 220, name: 'Recycled Cardboard',  icon: 'bi-circle', locked: false, parameters: { name: 'Recycled Cardboard',  description: 'FSC-certified corrugated board',   weight: 180, notes: 'Minimum FSC-Mix 70% certified' } },
            { id: 'ex-rm-3', type: 'raw-material', x: 80,  y: 360, name: 'Food-Grade Ink',      icon: 'bi-circle', locked: false, parameters: { name: 'Food-Grade Ink',      description: 'Water-based label ink',           weight: 2,   notes: '' } },

            // Packaging
            { id: 'ex-pk-1', type: 'packaging', x: 320, y: 80,  name: '500ml PET Bottle',    icon: 'bi-box-seam', locked: false, parameters: { name: '500ml PET Bottle',    description: 'Clear lightweight bottle',        height: 215, weight: 25, depth: 70,  notes: '' } },
            { id: 'ex-pk-2', type: 'packaging', x: 320, y: 220, name: 'Corrugated Shelf Tray',icon: 'bi-box-seam', locked: false, parameters: { name: 'Corrugated Shelf Tray',description: 'Holds 24 bottles, retail-ready',  height: 100, weight: 180, depth: 370, notes: '' } },
            { id: 'ex-pk-3', type: 'packaging', x: 320, y: 360, name: 'Pressure-Sensitive Label',icon:'bi-box-seam',locked: false, parameters: { name: 'Pressure-Sensitive Label', description: 'BOPP wrap-around label',      height: 65,  weight: 2,  depth: 210, notes: '' } },

            // Supplier Packaging
            { id: 'ex-sp-1', type: 'supplier-packaging', x: 480, y: 80,  name: 'BevPak – 500ml Clear PET', icon: 'bi-shop', locked: false, parameters: { name: 'BevPak – 500ml Clear PET', companyName: 'BevPak Ltd',      description: 'Approved Tier 1 supplier',      contactEmail: 'orders@bevpak.com',    certifications: 'ISO 9001, BRC Grade A', notes: '12-week lead time; MOQ 100,000 units' } },
            { id: 'ex-sp-2', type: 'supplier-packaging', x: 480, y: 220, name: 'PackCo – Shelf Tray',       icon: 'bi-shop', locked: false, parameters: { name: 'PackCo – Shelf Tray',       companyName: 'PackCo Europe',   description: 'Secondary packaging supplier',  contactEmail: 'sales@packco.eu',      certifications: 'ISO 14001, FSC COC',    notes: '4-week lead time' } },

            // Packaging Groups
            { id: 'ex-pg-1', type: 'packaging-group', x: 620, y: 80,  name: 'Primary Pack',          icon: 'bi-collection', locked: false, parameters: { name: 'Primary Pack',          layer: 'Primary',   holdsQuantity: 1 } },
            { id: 'ex-pg-2', type: 'packaging-group', x: 620, y: 220, name: 'Shelf-Ready Tray (24s)', icon: 'bi-collection', locked: false, parameters: { name: 'Shelf-Ready Tray (24s)', layer: 'Secondary', holdsQuantity: 24 } },

            // Products
            { id: 'ex-pr-1', type: 'product', x: 890, y: 80, name: 'Sparkling Water 500ml', icon: 'bi-bag', locked: false, parameters: { name: 'Sparkling Water 500ml', sku: 'SW-500', description: 'Natural sparkling mineral water', weight: 500, quantity: 2400000 } },

            // Distribution
            { id: 'ex-di-1', type: 'distribution', x: 1160, y: 80,  name: 'London – Central DC',   icon: 'bi-geo-alt', locked: false, parameters: { name: 'London – Central DC',   companyName: 'AquaCo Ltd', siteName: 'Canning Town Hub',     city: 'London',     country: 'UK', type: 'Hub' } },
            { id: 'ex-di-2', type: 'distribution', x: 1160, y: 220, name: 'Manchester – North DC',  icon: 'bi-geo-alt', locked: false, parameters: { name: 'Manchester – North DC',  companyName: 'AquaCo Ltd', siteName: 'Trafford Park Depot',  city: 'Manchester', country: 'UK', type: 'Warehouse' } }
        ];

        const connections = [
            // Raw material → Packaging
            { from: 'ex-rm-1', to: 'ex-pk-1', fromPort: 'right', toPort: 'left' },
            { from: 'ex-rm-2', to: 'ex-pk-2', fromPort: 'right', toPort: 'left' },
            { from: 'ex-rm-3', to: 'ex-pk-3', fromPort: 'right', toPort: 'left' },
            // Packaging → Groups
            { from: 'ex-pk-1', to: 'ex-pg-1', fromPort: 'right', toPort: 'left' },
            { from: 'ex-pk-3', to: 'ex-pg-1', fromPort: 'right', toPort: 'left' },
            { from: 'ex-pk-2', to: 'ex-pg-2', fromPort: 'right', toPort: 'left' },
            // Groups → Product
            { from: 'ex-pg-1', to: 'ex-pr-1', fromPort: 'right', toPort: 'left' },
            { from: 'ex-pg-2', to: 'ex-pr-1', fromPort: 'right', toPort: 'left' },
            // Supplier Packaging → Product
            { from: 'ex-sp-1', to: 'ex-pr-1', fromPort: 'right', toPort: 'left' },
            { from: 'ex-sp-2', to: 'ex-pr-1', fromPort: 'right', toPort: 'left' },
            // Product → Distribution
            { from: 'ex-pr-1', to: 'ex-di-1', fromPort: 'right', toPort: 'left' },
            { from: 'ex-pr-1', to: 'ex-di-2', fromPort: 'right', toPort: 'left' }
        ];

        return { projectName: 'Example: Sparkling Water 500ml', nodes, connections, savedAt: new Date().toISOString() };
    }

    function createBlankProject(name) {
        return { projectName: name, nodes: [], connections: [], savedAt: new Date().toISOString() };
    }

    // ─── Project helpers ──────────────────────────────────────────────────────
    function getNodesOfType(type) {
        return (currentProject?.nodes || []).filter(n => n.type === type);
    }

    function getNodeById(id) {
        return (currentProject?.nodes || []).find(n => n.id === id);
    }

    function getConnectedTo(nodeId) {
        return (currentProject?.connections || [])
            .filter(c => c.from === nodeId)
            .map(c => getNodeById(c.to)).filter(Boolean);
    }

    function getConnectedFrom(nodeId) {
        return (currentProject?.connections || [])
            .filter(c => c.to === nodeId)
            .map(c => getNodeById(c.from)).filter(Boolean);
    }

    function connectionExists(fromId, toId) {
        return (currentProject?.connections || []).some(c => c.from === fromId && c.to === toId);
    }

    function generateNodeId() {
        return `node-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
    }

    function assignCoordinates(type) {
        const x = LAYOUT_X[type] || 100;
        const y = 80 + getNodesOfType(type).length * 140;
        return { x, y };
    }

    // ─── CRUD ─────────────────────────────────────────────────────────────────
    function addNode(type, params) {
        const { x, y } = assignCoordinates(type);
        const step = STEPS.find(s => s.type === type);
        const node = {
            id: generateNodeId(), type, x, y,
            name: params.name || 'Untitled',
            icon: step?.icon || 'bi-square',
            locked: false,
            parameters: { ...params }
        };
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
        currentProject.savedAt = new Date().toISOString();
        saveToStorage();
    }

    function deleteNode(id) {
        if (!currentProject) return;
        currentProject.nodes = currentProject.nodes.filter(n => n.id !== id);
        currentProject.connections = currentProject.connections.filter(c => c.from !== id && c.to !== id);
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
        currentProject.connections = currentProject.connections.filter(c => !(c.from === fromId && c.to === toId));
        currentProject.savedAt = new Date().toISOString();
        saveToStorage();
    }

    // ─── Render ───────────────────────────────────────────────────────────────
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
        const delBtn  = document.getElementById('teDeleteProjectBtn');
        const renBtn  = document.getElementById('teRenameProjectBtn');
        if (delBtn)  delBtn.disabled  = !hasPrj;
        if (renBtn)  renBtn.disabled  = !hasPrj;
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
            const name = prompt('New project name:', 'My Supply Chain');
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
            if (allProjects.length === 0) allProjects.push(createBlankProject('My Supply Chain'));
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
            const isDone = i < STEPS.length - 1 && getNodesOfType(step.type).length > 0 && i !== currentStep;
            if (isDone) pill.classList.add('done');
            const numContent = isDone ? '<i class="bi bi-check" style="font-size:0.7rem"></i>' : (i + 1);
            pill.innerHTML = `<span class="te-step-num">${numContent}</span>${step.label}`;
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
        if (!currentProject?.nodes?.length) {
            container.innerHTML = '<div style="padding:1rem 0.5rem; color:#ced4da; font-size:0.8rem; text-align:center;">No items yet.<br>Start at Step 1.</div>';
            return;
        }
        const html = [];
        const typeOrder = ['raw-material', 'packaging', 'supplier-packaging', 'packaging-group', 'product', 'distribution'];
        typeOrder.forEach(type => {
            const nodes = (currentProject.nodes || []).filter(n => n.type === type);
            if (!nodes.length) return;
            const step = STEPS.find(s => s.type === type);
            html.push(`<div class="te-tree-section-label">${step?.label || type}</div>`);
            nodes.forEach(node => {
                const downstream = getConnectedTo(node.id);
                const isActive = expandedCardId === node.id;
                html.push(`<div class="te-tree-item${isActive ? ' te-active' : ''}" data-node-id="${node.id}" title="${esc(node.name)}">
                    <i class="bi ${step?.icon || 'bi-dot'}" style="color:${step?.color};font-size:0.8rem;flex-shrink:0;margin-top:0.05rem;"></i>
                    <span style="flex:1;min-width:0;overflow:hidden;text-overflow:ellipsis;white-space:nowrap;">${esc(node.name)}</span>
                    ${downstream.length ? '<i class="bi bi-arrow-right-short" style="color:#ced4da;flex-shrink:0;"></i>' : ''}
                </div>`);
                if (downstream.length) {
                    html.push('<div class="te-tree-indent">');
                    downstream.forEach(child => {
                        const cs = STEPS.find(s => s.type === child.type);
                        html.push(`<div class="te-tree-item${expandedCardId === child.id ? ' te-active' : ''}" data-node-id="${child.id}" title="${esc(child.name)}">
                            <i class="bi ${cs?.icon || 'bi-dot'}" style="color:${cs?.color};font-size:0.8rem;flex-shrink:0;"></i>
                            <span style="flex:1;min-width:0;overflow:hidden;text-overflow:ellipsis;white-space:nowrap;">${esc(child.name)}</span>
                        </div>`);
                    });
                    html.push('</div>');
                }
            });
        });
        container.innerHTML = html.join('');
        container.querySelectorAll('.te-tree-item[data-node-id]').forEach(el => {
            el.addEventListener('click', () => {
                const node = getNodeById(el.dataset.nodeId);
                if (!node) return;
                const stepIdx = STEPS.findIndex(s => s.type === node.type);
                if (stepIdx >= 0) {
                    expandedCardId = node.id;
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
        const count = step.type !== 'review' ? getNodesOfType(step.type).length : null;
        el.innerHTML = `<div class="d-flex align-items-center gap-3">
            <div class="te-step-header-icon" style="background:${step.bg};color:${step.color};">
                <i class="bi ${step.icon}"></i>
            </div>
            <div style="flex:1;min-width:0;">
                <div style="font-weight:700;font-size:0.95rem;">Step ${currentStep + 1} · ${step.label}</div>
                <div class="text-muted" style="font-size:0.82rem;margin-top:0.1rem;">${step.desc}</div>
            </div>
            ${count !== null ? `<span style="font-size:0.8rem;color:#adb5bd;flex-shrink:0;">${count} item${count !== 1 ? 's' : ''}</span>` : ''}
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
        const parts = [];

        nodes.forEach(node => parts.push(renderNodeCard(node, step, expandedCardId === node.id)));

        if (addingInStep) {
            parts.push(renderAddForm(step));
        } else {
            parts.push(`<button type="button" class="te-add-card" id="teAddNodeBtn">
                <i class="bi bi-plus-circle" style="font-size:1rem;color:#0d6efd;"></i>
                Add ${singularise(step.label)}…
            </button>`);
        }

        if (!nodes.length && !addingInStep) {
            el.innerHTML = `<div class="te-empty-state">
                <span class="te-empty-icon"><i class="bi ${step.icon}" style="color:${step.color};"></i></span>
                <div style="font-size:0.95rem;font-weight:600;color:#6c757d;margin-bottom:0.35rem;">No ${step.label.toLowerCase()} yet</div>
                <div class="text-muted" style="font-size:0.82rem;max-width:360px;margin:0 auto;">${step.desc}</div>
            </div>` + parts.join('');
        } else {
            el.innerHTML = parts.join('');
        }

        bindStepBodyEvents(el, step);
    }

    function bindStepBodyEvents(el, step) {
        el.querySelectorAll('.te-node-card[data-node-id]').forEach(card => {
            const nodeId = card.dataset.nodeId;

            // Toggle expand on header click
            card.querySelector('.te-card-header')?.addEventListener('click', e => {
                if (e.target.closest('button')) return;
                expandedCardId = expandedCardId === nodeId ? null : nodeId;
                addingInStep = false;
                renderStep();
                renderTree();
            });

            // Delete
            card.querySelector('.te-delete-btn')?.addEventListener('click', e => {
                e.stopPropagation();
                const node = getNodeById(nodeId);
                if (!confirm(`Delete "${node?.name}"? All connections will also be removed.`)) return;
                if (expandedCardId === nodeId) expandedCardId = null;
                deleteNode(nodeId);
                renderAll();
            });

            // Save edit
            card.querySelector('.te-save-node-btn')?.addEventListener('click', e => {
                e.stopPropagation();
                saveNodeEdit(card, nodeId);
            });

            // Remove connections
            card.querySelectorAll('.te-pill-remove').forEach(btn => {
                btn.addEventListener('click', e => {
                    e.stopPropagation();
                    removeConnection(btn.dataset.from, btn.dataset.to);
                    renderStep(); renderTree();
                });
            });

            // Add connections
            card.querySelectorAll('.te-conn-add-select').forEach(sel => {
                sel.addEventListener('change', e => {
                    const targetId = e.target.value;
                    if (!targetId) return;
                    e.target.value = '';
                    if (e.target.dataset.direction === 'to') {
                        addConnection(nodeId, targetId);
                    } else {
                        addConnection(targetId, nodeId);
                    }
                    renderStep(); renderTree();
                });
            });
        });

        el.querySelector('#teAddNodeBtn')?.addEventListener('click', () => {
            addingInStep = true;
            expandedCardId = null;
            renderStep();
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
        const upstreamTypes  = UPSTREAM[step.type]  || [];
        const downstreamTypes = DOWNSTREAM[step.type] || [];
        const connFrom = getConnectedFrom(node.id);
        const connTo   = getConnectedTo(node.id);

        // Connection pills — shown collapsed too
        let connSummary = '';
        if (!isExpanded && (connFrom.length || connTo.length)) {
            const pills = [
                ...connFrom.map(n => { const s = STEPS.find(st => st.type === n.type); return `<span class="te-conn-pill" style="background:${s?.bg||'#f0f4ff'};color:${s?.color||'#0d6efd'};border-color:${s?.color||'#c8d9ff'}30;"><i class="bi ${s?.icon||'bi-dot'}" style="font-size:0.75rem;"></i>${esc(n.name)}</span>`; }),
                ...connTo.map(n => { const s = STEPS.find(st => st.type === n.type); return `<span class="te-conn-pill" style="background:${s?.bg||'#f0f4ff'};color:${s?.color||'#0d6efd'};border-color:${s?.color||'#c8d9ff'}30;"><i class="bi ${s?.icon||'bi-dot'}" style="font-size:0.75rem;"></i>${esc(n.name)}</span>`; })
            ];
            if (pills.length) connSummary = `<div class="te-conn-pills mt-2">${pills.join('')}</div>`;
        }

        // Detailed connections when expanded
        let connDetails = '';
        if (isExpanded) {
            upstreamTypes.forEach(upType => {
                const upStep = STEPS.find(s => s.type === upType);
                const existing = connFrom.filter(n => n.type === upType);
                const available = getNodesOfType(upType).filter(n => !existing.find(e => e.id === n.id));
                connDetails += `<div class="te-conn-section">
                    <div class="te-conn-label"><i class="bi bi-arrow-left-short" style="font-size:1rem;"></i> From ${upStep?.label || upType}</div>
                    <div class="te-conn-pills">
                        ${existing.map(n => `<span class="te-conn-pill" style="background:${upStep?.bg};color:${upStep?.color};border-color:${upStep?.color}30;">
                            <i class="bi ${upStep?.icon}" style="font-size:0.75rem;"></i>${esc(n.name)}
                            <span class="te-pill-remove" data-from="${n.id}" data-to="${node.id}" title="Remove">×</span>
                        </span>`).join('')}
                        ${available.length ? `<select class="te-conn-add-select" data-direction="from" aria-label="Link ${upStep?.label}">
                            <option value="">+ Link ${singularise(upStep?.label || upType)}…</option>
                            ${available.map(n => `<option value="${n.id}">${esc(n.name)}</option>`).join('')}
                        </select>` : (existing.length === 0 ? `<span style="font-size:0.78rem;color:#adb5bd;font-style:italic;">None — add ${singularise(upStep?.label || upType).toLowerCase()}s in Step ${STEPS.findIndex(s => s.type === upType) + 1} first</span>` : '')}
                    </div>
                </div>`;
            });

            downstreamTypes.forEach(downType => {
                const downStep = STEPS.find(s => s.type === downType);
                const existing = connTo.filter(n => n.type === downType);
                const available = getNodesOfType(downType).filter(n => !existing.find(e => e.id === n.id));
                connDetails += `<div class="te-conn-section">
                    <div class="te-conn-label"><i class="bi bi-arrow-right-short" style="font-size:1rem;"></i> To ${downStep?.label || downType}</div>
                    <div class="te-conn-pills">
                        ${existing.map(n => `<span class="te-conn-pill" style="background:${downStep?.bg};color:${downStep?.color};border-color:${downStep?.color}30;">
                            <i class="bi ${downStep?.icon}" style="font-size:0.75rem;"></i>${esc(n.name)}
                            <span class="te-pill-remove" data-from="${node.id}" data-to="${n.id}" title="Remove">×</span>
                        </span>`).join('')}
                        ${available.length ? `<select class="te-conn-add-select" data-direction="to" aria-label="Link ${downStep?.label}">
                            <option value="">+ Link ${singularise(downStep?.label || downType)}…</option>
                            ${available.map(n => `<option value="${n.id}">${esc(n.name)}</option>`).join('')}
                        </select>` : (existing.length === 0 ? `<span style="font-size:0.78rem;color:#adb5bd;font-style:italic;">None — add ${singularise(downStep?.label || downType).toLowerCase()}s in Step ${STEPS.findIndex(s => s.type === downType) + 1} first</span>` : '')}
                    </div>
                </div>`;
            });
        }

        const meta = buildNodeMeta(node, step);
        const editForm = isExpanded ? renderEditForm(node, step.type) : '';

        return `<div class="te-node-card${isExpanded ? ' te-card-expanded' : ''}" data-node-id="${node.id}">
            <div class="te-card-header">
                <div class="te-card-icon" style="background:${step.bg};color:${step.color};">
                    <i class="bi ${step.icon}"></i>
                </div>
                <div style="flex:1;min-width:0;">
                    <div style="font-weight:600;font-size:0.9rem;white-space:nowrap;overflow:hidden;text-overflow:ellipsis;">${esc(node.name)}</div>
                    ${meta ? `<div class="te-card-meta">${meta}</div>` : ''}
                </div>
                <div style="display:flex;gap:0.25rem;flex-shrink:0;margin-left:0.5rem;">
                    <button type="button" class="te-card-actions te-danger te-delete-btn" title="Delete ${singularise(step.label)}">
                        <i class="bi bi-trash" style="font-size:0.8rem;"></i>
                    </button>
                    <button type="button" class="te-card-actions" title="${isExpanded ? 'Collapse' : 'Edit'}">
                        <i class="bi bi-chevron-${isExpanded ? 'up' : 'down'}" style="font-size:0.8rem;"></i>
                    </button>
                </div>
            </div>
            ${!isExpanded ? connSummary : ''}
            ${isExpanded ? `<div class="te-card-body">${editForm}${connDetails}</div>` : ''}
        </div>`;
    }

    function buildNodeMeta(node, step) {
        const p = node.parameters || {};
        const parts = [];
        const badge = (txt) => `<span style="display:inline-flex;align-items:center;padding:0.12rem 0.45rem;border-radius:10px;font-size:0.75rem;background:#f1f3f5;color:#6c757d;border:1px solid #e9ecef;">${txt}</span>`;
        const colBadge = (txt, color) => `<span style="display:inline-flex;align-items:center;padding:0.12rem 0.45rem;border-radius:10px;font-size:0.75rem;background:${color}20;color:${color};border:1px solid ${color}30;">${txt}</span>`;

        if (step.type === 'raw-material' && p.weight) parts.push(badge(`${p.weight}g`));
        if (step.type === 'packaging') {
            if (p.height) parts.push(badge(`${p.height}mm`));
            if (p.weight) parts.push(badge(`${p.weight}g`));
        }
        if (step.type === 'supplier-packaging' && p.companyName) parts.push(badge(esc(p.companyName)));
        if (step.type === 'packaging-group' && p.layer) {
            const lc = { Primary: '#6f42c1', Secondary: '#0d6efd', Tertiary: '#198754', Quaternary: '#fd7e14' };
            parts.push(colBadge(p.layer, lc[p.layer] || '#6c757d'));
            if (p.holdsQuantity) parts.push(badge(`×${p.holdsQuantity}`));
        }
        if (step.type === 'product') {
            if (p.sku) parts.push(badge(esc(p.sku)));
            if (p.weight) parts.push(badge(`${p.weight}g net`));
        }
        if (step.type === 'distribution') {
            if (p.city) parts.push(badge(`<i class="bi bi-geo-alt me-1"></i>${esc(p.city)}`));
            if (p.type) parts.push(badge(esc(p.type)));
        }
        if (!parts.length && p.description) {
            parts.push(`<span style="font-size:0.78rem;color:#adb5bd;">${esc(p.description.substring(0, 70))}</span>`);
        }
        return parts.join('');
    }

    // ─── Edit / Add forms ─────────────────────────────────────────────────────
    function renderEditForm(node, type) {
        const fields = NODE_FIELDS[type] || [];
        const p = node.parameters || {};
        return `<div class="te-edit-form" data-node-id="${node.id}">
            ${buildFormFields(fields, p, 'te-edit-field')}
            <div style="display:flex;gap:0.5rem;margin-top:0.875rem;">
                <button type="button" class="te-save-btn te-save-node-btn">Save changes</button>
            </div>
        </div>`;
    }

    function renderAddForm(step) {
        const fields = NODE_FIELDS[step.type] || [];
        const defaults = {};
        fields.forEach(f => { if (f.type === 'select') defaults[f.key] = f.options?.[0] || ''; });
        return `<div class="te-node-card te-card-expanded te-add-form" style="border-style:dashed;cursor:default;">
            <div style="display:flex;align-items:center;gap:0.75rem;margin-bottom:1rem;">
                <div class="te-card-icon" style="background:${step.bg};color:${step.color};"><i class="bi ${step.icon}"></i></div>
                <span style="font-weight:600;font-size:0.9rem;">New ${singularise(step.label)}</span>
            </div>
            ${buildFormFields(fields, defaults, 'te-add-field')}
            <div style="display:flex;gap:0.5rem;margin-top:0.875rem;">
                <button type="button" class="te-save-btn" id="teSaveAddBtn">Add ${singularise(step.label)}</button>
                <button type="button" class="te-cancel-btn" id="teCancelAddBtn">Cancel</button>
            </div>
        </div>`;
    }

    function buildFormFields(fields, values, cls) {
        const text = fields.filter(f => f.type !== 'textarea');
        const areas = fields.filter(f => f.type === 'textarea');
        const rows = [];
        for (let i = 0; i < text.length; i += 2) {
            const f1 = text[i], f2 = text[i + 1];
            rows.push(`<div class="te-form-row${!f2 ? ' te-single' : ''}">
                ${formField(f1, values[f1.key] ?? '', cls)}
                ${f2 ? formField(f2, values[f2.key] ?? '', cls) : ''}
            </div>`);
        }
        areas.forEach(f => rows.push(`<div class="te-form-row te-single">${formField(f, values[f.key] ?? '', cls)}</div>`));
        return rows.join('');
    }

    function formField(field, value, cls) {
        const baseAttrs = `class="form-control form-control-sm ${cls}" data-field="${field.key}" ${field.required ? 'required' : ''}`;
        let input;
        if (field.type === 'select') {
            const opts = (field.options || []).map(o => `<option value="${o}"${o === value ? ' selected' : ''}>${o}</option>`).join('');
            input = `<select ${baseAttrs}>${opts}</select>`;
        } else if (field.type === 'textarea') {
            input = `<textarea ${baseAttrs} rows="2" placeholder="${esc(field.placeholder || '')}">${esc(String(value))}</textarea>`;
        } else {
            input = `<input type="${field.type}" ${baseAttrs} value="${esc(String(value ?? ''))}" placeholder="${esc(field.placeholder || '')}">`;
        }
        return `<div>
            <label style="display:block;font-size:0.75rem;font-weight:600;color:#6c757d;margin-bottom:0.25rem;">
                ${field.label}${field.required ? ' <span style="color:#dc3545;">*</span>' : ''}
            </label>
            ${input}
        </div>`;
    }

    function collectFormData(form, type) {
        const fields = NODE_FIELDS[type] || [];
        const params = {};
        let valid = true;
        fields.forEach(f => {
            const el = form.querySelector(`[data-field="${f.key}"]`);
            if (!el) return;
            const val = el.value.trim();
            if (f.required && !val) { el.classList.add('is-invalid'); valid = false; }
            else { el.classList.remove('is-invalid'); params[f.key] = f.type === 'number' ? (val ? parseFloat(val) : null) : val; }
        });
        return valid ? params : null;
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

    // ─── Footer ───────────────────────────────────────────────────────────────
    function renderStepFooter() {
        const el = document.getElementById('teStepFooter');
        if (!el) return;
        const isFirst = currentStep === 0;
        const isLast  = currentStep === STEPS.length - 1;
        el.innerHTML = `
            <button type="button" class="te-nav-btn te-nav-btn-secondary${isFirst ? ' invisible' : ''}" id="teBackBtn">
                <i class="bi bi-arrow-left"></i> Back
            </button>
            <span style="font-size:0.78rem;color:#adb5bd;">${currentStep + 1} of ${STEPS.length}</span>
            <button type="button" class="te-nav-btn te-nav-btn-${isLast ? 'success' : 'primary'}" id="teNextBtn">
                ${isLast ? '<i class="bi bi-check2"></i> Done' : 'Next <i class="bi bi-arrow-right"></i>'}
            </button>`;
        document.getElementById('teBackBtn')?.addEventListener('click', () => goToStep(currentStep - 1));
        document.getElementById('teNextBtn')?.addEventListener('click', () => goToStep(isLast ? 0 : currentStep + 1));
    }

    // ─── Review ───────────────────────────────────────────────────────────────
    function renderReview() {
        if (!currentProject?.nodes?.length) {
            return `<div class="te-empty-state">
                <span class="te-empty-icon"><i class="bi bi-check-circle" style="color:#20c997;"></i></span>
                <div style="font-size:0.95rem;font-weight:600;color:#6c757d;">Nothing to review yet</div>
                <div class="text-muted" style="font-size:0.82rem;margin-top:0.35rem;">Add nodes in the earlier steps to build your supply chain.</div>
            </div>`;
        }

        const chains = buildChains();
        const html = ['<div style="margin-bottom:1rem;"><p class="text-muted" style="font-size:0.82rem;">Each row is a complete path through your supply chain. Click any node to jump to that step.</p></div>'];

        if (chains.length === 0) {
            html.push('<div class="text-muted" style="font-size:0.85rem;padding:1rem 0;">No connections defined yet. Go back to each step and link items together using the connection dropdowns.</div>');
        } else {
            chains.forEach(chain => {
                const pills = chain.map(node => {
                    const s = STEPS.find(st => st.type === node.type);
                    return `<span class="te-review-chain-node" style="background:${s?.bg};color:${s?.color};cursor:pointer;" data-jump-node="${node.id}" title="Go to ${esc(node.name)}">
                        <i class="bi ${s?.icon}" style="font-size:0.78rem;"></i> ${esc(node.name)}
                    </span>`;
                }).join('<span class="te-review-arrow"><i class="bi bi-arrow-right"></i></span>');
                html.push(`<div class="te-review-chain">${pills}</div>`);
            });
        }

        // Summary counts
        const typeOrder = ['raw-material', 'packaging', 'supplier-packaging', 'packaging-group', 'product', 'distribution'];
        const summaryItems = typeOrder.map(type => {
            const n = getNodesOfType(type).length;
            if (!n) return '';
            const s = STEPS.find(st => st.type === type);
            return `<span style="display:inline-flex;align-items:center;gap:0.3rem;padding:0.25rem 0.65rem;border-radius:20px;font-size:0.8rem;background:${s?.bg};color:${s?.color};">
                <i class="bi ${s?.icon}" style="font-size:0.78rem;"></i> ${n} ${s?.label}
            </span>`;
        }).filter(Boolean).join('');
        if (summaryItems) {
            html.push(`<div style="margin-top:1.5rem;border-top:1px solid #f0f2f5;padding-top:1rem;">
                <div style="font-size:0.72rem;font-weight:700;text-transform:uppercase;letter-spacing:0.07em;color:#adb5bd;margin-bottom:0.6rem;">Summary</div>
                <div style="display:flex;flex-wrap:wrap;gap:0.4rem;">${summaryItems}</div>
            </div>`);
        }

        return html.join('');
    }

    function buildChains() {
        const allNodes = currentProject?.nodes || [];
        const connections = currentProject?.connections || [];
        if (!connections.length) return [];
        const pointedTo = new Set(connections.map(c => c.to));
        const sources = allNodes.filter(n => !pointedTo.has(n.id));
        const chains = [];
        function dfs(node, path) {
            const next = connections.filter(c => c.from === node.id).map(c => allNodes.find(n => n.id === c.to)).filter(Boolean);
            if (!next.length) { if (path.length > 1) chains.push([...path]); }
            else next.forEach(child => dfs(child, [...path, child]));
        }
        sources.forEach(s => dfs(s, [s]));
        return chains.slice(0, 60);
    }

    // ─── Utilities ────────────────────────────────────────────────────────────
    function singularise(label) {
        if (!label) return '';
        if (label === 'Raw Materials') return 'Raw Material';
        if (label === 'Suppliers') return 'Supplier';
        if (label === 'Groups') return 'Group';
        if (label === 'Products') return 'Product';
        if (label === 'Distribution') return 'Distribution Point';
        return label.replace(/s$/, '');
    }

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
        const c = document.getElementById('textEditorContainer');
        if (c && !c.classList.contains('d-none')) renderAll();
    };

})();
