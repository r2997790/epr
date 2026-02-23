// EPR Visual Editor - Complete Standalone Implementation
// All functionality in one file to avoid loading/initialization issues
// VERSION: 2024-01-16-TransportNodeFix2 - Removed old transport node UI, fixed visibility

console.log('%c[EPR Visual Editor] visual-editor-complete.js loading... VERSION: 2024-01-16-TransportNodeFix2', 'color: green; font-weight: bold; font-size: 14px;');
console.log('[EPR Visual Editor] Timestamp:', new Date().toISOString());

(function() {
    'use strict';
    
    // ============================================================================
    // CANVAS MANAGER
    // ============================================================================
    // Version: 2024-01-16-TransportNodeFix2 - Removed old transport node UI
    class EPRCanvasManager {
        constructor() {
            console.log('[EPR Canvas] Creating CanvasManager... Version: 2024-01-16-TransportNodeFix2');
            
            // Generate unique instance ID for this Visual Editor instance (for multiple concurrent instances)
            // Check URL parameter first to ensure each tab has its own instance
            const urlParams = new URLSearchParams(window.location.search);
            const instanceParam = urlParams.get('instance');
            
            if (instanceParam) {
                // Use instance ID from URL parameter (for tab switching)
                this.instanceId = `epr-instance-${instanceParam}`;
            } else {
                // Create new instance ID if no parameter in URL
                const newInstanceId = `${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
                this.instanceId = `epr-instance-${newInstanceId}`;
                // Store in global for backward compatibility, but prefer URL parameter
                window.eprCurrentInstanceId = this.instanceId;
            }
            
            this.canvas = document.getElementById('eprCanvas');
            this.nodesLayer = document.getElementById('eprNodesLayer');
            this.connectionsLayer = document.getElementById('eprConnectionsLayer');
            
            if (!this.canvas || !this.nodesLayer || !this.connectionsLayer) {
                console.error('[EPR Canvas] Required elements not found!');
                throw new Error('Canvas elements not found');
            }
            
            this.nodes = new Map();
            this.connections = [];
            this.selectedNode = null;
            this.selectedNodes = new Set(); // For multi-select (Ctrl+A)
            this.copiedNodes = null; // Store copied nodes for paste
            this.toolMode = 'select'; // 'select' or 'pan'
            this.isSelecting = false;
            this.selectionBox = null;
            this.selectionStart = null;
            this.gridSize = 20;
            this.showGrid = false;
            this.snapToGrid = false;
            this.zoomLevel = 1;
            this.panOffset = { x: 0, y: 0 };
            this.history = [];
            this.historyIndex = -1;
            this.lastActionType = null; // Track last action type for undo logic
            this.lastRemovedFromGroup = null; // Track nodes removed from group for undo
            this.viewMode = 'flow'; // 'flow' or 'sankey'
            this.showTransportNodes = true; // Transport nodes visible by default
            
            console.log('[EPR Canvas] CanvasManager created with instance ID:', this.instanceId);
            this.init();
            this.setupTransportNodeObserver();
            this.setupWatermark();
        }
        
        // Helper method to get instance-specific storage key
        getStorageKey(baseKey) {
            return `${baseKey}-${this.instanceId}`;
        }
        
        setupWatermark() {
            // Create SVG marker definitions for arrows if they don't exist
            if (!this.connectionsLayer.querySelector('defs')) {
                const defs = document.createElementNS('http://www.w3.org/2000/svg', 'defs');
                
                // Arrow marker for normal state
                const marker = document.createElementNS('http://www.w3.org/2000/svg', 'marker');
                marker.setAttribute('id', 'arrowhead');
                marker.setAttribute('markerWidth', '12');
                marker.setAttribute('markerHeight', '12');
                marker.setAttribute('refX', '10');
                marker.setAttribute('refY', '6');
                marker.setAttribute('orient', 'auto');
                marker.setAttribute('markerUnits', 'userSpaceOnUse');
                
                const polygon = document.createElementNS('http://www.w3.org/2000/svg', 'polygon');
                polygon.setAttribute('points', '0 0, 12 6, 0 12');
                polygon.setAttribute('fill', '#6c757d');
                marker.appendChild(polygon);
                defs.appendChild(marker);
                
                // Arrow marker for hover state
                const markerHover = document.createElementNS('http://www.w3.org/2000/svg', 'marker');
                markerHover.setAttribute('id', 'arrowhead-hover');
                markerHover.setAttribute('markerWidth', '12');
                markerHover.setAttribute('markerHeight', '12');
                markerHover.setAttribute('refX', '10');
                markerHover.setAttribute('refY', '6');
                markerHover.setAttribute('orient', 'auto');
                markerHover.setAttribute('markerUnits', 'userSpaceOnUse');
                
                const polygonHover = document.createElementNS('http://www.w3.org/2000/svg', 'polygon');
                polygonHover.setAttribute('points', '0 0, 12 6, 0 12');
                polygonHover.setAttribute('fill', '#dc3545');
                markerHover.appendChild(polygonHover);
                defs.appendChild(markerHover);
                
                this.connectionsLayer.appendChild(defs);
            }
            
            // Add watermark SVG to canvas container
            const canvasContainer = document.getElementById('eprCanvasContainer');
            if (canvasContainer && !canvasContainer.querySelector('.epr-watermark')) {
                const watermarkDiv = document.createElement('div');
                watermarkDiv.className = 'epr-watermark';
                watermarkDiv.style.cssText = `
                    position: absolute;
                    top: 50%;
                    left: 50%;
                    transform: translate(-50%, -50%) scale(2);
                    opacity: 0.02;
                    pointer-events: none;
                    z-index: 1;
                    width: 441.101px;
                    height: 163.381px;
                `;
                
                // Load and embed the SVG
                fetch('/empauer-watermark.svg')
                    .then(response => response.text())
                    .then(svgText => {
                        watermarkDiv.innerHTML = svgText;
                        canvasContainer.appendChild(watermarkDiv);
                    })
                    .catch(error => {
                        console.warn('[EPR Canvas] Could not load watermark SVG:', error);
                        // Fallback: create a simple text watermark
                        watermarkDiv.innerHTML = '<div style="font-size: 48px; color: #000; text-align: center; font-weight: bold;">EMPAUER</div>';
                        canvasContainer.appendChild(watermarkDiv);
                    });
            }
        }
        
        setupTransportNodeObserver() {
            // CRITICAL: MutationObserver to catch ANY transport nodes that appear and hide them if between nodes
            // This is a final safeguard to ensure transport nodes are ALWAYS hidden when between two nodes
            const observer = new MutationObserver((mutations) => {
                mutations.forEach((mutation) => {
                    mutation.addedNodes.forEach((node) => {
                        if (node.nodeType === 1 && node.classList && node.classList.contains('epr-canvas-node')) {
                            const nodeId = node.getAttribute('data-node-id');
                            const nodeType = node.getAttribute('data-type');
                            
                            if (nodeType === 'transport' && nodeId) {
                                // Transport node was added - check if it should be hidden
                                setTimeout(() => {
                                    const transportNode = this.nodes.get(nodeId);
                                    if (transportNode) {
                                        const incomingConn = this.connections.find(c => c.to === nodeId);
                                        const outgoingConn = this.connections.find(c => c.from === nodeId);
                                        
                                        if (incomingConn && outgoingConn) {
                                            // Hide it immediately with ALL methods
                                            const nodeEl = document.querySelector(`[data-node-id="${nodeId}"]`);
                                            if (nodeEl) {
                                                nodeEl.style.display = 'none';
                                                nodeEl.style.setProperty('display', 'none', 'important');
                                                nodeEl.style.visibility = 'hidden';
                                                nodeEl.style.setProperty('visibility', 'hidden', 'important');
                                                nodeEl.style.opacity = '0';
                                                nodeEl.style.setProperty('opacity', '0', 'important');
                                                nodeEl.style.height = '0';
                                                nodeEl.style.setProperty('height', '0', 'important');
                                                nodeEl.style.width = '0';
                                                nodeEl.style.setProperty('width', '0', 'important');
                                                nodeEl.style.position = 'absolute';
                                                nodeEl.style.setProperty('position', 'absolute', 'important');
                                                nodeEl.style.left = '-9999px';
                                                nodeEl.style.setProperty('left', '-9999px', 'important');
                                                nodeEl.style.pointerEvents = 'none';
                                                nodeEl.style.setProperty('pointer-events', 'none', 'important');
                                                nodeEl.setAttribute('data-transport-on-connection', 'true');
                                                nodeEl.classList.add('epr-transport-hidden');
                                                this.updateConnections();
                                            }
                                        } else {
                                            // Transport node without connections - hide it anyway
                                            const nodeEl = document.querySelector(`[data-node-id="${nodeId}"]`);
                                            if (nodeEl) {
                                                console.warn('[EPR Transport] Standalone transport node detected - hiding it');
                                                nodeEl.style.display = 'none';
                                                nodeEl.style.setProperty('display', 'none', 'important');
                                                nodeEl.style.visibility = 'hidden';
                                                nodeEl.style.setProperty('visibility', 'hidden', 'important');
                                                nodeEl.setAttribute('data-transport-on-connection', 'true');
                                                nodeEl.classList.add('epr-transport-hidden');
                                            }
                                        }
                                    }
                                }, 0);
                            }
                        }
                    });
                });
            });
            
            // Observe the nodes layer for new nodes
            if (this.nodesLayer) {
                observer.observe(this.nodesLayer, {
                    childList: true,
                    subtree: true
                });
            }
            
            this.transportObserver = observer;
        }
        
        init() {
            console.log('[EPR Canvas] Initializing canvas...');
            this.setupEventListeners();
            this.setupDragAndDrop();
            this.setupToolbarButtons();
            this.updateGridDisplay();
            this.resizeConnectionsLayer();
            this.updatePlaceholder();
            this.setupKeyboardShortcuts();
            
            // CRITICAL: Hide ALL existing transport nodes immediately on initialization
            setTimeout(() => {
                this.hideAllTransportNodes();
            }, 100);
            
            window.addEventListener('resize', () => {
                setTimeout(() => this.resizeConnectionsLayer(), 100);
            });
            
            setTimeout(() => {
                this.resizeConnectionsLayer();
                this.hideAllTransportNodes(); // Hide again after resize
                console.log('[EPR Canvas] Canvas initialized. Size:', {
                    width: this.canvas.offsetWidth,
                    height: this.canvas.offsetHeight
                });
                
                // CRITICAL: Remove any blocking modal backdrops
                setTimeout(() => {
                    document.querySelectorAll('.modal-backdrop:not(.show)').forEach(backdrop => {
                        backdrop.remove();
                    });
                    // Also check for backdrops without .show class
                    document.querySelectorAll('.modal-backdrop').forEach(backdrop => {
                        if (!backdrop.classList.contains('show')) {
                            backdrop.remove();
                        }
                    });
                }, 100);
            }, 200);
        }
        
        setupKeyboardShortcuts() {
            // Keyboard support for various shortcuts
            document.addEventListener('keydown', (e) => {
                // Only handle if not typing in an input field
                if (e.target.tagName === 'INPUT' || e.target.tagName === 'TEXTAREA' || e.target.isContentEditable) {
                    return;
                }
                
                // Delete key - delete selected node(s)
                if ((e.key === 'Delete' || e.key === 'Del')) {
                    if (this.selectedNodes && this.selectedNodes.size > 0) {
                        e.preventDefault();
                        e.stopPropagation();
                        // Delete all selected nodes
                        const nodesToDelete = Array.from(this.selectedNodes);
                        nodesToDelete.forEach(nodeId => {
                            this.deleteNode(nodeId);
                        });
                        this.selectedNodes.clear();
                        this.selectedNode = null;
                    } else if (this.selectedNode) {
                        e.preventDefault();
                        e.stopPropagation();
                        this.deleteNode(this.selectedNode);
                    }
                }
                
                // Ctrl+Z - Undo
                if ((e.ctrlKey || e.metaKey) && e.key === 'z' && !e.shiftKey) {
                    e.preventDefault();
                    e.stopPropagation();
                    this.undo();
                }
                
                // Ctrl+A - Select all nodes
                if ((e.ctrlKey || e.metaKey) && e.key === 'a') {
                    e.preventDefault();
                    e.stopPropagation();
                    this.selectAllNodes();
                }
                
                // Ctrl+C - Copy selected nodes
                if ((e.ctrlKey || e.metaKey) && e.key === 'c') {
                    e.preventDefault();
                    e.stopPropagation();
                    this.copySelectedNodes();
                }
                
                // Ctrl+V - Paste copied nodes
                if ((e.ctrlKey || e.metaKey) && e.key === 'v') {
                    e.preventDefault();
                    e.stopPropagation();
                    this.pasteNodes();
                }
            });
        }
        
        selectAllNodes() {
            // Select all nodes on the canvas
            const allNodeIds = Array.from(this.nodes.keys());
            if (allNodeIds.length === 0) return;
            
            // Deselect current selection
            if (this.selectedNode) {
                const currentEl = document.querySelector(`[data-node-id="${this.selectedNode}"]`);
                if (currentEl) {
                    currentEl.classList.remove('selected');
                }
            }
            
            // Clear any existing selected nodes
            document.querySelectorAll('.epr-canvas-node.selected').forEach(el => {
                el.classList.remove('selected');
            });
            
            // Select all nodes visually
            allNodeIds.forEach(nodeId => {
                const nodeEl = document.querySelector(`[data-node-id="${nodeId}"]`);
                if (nodeEl) {
                    nodeEl.classList.add('selected');
                }
            });
            
            // Store all selected nodes (for potential multi-select operations)
            this.selectedNodes = new Set(allNodeIds);
            this.selectedNode = null; // Clear single selection when multi-selecting
            
            console.log(`[EPR Canvas] Selected all ${allNodeIds.length} nodes`);
        }
        
        copySelectedNodes() {
            const nodesToCopy = this.selectedNodes.size > 0 
                ? Array.from(this.selectedNodes) 
                : (this.selectedNode ? [this.selectedNode] : []);
            
            if (nodesToCopy.length === 0) {
                console.log('[EPR Canvas] No nodes selected to copy');
                return;
            }
            
            // Collect all related nodes and connections
            const elementData = this.collectRelatedNodes(nodesToCopy);
            
            // Store copied data
            this.copiedNodes = {
                nodes: elementData.nodes,
                connections: elementData.connections,
                timestamp: Date.now()
            };
            
            console.log(`[EPR Canvas] Copied ${elementData.nodes.length} nodes and ${elementData.connections.length} connections`);
        }
        
        pasteNodes() {
            if (!this.copiedNodes || !this.copiedNodes.nodes || this.copiedNodes.nodes.length === 0) {
                console.log('[EPR Canvas] No nodes to paste');
                return;
            }
            
            // Calculate offset to paste nodes next to original position
            const offsetX = 200;
            const offsetY = 100;
            
            // Load the copied nodes with offset
            this.loadElement({
                nodes: this.copiedNodes.nodes,
                connections: this.copiedNodes.connections
            }, { x: offsetX, y: offsetY });
            
            console.log(`[EPR Canvas] Pasted ${this.copiedNodes.nodes.length} nodes`);
        }
        
        hideAllTransportNodes() {
            // CRITICAL: Hide ALL transport nodes - they should ONLY appear as boxes on connection lines
            this.nodes.forEach((node, nodeId) => {
                if (node.type === 'transport') {
                    const nodeEl = document.querySelector(`[data-node-id="${nodeId}"]`);
                    if (nodeEl) {
                        const incomingConn = this.connections.find(c => c.to === nodeId);
                        const outgoingConn = this.connections.find(c => c.from === nodeId);
                        
                        // If transport node is between two nodes, ALWAYS hide it
                        if (incomingConn && outgoingConn) {
                            nodeEl.style.display = 'none';
                            nodeEl.style.setProperty('display', 'none', 'important');
                            nodeEl.style.visibility = 'hidden';
                            nodeEl.style.setProperty('visibility', 'hidden', 'important');
                            nodeEl.style.opacity = '0';
                            nodeEl.style.setProperty('opacity', '0', 'important');
                            nodeEl.style.height = '0';
                            nodeEl.style.setProperty('height', '0', 'important');
                            nodeEl.style.width = '0';
                            nodeEl.style.setProperty('width', '0', 'important');
                            nodeEl.style.position = 'absolute';
                            nodeEl.style.setProperty('position', 'absolute', 'important');
                            nodeEl.style.left = '-9999px';
                            nodeEl.style.setProperty('left', '-9999px', 'important');
                            nodeEl.style.pointerEvents = 'none';
                            nodeEl.style.setProperty('pointer-events', 'none', 'important');
                            nodeEl.setAttribute('data-transport-on-connection', 'true');
                            nodeEl.classList.add('epr-transport-hidden');
                        } else {
                            // Transport node without connections - hide it anyway (shouldn't exist)
                            console.warn('[EPR Transport] Found standalone transport node - hiding it:', nodeId);
                            nodeEl.style.display = 'none';
                            nodeEl.style.setProperty('display', 'none', 'important');
                            nodeEl.style.visibility = 'hidden';
                            nodeEl.style.setProperty('visibility', 'hidden', 'important');
                            nodeEl.setAttribute('data-transport-on-connection', 'true');
                            nodeEl.classList.add('epr-transport-hidden');
                        }
                    }
                }
            });
            
            // Force update connections to render transport boxes
            this.updateConnections();
        }
        
        resizeConnectionsLayer() {
            if (!this.canvas || !this.connectionsLayer) return;
            
            const rect = this.canvas.getBoundingClientRect();
            const width = rect.width || window.innerWidth;
            const height = rect.height || window.innerHeight;
            
            this.connectionsLayer.setAttribute('width', width);
            this.connectionsLayer.setAttribute('height', height);
            this.connectionsLayer.style.width = width + 'px';
            this.connectionsLayer.style.height = height + 'px';
        }
        
        setupEventListeners() {
            // Canvas click to deselect - use capture phase to check first
            this.canvas.addEventListener('click', (e) => {
                // CRITICAL: Check if click is on an interactive element FIRST
                const target = e.target;
                const isInteractive = target.closest('button') || 
                    target.closest('.epr-floating-toolbar') || 
                    target.closest('.epr-types-panel') || 
                    target.closest('.epr-project-header') ||
                    target.closest('.epr-parameters-panel') || 
                    target.closest('input') || 
                    target.closest('select') || 
                    target.closest('textarea') ||
                    target.closest('.modal') ||
                    target.closest('.dropdown-menu') ||
                    target.closest('a') ||
                    target.closest('.form-control') ||
                    target.closest('.form-select') ||
                    target.closest('.dropdown-toggle') ||
                    target.closest('.dropdown-item') ||
                    target.tagName === 'BUTTON' ||
                    target.tagName === 'INPUT' ||
                    target.tagName === 'SELECT' ||
                    target.tagName === 'A';
                
                if (isInteractive) {
                    // Don't prevent default or stop propagation - let it work normally
                    return;
                }
                
                // Only handle canvas clicks if it's actually the canvas (not on a node)
                // Check if click is on a node - if so, don't clear selection (node handler will manage it)
                const clickedNode = target.closest('.epr-canvas-node');
                if (!clickedNode && (target === this.canvas || target === this.nodesLayer || target === this.connectionsLayer)) {
                    // Only clear selection if clicking on empty canvas (not on a node)
                    this.deselectNode();
                }
            }, { capture: false, passive: true });
            
            // Canvas panning and selection
            let isPanning = false;
            let panStart = { x: 0, y: 0 };
            
            this.canvas.addEventListener('mousedown', (e) => {
                // CRITICAL: Check if mousedown is on an interactive element FIRST
                const target = e.target;
                const isInteractive = target.closest('button') || 
                    target.closest('.epr-floating-toolbar') || 
                    target.closest('.epr-types-panel') || 
                    target.closest('.epr-project-header') ||
                    target.closest('.epr-parameters-panel') || 
                    target.closest('input') || 
                    target.closest('select') || 
                    target.closest('textarea') ||
                    target.closest('.modal') ||
                    target.closest('.dropdown-menu') ||
                    target.closest('a') ||
                    target.closest('.form-control') ||
                    target.closest('.form-select') ||
                    target.closest('.dropdown-toggle') ||
                    target.closest('.dropdown-item') ||
                    target.tagName === 'BUTTON' ||
                    target.tagName === 'INPUT' ||
                    target.tagName === 'SELECT' ||
                    target.tagName === 'A';
                
                if (isInteractive) {
                    // Don't prevent default or stop propagation - let it work normally
                    return;
                }
                
                // Don't start selection/pan if clicking on a node
                if (target.closest('.epr-canvas-node')) return;
                
                if (target === this.canvas || target === this.nodesLayer || target === this.connectionsLayer) {
                    if (this.toolMode === 'select') {
                        // Start selection box
                        this.startSelection(e);
                    } else {
                        // Pan mode
                        isPanning = true;
                        panStart.x = e.clientX - this.panOffset.x;
                        panStart.y = e.clientY - this.panOffset.y;
                        this.canvas.style.cursor = 'grabbing';
                    }
                }
            }, { capture: false, passive: true });
            
            this.canvas.addEventListener('mousemove', (e) => {
                if (isPanning) {
                    this.panOffset.x = e.clientX - panStart.x;
                    this.panOffset.y = e.clientY - panStart.y;
                    this.updateTransform();
                } else if (this.isSelecting) {
                    this.updateSelection(e);
                }
            });
            
            this.canvas.addEventListener('mouseup', (e) => {
                if (isPanning) {
                    isPanning = false;
                    this.canvas.style.cursor = 'default';
                } else if (this.isSelecting) {
                    this.finishSelection(e);
                }
            });
            
            this.canvas.addEventListener('contextmenu', (e) => {
                e.preventDefault();
            });
        }
        
        setupToolbarButtons() {
            // Select tool button
            const selectToolBtn = document.getElementById('eprSelectToolBtn');
            if (selectToolBtn) {
                selectToolBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    this.toolMode = 'select';
                    selectToolBtn.classList.add('active');
                    const dragBtn = document.getElementById('eprDragModeBtn');
                    if (dragBtn) dragBtn.classList.remove('active');
                    this.canvas.style.cursor = 'default';
                    console.log('[EPR Canvas] Select tool activated');
                });
            }
            
            // Drag/Pan mode button
            const dragModeBtn = document.getElementById('eprDragModeBtn');
            if (dragModeBtn) {
                dragModeBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    this.toolMode = 'pan';
                    dragModeBtn.classList.add('active');
                    if (selectToolBtn) selectToolBtn.classList.remove('active');
                    this.canvas.style.cursor = 'grab';
                    console.log('[EPR Canvas] Drag/Pan tool activated');
                });
            }
            
            // Grid toggle
            const toggleGridBtn = document.getElementById('eprToggleGridBtn');
            if (toggleGridBtn) {
                toggleGridBtn.addEventListener('click', (e) => {
                    // Don't prevent default - let button work normally
                    console.log('[EPR Canvas] Grid toggle button clicked');
                    this.toggleGrid();
                });
                console.log('[EPR Canvas] Grid toggle button handler attached');
            } else {
                console.warn('[EPR Canvas] Grid toggle button not found!');
            }
            
            // Snap to grid toggle
            const snapToGridBtn = document.getElementById('eprSnapToGridBtn');
            if (snapToGridBtn) {
                // Initialize icon color based on current state
                const icon = snapToGridBtn.querySelector('i');
                if (icon && this.snapToGrid) {
                    icon.style.color = '#28a745';
                }
                
                snapToGridBtn.addEventListener('click', () => {
                    this.toggleSnapToGrid();
                });
            }
        }
        
        updateTransform() {
            const transform = `translate(${this.panOffset.x}px, ${this.panOffset.y}px) scale(${this.zoomLevel})`;
            this.nodesLayer.style.transform = transform;
            this.connectionsLayer.style.transform = transform;
        }
        
        setupDragAndDrop() {
            if (!this.canvas) {
                console.error('[EPR Canvas] Canvas not available for drag and drop');
                return;
            }
            
            const canvasContainer = document.getElementById('eprCanvasContainer');
            if (!canvasContainer) {
                console.error('[EPR Canvas] Canvas container not found');
                return;
            }
            
            console.log('[EPR Canvas] Setting up drag and drop...');
            
            // Handler function for drop
            const handleDrop = (e) => {
                e.preventDefault();
                e.stopPropagation();
                
                // Remove drag-over class from all elements
                this.canvas.classList.remove('drag-over');
                if (canvasContainer) canvasContainer.classList.remove('drag-over');
                
                // Try to get data - check both application/json and text/plain
                let data = null;
                try {
                    data = e.dataTransfer.getData('application/json');
                    if (!data || data === '') {
                        data = e.dataTransfer.getData('text/plain');
                    }
                } catch (err) {
                    console.warn('[EPR Canvas] Error getting data:', err);
                }
                
                // Also check dataTransfer.types for debugging
                const availableTypes = Array.from(e.dataTransfer.types || []);
                console.log('[EPR Canvas] Drop event, data:', data, 'types:', availableTypes, 'items:', e.dataTransfer.items?.length);
                
                if (!data || data === '') {
                    console.warn('[EPR Canvas] No drop data found. Available types:', availableTypes);
                    return;
                }
                
                try {
                    // First, try to parse as JSON to see if it's node data
                    let nodeData = null;
                    let isItemId = false;
                    
                    try {
                        nodeData = JSON.parse(data);
                        // If parsing succeeds, check if it's actually an item ID (just a string)
                        if (typeof nodeData === 'string') {
                            isItemId = true;
                            nodeData = null;
                        }
                    } catch (parseErr) {
                        // Not JSON, might be an item ID
                        isItemId = true;
                    }
                    
                    // Check if this is an item ID being dragged out of a group
                    if (isItemId && !nodeData) {
                        const itemNode = this.nodes.get(data);
                        if (itemNode && itemNode.groupId) {
                            // Item is being dragged out of a group
                            const rect = this.canvas.getBoundingClientRect();
                            const rawX = e.clientX - rect.left;
                            const rawY = e.clientY - rect.top;
                            const x = (rawX / (this.zoomLevel || 1)) - (this.panOffset?.x || 0);
                            const y = (rawY / (this.zoomLevel || 1)) - (this.panOffset?.y || 0);
                            
                            let finalX = Math.max(0, x);
                            let finalY = Math.max(0, y);
                            if (this.snapToGrid) {
                                finalX = Math.round(finalX / this.gridSize) * this.gridSize;
                                finalY = Math.round(finalY / this.gridSize) * this.gridSize;
                            }
                            
                            // Remove from group and show on canvas
                            this.removeItemFromGroup(itemNode.id);
                            
                            // Position the item
                            itemNode.x = finalX;
                            itemNode.y = finalY;
                            const itemNodeEl = document.querySelector(`[data-node-id="${itemNode.id}"]`);
                            if (itemNodeEl) {
                                itemNodeEl.style.display = '';
                                itemNodeEl.style.left = `${finalX}px`;
                                itemNodeEl.style.top = `${finalY}px`;
                                itemNodeEl.removeAttribute('data-in-group');
                            }
                            
                            // If it's a packaging item, also show connected raw materials
                            if (itemNode.type === 'packaging') {
                                this.connections.forEach(conn => {
                                    if (conn.to === itemNode.id) {
                                        const rawMaterialNode = this.nodes.get(conn.from);
                                        if (rawMaterialNode && rawMaterialNode.type === 'raw-material') {
                                            rawMaterialNode.x = finalX - 150;
                                            rawMaterialNode.y = finalY;
                                            const rawMaterialEl = document.querySelector(`[data-node-id="${rawMaterialNode.id}"]`);
                                            if (rawMaterialEl) {
                                                rawMaterialEl.style.display = '';
                                                rawMaterialEl.style.left = `${rawMaterialNode.x}px`;
                                                rawMaterialEl.style.top = `${rawMaterialNode.y}px`;
                                                rawMaterialEl.removeAttribute('data-in-group');
                                            }
                                        }
                                    }
                                });
                            }
                            
                            this.updateConnections();
                            this.saveState();
                            return;
                        }
                    }
                    
                    // If we have nodeData from JSON parsing, use it
                    if (!nodeData) {
                        // Try parsing again if we didn't get nodeData
                        try {
                            nodeData = JSON.parse(data);
                        } catch (parseError) {
                            console.error('[EPR Canvas] Failed to parse JSON:', parseError, 'Data:', data);
                            return;
                        }
                    }
                    
                    if (!nodeData || !nodeData.type) {
                        console.error('[EPR Canvas] Invalid node data:', nodeData);
                        return;
                    }
                    
                    const rect = this.canvas.getBoundingClientRect();
                    
                    // Calculate drop position - account for canvas transform
                    const rawX = e.clientX - rect.left;
                    const rawY = e.clientY - rect.top;
                    
                    // Account for pan offset and zoom level
                    const x = (rawX / (this.zoomLevel || 1)) - (this.panOffset?.x || 0);
                    const y = (rawY / (this.zoomLevel || 1)) - (this.panOffset?.y || 0);
                    
                    let finalX = Math.max(0, x);
                    let finalY = Math.max(0, y);
                    if (this.snapToGrid) {
                        finalX = Math.round(finalX / this.gridSize) * this.gridSize;
                        finalY = Math.round(finalY / this.gridSize) * this.gridSize;
                    }
                    
                    nodeData.x = finalX;
                    nodeData.y = finalY;
                    
                    // Initialize taxonomy parameters for raw materials if taxonomyId is provided
                    if (nodeData.type === 'raw-material' && nodeData.taxonomyId && !nodeData.parameters) {
                        nodeData.parameters = {
                            taxonomyId: nodeData.taxonomyId,
                            taxonomyCode: nodeData.taxonomyCode || null,
                            level1Classification: nodeData.name || '',
                            level2Classification: null,
                            level3Classification: null,
                            level4Classification: null,
                            level5Classification: null
                        };
                    }
                    
                    console.log('[EPR Canvas] Adding node with data:', nodeData, 'type:', nodeData.type);
                    const node = this.addNode(nodeData);
                    console.log('[EPR Canvas] ✅ Node added:', node.id, node.name, 'at position', finalX, finalY);
                } catch (error) {
                    console.error('[EPR Canvas] Error parsing drop data:', error, 'Raw data:', data);
                    // If parsing fails, check if it's an item ID
                    const itemNode = this.nodes.get(data);
                    if (itemNode && itemNode.groupId) {
                        // Handle as item dragged out of group
                        this.removeItemFromGroup(itemNode.id);
                    } else {
                        alert('Error adding item: ' + error.message);
                    }
                }
            };
            
            // Handler function for dragover
            const handleDragOver = (e) => {
                e.preventDefault();
                e.stopPropagation();
                this.canvas.classList.add('drag-over');
                if (canvasContainer) canvasContainer.classList.add('drag-over');
                // Check if dragging an item from a group by checking types
                const types = Array.from(e.dataTransfer.types || []);
                // If dragging text/plain and it might be an item ID, allow move
                if (types.includes('text/plain')) {
                    e.dataTransfer.dropEffect = 'move';
                } else {
                    e.dataTransfer.dropEffect = 'copy';
                }
            };
            
            // Handler function for dragleave
            const handleDragLeave = (e) => {
                e.preventDefault();
                e.stopPropagation();
                if (!this.canvas.contains(e.relatedTarget) && 
                    (!canvasContainer || !canvasContainer.contains(e.relatedTarget))) {
                    this.canvas.classList.remove('drag-over');
                    if (canvasContainer) canvasContainer.classList.remove('drag-over');
                }
            };
            
            // Attach handlers to canvas container (catches all drops)
            canvasContainer.addEventListener('dragover', handleDragOver);
            canvasContainer.addEventListener('dragleave', handleDragLeave);
            canvasContainer.addEventListener('drop', handleDrop);
            
            // Also attach to canvas itself
            this.canvas.addEventListener('dragover', handleDragOver);
            this.canvas.addEventListener('dragleave', handleDragLeave);
            this.canvas.addEventListener('drop', handleDrop);
            
            // Attach to child elements (nodesLayer, connectionsLayer)
            if (this.nodesLayer) {
                this.nodesLayer.addEventListener('dragover', handleDragOver);
                this.nodesLayer.addEventListener('dragleave', handleDragLeave);
                this.nodesLayer.addEventListener('drop', handleDrop);
            }
            
            if (this.connectionsLayer) {
                // For connections layer, ensure drops work even with pointer-events: stroke
                this.connectionsLayer.addEventListener('dragover', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    handleDragOver(e);
                });
                this.connectionsLayer.addEventListener('dragleave', handleDragLeave);
                this.connectionsLayer.addEventListener('drop', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    handleDrop(e);
                });
            }
            
            // Ensure canvas accepts drops
            if (this.canvas) {
                this.canvas.style.pointerEvents = 'auto';
            }
            if (canvasContainer) {
                canvasContainer.style.pointerEvents = 'auto';
            }
            
            console.log('[EPR Canvas] ✅ Drag and drop handlers attached to canvas, container, and layers');
        }
        
        addNode(nodeData) {
            // If no x/y provided, place to the right of the last node: position + width + 60px
            let defaultX = 100;
            let defaultY = 100;
            
            if ((nodeData.x === undefined || nodeData.x === null) && 
                (nodeData.y === undefined || nodeData.y === null) && 
                this.nodes.size > 0) {
                const nodes = Array.from(this.nodes.values());
                const lastNode = nodes[nodes.length - 1];
                // Get the last node's DOM element to get its width
                const lastNodeEl = document.querySelector(`[data-node-id="${lastNode.id}"]`);
                let lastNodeWidth = 200; // Default width if element not found
                if (lastNodeEl) {
                    const rect = lastNodeEl.getBoundingClientRect();
                    lastNodeWidth = rect.width;
                }
                defaultX = lastNode.x + lastNodeWidth + 60;
                defaultY = lastNode.y;
            }
            
            // Apply snap to grid if enabled
            let finalX = nodeData.x !== undefined && nodeData.x !== null ? nodeData.x : defaultX;
            let finalY = nodeData.y !== undefined && nodeData.y !== null ? nodeData.y : defaultY;
            
            if (this.snapToGrid) {
                finalX = Math.round(finalX / this.gridSize) * this.gridSize;
                finalY = Math.round(finalY / this.gridSize) * this.gridSize;
            }
            
            const node = {
                id: nodeData.id || `node-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`,
                type: nodeData.type,
                entityId: nodeData.entityId,
                x: finalX,
                y: finalY,
                name: nodeData.name || 'Untitled',
                icon: nodeData.icon || 'bi-circle',
                parameters: nodeData.parameters || {},
                ...nodeData
            };
            
            this.nodes.set(node.id, node);
            this.renderNode(node);
            this.updatePlaceholder();
            this.saveState();
            return node;
        }
        
        /**
         * Determine the hierarchy level of a Packaging Group based on connections
         * Returns: 'Primary', 'Secondary', 'Tertiary', 'Quaternary', or null
         */
        getPackagingGroupHierarchyLevel(packagingGroupId) {
            const groupNode = this.nodes.get(packagingGroupId);
            if (!groupNode || groupNode.type !== 'packaging-group') {
                return null;
            }
            
            // Check if directly connected to a product (Primary)
            const connectedToProduct = this.connections.some(conn => {
                if (conn.from === packagingGroupId) {
                    const toNode = this.nodes.get(conn.to);
                    return toNode && toNode.type === 'product';
                }
                if (conn.to === packagingGroupId) {
                    const fromNode = this.nodes.get(conn.from);
                    return fromNode && fromNode.type === 'product';
                }
                return false;
            });
            
            if (connectedToProduct) {
                return 'Primary';
            }
            
            // Check if directly connected to a Primary Packaging Group (Secondary)
            const connectedToPrimary = this.connections.some(conn => {
                let otherGroupId = null;
                if (conn.from === packagingGroupId) {
                    otherGroupId = conn.to;
                } else if (conn.to === packagingGroupId) {
                    otherGroupId = conn.from;
                }
                
                if (otherGroupId) {
                    const otherGroup = this.nodes.get(otherGroupId);
                    if (otherGroup && otherGroup.type === 'packaging-group') {
                        const otherLevel = this.getPackagingGroupHierarchyLevel(otherGroupId);
                        return otherLevel === 'Primary';
                    }
                }
                return false;
            });
            
            if (connectedToPrimary) {
                return 'Secondary';
            }
            
            // Check if directly connected to a Secondary Packaging Group (Tertiary)
            const connectedToSecondary = this.connections.some(conn => {
                let otherGroupId = null;
                if (conn.from === packagingGroupId) {
                    otherGroupId = conn.to;
                } else if (conn.to === packagingGroupId) {
                    otherGroupId = conn.from;
                }
                
                if (otherGroupId) {
                    const otherGroup = this.nodes.get(otherGroupId);
                    if (otherGroup && otherGroup.type === 'packaging-group') {
                        const otherLevel = this.getPackagingGroupHierarchyLevel(otherGroupId);
                        return otherLevel === 'Secondary';
                    }
                }
                return false;
            });
            
            if (connectedToSecondary) {
                return 'Tertiary';
            }
            
            // Check if connected to any other Packaging Group (Quaternary)
            const connectedToAnyGroup = this.connections.some(conn => {
                let otherGroupId = null;
                if (conn.from === packagingGroupId) {
                    otherGroupId = conn.to;
                } else if (conn.to === packagingGroupId) {
                    otherGroupId = conn.from;
                }
                
                if (otherGroupId) {
                    const otherGroup = this.nodes.get(otherGroupId);
                    if (otherGroup && otherGroup.type === 'packaging-group') {
                        const otherLevel = this.getPackagingGroupHierarchyLevel(otherGroupId);
                        // If connected to Tertiary or any other Packaging Group that's not Primary/Secondary
                        return otherLevel === 'Tertiary' || (otherLevel !== 'Primary' && otherLevel !== 'Secondary');
                    }
                }
                return false;
            });
            
            if (connectedToAnyGroup) {
                return 'Quaternary';
            }
            
            // No connections to products or other groups - return null (no pill)
            return null;
        }
        
        renderNode(node) {
            const existing = document.querySelector(`[data-node-id="${node.id}"]`);
            if (existing) {
                existing.remove();
            }
            
            if (!this.nodesLayer) {
                console.error('[EPR Canvas] Nodes layer not found!');
                return;
            }
            
            const nodeEl = document.createElement('div');
            nodeEl.className = 'epr-canvas-node';
            // Don't add epr-transport-node class - transport nodes should be hidden when between nodes
            // The old large orange transport node UI is removed
            // Transport nodes are now displayed as small grey boxes on connection lines
            if (node.type === 'packaging-group') {
                nodeEl.classList.add('epr-packaging-group-node');
            }
            if (node.type === 'distribution-group') {
                nodeEl.classList.add('epr-distribution-group-node');
            }
            nodeEl.setAttribute('data-node-id', node.id);
            nodeEl.setAttribute('data-type', node.type);
            nodeEl.setAttribute('data-locked', node.locked ? 'true' : 'false');
            nodeEl.style.position = 'absolute';
            nodeEl.style.left = `${node.x}px`;
            nodeEl.style.top = `${node.y}px`;
            nodeEl.style.pointerEvents = 'all';
            
            // For distribution groups, ensure proper sizing
            if (node.type === 'distribution-group') {
                if (node.width) {
                    nodeEl.style.width = node.width + 'px';
                }
                if (node.height) {
                    nodeEl.style.height = node.height + 'px';
                }
                nodeEl.style.overflow = 'visible';
            }
            
            // Hide nodes that are inside packaging groups or distribution groups
            // Only hide if groupId is explicitly set and is a valid group
            if (node.groupId) {
                const groupNode = this.nodes.get(node.groupId);
                if (groupNode) {
                    if (groupNode.type === 'packaging-group' && (node.type === 'packaging' || node.type === 'raw-material')) {
                        nodeEl.style.display = 'none';
                        nodeEl.setAttribute('data-in-group', 'true');
                        // Set parent-item attribute if this is nested under a packaging item
                        if (node.parentItemId) {
                            nodeEl.setAttribute('data-parent-item', node.parentItemId);
                        }
                    } else if (groupNode.type === 'distribution-group' && node.type === 'distribution') {
                        // Hide distribution nodes when in a distribution group - they're shown in the group's list
                        nodeEl.style.display = 'none';
                        nodeEl.setAttribute('data-in-group', 'true');
                    } else if (groupNode.type === 'distribution' && node.type === 'distribution') {
                        // Nested distribution nodes (hub under manufacturer, store under hub) - hide them too
                        // They should only appear in the distribution group's nested list
                        nodeEl.style.display = 'none';
                        nodeEl.setAttribute('data-in-group', 'true');
                    } else {
                        // Invalid groupId, clear it
                        node.groupId = undefined;
                        nodeEl.style.display = '';
                        nodeEl.removeAttribute('data-in-group');
                        nodeEl.removeAttribute('data-parent-item');
                    }
                } else {
                    // Invalid groupId, clear it
                    node.groupId = undefined;
                    nodeEl.style.display = '';
                    nodeEl.removeAttribute('data-in-group');
                    nodeEl.removeAttribute('data-parent-item');
                }
            } else {
                nodeEl.style.display = '';
                nodeEl.removeAttribute('data-in-group');
                nodeEl.removeAttribute('data-parent-item');
            }
            
            // Hide transport nodes that are displayed as small boxes on connections
            // Transport nodes should only be visible as regular nodes if they're not between two connected nodes
            if (node.type === 'transport') {
                // Find incoming connection (something -> transport)
                const incomingConn = this.connections.find(c => c.to === node.id);
                // Find outgoing connection (transport -> something)
                const outgoingConn = this.connections.find(c => c.from === node.id);
                
                if (incomingConn && outgoingConn) {
                    // Transport node is in a path between two nodes
                    // ALWAYS hide it - it will be displayed as a small box on the connection line
                    // Use MULTIPLE methods to ensure it's hidden - this is CRITICAL
                    nodeEl.style.display = 'none';
                    nodeEl.style.setProperty('display', 'none', 'important');
                    nodeEl.style.visibility = 'hidden';
                    nodeEl.style.setProperty('visibility', 'hidden', 'important');
                    nodeEl.style.opacity = '0';
                    nodeEl.style.setProperty('opacity', '0', 'important');
                    nodeEl.style.height = '0';
                    nodeEl.style.setProperty('height', '0', 'important');
                    nodeEl.style.width = '0';
                    nodeEl.style.setProperty('width', '0', 'important');
                    nodeEl.style.position = 'absolute';
                    nodeEl.style.setProperty('position', 'absolute', 'important');
                    nodeEl.style.left = '-9999px';
                    nodeEl.style.setProperty('left', '-9999px', 'important');
                    nodeEl.setAttribute('data-transport-on-connection', 'true');
                    nodeEl.classList.add('epr-transport-hidden');
                } else {
                    // Transport node is not between two nodes - show as regular node if enabled
                    nodeEl.removeAttribute('data-transport-on-connection');
                    nodeEl.classList.remove('epr-transport-hidden');
                    // Reset all hiding styles
                    nodeEl.style.removeProperty('display');
                    nodeEl.style.removeProperty('visibility');
                    nodeEl.style.removeProperty('opacity');
                    nodeEl.style.removeProperty('height');
                    nodeEl.style.removeProperty('width');
                    nodeEl.style.removeProperty('position');
                    nodeEl.style.removeProperty('left');
                    if (this.showTransportNodes) {
                        nodeEl.style.display = '';
                    } else {
                        nodeEl.style.display = 'none';
                    }
                }
            }
            
            // Determine which connection ports to show based on node type
            let leftPort = '';
            let rightPort = '';
            
            // Left port rules (what can connect TO this node from the left)
            const leftPortRules = {
                'raw-material': false, // Raw materials can't receive left connections
                'packaging': true,     // Packaging can receive from Raw Materials
                'product': true,       // Product can receive from Packaging or Supplier Packaging
                'supplier-packaging': false, // Supplier packaging connects TO product (right port)
                'distribution': true,   // Distribution can receive from Product or Distribution
                'transport': true,      // Transport can receive from any node
                'packaging-group': true, // Packaging Groups can receive from other groups or items
                'distribution-group': true // Distribution Groups can receive from other groups or items
            };
            
            // Right port rules (what this node can connect TO on the right)
            const rightPortRules = {
                'raw-material': true,  // Raw Materials can connect to Packaging
                'packaging': true,      // Packaging can connect to Product
                'product': true,        // Product can connect to Distribution
                'supplier-packaging': true,  // Supplier Packaging can connect to Product
                'distribution': true,   // Distribution can connect to Distribution
                'transport': true,       // Transport can connect to any node
                'packaging-group': true,  // Packaging Groups can connect to other groups
                'distribution-group': true  // Distribution Groups can connect to other groups
            };
            
            if (leftPortRules[node.type]) {
                leftPort = `<div class="epr-connection-port epr-port-left" data-node-id="${node.id}" data-port="left" style="position: absolute; left: -8px; top: 50%; transform: translateY(-50%); width: 16px; height: 16px; background: #007bff; border: 2px solid white; border-radius: 50%; cursor: crosshair; z-index: 10;" title="Drag to connect"></div>`;
            }
            
            if (rightPortRules[node.type]) {
                rightPort = `<div class="epr-connection-port epr-port-right" data-node-id="${node.id}" data-port="right" style="position: absolute; right: -8px; top: 50%; transform: translateY(-50%); width: 16px; height: 16px; background: #28a745; border: 2px solid white; border-radius: 50%; cursor: crosshair; z-index: 10;" title="Drag to connect"></div>`;
            }
            
            // For transport nodes, show transport types and distances with icons
            let transportInfo = '';
            if (node.type === 'transport' && node.transportTypes && node.transportTypes.length > 0) {
                // Helper function to get icon class for transport type
                const getTransportIconClass = (transportType) => {
                    const typeLower = (transportType || '').toLowerCase().trim();
                    if (!typeLower || typeLower === '') {
                        return 'bi-truck';
                    } else if (typeLower === 'air' || typeLower === 'airplane' || typeLower === 'plane' || 
                              typeLower.includes('air') || typeLower.includes('plane')) {
                        return 'bi-airplane';
                    } else if (typeLower === 'ship' || typeLower === 'sea' || 
                              typeLower.includes('ship') || typeLower.includes('sea')) {
                        return 'bi-ship';
                    } else if (typeLower === 'train' || typeLower === 'rail' || typeLower === 'railway' ||
                              typeLower.includes('train') || typeLower.includes('rail')) {
                        return 'bi-train-front';
                    } else if (typeLower === 'van' || typeLower.includes('van')) {
                        return 'bi-car-front';
                    } else if (typeLower === 'truck' || typeLower.includes('truck')) {
                        return 'bi-truck';
                    } else {
                        return 'bi-truck';
                    }
                };
                
                // Create icon list with numbers
                const transportItems = node.transportTypes.map(t => {
                    const iconClass = getTransportIconClass(t.type);
                    return `<div style="display: flex; align-items: center; margin: 0.15rem 0;">
                        <i class="bi ${iconClass}" style="font-size: 0.9rem; color: #6c757d; margin-right: 0.3rem;"></i>
                        <span style="font-size: 0.75rem; color: #6c757d;">${t.distance || '0'}</span>
                    </div>`;
                }).join('');
                
                transportInfo = `<div class="epr-transport-info" style="margin-top: 0.25rem;">
                    ${transportItems}
                </div>`;
            }
            
            // Determine Packaging Group hierarchy level for pills
            let packagingHierarchyPill = '';
            if (node.type === 'packaging-group') {
                const hierarchyLevel = this.getPackagingGroupHierarchyLevel(node.id);
                if (hierarchyLevel) {
                    const pillColors = {
                        'Primary': { bg: '#007bff', text: '#fff' },
                        'Secondary': { bg: '#28a745', text: '#fff' },
                        'Tertiary': { bg: '#ffc107', text: '#000' },
                        'Quaternary': { bg: '#6c757d', text: '#fff' }
                    };
                    const color = pillColors[hierarchyLevel] || pillColors['Quaternary'];
                    // Set width based on hierarchy level text (matching "Secondary" = 61px pattern)
                    const widthMap = {
                        'Primary': '50px',
                        'Secondary': '61px',
                        'Tertiary': '55px',
                        'Quaternary': '70px'
                    };
                    const pillWidth = widthMap[hierarchyLevel] || '61px';
                    packagingHierarchyPill = `<span class="epr-packaging-hierarchy-pill" style="position: absolute; top: -24px; left: 0px; width: ${pillWidth}; background: ${color.bg}; color: ${color.text}; padding: 2px 8px; border-radius: 5px; font-size: 6pt; font-weight: 600; white-space: nowrap; border: 2px solid white; z-index: 100;">${hierarchyLevel}</span>`;
                }
            }
            
            // For packaging-group nodes, show contained items visually
            let groupContent = '';
            if (node.type === 'packaging-group') {
                // Ensure containedItems is always an array
                if (!node.containedItems) {
                    node.containedItems = [];
                }
                const containedItems = node.containedItems || [];
                const packagingItems = containedItems.filter(itemId => {
                    const itemNode = this.nodes.get(itemId);
                    return itemNode && itemNode.type === 'packaging';
                });
                const rawMaterials = containedItems.filter(itemId => {
                    const itemNode = this.nodes.get(itemId);
                    return itemNode && itemNode.type === 'raw-material';
                });
                
                // Build list of contained items - nest raw materials under packaging items
                let itemsList = '';
                if (containedItems.length > 0) {
                    itemsList = '<div class="epr-group-items-list" style="margin-top: 0.5rem; padding-top: 0.5rem; border-top: 1px solid #dee2e6;">';
                    
                    // First, show packaging items with their raw materials nested
                    packagingItems.forEach(packagingId => {
                        const packagingNode = this.nodes.get(packagingId);
                        if (packagingNode) {
                            itemsList += `<div class="epr-group-item epr-group-packaging-item" data-item-id="${packagingId}" style="font-size: 0.7rem; padding: 0.25rem 0.5rem; margin: 0.25rem 0; background: white; border-radius: 3px; border-left: 3px solid #007bff; cursor: pointer;">
                                📦 ${this.escapeHtml(packagingNode.name)}
                            </div>`;
                            
                            // Find raw materials in the same group that are nested under this packaging item
                            // Check parentItemId first (for loaded projects), then fall back to connections
                            const groupRawMaterials = rawMaterials.filter(rawId => {
                                const rawNode = this.nodes.get(rawId);
                                if (!rawNode) return false;
                                // Must be in the same group
                                if (rawNode.groupId !== node.id) return false;
                                // Check if parentItemId matches (for loaded projects)
                                if (rawNode.parentItemId === packagingId) return true;
                                // Fall back to checking connections
                                return this.connections.some(conn => 
                                    conn.from === rawId && conn.to === packagingId
                                );
                            });
                            
                            // Show nested raw materials
                            groupRawMaterials.forEach(rawId => {
                                const rawNode = this.nodes.get(rawId);
                                if (rawNode) {
                                    itemsList += `<div class="epr-group-item epr-group-raw-material-item" data-item-id="${rawId}" style="font-size: 0.65rem; padding: 0.2rem 0.5rem 0.2rem 1.5rem; margin: 0.1rem 0; background: #f8f9fa; border-radius: 3px; border-left: 3px solid #6c757d; cursor: pointer;">
                                        🔵 ${this.escapeHtml(rawNode.name)}
                                    </div>`;
                                }
                            });
                        }
                    });
                    
                    // Show any raw materials not connected to packaging items (already in group, so no need to check groupId)
                    const orphanRawMaterials = rawMaterials.filter(rawId => {
                        const rawNode = this.nodes.get(rawId);
                        if (!rawNode) return false;
                        // Check if this raw material is NOT connected to any packaging in the group
                        // Note: rawMaterials already contains only items in this group
                        return !this.connections.some(conn => 
                            conn.from === rawId && packagingItems.includes(conn.to)
                        );
                    });
                    
                    orphanRawMaterials.forEach(rawId => {
                        const rawNode = this.nodes.get(rawId);
                        if (rawNode) {
                            itemsList += `<div class="epr-group-item epr-group-raw-material-item" data-item-id="${rawId}" style="font-size: 0.7rem; padding: 0.25rem 0.5rem; margin: 0.25rem 0; background: white; border-radius: 3px; border-left: 3px solid #6c757d; cursor: pointer;">
                                🔵 ${this.escapeHtml(rawNode.name)}
                            </div>`;
                        }
                    });
                    
                    itemsList += '</div>';
                }
                
                groupContent = `<div class="epr-group-content" style="font-size: 0.75rem; color: #495057; margin-top: 0.5rem;">
                    <div style="font-weight: 600; margin-bottom: 0.25rem; padding: 0.5rem; background: #f8f9fa; border-radius: 4px;">
                        <div>Packaging Items: ${packagingItems.length}</div>
                        <div>Raw Materials: ${rawMaterials.length}</div>
                    </div>
                    ${itemsList}
                </div>`;
            }
            
            // For distribution-group nodes, show contained items visually with hierarchy
            if (node.type === 'distribution-group') {
                // Ensure containedItems is always an array
                if (!node.containedItems) {
                    node.containedItems = [];
                }
                const containedItems = node.containedItems || [];
                const distributionItems = containedItems.filter(itemId => {
                    const itemNode = this.nodes.get(itemId);
                    return itemNode && itemNode.type === 'distribution';
                });
                
                // Build hierarchy using groupId relationships (not connections)
                // Find root nodes: nodes directly in the distribution group (groupId === node.id)
                // These are Level 1 nodes like the manufacturer
                const rootNodes = [];
                
                // First, try to find nodes with groupId pointing directly to the distribution group
                // These are Level 1 (Manufacturer)
                this.nodes.forEach((itemNode, itemId) => {
                    if (itemNode.type === 'distribution' && itemNode.groupId === node.id) {
                        rootNodes.push(itemId);
                    }
                });
                
                // If no root nodes found by groupId, check containedItems
                // Find nodes in containedItems that have groupId pointing to the distribution group
                if (rootNodes.length === 0 && distributionItems.length > 0) {
                    distributionItems.forEach(itemId => {
                        const itemNode = this.nodes.get(itemId);
                        if (itemNode && itemNode.type === 'distribution' && itemNode.groupId === node.id) {
                            if (!rootNodes.includes(itemId)) {
                                rootNodes.push(itemId);
                            }
                        }
                    });
                }
                
                // If still no root nodes, look for manufacturer by name
                // This is a fallback - ideally manufacturer should have groupId = distributionGroupId
                if (rootNodes.length === 0 && distributionItems.length > 0) {
                    // Try to find manufacturer by name (M&S Food Supplies Ltd)
                    const manufacturer = distributionItems.find(itemId => {
                        const itemNode = this.nodes.get(itemId);
                        if (!itemNode) return false;
                        const name = (itemNode.name || '').toLowerCase();
                        return name.includes('food supplies') || 
                               name.includes('manufacturer') ||
                               name.includes('m&s food');
                    });
                    
                    if (manufacturer) {
                        rootNodes.push(manufacturer);
                    } else if (distributionItems.length > 0) {
                        // Last resort: use first item in containedItems as root
                        rootNodes.push(distributionItems[0]);
                    }
                }
                
                console.log(`[EPR Canvas] Distribution group hierarchy: Found ${rootNodes.length} root node(s)`, rootNodes.map(id => {
                    const n = this.nodes.get(id);
                    return n ? n.name : id;
                }));
                
                // Helper function to determine node type for hierarchy
                const getNodeType = (node) => {
                    if (!node || !node.parameters) return 'unknown';
                    const locationType = node.parameters.locationType;
                    if (locationType === 'Manufacturer') return 'manufacturer';
                    if (locationType === 'Hub') return 'hub';
                    if (locationType === 'Store') return 'store';
                    // If no locationType, check if it's the manufacturer (has address parameter)
                    if (node.parameters.address) return 'manufacturer';
                    // Default: assume it's a manufacturer if it's a root node
                    return 'manufacturer';
                };
                
                // Helper function to check if a node is in the distribution group
                // This should return true for ALL nodes in the hierarchy: manufacturer, hubs, and stores
                const isNodeInDistributionGroup = (nodeId) => {
                    const checkNode = this.nodes.get(nodeId);
                    if (!checkNode || checkNode.type !== 'distribution') return false;
                    
                    // Direct check: is it in containedItems?
                    if (containedItems.includes(nodeId)) {
                        return true;
                    }
                    
                    // Check if it has groupId pointing to the distribution group (manufacturer)
                    if (checkNode.groupId === node.id) {
                        return true;
                    }
                    
                    // Walk up the hierarchy to see if it leads to this distribution group
                    // This will find stores under hubs, and hubs under manufacturer
                    let currentNode = checkNode;
                    let depth = 0;
                    const visited = new Set();
                    
                    while (currentNode && currentNode.groupId && depth < 10) {
                        if (visited.has(currentNode.id)) break; // Prevent infinite loops
                        visited.add(currentNode.id);
                        
                        const parent = this.nodes.get(currentNode.groupId);
                        if (!parent) break;
                        
                        // If parent is the distribution group, we're in the group
                        if (parent.id === node.id) {
                            return true;
                        }
                        
                        // If parent is in containedItems, we're in the group
                        if (containedItems.includes(parent.id)) {
                            return true;
                        }
                        
                        // If parent is a different distribution group, stop
                        if (parent.type === 'distribution-group') {
                            return false;
                        }
                        
                        // Continue walking up (e.g., store -> hub -> manufacturer -> distribution group)
                        currentNode = parent;
                        depth++;
                    }
                    
                    return false;
                };
                
                // Helper function to find children by groupId (nodes that have this node as their groupId)
                // Search ALL nodes to find nested children
                // For distribution groups, we need to find the full hierarchy:
                // Level 1: Manufacturer (groupId = distributionGroupId, has address parameter)
                // Level 2: Hubs (groupId = manufacturerId, parameters.locationType = "Hub")
                // Level 3: Stores (groupId = hubId, parameters.locationType = "Store")
                const findChildrenByGroupId = (parentNodeId) => {
                    const children = [];
                    const parentNode = this.nodes.get(parentNodeId);
                    if (!parentNode) return children;
                    
                    // Get parent type once, outside the loop
                    const parentType = getNodeType(parentNode);
                    
                    // Search ALL nodes to find children
                    this.nodes.forEach((childNode, childNodeId) => {
                        // If this node's groupId matches the parent, it's a child
                        if (childNode.groupId === parentNodeId && childNode.type === 'distribution') {
                            const childType = getNodeType(childNode);
                            
                            // Check if it's in the distribution group (walk up hierarchy)
                            const isInGroup = isNodeInDistributionGroup(childNodeId);
                            
                            if (isInGroup) {
                                // Validate hierarchy
                                if (parentType === 'manufacturer' && childType === 'hub') {
                                    // Hub under manufacturer - correct
                                    children.push(childNodeId);
                                } else if (parentType === 'hub' && childType === 'store') {
                                    // Store under hub - correct
                                    children.push(childNodeId);
                                } else if (parentType === 'manufacturer' && childType === 'store') {
                                    // Store directly under manufacturer - might be unassigned
                                    // Skip these - they should be under hubs
                                } else {
                                    // Other combinations - include them (fallback)
                                    children.push(childNodeId);
                                }
                            }
                        }
                    });
                    
                    // Debug logging for hubs
                    if (parentType === 'hub' && children.length > 0) {
                        console.log(`[EPR Canvas] Hub "${parentNode.name}" has ${children.length} stores:`, 
                            children.map(id => {
                                const n = this.nodes.get(id);
                                return n ? n.name : id;
                            }).slice(0, 5));
                    }
                    
                    // Sort: hubs first (if parent is manufacturer), then by name
                    return children.sort((a, b) => {
                        const nodeA = this.nodes.get(a);
                        const nodeB = this.nodes.get(b);
                        const typeA = getNodeType(nodeA);
                        const typeB = getNodeType(nodeB);
                        
                        // If parent is manufacturer, show hubs before stores
                        if (parentType === 'manufacturer') {
                            if (typeA === 'hub' && typeB === 'store') return -1;
                            if (typeA === 'store' && typeB === 'hub') return 1;
                        }
                        
                        // Otherwise sort by name
                        return (nodeA?.name || '').localeCompare(nodeB?.name || '');
                    });
                };
                
                // Helper function to format store names with word wrapping at 50 characters
                const formatStoreList = (storeNames) => {
                    if (storeNames.length === 0) return '';
                    
                    // Join with commas and spaces
                    const text = storeNames.join(', ');
                    
                    // Break at 50 characters, respecting comma boundaries
                    const maxLength = 50;
                    let result = '';
                    let currentLine = '';
                    
                    // Split by comma and space, but keep track of separators
                    const parts = text.split(', ');
                    parts.forEach((part, index) => {
                        const separator = index === 0 ? '' : ', ';
                        const testLine = currentLine + separator + part;
                        
                        if (testLine.length <= maxLength) {
                            currentLine = testLine;
                        } else {
                            if (currentLine) {
                                result += (result ? '<br>' : '') + currentLine;
                            }
                            currentLine = part;
                        }
                    });
                    
                    if (currentLine) {
                        result += (result ? '<br>' : '') + currentLine;
                    }
                    
                    return result || text; // Fallback to original text if formatting fails
                };
                
                // Helper function to render a node and its children recursively using groupId hierarchy
                const renderNodeWithChildren = (nodeId, depth = 0) => {
                    const distributionNode = this.nodes.get(nodeId);
                    if (!distributionNode) return '';
                    
                    // Determine node type based on parameters
                    const nodeType = getNodeType(distributionNode);
                    
                    // Calculate indentation based on hierarchy level
                    // Level 1 (depth 0): Manufacturer - no indentation
                    // Level 2 (depth 1): Hubs - indented
                    // Level 3 (depth 2+): Stores - more indented
                    const indent = depth * 1.5;
                    
                    // Choose icon based on node type
                    let icon = '🚚'; // Default
                    if (nodeType === 'manufacturer') icon = '🏭';
                    else if (nodeType === 'hub') icon = '📦';
                    else if (nodeType === 'store') icon = '🏪';
                    
                    let html = `<div class="epr-group-item epr-group-distribution-item" data-item-id="${nodeId}" style="font-size: 0.7rem; padding: 0.25rem 0.5rem; margin: 0.25rem 0; background: white; border-radius: 3px; border-left: 3px solid #6f42c1; cursor: pointer; padding-left: ${0.5 + indent}rem;">
                        ${icon} ${this.escapeHtml(distributionNode.name)}
                    </div>`;
                    
                    // Find children using groupId (nodes that have this node as their groupId)
                    const children = findChildrenByGroupId(nodeId);
                    
                    // Sort children: hubs first (if current is manufacturer), then stores
                    const sortedChildren = children.sort((a, b) => {
                        const nodeA = this.nodes.get(a);
                        const nodeB = this.nodes.get(b);
                        const typeA = getNodeType(nodeA);
                        const typeB = getNodeType(nodeB);
                        
                        // If current node is manufacturer, show hubs before stores
                        if (nodeType === 'manufacturer') {
                            if (typeA === 'hub' && typeB === 'store') return -1;
                            if (typeA === 'store' && typeB === 'hub') return 1;
                        }
                        
                        // Otherwise sort by name
                        return (nodeA?.name || '').localeCompare(nodeB?.name || '');
                    });
                    
                    // If this is a hub, group stores together and render as comma-separated list
                    if (nodeType === 'hub') {
                        const storeChildren = sortedChildren.filter(childId => {
                            const childNode = this.nodes.get(childId);
                            return childNode && getNodeType(childNode) === 'store';
                        });
                        
                        if (storeChildren.length > 0) {
                            const storeNames = storeChildren.map(childId => {
                                const childNode = this.nodes.get(childId);
                                return childNode ? this.escapeHtml(childNode.name) : '';
                            }).filter(name => name);
                            
                            const formattedStores = formatStoreList(storeNames);
                            if (formattedStores) {
                                html += `<div class="epr-group-item epr-group-distribution-item" style="font-size: 0.65rem; padding: 0.25rem 0.5rem; margin: 0.25rem 0; background: #f8f9fa; border-radius: 3px; border-left: 3px solid #28a745; padding-left: ${0.5 + indent + 1.5}rem; max-width: 50ch; word-wrap: break-word; overflow-wrap: break-word; line-height: 1.4;">
                                    🏪 ${formattedStores}
                                </div>`;
                            }
                        }
                        
                        // Render non-store children (shouldn't be any, but just in case)
                        sortedChildren.filter(childId => {
                            const childNode = this.nodes.get(childId);
                            return childNode && getNodeType(childNode) !== 'store';
                        }).forEach(childId => {
                            html += renderNodeWithChildren(childId, depth + 1);
                        });
                    } else {
                        // For manufacturer or other types, render children recursively
                        sortedChildren.forEach(childId => {
                            html += renderNodeWithChildren(childId, depth + 1);
                        });
                    }
                    
                    return html;
                };
                
                // Build list of contained distribution items with hierarchy
                let itemsList = '';
                if (containedItems.length > 0) {
                    // Use scrollable container with max height to prevent overflow
                    itemsList = '<div class="epr-group-items-list" style="margin-top: 0.5rem; padding-top: 0.5rem; border-top: 1px solid #dee2e6; max-height: 500px; overflow-y: auto; overflow-x: hidden; scrollbar-width: thin;">';
                    
                    // Render root nodes and their children (using groupId hierarchy)
                    rootNodes.forEach(rootId => {
                        itemsList += renderNodeWithChildren(rootId, 0);
                    });
                    
                    itemsList += '</div>';
                    
                    // Debug: Count total nodes in hierarchy
                    let totalInHierarchy = 0;
                    const countInHierarchy = (nodeId) => {
                        totalInHierarchy++;
                        const children = findChildrenByGroupId(nodeId);
                        children.forEach(childId => countInHierarchy(childId));
                    };
                    rootNodes.forEach(rootId => countInHierarchy(rootId));
                    console.log(`[EPR Canvas] Distribution group hierarchy: ${rootNodes.length} root(s), ${totalInHierarchy} total nodes in hierarchy, ${distributionItems.length} in containedItems`);
                }
                
                // Calculate the size needed to contain all nodes
                const childNodes = [];
                rootNodes.forEach(rootId => {
                    const findChildren = (nodeId) => {
                        const children = findChildrenByGroupId(nodeId);
                        children.forEach(childId => {
                            childNodes.push(childId);
                            findChildren(childId); // Recursively find nested children
                        });
                    };
                    findChildren(rootId);
                });
                
                // Calculate minimum size for the group node - use narrower width for better display
                // Width should accommodate 50 characters with some padding
                const minWidth = 500; // Narrower width for better display
                const minHeight = 400; // Minimum height
                
                // Use stored size if available, otherwise calculate
                const groupWidth = node.width || node.minWidth || minWidth;
                const groupHeight = node.height || node.minHeight || minHeight;
                
                groupContent = `<div class="epr-group-content" style="font-size: 0.75rem; color: #495057; margin-top: 0.5rem; width: ${groupWidth}px; min-height: ${groupHeight}px;">
                    <div style="font-weight: 600; margin-bottom: 0.25rem; padding: 0.5rem; background: #f8f9fa; border-radius: 4px;">
                        <div>Distribution Nodes: ${distributionItems.length}</div>
                    </div>
                    ${itemsList}
                </div>`;
                
                // Set node size - use fixed width, but allow height to grow with scrollable list
                const finalWidth = node.width || node.minWidth || minWidth;
                // Use auto height but limit max height to prevent overflow
                const maxHeight = 600;
                
                nodeEl.style.minWidth = finalWidth + 'px';
                nodeEl.style.width = finalWidth + 'px';
                nodeEl.style.height = 'auto';
                nodeEl.style.maxHeight = maxHeight + 'px';
                nodeEl.style.overflowY = 'auto'; // Allow scrolling if content is too tall
                nodeEl.style.overflowX = 'hidden';
                nodeEl.style.position = 'relative'; // Make it a container
                
                // Store size in node for later use
                node.minWidth = finalWidth;
                node.width = finalWidth;
                node.height = 'auto';
                node.maxHeight = maxHeight;
            }
            
            // Special rendering for note nodes
            if (node.type === 'note') {
                const headline = node.parameters?.headline || 'Note';
                const bodyText = node.parameters?.bodyText || '';
                nodeEl.innerHTML = `
                    <div class="epr-node-header">
                        <div style="display: flex; align-items: center;">
                            <i class="bi bi-sticky-fill epr-node-icon"></i>
                            <span class="epr-node-title" style="font-weight: bold;">${this.escapeHtml(headline)}</span>
                        </div>
                        <button class="epr-node-delete" onclick="window.EPRVisualEditor.canvasManager.deleteNode('${node.id}')" title="Delete">
                            <i class="bi bi-x-circle"></i>
                        </button>
                    </div>
                    <div class="epr-node-type">${this.getTypeLabel(node.type)}</div>
                    ${bodyText ? `<div class="epr-note-body" style="padding: 0.5rem; font-size: 0.85rem; color: #333; margin-top: 0.25rem; max-width: 50ch; white-space: pre-wrap; word-wrap: break-word; line-height: 1.4;">${this.escapeHtml(bodyText).replace(/\n/g, '<br>')}</div>` : ''}
                `;
            } else {
                // For raw materials, show classification DisplayName under the name if selected
                let classificationDisplay = '';
                if (node.type === 'raw-material' && node.parameters) {
                    // Show Level 3 DisplayName if available, otherwise Level 2, otherwise Level 1
                    const displayName = node.parameters.level3DisplayName || 
                                      node.parameters.level2DisplayName || 
                                      node.parameters.level1DisplayName || 
                                      null;
                    if (displayName && displayName !== node.name) {
                        classificationDisplay = `<div class="epr-node-classification" style="font-size: 0.75rem; color: #6c757d; margin-top: 0.25rem; font-style: italic;">${this.escapeHtml(displayName)}</div>`;
                    }
                }
                
                nodeEl.innerHTML = `
                    <div class="epr-node-header" style="position: relative;">
                        ${packagingHierarchyPill}
                        <div style="display: flex; align-items: center;">
                            <i class="bi ${node.icon} epr-node-icon"></i>
                            <div style="display: flex; flex-direction: column; flex: 1;">
                            <span class="epr-node-title">${this.escapeHtml(node.name)}</span>
                                ${classificationDisplay}
                            </div>
                        </div>
                        <div style="display: flex; gap: 0.25rem;">
                            <button class="epr-node-lock" onclick="window.EPRVisualEditor.canvasManager.toggleNodeLock('${node.id}')" title="${node.locked ? 'Unlock' : 'Lock'}">
                                <i class="bi ${node.locked ? 'bi-lock-fill' : 'bi-unlock'}"></i>
                            </button>
                            <button class="epr-node-delete" onclick="window.EPRVisualEditor.canvasManager.deleteNode('${node.id}')" title="Delete">
                                <i class="bi bi-x-circle"></i>
                            </button>
                        </div>
                    </div>
                    <div class="epr-node-type">${this.getTypeLabel(node.type)}</div>
                    ${node.parameters?.description ? `<div class="epr-node-description" style="padding: 0.25rem 0.5rem; font-size: 0.75rem; color: #666; border-top: 1px solid #eee; margin-top: 0.25rem;"><span class="epr-description-text" style="display: block; max-width: 30ch; word-wrap: break-word; overflow-wrap: break-word; white-space: pre-wrap;">${this.escapeHtml(node.parameters.description).replace(/\n/g, '<br>')}</span></div>` : ''}
                    ${node.parameters?.notes ? `<div class="epr-node-notes" style="padding: 0.25rem 0.5rem; font-size: 0.75rem; color: #666; border-top: 1px solid #eee; margin-top: 0.25rem;"><i class="bi bi-sticky"></i> <span class="epr-notes-text" style="display: block; max-width: 30ch; word-wrap: break-word; overflow-wrap: break-word; white-space: pre-wrap;">${this.escapeHtml(node.parameters.notes).replace(/\n/g, '<br>')}</span></div>` : ''}
                    ${transportInfo}
                    ${groupContent}
                    ${leftPort}
                    ${rightPort}
                `;
            }
            
            // CRITICAL: For transport nodes, check if they should be hidden BEFORE making draggable
            // This ensures transport nodes between two nodes are ALWAYS hidden immediately
            if (node.type === 'transport') {
                const incomingConn = this.connections.find(c => c.to === node.id);
                const outgoingConn = this.connections.find(c => c.from === node.id);
                
                if (incomingConn && outgoingConn) {
                    // Transport node is between two nodes - HIDE IT IMMEDIATELY
                    nodeEl.style.display = 'none';
                    nodeEl.style.setProperty('display', 'none', 'important');
                    nodeEl.style.visibility = 'hidden';
                    nodeEl.style.setProperty('visibility', 'hidden', 'important');
                    nodeEl.setAttribute('data-transport-on-connection', 'true');
                    nodeEl.classList.add('epr-transport-hidden');
                    // Don't make it draggable if it's hidden
                    // The node will be displayed as a small box on the connection line instead
                }
            }
            
            this.makeNodeDraggable(nodeEl, node);
            
            nodeEl.addEventListener('click', (e) => {
                if (!e.target.closest('.epr-node-delete') && !e.target.closest('.epr-connection-port')) {
                    // Handle multi-select with Ctrl/Cmd key
                    if (e.ctrlKey || e.metaKey) {
                        e.preventDefault();
                        e.stopPropagation();
                        // Toggle selection
                        if (this.selectedNodes.has(node.id)) {
                            this.selectedNodes.delete(node.id);
                            nodeEl.classList.remove('selected');
                            if (this.selectedNodes.size === 0) {
                                this.selectedNode = null;
                            }
                        } else {
                            this.selectedNodes.add(node.id);
                            nodeEl.classList.add('selected');
                            this.selectedNode = null; // Clear single selection when multi-selecting
                        }
                        // Update selection display
                        this.updateNodeSelection();
                        console.log('[EPR Canvas] Multi-select:', {
                            nodeId: node.id,
                            selectedNodes: Array.from(this.selectedNodes),
                            size: this.selectedNodes.size
                        });
                    } else {
                        // Single select - clear multi-select
                        if (this.selectedNodes.size > 0) {
                            this.selectedNodes.forEach(nodeId => {
                                const selectedEl = document.querySelector(`[data-node-id="${nodeId}"]`);
                                if (selectedEl) {
                                    selectedEl.classList.remove('selected');
                                }
                            });
                            this.selectedNodes.clear();
                        }
                        this.selectNode(node.id);
                    }
                }
            });
            
            // Right-click context menu for nodes
            nodeEl.addEventListener('contextmenu', (e) => {
                e.preventDefault();
                e.stopPropagation();
                this.showNodeContextMenu(e, node);
            });
            
            // Setup connection ports for dragging
            const leftPortEl = nodeEl.querySelector('.epr-port-left');
            const rightPortEl = nodeEl.querySelector('.epr-port-right');
            
            if (leftPortEl && window.EPRVisualEditor && window.EPRVisualEditor.setupConnectionPort) {
                window.EPRVisualEditor.setupConnectionPort(leftPortEl, node.id, 'left');
            }
            if (rightPortEl && window.EPRVisualEditor && window.EPRVisualEditor.setupConnectionPort) {
                window.EPRVisualEditor.setupConnectionPort(rightPortEl, node.id, 'right');
            }
            
            this.nodesLayer.appendChild(nodeEl);
            
            // CRITICAL: For transport nodes, ALWAYS check and hide if between two nodes
            // This must happen AFTER appending to DOM but BEFORE updateConnections
            if (node.type === 'transport') {
                // Use setTimeout to ensure connections are set up
                setTimeout(() => {
                    const incomingConn = this.connections.find(c => c.to === node.id);
                    const outgoingConn = this.connections.find(c => c.from === node.id);
                    
                    const nodeElCheck = document.querySelector(`[data-node-id="${node.id}"]`);
                    if (!nodeElCheck) return;
                    
                    if (incomingConn && outgoingConn) {
                        // Transport node is between two nodes - HIDE IT IMMEDIATELY with ALL methods
                        nodeElCheck.style.display = 'none';
                        nodeElCheck.style.setProperty('display', 'none', 'important');
                        nodeElCheck.style.visibility = 'hidden';
                        nodeElCheck.style.setProperty('visibility', 'hidden', 'important');
                        nodeElCheck.style.opacity = '0';
                        nodeElCheck.style.setProperty('opacity', '0', 'important');
                        nodeElCheck.style.height = '0';
                        nodeElCheck.style.setProperty('height', '0', 'important');
                        nodeElCheck.style.width = '0';
                        nodeElCheck.style.setProperty('width', '0', 'important');
                        nodeElCheck.style.position = 'absolute';
                        nodeElCheck.style.setProperty('position', 'absolute', 'important');
                        nodeElCheck.style.left = '-9999px';
                        nodeElCheck.style.setProperty('left', '-9999px', 'important');
                        nodeElCheck.style.pointerEvents = 'none';
                        nodeElCheck.style.setProperty('pointer-events', 'none', 'important');
                        nodeElCheck.setAttribute('data-transport-on-connection', 'true');
                        nodeElCheck.classList.add('epr-transport-hidden');
                        
                        // Force update connections to render transport box
                        this.updateConnections();
                    } else {
                        // Transport node is NOT between two nodes - this should not happen!
                        // Hide it anyway and show warning
                        console.warn('[EPR Transport] Transport node created without connections - hiding it');
                        nodeElCheck.style.display = 'none';
                        nodeElCheck.style.setProperty('display', 'none', 'important');
                        nodeElCheck.setAttribute('data-transport-on-connection', 'true');
                        nodeElCheck.classList.add('epr-transport-hidden');
                    }
                }, 0);
            }
            
            // If this is a packaging group or distribution group, make items inside draggable
            if (node.type === 'packaging-group' || node.type === 'distribution-group') {
                setTimeout(() => {
                    this.setupGroupItemDragHandlers(nodeEl, node);
                }, 0);
            }
            
            this.updateConnections();
            
            // Update Packaging Group hierarchy pills when connections change
            if (node.type === 'packaging-group') {
                this.updatePackagingGroupPills();
            }
            
            console.log('[EPR Canvas] Node rendered:', node.id);
        }
        
        /**
         * Update hierarchy pills for all Packaging Group nodes
         */
        updatePackagingGroupPills() {
            this.nodes.forEach((node, nodeId) => {
                if (node.type === 'packaging-group') {
                    const nodeEl = document.querySelector(`[data-node-id="${nodeId}"]`);
                    if (nodeEl) {
                        const hierarchyLevel = this.getPackagingGroupHierarchyLevel(nodeId);
                        let pillEl = nodeEl.querySelector('.epr-packaging-hierarchy-pill');
                        
                        if (hierarchyLevel) {
                            const pillColors = {
                                'Primary': { bg: '#007bff', text: '#fff' },
                                'Secondary': { bg: '#28a745', text: '#fff' },
                                'Tertiary': { bg: '#ffc107', text: '#000' },
                                'Quaternary': { bg: '#6c757d', text: '#fff' }
                            };
                            const color = pillColors[hierarchyLevel] || pillColors['Quaternary'];
                            
                            // Set width based on hierarchy level text (matching "Secondary" = 61px pattern)
                            const widthMap = {
                                'Primary': '50px',
                                'Secondary': '61px',
                                'Tertiary': '55px',
                                'Quaternary': '70px'
                            };
                            const pillWidth = widthMap[hierarchyLevel] || '61px';
                            
                            if (pillEl) {
                                // Update existing pill
                                pillEl.textContent = hierarchyLevel;
                                pillEl.style.background = color.bg;
                                pillEl.style.color = color.text;
                                pillEl.style.top = '-24px';
                                pillEl.style.left = '0px';
                                pillEl.style.right = 'auto';
                                pillEl.style.width = pillWidth;
                                pillEl.style.borderRadius = '5px';
                                pillEl.style.fontSize = '6pt';
                                pillEl.style.boxShadow = 'none';
                            } else {
                                // Create new pill
                                pillEl = document.createElement('span');
                                pillEl.className = 'epr-packaging-hierarchy-pill';
                                pillEl.style.cssText = `position: absolute; top: -24px; left: 0px; width: ${pillWidth}; background: ${color.bg}; color: ${color.text}; padding: 2px 8px; border-radius: 5px; font-size: 6pt; font-weight: 600; white-space: nowrap; border: 2px solid white; z-index: 100;`;
                                pillEl.textContent = hierarchyLevel;
                                
                                const headerEl = nodeEl.querySelector('.epr-node-header');
                                if (headerEl) {
                                    headerEl.style.position = 'relative';
                                    headerEl.appendChild(pillEl);
                                }
                            }
                        } else {
                            // Remove pill if no hierarchy level
                            if (pillEl) {
                                pillEl.remove();
                            }
                        }
                    }
                }
            });
        }
        
        setupGroupItemDragHandlers(groupNodeEl, groupNode) {
            // Make items in the group list draggable
            const itemElements = groupNodeEl.querySelectorAll('.epr-group-item[data-item-id]');
            itemElements.forEach(itemEl => {
                const itemId = itemEl.getAttribute('data-item-id');
                const itemNode = this.nodes.get(itemId);
                
                if (!itemNode) return;
                
                // Remove existing handlers to avoid duplicates
                const newItemEl = itemEl.cloneNode(true);
                itemEl.parentNode.replaceChild(newItemEl, itemEl);
                
                // Make item draggable
                newItemEl.style.cursor = 'grab';
                newItemEl.setAttribute('draggable', 'true');
                
                newItemEl.addEventListener('dragstart', (e) => {
                    e.stopPropagation();
                    e.dataTransfer.setData('text/plain', itemId);
                    e.dataTransfer.effectAllowed = 'move';
                    newItemEl.style.opacity = '0.5';
                });
                
                newItemEl.addEventListener('dragend', () => {
                    newItemEl.style.opacity = '1';
                });
            });
        }
        
        makeNodeDraggable(nodeEl, node) {
            let isDragging = false;
            let startX = 0;
            let startY = 0;
            let startMouseX = 0;
            let startMouseY = 0;
            let hasMoved = false;
            let dragOffset = {}; // Store initial positions for multi-select drag
            
            nodeEl.addEventListener('mousedown', (e) => {
                if (e.target.closest('.epr-node-delete') || e.target.closest('.epr-node-lock') || e.target.closest('.epr-connection-port')) return;
                
                // Check if this node is inside a locked group
                if (node.groupId) {
                    const groupNode = this.nodes.get(node.groupId);
                    if (groupNode && groupNode.locked) {
                        // Prevent dragging items out of locked groups
                        return;
                    }
                }
                
                // Allow dragging locked nodes themselves, but prevent dragging their contents
                // (The node itself can move, but items inside cannot be dragged out)
                
                isDragging = false;
                hasMoved = false;
                
                // Check if this node is part of multi-select
                const isMultiSelect = this.selectedNodes.has(node.id);
                
                // Store initial positions for all selected nodes if multi-selecting
                if (isMultiSelect && this.selectedNodes.size > 1) {
                    this.selectedNodes.forEach(nodeId => {
                        const selectedNode = this.nodes.get(nodeId);
                        if (selectedNode) {
                            dragOffset[nodeId] = { x: selectedNode.x, y: selectedNode.y };
                        }
                    });
                } else {
                    dragOffset[node.id] = { x: node.x, y: node.y };
                }
                
                const rect = nodeEl.getBoundingClientRect();
                const canvasRect = this.canvas.getBoundingClientRect();
                startX = node.x;
                startY = node.y;
                startMouseX = e.clientX;
                startMouseY = e.clientY;
                
                document.addEventListener('mousemove', handleMouseMove);
                document.addEventListener('mouseup', handleMouseUp);
            });
            
            const handleMouseMove = (e) => {
                if (!hasMoved) {
                    // Check if mouse has moved enough to start dragging
                    const moveThreshold = 5; // pixels
                    const deltaX = Math.abs(e.clientX - startMouseX);
                    const deltaY = Math.abs(e.clientY - startMouseY);
                    
                    if (deltaX > moveThreshold || deltaY > moveThreshold) {
                        isDragging = true;
                        hasMoved = true;
                        nodeEl.classList.add('dragging');
                        
                        // Add dragging class to all selected nodes
                        if (this.selectedNodes.size > 1) {
                            this.selectedNodes.forEach(nodeId => {
                                const selectedEl = document.querySelector(`[data-node-id="${nodeId}"]`);
                                if (selectedEl) {
                                    selectedEl.classList.add('dragging');
                                }
                            });
                        }
                    } else {
                        return;
                    }
                }
                
                if (!isDragging) return;
                
                const canvasRect = this.canvas.getBoundingClientRect();
                const deltaX = e.clientX - startMouseX;
                const deltaY = e.clientY - startMouseY;
                
                // Check if this node is part of multi-select
                const isMultiSelect = this.selectedNodes.has(node.id) && this.selectedNodes.size > 1;
                
                if (isMultiSelect) {
                    // Move all selected nodes together
                    this.selectedNodes.forEach(nodeId => {
                        const selectedNode = this.nodes.get(nodeId);
                        if (!selectedNode || !dragOffset[nodeId]) return;
                        
                        let newX = dragOffset[nodeId].x + deltaX;
                        let newY = dragOffset[nodeId].y + deltaY;
                        
                        if (this.snapToGrid) {
                            newX = Math.round(newX / this.gridSize) * this.gridSize;
                            newY = Math.round(newY / this.gridSize) * this.gridSize;
                            if (newX < 0) newX = 0;
                            if (newY < 0) newY = 0;
                        }
                        
                        selectedNode.x = newX;
                        selectedNode.y = newY;
                        const selectedEl = document.querySelector(`[data-node-id="${nodeId}"]`);
                        if (selectedEl) {
                            selectedEl.style.left = `${newX}px`;
                            selectedEl.style.top = `${newY}px`;
                        }
                    });
                } else {
                    // Move single node
                    let newX = startX + deltaX;
                    let newY = startY + deltaY;
                    
                    if (this.snapToGrid) {
                        // Snap to grid lines - ensure nodes align to grid intersections
                        newX = Math.round(newX / this.gridSize) * this.gridSize;
                        newY = Math.round(newY / this.gridSize) * this.gridSize;
                        
                        // Ensure minimum position is at grid line
                        if (newX < 0) newX = 0;
                        if (newY < 0) newY = 0;
                    }
                    
                    node.x = newX;
                    node.y = newY;
                    nodeEl.style.left = `${newX}px`;
                    nodeEl.style.top = `${newY}px`;
                }
                
                // Check if dragging over a packaging group or distribution group
                const elementBelow = document.elementFromPoint(e.clientX, e.clientY);
                const packagingGroupEl = elementBelow?.closest('.epr-packaging-group-node');
                const distributionGroupEl = elementBelow?.closest('.epr-distribution-group-node');
                const groupNodeEl = packagingGroupEl || distributionGroupEl;
                
                if (groupNodeEl) {
                    groupNodeEl.classList.add('drag-over');
                } else {
                    // Remove drag-over class from all groups
                    document.querySelectorAll('.epr-packaging-group-node, .epr-distribution-group-node').forEach(el => {
                        el.classList.remove('drag-over');
                    });
                }
                
                this.updateConnections();
            };
            
            const handleMouseUp = (e) => {
                // Remove drag-over class from all groups
                document.querySelectorAll('.epr-packaging-group-node, .epr-distribution-group-node').forEach(el => {
                    el.classList.remove('drag-over');
                });
                
                // Remove dragging class from all selected nodes
                if (this.selectedNodes.size > 1) {
                    this.selectedNodes.forEach(nodeId => {
                        const selectedEl = document.querySelector(`[data-node-id="${nodeId}"]`);
                        if (selectedEl) {
                            selectedEl.classList.remove('dragging');
                        }
                    });
                }
                
                if (isDragging && hasMoved) {
                    const mouseX = e.clientX;
                    const mouseY = e.clientY;
                    let droppedOnGroup = false;
                    
                    // Check if dropped on a packaging group (for packaging/raw-material nodes)
                    if (node.type === 'packaging' || node.type === 'raw-material') {
                        // Check all packaging groups to see if mouse is over any of them
                        this.nodes.forEach((groupNode, groupNodeId) => {
                            if (groupNode.type === 'packaging-group') {
                                const groupNodeEl = document.querySelector(`[data-node-id="${groupNodeId}"]`);
                                if (groupNodeEl) {
                                    const groupRect = groupNodeEl.getBoundingClientRect();
                                    
                                    // Check if mouse is within group bounds
                                    if (mouseX >= groupRect.left && mouseX <= groupRect.right &&
                                        mouseY >= groupRect.top && mouseY <= groupRect.bottom) {
                                        // Dropped on this group
                                        this.moveItemToGroup(node.id, groupNodeId);
                                        droppedOnGroup = true;
                                        return;
                                    }
                                }
                            }
                        });
                    }
                    
                    // Check if dropped on a distribution group (for distribution nodes)
                    if (node.type === 'distribution') {
                        // Check all distribution groups to see if mouse is over any of them
                        this.nodes.forEach((groupNode, groupNodeId) => {
                            if (groupNode.type === 'distribution-group' && !groupNode.locked) {
                                const groupNodeEl = document.querySelector(`[data-node-id="${groupNodeId}"]`);
                                if (groupNodeEl) {
                                    const groupRect = groupNodeEl.getBoundingClientRect();
                                    
                                    // Check if mouse is within group bounds
                                    if (mouseX >= groupRect.left && mouseX <= groupRect.right &&
                                        mouseY >= groupRect.top && mouseY <= groupRect.bottom) {
                                        // Dropped on this group
                                        this.moveItemToGroup(node.id, groupNodeId);
                                        droppedOnGroup = true;
                                        return;
                                    }
                                }
                            }
                        });
                    }
                    
                    if (droppedOnGroup) {
                        // Item was moved to group, don't update position
                        isDragging = false;
                        hasMoved = false;
                        nodeEl.classList.remove('dragging');
                        this.saveState();
                        document.removeEventListener('mousemove', handleMouseMove);
                        document.removeEventListener('mouseup', handleMouseUp);
                        return;
                    }
                    
                    // If not dropped on a group, check if item was dragged out of a group
                    if (node.groupId) {
                        const groupNode = this.nodes.get(node.groupId);
                        // Prevent removing from locked groups
                        if (groupNode && groupNode.locked) {
                            // Don't allow removal from locked group
                            isDragging = false;
                            hasMoved = false;
                            nodeEl.classList.remove('dragging');
                            this.saveState();
                            document.removeEventListener('mousemove', handleMouseMove);
                            document.removeEventListener('mouseup', handleMouseUp);
                            return;
                        }
                        
                        const currentGroupEl = document.querySelector(`[data-node-id="${node.groupId}"]`);
                        if (currentGroupEl) {
                            const groupRect = currentGroupEl.getBoundingClientRect();
                            
                            if (!(mouseX >= groupRect.left && mouseX <= groupRect.right &&
                                  mouseY >= groupRect.top && mouseY <= groupRect.bottom)) {
                                // Dragged out of group - remove from group and show on canvas
                                this.removeItemFromGroup(node.id);
                                
                                // Position the item near where it was dropped
                                const canvasRect = this.canvas.getBoundingClientRect();
                                const canvasX = mouseX - canvasRect.left - startX;
                                const canvasY = mouseY - canvasRect.top - startY;
                                
                                // Update node position
                                node.x = Math.max(0, canvasX);
                                node.y = Math.max(0, canvasY);
                                nodeEl.style.left = `${node.x}px`;
                                nodeEl.style.top = `${node.y}px`;
                                
                                // If it's a packaging item, also show connected raw materials
                                if (node.type === 'packaging') {
                                    this.connections.forEach(conn => {
                                        if (conn.to === node.id) {
                                            const rawMaterialNode = this.nodes.get(conn.from);
                                            if (rawMaterialNode && rawMaterialNode.type === 'raw-material') {
                                                // Position raw material near the packaging item
                                                rawMaterialNode.x = node.x - 150;
                                                rawMaterialNode.y = node.y;
                                                const rawMaterialEl = document.querySelector(`[data-node-id="${rawMaterialNode.id}"]`);
                                                if (rawMaterialEl) {
                                                    rawMaterialEl.style.display = '';
                                                    rawMaterialEl.style.left = `${rawMaterialNode.x}px`;
                                                    rawMaterialEl.style.top = `${rawMaterialNode.y}px`;
                                                    rawMaterialEl.removeAttribute('data-in-group');
                                                }
                                            }
                                        }
                                    });
                                }
                            }
                        }
                    }
                }
                
                if (isDragging || hasMoved) {
                    isDragging = false;
                    hasMoved = false;
                    nodeEl.classList.remove('dragging');
                    this.saveState();
                }
                document.removeEventListener('mousemove', handleMouseMove);
                document.removeEventListener('mouseup', handleMouseUp);
            };
        }
        
        deleteNode(nodeId) {
            const node = this.nodes.get(nodeId);
            if (!node) return;
            
            // Remove all connections involving this node FIRST
            // This ensures no orphaned connections remain
            const connectionsToRemove = this.connections.filter(c => 
                c.from === nodeId || c.to === nodeId
            );
            
            // If this is a transport node, preserve connections between the nodes it connects
            if (node && node.type === 'transport') {
                    // Find connections going into and out of this transport node
                    const incomingConn = this.connections.find(c => c.to === nodeId);
                    const outgoingConn = this.connections.find(c => c.from === nodeId);
                    
                    if (incomingConn && outgoingConn) {
                        // There's a path: fromNode -> transport -> toNode
                        // Reconnect fromNode directly to toNode
                        const fromNodeId = incomingConn.from;
                        const toNodeId = outgoingConn.to;
                        const fromPort = incomingConn.fromPort || 'right';
                        const toPort = outgoingConn.toPort || 'left';
                        
                        // Remove connections involving transport
                        this.connections = this.connections.filter(c => 
                            !(c.from === nodeId || c.to === nodeId)
                        );
                        
                        // Add direct connection
                        this.addConnection(fromNodeId, toNodeId, fromPort, toPort);
                    } else {
                        // Just remove connections involving transport
                        this.connections = this.connections.filter(c => 
                            !(c.from === nodeId || c.to === nodeId)
                        );
                    }
            } else {
                // For non-transport nodes, remove all connections
                this.connections = this.connections.filter(c => 
                    c.from !== nodeId && c.to !== nodeId
                );
            }
            
            // Clean up connection quantities that reference this node
            this.nodes.forEach((n) => {
                if (n.parameters && n.parameters.connectionQuantities) {
                    Object.keys(n.parameters.connectionQuantities).forEach(key => {
                        const [fromId, toId] = key.split('-');
                        if (fromId === nodeId || toId === nodeId) {
                            delete n.parameters.connectionQuantities[key];
                        }
                    });
                }
            });
            
            // If this is a packaging-group, remove items from group
            if (node && node.type === 'packaging-group' && node.containedItems) {
                node.containedItems.forEach(itemId => {
                    const itemNode = this.nodes.get(itemId);
                    if (itemNode) {
                        itemNode.groupId = undefined;
                    }
                });
            }
            
            // If this is a distribution-group, remove items from group
            if (node && node.type === 'distribution-group' && node.containedItems) {
                node.containedItems.forEach(itemId => {
                    const itemNode = this.nodes.get(itemId);
                    if (itemNode) {
                        itemNode.groupId = undefined;
                    }
                });
            }
            
            // If this node is in a group, remove it from the group
            if (node && node.groupId) {
                const groupNode = this.nodes.get(node.groupId);
                if (groupNode && groupNode.containedItems) {
                    groupNode.containedItems = groupNode.containedItems.filter(id => id !== nodeId);
                    this.renderNode(groupNode);
                }
            }
            
            this.nodes.delete(nodeId);
            
            const nodeEl = document.querySelector(`[data-node-id="${nodeId}"]`);
            if (nodeEl) {
                nodeEl.remove();
            }
            
            if (this.selectedNode === nodeId) {
                this.deselectNode();
            }
            
            // Force update connections to remove all orphaned lines, transport boxes, and quantity circles
            // Clear ALL connection-related elements from the SVG layer
            const connectionsLayer = this.connectionsLayer;
            if (connectionsLayer) {
                // Remove all child elements (lines, circles, text, foreignObjects, transport boxes, quantity circles, etc.)
                // This includes: .epr-connection-line, .epr-connection-quantity-circle, .epr-transport-box, etc.
                while (connectionsLayer.firstChild) {
                    connectionsLayer.removeChild(connectionsLayer.firstChild);
                }
            }
            
            // Also remove any transport nodes that were only connected to the deleted node
            this.nodes.forEach((n, nId) => {
                if (n.type === 'transport') {
                    // Check if transport node has any connections left
                    const hasConnections = this.connections.some(c => c.from === nId || c.to === nId);
                    if (!hasConnections) {
                        // Remove orphaned transport node
                        this.nodes.delete(nId);
                        const transportEl = document.querySelector(`[data-node-id="${nId}"]`);
                        if (transportEl) {
                            transportEl.remove();
                        }
                    }
                }
            });
            
            // Clean up any orphaned connections (connections where nodes don't exist)
            this.connections = this.connections.filter(conn => {
                const fromNode = this.nodes.get(conn.from);
                const toNode = this.nodes.get(conn.to);
                return fromNode && toNode; // Only keep connections where both nodes exist
            });
            
            // Now rebuild connections (this will only show remaining valid connections)
            this.updateConnections();
            this.updatePlaceholder();
            this.saveState();
        }
        
        /**
         * Clean up orphaned connector lines - removes any connection lines where nodes don't exist
         */
        cleanupOrphanedConnections() {
            // Remove connections where nodes don't exist
            const validConnections = this.connections.filter(conn => {
                const fromNode = this.nodes.get(conn.from);
                const toNode = this.nodes.get(conn.to);
                return fromNode && toNode;
            });
            
            // Update connections array
            if (validConnections.length !== this.connections.length) {
                this.connections = validConnections;
                // Re-render connections to remove orphaned lines
                this.updateConnections();
            }
        }
        
        /**
         * Clone/duplicate a node and all nested items with their relationships
         */
        cloneNode(nodeId) {
            const node = this.nodes.get(nodeId);
            if (!node) return;
            
            // Create ID mapping for cloned nodes
            const idMap = new Map();
            const clonedNodes = [];
            
            // Create new node with offset position
            const offsetX = 50;
            const offsetY = 50;
            
            // Helper function to recursively clone a node and its nested items
            const cloneNodeRecursive = (originalNode, originalParentNode = null, clonedParentNode = null) => {
                const newId = `node-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
                idMap.set(originalNode.id, newId);
                
                // Calculate position: maintain relative positioning for nested items
                let newX, newY;
                if (originalParentNode && clonedParentNode) {
                    // Calculate relative position from original parent to original child
                    const relativeX = originalNode.x - originalParentNode.x;
                    const relativeY = originalNode.y - originalParentNode.y;
                    // Apply relative position to cloned parent to maintain visual indenting
                    newX = clonedParentNode.x + relativeX;
                    newY = clonedParentNode.y + relativeY;
                } else {
                    // Root node: just apply offset
                    newX = originalNode.x + offsetX;
                    newY = originalNode.y + offsetY;
                }
                
                const clonedNode = {
                    ...originalNode,
                    id: newId,
                    x: newX,
                    y: newY,
                    name: `${originalNode.name} (Copy)`,
                    parameters: originalNode.parameters ? JSON.parse(JSON.stringify(originalNode.parameters)) : {},
                    // Don't clone groupId - cloned nodes should be standalone
                    groupId: undefined,
                    containedItems: []
                };
                
                // If it's a packaging group, clone all contained items
                if (originalNode.type === 'packaging-group' && originalNode.containedItems) {
                    clonedNode.containedItems = [];
                    originalNode.containedItems.forEach(itemId => {
                        const itemNode = this.nodes.get(itemId);
                        if (itemNode) {
                            // Pass original parent and cloned parent to maintain relative positioning
                            const clonedItem = cloneNodeRecursive(itemNode, originalNode, clonedNode);
                            clonedNode.containedItems.push(clonedItem.id);
                            clonedNodes.push(clonedItem);
                        }
                    });
                }
                
                // If it's a distribution group, clone all contained items
                if (originalNode.type === 'distribution-group' && originalNode.containedItems) {
                    clonedNode.containedItems = [];
                    originalNode.containedItems.forEach(itemId => {
                        const itemNode = this.nodes.get(itemId);
                        if (itemNode) {
                            // Pass original parent and cloned parent to maintain relative positioning
                            const clonedItem = cloneNodeRecursive(itemNode, originalNode, clonedNode);
                            clonedNode.containedItems.push(clonedItem.id);
                            clonedNodes.push(clonedItem);
                        }
                    });
                }
                
                clonedNodes.push(clonedNode);
                return clonedNode;
            };
            
            // Clone the main node and all nested items
            const mainClonedNode = cloneNodeRecursive(node);
            
            // Add all cloned nodes to the canvas
            clonedNodes.forEach(clonedNode => {
                this.addNode(clonedNode);
            });
            
            // Clone ALL connections between cloned nodes (including nested items)
            // This includes connections between:
            // - Main node and nested items
            // - Nested items and other nested items
            // - Main node and other nodes (if both are cloned)
            const processedConnections = new Set();
            this.connections.forEach(conn => {
                // Clone if BOTH nodes were cloned (including nested items)
                if (idMap.has(conn.from) && idMap.has(conn.to)) {
                    const connectionKey = `${conn.from}-${conn.to}`;
                    if (!processedConnections.has(connectionKey)) {
                        const newFromId = idMap.get(conn.from);
                        const newToId = idMap.get(conn.to);
                        if (newFromId && newToId && this.nodes.has(newFromId) && this.nodes.has(newToId)) {
                            this.addConnection(newFromId, newToId, conn.fromPort, conn.toPort);
                            processedConnections.add(connectionKey);
                        }
                    }
                }
            });
            
            // Update connectionQuantities keys for cloned nodes
            clonedNodes.forEach(clonedNode => {
                const originalId = Array.from(idMap.entries()).find(([_, newId]) => newId === clonedNode.id)?.[0];
                if (!originalId || !clonedNode.parameters || !clonedNode.parameters.connectionQuantities) return;
                
                const updatedQuantities = {};
                Object.keys(clonedNode.parameters.connectionQuantities).forEach(key => {
                    const [fromId, toId] = key.split('-');
                    const newFromId = idMap.has(fromId) ? idMap.get(fromId) : fromId;
                    const newToId = idMap.has(toId) ? idMap.get(toId) : toId;
                    
                    // Only keep quantities for connections between cloned nodes
                    if (idMap.has(fromId) && idMap.has(toId)) {
                        const newKey = `${newFromId}-${newToId}`;
                        updatedQuantities[newKey] = clonedNode.parameters.connectionQuantities[key];
                    }
                });
                clonedNode.parameters.connectionQuantities = updatedQuantities;
                
                // Update the node in the map
                const nodeInMap = this.nodes.get(clonedNode.id);
                if (nodeInMap) {
                    nodeInMap.parameters = clonedNode.parameters;
                }
            });
            
            // Update connections display
            this.updateConnections();
            
            // Update Packaging Group hierarchy pills
            this.updatePackagingGroupPills();
            
            this.saveState();
            
            // Select the main cloned node
            this.selectNode(mainClonedNode.id);
            
            return mainClonedNode;
        }
        
        moveItemToGroup(itemNodeId, groupNodeId) {
            const itemNode = this.nodes.get(itemNodeId);
            const groupNode = this.nodes.get(groupNodeId);
            
            if (!itemNode || !groupNode) {
                return false;
            }
            
            // Prevent moving items into locked groups
            if (groupNode.locked) {
                alert('Cannot add items to a locked group. Unlock the group first.');
                return false;
            }
            
            // Handle packaging groups
            if (groupNode.type === 'packaging-group') {
                // Only allow packaging items and raw materials to be moved to packaging groups
                if (itemNode.type !== 'packaging' && itemNode.type !== 'raw-material') {
                    alert('Only packaging items and raw materials can be moved to packaging groups.');
                    return false;
                }
            }
            // Handle distribution groups
            else if (groupNode.type === 'distribution-group') {
                // Only allow distribution nodes to be moved to distribution groups
                if (itemNode.type !== 'distribution') {
                    alert('Only distribution nodes can be moved to distribution groups.');
                    return false;
                }
            } else {
                return false;
            }
            
            // Remove item from previous group if it was in one
            if (itemNode.groupId) {
                const oldGroup = this.nodes.get(itemNode.groupId);
                if (oldGroup && oldGroup.containedItems) {
                    oldGroup.containedItems = oldGroup.containedItems.filter(id => id !== itemNodeId);
                    this.renderNode(oldGroup);
                }
            }
            
            // Add item to new group
            if (!groupNode.containedItems) {
                groupNode.containedItems = [];
            }
            if (!groupNode.containedItems.includes(itemNodeId)) {
                groupNode.containedItems.push(itemNodeId);
            }
            itemNode.groupId = groupNodeId;
            
            // Don't remove connections - they'll be hidden automatically by updateConnections
            // Connections will be restored when items are dragged out of groups
            
            // If moving a packaging item, also move connected raw materials
            if (itemNode.type === 'packaging' && groupNode.type === 'packaging-group') {
                this.connections.forEach(conn => {
                    if (conn.to === itemNodeId) {
                        const rawMaterialNode = this.nodes.get(conn.from);
                        if (rawMaterialNode && rawMaterialNode.type === 'raw-material') {
                            // Move raw material to the same group
                            this.moveItemToGroup(rawMaterialNode.id, groupNodeId);
                        }
                    }
                });
            }
            
            // If moving a distribution node, also move all downstream connected nodes
            if (itemNode.type === 'distribution' && groupNode.type === 'distribution-group') {
                // Find all downstream nodes (nodes that this node connects to, and their children)
                const nodesToMove = new Set([itemNodeId]);
                const visited = new Set();
                
                const findDownstreamNodes = (nodeId) => {
                    if (visited.has(nodeId)) return;
                    visited.add(nodeId);
                    
                    // Find all nodes this node connects TO (outgoing connections)
                    this.connections.forEach(conn => {
                        if (conn.from === nodeId) {
                            const targetNode = this.nodes.get(conn.to);
                            if (targetNode && targetNode.type === 'distribution' && !targetNode.groupId) {
                                nodesToMove.add(conn.to);
                                // Recursively find downstream nodes
                                findDownstreamNodes(conn.to);
                            }
                        }
                    });
                    
                    // Also find nodes that connect TO this node (incoming connections)
                    // These should also be moved if they're not already in a group
                    this.connections.forEach(conn => {
                        if (conn.to === nodeId) {
                            const sourceNode = this.nodes.get(conn.from);
                            if (sourceNode && sourceNode.type === 'distribution' && !sourceNode.groupId) {
                                nodesToMove.add(conn.from);
                                // Recursively find downstream nodes from the source
                                findDownstreamNodes(conn.from);
                            }
                        }
                    });
                };
                
                // Start finding downstream nodes
                findDownstreamNodes(itemNodeId);
                
                // Move all found nodes to the group
                nodesToMove.forEach(nodeId => {
                    if (nodeId !== itemNodeId) {
                        const nodeToMove = this.nodes.get(nodeId);
                        if (nodeToMove && nodeToMove.groupId !== groupNodeId) {
                            // Remove from previous group if any
                            if (nodeToMove.groupId) {
                                const oldGroup = this.nodes.get(nodeToMove.groupId);
                                if (oldGroup && oldGroup.containedItems) {
                                    oldGroup.containedItems = oldGroup.containedItems.filter(id => id !== nodeId);
                                    this.renderNode(oldGroup);
                                }
                            }
                            
                            // Add to new group
                            if (!groupNode.containedItems.includes(nodeId)) {
                                groupNode.containedItems.push(nodeId);
                            }
                            nodeToMove.groupId = groupNodeId;
                            
                            // Hide nodes - they're shown in the group's list instead
                            const nodeEl = document.querySelector(`[data-node-id="${nodeId}"]`);
                            if (nodeEl) {
                                nodeEl.style.display = 'none';
                                nodeEl.setAttribute('data-in-group', 'true');
                            }
                        }
                    }
                });
            }
            
            // For distribution groups, keep nodes visible (they should be visible on canvas)
            // For packaging groups, hide nodes (they're only shown in the group list)
            const itemNodeEl = document.querySelector(`[data-node-id="${itemNodeId}"]`);
            if (itemNodeEl) {
                // Hide both distribution and packaging group items - they're shown in the group's list
                itemNodeEl.style.display = 'none';
                itemNodeEl.setAttribute('data-in-group', 'true');
            }
            
            // Update group display - this will show the expanded group with items
            this.renderNode(groupNode);
            
            // Update connections to hide connections involving items in groups
            this.updateConnections();
            
            // Ensure group has minimum height to show items
            const groupNodeEl = document.querySelector(`[data-node-id="${groupNodeId}"]`);
            if (groupNodeEl) {
                const itemCount = groupNode.containedItems ? groupNode.containedItems.length : 0;
                const minHeight = Math.max(120, 80 + (itemCount * 30)); // Base height + item height
                groupNodeEl.style.minHeight = `${minHeight}px`;
            }
            
            // Track that this was a move-to-group action
            this.lastActionType = 'moveToGroup';
            this.saveState();
            return true;
        }
        
        removeItemFromGroup(itemNodeId) {
            const itemNode = this.nodes.get(itemNodeId);
            if (!itemNode || !itemNode.groupId) {
                return false;
            }
            
            const groupNode = this.nodes.get(itemNode.groupId);
            if (groupNode && groupNode.containedItems) {
                // Store group ID for undo tracking
                const groupId = groupNode.id;
                
                // Find all children (downstream nodes) that should also be removed
                const nodesToRemove = new Set([itemNodeId]);
                const visited = new Set();
                
                const findChildren = (nodeId) => {
                    if (visited.has(nodeId)) return;
                    visited.add(nodeId);
                    
                    // Find all nodes this node connects TO (children)
                    this.connections.forEach(conn => {
                        if (conn.from === nodeId) {
                            const childNode = this.nodes.get(conn.to);
                            if (childNode && childNode.type === 'distribution' && childNode.groupId === groupNode.id) {
                                nodesToRemove.add(conn.to);
                                findChildren(conn.to);
                            }
                        }
                    });
                };
                
                // Find all children recursively (only for distribution nodes)
                if (itemNode.type === 'distribution' && groupNode.type === 'distribution-group') {
                    findChildren(itemNodeId);
                }
                
                // Remove all nodes (the item and its children)
                // Connections and metadata are preserved automatically since we're not deleting nodes
                nodesToRemove.forEach(nodeId => {
                    const nodeToRemove = this.nodes.get(nodeId);
                    if (nodeToRemove && nodeToRemove.groupId === groupNode.id) {
                        groupNode.containedItems = groupNode.containedItems.filter(id => id !== nodeId);
                        nodeToRemove.groupId = undefined;
                        
                        // Show the node again (it's now outside the group)
                        // Connections are preserved - they'll be visible again after updateConnections
                        const nodeEl = document.querySelector(`[data-node-id="${nodeId}"]`);
                        if (nodeEl) {
                            nodeEl.style.display = '';
                            nodeEl.removeAttribute('data-in-group');
                        }
                    }
                });
                
                // If removing a packaging item, also remove connected raw materials from group
                if (itemNode.type === 'packaging') {
                    this.connections.forEach(conn => {
                        if (conn.to === itemNodeId) {
                            const rawMaterialNode = this.nodes.get(conn.from);
                            if (rawMaterialNode && rawMaterialNode.type === 'raw-material' && rawMaterialNode.groupId === groupNode.id) {
                                this.removeItemFromGroup(rawMaterialNode.id);
                            }
                        }
                    });
                }
                
                // Track this as a removeFromGroup action for undo
                this.lastActionType = 'removeFromGroup';
                this.lastRemovedFromGroup = {
                    nodeIds: Array.from(nodesToRemove),
                    groupId: groupId
                };
                
                this.renderNode(groupNode);
                this.updateConnections(); // Update connections to show them again - connections are preserved
                this.saveState();
                return true;
            }
            
            return false;
        }
        
        /**
         * Show node context menu with Delete and Clone options
         */
        showNodeContextMenu(e, node) {
            // Remove existing context menus
            const existingMenus = document.querySelectorAll('.epr-node-context-menu, #eprGroupContextMenu');
            existingMenus.forEach(menu => menu.remove());
            
            // Create context menu
            const menu = document.createElement('div');
            menu.className = 'epr-node-context-menu';
            menu.style.position = 'fixed';
            menu.style.left = `${e.clientX}px`;
            menu.style.top = `${e.clientY}px`;
            menu.style.background = 'white';
            menu.style.border = '1px solid #dee2e6';
            menu.style.borderRadius = '4px';
            menu.style.boxShadow = '0 2px 8px rgba(0,0,0,0.15)';
            menu.style.zIndex = '10000';
            menu.style.padding = '0.25rem';
            menu.style.minWidth = '150px';
            
            // Delete option
            const deleteItem = document.createElement('div');
            deleteItem.style.padding = '0.5rem';
            deleteItem.style.cursor = 'pointer';
            deleteItem.style.borderRadius = '4px';
            deleteItem.style.color = '#dc3545';
            deleteItem.innerHTML = '<i class="bi bi-trash"></i> Delete';
            
            deleteItem.addEventListener('mouseenter', () => {
                deleteItem.style.background = '#f8f9fa';
            });
            deleteItem.addEventListener('mouseleave', () => {
                deleteItem.style.background = 'white';
            });
            
            deleteItem.addEventListener('click', () => {
                this.deleteNode(node.id);
                menu.remove();
            });
            
            menu.appendChild(deleteItem);
            
            // Clone option
            const cloneItem = document.createElement('div');
            cloneItem.style.padding = '0.5rem';
            cloneItem.style.cursor = 'pointer';
            cloneItem.style.borderRadius = '4px';
            cloneItem.style.color = '#007bff';
            cloneItem.innerHTML = '<i class="bi bi-files"></i> Clone';
            
            cloneItem.addEventListener('mouseenter', () => {
                cloneItem.style.background = '#f8f9fa';
            });
            cloneItem.addEventListener('mouseleave', () => {
                cloneItem.style.background = 'white';
            });
            
            cloneItem.addEventListener('click', () => {
                this.cloneNode(node.id);
                menu.remove();
            });
            
            menu.appendChild(cloneItem);
            
            // For packaging and raw-material nodes, add packaging group options
            if (node.type === 'packaging' || node.type === 'raw-material') {
                const divider = document.createElement('div');
                divider.style.height = '1px';
                divider.style.background = '#dee2e6';
                divider.style.margin = '0.25rem 0';
                menu.appendChild(divider);
                
                const groupTitle = document.createElement('div');
                groupTitle.style.fontWeight = '600';
                groupTitle.style.padding = '0.5rem';
                groupTitle.style.fontSize = '0.85rem';
                groupTitle.style.color = '#6c757d';
                groupTitle.textContent = 'Move to Packaging Group:';
                menu.appendChild(groupTitle);
                
                // Get all packaging groups
                const groups = Array.from(this.nodes.values()).filter(n => n.type === 'packaging-group');
                
                if (groups.length > 0) {
                    groups.forEach(group => {
                        const groupItem = document.createElement('div');
                        groupItem.style.padding = '0.5rem';
                        groupItem.style.cursor = 'pointer';
                        groupItem.style.borderRadius = '4px';
                        groupItem.style.fontSize = '0.9rem';
                        groupItem.textContent = node.groupId === group.id ? `✓ ${group.name}` : group.name;
                        
                        if (node.groupId === group.id) {
                            groupItem.style.fontWeight = '600';
                        }
                        
                        groupItem.addEventListener('mouseenter', () => {
                            groupItem.style.background = '#f8f9fa';
                        });
                        groupItem.addEventListener('mouseleave', () => {
                            groupItem.style.background = 'white';
                        });
                        
                        groupItem.addEventListener('click', () => {
                            if (node.groupId === group.id) {
                                this.removeItemFromGroup(node.id);
                            } else {
                                this.moveItemToGroup(node.id, group.id);
                            }
                            menu.remove();
                        });
                        
                        menu.appendChild(groupItem);
                    });
                } else {
                    const noGroups = document.createElement('div');
                    noGroups.style.padding = '0.5rem';
                    noGroups.style.fontSize = '0.85rem';
                    noGroups.style.color = '#6c757d';
                    noGroups.style.fontStyle = 'italic';
                    noGroups.textContent = 'No groups available';
                    menu.appendChild(noGroups);
                }
                
                // Remove from group option if item is already in a group
                if (node.groupId) {
                    const removeFromGroup = document.createElement('div');
                    removeFromGroup.style.padding = '0.5rem';
                    removeFromGroup.style.cursor = 'pointer';
                    removeFromGroup.style.borderRadius = '4px';
                    removeFromGroup.style.marginTop = '0.25rem';
                    removeFromGroup.style.borderTop = '1px solid #dee2e6';
                    removeFromGroup.style.color = '#dc3545';
                    removeFromGroup.style.fontSize = '0.9rem';
                    removeFromGroup.textContent = 'Remove from Group';
                    
                    removeFromGroup.addEventListener('mouseenter', () => {
                        removeFromGroup.style.background = '#f8f9fa';
                    });
                    removeFromGroup.addEventListener('mouseleave', () => {
                        removeFromGroup.style.background = 'white';
                    });
                    
                    removeFromGroup.addEventListener('click', () => {
                        this.removeItemFromGroup(node.id);
                        menu.remove();
                    });
                    
                    menu.appendChild(removeFromGroup);
                }
            }
            
            // For distribution nodes, add distribution group options
            if (node.type === 'distribution') {
                const divider = document.createElement('div');
                divider.style.height = '1px';
                divider.style.background = '#dee2e6';
                divider.style.margin = '0.25rem 0';
                menu.appendChild(divider);
                
                const groupTitle = document.createElement('div');
                groupTitle.style.fontWeight = '600';
                groupTitle.style.padding = '0.5rem';
                groupTitle.style.fontSize = '0.85rem';
                groupTitle.style.color = '#6c757d';
                groupTitle.textContent = 'Move to Distribution Group:';
                menu.appendChild(groupTitle);
                
                // Get all distribution groups
                const groups = Array.from(this.nodes.values()).filter(n => n.type === 'distribution-group');
                
                if (groups.length > 0) {
                    groups.forEach(group => {
                        const groupItem = document.createElement('div');
                        groupItem.style.padding = '0.5rem';
                        groupItem.style.cursor = 'pointer';
                        groupItem.style.borderRadius = '4px';
                        groupItem.style.fontSize = '0.9rem';
                        groupItem.textContent = node.groupId === group.id ? `✓ ${group.name}` : group.name;
                        
                        if (node.groupId === group.id) {
                            groupItem.style.fontWeight = '600';
                        }
                        
                        groupItem.addEventListener('mouseenter', () => {
                            groupItem.style.background = '#f8f9fa';
                        });
                        groupItem.addEventListener('mouseleave', () => {
                            groupItem.style.background = 'white';
                        });
                        
                        groupItem.addEventListener('click', () => {
                            if (node.groupId === group.id) {
                                this.removeItemFromGroup(node.id);
                            } else {
                                this.moveItemToGroup(node.id, group.id);
                            }
                            menu.remove();
                        });
                        
                        menu.appendChild(groupItem);
                    });
                } else {
                    const noGroups = document.createElement('div');
                    noGroups.style.padding = '0.5rem';
                    noGroups.style.fontSize = '0.85rem';
                    noGroups.style.color = '#6c757d';
                    noGroups.style.fontStyle = 'italic';
                    noGroups.textContent = 'No groups available';
                    menu.appendChild(noGroups);
                }
                
                // Remove from group option if item is already in a group
                if (node.groupId) {
                    const removeFromGroup = document.createElement('div');
                    removeFromGroup.style.padding = '0.5rem';
                    removeFromGroup.style.cursor = 'pointer';
                    removeFromGroup.style.borderRadius = '4px';
                    removeFromGroup.style.marginTop = '0.25rem';
                    removeFromGroup.style.borderTop = '1px solid #dee2e6';
                    removeFromGroup.style.color = '#dc3545';
                    removeFromGroup.style.fontSize = '0.9rem';
                    removeFromGroup.textContent = 'Remove from Group';
                    
                    removeFromGroup.addEventListener('mouseenter', () => {
                        removeFromGroup.style.background = '#f8f9fa';
                    });
                    removeFromGroup.addEventListener('mouseleave', () => {
                        removeFromGroup.style.background = 'white';
                    });
                    
                    removeFromGroup.addEventListener('click', () => {
                        this.removeItemFromGroup(node.id);
                        menu.remove();
                    });
                    
                    menu.appendChild(removeFromGroup);
                }
            }
            
            document.body.appendChild(menu);
            
            // Close menu when clicking outside
            const closeMenu = (e) => {
                if (!menu.contains(e.target)) {
                    menu.remove();
                    document.removeEventListener('click', closeMenu);
                }
            };
            
            setTimeout(() => {
                document.addEventListener('click', closeMenu);
            }, 100);
        }
        
        showGroupContextMenu(e, node) {
            // Remove existing context menu
            const existingMenu = document.getElementById('eprGroupContextMenu');
            if (existingMenu) {
                existingMenu.remove();
            }
            
            // Get all packaging groups
            const groups = Array.from(this.nodes.values()).filter(n => n.type === 'packaging-group');
            
            if (groups.length === 0) {
                alert('No packaging groups available. Create a packaging group first.');
                return;
            }
            
            // Create context menu
            const menu = document.createElement('div');
            menu.id = 'eprGroupContextMenu';
            menu.style.position = 'fixed';
            menu.style.left = `${e.clientX}px`;
            menu.style.top = `${e.clientY}px`;
            menu.style.background = 'white';
            menu.style.border = '1px solid #dee2e6';
            menu.style.borderRadius = '4px';
            menu.style.boxShadow = '0 2px 8px rgba(0,0,0,0.15)';
            menu.style.zIndex = '10000';
            menu.style.padding = '0.5rem';
            menu.style.minWidth = '200px';
            
            const title = document.createElement('div');
            title.style.fontWeight = '600';
            title.style.marginBottom = '0.5rem';
            title.style.paddingBottom = '0.5rem';
            title.style.borderBottom = '1px solid #dee2e6';
            title.textContent = 'Move to Group:';
            menu.appendChild(title);
            
            groups.forEach(group => {
                const item = document.createElement('div');
                item.style.padding = '0.5rem';
                item.style.cursor = 'pointer';
                item.style.borderRadius = '4px';
                item.textContent = group.name;
                
                item.addEventListener('mouseenter', () => {
                    item.style.background = '#f8f9fa';
                });
                item.addEventListener('mouseleave', () => {
                    item.style.background = 'white';
                });
                
                item.addEventListener('click', () => {
                    if (node.groupId === group.id) {
                        // Remove from group
                        this.removeItemFromGroup(node.id);
                    } else {
                        // Move to group
                        this.moveItemToGroup(node.id, group.id);
                    }
                    menu.remove();
                });
                
                if (node.groupId === group.id) {
                    item.style.fontWeight = '600';
                    item.textContent = `✓ ${group.name} (Remove)`;
                }
                
                menu.appendChild(item);
            });
            
            // Remove from group option if item is already in a group
            if (node.groupId) {
                const removeItem = document.createElement('div');
                removeItem.style.padding = '0.5rem';
                removeItem.style.cursor = 'pointer';
                removeItem.style.borderRadius = '4px';
                removeItem.style.marginTop = '0.5rem';
                removeItem.style.borderTop = '1px solid #dee2e6';
                removeItem.style.color = '#dc3545';
                removeItem.textContent = 'Remove from Group';
                
                removeItem.addEventListener('mouseenter', () => {
                    removeItem.style.background = '#f8f9fa';
                });
                removeItem.addEventListener('mouseleave', () => {
                    removeItem.style.background = 'white';
                });
                
                removeItem.addEventListener('click', () => {
                    this.removeItemFromGroup(node.id);
                    menu.remove();
                });
                
                menu.appendChild(removeItem);
            }
            
            document.body.appendChild(menu);
            
            // Close menu when clicking outside
            const closeMenu = (e) => {
                if (!menu.contains(e.target)) {
                    menu.remove();
                    document.removeEventListener('click', closeMenu);
                }
            };
            setTimeout(() => {
                document.addEventListener('click', closeMenu);
            }, 100);
        }
        
        selectNode(nodeId) {
            this.selectedNode = nodeId;
            // Clear multi-select when single selecting
            if (this.selectedNodes.size > 0) {
                this.selectedNodes.forEach(id => {
                    const el = document.querySelector(`[data-node-id="${id}"]`);
                    if (el) el.classList.remove('selected');
                });
                this.selectedNodes.clear();
            }
            this.updateNodeSelection();
            
            if (window.EPRVisualEditor && window.EPRVisualEditor.parametersPanel) {
                window.EPRVisualEditor.parametersPanel.loadNodeParameters(this.nodes.get(nodeId));
            }
        }
        
        deselectNode() {
            this.selectedNode = null;
            // Also clear multi-select
            if (this.selectedNodes.size > 0) {
                this.selectedNodes.forEach(id => {
                    const el = document.querySelector(`[data-node-id="${id}"]`);
                    if (el) el.classList.remove('selected');
                });
                this.selectedNodes.clear();
            }
            this.updateNodeSelection();
            
            if (window.EPRVisualEditor && window.EPRVisualEditor.parametersPanel) {
                window.EPRVisualEditor.parametersPanel.clearParameters();
            }
        }
        
        startSelection(e) {
            this.isSelecting = true;
            const rect = this.canvas.getBoundingClientRect();
            this.selectionStart = {
                x: e.clientX - rect.left,
                y: e.clientY - rect.top
            };
            
            // Create selection box element
            if (!this.selectionBox) {
                this.selectionBox = document.createElement('div');
                this.selectionBox.className = 'epr-selection-box';
                this.selectionBox.style.cssText = 'position: absolute; border: 2px dashed #28a745; background: rgba(40, 167, 69, 0.1); pointer-events: none; z-index: 1000; display: none;';
                this.canvas.appendChild(this.selectionBox);
            }
            this.selectionBox.style.display = 'block';
            this.selectionBox.style.left = this.selectionStart.x + 'px';
            this.selectionBox.style.top = this.selectionStart.y + 'px';
            this.selectionBox.style.width = '0px';
            this.selectionBox.style.height = '0px';
        }
        
        updateSelection(e) {
            if (!this.isSelecting || !this.selectionStart) return;
            
            const rect = this.canvas.getBoundingClientRect();
            const currentX = e.clientX - rect.left;
            const currentY = e.clientY - rect.top;
            
            const left = Math.min(this.selectionStart.x, currentX);
            const top = Math.min(this.selectionStart.y, currentY);
            const width = Math.abs(currentX - this.selectionStart.x);
            const height = Math.abs(currentY - this.selectionStart.y);
            
            if (this.selectionBox) {
                this.selectionBox.style.left = left + 'px';
                this.selectionBox.style.top = top + 'px';
                this.selectionBox.style.width = width + 'px';
                this.selectionBox.style.height = height + 'px';
            }
        }
        
        finishSelection(e) {
            if (!this.isSelecting || !this.selectionStart) return;
            
            const rect = this.canvas.getBoundingClientRect();
            const endX = e.clientX - rect.left;
            const endY = e.clientY - rect.top;
            
            // Only process selection if there's a meaningful selection box (at least 5px)
            const width = Math.abs(endX - this.selectionStart.x);
            const height = Math.abs(endY - this.selectionStart.y);
            
            if (width < 5 && height < 5) {
                // Very small selection - treat as click, clear selection
                this.selectedNodes.forEach(id => {
                    const el = document.querySelector(`[data-node-id="${id}"]`);
                    if (el) el.classList.remove('selected');
                });
                this.selectedNodes.clear();
                this.selectedNode = null;
                this.updateNodeSelection();
                
                // Clean up
                this.isSelecting = false;
                this.selectionStart = null;
                if (this.selectionBox) {
                    this.selectionBox.style.display = 'none';
                }
                return;
            }
            
            const left = Math.min(this.selectionStart.x, endX);
            const top = Math.min(this.selectionStart.y, endY);
            const right = Math.max(this.selectionStart.x, endX);
            const bottom = Math.max(this.selectionStart.y, endY);
            
            // Find all nodes within selection box
            // Account for zoom and pan transforms
            const selectedNodeIds = new Set();
            this.nodes.forEach((node, nodeId) => {
                const nodeEl = document.querySelector(`[data-node-id="${nodeId}"]`);
                if (!nodeEl) return;
                
                // Get node position accounting for transform
                const nodeX = node.x * this.zoomLevel + this.panOffset.x;
                const nodeY = node.y * this.zoomLevel + this.panOffset.y;
                
                // Get node dimensions (accounting for zoom)
                const nodeRect = nodeEl.getBoundingClientRect();
                const canvasRect = this.canvas.getBoundingClientRect();
                const nodeWidth = nodeRect.width;
                const nodeHeight = nodeRect.height;
                
                // Calculate node bounds in canvas coordinates (accounting for transform)
                const nodeLeft = nodeX;
                const nodeTop = nodeY;
                const nodeRight = nodeLeft + nodeWidth;
                const nodeBottom = nodeTop + nodeHeight;
                
                // Check if node overlaps with selection box
                if (nodeRight >= left && nodeLeft <= right && nodeBottom >= top && nodeTop <= bottom) {
                    selectedNodeIds.add(nodeId);
                }
            });
            
            // Update selection - always update even if no nodes selected (to clear previous selection)
            // Clear previous selection first
            this.selectedNodes.forEach(id => {
                const el = document.querySelector(`[data-node-id="${id}"]`);
                if (el) el.classList.remove('selected');
            });
            this.selectedNodes.clear();
            this.selectedNode = null;
            
            // Add new selections if any
            if (selectedNodeIds.size > 0) {
                selectedNodeIds.forEach(id => {
                    this.selectedNodes.add(id);
                    const el = document.querySelector(`[data-node-id="${id}"]`);
                    if (el) el.classList.add('selected');
                });
            }
            
            this.updateNodeSelection();
            
            // Clean up
            this.isSelecting = false;
            this.selectionStart = null;
            if (this.selectionBox) {
                this.selectionBox.style.display = 'none';
            }
        }
        
        updateNodeSelection() {
            // Update visual selection for both single and multi-select
            document.querySelectorAll('.epr-canvas-node').forEach(el => {
                el.classList.remove('selected');
            });
            
            // Show multi-select first
            if (this.selectedNodes && this.selectedNodes.size > 0) {
                this.selectedNodes.forEach(nodeId => {
                    const nodeEl = document.querySelector(`[data-node-id="${nodeId}"]`);
                    if (nodeEl) {
                        nodeEl.classList.add('selected');
                    }
                });
            }
            
            // Then show single select (if no multi-select)
            if (this.selectedNode && (!this.selectedNodes || this.selectedNodes.size === 0)) {
                const nodeEl = document.querySelector(`[data-node-id="${this.selectedNode}"]`);
                if (nodeEl) {
                    nodeEl.classList.add('selected');
                }
            }
        }
        
        addConnection(fromNodeId, toNodeId, fromPortParam, toPortParam) {
            // Validate connection rules
            const fromNode = this.nodes.get(fromNodeId);
            const toNode = this.nodes.get(toNodeId);
            
            if (!fromNode || !toNode) {
                console.warn('[EPR Canvas] Invalid nodes for connection');
                return false;
            }
            
            // Check if connection already exists
            if (this.connections.some(c => c.from === fromNodeId && c.to === toNodeId)) {
                return false;
            }
            
            // Determine port configuration based on connection rules
            // Use provided ports if available, otherwise determine from rules
            let fromPort = fromPortParam || 'right';
            let toPort = toPortParam || 'left';
            
            // Connection rules:
            // Raw Materials → Packaging (RIGHT to LEFT)
            if (fromNode.type === 'raw-material' && toNode.type === 'packaging') {
                fromPort = 'right';
                toPort = 'left';
            }
            // Packaging → Product (RIGHT to LEFT)
            else if (fromNode.type === 'packaging' && toNode.type === 'product') {
                fromPort = 'right';
                toPort = 'left';
            }
            // Product → Distribution (RIGHT to LEFT)
            else if (fromNode.type === 'product' && toNode.type === 'distribution') {
                fromPort = 'right';
                toPort = 'left';
            }
            // Product → Packaging Group (RIGHT to LEFT or RIGHT)
            else if (fromNode.type === 'product' && toNode.type === 'packaging-group') {
                fromPort = fromPortParam || 'right';
                toPort = toPortParam || 'left';
            }
            // Packaging Group → Distribution (RIGHT to LEFT)
            else if (fromNode.type === 'packaging-group' && toNode.type === 'distribution') {
                fromPort = fromPortParam || 'right';
                toPort = toPortParam || 'left';
            }
            // Packaging → Raw Materials (RIGHT to RIGHT)
            else if (fromNode.type === 'packaging' && toNode.type === 'raw-material') {
                fromPort = 'right';
                toPort = 'right';
            }
            // Product → Packaging (RIGHT to RIGHT)
            else if (fromNode.type === 'product' && toNode.type === 'packaging') {
                fromPort = 'right';
                toPort = 'right';
            }
            // Distribution → Product (RIGHT to RIGHT)
            else if (fromNode.type === 'distribution' && toNode.type === 'product') {
                fromPort = 'right';
                toPort = 'right';
            }
            // Supplier Packaging → Product (RIGHT to LEFT)
            else if (fromNode.type === 'supplier-packaging' && toNode.type === 'product') {
                fromPort = 'right';
                toPort = 'left';
            }
            // Distribution → Distribution (can connect to LEFT or RIGHT)
            else if (fromNode.type === 'distribution' && toNode.type === 'distribution') {
                // Use provided ports if available, otherwise default to right->left
                // But allow connecting to either port of the target distribution node
                if (!fromPortParam) fromPort = 'right';
                if (!toPortParam) {
                    // Default to left, but will be overridden if user drags to right port
                    toPort = 'left';
                } else {
                    // Use the port the user specified
                    toPort = toPortParam;
                }
            }
            // Packaging Group → Packaging Group (can connect LEFT or RIGHT)
            else if (fromNode.type === 'packaging-group' && toNode.type === 'packaging-group') {
                // Groups can connect to each other on left or right
                if (!fromPortParam) fromPort = 'right';
                if (!toPortParam) {
                    toPort = 'left';
                } else {
                    toPort = toPortParam;
                }
            }
            // Packaging Group → Other nodes (RIGHT to LEFT)
            else if (fromNode.type === 'packaging-group') {
                fromPort = 'right';
                toPort = toPortParam || 'left';
            }
            // Other nodes → Packaging Group (RIGHT to LEFT or RIGHT)
            else if (toNode.type === 'packaging-group') {
                fromPort = fromPortParam || 'right';
                toPort = toPortParam || 'left';
            }
            // Default: allow connection but use standard ports
            else {
                if (!fromPortParam) fromPort = 'right';
                if (!toPortParam) toPort = 'left';
            }
            
            this.connections.push({ 
                from: fromNodeId, 
                to: toNodeId,
                fromPort: fromPort,
                toPort: toPort
            });
            
            // Persist supplier-packaging to product attachment
            if (fromNode.type === 'supplier-packaging' && toNode.type === 'product' && fromNode.entityId && toNode.entityId) {
                fetch(`/api/visual-editor/product/${toNode.entityId}/supplier-packaging/${fromNode.entityId}`, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' }
                }).then(r => r.json()).then(data => {
                    if (data.success) console.log('[EPR] Supplier packaging attached to product');
                }).catch(err => console.warn('[EPR] Failed to persist supplier packaging attachment:', err));
            }
            // Persist raw-material to packaging item (supply chain: raw materials -> packaging items)
            if (fromNode.type === 'raw-material' && toNode.type === 'packaging' && fromNode.entityId && toNode.entityId) {
                fetch(`/api/packaging-management/packaging-items/${toNode.entityId}/raw-materials`, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ materialTaxonomyId: fromNode.entityId })
                }).then(r => r.json()).then(data => {
                    if (data.success) console.log('[EPR] Raw material linked to packaging item');
                }).catch(err => console.warn('[EPR] Failed to persist raw material link:', err));
            }
            // Persist packaging to product attachment
            if (fromNode.type === 'packaging' && toNode.type === 'product' && fromNode.entityId && toNode.entityId) {
                fetch(`/api/visual-editor/product/${toNode.entityId}/packaging/${fromNode.entityId}`, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' }
                }).then(r => r.json()).then(data => {
                    if (data.success) console.log('[EPR] Packaging attached to product');
                }).catch(err => console.warn('[EPR] Failed to persist packaging-product link:', err));
            }
            // Persist product to distribution
            if (fromNode.type === 'product' && toNode.type === 'distribution' && fromNode.entityId) {
                const distParams = toNode.parameters || {};
                fetch(`/api/visual-editor/product/${fromNode.entityId}/distribution`, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ city: distParams.city || toNode.name, country: distParams.country || '', retailerName: distParams.retailerName || toNode.name, quantity: 1 })
                }).then(r => r.json()).then(data => {
                    if (data.success && data.distributionId) {
                        toNode.entityId = data.distributionId;
                        console.log('[EPR] Distribution created and linked to product');
                    }
                }).catch(err => console.warn('[EPR] Failed to persist product-distribution link:', err));
            }
            
            this.updateConnections();
            
            // Update Packaging Group hierarchy pills when connections change
            this.updatePackagingGroupPills();
            
            this.saveState();
            return true;
        }
        
        removeConnection(fromNodeId, toNodeId) {
            const fromNode = this.nodes.get(fromNodeId);
            const toNode = this.nodes.get(toNodeId);
            if (fromNode && toNode && fromNode.type === 'supplier-packaging' && toNode.type === 'product' && fromNode.entityId && toNode.entityId) {
                fetch(`/api/visual-editor/product/${toNode.entityId}/supplier-packaging/${fromNode.entityId}`, { method: 'DELETE' })
                    .then(r => r.json()).then(() => {}).catch(() => {});
            }
            if (fromNode && toNode && fromNode.type === 'raw-material' && toNode.type === 'packaging' && fromNode.entityId && toNode.entityId) {
                fetch(`/api/packaging-management/packaging-items/${toNode.entityId}/raw-materials/${fromNode.entityId}`, { method: 'DELETE' })
                    .then(r => r.json()).then(() => {}).catch(() => {});
            }
            if (fromNode && toNode && fromNode.type === 'packaging' && toNode.type === 'product' && fromNode.entityId && toNode.entityId) {
                fetch(`/api/visual-editor/product/${toNode.entityId}/packaging/${fromNode.entityId}`, { method: 'DELETE' })
                    .then(r => r.json()).then(() => {}).catch(() => {});
            }
            if (fromNode && toNode && fromNode.type === 'product' && toNode.type === 'distribution' && toNode.entityId) {
                fetch(`/api/visual-editor/distribution/${toNode.entityId}`, { method: 'DELETE' })
                    .then(r => r.json()).then(() => {}).catch(() => {});
            }
            this.connections = this.connections.filter(
                c => !(c.from === fromNodeId && c.to === toNodeId)
            );
            this.updateConnections();
            
            // Update Packaging Group hierarchy pills when connections change
            this.updatePackagingGroupPills();
            
            this.saveState();
        }
        
        showConnectionModal(connectionInfo, clickX = null, clickY = null) {
            const fromNode = this.nodes.get(connectionInfo.from);
            const toNode = this.nodes.get(connectionInfo.to);
            
            // Check if connection is to/from a product
            const isProductConnection = (fromNode && fromNode.type === 'product') || (toNode && toNode.type === 'product');
            const buttonText = isProductConnection ? 'Add Number' : 'Add Quantity';
            
            // Create small modal for connection options - positioned at click location if provided
            const modal = document.createElement('div');
            modal.className = 'modal fade show';
            modal.style.display = 'block';
            modal.style.zIndex = '9999';
            modal.style.backgroundColor = 'rgba(0,0,0,0.5)';
            modal.style.position = 'fixed';
            modal.style.top = '0';
            modal.style.left = '0';
            modal.style.width = '100%';
            modal.style.height = '100%';
            
            // Calculate position for modal dialog
            let dialogStyle = 'max-width: 200px;';
            if (clickX !== null && clickY !== null) {
                // Position modal at click location - ensure it's within viewport
                const modalWidth = 200;
                const modalHeight = 150;
                const padding = 20;
                
                // Clamp coordinates to viewport bounds
                const maxX = window.innerWidth - modalWidth / 2 - padding;
                const minX = modalWidth / 2 + padding;
                const maxY = window.innerHeight - modalHeight / 2 - padding;
                const minY = modalHeight / 2 + padding;
                
                const clampedX = Math.max(minX, Math.min(maxX, clickX));
                const clampedY = Math.max(minY, Math.min(maxY, clickY));
                
                dialogStyle += `position: absolute; left: ${clampedX}px; top: ${clampedY}px; transform: translate(-50%, -50%);`;
            } else {
                dialogStyle += 'margin: auto;';
            }
            
            modal.innerHTML = `
                <div class="modal-dialog" style="${dialogStyle}">
                    <div class="modal-content" style="padding: 10px;">
                        <div style="display: flex; gap: 5px; justify-content: flex-end; margin-bottom: 5px;">
                            <button type="button" class="btn-close btn-sm" style="font-size: 0.7rem;"></button>
                        </div>
                        <div style="display: flex; flex-direction: column; gap: 5px;">
                            <button type="button" class="btn btn-sm btn-primary" id="eprAddQuantityToConnectionBtn" style="font-size: 0.85rem;">${buttonText}</button>
                            <button type="button" class="btn btn-sm btn-primary" id="eprAddTransportToConnectionBtn" style="font-size: 0.85rem;">Add Transport</button>
                            <button type="button" class="btn btn-sm btn-danger" id="eprDeleteConnectionBtn" style="font-size: 0.85rem;">Delete</button>
                        </div>
                    </div>
                </div>
            `;
            document.body.appendChild(modal);
            
            const canvasManager = this;
            
            // Delete button handler
            document.getElementById('eprDeleteConnectionBtn').addEventListener('click', () => {
                canvasManager.removeConnection(connectionInfo.from, connectionInfo.to);
                modal.remove();
            });
            
            // Add Quantity/Number button handler
            document.getElementById('eprAddQuantityToConnectionBtn').addEventListener('click', () => {
                modal.remove();
                canvasManager.showQuantityModal(connectionInfo, isProductConnection);
            });
            
            // Add Transport button handler
            document.getElementById('eprAddTransportToConnectionBtn').addEventListener('click', () => {
                modal.remove();
                canvasManager.addTransportBetweenNodes(connectionInfo);
            });
            
            // Close button handler
            modal.querySelector('.btn-close').addEventListener('click', () => {
                modal.remove();
            });
            
            // Close on backdrop click
            modal.addEventListener('click', (e) => {
                if (e.target === modal) {
                    modal.remove();
                }
            });
        }
        
        showQuantityModal(connectionInfo, isProductConnection = false) {
            const fromNode = this.nodes.get(connectionInfo.from);
            if (!fromNode) return;
            
            // Get current quantity for this connection
            const connectionKey = `${connectionInfo.from}-${connectionInfo.to}`;
            const currentQuantityData = fromNode.parameters?.connectionQuantities?.[connectionKey];
            const currentQuantity = currentQuantityData?.value || currentQuantityData || '';
            const currentType = currentQuantityData?.type || '';
            const currentUnit = currentQuantityData?.unit || '';
            
            // Create modal for quantity/number input
            const modal = document.createElement('div');
            modal.className = 'modal fade show';
            modal.style.display = 'block';
            modal.style.zIndex = '9999';
            modal.style.backgroundColor = 'rgba(0,0,0,0.5)';
            
            let modalBody = '';
            if (isProductConnection) {
                // Product connection - show type and unit selection
                modalBody = `
                    <div class="mb-3">
                        <label for="eprNumberType" class="form-label">Type</label>
                        <select class="form-select" id="eprNumberType">
                            <option value="volume" ${currentType === 'volume' ? 'selected' : ''}>Volume</option>
                            <option value="quantity" ${currentType === 'quantity' || !currentType ? 'selected' : ''}>Quantity</option>
                            <option value="weight" ${currentType === 'weight' ? 'selected' : ''}>Weight</option>
                        </select>
                    </div>
                    <div class="mb-3">
                        <label for="eprNumberUnit" class="form-label">Unit</label>
                        <select class="form-select" id="eprNumberUnit">
                            <option value="">Select unit...</option>
                        </select>
                    </div>
                    <div class="mb-3">
                        <label for="eprQuantityInput" class="form-label">Value</label>
                        <input type="number" class="form-control" id="eprQuantityInput" 
                               value="${currentQuantity}" placeholder="Enter value" min="0" 
                               step="${currentType === 'quantity' ? '0.01' : '1'}">
                        <small class="form-text text-muted">Enter the numerical value for this connection</small>
                    </div>
                `;
            } else {
                // Regular connection - simple quantity input
                modalBody = `
                    <div class="mb-3">
                        <label for="eprQuantityInput" class="form-label">Quantity</label>
                        <input type="number" class="form-control" id="eprQuantityInput" 
                               value="${currentQuantity}" placeholder="Enter quantity" min="0" step="1">
                        <small class="form-text text-muted">Enter the quantity value for this connection</small>
                    </div>
                `;
            }
            
            modal.innerHTML = `
                <div class="modal-dialog modal-dialog-centered">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">${isProductConnection ? 'Add Number' : 'Add Quantity'}</h5>
                            <button type="button" class="btn-close"></button>
                        </div>
                        <div class="modal-body">
                            ${modalBody}
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" id="eprCancelQuantityBtn">Cancel</button>
                            <button type="button" class="btn btn-primary" id="eprSaveQuantityBtn">Save</button>
                        </div>
                    </div>
                </div>
            `;
            document.body.appendChild(modal);
            
            const canvasManager = this;
            
            // Update unit options based on type selection (for product connections)
            if (isProductConnection) {
                const typeSelect = document.getElementById('eprNumberType');
                const unitSelect = document.getElementById('eprNumberUnit');
                const quantityInput = document.getElementById('eprQuantityInput');
                
                const updateUnits = () => {
                    const type = typeSelect.value;
                    unitSelect.innerHTML = '<option value="">Select unit...</option>';
                    
                    if (type === 'volume') {
                        unitSelect.innerHTML += '<option value="ml" ' + (currentUnit === 'ml' ? 'selected' : '') + '>ml</option>';
                        unitSelect.innerHTML += '<option value="L" ' + (currentUnit === 'L' ? 'selected' : '') + '>L</option>';
                        quantityInput.step = '1';
                    } else if (type === 'quantity') {
                        unitSelect.innerHTML += '<option value="count" ' + (currentUnit === 'count' || !currentUnit ? 'selected' : '') + '>count</option>';
                        quantityInput.step = '0.01';
                    } else if (type === 'weight') {
                        unitSelect.innerHTML += '<option value="g" ' + (currentUnit === 'g' ? 'selected' : '') + '>g</option>';
                        unitSelect.innerHTML += '<option value="kg" ' + (currentUnit === 'kg' ? 'selected' : '') + '>kg</option>';
                        quantityInput.step = '1';
                    }
                };
                
                typeSelect.addEventListener('change', updateUnits);
                updateUnits(); // Initialize units
            }
            
            // Save button handler
            document.getElementById('eprSaveQuantityBtn').addEventListener('click', () => {
                const quantityInput = document.getElementById('eprQuantityInput');
                const quantity = quantityInput.value.trim();
                
                if (!fromNode.parameters) {
                    fromNode.parameters = {};
                }
                if (!fromNode.parameters.connectionQuantities) {
                    fromNode.parameters.connectionQuantities = {};
                }
                
                if (quantity === '' || quantity === '0') {
                    // Remove quantity if empty or zero
                    delete fromNode.parameters.connectionQuantities[connectionKey];
                } else {
                    if (isProductConnection) {
                        const type = document.getElementById('eprNumberType').value;
                        const unit = document.getElementById('eprNumberUnit').value;
                        
                        if (!unit) {
                            alert('Please select a unit');
                            return;
                        }
                        
                        // Format quantity to 2 decimal places if type is quantity
                        let formattedQuantity = parseFloat(quantity) || 0;
                        if (type === 'quantity') {
                            formattedQuantity = parseFloat(formattedQuantity.toFixed(2));
                        }
                        
                        fromNode.parameters.connectionQuantities[connectionKey] = {
                            value: formattedQuantity,
                            type: type,
                            unit: unit
                        };
                    } else {
                        fromNode.parameters.connectionQuantities[connectionKey] = parseFloat(quantity) || 0;
                    }
                }
                
                // Update connections to show/hide quantity circles
                this.updateConnections();
                this.saveState();
                modal.remove();
            });
            
            // Cancel and close handlers
            document.getElementById('eprCancelQuantityBtn').addEventListener('click', () => {
                modal.remove();
            });
            
            modal.querySelector('.btn-close').addEventListener('click', () => {
                modal.remove();
            });
            
            modal.addEventListener('click', (e) => {
                if (e.target === modal) {
                    modal.remove();
                }
            });
        }
        
        addTransportBetweenNodes(connectionInfo) {
            const fromNode = this.nodes.get(connectionInfo.from);
            const toNode = this.nodes.get(connectionInfo.to);
            
            if (!fromNode || !toNode) return;
            
            // Check if a transport node already exists between these two nodes
            const existingTransport = this.findTransportBetweenNodes(connectionInfo.from, connectionInfo.to);
            if (existingTransport) {
                // Check if there's a quantity circle - if so, allow adding transport as circle
                const connectionKey = `${connectionInfo.from}-${connectionInfo.to}`;
                const connectionQuantityData = fromNode.parameters?.connectionQuantities?.[connectionKey];
                const connectionQuantity = typeof connectionQuantityData === 'object' ? connectionQuantityData.value : connectionQuantityData;
                
                if (connectionQuantity && connectionQuantity > 0) {
                    // Quantity exists, so transport will be shown as circle - allow adding more transport types
                    // Just show the transport popup to add more transport types to existing node
                    const midX = (fromNode.x + toNode.x) / 2;
                    const midY = (fromNode.y + toNode.y) / 2;
                    const canvas = document.getElementById('eprCanvas');
                    const rect = canvas.getBoundingClientRect();
                    const screenX = rect.left + midX * (this.zoomLevel || 1) + (this.panOffset?.x || 0);
                    const screenY = rect.top + midY * (this.zoomLevel || 1) + (this.panOffset?.y || 0);
                    
                    // Store connection info and existing transport node
                    if (window.EPRVisualEditor && window.EPRVisualEditor.actionsToolbar) {
                        window.EPRVisualEditor.pendingConnectionInfo = {
                            fromNode: connectionInfo.from,
                            toNode: connectionInfo.to,
                            fromPort: connectionInfo.fromPort || 'right',
                            toPort: connectionInfo.toPort || 'left',
                            existingTransportId: existingTransport.id
                        };
                        
                        window.EPRVisualEditor.actionsToolbar.showTransportPopup(screenX, screenY, midX, midY);
                    }
                    return;
                } else {
                    alert('A transport node already exists between these two nodes. Only one transport node is allowed between any two nodes.');
                    return;
                }
            }
            
            // Calculate position between nodes
            const midX = (fromNode.x + toNode.x) / 2;
            const midY = (fromNode.y + toNode.y) / 2;
            
            // CRITICAL: Ensure the original direct connection exists BEFORE creating transport
            // This is required for the transport to be displayed as a small box on the connection line
            const originalConnExists = this.connections.some(c => 
                c.from === connectionInfo.from && c.to === connectionInfo.to
            );
            
            if (!originalConnExists) {
                // Create the original direct connection FIRST - this is REQUIRED!
                this.addConnection(connectionInfo.from, connectionInfo.to, connectionInfo.fromPort || 'right', connectionInfo.toPort || 'left');
            }
            
            // Show transport popup
            const canvas = document.getElementById('eprCanvas');
            const rect = canvas.getBoundingClientRect();
            const screenX = rect.left + midX * (this.zoomLevel || 1) + (this.panOffset?.x || 0);
            const screenY = rect.top + midY * (this.zoomLevel || 1) + (this.panOffset?.y || 0);
            
            // Store connection info for after transport node is created
            if (window.EPRVisualEditor && window.EPRVisualEditor.actionsToolbar) {
                window.EPRVisualEditor.pendingConnectionInfo = {
                    fromNode: connectionInfo.from,
                    toNode: connectionInfo.to,
                    fromPort: connectionInfo.fromPort || 'right',
                    toPort: connectionInfo.toPort || 'left'
                };
                
                window.EPRVisualEditor.actionsToolbar.showTransportPopup(screenX, screenY, midX, midY);
            }
        }
        
        findTransportBetweenNodes(fromNodeId, toNodeId) {
            // Check if there's a transport node that connects fromNodeId -> transport -> toNodeId
            // This works whether or not there's a direct connection between the nodes
            // The transport node should be displayed as a small box on the connection line
            
            // Check all connections to find a transport node in the path
            for (const conn of this.connections) {
                const toNode = this.nodes.get(conn.to);
                
                // Check if this connection goes from fromNodeId to a transport node
                if (conn.from === fromNodeId && toNode && toNode.type === 'transport') {
                    // Check if the transport node connects to toNodeId
                    const transportToTarget = this.connections.find(c => 
                        c.from === conn.to && c.to === toNodeId
                    );
                    if (transportToTarget) {
                        // Found transport node between these two nodes
                        // Return the transport node - it will be displayed as a box on the connection line
                        return this.nodes.get(conn.to);
                    }
                }
            }
            return null;
        }
        
        updateConnectionsWithTransportHidden() {
            const existingLines = this.connectionsLayer.querySelectorAll('.epr-connection-line');
            existingLines.forEach(line => line.remove());
            
            // Find all connections that go through transport nodes and create direct connections
            const directConnections = new Map();
            
            this.connections.forEach(conn => {
                const fromNode = this.nodes.get(conn.from);
                const toNode = this.nodes.get(conn.to);
                
                if (!fromNode || !toNode) return;
                
                // If fromNode is transport, find what connects to it
                if (fromNode.type === 'transport') {
                    const incomingConn = this.connections.find(c => c.to === conn.from);
                    if (incomingConn) {
                        const key = `${incomingConn.from}-${conn.to}`;
                        if (!directConnections.has(key)) {
                            directConnections.set(key, {
                                from: incomingConn.from,
                                to: conn.to,
                                fromPort: incomingConn.fromPort,
                                toPort: conn.toPort
                            });
                        }
                    }
                } else if (toNode.type === 'transport') {
                    // If toNode is transport, find what it connects to
                    const outgoingConn = this.connections.find(c => c.from === conn.to);
                    if (outgoingConn) {
                        const key = `${conn.from}-${outgoingConn.to}`;
                        if (!directConnections.has(key)) {
                            directConnections.set(key, {
                                from: conn.from,
                                to: outgoingConn.to,
                                fromPort: conn.fromPort,
                                toPort: outgoingConn.toPort
                            });
                        }
                    }
                } else {
                    // Regular connection, render it
                    this.renderConnection(conn);
                }
            });
            
            // Render direct connections
            directConnections.forEach(conn => {
                this.renderConnection(conn);
            });
        }
        
        renderConnection(conn) {
            const fromNode = this.nodes.get(conn.from);
            const toNode = this.nodes.get(conn.to);
            
            if (!fromNode || !toNode) return;
            
            const fromPort = conn.fromPort || 'right';
            const toPort = conn.toPort || 'left';
            
            // Get actual node dimensions from DOM if available, otherwise use defaults
            const fromNodeEl = document.querySelector(`[data-node-id="${conn.from}"]`);
            const toNodeEl = document.querySelector(`[data-node-id="${conn.to}"]`);
            
            const fromNodeWidth = fromNodeEl ? fromNodeEl.offsetWidth : 150;
            const fromNodeHeight = fromNodeEl ? fromNodeEl.offsetHeight : 80;
            const toNodeWidth = toNodeEl ? toNodeEl.offsetWidth : 150;
            const toNodeHeight = toNodeEl ? toNodeEl.offsetHeight : 80;
            
            const portRadius = 8; // Radius of connector circle (16px diameter / 2)
            
            // Connect from the center of the connector circle
            // Left port: positioned at left: -8px, so center is at node.x - 8px + 8px = node.x (but port extends 8px left, so center is at node.x - 8px + 8px = node.x)
            // Actually, left: -8px means the port's LEFT edge is 8px left of node.x, so center is at node.x - 8px + 8px = node.x
            // Right port: positioned at right: -8px, so center is at node.x + nodeWidth - 8px + 8px = node.x + nodeWidth
            // But wait, right: -8px means 8px to the right of the node's right edge, so center is at node.x + nodeWidth + 8px
            const startX = fromPort === 'right' ? fromNode.x + fromNodeWidth + portRadius : fromNode.x - portRadius;
            const startY = fromNode.y + fromNodeHeight / 2;
            const endX = toPort === 'right' ? toNode.x + toNodeWidth + portRadius : toNode.x - portRadius;
            const endY = toNode.y + toNodeHeight / 2;
            
            const dx = endX - startX;
            const dy = endY - startY;
            const distance = Math.sqrt(dx * dx + dy * dy);
            const curvature = Math.min(distance * 0.3, 100);
            
            const controlPoint1X = startX + Math.max(dx * 0.5, curvature);
            const controlPoint1Y = startY;
            const controlPoint2X = endX - Math.max(dx * 0.5, curvature);
            const controlPoint2Y = endY;
            
            const path = document.createElementNS('http://www.w3.org/2000/svg', 'path');
            const pathData = `M ${startX} ${startY} C ${controlPoint1X} ${controlPoint1Y}, ${controlPoint2X} ${controlPoint2Y}, ${endX} ${endY}`;
            path.setAttribute('d', pathData);
            path.setAttribute('class', 'epr-connection-line');
            path.setAttribute('stroke', '#6c757d');
            path.setAttribute('stroke-width', '4');
            path.setAttribute('fill', 'none');
            path.setAttribute('stroke-linecap', 'round');
            path.style.cursor = 'pointer';
            path.style.pointerEvents = 'stroke';
            path.setAttribute('data-from', conn.from);
            path.setAttribute('data-to', conn.to);
            
            const connectionInfo = { from: conn.from, to: conn.to, fromPort: conn.fromPort, toPort: conn.toPort };
            
            path.addEventListener('click', (e) => {
                e.stopPropagation();
                e.preventDefault();
                // Calculate center point of the connector line (midpoint of bezier curve)
                const midX = (startX + endX) / 2;
                const midY = (startY + endY) / 2;
                // Convert canvas coordinates to screen coordinates
                // Transform is: translate(panOffset) scale(zoomLevel)
                // So: screenX = canvasLeft + (midX * zoomLevel) + panOffset.x
                const canvas = document.getElementById('eprCanvas');
                const canvasRect = canvas.getBoundingClientRect();
                const screenX = canvasRect.left + (midX * this.zoomLevel) + this.panOffset.x;
                const screenY = canvasRect.top + (midY * this.zoomLevel) + this.panOffset.y;
                this.showConnectionModal(connectionInfo, screenX, screenY);
            });
            
            path.addEventListener('contextmenu', (e) => {
                e.preventDefault();
                e.stopPropagation();
                // Calculate center point of the connector line (midpoint of bezier curve)
                const midX = (startX + endX) / 2;
                const midY = (startY + endY) / 2;
                // Convert canvas coordinates to screen coordinates
                // Transform is: translate(panOffset) scale(zoomLevel)
                // So: screenX = canvasLeft + (midX * zoomLevel) + panOffset.x
                const canvas = document.getElementById('eprCanvas');
                const canvasRect = canvas.getBoundingClientRect();
                const screenX = canvasRect.left + (midX * this.zoomLevel) + this.panOffset.x;
                const screenY = canvasRect.top + (midY * this.zoomLevel) + this.panOffset.y;
                this.showConnectionModal(connectionInfo, screenX, screenY);
            });
            
            // Use SVG marker for arrow - automatically follows path direction
            path.setAttribute('marker-end', 'url(#arrowhead)');
            
            // Update arrow marker on hover
            path.addEventListener('mouseenter', () => {
                path.setAttribute('stroke', '#dc3545');
                path.setAttribute('stroke-width', '5');
                path.setAttribute('marker-end', 'url(#arrowhead-hover)');
            });
            
            path.addEventListener('mouseleave', () => {
                path.setAttribute('stroke', '#6c757d');
                path.setAttribute('stroke-width', '4');
                path.setAttribute('marker-end', 'url(#arrowhead)');
            });
            
            this.connectionsLayer.appendChild(path);
        }
        
        updateConnections() {
            // CRITICAL: Remove ALL elements from connections layer to prevent orphaned lines
            // Clear everything first, then rebuild only valid connections
            if (this.connectionsLayer) {
                // Preserve defs (marker definitions) and remove only connection elements
                const defs = this.connectionsLayer.querySelector('defs');
                const childrenToRemove = [];
                for (let i = 0; i < this.connectionsLayer.childNodes.length; i++) {
                    const child = this.connectionsLayer.childNodes[i];
                    if (child.nodeName !== 'defs') {
                        childrenToRemove.push(child);
                    }
                }
                childrenToRemove.forEach(child => this.connectionsLayer.removeChild(child));
                
                // Re-add defs if it was removed (shouldn't happen, but safety check)
                if (!defs && !this.connectionsLayer.querySelector('defs')) {
                    this.setupWatermark(); // This will recreate defs
                }
            }
            
            // Also remove any orphaned connection lines using querySelector as backup
            const existingLines = document.querySelectorAll('.epr-connection-line');
            existingLines.forEach(line => {
                if (line.parentNode) {
                    line.parentNode.removeChild(line);
                }
            });
            
            // Remove any other connection-related elements that might exist
            const orphanedElements = document.querySelectorAll(
                '.epr-holds-quantity-circle, .epr-holds-quantity-text, ' +
                '.epr-connection-quantity-circle, .epr-connection-quantity-text, .epr-connection-quantity-unit, ' +
                '.epr-transport-box, .epr-transport-foreign, .epr-transport-circle, .epr-transport-rectangle, .epr-transport-icon, .epr-transport-distance, ' +
                '.epr-quantity-transport-line'
            );
            orphanedElements.forEach(el => {
                if (el.parentNode) {
                    el.parentNode.removeChild(el);
                }
            });
            
            // Update transport node visibility - CRITICAL: Hide transport nodes that are between two nodes
            // They will be displayed as small boxes on connection lines instead
            this.nodes.forEach((node, nodeId) => {
                if (node.type === 'transport') {
                    const nodeEl = document.querySelector(`[data-node-id="${nodeId}"]`);
                    if (nodeEl) {
                        // Find incoming connection (something -> transport)
                        const incomingConn = this.connections.find(c => c.to === nodeId);
                        // Find outgoing connection (transport -> something)
                        const outgoingConn = this.connections.find(c => c.from === nodeId);
                        
                        if (incomingConn && outgoingConn) {
                            // Transport node is in a path between two nodes
                            // ALWAYS hide it - it will be displayed as a small box on the connection line
                            // Use MULTIPLE methods to ensure it's hidden - this is CRITICAL
                            nodeEl.style.display = 'none';
                            nodeEl.style.setProperty('display', 'none', 'important');
                            nodeEl.style.visibility = 'hidden';
                            nodeEl.style.setProperty('visibility', 'hidden', 'important');
                            nodeEl.style.opacity = '0';
                            nodeEl.style.setProperty('opacity', '0', 'important');
                            nodeEl.style.height = '0';
                            nodeEl.style.setProperty('height', '0', 'important');
                            nodeEl.style.width = '0';
                            nodeEl.style.setProperty('width', '0', 'important');
                            nodeEl.style.position = 'absolute';
                            nodeEl.style.setProperty('position', 'absolute', 'important');
                            nodeEl.style.left = '-9999px';
                            nodeEl.style.setProperty('left', '-9999px', 'important');
                            nodeEl.setAttribute('data-transport-on-connection', 'true');
                            nodeEl.classList.add('epr-transport-hidden');
                        } else {
                            // Transport node is NOT between two nodes - this should not happen!
                            // Transport nodes should ONLY exist between two nodes
                            // Hide it anyway - standalone transport nodes are not allowed
                            console.warn('[EPR Transport] Transport node found without connections - hiding it:', nodeId);
                            nodeEl.style.display = 'none';
                            nodeEl.style.setProperty('display', 'none', 'important');
                            nodeEl.style.visibility = 'hidden';
                            nodeEl.style.setProperty('visibility', 'hidden', 'important');
                            nodeEl.style.opacity = '0';
                            nodeEl.style.setProperty('opacity', '0', 'important');
                            nodeEl.style.height = '0';
                            nodeEl.style.setProperty('height', '0', 'important');
                            nodeEl.style.width = '0';
                            nodeEl.style.setProperty('width', '0', 'important');
                            nodeEl.style.position = 'absolute';
                            nodeEl.style.setProperty('position', 'absolute', 'important');
                            nodeEl.style.left = '-9999px';
                            nodeEl.style.setProperty('left', '-9999px', 'important');
                            nodeEl.style.pointerEvents = 'none';
                            nodeEl.style.setProperty('pointer-events', 'none', 'important');
                            nodeEl.setAttribute('data-transport-on-connection', 'true');
                            nodeEl.classList.add('epr-transport-hidden');
                        }
                    }
                }
            });
            
            // In Sankey mode, render differently (will be enhanced with data later)
            if (this.viewMode === 'sankey') {
                this.renderSankeyConnections();
                return;
            }
            
            // Flow mode - render standard connections
            // First, filter out any orphaned connections (where nodes don't exist)
            const validConnections = this.connections.filter(conn => {
                const fromNode = this.nodes.get(conn.from);
                const toNode = this.nodes.get(conn.to);
                return fromNode && toNode;
            });
            
            // Update connections array to remove orphaned ones
            if (validConnections.length !== this.connections.length) {
                this.connections = validConnections;
            }
            
            // Now render only valid connections
            validConnections.forEach(conn => {
                const fromNode = this.nodes.get(conn.from);
                const toNode = this.nodes.get(conn.to);
                
                // Double-check nodes still exist (defensive programming)
                if (!fromNode || !toNode) return;
                
                // Skip connections if either node is in a packaging group (items in groups are hidden)
                if ((fromNode.groupId && (fromNode.type === 'packaging' || fromNode.type === 'raw-material')) ||
                    (toNode.groupId && (toNode.type === 'packaging' || toNode.type === 'raw-material'))) {
                    return;
                }
                
                // Skip connections if either node is in a distribution group (items in groups are hidden)
                if ((fromNode.groupId && fromNode.type === 'distribution') ||
                    (toNode.groupId && toNode.type === 'distribution')) {
                    return;
                }
                
                // Skip connections that go through transport nodes if there's a direct connection
                // We want to render the direct connection and overlay transport circles, not separate lines
                // Skip connections TO transport nodes (we'll render the direct connection instead)
                if (toNode.type === 'transport') {
                    // Find what connects to this transport
                    const incomingConn = this.connections.find(c => c.to === conn.to);
                    // Find what this transport connects to
                    const outgoingConn = this.connections.find(c => c.from === conn.to);
                    if (incomingConn && outgoingConn) {
                        // Check if there's a direct connection between the original nodes
                        const directConn = this.connections.find(c => 
                            c.from === incomingConn.from && c.to === outgoingConn.to
                        );
                        if (directConn) {
                            // Skip this transport connection - the direct connection will be rendered instead
                            return;
                        }
                    }
                }
                // Skip connections FROM transport nodes if there's a direct connection
                if (fromNode.type === 'transport') {
                    // Find what connects to this transport
                    const incomingConn = this.connections.find(c => c.to === conn.from);
                    if (incomingConn) {
                        // Check if there's a direct connection from the source to the destination
                        const directConn = this.connections.find(c => 
                            c.from === incomingConn.from && c.to === conn.to
                        );
                        if (directConn) {
                            // Skip this transport connection - the direct connection will be rendered instead
                            return;
                        }
                    }
                }
                
                // Get port positions based on connection ports
                const fromPort = conn.fromPort || 'right';
                const toPort = conn.toPort || 'left';
                
                // Calculate port positions - connect from center of connector circles
                // Get actual node dimensions from DOM if available
                const fromNodeEl = document.querySelector(`[data-node-id="${conn.from}"]`);
                const toNodeEl = document.querySelector(`[data-node-id="${conn.to}"]`);
                
                const fromNodeWidth = fromNodeEl ? fromNodeEl.offsetWidth : 150;
                const fromNodeHeight = fromNodeEl ? fromNodeEl.offsetHeight : 80;
                const toNodeWidth = toNodeEl ? toNodeEl.offsetWidth : 150;
                const toNodeHeight = toNodeEl ? toNodeEl.offsetHeight : 80;
                
                const portRadius = 8; // Radius of connector circle (16px diameter / 2)
                
                // Connect from the center of the connector circle
                const startX = fromPort === 'right' ? fromNode.x + fromNodeWidth + portRadius : fromNode.x - portRadius;
                const startY = fromNode.y + fromNodeHeight / 2;
                const endX = toPort === 'right' ? toNode.x + toNodeWidth + portRadius : toNode.x - portRadius;
                const endY = toNode.y + toNodeHeight / 2;
                
                // Control points for smooth bezier curve (cubic bezier)
                const dx = endX - startX;
                const dy = endY - startY;
                const distance = Math.sqrt(dx * dx + dy * dy);
                const curvature = Math.min(distance * 0.3, 100); // Curvature factor
                
                // Control points positioned to create smooth S-curve
                const controlPoint1X = startX + Math.max(dx * 0.5, curvature);
                const controlPoint1Y = startY;
                const controlPoint2X = endX - Math.max(dx * 0.5, curvature);
                const controlPoint2Y = endY;
                
                // Create bezier curve path
                const path = document.createElementNS('http://www.w3.org/2000/svg', 'path');
                const pathData = `M ${startX} ${startY} C ${controlPoint1X} ${controlPoint1Y}, ${controlPoint2X} ${controlPoint2Y}, ${endX} ${endY}`;
                path.setAttribute('d', pathData);
                path.setAttribute('class', 'epr-connection-line');
                path.setAttribute('stroke', '#6c757d');
                path.setAttribute('stroke-width', '4'); // Thicker for easier clicking
                path.setAttribute('fill', 'none');
                path.setAttribute('stroke-linecap', 'round');
                
                // Make connection clickable to delete
                path.style.cursor = 'pointer';
                path.style.pointerEvents = 'stroke';
                path.setAttribute('data-from', conn.from);
                path.setAttribute('data-to', conn.to);
                
                const persistable = (
                    (fromNode.type === 'supplier-packaging' && toNode.type === 'product') ||
                    (fromNode.type === 'raw-material' && toNode.type === 'packaging') ||
                    (fromNode.type === 'packaging' && toNode.type === 'product') ||
                    (fromNode.type === 'product' && toNode.type === 'distribution')
                );
                path.setAttribute('data-persisted', persistable && fromNode.entityId && toNode.entityId ? 'true' : 'false');
                
                // Store connection info for deletion
                const connectionInfo = { from: conn.from, to: conn.to, fromPort: conn.fromPort, toPort: conn.toPort };
                
                path.addEventListener('click', (e) => {
                    e.stopPropagation();
                    e.preventDefault();
                    this.showConnectionModal(connectionInfo);
                });
                
                path.addEventListener('contextmenu', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    this.showConnectionModal(connectionInfo);
                });
                
                // Add hover effect
                path.addEventListener('mouseenter', () => {
                    path.setAttribute('stroke', '#dc3545');
                    path.setAttribute('stroke-width', '5');
                    path.setAttribute('marker-end', 'url(#arrowhead-hover)');
                });
                
                path.addEventListener('mouseleave', () => {
                    path.setAttribute('stroke', '#6c757d');
                    path.setAttribute('stroke-width', '4');
                    path.setAttribute('marker-end', 'url(#arrowhead)');
                });
                
                // Use SVG marker for arrow at the END of the connection (where it connects to the node)
                path.setAttribute('marker-end', 'url(#arrowhead)');
                
                this.connectionsLayer.appendChild(path);
                
                // Check for connection quantity (from "Add Quantity" or "Add Number" option)
                const connectionKey = `${conn.from}-${conn.to}`;
                const connectionQuantityData = fromNode.parameters?.connectionQuantities?.[connectionKey];
                const connectionQuantity = typeof connectionQuantityData === 'object' ? connectionQuantityData.value : connectionQuantityData;
                const connectionQuantityType = typeof connectionQuantityData === 'object' ? connectionQuantityData.type : null;
                const connectionQuantityUnit = typeof connectionQuantityData === 'object' ? connectionQuantityData.unit : null;
                
                // Calculate midpoint along the bezier curve for displaying circles
                // (control points are already calculated above)
                const t = 0.5;
                const t2 = t * t;
                const t3 = t2 * t;
                const mt = 1 - t;
                const mt2 = mt * mt;
                const mt3 = mt2 * mt;
                
                const midX = mt3 * startX + 3 * mt2 * t * controlPoint1X + 3 * mt * t2 * controlPoint2X + t3 * endX;
                const midY = mt3 * startY + 3 * mt2 * t * controlPoint1Y + 3 * mt * t2 * controlPoint2Y + t3 * endY;
                
                // Show connection quantity circle if set
                if (connectionQuantity && connectionQuantity > 0) {
                    const circleBg = document.createElementNS('http://www.w3.org/2000/svg', 'circle');
                    circleBg.setAttribute('cx', midX);
                    circleBg.setAttribute('cy', midY);
                    circleBg.setAttribute('r', '18');
                    circleBg.setAttribute('fill', '#17a2b8');
                    circleBg.setAttribute('stroke', 'white');
                    circleBg.setAttribute('stroke-width', '2');
                    circleBg.setAttribute('class', 'epr-connection-quantity-circle');
                    circleBg.setAttribute('data-connection', connectionKey);
                    circleBg.style.cursor = 'pointer';
                    this.connectionsLayer.appendChild(circleBg);
                    
                    // Format display - value and unit on separate lines if unit exists
                    let valueText = connectionQuantity.toString();
                    if (connectionQuantityType === 'quantity' && connectionQuantityUnit) {
                        // Format to 2 decimal places for quantity type
                        valueText = parseFloat(connectionQuantity).toFixed(2);
                    }
                    
                    // Create value text (always shown)
                    const text = document.createElementNS('http://www.w3.org/2000/svg', 'text');
                    text.setAttribute('x', midX);
                    text.setAttribute('y', midY - (connectionQuantityUnit && connectionQuantityUnit !== 'count' ? 4 : 0));
                    text.setAttribute('text-anchor', 'middle');
                    text.setAttribute('dominant-baseline', 'central');
                    text.setAttribute('fill', 'white');
                    text.setAttribute('font-size', '12');
                    text.setAttribute('font-weight', 'bold');
                    text.setAttribute('class', 'epr-connection-quantity-text');
                    text.textContent = valueText;
                    this.connectionsLayer.appendChild(text);
                    
                    // Create unit text on separate line if unit exists and is not 'count'
                    if (connectionQuantityUnit && connectionQuantityUnit !== 'count') {
                        const unitText = document.createElementNS('http://www.w3.org/2000/svg', 'text');
                        unitText.setAttribute('x', midX);
                        unitText.setAttribute('y', midY + 8);
                        unitText.setAttribute('text-anchor', 'middle');
                        unitText.setAttribute('dominant-baseline', 'central');
                        unitText.setAttribute('fill', 'white');
                        unitText.setAttribute('font-size', '9');
                        unitText.setAttribute('font-weight', 'normal');
                        unitText.setAttribute('class', 'epr-connection-quantity-unit');
                        unitText.textContent = connectionQuantityUnit;
                        this.connectionsLayer.appendChild(unitText);
                        
                        // Make unit text clickable too
                        unitText.addEventListener('click', (e) => {
                            e.stopPropagation();
                            const fromNode = this.nodes.get(conn.from);
                            const toNode = this.nodes.get(conn.to);
                            const isProductConnection = (fromNode && fromNode.type === 'product') || (toNode && toNode.type === 'product');
                            this.showQuantityModal({ from: conn.from, to: conn.to, fromPort: conn.fromPort, toPort: conn.toPort }, isProductConnection);
                        });
                    }
                    
                    // Make circle and value text clickable to open quantity modal for editing
                    const connectionInfo = { from: conn.from, to: conn.to, fromPort: conn.fromPort, toPort: conn.toPort };
                    const fromNode = this.nodes.get(conn.from);
                    const toNode = this.nodes.get(conn.to);
                    const isProductConnection = (fromNode && fromNode.type === 'product') || (toNode && toNode.type === 'product');
                    
                    circleBg.addEventListener('click', (e) => {
                        e.stopPropagation();
                        this.showQuantityModal(connectionInfo, isProductConnection);
                    });
                    text.addEventListener('click', (e) => {
                        e.stopPropagation();
                        this.showQuantityModal(connectionInfo, isProductConnection);
                    });
                }
                
                // Show transport nodes as a single compact box under connector line
                // Positioned 15px below quantity circle if it exists, otherwise below midpoint
                // Check if there's a transport node between these two nodes
                const transportNode = this.findTransportBetweenNodes(conn.from, conn.to);
                // Show transport box if transport node exists and has transport types
                // Note: showTransportNodes flag controls visibility toggle, but we always render if node exists
                if (transportNode && transportNode.transportTypes && transportNode.transportTypes.length > 0 && this.showTransportNodes) {
                    const transportTypes = transportNode.transportTypes || [];
                    if (transportTypes.length > 0) {
                        // Sort transport types alphabetically by type name
                        const sortedTransportTypes = [...transportTypes].sort((a, b) => {
                            const typeA = (a.type || '').toLowerCase();
                            const typeB = (b.type || '').toLowerCase();
                            return typeA.localeCompare(typeB);
                        });
                        
                        // Center horizontally between the two nodes
                        const transportX = midX;
                        
                        // Position vertically: under connector line, 15px below quantity circle if exists
                        // If no quantity circle, position below connection midpoint
                        const quantityCircleRadius = 18; // Radius of quantity circle
                        const verticalOffset = 15; // 15px below quantity circle
                        let transportY;
                        const hasQuantity = connectionQuantity && connectionQuantity > 0;
                        if (hasQuantity) {
                            transportY = midY + quantityCircleRadius + verticalOffset;
                        } else {
                            // No quantity circle - position below connection midpoint
                            transportY = midY + verticalOffset;
                        }
                        
                        // Calculate box dimensions - vertical layout (each transport type on new line)
                        // Each transport type needs: icon (10px) + gap (2px) + value (text width ~30px)
                        const itemHeight = 14; // Reduced height per transport type line
                        const boxPadding = 5; // 3px more padding (was 2px, now 5px)
                        const deleteBtnHeight = 12; // Height for delete button at bottom
                        const boxHeight = (sortedTransportTypes.length * itemHeight) + (boxPadding * 2) + deleteBtnHeight;
                        
                        // Calculate width: 50% of base width unless content needs more
                        const baseWidth = 100; // Base width for 50% calculation
                        const contentNeededWidth = Math.max(60, sortedTransportTypes.reduce((max, t) => {
                            // Estimate width needed: icon (10px) + gap (2px) + value text (~25px for distance)
                            return Math.max(max, 37);
                        }, 0));
                        const calculatedWidth = Math.max(contentNeededWidth, baseWidth * 0.5);
                        const boxWidth = Math.max(calculatedWidth, 60); // Minimum 60px
                        
                        const boxX = transportX - (boxWidth / 2);
                        const boxY = transportY;
                        
                        // Draw connecting line FIRST (so it appears behind the box)
                        // Draw line from quantity circle (or connection midpoint) to transport box
                        let lineStartX, lineStartY;
                        if (hasQuantity) {
                            // Connect from bottom of quantity circle
                            lineStartX = midX; // Center of quantity circle
                            lineStartY = midY + quantityCircleRadius; // Bottom of quantity circle
                        } else {
                            // Connect from connection midpoint if no quantity circle
                            lineStartX = midX; // Connection midpoint
                            lineStartY = midY; // Connection midpoint
                        }
                        
                        const lineEndX = transportX; // Center of transport box
                        const lineEndY = boxY; // Top of transport box
                        
                        // Create connecting line - ALWAYS draw it (required feature)
                        const connectingLine = document.createElementNS('http://www.w3.org/2000/svg', 'line');
                        connectingLine.setAttribute('x1', lineStartX);
                        connectingLine.setAttribute('y1', lineStartY);
                        connectingLine.setAttribute('x2', lineEndX);
                        connectingLine.setAttribute('y2', lineEndY);
                        connectingLine.setAttribute('stroke', '#6c757d'); // Grey line matching transport box
                        connectingLine.setAttribute('stroke-width', '1');
                        connectingLine.setAttribute('stroke-dasharray', '3,3'); // Dashed line for visual distinction
                        connectingLine.setAttribute('class', 'epr-quantity-transport-line');
                        connectingLine.style.pointerEvents = 'none'; // Don't interfere with clicks
                        this.connectionsLayer.appendChild(connectingLine);
                        
                        // Create compact rounded rectangle box - grey and less prominent (ALWAYS discreet)
                        const rectRadius = 4;
                        const roundedRect = document.createElementNS('http://www.w3.org/2000/svg', 'rect');
                        roundedRect.setAttribute('x', boxX);
                        roundedRect.setAttribute('y', boxY);
                        roundedRect.setAttribute('width', boxWidth);
                        roundedRect.setAttribute('height', boxHeight);
                        roundedRect.setAttribute('rx', rectRadius);
                        roundedRect.setAttribute('ry', rectRadius);
                        roundedRect.setAttribute('fill', '#f8f9fa'); // Light grey background
                        roundedRect.setAttribute('stroke', '#6c757d'); // Grey border
                        roundedRect.setAttribute('stroke-width', '1'); // 1 pixel border
                        roundedRect.setAttribute('class', 'epr-transport-box');
                        roundedRect.setAttribute('data-transport-id', transportNode.id);
                        roundedRect.style.cursor = 'pointer';
                        roundedRect.style.opacity = '0.85'; // More discrete - always discreet
                        this.connectionsLayer.appendChild(roundedRect);
                        
                        // Create foreignObject to embed HTML with Bootstrap Icons
                        const foreignObject = document.createElementNS('http://www.w3.org/2000/svg', 'foreignObject');
                        foreignObject.setAttribute('x', boxX);
                        foreignObject.setAttribute('y', boxY);
                        foreignObject.setAttribute('width', boxWidth);
                        foreignObject.setAttribute('height', boxHeight);
                        foreignObject.setAttribute('class', 'epr-transport-foreign');
                        
                        // Create HTML content with Bootstrap Icons - vertical layout
                        const transportContent = document.createElement('div');
                        transportContent.style.cssText = 'display: flex; flex-direction: column; padding: 5px; font-family: "Inter", -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif; overflow: visible; width: 100%; height: 100%; box-sizing: border-box;';
                        transportContent.setAttribute('data-transport-id', transportNode.id);
                        
                        // Create container for transport items
                        const itemsContainer = document.createElement('div');
                        itemsContainer.style.cssText = 'display: flex; flex-direction: column; flex: 1; gap: 1px;';
                        
                        // Add each transport type on a new line (icon + value) - sorted alphabetically
                        // IMPORTANT: Every line MUST have an icon, even if transport type is missing
                        sortedTransportTypes.forEach((transport, index) => {
                            // Get Bootstrap Icon class for transport type - ensure all types have icons
                            let iconClass = 'bi-truck'; // default truck
                            const typeLower = (transport.type || '').toLowerCase().trim();
                            
                            // More comprehensive icon matching - check exact matches first, then partial
                            // Handle all possible transport type values including empty strings
                            if (!typeLower || typeLower === '') {
                                // Empty type - default to truck icon
                                iconClass = 'bi-truck';
                            } else if (typeLower === 'air' || typeLower === 'airplane' || typeLower === 'plane' || 
                                      typeLower.includes('air') || typeLower.includes('plane')) {
                                iconClass = 'bi-airplane';
                            } else if (typeLower === 'ship' || typeLower === 'sea' || 
                                      typeLower.includes('ship') || typeLower.includes('sea')) {
                                iconClass = 'bi-ship'; // Use ship/boat icon for ship
                            } else if (typeLower === 'train' || typeLower === 'rail' || typeLower === 'railway' ||
                                      typeLower.includes('train') || typeLower.includes('rail')) {
                                iconClass = 'bi-train-front';
                            } else if (typeLower === 'van' || typeLower.includes('van')) {
                                iconClass = 'bi-car-front'; // Use car icon for van
                            } else if (typeLower === 'truck' || typeLower.includes('truck')) {
                                iconClass = 'bi-truck';
                            } else {
                                // If type doesn't match any known type, still show truck icon
                                // This ensures every line has an icon - NEVER skip the icon
                                iconClass = 'bi-truck';
                            }
                            
                            // Create transport item container (one per line)
                            const itemDiv = document.createElement('div');
                            itemDiv.style.cssText = 'display: flex; align-items: center; width: 100%; min-height: 14px;';
                            
                            // Create icon (precedes value) - grey color - ALWAYS present
                            const icon = document.createElement('i');
                            icon.className = `bi ${iconClass}`;
                            icon.style.cssText = 'font-size: 10px; color: #6c757d; margin-right: 2px; line-height: 1; flex-shrink: 0;'; // Grey icon, 2px gap
                            
                            // Create value text - regular weight, grey color
                            const valueSpan = document.createElement('span');
                            valueSpan.textContent = transport.distance || '0';
                            valueSpan.style.cssText = 'font-size: 9px; font-weight: normal; color: #6c757d; white-space: nowrap; line-height: 1; font-family: "Inter", sans-serif;';
                            
                            // ALWAYS append icon first, then value - every line has an icon
                            itemDiv.appendChild(icon);
                            itemDiv.appendChild(valueSpan);
                            itemsContainer.appendChild(itemDiv);
                        });
                        
                        transportContent.appendChild(itemsContainer);
                        
                        // Add delete button (like main nodes) - positioned at bottom center with 3px padding below
                        const deleteBtn = document.createElement('button');
                        deleteBtn.innerHTML = '<i class="bi bi-x-circle"></i>';
                        deleteBtn.style.cssText = 'background: none; border: none; color: #dc3545; cursor: pointer; padding: 0 0 3px 0; font-size: 10px; opacity: 0.7; display: flex; align-items: center; justify-content: center; flex-shrink: 0; line-height: 1; margin-top: auto; width: 100%;';
                        deleteBtn.title = 'Delete';
                        deleteBtn.onclick = (e) => {
                            e.stopPropagation();
                            e.preventDefault();
                            if (confirm('Are you sure you want to delete this transport node?')) {
                                this.deleteNode(transportNode.id);
                            }
                        };
                        deleteBtn.onmouseover = () => { deleteBtn.style.opacity = '1'; };
                        deleteBtn.onmouseout = () => { deleteBtn.style.opacity = '0.7'; };
                        
                        transportContent.appendChild(deleteBtn);
                        foreignObject.appendChild(transportContent);
                        this.connectionsLayer.appendChild(foreignObject);
                        
                        // Make entire box clickable to open parameters (except delete button)
                        const clickHandler = (e) => {
                            if (!e.target.closest('button')) {
                                e.stopPropagation();
                                this.selectNode(transportNode.id);
                            }
                        };
                        roundedRect.addEventListener('click', clickHandler);
                        transportContent.addEventListener('click', clickHandler);
                    }
                }
                
                // Add holds quantity circle for packaging group connections
                // Check if fromNode is a packaging group connecting to another packaging group or a product
                if (fromNode.type === 'packaging-group' && 
                    (toNode.type === 'packaging-group' || toNode.type === 'product')) {
                    const holdsQuantity = fromNode.parameters?.holdsQuantity || 0;
                    const holdsItemType = fromNode.parameters?.holdsItemType || '';
                    
                    // Check if this connection matches the holdsItemType
                    // If holdsItemType is not set, show quantity on all connections from this group
                    let shouldShowQuantity = false;
                    if (holdsItemType === '') {
                        // No item type selected - show quantity on all connections if quantity is set
                        shouldShowQuantity = holdsQuantity > 0;
                    } else if (toNode.type === 'packaging-group') {
                        // Check if holdsItemType matches this group
                        if (holdsItemType === `group-${toNode.id}`) {
                            shouldShowQuantity = true;
                        }
                    } else if (toNode.type === 'product') {
                        // Check if holdsItemType matches this product
                        if (holdsItemType === `product-${toNode.id}`) {
                            shouldShowQuantity = true;
                        }
                    }
                    
                    if (shouldShowQuantity && holdsQuantity > 0) {
                        // Calculate midpoint along the bezier curve (at t=0.5)
                        // For cubic bezier: B(t) = (1-t)³P₀ + 3(1-t)²tP₁ + 3(1-t)t²P₂ + t³P₃
                        // At t=0.5: B(0.5) = 0.125P₀ + 0.375P₁ + 0.375P₂ + 0.125P₃
                        const t = 0.5;
                        const t2 = t * t;
                        const t3 = t2 * t;
                        const mt = 1 - t;
                        const mt2 = mt * mt;
                        const mt3 = mt2 * mt;
                        
                        const midX = mt3 * startX + 3 * mt2 * t * controlPoint1X + 3 * mt * t2 * controlPoint2X + t3 * endX;
                        const midY = mt3 * startY + 3 * mt2 * t * controlPoint1Y + 3 * mt * t2 * controlPoint2Y + t3 * endY;
                        
                        // Create circle background
                        const circleBg = document.createElementNS('http://www.w3.org/2000/svg', 'circle');
                        circleBg.setAttribute('cx', midX);
                        circleBg.setAttribute('cy', midY);
                        circleBg.setAttribute('r', '18');
                        circleBg.setAttribute('fill', '#17a2b8');
                        circleBg.setAttribute('stroke', 'white');
                        circleBg.setAttribute('stroke-width', '2');
                        circleBg.setAttribute('class', 'epr-holds-quantity-circle');
                        this.connectionsLayer.appendChild(circleBg);
                        
                        // Create text for quantity
                        const text = document.createElementNS('http://www.w3.org/2000/svg', 'text');
                        text.setAttribute('x', midX);
                        text.setAttribute('y', midY);
                        text.setAttribute('text-anchor', 'middle');
                        text.setAttribute('dominant-baseline', 'central');
                        text.setAttribute('fill', 'white');
                        text.setAttribute('font-size', '12');
                        text.setAttribute('font-weight', 'bold');
                        text.setAttribute('class', 'epr-holds-quantity-text');
                        text.textContent = holdsQuantity.toString();
                        this.connectionsLayer.appendChild(text);
                    }
                }
            });
        }
        
        renderSankeyConnections() {
            // Placeholder for Sankey visualization
            // Will be enhanced later to use node parameters for flow values
            this.connections.forEach(conn => {
                const fromNode = this.nodes.get(conn.from);
                const toNode = this.nodes.get(conn.to);
                
                if (!fromNode || !toNode) return;
                
                const nodeWidth = 150;
                const nodeHeight = 80;
                
                const startX = fromNode.x + nodeWidth;
                const startY = fromNode.y + nodeHeight / 2;
                const endX = toNode.x;
                const endY = toNode.y + nodeHeight / 2;
                
                // Sankey-style connection (thicker, curved)
                const dx = endX - startX;
                const dy = endY - startY;
                const distance = Math.sqrt(dx * dx + dy * dy);
                const curvature = Math.min(distance * 0.4, 150);
                
                const controlPoint1X = startX + Math.max(dx * 0.5, curvature);
                const controlPoint1Y = startY;
                const controlPoint2X = endX - Math.max(dx * 0.5, curvature);
                const controlPoint2Y = endY;
                
                const path = document.createElementNS('http://www.w3.org/2000/svg', 'path');
                const pathData = `M ${startX} ${startY} C ${controlPoint1X} ${controlPoint1Y}, ${controlPoint2X} ${controlPoint2Y}, ${endX} ${endY}`;
                path.setAttribute('d', pathData);
                path.setAttribute('class', 'epr-connection-line sankey-connection');
                path.setAttribute('stroke', '#4a90e2');
                path.setAttribute('stroke-width', '4'); // Thicker for Sankey
                path.setAttribute('fill', 'none');
                path.setAttribute('stroke-linecap', 'round');
                path.setAttribute('opacity', '0.6');
                
                // Make connection clickable to delete
                path.style.cursor = 'pointer';
                path.setAttribute('data-from', conn.from);
                path.setAttribute('data-to', conn.to);
                
                path.addEventListener('click', (e) => {
                    e.stopPropagation();
                    if (confirm('Delete this connection?')) {
                        this.removeConnection(conn.from, conn.to);
                    }
                });
                
                path.addEventListener('contextmenu', (e) => {
                    e.preventDefault();
                    if (confirm('Delete this connection?')) {
                        this.removeConnection(conn.from, conn.to);
                    }
                });
                
                this.connectionsLayer.appendChild(path);
            });
        }
        
        toggleGrid() {
            this.showGrid = !this.showGrid;
            this.updateGridDisplay();
            console.log('[EPR Canvas] Grid toggled:', this.showGrid);
        }
        
        toggleSnapToGrid() {
            this.snapToGrid = !this.snapToGrid;
            const btn = document.getElementById('eprSnapToGridBtn');
            if (btn) {
                btn.classList.toggle('active', this.snapToGrid);
                // Change icon color when active
                const icon = btn.querySelector('i');
                if (icon) {
                    if (this.snapToGrid) {
                        icon.style.color = '#28a745'; // Green when active
                    } else {
                        icon.style.color = ''; // Default color when inactive
                    }
                }
            }
            console.log('[EPR Canvas] Snap to grid toggled:', this.snapToGrid);
        }
        
        updateGridDisplay() {
            if (!this.canvas) {
                console.warn('[EPR Canvas] Canvas not found for grid display update');
                return;
            }
            
            // Remove grid-visible class if grid should be hidden
            if (!this.showGrid) {
                this.canvas.classList.remove('grid-visible');
            } else {
                // Add grid-visible class if grid should be shown
                this.canvas.classList.add('grid-visible');
            }
            
            // Update button state
            const btn = document.getElementById('eprToggleGridBtn');
            if (btn) {
                btn.classList.toggle('active', this.showGrid);
            }
            
            console.log('[EPR Canvas] Grid display updated:', this.showGrid, 'Canvas ID:', this.canvas.id, 'Canvas classes:', this.canvas.className, 'Has grid-visible:', this.canvas.classList.contains('grid-visible'));
        }
        
        zoomIn() {
            this.zoomLevel = Math.min(this.zoomLevel + 0.1, 3);
            this.updateTransform();
        }
        
        zoomOut() {
            this.zoomLevel = Math.max(this.zoomLevel - 0.1, 0.5);
            this.updateTransform();
        }
        
        fitToCanvas() {
            if (this.nodes.size === 0) return;
            
            const nodes = Array.from(this.nodes.values());
            const minX = Math.min(...nodes.map(n => n.x));
            const maxX = Math.max(...nodes.map(n => n.x));
            const minY = Math.min(...nodes.map(n => n.y));
            const maxY = Math.max(...nodes.map(n => n.y));
            
            const width = maxX - minX + 300;
            const height = maxY - minY + 300;
            
            const canvasWidth = this.canvas.offsetWidth;
            const canvasHeight = this.canvas.offsetHeight;
            
            const scaleX = canvasWidth / width;
            const scaleY = canvasHeight / height;
            this.zoomLevel = Math.min(scaleX, scaleY, 1);
            
            this.panOffset.x = (canvasWidth - (maxX + minX) * this.zoomLevel) / 2;
            this.panOffset.y = (canvasHeight - (maxY + minY) * this.zoomLevel) / 2;
            
            this.updateTransform();
        }
        
        arrangeNodes() {
            // Organize nodes by type with priority: product -> packaging -> distribution
            const nodesByType = {
                'product': [],
                'supplier-packaging': [],
                'packaging': [],
                'packaging-group': [],
                'raw-material': [],
                'distribution': [],
                'transport': [],
                'other': []
            };
            
            this.nodes.forEach(node => {
                if (nodesByType[node.type]) {
                    nodesByType[node.type].push(node);
                } else {
                    nodesByType['other'].push(node);
                }
            });
            
            const startX = 100;
            const startY = 100;
            const columnSpacing = 250; // Equidistant spacing
            const rowSpacing = 120; // Equidistant vertical spacing
            
            let currentX = startX;
            
            // Flow left to right: product -> packaging -> distribution
            const typeOrder = ['product', 'supplier-packaging', 'packaging', 'packaging-group', 'raw-material', 'distribution', 'distribution-group', 'transport', 'other'];
            
            typeOrder.forEach(type => {
                const nodes = nodesByType[type];
                if (nodes.length === 0) return;
                
                // Calculate center Y position for this column to keep nodes centered
                const totalHeight = (nodes.length - 1) * rowSpacing;
                const centerY = startY + (totalHeight / 2);
                const startYForColumn = centerY - (totalHeight / 2);
                
                let currentY = startYForColumn;
                nodes.forEach((node) => {
                    node.x = currentX;
                    node.y = currentY;
                    currentY += rowSpacing;
                    this.renderNode(node);
                });
                
                currentX += columnSpacing;
            });
            
            this.updateConnections();
            this.saveState();
        }
        
        alignNodes(alignment) {
            // Get selected nodes - prioritize multi-select, then single select
            let selectedNodeIds = [];
            
            // Check multi-select first (Ctrl+Click)
            if (this.selectedNodes && this.selectedNodes.size > 0) {
                selectedNodeIds = Array.from(this.selectedNodes);
            }
            
            // If no multi-select, check single select
            if (selectedNodeIds.length === 0 && this.selectedNode) {
                selectedNodeIds = [this.selectedNode];
            }
            
            // Debug logging
            console.log('[EPR Canvas] alignNodes called:', {
                alignment,
                selectedNodes: this.selectedNodes,
                selectedNodesSize: this.selectedNodes?.size || 0,
                selectedNode: this.selectedNode,
                selectedNodeIds,
                selectedNodesArray: this.selectedNodes ? Array.from(this.selectedNodes) : []
            });
            
            if (selectedNodeIds.length < 2) {
                alert('Please select at least 2 nodes to align. Use Ctrl+Click to select multiple nodes.');
                return;
            }
            
            const nodes = selectedNodeIds.map(id => this.nodes.get(id)).filter(n => n);
            if (nodes.length < 2) {
                alert('Please select at least 2 nodes to align');
                return;
            }
            
            let targetValue;
            
            switch (alignment) {
                case 'left':
                    targetValue = Math.min(...nodes.map(n => n.x));
                    nodes.forEach(node => {
                        node.x = targetValue;
                        this.renderNode(node);
                    });
                    break;
                case 'right':
                    targetValue = Math.max(...nodes.map(n => n.x + 200)); // Default width 200
                    nodes.forEach(node => {
                        node.x = targetValue - 200;
                        this.renderNode(node);
                    });
                    break;
                case 'center':
                    const minX = Math.min(...nodes.map(n => n.x));
                    const maxX = Math.max(...nodes.map(n => n.x + 200));
                    targetValue = (minX + maxX) / 2;
                    nodes.forEach(node => {
                        node.x = targetValue - 100; // Half of default width
                        this.renderNode(node);
                    });
                    break;
                case 'top':
                    targetValue = Math.min(...nodes.map(n => n.y));
                    nodes.forEach(node => {
                        node.y = targetValue;
                        this.renderNode(node);
                    });
                    break;
                case 'bottom':
                    targetValue = Math.max(...nodes.map(n => n.y + 100)); // Default height 100
                    nodes.forEach(node => {
                        node.y = targetValue - 100;
                        this.renderNode(node);
                    });
                    break;
            }
            
            this.updateConnections();
            this.saveState();
        }
        
        distributeNodes(direction) {
            // Get selected nodes - prioritize multi-select, then single select
            let selectedNodeIds = [];
            
            // Check multi-select first (Ctrl+Click)
            if (this.selectedNodes && this.selectedNodes.size > 0) {
                selectedNodeIds = Array.from(this.selectedNodes);
            }
            
            // If no multi-select, check single select
            if (selectedNodeIds.length === 0 && this.selectedNode) {
                selectedNodeIds = [this.selectedNode];
            }
            
            // Debug logging
            console.log('[EPR Canvas] distributeNodes called:', {
                direction,
                selectedNodes: this.selectedNodes,
                selectedNodesSize: this.selectedNodes?.size || 0,
                selectedNode: this.selectedNode,
                selectedNodeIds,
                selectedNodesArray: this.selectedNodes ? Array.from(this.selectedNodes) : []
            });
            
            if (selectedNodeIds.length < 3) {
                alert('Please select at least 3 nodes to distribute. Use Ctrl+Click to select multiple nodes.');
                return;
            }
            
            const nodes = selectedNodeIds.map(id => this.nodes.get(id)).filter(n => n);
            if (nodes.length < 3) {
                alert('Please select at least 3 nodes to distribute');
                return;
            }
            
            const spacing = 100; // 100 pixels spacing
            
            if (direction === 'horizontal') {
                // Sort by X position
                nodes.sort((a, b) => a.x - b.x);
                const firstX = nodes[0].x;
                
                let currentX = firstX;
                nodes.forEach((node, index) => {
                    node.x = currentX;
                    this.renderNode(node);
                    currentX += 200 + spacing; // Default width 200 + spacing
                });
            } else if (direction === 'vertical') {
                // Sort by Y position
                nodes.sort((a, b) => a.y - b.y);
                const firstY = nodes[0].y;
                
                let currentY = firstY;
                nodes.forEach((node, index) => {
                    node.y = currentY;
                    this.renderNode(node);
                    currentY += 100 + spacing; // Default height 100 + spacing
                });
            }
            
            this.updateConnections();
            this.saveState();
        }
        
        saveState() {
            // Trigger autosave if enabled (every 2 minutes)
            if (window.EPRVisualEditor && window.EPRVisualEditor.autosaveEnabled) {
                // Clear any existing timeout
                if (this.autosaveTimeout) {
                    clearTimeout(this.autosaveTimeout);
                }
                // Set autosave to trigger after 2 minutes (120000ms)
                this.autosaveTimeout = setTimeout(() => {
                    if (window.EPRVisualEditor && window.EPRVisualEditor.autosave) {
                        window.EPRVisualEditor.autosave();
                    }
                }, 120000); // Autosave every 2 minutes
            }
            
            const state = {
                nodes: Array.from(this.nodes.values()),
                connections: [...this.connections]
            };
            
            this.history = this.history.slice(0, this.historyIndex + 1);
            this.history.push(JSON.parse(JSON.stringify(state)));
            this.historyIndex = this.history.length - 1;
            
            if (this.history.length > 50) {
                this.history.shift();
                this.historyIndex--;
            }
        }
        
        undo() {
            if (this.historyIndex > 0) {
                // Check if last action was moving item to group or removing from group
                const wasMoveToGroup = this.lastActionType === 'moveToGroup';
                const wasRemoveFromGroup = this.lastActionType === 'removeFromGroup';
                const removedFromGroupInfo = this.lastRemovedFromGroup;
                
                this.historyIndex--;
                const state = this.history[this.historyIndex];
                
                // Store current group memberships before restore
                const currentGroupMemberships = new Map();
                if (!wasMoveToGroup && !wasRemoveFromGroup) {
                    // If last action wasn't moveToGroup or removeFromGroup, preserve current group memberships
                    this.nodes.forEach((node, nodeId) => {
                        if (node.groupId && (node.type === 'packaging' || node.type === 'raw-material' || node.type === 'distribution')) {
                            currentGroupMemberships.set(nodeId, node.groupId);
                        }
                    });
                }
                
                this.restoreState(state);
                
                // If last action was removing from group, restore nodes back to the group
                if (wasRemoveFromGroup && removedFromGroupInfo) {
                    const groupNode = this.nodes.get(removedFromGroupInfo.groupId);
                    if (groupNode) {
                        removedFromGroupInfo.nodeIds.forEach(nodeId => {
                            const node = this.nodes.get(nodeId);
                            if (node && groupNode) {
                                // Restore group membership without triggering moveItemToGroup (to avoid action tracking)
                                if (!groupNode.containedItems) {
                                    groupNode.containedItems = [];
                                }
                                if (!groupNode.containedItems.includes(nodeId)) {
                                    groupNode.containedItems.push(nodeId);
                                }
                                node.groupId = removedFromGroupInfo.groupId;
                                
                                // Hide node visually
                                const itemNodeEl = document.querySelector(`[data-node-id="${nodeId}"]`);
                                if (itemNodeEl) {
                                    itemNodeEl.style.display = 'none';
                                    itemNodeEl.setAttribute('data-in-group', 'true');
                                }
                            }
                        });
                        
                        // Update group display
                        this.renderNode(groupNode);
                        this.updateConnections();
                    }
                }
                // Restore group memberships if last action wasn't moveToGroup or removeFromGroup
                else if (!wasMoveToGroup && !wasRemoveFromGroup) {
                    currentGroupMemberships.forEach((groupId, nodeId) => {
                        const node = this.nodes.get(nodeId);
                        const groupNode = this.nodes.get(groupId);
                        if (node && groupNode && (groupNode.type === 'packaging-group' || groupNode.type === 'distribution-group')) {
                            // Restore group membership without triggering moveItemToGroup (to avoid action tracking)
                            if (!groupNode.containedItems) {
                                groupNode.containedItems = [];
                            }
                            if (!groupNode.containedItems.includes(nodeId)) {
                                groupNode.containedItems.push(nodeId);
                            }
                            node.groupId = groupId;
                            
                            // Hide node visually
                            const itemNodeEl = document.querySelector(`[data-node-id="${nodeId}"]`);
                            if (itemNodeEl) {
                                itemNodeEl.style.display = 'none';
                                itemNodeEl.setAttribute('data-in-group', 'true');
                            }
                            
                            // Update group display
                            this.renderNode(groupNode);
                        }
                    });
                    this.updateConnections();
                }
                
                // Clear last action type after undo
                this.lastActionType = null;
                this.lastRemovedFromGroup = null;
            }
        }
        
        restoreState(state) {
            this.nodes.clear();
            this.connections = [];
            this.nodesLayer.innerHTML = '';
            this.connectionsLayer.innerHTML = '';
            
            state.nodes.forEach(node => {
                this.nodes.set(node.id, node);
                this.renderNode(node);
            });
            
            this.connections = state.connections;
            this.updateConnections();
        }
        
        getState() {
            return {
                nodes: Array.from(this.nodes.values()),
                connections: this.connections
            };
        }
        
        /**
         * Collect all nodes related to the given node(s) recursively
         * Includes connected nodes, group members, and all their relationships
         */
        collectRelatedNodes(nodeIds) {
            const collectedNodes = new Set();
            const collectedConnections = [];
            const nodeIdSet = new Set(Array.isArray(nodeIds) ? nodeIds : [nodeIds]);
            
            // Helper to add a node and its relationships
            const addNodeAndRelations = (nodeId) => {
                if (collectedNodes.has(nodeId)) return;
                
                const node = this.nodes.get(nodeId);
                if (!node) return;
                
                collectedNodes.add(nodeId);
                
                // If it's a packaging group, add all contained items
                if (node.type === 'packaging-group' && node.containedItems) {
                    node.containedItems.forEach(itemId => {
                        addNodeAndRelations(itemId);
                    });
                }
                
                // If it has a groupId, add the group
                if (node.groupId) {
                    addNodeAndRelations(node.groupId);
                }
                
                // Add all connections involving this node
                this.connections.forEach(conn => {
                    if (conn.from === nodeId || conn.to === nodeId) {
                        if (!collectedConnections.some(c => c.from === conn.from && c.to === conn.to)) {
                            collectedConnections.push(conn);
                        }
                        
                        // Recursively add connected nodes
                        if (conn.from === nodeId) {
                            addNodeAndRelations(conn.to);
                        } else {
                            addNodeAndRelations(conn.from);
                        }
                    }
                });
            };
            
            // Start collecting from initial nodes
            nodeIdSet.forEach(nodeId => {
                addNodeAndRelations(nodeId);
            });
            
            // Return nodes and connections with deep cloning to preserve all data including connectionQuantities
            const nodes = Array.from(collectedNodes).map(id => {
                const node = this.nodes.get(id);
                if (!node) return null;
                
                // Deep clone to avoid reference issues - this preserves connectionQuantities
                const clonedNode = JSON.parse(JSON.stringify(node));
                
                // Ensure parameters object exists and connectionQuantities are preserved
                if (!clonedNode.parameters) {
                    clonedNode.parameters = {};
                }
                
                // Explicitly preserve connectionQuantities if they exist
                if (node.parameters && node.parameters.connectionQuantities) {
                    clonedNode.parameters.connectionQuantities = JSON.parse(JSON.stringify(node.parameters.connectionQuantities));
                }
                
                // Explicitly preserve transportTypes if they exist (for transport nodes and distribution nodes)
                if (node.transportTypes) {
                    clonedNode.transportTypes = JSON.parse(JSON.stringify(node.transportTypes));
                }
                
                return clonedNode;
            }).filter(node => node !== null);
            
            return {
                nodes: nodes,
                connections: JSON.parse(JSON.stringify(collectedConnections))
            };
        }
        
        /**
         * Find a safe position for loading nodes that doesn't overlap existing nodes
         */
        findSafePosition(nodeCount, nodeWidth = 200, nodeHeight = 150) {
            const padding = 50;
            
            // Find the rightmost and bottommost existing nodes
            let maxX = 0;
            let maxY = 0;
            
            this.nodes.forEach(node => {
                if (node.x > maxX) maxX = node.x;
                if (node.y > maxY) maxY = node.y;
            });
            
            // Start position: right of existing nodes + padding
            const startX = maxX + padding + nodeWidth;
            const startY = Math.max(100, maxY);
            
            return { x: startX, y: startY };
        }
        
        /**
         * Load element(s) onto canvas with collision detection
         */
        loadElement(elementData) {
            const { nodes, connections } = elementData;
            
            if (!nodes || nodes.length === 0) {
                console.warn('[EPR Canvas] No nodes to load');
                return;
            }
            
            // Find safe starting position
            const startPos = this.findSafePosition(nodes.length);
            
            // Create ID mapping to avoid conflicts
            const idMap = new Map();
            nodes.forEach(node => {
                const newId = `node-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
                idMap.set(node.id, newId);
            });
            
            // Calculate offset to position nodes
            let minX = Infinity;
            let minY = Infinity;
            nodes.forEach(node => {
                if (node.x < minX) minX = node.x;
                if (node.y < minY) minY = node.y;
            });
            
            const offsetX = startPos.x - minX;
            const offsetY = startPos.y - minY;
            
            // Add nodes with new IDs and adjusted positions
            const addedNodes = new Map();
            nodes.forEach(node => {
                const newNode = {
                    ...node,
                    id: idMap.get(node.id),
                    x: node.x + offsetX,
                    y: node.y + offsetY
                };
                
                // Update groupId if it exists
                if (newNode.groupId && idMap.has(newNode.groupId)) {
                    newNode.groupId = idMap.get(newNode.groupId);
                }
                
                // Store original containedItems for later update
                const originalContainedItems = newNode.containedItems ? [...newNode.containedItems] : null;
                
                // Update containedItems IDs if it's a packaging group
                if (newNode.type === 'packaging-group' && newNode.containedItems) {
                    newNode.containedItems = newNode.containedItems.map(itemId => 
                        idMap.has(itemId) ? idMap.get(itemId) : itemId
                    );
                }
                
                // Update connectionQuantities keys to use new node IDs
                if (newNode.parameters && newNode.parameters.connectionQuantities) {
                    const updatedQuantities = {};
                    Object.keys(newNode.parameters.connectionQuantities).forEach(key => {
                        const [fromId, toId] = key.split('-');
                        const newFromId = idMap.has(fromId) ? idMap.get(fromId) : fromId;
                        const newToId = idMap.has(toId) ? idMap.get(toId) : toId;
                        const newKey = `${newFromId}-${newToId}`;
                        updatedQuantities[newKey] = newNode.parameters.connectionQuantities[key];
                    });
                    newNode.parameters.connectionQuantities = updatedQuantities;
                }
                
                this.addNode(newNode);
                addedNodes.set(newNode.id, { node: newNode, originalContainedItems });
            });
            
            // Update group nodes' containedItems after all nodes are added
            addedNodes.forEach(({ node, originalContainedItems }) => {
                if (node.type === 'packaging-group' && originalContainedItems) {
                    const groupNode = this.nodes.get(node.id);
                    if (groupNode) {
                        groupNode.containedItems = originalContainedItems.map(itemId => 
                            idMap.has(itemId) ? idMap.get(itemId) : itemId
                        );
                        // Re-render to update display
                        this.renderNode(groupNode);
                    }
                }
            });
            
            // Add connections with new IDs
            connections.forEach(conn => {
                const newFrom = idMap.get(conn.from);
                const newTo = idMap.get(conn.to);
                
                if (newFrom && newTo) {
                    this.addConnection(newFrom, newTo, conn.fromPort, conn.toPort);
                }
            });
            
            this.updateConnections();
            this.saveState();
        }
        
        loadState(state) {
            this.restoreState(state);
        }
        
        updatePlaceholder() {
            const placeholder = document.getElementById('eprCanvasPlaceholder');
            if (placeholder) {
                if (this.nodes.size > 0) {
                    placeholder.classList.add('hidden');
                } else {
                    placeholder.classList.remove('hidden');
                }
            }
        }
        
        escapeHtml(text) {
            const div = document.createElement('div');
            div.textContent = text;
            return div.innerHTML;
        }
        
        toggleNodeLock(nodeId) {
            const node = this.nodes.get(nodeId);
            if (!node) return;
            
            node.locked = !node.locked;
            this.renderNode(node);
            this.saveState();
        }
        
        getTypeLabel(type) {
            const labels = {
                'raw-material': 'Raw Material',
                'packaging': 'Packaging',
                'product': 'Product',
                'supplier-packaging': 'Supplier Packaging',
                'distribution': 'Distribution',
                'transport': 'Transport',
                'packaging-group': 'Packaging Group',
                'distribution-group': 'Distribution Group',
                'note': 'Note'
            };
            return labels[type] || type;
        }
    }
    
    // ============================================================================
    // TYPES TOOLBAR
    // ============================================================================
    class EPRTypesToolbar {
        constructor(canvasManager) {
            console.log('[EPR Types Toolbar] Creating TypesToolbar...');
            this.canvasManager = canvasManager;
            this.rawMaterials = [];
            this.packagingTypes = [];
            this.products = [];
            this.supplierPackaging = [];
            this.geographies = [];
            
            this.init();
        }
        
        async init() {
            console.log('[EPR Types Toolbar] Initializing...');
            
            // FIRST: Ensure seed data is available immediately (don't wait for API)
            this.ensureSeedData();
            console.log('[EPR Types Toolbar] Seed data ensured');
            
            // Render immediately with seed data
            this.renderRawMaterials();
            await this.renderPackagingDropdown();
            this.renderProductDropdown();
            this.renderSupplierPackagingDropdown();
            this.renderDistributionDropdowns();
            console.log('[EPR Types Toolbar] Initial render complete');
            
            // Setup event listeners immediately
            this.setupEventListeners();
            console.log('[EPR Types Toolbar] Event listeners attached');
            
            // THEN: Try to load data from API (non-blocking)
            try {
                await this.loadData();
                // Re-render with API data if available
                await this.renderPackagingDropdown();
                this.renderProductDropdown();
                this.renderSupplierPackagingDropdown();
                this.renderDistributionDropdowns();
                console.log('[EPR Types Toolbar] API data loaded and rendered');
            } catch (error) {
                console.warn('[EPR Types Toolbar] API load failed, using seed data:', error);
            }
            
            console.log('[EPR Types Toolbar] ✅ Fully initialized');
        }
        
        ensureSeedData() {
            // Always ensure we have seed data
            if (!this.packagingTypes || this.packagingTypes.length === 0) {
                console.log('[EPR Types Toolbar] Ensuring packaging seed data');
                this.packagingTypes = [
                    { id: 1, name: 'Cardboard Box', description: 'Standard cardboard box', height: 10, weight: 0.5, depth: 10, volume: 1000 },
                    { id: 2, name: 'Plastic Bottle', description: 'Plastic bottle container', height: 20, weight: 0.3, depth: 5, volume: 500 },
                    { id: 3, name: 'Glass Jar', description: 'Glass jar container', height: 15, weight: 0.8, depth: 8, volume: 750 },
                    { id: 4, name: 'Aluminum Can', description: 'Aluminum beverage can', height: 12, weight: 0.2, depth: 6, volume: 350 },
                    { id: 5, name: 'Plastic Bag', description: 'Plastic shopping bag', height: 30, weight: 0.1, depth: 2, volume: 200 }
                ];
            }
            
            if (!this.products || this.products.length === 0) {
                console.log('[EPR Types Toolbar] Ensuring product seed data');
                this.products = [
                    { id: 1, sku: 'PROD-001', name: 'Product A', description: 'Sample product A', size: 'Medium', weight: 1.0, height: 25, quantity: 100 },
                    { id: 2, sku: 'PROD-002', name: 'Product B', description: 'Sample product B', size: 'Large', weight: 2.0, height: 30, quantity: 50 },
                    { id: 3, sku: 'PROD-003', name: 'Product C', description: 'Sample product C', size: 'Small', weight: 0.5, height: 15, quantity: 200 },
                    { id: 4, sku: 'PROD-004', name: 'Product D', description: 'Sample product D', size: 'Extra Large', weight: 3.0, height: 40, quantity: 25 },
                    { id: 5, sku: 'PROD-005', name: 'Product E', description: 'Sample product E', size: 'Medium', weight: 1.5, height: 20, quantity: 150 }
                ];
            }
            
            if (!this.geographies || this.geographies.length === 0) {
                console.log('[EPR Types Toolbar] Ensuring geography seed data');
                this.geographies = [
                    { id: 1, name: 'United States', code: 'US', parentId: null },
                    { id: 2, name: 'United Kingdom', code: 'UK', parentId: null },
                    { id: 3, name: 'Canada', code: 'CA', parentId: null },
                    { id: 4, name: 'Germany', code: 'DE', parentId: null },
                    { id: 5, name: 'France', code: 'FR', parentId: null },
                    { id: 6, name: 'California', code: 'CA', parentId: 1 },
                    { id: 7, name: 'New York', code: 'NY', parentId: 1 },
                    { id: 8, name: 'Texas', code: 'TX', parentId: 1 },
                    { id: 9, name: 'England', code: 'ENG', parentId: 2 },
                    { id: 10, name: 'Scotland', code: 'SCT', parentId: 2 }
                ];
            }
        }
        
        async loadData() {
            try {
                const [materialsRes, packagingRes, productsRes, geographiesRes] = await Promise.all([
                    fetch('/api/visual-editor/raw-materials').catch(() => ({ ok: false })),
                    fetch('/api/visual-editor/packaging-types').catch(() => ({ ok: false })),
                    fetch('/api/visual-editor/products').catch(() => ({ ok: false })),
                    fetch('/api/visual-editor/geographies').catch(() => ({ ok: false }))
                ]);
                
                if (materialsRes.ok) {
                    this.rawMaterials = await materialsRes.json();
                    console.log('[EPR Types Toolbar] Loaded raw materials:', this.rawMaterials.length);
                } else {
                    // Seed default raw materials if API fails
                    this.rawMaterials = [
                        { id: 1, name: 'Glass', description: 'Glass material' },
                        { id: 2, name: 'Cardboard', description: 'Cardboard material' },
                        { id: 3, name: 'Plastic', description: 'Plastic material' },
                        { id: 4, name: 'Foil', description: 'Foil material' },
                        { id: 5, name: 'Plastic Wrap', description: 'Plastic wrap material' },
                        { id: 6, name: 'Sellotape', description: 'Sellotape material' }
                    ];
                    console.log('[EPR Types Toolbar] Using seeded raw materials');
                }
                
                // Try packaging library API first, then fallback to packaging-types
                const packagingLibraryRes = await fetch('/api/visual-editor/packaging-library').catch(() => ({ ok: false }));
                if (packagingLibraryRes.ok) {
                    const libraryItems = await packagingLibraryRes.json();
                    if (libraryItems && libraryItems.length > 0) {
                        this.packagingTypes = libraryItems.map(item => ({
                            id: item.id,
                            name: item.name,
                            description: item.name,
                            taxonomyCode: item.taxonomyCode,
                            weight: item.weight,
                            materialTaxonomyId: item.materialTaxonomyId
                        }));
                        console.log('[EPR Types Toolbar] Loaded packaging library items:', this.packagingTypes.length);
                    }
                } else if (packagingRes.ok) {
                    this.packagingTypes = await packagingRes.json();
                    console.log('[EPR Types Toolbar] Loaded packaging types:', this.packagingTypes.length);
                } else {
                    // Seed default packaging if API fails
                    this.packagingTypes = [
                        { id: 1, name: 'Cardboard Box', description: 'Standard cardboard box', height: 10, weight: 0.5, depth: 10, volume: 1000 },
                        { id: 2, name: 'Plastic Bottle', description: 'Plastic bottle container', height: 20, weight: 0.3, depth: 5, volume: 500 },
                        { id: 3, name: 'Glass Jar', description: 'Glass jar container', height: 15, weight: 0.8, depth: 8, volume: 750 }
                    ];
                    console.log('[EPR Types Toolbar] Using seeded packaging types');
                }
                
                if (productsRes.ok) {
                    this.products = await productsRes.json();
                    console.log('[EPR Types Toolbar] Loaded products:', this.products.length);
                } else {
                    // Seed default products if API fails
                    this.products = [
                        { id: 1, sku: 'PROD-001', name: 'Product A', description: 'Sample product A', size: 'Medium', weight: 1.0, height: 25, quantity: 100 },
                        { id: 2, sku: 'PROD-002', name: 'Product B', description: 'Sample product B', size: 'Large', weight: 2.0, height: 30, quantity: 50 },
                        { id: 3, sku: 'PROD-003', name: 'Product C', description: 'Sample product C', size: 'Small', weight: 0.5, height: 15, quantity: 200 }
                    ];
                    console.log('[EPR Types Toolbar] Using seeded products');
                }
                
                if (geographiesRes.ok) {
                    this.geographies = await geographiesRes.json();
                } else {
                    this.geographies = [];
                }
                
                const supplierPackagingRes = await fetch('/api/visual-editor/supplier-packaging').catch(() => ({ ok: false }));
                if (supplierPackagingRes.ok) {
                    this.supplierPackaging = await supplierPackagingRes.json();
                    console.log('[EPR Types Toolbar] Loaded supplier packaging:', this.supplierPackaging.length);
                } else {
                    this.supplierPackaging = [];
                }
            } catch (error) {
                console.error('[EPR Types Toolbar] Error loading data:', error);
                // Use seeded data on error
                this.rawMaterials = [
                    { id: 1, name: 'Glass', description: 'Glass material' },
                    { id: 2, name: 'Cardboard', description: 'Cardboard material' },
                    { id: 3, name: 'Plastic', description: 'Plastic material' }
                ];
                this.packagingTypes = [
                    { id: 1, name: 'Cardboard Box', description: 'Standard cardboard box' },
                    { id: 2, name: 'Plastic Bottle', description: 'Plastic bottle container' }
                ];
                this.products = [
                    { id: 1, sku: 'PROD-001', name: 'Product A', description: 'Sample product A' },
                    { id: 2, sku: 'PROD-002', name: 'Product B', description: 'Sample product B' }
                ];
            }
        }
        
        setupEventListeners() {
            console.log('[EPR Types Toolbar] Setting up event listeners...');
            
            // Raw materials buttons are set up in renderRawMaterials
            
            // Packaging - Unified toolbar
            const addPackagingBtn = document.getElementById('eprAddPackagingBtn');
            if (addPackagingBtn) {
                addPackagingBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    console.log('[EPR Types Toolbar] Add packaging clicked');
                    this.addSelectedPackaging();
                });
                console.log('[EPR Types Toolbar] Add packaging button handler attached');
            } else {
                console.warn('[EPR Types Toolbar] Add packaging button not found!');
            }
            
            // Packaging - Split panel
            const addPackagingBtnSplit = document.getElementById('eprAddPackagingBtnSplit');
            if (addPackagingBtnSplit) {
                addPackagingBtnSplit.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    console.log('[EPR Types Toolbar] Add packaging (split) clicked');
                    this.addSelectedPackaging();
                });
            }
            
            const newPackagingBtn = document.getElementById('eprNewPackagingBtn');
            if (newPackagingBtn) {
                newPackagingBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    console.log('[EPR Types Toolbar] New packaging clicked');
                    this.showNewPackagingModal();
                });
                console.log('[EPR Types Toolbar] New packaging button handler attached');
            } else {
                console.warn('[EPR Types Toolbar] New packaging button not found!');
            }
            
            // Packaging - Split panel New button
            const newPackagingBtnSplit = document.getElementById('eprNewPackagingBtnSplit');
            if (newPackagingBtnSplit) {
                newPackagingBtnSplit.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    console.log('[EPR Types Toolbar] New packaging (split) clicked');
                    this.showNewPackagingModal();
                });
            }
            
            const saveNewPackagingBtn = document.getElementById('eprSaveNewPackagingBtn');
            if (saveNewPackagingBtn) {
                saveNewPackagingBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    this.saveNewPackaging();
                });
            }
            
            // Packaging Groups
            const addPackagingGroupBtn = document.getElementById('eprAddPackagingGroupBtn');
            if (addPackagingGroupBtn) {
                addPackagingGroupBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    console.log('[EPR Types Toolbar] Add packaging group clicked');
                    this.addPackagingGroup();
                });
                console.log('[EPR Types Toolbar] Add packaging group button handler attached');
            } else {
                console.warn('[EPR Types Toolbar] Add packaging group button not found!');
            }
            
            // Packaging Groups - Split panel
            const addPackagingGroupBtnSplit = document.getElementById('eprAddPackagingGroupBtnSplit');
            if (addPackagingGroupBtnSplit) {
                addPackagingGroupBtnSplit.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    console.log('[EPR Types Toolbar] Add packaging group (split) clicked');
                    this.addPackagingGroup();
                });
            }
            
            // Load Packaging Group button
            const loadPackagingGroupBtn = document.getElementById('eprLoadPackagingGroupBtn');
            if (loadPackagingGroupBtn) {
                loadPackagingGroupBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    console.log('[EPR Types Toolbar] Load packaging group clicked');
                    this.showLoadPackagingGroupModal();
                });
            }
            
            // Load Packaging Group button - Split panel
            const loadPackagingGroupBtnSplit = document.getElementById('eprLoadPackagingGroupBtnSplit');
            if (loadPackagingGroupBtnSplit) {
                loadPackagingGroupBtnSplit.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    console.log('[EPR Types Toolbar] Load packaging group (split) clicked');
                    this.showLoadPackagingGroupModal();
                });
            }
            
            // Distribution Groups
            const addDistributionGroupBtn = document.getElementById('eprAddDistributionGroupBtn');
            if (addDistributionGroupBtn) {
                addDistributionGroupBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    console.log('[EPR Types Toolbar] Add distribution group clicked');
                    this.addDistributionGroup();
                });
                console.log('[EPR Types Toolbar] Add distribution group button handler attached');
            } else {
                console.warn('[EPR Types Toolbar] Add distribution group button not found!');
            }
            
            // Distribution Groups - Split panel
            const addDistributionGroupBtnSplit = document.getElementById('eprAddDistributionGroupBtnSplit');
            if (addDistributionGroupBtnSplit) {
                addDistributionGroupBtnSplit.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    console.log('[EPR Types Toolbar] Add distribution group (split) clicked');
                    this.addDistributionGroup();
                });
            }
            
            // Load Distribution Group button
            const loadDistributionGroupBtn = document.getElementById('eprLoadDistributionGroupBtn');
            if (loadDistributionGroupBtn) {
                loadDistributionGroupBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    console.log('[EPR Types Toolbar] Load distribution group clicked');
                    this.showLoadDistributionGroupModal();
                });
            }
            
            // Load Distribution Group button - Split panel
            const loadDistributionGroupBtnSplit = document.getElementById('eprLoadDistributionGroupBtnSplit');
            if (loadDistributionGroupBtnSplit) {
                loadDistributionGroupBtnSplit.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    console.log('[EPR Types Toolbar] Load distribution group (split) clicked');
                    this.showLoadDistributionGroupModal();
                });
            }
            
            // Products
            const addProductBtn = document.getElementById('eprAddProductBtn');
            if (addProductBtn) {
                addProductBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    console.log('[EPR Types Toolbar] Add product clicked');
                    this.addSelectedProduct();
                });
                console.log('[EPR Types Toolbar] Add product button handler attached');
            } else {
                console.warn('[EPR Types Toolbar] Add product button not found!');
            }
            
            // Supplier Packaging
            const addSupplierPackagingBtn = document.getElementById('eprAddSupplierPackagingBtn');
            if (addSupplierPackagingBtn) {
                addSupplierPackagingBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    this.addSelectedSupplierPackaging();
                });
            }
            
            // Product - Split panel
            const addProductBtnSplit = document.getElementById('eprAddProductBtnSplit');
            if (addProductBtnSplit) {
                addProductBtnSplit.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    console.log('[EPR Types Toolbar] Add product (split) clicked');
                    this.addSelectedProduct();
                });
            }
            
            const newProductBtn = document.getElementById('eprNewProductBtn');
            if (newProductBtn) {
                newProductBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    console.log('[EPR Types Toolbar] New product clicked');
                    this.showNewProductModal();
                });
                console.log('[EPR Types Toolbar] New product button handler attached');
            } else {
                console.warn('[EPR Types Toolbar] New product button not found!');
            }
            
            // Product - Split panel New button
            const newProductBtnSplit = document.getElementById('eprNewProductBtnSplit');
            if (newProductBtnSplit) {
                newProductBtnSplit.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    console.log('[EPR Types Toolbar] New product (split) clicked');
                    this.showNewProductModal();
                });
            }
            
            const saveNewProductBtn = document.getElementById('eprSaveNewProductBtn');
            if (saveNewProductBtn) {
                saveNewProductBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    this.saveNewProduct();
                });
            }
            
            // Distribution
            const addDistributionBtn = document.getElementById('eprAddDistributionBtn');
            if (addDistributionBtn) {
                addDistributionBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    console.log('[EPR Types Toolbar] Add distribution clicked');
                    this.addSelectedDistribution();
                });
                console.log('[EPR Types Toolbar] Add distribution button handler attached');
            } else {
                console.warn('[EPR Types Toolbar] Add distribution button not found!');
            }
            
            // Distribution - Split panel
            const addDistributionBtnSplit = document.getElementById('eprAddDistributionBtnSplit');
            if (addDistributionBtnSplit) {
                addDistributionBtnSplit.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    console.log('[EPR Types Toolbar] Add distribution (split) clicked');
                    this.addSelectedDistribution();
                });
            }
            
            const newLocationBtn = document.getElementById('eprNewLocationBtn');
            if (newLocationBtn) {
                newLocationBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    console.log('[EPR Types Toolbar] New location clicked');
                    this.showNewLocationModal();
                });
                console.log('[EPR Types Toolbar] New location button handler attached');
            } else {
                console.warn('[EPR Types Toolbar] New location button not found!');
            }
            
            // Distribution - Split panel New button
            const newLocationBtnSplit = document.getElementById('eprNewLocationBtnSplit');
            if (newLocationBtnSplit) {
                newLocationBtnSplit.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    console.log('[EPR Types Toolbar] New location (split) clicked');
                    this.showNewLocationModal();
                });
            }
            
            const saveNewLocationBtn = document.getElementById('eprSaveNewLocationBtn');
            if (saveNewLocationBtn) {
                saveNewLocationBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    this.saveNewLocation();
                });
            }
            
            // Notes
            const addNoteBtn = document.getElementById('eprAddNoteBtn');
            if (addNoteBtn) {
                addNoteBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    console.log('[EPR Types Toolbar] Add note clicked');
                    this.addNote();
                });
                console.log('[EPR Types Toolbar] Add note button handler attached');
            } else {
                console.warn('[EPR Types Toolbar] Add note button not found!');
            }
            
            // Country dropdown
            const countryDropdown = document.getElementById('eprCountryDropdown');
            if (countryDropdown) {
                countryDropdown.addEventListener('change', (e) => {
                    console.log('[EPR Types Toolbar] Country changed:', e.target.value);
                    this.onCountryChange(e.target.value);
                });
            }
            
            console.log('[EPR Types Toolbar] Event listeners setup complete');
        }
        
        async renderRawMaterials() {
            console.log('%c[EPR Types Toolbar] renderRawMaterials() CALLED', 'color: blue; font-weight: bold; font-size: 14px;');
            const container = document.getElementById('eprRawMaterialsButtons');
            if (!container) {
                console.error('%c[EPR Types Toolbar] ❌ Raw materials container not found!', 'color: red; font-weight: bold;');
                console.log('[EPR Types Toolbar] Available elements:', {
                    typesToolbar: !!document.getElementById('eprTypesToolbar'),
                    rawMaterialsSection: !!document.querySelector('[data-section="raw-materials"]'),
                    allContainers: document.querySelectorAll('[id*="Raw"]').length
                });
                // Retry after a short delay
                setTimeout(() => {
                    const retryContainer = document.getElementById('eprRawMaterialsButtons');
                    if (retryContainer) {
                        console.log('[EPR Types Toolbar] Retrying raw materials render...');
                        this.renderRawMaterials();
                    } else {
                        console.error('[EPR Types Toolbar] Container still not found after retry!');
                    }
                }, 200);
                return;
            }
            
            console.log('%c[EPR Types Toolbar] ✅ Container found! Loading raw materials from API...', 'color: green; font-weight: bold;');
            container.innerHTML = '<div style="padding: 0.5rem; text-align: center; color: #6c757d;">Loading...</div>';
            
            try {
                // Fetch Level 1 taxonomy items from API
                const response = await fetch('/api/visual-editor/raw-materials');
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                const materialTypes = await response.json();
                
                console.log('%c[EPR Types Toolbar] ✅ Loaded', 'color: green; font-weight: bold;', materialTypes.length, 'material types');
                container.innerHTML = '';
                
                // Define the 8 specific raw materials with their icons
                const requiredMaterials = [
                    { name: 'Composites & Multimaterial', iconClass: 'bi-layers' },
                    { name: 'Glass', iconClass: 'bi-circle' },
                    { name: 'Metals', iconClass: 'bi-gear' },
                    { name: 'Paper & Cardboard', iconClass: 'bi-file-text' },
                    { name: 'Plastics', iconClass: 'bi-circle-fill' },
                    { name: 'Textiles', iconClass: 'bi-grid' },
                    { name: 'Wood', iconClass: 'bi-square' },
                    { name: 'Other', iconClass: 'bi-three-dots' }
                ];
                
                // Filter and map materials from API to required list
                const filteredMaterials = requiredMaterials.map(reqMat => {
                    // Try to find matching material from API (case-insensitive, flexible matching)
                    const matched = materialTypes.find(m => {
                        const apiName = (m.name || '').toLowerCase().trim();
                        const reqName = reqMat.name.toLowerCase().trim();
                        return apiName === reqName || 
                               apiName.includes(reqName) || 
                               reqName.includes(apiName) ||
                               apiName.replace(/[^a-z0-9]/g, '') === reqName.replace(/[^a-z0-9]/g, '');
                    });
                    
                    if (matched) {
                        return {
                            ...matched,
                            name: reqMat.name, // Use standardized name
                            iconClass: matched.iconClass || reqMat.iconClass
                        };
                    }
                    
                    // Fallback: create material entry if not found in API
                    return {
                        id: null,
                        code: '',
                        name: reqMat.name,
                        description: reqMat.name,
                        iconClass: reqMat.iconClass
                    };
                });
                
                // Use filtered materials (either from API or fallback)
                let finalMaterialTypes = filteredMaterials;
                if (materialTypes.length === 0) {
                    console.log('[EPR Types Toolbar] No materials from API, using fallback defaults');
                }
                
                let buttonsCreated = 0;
                finalMaterialTypes.forEach(material => {
                    const btn = document.createElement('button');
                    const iconClass = material.iconClass || 'bi-circle';
                    const materialClass = `material-icon-${material.name.toLowerCase().replace(/\s+/g, '-')}`;
                    btn.className = `epr-material-btn ${materialClass}`;
                    btn.type = 'button'; // Prevent form submission
                    btn.draggable = true;
                    btn.setAttribute('data-material', material.name);
                    btn.setAttribute('data-taxonomy-id', material.id);
                    btn.setAttribute('data-taxonomy-code', material.code || '');
                    // Buttons will use CSS width (33.333% for 3 columns)
                    btn.innerHTML = `
                        <i class="bi ${iconClass}"></i>
                        <span>${material.name}</span>
                    `;
                    
                    // Raw Materials can be clicked or dragged to add to canvas
                    btn.style.cursor = 'pointer';
                    btn.title = `Add ${material.name} to canvas`;
                    
                    // Click handler - add raw material to canvas
                    btn.addEventListener('click', (e) => {
                        e.preventDefault();
                        e.stopPropagation();
                        this.addRawMaterial(material.name, iconClass, material.id, material.code || '');
                    });
                    
                    // Drag handler - allow dragging to canvas
                    btn.addEventListener('dragstart', (e) => {
                        const nodeData = {
                            type: 'raw-material',
                            name: material.name,
                            icon: iconClass,
                            taxonomyId: material.id,
                            taxonomyCode: material.code || ''
                        };
                        e.dataTransfer.setData('application/json', JSON.stringify(nodeData));
                        e.dataTransfer.effectAllowed = 'copy';
                        btn.classList.add('dragging');
                    });
                    
                    btn.addEventListener('dragend', () => {
                        btn.classList.remove('dragging');
                    });
                    
                    // Ensure icon is visible
                    const iconEl = btn.querySelector('i');
                    if (iconEl) {
                        iconEl.style.opacity = '1';
                        iconEl.style.display = 'inline-block';
                    }
                    
                    container.appendChild(btn);
                    buttonsCreated++;
                });
                
                console.log('%c[EPR Types Toolbar] ✅✅✅ Raw materials buttons rendered:', 'color: green; font-weight: bold; font-size: 14px;', buttonsCreated, 'buttons');
                console.log('[EPR Types Toolbar] Container now has', container.children.length, 'children');
                
                // Also render to split panel if it exists
                this.renderRawMaterialsToSplitPanel(finalMaterialTypes);
                
                // Store materials for later use
                this.rawMaterials = materialTypes;
            } catch (error) {
                console.error('[EPR Types Toolbar] Error loading raw materials:', error);
                // Fallback to the 8 required materials if API fails
                const defaultMaterials = [
                    { id: null, code: '', name: 'Composites & Multimaterial', description: 'Composites & Multimaterial', iconClass: 'bi-layers' },
                    { id: null, code: '', name: 'Glass', description: 'Glass', iconClass: 'bi-circle' },
                    { id: null, code: '', name: 'Metals', description: 'Metals', iconClass: 'bi-gear' },
                    { id: null, code: '', name: 'Paper & Cardboard', description: 'Paper & Cardboard', iconClass: 'bi-file-text' },
                    { id: null, code: '', name: 'Plastics', description: 'Plastics', iconClass: 'bi-circle-fill' },
                    { id: null, code: '', name: 'Textiles', description: 'Textiles', iconClass: 'bi-grid' },
                    { id: null, code: '', name: 'Wood', description: 'Wood', iconClass: 'bi-square' },
                    { id: null, code: '', name: 'Other', description: 'Other', iconClass: 'bi-three-dots' }
                ];
                
                container.innerHTML = '';
                defaultMaterials.forEach(material => {
                    const btn = document.createElement('button');
                    const iconClass = material.iconClass || 'bi-circle';
                    const materialClass = `material-icon-${material.name.toLowerCase().replace(/\s+/g, '-')}`;
                    btn.className = `epr-material-btn ${materialClass}`;
                    btn.type = 'button';
                    btn.draggable = true;
                    btn.setAttribute('data-material', material.name);
                    btn.setAttribute('data-taxonomy-id', material.id);
                    btn.setAttribute('data-taxonomy-code', material.code || '');
                    // Buttons will use CSS width (33.333% for 3 columns)
                    btn.innerHTML = `
                        <i class="bi ${iconClass}"></i>
                        <span>${material.name}</span>
                    `;
                    
                    // Raw Materials can be clicked or dragged to add to canvas
                    btn.style.cursor = 'pointer';
                    btn.title = `Add ${material.name} to canvas`;
                    
                    // Click handler - add raw material to canvas
                    btn.addEventListener('click', (e) => {
                        e.preventDefault();
                        e.stopPropagation();
                        this.addRawMaterial(material.name, iconClass, material.id, material.code || '');
                    });
                    
                    // Drag handler - allow dragging to canvas
                    btn.addEventListener('dragstart', (e) => {
                        const nodeData = {
                            type: 'raw-material',
                            name: material.name,
                            icon: iconClass,
                            taxonomyId: material.id,
                            taxonomyCode: material.code || ''
                        };
                        e.dataTransfer.setData('application/json', JSON.stringify(nodeData));
                        e.dataTransfer.effectAllowed = 'copy';
                        btn.classList.add('dragging');
                    });
                    
                    btn.addEventListener('dragend', () => {
                        btn.classList.remove('dragging');
                    });
                    
                    // Ensure icon is visible
                    const iconEl = btn.querySelector('i');
                    if (iconEl) {
                        iconEl.style.opacity = '1';
                        iconEl.style.display = 'inline-block';
                    }
                    
                    container.appendChild(btn);
                });
                
                // Also render to split panel
                const splitContainer = document.getElementById('eprRawMaterialsButtonsSplit');
                if (splitContainer) {
                    splitContainer.innerHTML = container.innerHTML;
                }
                
                // Store materials for later use
                this.rawMaterials = defaultMaterials;
            }
        }
        
        async renderRawMaterialsToSplitPanel(materialTypes = null) {
            const splitContainer = document.getElementById('eprRawMaterialsButtonsSplit');
            if (!splitContainer) return;
            
            // Use the same filtering logic as renderRawMaterials
            if (!materialTypes) {
                if (this.rawMaterials && this.rawMaterials.length > 0) {
                    materialTypes = this.rawMaterials;
                } else {
                    try {
                        const response = await fetch('/api/visual-editor/raw-materials');
                        if (response.ok) {
                            const apiMaterials = await response.json();
                            
                            // Define the 8 specific raw materials with their icons
                            const requiredMaterials = [
                                { name: 'Composites & Multimaterial', iconClass: 'bi-layers' },
                                { name: 'Glass', iconClass: 'bi-circle' },
                                { name: 'Metals', iconClass: 'bi-gear' },
                                { name: 'Paper & Cardboard', iconClass: 'bi-file-text' },
                                { name: 'Plastics', iconClass: 'bi-circle-fill' },
                                { name: 'Textiles', iconClass: 'bi-grid' },
                                { name: 'Wood', iconClass: 'bi-square' },
                                { name: 'Other', iconClass: 'bi-three-dots' }
                            ];
                            
                            // Filter and map materials from API to required list
                            materialTypes = requiredMaterials.map(reqMat => {
                                const matched = apiMaterials.find(m => {
                                    const apiName = (m.name || '').toLowerCase().trim();
                                    const reqName = reqMat.name.toLowerCase().trim();
                                    return apiName === reqName || 
                                           apiName.includes(reqName) || 
                                           reqName.includes(apiName) ||
                                           apiName.replace(/[^a-z0-9]/g, '') === reqName.replace(/[^a-z0-9]/g, '');
                                });
                                
                                if (matched) {
                                    return {
                                        ...matched,
                                        name: reqMat.name,
                                        iconClass: matched.iconClass || reqMat.iconClass
                                    };
                                }
                                
                                return {
                                    id: null,
                                    code: '',
                                    name: reqMat.name,
                                    description: reqMat.name,
                                    iconClass: reqMat.iconClass
                                };
                            });
                        } else {
                            // Fallback to the 8 required materials
                            materialTypes = [
                                { id: null, code: '', name: 'Composites & Multimaterial', description: 'Composites & Multimaterial', iconClass: 'bi-layers' },
                                { id: null, code: '', name: 'Glass', description: 'Glass', iconClass: 'bi-circle' },
                                { id: null, code: '', name: 'Metals', description: 'Metals', iconClass: 'bi-gear' },
                                { id: null, code: '', name: 'Paper & Cardboard', description: 'Paper & Cardboard', iconClass: 'bi-file-text' },
                                { id: null, code: '', name: 'Plastics', description: 'Plastics', iconClass: 'bi-circle-fill' },
                                { id: null, code: '', name: 'Textiles', description: 'Textiles', iconClass: 'bi-grid' },
                                { id: null, code: '', name: 'Wood', description: 'Wood', iconClass: 'bi-square' },
                                { id: null, code: '', name: 'Other', description: 'Other', iconClass: 'bi-three-dots' }
                            ];
                        }
                    } catch (error) {
                        console.error('[EPR Types Toolbar] Error loading materials for split panel:', error);
                        // Use defaults on error
                        materialTypes = [
                            { id: null, code: '', name: 'Composites & Multimaterial', description: 'Composites & Multimaterial', iconClass: 'bi-layers' },
                            { id: null, code: '', name: 'Glass', description: 'Glass', iconClass: 'bi-circle' },
                            { id: null, code: '', name: 'Metals', description: 'Metals', iconClass: 'bi-gear' },
                            { id: null, code: '', name: 'Paper & Cardboard', description: 'Paper & Cardboard', iconClass: 'bi-file-text' },
                            { id: null, code: '', name: 'Plastics', description: 'Plastics', iconClass: 'bi-circle-fill' },
                            { id: null, code: '', name: 'Textiles', description: 'Textiles', iconClass: 'bi-grid' },
                            { id: null, code: '', name: 'Wood', description: 'Wood', iconClass: 'bi-square' },
                            { id: null, code: '', name: 'Other', description: 'Other', iconClass: 'bi-three-dots' }
                        ];
                    }
                }
            }
            
            splitContainer.innerHTML = '';
            materialTypes.forEach(material => {
                const btn = document.createElement('button');
                const iconClass = material.iconClass || 'bi-circle';
                const materialClass = `material-icon-${material.name.toLowerCase().replace(/\s+/g, '-')}`;
                btn.className = `epr-material-btn ${materialClass}`;
                btn.type = 'button';
                btn.draggable = true;
                btn.setAttribute('data-material', material.name);
                btn.setAttribute('data-taxonomy-id', material.id);
                btn.setAttribute('data-taxonomy-code', material.code || '');
                // Buttons will use CSS width (33.333% for 3 columns)
                btn.innerHTML = `
                    <i class="bi ${iconClass}"></i>
                    <span>${material.name}</span>
                `;
                
                // Raw Materials can be clicked or dragged to add to canvas
                btn.style.cursor = 'pointer';
                btn.title = `Add ${material.name} to canvas`;
                
                // Click handler - add raw material to canvas
                btn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    this.addRawMaterial(material.name, iconClass, material.id, material.code || '');
                });
                
                // Drag handler - allow dragging to canvas
                btn.addEventListener('dragstart', (e) => {
                    const nodeData = {
                        type: 'raw-material',
                        name: material.name,
                        icon: iconClass,
                        taxonomyId: material.id,
                        taxonomyCode: material.code || ''
                    };
                    e.dataTransfer.setData('application/json', JSON.stringify(nodeData));
                    e.dataTransfer.effectAllowed = 'copy';
                    btn.classList.add('dragging');
                });
                
                btn.addEventListener('dragend', () => {
                    btn.classList.remove('dragging');
                });
                
                // Ensure icon is visible
                const iconEl = btn.querySelector('i');
                if (iconEl) {
                    iconEl.style.opacity = '1';
                    iconEl.style.display = 'inline-block';
                }
                
                splitContainer.appendChild(btn);
            });
            console.log('[EPR Types Toolbar] ✅ Raw materials rendered to split panel');
        }
        
        syncToSplitPanels() {
            // Copy raw materials buttons - use the same renderRawMaterials logic
            const splitButtons = document.getElementById('eprRawMaterialsButtonsSplit');
            if (splitButtons) {
                // Re-render raw materials to split panel using the same async method
                this.renderRawMaterialsToSplitPanel();
            }
            
            // Copy dropdowns
            const unifiedPackaging = document.getElementById('eprPackagingLibraryDropdown');
            const splitPackaging = document.getElementById('eprPackagingLibraryDropdownSplit');
            if (unifiedPackaging && splitPackaging) {
                splitPackaging.innerHTML = unifiedPackaging.innerHTML;
            }
            
            const unifiedProduct = document.getElementById('eprProductLibraryDropdown');
            const splitProduct = document.getElementById('eprProductLibraryDropdownSplit');
            if (unifiedProduct && splitProduct) {
                splitProduct.innerHTML = unifiedProduct.innerHTML;
            }
            
            const unifiedCountry = document.getElementById('eprCountryDropdown');
            const splitCountry = document.getElementById('eprCountryDropdownSplit');
            if (unifiedCountry && splitCountry) {
                splitCountry.innerHTML = unifiedCountry.innerHTML;
            }
            
            // Ensure Distribution Groups buttons have correct text
            const addDistGroupBtnSplit = document.getElementById('eprAddDistributionGroupBtnSplit');
            const loadDistGroupBtnSplit = document.getElementById('eprLoadDistributionGroupBtnSplit');
            if (addDistGroupBtnSplit) {
                addDistGroupBtnSplit.innerHTML = '<i class="bi bi-collection"></i> Add Network Distribution';
            }
            if (loadDistGroupBtnSplit) {
                loadDistGroupBtnSplit.innerHTML = '<i class="bi bi-folder-open"></i> Load Network Distribution';
            }
            
            // Packaging Groups panel doesn't need syncing as it only has a button
            // The button handler is already set up above
        }
        
        async addRawMaterial(name, icon, taxonomyId = null, taxonomyCode = null) {
            console.log('[EPR Types Toolbar] addRawMaterial called:', name, icon, taxonomyId, taxonomyCode);
            
            // Fetch DisplayName if taxonomyId is available
            let level1DisplayName = name;
            if (taxonomyId) {
                try {
                    const taxonomyResponse = await fetch(`/api/visual-editor/material-taxonomy/${taxonomyId}`);
                    if (taxonomyResponse.ok) {
                        const taxonomyData = await taxonomyResponse.json();
                        if (taxonomyData.displayName) {
                            level1DisplayName = taxonomyData.displayName;
                        }
                    }
                } catch (error) {
                    console.error('Error fetching taxonomy DisplayName:', error);
                }
            }
            
            // Don't provide x/y so it uses default positioning (150px to the right of last node)
            const node = this.canvasManager.addNode({
                type: 'raw-material',
                name: name,
                icon: icon,
                groupId: undefined, // Ensure no group association
                parameters: {
                    taxonomyId: taxonomyId,
                    taxonomyCode: taxonomyCode,
                    level1Classification: name,
                    level1DisplayName: level1DisplayName,
                    level2Classification: null,
                    level3Classification: null,
                    level4Classification: null,
                    level5Classification: null
                }
            });
            
            console.log('[EPR Types Toolbar] Raw material node created:', node.id, node.name);
            
            // Ensure node is visible
            const nodeEl = document.querySelector(`[data-node-id="${node.id}"]`);
            if (nodeEl) {
                nodeEl.style.display = '';
                nodeEl.removeAttribute('data-in-group');
                console.log('[EPR Types Toolbar] Raw material node element found and made visible');
            } else {
                console.error('[EPR Types Toolbar] Raw material node element not found!');
            }
            
            this.canvasManager.selectNode(node.id);
        }
        
        async renderPackagingDropdown() {
            const dropdown = document.getElementById('eprPackagingLibraryDropdown');
            const splitDropdown = document.getElementById('eprPackagingLibraryDropdownSplit');
            
            if (!dropdown) {
                console.warn('[EPR Types Toolbar] Packaging dropdown not found');
                return;
            }
            
            dropdown.innerHTML = '<option value="">Select packaging...</option>';
            if (splitDropdown) {
                splitDropdown.innerHTML = '<option value="">Select packaging...</option>';
            }
            
            try {
                // Fetch packaging library items from database
                const response = await fetch('/api/visual-editor/packaging-library');
                if (!response.ok) {
                    throw new Error('Failed to fetch packaging library');
                }
                const libraryItems = await response.json();
                
                if (libraryItems && libraryItems.length > 0) {
                    // Deduplicate library items by taxonomy (supplier products have unique ids 500000+)
                    const uniqueItems = new Map();
                    libraryItems.forEach(item => {
                        const key = item.id; // Use id - library and supplier items have distinct id ranges
                        uniqueItems.set(key, item);
                    });
                    
                    const uniqueItemsArray = Array.from(uniqueItems.values());
                    
                    this.packagingTypes = uniqueItemsArray.map(item => ({
                        id: item.id,
                        name: item.name,
                        description: item.name,
                        taxonomyCode: item.taxonomyCode,
                        weight: item.weight,
                        materialTaxonomyId: item.materialTaxonomyId
                    }));
                    
                    // Sort by name for better UX
                    uniqueItemsArray.sort((a, b) => {
                        const nameA = (a.name || '').toLowerCase();
                        const nameB = (b.name || '').toLowerCase();
                        return nameA.localeCompare(nameB);
                    });
                    
                    uniqueItemsArray.forEach(item => {
                        const option = document.createElement('option');
                        option.value = item.id;
                        option.textContent = item.name;
                        dropdown.appendChild(option);
                        
                        if (splitDropdown) {
                            const splitOption = option.cloneNode(true);
                            splitDropdown.appendChild(splitOption);
                        }
                    });
                    
                    console.log('[EPR Types Toolbar] Packaging dropdown rendered with', uniqueItemsArray.length, 'unique items from database');
                } else {
                    // Fallback to seed data if no database items
                    console.log('[EPR Types Toolbar] No packaging library items in database, using seed data');
                    this.packagingTypes = [
                        { id: 1, name: 'Cardboard Box', description: 'Standard cardboard box', height: 10, weight: 0.5, depth: 10, volume: 1000 },
                        { id: 2, name: 'Plastic Bottle', description: 'Plastic bottle container', height: 20, weight: 0.3, depth: 5, volume: 500 },
                        { id: 3, name: 'Glass Jar', description: 'Glass jar container', height: 15, weight: 0.8, depth: 8, volume: 750 },
                        { id: 4, name: 'Aluminum Can', description: 'Aluminum beverage can', height: 12, weight: 0.2, depth: 6, volume: 350 },
                        { id: 5, name: 'Plastic Bag', description: 'Plastic shopping bag', height: 30, weight: 0.1, depth: 2, volume: 200 }
                    ];
                    
                    this.packagingTypes.forEach(packaging => {
                        const option = document.createElement('option');
                        option.value = packaging.id;
                        option.textContent = packaging.name;
                        dropdown.appendChild(option);
                        
                        if (splitDropdown) {
                            const splitOption = option.cloneNode(true);
                            splitDropdown.appendChild(splitOption);
                        }
                    });
                }
            } catch (error) {
                console.error('[EPR Types Toolbar] Error loading packaging library:', error);
                // Fallback to seed data on error
                if (!this.packagingTypes || this.packagingTypes.length === 0) {
                    this.packagingTypes = [
                        { id: 1, name: 'Cardboard Box', description: 'Standard cardboard box', height: 10, weight: 0.5, depth: 10, volume: 1000 },
                        { id: 2, name: 'Plastic Bottle', description: 'Plastic bottle container', height: 20, weight: 0.3, depth: 5, volume: 500 },
                        { id: 3, name: 'Glass Jar', description: 'Glass jar container', height: 15, weight: 0.8, depth: 8, volume: 750 },
                        { id: 4, name: 'Aluminum Can', description: 'Aluminum beverage can', height: 12, weight: 0.2, depth: 6, volume: 350 },
                        { id: 5, name: 'Plastic Bag', description: 'Plastic shopping bag', height: 30, weight: 0.1, depth: 2, volume: 200 }
                    ];
                    
                    this.packagingTypes.forEach(packaging => {
                        const option = document.createElement('option');
                        option.value = packaging.id;
                        option.textContent = packaging.name;
                        dropdown.appendChild(option);
                        
                        if (splitDropdown) {
                            const splitOption = option.cloneNode(true);
                            splitDropdown.appendChild(splitOption);
                        }
                    });
                }
            }
            
            // Make dropdown draggable
            dropdown.draggable = true;
            dropdown.addEventListener('dragstart', async (e) => {
                const packagingId = parseInt(dropdown.value);
                if (!packagingId) {
                    e.preventDefault();
                    return;
                }
                
                try {
                    const res = await fetch(`/api/visual-editor/packaging-type/${packagingId}`);
                    const packaging = await res.json();
                    
                    const nodeData = {
                        type: 'packaging',
                        entityId: packaging.id,
                        name: packaging.name,
                        icon: 'bi-box-seam',
                        parameters: {
                            height: packaging.height,
                            weight: packaging.weight,
                            depth: packaging.depth,
                            volume: packaging.volume,
                            description: packaging.description
                        }
                    };
                    e.dataTransfer.setData('application/json', JSON.stringify(nodeData));
                    e.dataTransfer.effectAllowed = 'copy';
                } catch (error) {
                    console.error('[EPR Types Toolbar] Error loading packaging:', error);
                    e.preventDefault();
                }
            });
            
            dropdown.addEventListener('mousedown', (e) => {
                if (dropdown.value) {
                    dropdown.draggable = true;
                } else {
                    dropdown.draggable = false;
                }
            });
        }
        
        async addSelectedPackaging() {
            // Check both unified and split dropdowns
            let dropdown = document.getElementById('eprPackagingLibraryDropdown');
            if (!dropdown || !dropdown.value || dropdown.value === '') {
                dropdown = document.getElementById('eprPackagingLibraryDropdownSplit');
            }
            
            if (!dropdown) {
                console.error('[EPR Types Toolbar] Packaging dropdown not found');
                alert('Packaging dropdown not found');
                return;
            }
            
            const packagingId = parseInt(dropdown.value);
            console.log('[EPR Types Toolbar] Packaging dropdown value:', dropdown.value, 'parsed:', packagingId);
            
            if (!dropdown.value || dropdown.value === '' || !packagingId || isNaN(packagingId) || packagingId === 0) {
                alert('Please select a packaging item');
                return;
            }
            
            console.log('[EPR Types Toolbar] Adding packaging with ID:', packagingId);
            
            // Check if we have seed data
            const seedPackaging = this.packagingTypes.find(p => p.id === packagingId);
            if (seedPackaging) {
                console.log('[EPR Types Toolbar] Using seed packaging data');
                // Don't provide x/y so it uses default positioning (150px to the right of last node)
                const node = this.canvasManager.addNode({
                    type: 'packaging',
                    entityId: seedPackaging.id,
                    name: seedPackaging.name,
                    icon: 'bi-box-seam',
                    parameters: {
                        height: seedPackaging.height,
                        weight: seedPackaging.weight,
                        depth: seedPackaging.depth,
                        volume: seedPackaging.volume,
                        description: seedPackaging.description
                    }
                });
                this.canvasManager.selectNode(node.id);
                return;
            }
            
            try {
                const res = await fetch(`/api/visual-editor/packaging-type/${packagingId}`);
                if (!res.ok) {
                    throw new Error('Failed to fetch packaging');
                }
                const packaging = await res.json();
                
                // Don't provide x/y so it uses default positioning (150px to the right of last node)
                const node = this.canvasManager.addNode({
                    type: 'packaging',
                    entityId: packaging.id,
                    name: packaging.name,
                    icon: 'bi-box-seam',
                    parameters: {
                        height: packaging.height,
                        weight: packaging.weight,
                        depth: packaging.depth,
                        volume: packaging.volume,
                        description: packaging.description
                    }
                });
                
                if (packaging.materials && packaging.materials.length > 0) {
                    packaging.materials.forEach((material, index) => {
                        const materialNode = this.canvasManager.addNode({
                            type: 'raw-material',
                            entityId: material.id,
                            name: material.name,
                            icon: 'bi-circle',
                            x: node.x - 200,
                            y: node.y + (index * 100),
                        });
                        
                        this.canvasManager.addConnection(materialNode.id, node.id);
                    });
                }
                
                this.canvasManager.selectNode(node.id);
            } catch (error) {
                console.error('[EPR Types Toolbar] Error adding packaging:', error);
                alert('Error loading packaging item');
            }
        }
        
        showNewPackagingModal() {
            const modal = new bootstrap.Modal(document.getElementById('eprNewPackagingModal'));
            document.getElementById('eprNewPackagingName').value = '';
            document.getElementById('eprNewPackagingDescription').value = '';
            modal.show();
        }
        
        addPackagingGroup() {
            // Create a new packaging group node
            const groupName = prompt('Enter a name for the packaging group:', 'Packaging Group');
            if (!groupName || groupName.trim() === '') {
                return;
            }
            
            const node = this.canvasManager.addNode({
                type: 'packaging-group',
                name: groupName.trim(),
                icon: 'bi-collection',
                containedItems: [], // Array of node IDs contained in this group
                parameters: {
                    holdsQuantity: 0, // How many of the associated item this group holds
                    holdsItemType: '' // Type of item this group holds (e.g., 'packaging-group', 'packaging')
                }
            });
            
            this.canvasManager.selectNode(node.id);
            console.log('[EPR Types Toolbar] Created packaging group:', node.id);
        }
        
        escapeHtml(text) {
            if (!text) return '';
            const div = document.createElement('div');
            div.textContent = text;
            return div.innerHTML;
        }
        
        async showLoadPackagingGroupModal() {
            try {
                // Fetch packaging groups from API
                const response = await fetch('/api/visual-editor/packaging-groups');
                if (!response.ok) {
                    throw new Error('Failed to fetch packaging groups');
                }
                const groups = await response.json();
                
                if (groups.length === 0) {
                    alert('No packaging groups found in the database. Please import packaging library data first.');
                    return;
                }
                
                // Create modal HTML
                const modalHtml = `
                    <div class="modal fade" id="eprLoadPackagingGroupModal" tabindex="-1">
                        <div class="modal-dialog modal-lg">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h5 class="modal-title">Load Packaging Group</h5>
                                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                                </div>
                                <div class="modal-body">
                                    <div class="mb-3">
                                        <input type="text" class="form-control" id="eprPackagingGroupFilter" placeholder="Filter by name, layer, or items..." />
                                    </div>
                                    <p class="text-muted">Select a packaging group to load:</p>
                                    <div class="list-group" id="eprPackagingGroupList" style="max-height: 400px; overflow-y: auto;">
                                        ${groups.map(group => `
                                            <a href="#" class="list-group-item list-group-item-action epr-packaging-group-item" data-group-id="${group.id}" data-group-name="${this.escapeHtml(group.name).toLowerCase()}" data-group-layer="${group.packagingLayer ? this.escapeHtml(group.packagingLayer).toLowerCase() : ''}" data-group-items="${group.items && group.items.length > 0 ? group.items.map(i => this.escapeHtml(i.name).toLowerCase()).join(' ') : ''}">
                                                <div class="d-flex w-100 justify-content-between">
                                                    <h6 class="mb-1">${this.escapeHtml(group.name)}</h6>
                                                    <small>${group.items ? group.items.length : 0} items</small>
                                                </div>
                                                ${group.packagingLayer ? `<p class="mb-1"><small>Layer: ${this.escapeHtml(group.packagingLayer)}</small></p>` : ''}
                                                ${group.items && group.items.length > 0 ? `<p class="mb-1"><small>Items: ${group.items.map(i => this.escapeHtml(i.name)).join(', ')}</small></p>` : ''}
                                            </a>
                                        `).join('')}
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                                </div>
                            </div>
                        </div>
                    </div>
                `;
                
                // Remove existing modal if any
                const existingModal = document.getElementById('eprLoadPackagingGroupModal');
                if (existingModal) {
                    existingModal.remove();
                }
                
                // Add modal to body
                document.body.insertAdjacentHTML('beforeend', modalHtml);
                
                // Get modal element after it's added to DOM
                const modalElement = document.getElementById('eprLoadPackagingGroupModal');
                if (!modalElement) {
                    throw new Error('Failed to create modal element');
                }
                
                // Show modal
                const modal = new bootstrap.Modal(modalElement);
                modal.show();
                
                // Add filter functionality
                const filterInput = document.getElementById('eprPackagingGroupFilter');
                if (filterInput) {
                    filterInput.addEventListener('input', (e) => {
                        const filterText = e.target.value.toLowerCase();
                        const items = modalElement.querySelectorAll('.epr-packaging-group-item');
                        items.forEach(item => {
                            const name = item.dataset.groupName || '';
                            const layer = item.dataset.groupLayer || '';
                            const items = item.dataset.groupItems || '';
                            const matches = name.includes(filterText) || layer.includes(filterText) || items.includes(filterText);
                            item.style.display = matches ? '' : 'none';
                        });
                    });
                }
                
                // Handle group selection - use event delegation to avoid timing issues
                modalElement.addEventListener('click', async (e) => {
                    const listItem = e.target.closest('.list-group-item');
                    if (listItem) {
                        e.preventDefault();
                        e.stopPropagation();
                        const groupId = parseInt(listItem.dataset.groupId);
                        if (!isNaN(groupId)) {
                            modal.hide();
                            await this.loadPackagingGroup(groupId);
                        }
                    }
                });
                
                // Clean up modal when hidden
                modalElement.addEventListener('hidden.bs.modal', function() {
                    this.remove();
                });
            } catch (error) {
                console.error('[EPR Types Toolbar] Error loading packaging groups:', error);
                alert('Error loading packaging groups: ' + error.message);
            }
        }
        
        async loadPackagingGroup(groupId) {
            try {
                const response = await fetch(`/api/visual-editor/packaging-group/${groupId}`);
                if (!response.ok) {
                    throw new Error('Failed to fetch packaging group');
                }
                const group = await response.json();
                
                // Calculate starting position
                const nodeCount = this.canvasManager.nodes.size;
                const startX = 200 + (nodeCount % 5) * 250;
                const startY = 200 + Math.floor(nodeCount / 5) * 200;
                
                // Create packaging group node
                const groupNode = this.canvasManager.addNode({
                    type: 'packaging-group',
                    name: group.name,
                    icon: 'bi-collection',
                    x: startX,
                    y: startY,
                    containedItems: [],
                    parameters: {
                        packId: group.packId,
                        packagingLayer: group.packagingLayer,
                        style: group.style,
                        shape: group.shape,
                        size: group.size,
                        volumeDimensions: group.volumeDimensions,
                        coloursAvailable: group.coloursAvailable,
                        recycledContent: group.recycledContent,
                        totalPackWeight: group.totalPackWeight,
                        weightBasis: group.weightBasis,
                        notes: group.notes,
                        exampleReference: group.exampleReference,
                        source: group.source,
                        url: group.url
                    }
                });
                
                // Create packaging library items and raw materials
                const itemNodes = [];
                if (group.items && group.items.length > 0) {
                    for (let i = 0; i < group.items.length; i++) {
                        const item = group.items[i];
                        
                        // Create packaging library item node
                        const packagingNode = this.canvasManager.addNode({
                            type: 'packaging',
                            entityId: item.id,
                            name: item.name,
                            icon: 'bi-box-seam',
                            x: startX - 200,
                            y: startY + (i * 120),
                            parameters: {
                                taxonomyCode: item.taxonomyCode,
                                weight: item.weight,
                                materialTaxonomyId: item.materialTaxonomyId
                            }
                        });
                        
                        itemNodes.push(packagingNode);
                        
                        // Move packaging item into the group
                        this.canvasManager.moveItemToGroup(packagingNode.id, groupNode.id);
                        
                        // Store packaging node ID for nesting raw materials
                        const packagingNodeId = packagingNode.id;
                        
                        // Create raw material nodes - support multiple via rawMaterialIds, or single via materialTaxonomyId
                        const materialIdsToCreate = (item.rawMaterialIds && item.rawMaterialIds.length > 0)
                            ? item.rawMaterialIds
                            : (item.materialTaxonomyId ? [item.materialTaxonomyId] : []);
                        let rawMaterialCreated = false;
                        for (let mIdx = 0; mIdx < materialIdsToCreate.length; mIdx++) {
                            const matId = materialIdsToCreate[mIdx];
                            try {
                                const taxonomyResponse = await fetch(`/api/visual-editor/material-taxonomy/${matId}`);
                                if (taxonomyResponse.ok) {
                                    const taxonomy = await taxonomyResponse.json();
                                    const rawMaterialNode = this.canvasManager.addNode({
                                        type: 'raw-material',
                                        entityId: matId,
                                        name: taxonomy.displayName || item.name,
                                        icon: taxonomy.iconClass || 'bi-circle',
                                        x: startX - 400 - (mIdx * 50),
                                        y: startY + (i * 120) + (mIdx * 30),
                                        parameters: {
                                            taxonomyId: matId,
                                            taxonomyCode: taxonomy.code || item.taxonomyCode,
                                            level1Classification: taxonomy.code,
                                            weight: item.weight
                                        }
                                    });
                                    this.canvasManager.addConnection(rawMaterialNode.id, packagingNodeId);
                                    this.canvasManager.moveItemToGroup(rawMaterialNode.id, groupNode.id);
                                    const rawMaterialNodeData = this.canvasManager.nodes.get(rawMaterialNode.id);
                                    if (rawMaterialNodeData) rawMaterialNodeData.parentItemId = packagingNodeId;
                                    rawMaterialCreated = true;
                                }
                            } catch (error) {
                                console.warn('[EPR Types Toolbar] Could not load material taxonomy by ID:', error);
                            }
                        }
                        // If raw material wasn't created yet, try to find taxonomy by code
                        if (!rawMaterialCreated && item.taxonomyCode) {
                            const rawMaterialNode = await this.createRawMaterialFromTaxonomyCode(item.taxonomyCode, item.name, item.weight, startX - 400, startY + (i * 120), packagingNodeId);
                            if (rawMaterialNode) {
                                // Move raw material into the group (nested under packaging item)
                                this.canvasManager.moveItemToGroup(rawMaterialNode.id, groupNode.id);
                                
                                // Store parent item reference for proper nesting display
                                const rawMaterialNodeData = this.canvasManager.nodes.get(rawMaterialNode.id);
                                if (rawMaterialNodeData) {
                                    rawMaterialNodeData.parentItemId = packagingNodeId;
                                }
                            }
                        }
                    }
                }
                
                this.canvasManager.selectNode(groupNode.id);
                console.log('[EPR Types Toolbar] Loaded packaging group:', group.name, 'with', itemNodes.length, 'items');
            } catch (error) {
                console.error('[EPR Types Toolbar] Error loading packaging group:', error);
                alert('Error loading packaging group: ' + error.message);
            }
        }
        
        async createRawMaterialFromTaxonomyCode(taxonomyCode, itemName, weight, x, y, packagingNodeId) {
            if (!taxonomyCode) {
                console.warn('[EPR Types Toolbar] No taxonomy code provided for raw material');
                return;
            }
            
            try {
                // Try to find taxonomy by code - search all taxonomies
                const taxonomyResponse = await fetch('/api/visual-editor/raw-materials');
                if (taxonomyResponse.ok) {
                    const allTaxonomies = await taxonomyResponse.json();
                    
                    // Clean taxonomy code (remove weight if embedded)
                    let cleanCode = taxonomyCode.trim();
                    const weightMatch = cleanCode.match(/\s+(\d+\.?\d*)\s*g\s*$/i);
                    if (weightMatch) {
                        cleanCode = cleanCode.substring(0, weightMatch.index).trim();
                    }
                    
                    // Try exact match first
                    let taxonomy = allTaxonomies.find(t => t.code === cleanCode);
                    
                    // If not found, try prefix match (e.g., "PL.PET" for "PL.PET.BF.TR.CLEAR")
                    if (!taxonomy && cleanCode.includes('.')) {
                        const codeParts = cleanCode.split('.');
                        for (let prefixLen = codeParts.length - 1; prefixLen >= 1; prefixLen--) {
                            const prefixCode = codeParts.slice(0, prefixLen).join('.');
                            taxonomy = allTaxonomies.find(t => t.code === prefixCode);
                            if (taxonomy) {
                                console.log(`[EPR Types Toolbar] Matched taxonomy code '${cleanCode}' to '${prefixCode}'`);
                                break;
                            }
                        }
                    }
                    
                    if (taxonomy) {
                        const rawMaterialNode = this.canvasManager.addNode({
                            type: 'raw-material',
                            entityId: taxonomy.id,
                            name: taxonomy.name || itemName,
                            icon: taxonomy.iconClass || 'bi-circle',
                            x: x,
                            y: y,
                            parameters: {
                                taxonomyId: taxonomy.id,
                                taxonomyCode: taxonomy.code,
                                level1Classification: taxonomy.code,
                                weight: weight
                            }
                        });
                        
                        // Connect raw material to packaging item
                        this.canvasManager.addConnection(rawMaterialNode.id, packagingNodeId);
                        console.log(`[EPR Types Toolbar] Created raw material '${rawMaterialNode.name}' linked to packaging item`);
                    } else {
                        console.warn(`[EPR Types Toolbar] Could not find taxonomy for code '${cleanCode}'`);
                    }
                }
            } catch (error) {
                console.warn('[EPR Types Toolbar] Error creating raw material from taxonomy code:', error);
            }
        }
        
        addDistributionGroup() {
            // Create a new distribution group node
            const groupName = prompt('Enter a name for the distribution group:', 'Distribution Group');
            if (!groupName || groupName.trim() === '') {
                return;
            }
            
            const node = this.canvasManager.addNode({
                type: 'distribution-group',
                name: groupName.trim(),
                icon: 'bi-collection',
                containedItems: [] // Array of distribution node IDs contained in this group
            });
            
            this.canvasManager.selectNode(node.id);
            console.log('[EPR Types Toolbar] Created distribution group:', node.id);
        }
        
        async showLoadDistributionGroupModal() {
            try {
                // Fetch both distribution groups and packaging groups from API
                const [distResponse, packResponse] = await Promise.all([
                    fetch('/api/visual-editor/distribution-groups'),
                    fetch('/api/visual-editor/packaging-groups')
                ]);
                
                let distGroups = [];
                let packGroups = [];
                
                if (distResponse.ok) {
                    const distResult = await distResponse.json();
                    distGroups = distResult.success ? distResult.data : [];
                } else {
                    console.warn('[EPR Types Toolbar] Failed to fetch distribution groups:', distResponse.status);
                }
                
                if (packResponse.ok) {
                    const packResult = await packResponse.json();
                    // Packaging groups API returns array directly, not wrapped
                    packGroups = Array.isArray(packResult) ? packResult : [];
                    console.log('[EPR Types Toolbar] Loaded', packGroups.length, 'packaging groups');
                } else {
                    console.warn('[EPR Types Toolbar] Failed to fetch packaging groups:', packResponse.status);
                }
                
                if (distGroups.length === 0 && packGroups.length === 0) {
                    alert('No groups found in the database.');
                    return;
                }
                
                // Create modal HTML with tabs or sections for both types
                const modalHtml = `
                    <div class="modal fade" id="eprLoadDistributionGroupModal" tabindex="-1">
                        <div class="modal-dialog modal-lg">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h5 class="modal-title">Load Network Distribution</h5>
                                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                                </div>
                                <div class="modal-body">
                                    <div class="mb-3">
                                        <input type="text" class="form-control" id="eprDistributionGroupFilter" placeholder="Filter by name..." />
                                    </div>
                                    
                                    ${distGroups.length > 0 ? `
                                        <div class="mb-3">
                                            <h6 class="text-muted mb-2">Distribution Groups</h6>
                                            <div class="list-group" id="eprDistributionGroupList" style="max-height: 300px; overflow-y: auto;">
                                                ${distGroups.map(group => `
                                                    <a href="#" class="list-group-item list-group-item-action epr-group-item" data-group-type="distribution" data-group-id="${group.id}" data-group-key="${group.key || ''}" data-group-name="${this.escapeHtml(group.name || '').toLowerCase()}">
                                                        <div class="d-flex w-100 justify-content-between">
                                                            <h6 class="mb-1">
                                                                <i class="bi bi-collection me-2"></i>
                                                                ${this.escapeHtml(group.name || 'Unnamed Group')}
                                                            </h6>
                                                            <small>${group.createdAt ? new Date(group.createdAt).toLocaleDateString() : ''}</small>
                                                        </div>
                                                        ${group.key ? `<p class="mb-1"><small>Key: ${this.escapeHtml(group.key)}</small></p>` : ''}
                                                    </a>
                                                `).join('')}
                                            </div>
                                        </div>
                                    ` : ''}
                                    
                                    ${packGroups.length > 0 ? `
                                        <div class="mb-3">
                                            <h6 class="text-muted mb-2">Packaging Groups</h6>
                                            <div class="list-group" id="eprPackagingGroupList" style="max-height: 300px; overflow-y: auto;">
                                                ${packGroups.map(group => `
                                                    <a href="#" class="list-group-item list-group-item-action epr-group-item" data-group-type="packaging" data-group-id="${group.id}" data-group-name="${this.escapeHtml(group.name || '').toLowerCase()}" data-group-layer="${group.packagingLayer ? this.escapeHtml(group.packagingLayer).toLowerCase() : ''}" data-group-items="${group.items && group.items.length > 0 ? group.items.map(i => this.escapeHtml(i.name).toLowerCase()).join(' ') : ''}">
                                                        <div class="d-flex w-100 justify-content-between">
                                                            <h6 class="mb-1">
                                                                <i class="bi bi-box-seam me-2"></i>
                                                                ${this.escapeHtml(group.name)}
                                                            </h6>
                                                            <small>${group.items ? group.items.length : 0} items</small>
                                                        </div>
                                                        ${group.packagingLayer ? `<p class="mb-1"><small>Layer: ${this.escapeHtml(group.packagingLayer)}</small></p>` : ''}
                                                        ${group.items && group.items.length > 0 ? `<p class="mb-1"><small>Items: ${group.items.map(i => this.escapeHtml(i.name)).join(', ')}</small></p>` : ''}
                                                    </a>
                                                `).join('')}
                                            </div>
                                        </div>
                                    ` : ''}
                                </div>
                                <div class="modal-footer">
                                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                                </div>
                            </div>
                        </div>
                    </div>
                `;
                
                // Remove existing modal if any
                const existingModal = document.getElementById('eprLoadDistributionGroupModal');
                if (existingModal) {
                    existingModal.remove();
                }
                
                // Add modal to body
                document.body.insertAdjacentHTML('beforeend', modalHtml);
                
                // Get modal element after it's added to DOM
                const modalElement = document.getElementById('eprLoadDistributionGroupModal');
                if (!modalElement) {
                    throw new Error('Failed to create modal element');
                }
                
                // Show modal
                const modal = new bootstrap.Modal(modalElement);
                modal.show();
                
                // Add filter functionality
                const filterInput = document.getElementById('eprDistributionGroupFilter');
                if (filterInput) {
                    filterInput.addEventListener('input', (e) => {
                        const filterText = e.target.value.toLowerCase();
                        const items = modalElement.querySelectorAll('.epr-group-item');
                        items.forEach(item => {
                            const name = item.dataset.groupName || '';
                            const layer = item.dataset.groupLayer || '';
                            const items = item.dataset.groupItems || '';
                            const matches = name.includes(filterText) || layer.includes(filterText) || items.includes(filterText);
                            item.style.display = matches ? '' : 'none';
                        });
                    });
                }
                
                // Handle group selection - use event delegation to avoid timing issues
                modalElement.addEventListener('click', async (e) => {
                    const listItem = e.target.closest('.epr-group-item');
                    if (listItem) {
                        e.preventDefault();
                        e.stopPropagation();
                        const groupType = listItem.dataset.groupType;
                        const groupId = parseInt(listItem.dataset.groupId);
                        if (!isNaN(groupId)) {
                            modal.hide();
                            if (groupType === 'packaging') {
                                await this.loadPackagingGroup(groupId);
                            } else {
                                await this.loadDistributionGroup(groupId);
                            }
                        }
                    }
                });
                
                // Clean up modal when hidden
                modalElement.addEventListener('hidden.bs.modal', function() {
                    this.remove();
                });
            } catch (error) {
                console.error('[EPR Types Toolbar] Error loading distribution groups:', error);
                alert('Error loading distribution groups: ' + error.message);
            }
        }
        
        async loadDistributionGroup(projectId) {
            try {
                // Show loading indicator with progress bar
                const loadingIndicator = document.createElement('div');
                loadingIndicator.id = 'eprLoadingIndicator';
                loadingIndicator.style.cssText = 'position: fixed; top: 50%; left: 50%; transform: translate(-50%, -50%); background: rgba(0,0,0,0.9); color: white; padding: 30px 50px; border-radius: 12px; z-index: 10000; font-size: 16px; min-width: 400px; box-shadow: 0 4px 20px rgba(0,0,0,0.5);';
                loadingIndicator.innerHTML = `
                    <div style="text-align: center; margin-bottom: 20px;">
                        <div style="font-size: 18px; font-weight: bold; margin-bottom: 10px;">Loading Distribution Group</div>
                        <div id="eprLoadingStatus" style="font-size: 14px; color: #ccc; margin-bottom: 15px;">Initializing...</div>
                        <div style="width: 100%; background: rgba(255,255,255,0.2); border-radius: 10px; height: 24px; overflow: hidden;">
                            <div id="eprLoadingProgress" style="width: 0%; background: linear-gradient(90deg, #28a745, #20c997); height: 100%; transition: width 0.3s ease; display: flex; align-items: center; justify-content: center; color: white; font-size: 12px; font-weight: bold;">0%</div>
                        </div>
                    </div>
                `;
                document.body.appendChild(loadingIndicator);
                
                const updateProgress = (percent, status) => {
                    const progressBar = document.getElementById('eprLoadingProgress');
                    const statusText = document.getElementById('eprLoadingStatus');
                    if (progressBar) {
                        progressBar.style.width = percent + '%';
                        progressBar.textContent = Math.round(percent) + '%';
                    }
                    if (statusText && status) {
                        statusText.textContent = status;
                    }
                };
                
                updateProgress(10, 'Fetching project data...');
                const response = await fetch(`/api/visual-editor/project/${projectId}`);
                if (!response.ok) {
                    const errorText = await response.text();
                    throw new Error(`Failed to fetch distribution group: ${response.status} ${response.statusText}`);
                }
                updateProgress(20, 'Parsing project data...');
                const result = await response.json();
                
                if (!result.success || !result.data) {
                    throw new Error('Invalid project data: ' + (result.message || 'No data returned'));
                }
                
                const projectData = result.data;
                
                // Parse the project JSON - handle different response formats
                let projectJson;
                if (typeof projectData === 'string') {
                    // If data is a string, parse it
                    projectJson = JSON.parse(projectData);
                } else if (projectData.projectDataJson) {
                    // If data has projectDataJson property, parse that
                    projectJson = JSON.parse(projectData.projectDataJson);
                } else if (projectData.nodes && Array.isArray(projectData.nodes)) {
                    // If data already has nodes array, use it directly
                    projectJson = projectData;
                } else {
                    // Try to extract from nested structure
                    projectJson = projectData;
                }
                
                // If we still don't have nodes, try to get them from the parsed JSON elements
                if (!projectJson.nodes || !Array.isArray(projectJson.nodes)) {
                    // Check if nodes are in a different format (JsonElement)
                    if (projectData.nodes && typeof projectData.nodes === 'object') {
                        // Convert JsonElement to array
                        try {
                            projectJson.nodes = JSON.parse(JSON.stringify(projectData.nodes));
                        } catch (e) {
                            console.error('[EPR Types Toolbar] Error parsing nodes:', e);
                        }
                    }
                    if (projectData.connections && typeof projectData.connections === 'object') {
                        try {
                            projectJson.connections = JSON.parse(JSON.stringify(projectData.connections));
                        } catch (e) {
                            console.error('[EPR Types Toolbar] Error parsing connections:', e);
                        }
                    }
                }
                
                // Final check if this is a valid project with nodes
                if (!projectJson.nodes || !Array.isArray(projectJson.nodes)) {
                    console.error('[EPR Types Toolbar] Invalid project format - nodes:', projectJson.nodes);
                    throw new Error('Invalid project format: missing nodes array. Project may be empty or corrupted.');
                }
                
                updateProgress(30, 'Preparing node layout...');
                
                // Calculate starting position - find or create distribution group position
                const nodeCount = this.canvasManager.nodes.size;
                const startX = 200 + (nodeCount % 5) * 250;
                const startY = 200 + Math.floor(nodeCount / 5) * 200;
                
                // Load nodes from project (OPTIMIZED - batch creation)
                const loadedNodes = [];
                const nodeIdMap = new Map(); // Map old node IDs to new node IDs
                
                // Temporarily disable state saving during batch operations
                const originalSaveState = this.canvasManager.saveState;
                this.canvasManager.saveState = () => {}; // Disable during batch
                
                // First pass: create all nodes (batch mode - no individual renders)
                // Find the distribution group first to position nodes relative to it
                const distGroupData = projectJson.nodes.find(n => n.type === 'distribution-group');
                
                // Build a map of node hierarchy to position nodes relative to their parents
                const nodeHierarchy = new Map(); // nodeId -> parentId
                projectJson.nodes.forEach(nodeData => {
                    if (nodeData.groupId) {
                        nodeHierarchy.set(nodeData.id, nodeData.groupId);
                    }
                });
                
                // Helper to find root parent (distribution group)
                const findRootParent = (nodeId) => {
                    let currentId = nodeId;
                    while (nodeHierarchy.has(currentId)) {
                        currentId = nodeHierarchy.get(currentId);
                        const parentData = projectJson.nodes.find(n => n.id === currentId);
                        if (parentData && parentData.type === 'distribution-group') {
                            return currentId;
                        }
                    }
                    return null;
                };
                
                const nodesToCreate = projectJson.nodes.map((nodeData, index) => {
                    let nodeX = nodeData.x ? nodeData.x + startX : startX + (index * 250);
                    let nodeY = nodeData.y ? nodeData.y + startY : startY;
                    
                    // If this node is part of the distribution group hierarchy, position it inside the group
                    const rootParentId = findRootParent(nodeData.id);
                    if (rootParentId === distGroupData?.id) {
                        // This node is part of the distribution group - position it relative to the group
                        // Use a layout that shows hierarchy: manufacturer at top, hubs below, stores below hubs
                        const depth = nodeData.type === 'distribution-group' ? 0 :
                                     (nodeData.groupId === distGroupData?.id ? 1 : 
                                     (nodeHierarchy.has(nodeData.id) ? 2 : 3));
                        
                        // Position nodes in a grid layout inside the group
                        // Use startX/startY as fallback - actual positioning will be done later
                        const offsetX = startX + 50 + ((depth - 1) * 200);
                        const offsetY = startY + 100 + ((index % 10) * 80);
                        nodeX = offsetX;
                        nodeY = offsetY;
                    }
                    
                    return {
                        ...nodeData,
                        x: nodeX,
                        y: nodeY
                    };
                });
                
                // Create all nodes with progress updates
                const totalNodes = nodesToCreate.length;
                updateProgress(35, `Creating ${totalNodes} nodes...`);
                
                for (let nodeIndex = 0; nodeIndex < nodesToCreate.length; nodeIndex++) {
                    const nodeData = nodesToCreate[nodeIndex];
                    const newNode = this.canvasManager.addNode(nodeData);
                    nodeIdMap.set(nodeData.id, newNode.id);
                    loadedNodes.push(newNode);
                    
                    // Update progress every 10 nodes or for important nodes
                    if ((nodeIndex + 1) % 10 === 0 || nodeIndex === 0 || nodeIndex === nodesToCreate.length - 1) {
                        const percent = 35 + Math.floor((nodeIndex + 1) / totalNodes * 25);
                        const nodeName = nodeData.name || `Node ${nodeIndex + 1}`;
                        updateProgress(percent, `Creating: ${nodeName} (${nodeIndex + 1}/${totalNodes})`);
                        // Small delay to allow UI to update
                        await new Promise(resolve => setTimeout(resolve, 10));
                    }
                }
                
                updateProgress(60, 'Restoring connections...');
                
                // Don't restore state saving yet - we'll restore after groups are applied
                
                // Second pass: restore connections (batch mode)
                if (projectJson.connections && Array.isArray(projectJson.connections)) {
                    // Collect all valid connections first
                    const validConnections = [];
                    projectJson.connections.forEach(conn => {
                        const fromId = nodeIdMap.get(conn.from);
                        const toId = nodeIdMap.get(conn.to);
                        if (fromId && toId) {
                            validConnections.push({ from: fromId, to: toId, ...conn });
                        }
                    });
                    
                    // Add connections with progress updates
                    const totalConnections = validConnections.length;
                    for (let i = 0; i < validConnections.length; i++) {
                        const conn = validConnections[i];
                        this.canvasManager.addConnection(conn.from, conn.to);
                        
                        // Update progress every 50 connections
                        if ((i + 1) % 50 === 0 || i === validConnections.length - 1) {
                            const percent = 60 + Math.floor((i + 1) / totalConnections * 10);
                            updateProgress(percent, `Restoring connections... (${i + 1}/${totalConnections})`);
                            await new Promise(resolve => setTimeout(resolve, 5));
                        }
                    }
                }
                
                updateProgress(70, 'Building hierarchy...');
                
                // Third pass: restore groups (OPTIMIZED - batch operations)
                // Collect all group relationships first, then apply them in batch
                const groupRelationships = new Map(); // groupNodeId -> Set of itemNodeIds
                
                // Find the distribution group node once (used in multiple places)
                const distGroupNode = loadedNodes.find(n => n.type === 'distribution-group');
                
                // Method 1: Check for groups array
                if (projectJson.groups && Array.isArray(projectJson.groups)) {
                    projectJson.groups.forEach(groupData => {
                        const groupNodeId = nodeIdMap.get(groupData.id);
                        if (groupNodeId && groupData.containedItems) {
                            if (!groupRelationships.has(groupNodeId)) {
                                groupRelationships.set(groupNodeId, new Set());
                            }
                            groupData.containedItems.forEach(oldItemId => {
                                const newItemId = nodeIdMap.get(oldItemId);
                                if (newItemId) {
                                    groupRelationships.get(groupNodeId).add(newItemId);
                                }
                            });
                        }
                    });
                }
                
                // Method 2: Check each group node's containedItems property
                // Build a reverse map for faster lookup
                const originalNodeMap = new Map();
                projectJson.nodes.forEach(n => originalNodeMap.set(nodeIdMap.get(n.id), n));
                
                loadedNodes.forEach(newNode => {
                    if ((newNode.type === 'distribution-group' || newNode.type === 'packaging-group')) {
                        const originalNodeData = originalNodeMap.get(newNode.id);
                        if (originalNodeData && originalNodeData.containedItems && Array.isArray(originalNodeData.containedItems)) {
                            if (!groupRelationships.has(newNode.id)) {
                                groupRelationships.set(newNode.id, new Set());
                            }
                            originalNodeData.containedItems.forEach(oldItemId => {
                                const newItemId = nodeIdMap.get(oldItemId);
                                if (newItemId) {
                                    groupRelationships.get(newNode.id).add(newItemId);
                                }
                            });
                        }
                    }
                });
                
                // Method 3: Check each node's groupId property
                // This handles both direct group relationships and nested relationships
                projectJson.nodes.forEach(nodeData => {
                    if (nodeData.groupId) {
                        const itemNodeId = nodeIdMap.get(nodeData.id);
                        const groupNodeId = nodeIdMap.get(nodeData.groupId);
                        if (itemNodeId && groupNodeId) {
                            const groupNode = this.canvasManager.nodes.get(groupNodeId);
                            if (groupNode && (groupNode.type === 'distribution-group' || groupNode.type === 'packaging-group')) {
                                // Direct group relationship
                                if (!groupRelationships.has(groupNodeId)) {
                                    groupRelationships.set(groupNodeId, new Set());
                                }
                                groupRelationships.get(groupNodeId).add(itemNodeId);
                            } else if (groupNode && groupNode.type === 'distribution') {
                                // Nested relationship (e.g., hub under manufacturer, store under hub)
                                // Use the distribution group found above
                                if (distGroupNode) {
                                    // Add this node to the distribution group's containedItems
                                    if (!groupRelationships.has(distGroupNode.id)) {
                                        groupRelationships.set(distGroupNode.id, new Set());
                                    }
                                    groupRelationships.get(distGroupNode.id).add(itemNodeId);
                                }
                            }
                        }
                    }
                });
                
                // Method 4: Ensure ALL distribution nodes are in the distribution group's containedItems
                // This catches any nodes that might have been missed
                if (distGroupNode) {
                    if (!groupRelationships.has(distGroupNode.id)) {
                        groupRelationships.set(distGroupNode.id, new Set());
                    }
                    // Add all distribution nodes (manufacturer, hubs, stores) to the distribution group
                    loadedNodes.forEach(newNode => {
                        if (newNode.type === 'distribution') {
                            groupRelationships.get(distGroupNode.id).add(newNode.id);
                        }
                    });
                }
                
                updateProgress(80, 'Nesting nodes in groups...');
                
                // Now batch apply all group relationships
                // For distribution groups, nodes are hidden and shown only in the group's nested list
                if (distGroupNode) {
                    // Collect all nodes that should be in the group
                    const nodesToHide = [];
                    let processedCount = 0;
                    
                    // Collect all nodes that should be in the group
                    if (distGroupNode.containedItems && Array.isArray(distGroupNode.containedItems)) {
                        for (const itemId of distGroupNode.containedItems) {
                            const itemNode = this.canvasManager.nodes.get(itemId);
                            if (itemNode && itemNode.type === 'distribution') {
                                nodesToHide.push(itemNode);
                                processedCount++;
                                
                                // Update progress
                                if (processedCount % 50 === 0) {
                                    const percent = 80 + Math.floor((processedCount / distGroupNode.containedItems.length) * 10);
                                    updateProgress(percent, `Processing: ${itemNode.name || 'node'} (${processedCount}/${distGroupNode.containedItems.length})`);
                                    await new Promise(resolve => setTimeout(resolve, 10));
                                }
                                
                                // Also get nested children (hubs and stores)
                                this.canvasManager.nodes.forEach((childNode, childId) => {
                                    if (childNode.type === 'distribution' && childNode.groupId === itemId) {
                                        if (!nodesToHide.includes(childNode)) {
                                            nodesToHide.push(childNode);
                                        }
                                        
                                        // Get stores under hubs
                                        this.canvasManager.nodes.forEach((storeNode) => {
                                            if (storeNode.type === 'distribution' && storeNode.groupId === childId) {
                                                if (!nodesToHide.includes(storeNode)) {
                                                    nodesToHide.push(storeNode);
                                                }
                                            }
                                        });
                                    }
                                });
                            }
                        }
                    }
                    
                    updateProgress(90, `Hiding ${nodesToHide.length} nodes...`);
                    
                    // Hide all nodes - they're shown in the group's list
                    for (let i = 0; i < nodesToHide.length; i++) {
                        const node = nodesToHide[i];
                        const nodeEl = document.querySelector(`[data-node-id="${node.id}"]`);
                        if (nodeEl) {
                            nodeEl.style.display = 'none';
                            nodeEl.setAttribute('data-in-group', 'true');
                        }
                        
                        // Update progress every 100 nodes
                        if ((i + 1) % 100 === 0 || i === nodesToHide.length - 1) {
                            const percent = 90 + Math.floor((i + 1) / nodesToHide.length * 5);
                            updateProgress(percent, `Hiding: ${node.name || 'node'} (${i + 1}/${nodesToHide.length})`);
                            await new Promise(resolve => setTimeout(resolve, 5));
                        }
                    }
                    
                    // Re-render distribution group to show the nested list
                    updateProgress(95, 'Rendering distribution group...');
                    this.canvasManager.renderNode(distGroupNode);
                }
                
                let groupsRestored = 0;
                groupRelationships.forEach((itemIds, groupNodeId) => {
                    const groupNode = this.canvasManager.nodes.get(groupNodeId);
                    if (!groupNode) return;
                    
                    // Initialize containedItems if needed
                    if (!groupNode.containedItems) {
                        groupNode.containedItems = [];
                    }
                    
                    // For distribution groups, just set groupId and hide nodes (they're shown in list)
                    if (groupNode.type === 'distribution-group') {
                        itemIds.forEach(itemNodeId => {
                            const itemNode = this.canvasManager.nodes.get(itemNodeId);
                            if (!itemNode) return;
                            
                            // Remove from old group if any
                            if (itemNode.groupId) {
                                const oldGroup = this.canvasManager.nodes.get(itemNode.groupId);
                                if (oldGroup && oldGroup.containedItems) {
                                    oldGroup.containedItems = oldGroup.containedItems.filter(id => id !== itemNodeId);
                                }
                            }
                            
                            // Add to new group
                            itemNode.groupId = groupNodeId;
                            if (!groupNode.containedItems.includes(itemNodeId)) {
                                groupNode.containedItems.push(itemNodeId);
                            }
                            
                            // Hide the node - it's shown in the group's list
                            const itemNodeEl = document.querySelector(`[data-node-id="${itemNodeId}"]`);
                            if (itemNodeEl) {
                                itemNodeEl.style.display = 'none';
                                itemNodeEl.setAttribute('data-in-group', 'true');
                            }
                            
                            groupsRestored++;
                        });
                    } else {
                        // For packaging groups, just set groupId (nodes will be hidden)
                        itemIds.forEach(itemNodeId => {
                            const itemNode = this.canvasManager.nodes.get(itemNodeId);
                            if (!itemNode) return;
                            
                            // Remove from old group if any
                            if (itemNode.groupId) {
                                const oldGroup = this.canvasManager.nodes.get(itemNode.groupId);
                                if (oldGroup && oldGroup.containedItems) {
                                    oldGroup.containedItems = oldGroup.containedItems.filter(id => id !== itemNodeId);
                                }
                            }
                            
                            // Add to new group
                            itemNode.groupId = groupNodeId;
                            if (!groupNode.containedItems.includes(itemNodeId)) {
                                groupNode.containedItems.push(itemNodeId);
                            }
                            
                            // Hide packaging group items
                            const itemNodeEl = document.querySelector(`[data-node-id="${itemNodeId}"]`);
                            if (itemNodeEl) {
                                itemNodeEl.style.display = 'none';
                                itemNodeEl.setAttribute('data-in-group', 'true');
                            }
                            
                            groupsRestored++;
                        });
                    }
                });
                
                updateProgress(90, 'Rendering nodes and connections...');
                
                // First, ensure distribution group is positioned and sized correctly
                if (distGroupNode) {
                    // Re-render to apply size changes
                    this.canvasManager.renderNode(distGroupNode);
                    
                    // Set z-index so group is behind children
                    const groupEl = document.querySelector(`[data-node-id="${distGroupNode.id}"]`);
                    if (groupEl) {
                        groupEl.style.zIndex = '5';
                        // Ensure group node has proper styling to visually contain children
                        groupEl.style.overflow = 'visible';
                        groupEl.style.position = 'absolute';
                    }
                }
                
                // Then render all other group nodes
                groupRelationships.forEach((itemIds, groupNodeId) => {
                    if (groupNodeId !== distGroupNode?.id) {
                        const groupNode = this.canvasManager.nodes.get(groupNodeId);
                        if (groupNode) {
                            this.canvasManager.renderNode(groupNode);
                        }
                    }
                });
                
                // Distribution nodes in the group are hidden - they're shown in the group's nested list
                // Only render nodes that are NOT in the distribution group
                loadedNodes.forEach(node => {
                    if (node.type === 'distribution-group') {
                        // Already rendered above
                        return;
                    }
                    
                    // Check if node is in the distribution group
                    let isInGroup = false;
                    if (node.type === 'distribution' && distGroupNode) {
                        if (node.groupId === distGroupNode.id) {
                            isInGroup = true;
                        } else if (node.groupId) {
                            // Check if parent is in the group
                            const parent = this.canvasManager.nodes.get(node.groupId);
                            if (parent && (parent.groupId === distGroupNode.id || 
                                (distGroupNode.containedItems && distGroupNode.containedItems.includes(parent.id)))) {
                                isInGroup = true;
                            }
                        } else if (distGroupNode.containedItems && distGroupNode.containedItems.includes(node.id)) {
                            isInGroup = true;
                        }
                    }
                    
                    // Only render nodes that are NOT in the distribution group
                    if (!isInGroup) {
                        this.canvasManager.renderNode(node);
                    } else {
                        // Hide nodes in the group - they're shown in the nested list
                        const nodeEl = document.querySelector(`[data-node-id="${node.id}"]`);
                        if (nodeEl) {
                            nodeEl.style.display = 'none';
                            nodeEl.setAttribute('data-in-group', 'true');
                        }
                    }
                });
                
                // Update connections once at the end - this will draw all connection lines
                this.canvasManager.updateConnections();
                
                updateProgress(100, 'Complete!');
                
                // Restore state saving and save once at the end
                this.canvasManager.saveState = originalSaveState;
                this.canvasManager.saveState();
                
                // Remove loading indicator
                const loadingIndicatorEl = document.getElementById('eprLoadingIndicator');
                if (loadingIndicatorEl) {
                    loadingIndicatorEl.remove();
                }
                
                // Select the first loaded node if any
                if (loadedNodes.length > 0) {
                    this.canvasManager.selectNode(loadedNodes[0].id);
                }
            } catch (error) {
                // Remove loading indicator on error
                const loadingIndicatorError = document.getElementById('eprLoadingIndicator');
                if (loadingIndicatorError) {
                    loadingIndicatorError.remove();
                }
                console.error('[EPR Types Toolbar] Error loading distribution group:', error);
                alert('Error loading distribution group: ' + error.message);
            }
        }
        
        addNote() {
            // Show modal to create note with formatting options
            this.showNoteModal();
        }
        
        showNoteModal() {
            // Create modal HTML with formatting toolbar
            const modalHtml = `
                <div class="modal fade" id="eprNoteModal" tabindex="-1">
                    <div class="modal-dialog modal-lg">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title">Create Note</h5>
                                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                            </div>
                            <div class="modal-body">
                                <div class="mb-3">
                                    <label class="form-label">Title (Bold)</label>
                                    <input type="text" class="form-control" id="eprNoteHeadline" placeholder="Enter note title..." />
                                </div>
                                <div class="mb-3">
                                    <label class="form-label">Body Text</label>
                                    <div class="btn-toolbar mb-2" role="toolbar" id="eprNoteFormatToolbar">
                                        <div class="btn-group me-2" role="group">
                                            <button type="button" class="btn btn-sm btn-outline-secondary" id="eprNoteBold" title="Bold">
                                                <i class="bi bi-type-bold"></i>
                                            </button>
                                            <button type="button" class="btn btn-sm btn-outline-secondary" id="eprNoteItalic" title="Italic">
                                                <i class="bi bi-type-italic"></i>
                                            </button>
                                            <button type="button" class="btn btn-sm btn-outline-secondary" id="eprNoteUnderline" title="Underline">
                                                <i class="bi bi-type-underline"></i>
                                            </button>
                                        </div>
                                    </div>
                                    <div class="form-control" id="eprNoteBodyText" contenteditable="true" style="min-height: 200px; padding: 0.375rem 0.75rem; border: 1px solid #ced4da; border-radius: 0.25rem; background-color: #fff;" placeholder="Enter note body text..."></div>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                                <button type="button" class="btn btn-primary" id="eprSaveNoteBtn">Create Note</button>
                            </div>
                        </div>
                    </div>
                </div>
            `;
            
            // Remove existing modal if any
            const existingModal = document.getElementById('eprNoteModal');
            if (existingModal) {
                existingModal.remove();
            }
            
            // Add modal to body
            document.body.insertAdjacentHTML('beforeend', modalHtml);
            
            // Get modal element
            const modalElement = document.getElementById('eprNoteModal');
            const modal = new bootstrap.Modal(modalElement);
            
            // Setup formatting toolbar
            const bodyTextDiv = document.getElementById('eprNoteBodyText');
            const boldBtn = document.getElementById('eprNoteBold');
            const italicBtn = document.getElementById('eprNoteItalic');
            const underlineBtn = document.getElementById('eprNoteUnderline');
            
            boldBtn.addEventListener('click', (e) => {
                e.preventDefault();
                document.execCommand('bold', false, null);
                bodyTextDiv.focus();
            });
            
            italicBtn.addEventListener('click', (e) => {
                e.preventDefault();
                document.execCommand('italic', false, null);
                bodyTextDiv.focus();
            });
            
            underlineBtn.addEventListener('click', (e) => {
                e.preventDefault();
                document.execCommand('underline', false, null);
                bodyTextDiv.focus();
            });
            
            // Save note button
            document.getElementById('eprSaveNoteBtn').addEventListener('click', () => {
                const headline = document.getElementById('eprNoteHeadline').value.trim();
                const bodyText = bodyTextDiv.innerHTML.trim() || bodyTextDiv.innerText.trim();
                
                if (!headline && !bodyText) {
                    alert('Please enter a title or body text');
                    return;
                }
                
                // Create note node
                const node = this.canvasManager.addNode({
                    type: 'note',
                    name: headline || 'Note',
                    icon: 'bi-sticky-fill',
                    parameters: {
                        headline: headline || 'Note',
                        bodyText: bodyText
                    }
                });
                
                this.canvasManager.selectNode(node.id);
                modal.hide();
                
                // Clean up modal
                modalElement.addEventListener('hidden.bs.modal', function() {
                    this.remove();
                });
            });
            
            modal.show();
        }
        
        async saveNewPackaging() {
            const name = document.getElementById('eprNewPackagingName').value.trim();
            const description = document.getElementById('eprNewPackagingDescription').value.trim();
            
            if (!name) {
                alert('Please enter a name');
                return;
            }
            
            // Don't provide x/y so it uses default positioning (150px to the right of last node)
            const node = this.canvasManager.addNode({
                type: 'packaging',
                name: name,
                icon: 'bi-box-seam',
                parameters: { description: description }
            });
            
            this.canvasManager.selectNode(node.id);
            
            const modal = bootstrap.Modal.getInstance(document.getElementById('eprNewPackagingModal'));
            modal.hide();
        }
        
        renderProductDropdown() {
            const dropdown = document.getElementById('eprProductLibraryDropdown');
            if (!dropdown) {
                console.warn('[EPR Types Toolbar] Product dropdown not found');
                return;
            }
            
            dropdown.innerHTML = '<option value="">Select product...</option>';
            
            // ALWAYS ensure we have seed data
            if (!this.products || this.products.length === 0) {
                console.log('[EPR Types Toolbar] No product data, using seed data');
                this.products = [
                    { id: 1, sku: 'PROD-001', name: 'Product A', description: 'Sample product A', size: 'Medium', weight: 1.0, height: 25, quantity: 100 },
                    { id: 2, sku: 'PROD-002', name: 'Product B', description: 'Sample product B', size: 'Large', weight: 2.0, height: 30, quantity: 50 },
                    { id: 3, sku: 'PROD-003', name: 'Product C', description: 'Sample product C', size: 'Small', weight: 0.5, height: 15, quantity: 200 },
                    { id: 4, sku: 'PROD-004', name: 'Product D', description: 'Sample product D', size: 'Extra Large', weight: 3.0, height: 40, quantity: 25 },
                    { id: 5, sku: 'PROD-005', name: 'Product E', description: 'Sample product E', size: 'Medium', weight: 1.5, height: 20, quantity: 150 }
                ];
            }
            
            this.products.forEach(product => {
                const option = document.createElement('option');
                option.value = product.id;
                option.textContent = `${product.name} (${product.sku || 'N/A'})`;
                dropdown.appendChild(option);
            });
            
            console.log('[EPR Types Toolbar] Product dropdown rendered with', this.products.length, 'items');
            
            // Make dropdown draggable
            dropdown.draggable = true;
            dropdown.addEventListener('dragstart', async (e) => {
                const productId = parseInt(dropdown.value, 10);
                if (!productId) {
                    e.preventDefault();
                    return;
                }
                
                try {
                    const res = await fetch(`/api/visual-editor/product/${productId}`);
                    const product = await res.json();
                    
                    const nodeData = {
                        type: 'product',
                        entityId: product.id,
                        name: product.name,
                        icon: 'bi-bag',
                        parameters: {
                            sku: product.sku,
                            description: product.description,
                            size: product.size,
                            weight: product.weight,
                            height: product.height,
                            quantity: product.quantity
                        }
                    };
                    e.dataTransfer.setData('application/json', JSON.stringify(nodeData));
                    e.dataTransfer.effectAllowed = 'copy';
                } catch (error) {
                    console.error('[EPR Types Toolbar] Error loading product:', error);
                    e.preventDefault();
                }
            });
            
            dropdown.addEventListener('mousedown', (e) => {
                if (dropdown.value) {
                    dropdown.draggable = true;
                } else {
                    dropdown.draggable = false;
                }
            });
        }
        
        renderSupplierPackagingDropdown() {
            const dropdown = document.getElementById('eprSupplierPackagingDropdown');
            if (!dropdown) return;
            dropdown.innerHTML = '<option value="">Select supplier packaging...</option>';
            if (!this.supplierPackaging || this.supplierPackaging.length === 0) return;
            this.supplierPackaging.forEach(sp => {
                const option = document.createElement('option');
                option.value = sp.id;
                option.textContent = `${sp.name} (${sp.supplierName || 'N/A'})`;
                dropdown.appendChild(option);
            });
        }
        
        addSelectedSupplierPackaging() {
            const dropdown = document.getElementById('eprSupplierPackagingDropdown');
            if (!dropdown || !dropdown.value) {
                alert('Please select supplier packaging');
                return;
            }
            const id = parseInt(dropdown.value, 10);
            const sp = this.supplierPackaging.find(p => p.id === id);
            if (!sp) return;
            const node = this.canvasManager.addNode({
                type: 'supplier-packaging',
                entityId: sp.id,
                name: sp.name,
                icon: 'bi-truck',
                parameters: {
                    supplierName: sp.supplierName,
                    productCode: sp.productCode,
                    taxonomyCode: sp.taxonomyCode
                }
            });
            this.canvasManager.selectNode(node.id);
        }
        
        async addSelectedProduct() {
            let dropdown = document.getElementById('eprProductLibraryDropdown');
            if (!dropdown || !dropdown.value) {
                dropdown = document.getElementById('eprProductLibraryDropdownSplit');
            }
            
            if (!dropdown) {
                console.error('[EPR Types Toolbar] Product dropdown not found');
                alert('Product dropdown not found');
                return;
            }
            
            const productId = parseInt(dropdown.value);
            console.log('[EPR Types Toolbar] Product dropdown value:', dropdown.value, 'parsed:', productId);
            
            if (!dropdown.value || dropdown.value === '' || !productId || isNaN(productId) || productId === 0) {
                alert('Please select a product');
                return;
            }
            
            console.log('[EPR Types Toolbar] Adding product with full supply chain, ID:', productId);
            
            try {
                const resp = await fetch(`/api/visual-editor/product/${productId}/supply-chain`);
                const result = await resp.json();
                if (result.success && result.nodes && result.nodes.length > 0) {
                    this._loadSupplyChainIntoCanvas(result, productId);
                    return;
                }
            } catch (err) {
                console.warn('[EPR Types Toolbar] Supply chain fetch failed, falling back to basic add:', err);
            }
            
            // Fallback: add just the product node if supply chain endpoint unavailable
            try {
                const res = await fetch(`/api/visual-editor/product/${productId}`);
                if (!res.ok) throw new Error('Failed to fetch product');
                const product = await res.json();
                
                const node = this.canvasManager.addNode({
                    type: 'product',
                    entityId: product.id,
                    name: product.name,
                    icon: 'bi-bag',
                    parameters: {
                        sku: product.sku,
                        description: product.description,
                        size: product.size,
                        weight: product.weight,
                        height: product.height,
                        quantity: product.quantity
                    }
                });
                this.canvasManager.selectNode(node.id);
            } catch (error) {
                console.error('[EPR Types Toolbar] Error adding product:', error);
                alert('Error loading product: ' + error.message);
            }
        }
        
        _loadSupplyChainIntoCanvas(result, focusProductId) {
            const columnOffsets = {
                'supplier':           -1270,
                'supplier-packaging': -990,
                'raw-material':       -990,
                'packaging':          -660,
                'packaging-group':    -330,
                'product':            0,
                'distribution':       330
            };
            const columnLabels = {
                'supplier':           'Suppliers',
                'supplier-packaging': 'Supplier Products',
                'raw-material':       'Raw Materials',
                'packaging':          'Packaging Items',
                'packaging-group':    'Packaging Groups',
                'product':            'Products',
                'distribution':       'Distribution'
            };
            const ySpacing = 140;

            // Find rightmost node to position the new subgraph
            let baseX = 100;
            let baseY = 80;
            if (this.canvasManager.nodes.size > 0) {
                let maxX = 0;
                for (const [, n] of this.canvasManager.nodes) {
                    if (n.x + 250 > maxX) maxX = n.x + 250;
                }
                baseX = maxX + 100;
            }

            const productCol = baseX + Math.abs(columnOffsets['supplier']);
            const columnY = {};
            for (const t of Object.keys(columnOffsets)) columnY[t] = 0;

            const nodeIdMap = new Map();
            const addedIds = new Set();

            for (const n of result.nodes) {
                if (this.canvasManager.nodes.has(n.id)) {
                    nodeIdMap.set(n.id, true);
                    addedIds.add(n.id);
                    continue;
                }

                const offset = columnOffsets[n.type] ?? 0;
                const yOff = columnY[n.type] ?? 0;

                const nodeData = {
                    id: n.id,
                    type: n.type,
                    entityId: n.entityId,
                    name: n.label || n.sku || 'Unknown',
                    x: productCol + offset,
                    y: baseY + yOff,
                    parameters: {}
                };

                if (n.sku) nodeData.parameters.sku = n.sku;
                if (n.imageUrl) nodeData.parameters.imageUrl = n.imageUrl;
                if (n.code) nodeData.parameters.code = n.code;
                if (n.city) nodeData.parameters.city = n.city;
                if (n.country) nodeData.parameters.country = n.country;
                if (n.supplierName) nodeData.parameters.supplierName = n.supplierName;
                if (n.taxonomyCode) nodeData.parameters.taxonomyCode = n.taxonomyCode;
                if (n.packId) nodeData.parameters.packId = n.packId;
                if (n.layer) nodeData.parameters.layer = n.layer;
                if (n.productCode) nodeData.parameters.productCode = n.productCode;

                this.canvasManager.nodes.set(n.id, nodeData);
                this.canvasManager.renderNode(nodeData);
                nodeIdMap.set(n.id, true);
                addedIds.add(n.id);
                columnY[n.type] = (columnY[n.type] || 0) + ySpacing;
            }

            // Column headers
            const renderedLabels = new Set();
            for (const [type, label] of Object.entries(columnLabels)) {
                if (columnY[type] === 0) continue;
                const xPos = productCol + (columnOffsets[type] ?? 0);
                const key = `${xPos}-${label}`;
                if (renderedLabels.has(key)) continue;
                renderedLabels.add(key);
                const header = document.createElement('div');
                header.className = 'epr-column-header';
                header.style.cssText = `position:absolute;left:${xPos}px;top:${baseY - 30}px;font-size:11px;font-weight:700;text-transform:uppercase;letter-spacing:0.5px;color:var(--bs-secondary,#6c757d);pointer-events:none;white-space:nowrap;z-index:1;`;
                header.textContent = label;
                this.canvasManager.nodesLayer.appendChild(header);
            }

            this.canvasManager.updatePlaceholder();

            for (const e of result.edges) {
                if (nodeIdMap.has(e.from) && nodeIdMap.has(e.to)) {
                    this.canvasManager.addConnection(e.from, e.to);
                }
            }

            this.canvasManager.updateConnections();

            const productNodeId = `product-${focusProductId}`;
            if (this.canvasManager.nodes.has(productNodeId)) {
                this.canvasManager.selectNode(productNodeId);
            }

            console.log(`[EPR] Loaded product supply chain: ${addedIds.size} nodes, ${result.edges.length} edges`);
        }
        
        showNewProductModal() {
            const modal = new bootstrap.Modal(document.getElementById('eprNewProductModal'));
            document.getElementById('eprNewProductSku').value = '';
            document.getElementById('eprNewProductName').value = '';
            document.getElementById('eprNewProductDescription').value = '';
            modal.show();
        }
        
        async saveNewProduct() {
            const sku = document.getElementById('eprNewProductSku').value.trim();
            const name = document.getElementById('eprNewProductName').value.trim();
            const description = document.getElementById('eprNewProductDescription').value.trim();
            
            if (!sku || !name) {
                alert('Please enter SKU and name');
                return;
            }
            
            // Don't provide x/y so it uses default positioning (150px to the right of last node)
            const node = this.canvasManager.addNode({
                type: 'product',
                name: name,
                icon: 'bi-bag',
                parameters: {
                    sku: sku,
                    description: description
                }
            });
            
            this.canvasManager.selectNode(node.id);
            
            const modal = bootstrap.Modal.getInstance(document.getElementById('eprNewProductModal'));
            modal.hide();
        }
        
        renderDistributionDropdowns() {
            const countryDropdown = document.getElementById('eprCountryDropdown');
            if (!countryDropdown) {
                console.warn('[EPR Types Toolbar] Country dropdown not found');
                return;
            }
            
            countryDropdown.innerHTML = '<option value="">Select country...</option>';
            
            // ALWAYS ensure we have seed data
            if (!this.geographies || this.geographies.length === 0) {
                console.log('[EPR Types Toolbar] No geography data, using seed data');
                this.geographies = [
                    { id: 1, name: 'United States', code: 'US', parentId: null },
                    { id: 2, name: 'United Kingdom', code: 'UK', parentId: null },
                    { id: 3, name: 'Canada', code: 'CA', parentId: null },
                    { id: 4, name: 'Germany', code: 'DE', parentId: null },
                    { id: 5, name: 'France', code: 'FR', parentId: null },
                    { id: 6, name: 'California', code: 'CA', parentId: 1 },
                    { id: 7, name: 'New York', code: 'NY', parentId: 1 },
                    { id: 8, name: 'Texas', code: 'TX', parentId: 1 },
                    { id: 9, name: 'England', code: 'ENG', parentId: 2 },
                    { id: 10, name: 'Scotland', code: 'SCT', parentId: 2 }
                ];
            }
            
            const countries = this.geographies.filter(g => !g.parentId);
            countries.forEach(geo => {
                const option = document.createElement('option');
                option.value = geo.id;
                option.textContent = geo.name;
                countryDropdown.appendChild(option);
            });
            
            // Make country dropdown draggable
            countryDropdown.draggable = true;
            countryDropdown.addEventListener('dragstart', (e) => {
                const countryId = countryDropdown.value;
                if (!countryId) {
                    e.preventDefault();
                    return;
                }
                
                const selectedGeo = this.geographies.find(g => g.id == parseInt(countryId));
                const locationName = selectedGeo ? selectedGeo.name : (countryDropdown.options[countryDropdown.selectedIndex]?.text || 'Unknown Location');
                
                const nodeData = {
                    type: 'distribution',
                    name: locationName,
                    icon: 'bi-geo-alt',
                    parameters: {
                        geographyId: parseInt(countryId),
                        country: locationName
                    }
                };
                e.dataTransfer.setData('application/json', JSON.stringify(nodeData));
                e.dataTransfer.effectAllowed = 'copy';
            });
            
            // Make region dropdown draggable
            const regionDropdown = document.getElementById('eprRegionDropdown');
            if (regionDropdown) {
                regionDropdown.draggable = true;
                regionDropdown.addEventListener('dragstart', (e) => {
                    const regionId = regionDropdown.value;
                    if (!regionId) {
                        e.preventDefault();
                        return;
                    }
                    
                    const selectedGeo = this.geographies.find(g => g.id == parseInt(regionId));
                    const locationName = selectedGeo ? selectedGeo.name : (regionDropdown.options[regionDropdown.selectedIndex]?.text || 'Unknown Location');
                    
                    const nodeData = {
                        type: 'distribution',
                        name: locationName,
                        icon: 'bi-geo-alt',
                        parameters: {
                            geographyId: parseInt(regionId),
                            country: locationName
                        }
                    };
                    e.dataTransfer.setData('application/json', JSON.stringify(nodeData));
                    e.dataTransfer.effectAllowed = 'copy';
                });
            }
            
            console.log('[EPR Types Toolbar] Distribution dropdown rendered with', countries.length, 'countries');
        }
        
        onCountryChange(countryId) {
            const regionDropdown = document.getElementById('eprRegionDropdown');
            if (!regionDropdown) return;
            
            if (!countryId) {
                regionDropdown.style.display = 'none';
                return;
            }
            
            const regions = this.geographies.filter(g => g.parentId == countryId);
            
            if (regions.length > 0) {
                regionDropdown.innerHTML = '<option value="">Select region...</option>';
                regions.forEach(region => {
                    const option = document.createElement('option');
                    option.value = region.id;
                    option.textContent = region.name;
                    regionDropdown.appendChild(option);
                });
                regionDropdown.style.display = 'block';
            } else {
                regionDropdown.style.display = 'none';
            }
        }
        
        addSelectedDistribution() {
            // Check both unified and split dropdowns
            let countryDropdown = document.getElementById('eprCountryDropdown');
            let regionDropdown = document.getElementById('eprRegionDropdown');
            
            if (!countryDropdown) {
                countryDropdown = document.getElementById('eprCountryDropdownSplit');
                regionDropdown = document.getElementById('eprRegionDropdownSplit');
            }
            
            if (!countryDropdown) {
                console.error('[EPR Types Toolbar] Country dropdown not found');
                alert('Country dropdown not found');
                return;
            }
            
            const countryId = countryDropdown.value;
            const regionId = regionDropdown && regionDropdown.value ? regionDropdown.value : null;
            
            // Check if country is selected (required)
            if (!countryId || countryId === '' || countryId === '0') {
                alert('Please select a country');
                return;
            }
            
            // Use region if selected, otherwise use country
            const finalId = regionId && regionId !== '' && regionId !== '0' ? regionId : countryId;
            
            // Validate final selection
            if (!finalId || finalId === '' || finalId === '0') {
                alert('Please select a location');
                return;
            }
            
            console.log('[EPR Types Toolbar] Adding distribution for country:', countryId, 'region:', regionId, 'finalId:', finalId);
            
            console.log('[EPR Types Toolbar] Adding distribution for country:', countryId, 'region:', regionId);
            
            const selectedGeo = this.geographies.find(g => g.id == parseInt(finalId));
            const locationName = selectedGeo ? selectedGeo.name : (countryDropdown.options[countryDropdown.selectedIndex]?.text || 'Unknown Location');
            
            // Don't provide x/y so it uses default positioning (150px to the right of last node)
            const node = this.canvasManager.addNode({
                type: 'distribution',
                name: locationName,
                icon: 'bi-geo-alt',
                parameters: {
                    geographyId: parseInt(finalId),
                    country: countryDropdown.options[countryDropdown.selectedIndex]?.text || locationName
                }
            });
            
            console.log('[EPR Types Toolbar] ✅ Distribution node added:', node.id, node.name);
            this.canvasManager.selectNode(node.id);
        }
        
        showNewLocationModal() {
            const modal = new bootstrap.Modal(document.getElementById('eprNewLocationModal'));
            // Clear all fields
            const uniqueLocationCodeEl = document.getElementById('eprNewLocationUniqueLocationCode');
            const companyNameEl = document.getElementById('eprNewLocationCompanyName');
            const siteNameEl = document.getElementById('eprNewLocationSiteName');
            const streetAddressEl = document.getElementById('eprNewLocationStreetAddress');
            const postcodeEl = document.getElementById('eprNewLocationPostcode');
            const cityEl = document.getElementById('eprNewLocationCity');
            const stateEl = document.getElementById('eprNewLocationState');
            const countryEl = document.getElementById('eprNewLocationCountry');
            const localAuthorityEl = document.getElementById('eprNewLocationLocalAuthority');
            const regionalAuthorityEl = document.getElementById('eprNewLocationRegionalAuthority');
            const industryAuthorityEl = document.getElementById('eprNewLocationIndustryAuthority');
            const typeEl = document.getElementById('eprNewLocationType');
            
            if (uniqueLocationCodeEl) uniqueLocationCodeEl.value = '';
            if (companyNameEl) companyNameEl.value = '';
            if (siteNameEl) siteNameEl.value = '';
            if (streetAddressEl) streetAddressEl.value = '';
            if (postcodeEl) postcodeEl.value = '';
            if (cityEl) cityEl.value = '';
            if (stateEl) stateEl.value = '';
            if (countryEl) countryEl.value = '';
            if (localAuthorityEl) localAuthorityEl.value = '';
            if (regionalAuthorityEl) regionalAuthorityEl.value = '';
            if (industryAuthorityEl) industryAuthorityEl.value = '';
            if (typeEl) typeEl.value = '';
            
            modal.show();
        }
        
        async saveNewLocation() {
            const uniqueLocationCode = document.getElementById('eprNewLocationUniqueLocationCode')?.value.trim() || '';
            const companyName = document.getElementById('eprNewLocationCompanyName')?.value.trim() || '';
            const siteName = document.getElementById('eprNewLocationSiteName')?.value.trim() || '';
            const streetAddress = document.getElementById('eprNewLocationStreetAddress')?.value.trim() || '';
            const postcodeZipcode = document.getElementById('eprNewLocationPostcode')?.value.trim() || '';
            const city = document.getElementById('eprNewLocationCity')?.value.trim() || '';
            const state = document.getElementById('eprNewLocationState')?.value.trim() || '';
            const country = document.getElementById('eprNewLocationCountry')?.value.trim() || '';
            const localAuthority = document.getElementById('eprNewLocationLocalAuthority')?.value.trim() || '';
            const regionalAuthority = document.getElementById('eprNewLocationRegionalAuthority')?.value.trim() || '';
            const industryAuthority = document.getElementById('eprNewLocationIndustryAuthority')?.value.trim() || '';
            const type = document.getElementById('eprNewLocationType')?.value || '';
            
            if (!country) {
                alert('Please enter a country (required)');
                return;
            }
            
            // Create location name from available fields (prefer site name or company name)
            const locationName = siteName || companyName || [city, state, country].filter(Boolean).join(', ') || country;
            
            // Don't provide x/y so it uses default positioning (150px to the right of last node)
            const node = this.canvasManager.addNode({
                type: 'distribution',
                name: locationName,
                icon: 'bi-geo-alt',
                parameters: {
                    uniqueLocationCode: uniqueLocationCode,
                    companyName: companyName,
                    siteName: siteName,
                    streetAddress: streetAddress,
                    postcodeZipcode: postcodeZipcode,
                    city: city,
                    county: state,
                    stateProvince: state,
                    country: country,
                    localAuthority: localAuthority,
                    regionalAuthority: regionalAuthority,
                    industryAuthority: industryAuthority,
                    type: type
                }
            });
            
            this.canvasManager.selectNode(node.id);
            
            const modal = bootstrap.Modal.getInstance(document.getElementById('eprNewLocationModal'));
            modal.hide();
        }
        
        showNewLocationModal() {
            const modal = new bootstrap.Modal(document.getElementById('eprNewLocationModal'));
            // Clear all fields
            document.getElementById('eprNewLocationStreetAddress').value = '';
            document.getElementById('eprNewLocationPostcode').value = '';
            document.getElementById('eprNewLocationCity').value = '';
            document.getElementById('eprNewLocationState').value = '';
            document.getElementById('eprNewLocationCountry').value = '';
            document.getElementById('eprNewLocationLocalAuthority').value = '';
            document.getElementById('eprNewLocationRegionalAuthority').value = '';
            document.getElementById('eprNewLocationIndustryAuthority').value = '';
            document.getElementById('eprNewLocationType').value = '';
            modal.show();
        }
    }
    
    // ============================================================================
    // PARAMETERS PANEL
    // ============================================================================
    class EPRParametersPanel {
        constructor(canvasManager) {
            console.log('[EPR Parameters Panel] Creating ParametersPanel...');
            this.canvasManager = canvasManager;
            this.currentNode = null;
        }
        
        loadNodeParameters(node) {
            this.currentNode = node;
            const container = document.getElementById('eprParametersContent');
            if (!container) return;
            
            if (!node) {
                container.innerHTML = '<p class="text-muted small">Select a node to view/edit parameters</p>';
                return;
            }
            
            const isLocked = node.locked || false;
            const lockMessage = isLocked ? '<div class="alert alert-warning mb-2"><small><i class="bi bi-lock-fill"></i> This node is locked. Parameters cannot be edited.</small></div>' : '';
            let html = `<h6 class="mb-3">${this.escapeHtml(node.name)}</h6>${lockMessage}`;
            
            // Special handling for note nodes
            if (node.type === 'note') {
                const headline = node.parameters?.headline || 'Note';
                const bodyText = node.parameters?.bodyText || '';
                const isLocked = node.locked || false;
                const disabledAttr = isLocked ? 'disabled' : '';
                html += `
                    <div class="parameter-group">
                        <label for="eprNoteHeadline">Headline</label>
                        <input type="text" id="eprNoteHeadline" class="form-control epr-parameter-input" data-param-key="headline" value="${this.escapeHtml(headline)}" maxlength="50" ${disabledAttr} />
                    </div>
                    <div class="parameter-group">
                        <label for="eprNoteBodyText">Body Text</label>
                        <textarea id="eprNoteBodyText" class="form-control epr-parameter-input" data-param-key="bodyText" rows="10" maxlength="500" style="max-width: 50ch; resize: vertical;" ${disabledAttr}>${this.escapeHtml(bodyText)}</textarea>
                    </div>
                `;
                container.innerHTML = html;
                
                // Setup parameter input event listeners (only if not locked)
                if (!isLocked) {
                    container.querySelectorAll('.epr-parameter-input').forEach(input => {
                        const paramKey = input.dataset.paramKey;
                        if (!paramKey) return;
                        
                        input.addEventListener('input', (e) => {
                            if (!node.parameters) {
                                node.parameters = {};
                            }
                            node.parameters[paramKey] = e.target.value;
                            this.canvasManager.saveState();
                            this.canvasManager.renderNode(node);
                        });
                    });
                }
                return;
            }
            
            const commonParams = [
                { key: 'name', label: 'Name', type: 'text', value: node.name },
                { key: 'description', label: 'Description', type: 'textarea', value: node.parameters?.description || '' },
                { key: 'notes', label: 'Notes', type: 'textarea', value: node.parameters?.notes || '' }
            ];
            
            // Add weight parameter for raw materials
            if (node.type === 'raw-material') {
                commonParams.push(
                    { key: 'weight', label: 'Weight', type: 'number', value: node.parameters?.weight || '' }
                );
            }
            
            if (node.type === 'packaging' || node.type === 'product') {
                commonParams.push(
                    { key: 'height', label: 'Height', type: 'number', value: node.parameters?.height || '' },
                    { key: 'weight', label: 'Weight', type: 'number', value: node.parameters?.weight || '' },
                    { key: 'depth', label: 'Depth', type: 'number', value: node.parameters?.depth || '' }
                );
            }
            
            if (node.type === 'product') {
                commonParams.push(
                    { key: 'sku', label: 'SKU', type: 'text', value: node.parameters?.sku || '' },
                    { key: 'quantity', label: 'Quantity', type: 'number', value: node.parameters?.quantity || '' }
                );
            }
            
            if (node.type === 'distribution') {
                commonParams.push(
                    { key: 'uniqueLocationCode', label: 'Unique Location Code', type: 'text', value: node.parameters?.uniqueLocationCode || '' },
                    { key: 'companyName', label: 'Company Name', type: 'text', value: node.parameters?.companyName || '' },
                    { key: 'siteName', label: 'Site Name', type: 'text', value: node.parameters?.siteName || '' },
                    { key: 'streetAddress', label: 'Street Address', type: 'text', value: node.parameters?.streetAddress || '' },
                    { key: 'postcodeZipcode', label: 'Postcode/Zip Code', type: 'text', value: node.parameters?.postcodeZipcode || '' },
                    { key: 'city', label: 'City', type: 'text', value: node.parameters?.city || '' },
                    { key: 'county', label: 'County/State/Province', type: 'text', value: node.parameters?.county || node.parameters?.stateProvince || '' },
                    { key: 'country', label: 'Country', type: 'text', value: node.parameters?.country || '', required: true },
                    { key: 'localAuthority', label: 'Local Authority', type: 'text', value: node.parameters?.localAuthority || '' },
                    { key: 'regionalAuthority', label: 'Regional Authority', type: 'text', value: node.parameters?.regionalAuthority || '' },
                    { key: 'industryAuthority', label: 'Industry Authority', type: 'text', value: node.parameters?.industryAuthority || '' },
                    { key: 'type', label: 'Type', type: 'select', value: node.parameters?.type || '', options: [
                        { value: '', label: 'Select type...' },
                        { value: 'Hub', label: 'Hub' },
                        { value: 'Warehouse', label: 'Warehouse' },
                        { value: 'Store', label: 'Store' }
                    ]}
                );
            }
            
            // Reuse isLocked already declared at line 6134
            commonParams.forEach(param => {
                param.disabled = isLocked;
                html += this.renderParameterField(param);
            });
            
            // Add connection quantities section if node has any connection quantities
            if (node.parameters?.connectionQuantities && Object.keys(node.parameters.connectionQuantities).length > 0) {
                html += '<div class="parameter-group mt-3"><h6>Connection Quantities</h6>';
                Object.entries(node.parameters.connectionQuantities).forEach(([connectionKey, quantityData]) => {
                    const [fromId, toId] = connectionKey.split('-');
                    const toNode = this.canvasManager.nodes.get(toId);
                    const toNodeName = toNode ? toNode.name : 'Unknown';
                    
                    // Handle both old format (number) and new format (object)
                    const quantity = typeof quantityData === 'object' ? quantityData.value : quantityData;
                    const type = typeof quantityData === 'object' ? quantityData.type : null;
                    const unit = typeof quantityData === 'object' ? quantityData.unit : null;
                    
                    let displayValue = quantity.toString();
                    if (type === 'quantity' && unit) {
                        displayValue = parseFloat(quantity).toFixed(2);
                    }
                    if (unit && unit !== 'count') {
                        displayValue += ' ' + unit;
                    }
                    if (type) {
                        displayValue += ' (' + type + ')';
                    }
                    
                    html += `<div class="mb-2">
                        <label class="form-label small">To ${this.escapeHtml(toNodeName)}</label>
                        <input type="text" class="form-control form-control-sm" 
                               value="${this.escapeHtml(displayValue)}" readonly>
                    </div>`;
                });
                html += '<small class="form-text text-muted">Right-click on connection lines to edit quantities</small>';
                html += '</div>';
            }
            
            // Add transport settings if this is a transport node
            if (node.type === 'transport') {
                html += this.renderTransportSettings(node);
            }
            
            // Add packaging group settings if this is a packaging-group node
            if (node.type === 'packaging-group') {
                html += this.renderPackagingGroupSettings(node);
            }
            
            container.innerHTML = html;
            
            // Setup parameter input event listeners (only if node is not locked)
            if (!isLocked) {
                container.querySelectorAll('.epr-parameter-input').forEach(input => {
                    const paramKey = input.dataset.paramKey;
                    if (!paramKey) return;
                    
                    input.addEventListener('input', (e) => {
                        if (!node.parameters) {
                            node.parameters = {};
                        }
                        node.parameters[paramKey] = e.target.value;
                        this.canvasManager.saveState();
                        
                        // If notes changed, update node display
                        if (paramKey === 'notes') {
                            this.canvasManager.renderNode(node);
                        }
                    });
                });
            }
            
            // Load hierarchical classification fields for raw materials asynchronously
            if (node.type === 'raw-material') {
                this.loadRawMaterialTaxonomyFields(node, container);
            }
            
            // Setup transport settings event listeners if transport node
            if (node.type === 'transport') {
                this.setupTransportSettingsListeners(node);
            }
            
            // Setup packaging group settings event listeners if packaging-group node
            if (node.type === 'packaging-group') {
                this.setupPackagingGroupSettingsListeners(node);
            }
        }
        
        renderPackagingGroupSettings(node) {
            const holdsQuantity = node.parameters?.holdsQuantity || 0;
            const holdsItemType = node.parameters?.holdsItemType || '';
            
            // Get all other groups, packaging items, and products for the dropdown
            const otherGroups = Array.from(this.canvasManager.nodes.values())
                .filter(n => n.type === 'packaging-group' && n.id !== node.id)
                .map(n => ({ value: `group-${n.id}`, label: n.name }));
            
            const packagingItems = Array.from(this.canvasManager.nodes.values())
                .filter(n => n.type === 'packaging')
                .map(n => ({ value: `packaging-${n.id}`, label: n.name }));
            
            const products = Array.from(this.canvasManager.nodes.values())
                .filter(n => n.type === 'product')
                .map(n => ({ value: `product-${n.id}`, label: n.name }));
            
            const allOptions = [
                { value: '', label: 'Select item type...' },
                ...otherGroups,
                ...products,
                ...packagingItems
            ];
            
            let html = '<div class="parameter-group mt-3"><h6>Group Capacity</h6>';
            
            html += `
                <div class="mb-3">
                    <label class="form-label">Holds Item Type</label>
                    <select id="eprGroupHoldsItemType" class="form-select form-select-sm">
                        ${allOptions.map(opt => 
                            `<option value="${opt.value}" ${holdsItemType === opt.value ? 'selected' : ''}>${this.escapeHtml(opt.label)}</option>`
                        ).join('')}
                    </select>
                    <small class="form-text text-muted">Select what type of item this group holds (e.g., Box Group holds Bottle Group)</small>
                </div>
                <div class="mb-3">
                    <label class="form-label">Holds Quantity</label>
                    <input type="number" id="eprGroupHoldsQuantity" class="form-control form-control-sm" 
                           value="${holdsQuantity}" placeholder="0" min="0" step="1" />
                    <small class="form-text text-muted">How many of the selected item type this group holds (e.g., 24 bottles per box)</small>
                </div>
            `;
            
            // Show contained items
            const containedItems = node.containedItems || [];
            const packagingItemsInGroup = containedItems.filter(itemId => {
                const itemNode = this.canvasManager.nodes.get(itemId);
                return itemNode && itemNode.type === 'packaging';
            });
            const rawMaterialsInGroup = containedItems.filter(itemId => {
                const itemNode = this.canvasManager.nodes.get(itemId);
                return itemNode && itemNode.type === 'raw-material';
            });
            
            html += `
                <div class="mt-3">
                    <h6>Contained Items</h6>
                    <div class="small">
                        <div>Packaging Items: ${packagingItemsInGroup.length}</div>
                        <div>Raw Materials: ${rawMaterialsInGroup.length}</div>
                    </div>
                    <small class="form-text text-muted">Right-click on packaging items or raw materials to move them to this group</small>
                </div>
            `;
            
            html += '</div>';
            
            return html;
        }
        
        setupPackagingGroupSettingsListeners(node) {
            const container = document.getElementById('eprParametersContent');
            if (!container) return;
            
            // Update holds item type
            const holdsItemTypeSelect = container.querySelector('#eprGroupHoldsItemType');
            if (holdsItemTypeSelect) {
                holdsItemTypeSelect.addEventListener('change', (e) => {
                    if (!node.parameters) {
                        node.parameters = {};
                    }
                    node.parameters.holdsItemType = e.target.value;
                    // Update connections to show/hide quantity circles
                    this.canvasManager.updateConnections();
                    this.canvasManager.saveState();
                });
            }
            
            // Update holds quantity
            const holdsQuantityInput = container.querySelector('#eprGroupHoldsQuantity');
            if (holdsQuantityInput) {
                holdsQuantityInput.addEventListener('input', (e) => {
                    if (!node.parameters) {
                        node.parameters = {};
                    }
                    node.parameters.holdsQuantity = parseFloat(e.target.value) || 0;
                    // Update connections to show/hide quantity circles
                    this.canvasManager.updateConnections();
                    this.canvasManager.saveState();
                });
            }
        }
        
        renderTransportSettings(node) {
            const transportTypes = node.transportTypes || [];
            let html = '<div class="parameter-group mt-3"><h6>Transport Types</h6>';
            
            transportTypes.forEach((transport, index) => {
                html += `
                    <div class="transport-entry mb-2 p-2 border rounded" data-transport-index="${index}">
                        <div class="row g-2">
                            <div class="col-6">
                                <select class="form-select form-select-sm transport-type-select" data-index="${index}">
                                    <option value="Ship" ${transport.type === 'Ship' ? 'selected' : ''}>Ship</option>
                                    <option value="Rail" ${transport.type === 'Rail' ? 'selected' : ''}>Rail</option>
                                    <option value="Truck" ${transport.type === 'Truck' ? 'selected' : ''}>Truck</option>
                                    <option value="Van" ${transport.type === 'Van' ? 'selected' : ''}>Van</option>
                                    <option value="Air" ${transport.type === 'Air' ? 'selected' : ''}>Air</option>
                                </select>
                            </div>
                            <div class="col-4">
                                <input type="number" class="form-control form-control-sm transport-distance-input" 
                                       data-index="${index}" value="${transport.distance}" placeholder="Distance" min="0" step="0.1" />
                            </div>
                            <div class="col-2">
                                <button type="button" class="btn btn-sm btn-danger transport-delete-btn" data-index="${index}" title="Delete">
                                    <i class="bi bi-trash"></i>
                                </button>
                            </div>
                        </div>
                    </div>
                `;
            });
            
            html += `
                <button type="button" class="btn btn-sm btn-secondary mt-2" id="eprAddTransportTypeBtn">
                    <i class="bi bi-plus"></i> Add Transport Type
                </button>
            </div>`;
            
            return html;
        }
        
        setupTransportSettingsListeners(node) {
            const container = document.getElementById('eprParametersContent');
            if (!container) return;
            
            // Update transport type
            container.querySelectorAll('.transport-type-select').forEach(select => {
                select.addEventListener('change', (e) => {
                    const index = parseInt(e.target.dataset.index);
                    if (node.transportTypes && node.transportTypes[index]) {
                        node.transportTypes[index].type = e.target.value;
                        this.updateTransportNodeName(node);
                        // Immediately update transport box display
                        this.canvasManager.updateConnections();
                        this.canvasManager.saveState();
                    }
                });
            });
            
            // Update transport distance
            container.querySelectorAll('.transport-distance-input').forEach(input => {
                input.addEventListener('input', (e) => {
                    const index = parseInt(e.target.dataset.index);
                    if (node.transportTypes && node.transportTypes[index]) {
                        node.transportTypes[index].distance = parseFloat(e.target.value) || 0;
                        // Immediately update transport box display
                        this.canvasManager.updateConnections();
                        this.canvasManager.saveState();
                    }
                });
            });
            
            // Delete transport type
            container.querySelectorAll('.transport-delete-btn').forEach(btn => {
                btn.addEventListener('click', (e) => {
                    const index = parseInt(e.target.closest('.transport-delete-btn').dataset.index);
                    if (node.transportTypes && node.transportTypes.length > 1) {
                        node.transportTypes.splice(index, 1);
                        this.loadNodeParameters(node); // Reload to update UI
                        // Immediately update transport box display
                        this.canvasManager.updateConnections();
                        this.canvasManager.saveState();
                    } else {
                        alert('Transport node must have at least one transport type');
                    }
                });
            });
            
            // Add new transport type
            const addBtn = container.querySelector('#eprAddTransportTypeBtn');
            if (addBtn) {
                addBtn.addEventListener('click', () => {
                    if (!node.transportTypes) {
                        node.transportTypes = [];
                    }
                    node.transportTypes.push({ type: 'Ship', distance: 0 });
                    this.loadNodeParameters(node); // Reload to update UI
                    this.updateTransportNodeName(node);
                    // Immediately update transport box display
                    this.canvasManager.updateConnections();
                    this.canvasManager.saveState();
                });
            }
        }
        
        updateTransportNodeName(node) {
            if (node.transportTypes && node.transportTypes.length > 0) {
                node.name = `Transport (${node.transportTypes.map(t => t.type).join(', ')})`;
                // Update the node display
                const nodeEl = document.querySelector(`[data-node-id="${node.id}"]`);
                if (nodeEl) {
                    const titleEl = nodeEl.querySelector('.epr-node-title');
                    if (titleEl) {
                        titleEl.textContent = node.name;
                    }
                    // Update transport info display
                    const transportInfoEl = nodeEl.querySelector('.epr-transport-info');
                    if (transportInfoEl) {
                        transportInfoEl.textContent = node.transportTypes.map(t => `${t.type}: ${t.distance}km`).join(', ');
                    }
                }
            }
        }
        
        async loadRawMaterialTaxonomyFields(node, container) {
            const taxonomyId = node.parameters?.taxonomyId;
            if (!taxonomyId) {
                return;
            }
            
            let html = '<div class="parameter-group mt-3"><h6>Material Classification</h6>';
            
            // Get connected distribution countries to determine required level
            const connectedCountries = this.getConnectedDistributionCountries(node);
            let maxRequiredLevel = 1;
            
            if (connectedCountries.length > 0) {
                try {
                    const reqResponse = await fetch(`/api/visual-editor/material-taxonomy/requirements?taxonomyId=${taxonomyId}&countryCodes=${connectedCountries.join('&countryCodes=')}`);
                    if (reqResponse.ok) {
                        const reqData = await reqResponse.json();
                        maxRequiredLevel = reqData.maxRequiredLevel || 1;
                    }
                } catch (error) {
                    console.error('Error fetching taxonomy requirements:', error);
                }
            }
            
            // Level 1 is already set (from toolbar selection)
            html += `<div class="mb-2">
                <label class="form-label small">Level 1 Classification</label>
                <input type="text" class="form-control form-control-sm" 
                       value="${this.escapeHtml(node.parameters?.level1Classification || '')}" readonly>
            </div>`;
            
            // Render Level 2 classification (always show if Level 1 is selected)
            html += `<div class="mb-2" id="taxonomy-level-2-container">
                <label class="form-label small">Level 2 Classification${maxRequiredLevel >= 2 ? ' <span style="color: red;">*</span>' : ''}</label>
                <select class="form-select form-select-sm epr-taxonomy-select" 
                        data-level="2" 
                        data-parent-id="${taxonomyId}"
                        data-param-key="level2Classification"
                        ${maxRequiredLevel >= 2 ? 'required' : ''}>
                    <option value="">Select Level 2...</option>
                </select>
            </div>`;
            
            html += '<small class="form-text text-muted">Classification requirements depend on distribution countries</small></div>';
            
            // Append to container
            container.insertAdjacentHTML('beforeend', html);
            
            // Load Level 2 options
            const level2Select = container.querySelector('#taxonomy-level-2-container select');
            if (level2Select) {
                await this.loadTaxonomyOptions(level2Select, 2, taxonomyId, node);
                
                // Set up change handler to dynamically show next levels
                level2Select.addEventListener('change', async (e) => {
                    const selectedId = parseInt(e.target.value);
                    const selectedOption = e.target.options[e.target.selectedIndex];
                    
                    if (node && this.canvasManager.nodes.has(node.id)) {
                        if (!node.parameters) node.parameters = {};
                        node.parameters.level2Classification = selectedOption.textContent;
                        node.parameters.level2TaxonomyId = selectedId;
                        node.parameters.level2DisplayName = selectedOption.textContent; // Store DisplayName
                        
                        // Fetch full taxonomy data to get DisplayName
                        try {
                            const taxonomyResponse = await fetch(`/api/visual-editor/material-taxonomy/${selectedId}`);
                            if (taxonomyResponse.ok) {
                                const taxonomyData = await taxonomyResponse.json();
                                if (taxonomyData.displayName) {
                                    node.parameters.level2DisplayName = taxonomyData.displayName;
                                }
                            }
                        } catch (error) {
                            console.error('Error fetching taxonomy DisplayName:', error);
                        }
                        
                        this.canvasManager.saveState();
                        this.canvasManager.renderNode(node); // Refresh node display
                        
                        // Remove existing Level 3+ containers
                        for (let l = 3; l <= 5; l++) {
                            const existingContainer = container.querySelector(`#taxonomy-level-${l}-container`);
                            if (existingContainer) {
                                existingContainer.remove();
                            }
                        }
                        
                        // If a level is selected, check if it has children and show next level
                        if (selectedId) {
                            await this.renderNextTaxonomyLevel(container, 3, selectedId, node, maxRequiredLevel);
                        }
                    }
                });
                
                // If Level 2 is already selected, show Level 3
                if (node.parameters?.level2TaxonomyId) {
                    await this.renderNextTaxonomyLevel(container, 3, node.parameters.level2TaxonomyId, node, maxRequiredLevel);
                }
            }
        }
        
        async renderNextTaxonomyLevel(container, level, parentId, node, maxRequiredLevel) {
            if (level > 5) return;
            
            // Check if parent has children
            try {
                const response = await fetch(`/api/visual-editor/material-taxonomy/${parentId}/children?level=${level}`);
                if (!response.ok) return;
                
                const children = await response.json();
                if (children.length === 0) return; // No children, stop here
                
                // Create container for this level
                const levelKey = `level${level}Classification`;
                const levelContainer = document.createElement('div');
                levelContainer.className = 'mb-2';
                levelContainer.id = `taxonomy-level-${level}-container`;
                levelContainer.innerHTML = `
                    <label class="form-label small">Level ${level} Classification${level <= maxRequiredLevel ? ' <span style="color: red;">*</span>' : ''}</label>
                    <select class="form-select form-select-sm epr-taxonomy-select" 
                            data-level="${level}" 
                            data-parent-id="${parentId}"
                            data-param-key="${levelKey}"
                            ${level <= maxRequiredLevel ? 'required' : ''}>
                        <option value="">Select Level ${level}...</option>
                    </select>
                `;
                
                // Insert before the small text-muted element
                const smallText = container.querySelector('.form-text.text-muted');
                if (smallText) {
                    smallText.parentNode.insertBefore(levelContainer, smallText);
                } else {
                    container.appendChild(levelContainer);
                }
                
                const selectElement = levelContainer.querySelector('select');
                await this.loadTaxonomyOptions(selectElement, level, parentId, node);
                
                // Set up change handler for this level
                selectElement.addEventListener('change', async (e) => {
                    const selectedId = parseInt(e.target.value);
                    const selectedOption = e.target.options[e.target.selectedIndex];
                    const paramKey = e.target.dataset.paramKey;
                    
                    if (node && this.canvasManager.nodes.has(node.id)) {
                        if (!node.parameters) node.parameters = {};
                        node.parameters[paramKey] = selectedOption.textContent;
                        node.parameters[`level${level}TaxonomyId`] = selectedId;
                        node.parameters[`level${level}DisplayName`] = selectedOption.textContent; // Store DisplayName
                        
                        // Fetch full taxonomy data to get DisplayName
                        try {
                            const taxonomyResponse = await fetch(`/api/visual-editor/material-taxonomy/${selectedId}`);
                            if (taxonomyResponse.ok) {
                                const taxonomyData = await taxonomyResponse.json();
                                if (taxonomyData.displayName) {
                                    node.parameters[`level${level}DisplayName`] = taxonomyData.displayName;
                                }
                            }
                        } catch (error) {
                            console.error('Error fetching taxonomy DisplayName:', error);
                        }
                        
                        this.canvasManager.saveState();
                        this.canvasManager.renderNode(node); // Refresh node display
                        
                        // Remove existing next level containers
                        for (let l = level + 1; l <= 5; l++) {
                            const existingContainer = container.querySelector(`#taxonomy-level-${l}-container`);
                            if (existingContainer) {
                                existingContainer.remove();
                            }
                        }
                        
                        // If a level is selected, check if it has children and show next level
                        if (selectedId) {
                            await this.renderNextTaxonomyLevel(container, level + 1, selectedId, node, maxRequiredLevel);
                        }
                    }
                });
                
                // If this level is already selected, show next level
                if (node.parameters?.[`level${level}TaxonomyId`]) {
                    await this.renderNextTaxonomyLevel(container, level + 1, node.parameters[`level${level}TaxonomyId`], node, maxRequiredLevel);
                }
            } catch (error) {
                console.error(`Error rendering level ${level}:`, error);
            }
        }
        
        getConnectedDistributionCountries(node) {
            const countries = new Set();
            this.canvasManager.connections.forEach(conn => {
                if (conn.from === node.id) {
                    const toNode = this.canvasManager.nodes.get(conn.to);
                    if (toNode?.type === 'distribution' && toNode.parameters?.country) {
                        countries.add(toNode.parameters.country);
                    }
                }
            });
            return Array.from(countries);
        }
        
        async loadTaxonomyOptions(selectElement, level, parentId, node) {
            try {
                const response = await fetch(`/api/visual-editor/material-taxonomy/${parentId}/children?level=${level}`);
                if (!response.ok) return;
                
                const children = await response.json();
                children.forEach(child => {
                    const option = document.createElement('option');
                    option.value = child.id;
                    option.textContent = child.name; // This is the DisplayName from API
                    if (node.parameters?.[`level${level}TaxonomyId`] === child.id) {
                        option.selected = true;
                    }
                    selectElement.appendChild(option);
                });
            } catch (error) {
                console.error('Error loading taxonomy options:', error);
            }
        }
        
        renderParameterField(param) {
            const inputId = `eprParam-${param.key}`;
            const requiredAttr = param.required ? 'required' : '';
            const requiredStar = param.required ? ' <span style="color: red;">*</span>' : '';
            const disabledAttr = param.disabled ? 'disabled' : '';
            
            if (param.type === 'textarea') {
                return `
                    <div class="parameter-group">
                        <label for="${inputId}">${param.label}${requiredStar}</label>
                        <textarea id="${inputId}" class="form-control epr-parameter-input" data-param-key="${param.key}" rows="3" ${requiredAttr} ${disabledAttr}>${this.escapeHtml(param.value)}</textarea>
                    </div>
                `;
            } else if (param.type === 'number') {
                return `
                    <div class="parameter-group">
                        <label for="${inputId}">${param.label}${requiredStar}</label>
                        <input type="number" id="${inputId}" class="form-control epr-parameter-input" data-param-key="${param.key}" value="${param.value}" step="0.01" ${requiredAttr} ${disabledAttr} />
                    </div>
                `;
            } else if (param.type === 'select' && param.options) {
                const optionsHtml = param.options.map(opt => 
                    `<option value="${this.escapeHtml(opt.value)}" ${opt.value === param.value ? 'selected' : ''}>${this.escapeHtml(opt.label)}</option>`
                ).join('');
                return `
                    <div class="parameter-group">
                        <label for="${inputId}">${param.label}${requiredStar}</label>
                        <select id="${inputId}" class="form-select epr-parameter-input" data-param-key="${param.key}" ${requiredAttr} ${disabledAttr}>
                            ${optionsHtml}
                        </select>
                    </div>
                `;
            } else {
                return `
                    <div class="parameter-group">
                        <label for="${inputId}">${param.label}${requiredStar}</label>
                        <input type="text" id="${inputId}" class="form-control epr-parameter-input" data-param-key="${param.key}" value="${this.escapeHtml(param.value)}" ${requiredAttr} ${disabledAttr} />
                    </div>
                `;
            }
        }
        
        clearParameters() {
            this.currentNode = null;
            const container = document.getElementById('eprParametersContent');
            if (container) {
                container.innerHTML = '<p class="text-muted small">Select a node to view/edit parameters</p>';
            }
        }
        
        escapeHtml(text) {
            if (text == null) return '';
            const div = document.createElement('div');
            div.textContent = text;
            return div.innerHTML;
        }
    }
    
    // ============================================================================
    // ACTIONS TOOLBAR
    // ============================================================================
    class EPRActionsToolbar {
        constructor(canvasManager) {
            console.log('[EPR Actions Toolbar] Creating ActionsToolbar...');
            this.canvasManager = canvasManager;
            this.isDragMode = false;
            this.isTransportMode = false;
            this.init();
            // Setup toolbar dragging immediately - make all toolbars draggable by default
            setTimeout(() => {
                this.setupToolbarDragging();
                console.log('[EPR Actions Toolbar] Toolbar dragging setup complete');
            }, 100);
        }
        
        init() {
            this.setupEventListeners();
            this.setupToolbarDragging();
        }
        
        setupEventListeners() {
            console.log('[EPR Actions Toolbar] Setting up event listeners...');
            
            const dragModeBtn = document.getElementById('eprDragModeBtn');
            if (dragModeBtn) {
                dragModeBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    console.log('[EPR Actions] Drag mode clicked');
                    this.toggleDragMode();
                });
                console.log('[EPR Actions] Drag mode button handler attached');
            } else {
                console.warn('[EPR Actions] Drag mode button not found!');
            }
            
            const zoomInBtn = document.getElementById('eprZoomInBtn');
            if (zoomInBtn) {
                zoomInBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    console.log('[EPR Actions] Zoom in clicked');
                    this.canvasManager.zoomIn();
                });
                console.log('[EPR Actions] Zoom in button handler attached');
            } else {
                console.warn('[EPR Actions] Zoom in button not found!');
            }
            
            const zoomOutBtn = document.getElementById('eprZoomOutBtn');
            if (zoomOutBtn) {
                zoomOutBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    console.log('[EPR Actions] Zoom out clicked');
                    this.canvasManager.zoomOut();
                });
                console.log('[EPR Actions] Zoom out button handler attached');
            } else {
                console.warn('[EPR Actions] Zoom out button not found!');
            }
            
            const undoBtn = document.getElementById('eprUndoBtn');
            if (undoBtn) {
                undoBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    console.log('[EPR Actions] Undo clicked');
                    this.canvasManager.undo();
                });
                console.log('[EPR Actions] Undo button handler attached');
            } else {
                console.warn('[EPR Actions] Undo button not found!');
            }
            
            const fitToCanvasBtn = document.getElementById('eprFitToCanvasBtn');
            if (fitToCanvasBtn) {
                fitToCanvasBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    console.log('[EPR Actions] Fit to canvas clicked');
                    this.canvasManager.fitToCanvas();
                });
                console.log('[EPR Actions] Fit to canvas button handler attached');
            } else {
                console.warn('[EPR Actions] Fit to canvas button not found!');
            }
            
            const arrangeBtn = document.getElementById('eprArrangeBtn');
            if (arrangeBtn) {
                arrangeBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    console.log('[EPR Actions] Arrange clicked');
                    this.canvasManager.arrangeNodes();
                });
                console.log('[EPR Actions] Arrange button handler attached');
            } else {
                console.warn('[EPR Actions] Arrange button not found!');
            }
            
            const toggleGridBtn = document.getElementById('eprToggleGridBtn');
            if (toggleGridBtn) {
                toggleGridBtn.addEventListener('click', (e) => {
                    // Don't prevent default - let button work normally
                    console.log('[EPR Actions] Toggle grid clicked');
                    this.canvasManager.toggleGrid();
                });
                console.log('[EPR Actions] Toggle grid button handler attached');
            } else {
                console.warn('[EPR Actions] Toggle grid button not found!');
            }
            
            const snapToGridBtn = document.getElementById('eprSnapToGridBtn');
            if (snapToGridBtn) {
                snapToGridBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    console.log('[EPR Actions] Snap to grid clicked');
                    this.canvasManager.toggleSnapToGrid();
                });
                console.log('[EPR Actions] Snap to grid button handler attached');
            } else {
                console.warn('[EPR Actions] Snap to grid button not found!');
            }
            
            // Transport button
            // REMOVED: Transport button should NOT create transport nodes
            // Transport nodes can ONLY be added via right-click on connection lines
            // The button is kept in UI but disabled - users should use connection context menu
            // Transport button removed - transport nodes are added via connection context menu
            
            const toggleToolbarsBtn = document.getElementById('eprToggleToolbarsBtn');
            if (toggleToolbarsBtn) {
                toggleToolbarsBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    this.toggleToolbarsVisibility();
                });
                console.log('[EPR Actions] Toggle toolbars button handler attached');
            } else {
                console.warn('[EPR Actions] Toggle toolbars button not found!');
            }
            
            // Toggle Notes visibility button
            const toggleNotesBtn = document.getElementById('eprToggleNotesBtn');
            if (toggleNotesBtn) {
                let notesVisible = true;
                toggleNotesBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    notesVisible = !notesVisible;
                    const noteElements = document.querySelectorAll('.epr-node-notes');
                    noteElements.forEach(el => {
                        el.style.display = notesVisible ? 'block' : 'none';
                    });
                    toggleNotesBtn.querySelector('i').className = notesVisible ? 'bi bi-sticky-fill' : 'bi bi-sticky';
                    toggleNotesBtn.querySelector('span').textContent = notesVisible ? 'Hide Notes' : 'Show Notes';
                });
                console.log('[EPR Actions] Toggle notes button handler attached');
            } else {
                console.warn('[EPR Actions] Toggle notes button not found!');
            }
            
            // Clear Canvas button
            const clearCanvasBtn = document.getElementById('eprClearCanvasBtn');
            if (clearCanvasBtn) {
                clearCanvasBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    if (confirm('Are you sure you want to clear the canvas? This will remove all nodes and connections.')) {
                        // Clear all nodes
                        this.canvasManager.nodes.clear();
                        this.canvasManager.connections = [];
                        this.canvasManager.selectedNode = null;
                        this.canvasManager.selectedNodes.clear();
                        
                        // Clear DOM
                        this.canvasManager.nodesLayer.innerHTML = '';
                        this.canvasManager.connectionsLayer.innerHTML = '';
                        
                        // Clear parameters panel
                        const paramsContainer = document.getElementById('eprParametersContent');
                        if (paramsContainer) {
                            paramsContainer.innerHTML = '<p class="text-muted small">Select a node to view/edit parameters</p>';
                        }
                        
                        console.log('[EPR Actions] Canvas cleared');
                    }
                });
                console.log('[EPR Actions] Clear canvas button handler attached');
            } else {
                console.warn('[EPR Actions] Clear canvas button not found!');
            }
            
            // Visibility dropdown checkboxes
            const visibilityNodeNotes = document.getElementById('eprVisibilityNodeNotes');
            const visibilityContents = document.getElementById('eprVisibilityContents');
            const visibilityTransportNodes = document.getElementById('eprVisibilityTransportNodes');
            const visibilityToolbars = document.getElementById('eprVisibilityToolbars');
            const visibilityPalettes = document.getElementById('eprVisibilityPalettes');
            
            if (visibilityNodeNotes) {
                visibilityNodeNotes.addEventListener('change', (e) => {
                    const visible = e.target.checked;
                    document.querySelectorAll('.epr-node-notes').forEach(el => {
                        el.style.display = visible ? 'block' : 'none';
                    });
                });
            }
            
            if (visibilityContents) {
                visibilityContents.addEventListener('change', (e) => {
                    const visible = e.target.checked;
                    // Hide/show nested contents list ONLY in Packaging Group nodes
                    // Find all packaging group node elements
                    document.querySelectorAll('.epr-packaging-group-node').forEach(groupNodeEl => {
                        // Find the nested items list inside this packaging group node
                        const itemsList = groupNodeEl.querySelector('.epr-group-items-list');
                        if (itemsList) {
                            itemsList.style.display = visible ? '' : 'none';
                        }
                    });
                });
            }
            
            if (visibilityTransportNodes) {
                visibilityTransportNodes.addEventListener('change', (e) => {
                    const visible = e.target.checked;
                    this.canvasManager.showTransportNodes = visible;
                    this.canvasManager.updateConnections();
                });
            }
            
            if (visibilityToolbars) {
                visibilityToolbars.addEventListener('change', (e) => {
                    const visible = e.target.checked;
                    document.querySelectorAll('.epr-floating-toolbar').forEach(el => {
                        el.style.display = visible ? '' : 'none';
                    });
                });
            }
            
            if (visibilityPalettes) {
                visibilityPalettes.addEventListener('change', (e) => {
                    const visible = e.target.checked;
                    document.querySelectorAll('.epr-types-panel').forEach(el => {
                        el.style.display = visible ? '' : 'none';
                    });
                });
            }
            
            // Split types toolbar button
            const splitTypesBtn = document.getElementById('eprSplitTypesBtn');
            if (splitTypesBtn) {
                console.log('[EPR Actions] Split button found, attaching handler');
                splitTypesBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    console.log('[EPR Actions] Split button clicked');
                    this.toggleTypesToolbarSplit();
                });
                // Also allow mousedown to ensure it works
                splitTypesBtn.addEventListener('mousedown', (e) => {
                    e.stopPropagation();
                });
                console.log('[EPR Actions] Split button handler attached');
            } else {
                console.warn('[EPR Actions] Split button not found!');
            }
            
            // Toolbar minimize buttons
            document.querySelectorAll('.epr-btn-close-toolbar').forEach(btn => {
                btn.addEventListener('click', (e) => {
                    e.stopPropagation();
                    e.preventDefault();
                    const toolbar = e.target.closest('.epr-floating-toolbar') || 
                                   e.target.closest('.epr-types-panel');
                    if (toolbar) {
                        const wasMinimized = toolbar.classList.contains('minimized');
                        toolbar.classList.toggle('minimized');
                        const icon = btn.querySelector('i');
                        if (icon) {
                            if (toolbar.classList.contains('minimized')) {
                                icon.className = 'bi bi-plus-lg';
                                btn.title = 'Expand';
                            } else {
                                icon.className = 'bi bi-dash';
                                btn.title = 'Minimize';
                                // If panel was minimized and is now expanded, re-render raw materials if needed
                                if (wasMinimized && toolbar.id === 'eprTypesPanelRawMaterials') {
                                    if (window.EPRVisualEditor && window.EPRVisualEditor.typesToolbar) {
                                        console.log('[EPR Actions] Re-rendering raw materials after panel expansion');
                                        window.EPRVisualEditor.typesToolbar.renderRawMaterials();
                                    }
                                }
                            }
                        }
                    }
                });
            });
            
            // Toolbar lock buttons - controls whether toolbars move together (locked) or independently (unlocked)
            document.querySelectorAll('.epr-btn-lock-toolbar').forEach(btn => {
                btn.addEventListener('click', (e) => {
                    e.stopPropagation();
                    e.preventDefault();
                    const toolbar = e.target.closest('.epr-floating-toolbar') || 
                                   e.target.closest('.epr-types-panel');
                    if (toolbar) {
                        const toolbarId = toolbar.id || toolbar.className;
                        const isLocked = btn.classList.contains('locked');
                        
                        // REVERSED LOGIC: open lock (unlock icon) = independent, closed lock (lock-fill) = moves together
                        if (isLocked) {
                            // Currently locked (closed lock icon) - unlock to make independent (open lock icon)
                            btn.classList.remove('locked');
                            const icon = btn.querySelector('i');
                            if (icon) {
                                icon.className = 'bi bi-unlock'; // Open lock = independent
                            }
                            btn.title = 'Lock: Move together with other locked panels | Unlock: Move independently';
                            toolbar.setAttribute('data-toolbar-locked', 'false');
                        } else {
                            // Currently unlocked (open lock icon) - lock to move together (closed lock icon)
                            btn.classList.add('locked');
                            const icon = btn.querySelector('i');
                            if (icon) {
                                icon.className = 'bi bi-lock-fill'; // Closed lock = moves together
                            }
                            btn.title = 'Lock: Move together with other locked panels | Unlock: Move independently';
                            toolbar.setAttribute('data-toolbar-locked', 'true');
                        }
                    }
                });
            });
            
            // Double-click header to toggle minimize
            document.querySelectorAll('.epr-toolbar-header').forEach(header => {
                header.addEventListener('dblclick', (e) => {
                    // Don't toggle if clicking on buttons
                    if (e.target.closest('button')) return;
                    
                    const toolbar = header.closest('.epr-floating-toolbar') || 
                                   header.closest('.epr-types-panel');
                    if (toolbar) {
                        const wasMinimized = toolbar.classList.contains('minimized');
                        toolbar.classList.toggle('minimized');
                        const btn = toolbar.querySelector('.epr-btn-close-toolbar');
                        if (btn) {
                            const icon = btn.querySelector('i');
                            if (icon) {
                                if (toolbar.classList.contains('minimized')) {
                                    icon.className = 'bi bi-plus-lg';
                                    btn.title = 'Expand';
                                } else {
                                    icon.className = 'bi bi-dash';
                                    btn.title = 'Minimize';
                                    // If panel was minimized and is now expanded, re-render raw materials if needed
                                    if (wasMinimized && toolbar.id === 'eprTypesPanelRawMaterials') {
                                        if (window.EPRVisualEditor && window.EPRVisualEditor.typesToolbar) {
                                            console.log('[EPR Actions] Re-rendering raw materials after panel expansion (double-click)');
                                            window.EPRVisualEditor.typesToolbar.renderRawMaterials();
                                        }
                                    }
                                }
                            }
                        }
                    }
                });
            });
        }
        
        setupToolbarDragging() {
            try {
                // Track toolbar connections (which toolbars are snapped together)
                if (!window.eprToolbarConnections) {
                    window.eprToolbarConnections = new Map(); // toolbarId -> Set of connected toolbar IDs
                }
            } catch (err) {
                console.error('[EPR Actions] Error initializing toolbar connections:', err);
                return;
            }
            
            // Helper function to get all connected toolbars recursively
            const getConnectedToolbars = (toolbarId, visited = new Set()) => {
                if (visited.has(toolbarId)) return new Set();
                visited.add(toolbarId);
                const connected = new Set([toolbarId]);
                const connections = window.eprToolbarConnections.get(toolbarId) || new Set();
                connections.forEach(connectedId => {
                    const subConnected = getConnectedToolbars(connectedId, visited);
                    subConnected.forEach(id => connected.add(id));
                });
                return connected;
            };
            
            // Helper function to connect two toolbars
            const connectToolbars = (id1, id2) => {
                if (!window.eprToolbarConnections.has(id1)) {
                    window.eprToolbarConnections.set(id1, new Set());
                }
                if (!window.eprToolbarConnections.has(id2)) {
                    window.eprToolbarConnections.set(id2, new Set());
                }
                window.eprToolbarConnections.get(id1).add(id2);
                window.eprToolbarConnections.get(id2).add(id1);
            };
            
            // Helper function to disconnect toolbars
            const disconnectToolbars = (id1, id2) => {
                if (window.eprToolbarConnections.has(id1)) {
                    window.eprToolbarConnections.get(id1).delete(id2);
                }
                if (window.eprToolbarConnections.has(id2)) {
                    window.eprToolbarConnections.get(id2).delete(id1);
                }
            };
            
            // Setup dragging for all toolbars and panels independently
            const setupDrag = (toolbar) => {
                if (!toolbar) {
                    console.warn('[EPR Actions] Toolbar is null');
                    return;
                }
                
                const header = toolbar.querySelector('.epr-toolbar-header');
                if (!header) {
                    console.warn('[EPR Actions] Toolbar header not found for:', toolbar.id || toolbar.className);
                    return;
                }
                
                // Add unsnap button to header if not exists
                if (!header.querySelector('.epr-btn-unsnap')) {
                    const unsnapBtn = document.createElement('button');
                    unsnapBtn.className = 'epr-btn-unsnap';
                    unsnapBtn.innerHTML = '<i class="bi bi-unlock"></i>';
                    unsnapBtn.title = 'Unsnap from connected toolbars';
                    unsnapBtn.style.cssText = 'background: none; border: none; color: #6c757d; cursor: pointer; padding: 0.25rem; font-size: 0.9rem; opacity: 0.7;';
                    unsnapBtn.addEventListener('click', (e) => {
                        e.stopPropagation();
                        e.preventDefault();
                        const toolbarId = toolbar.id;
                        if (!toolbarId) return;
                        const connections = window.eprToolbarConnections.get(toolbarId);
                        if (connections && connections.size > 0) {
                            // Disconnect from all connected toolbars
                            const connectionsCopy = new Set(connections); // Copy to avoid modification during iteration
                            connectionsCopy.forEach(connectedId => {
                                disconnectToolbars(toolbarId, connectedId);
                                // Update unsnap button visibility for connected toolbar
                                const connectedToolbar = document.getElementById(connectedId);
                                if (connectedToolbar) {
                                    const connectedHeader = connectedToolbar.querySelector('.epr-toolbar-header');
                                    if (connectedHeader) {
                                        const connectedUnsnapBtn = connectedHeader.querySelector('.epr-btn-unsnap');
                                        if (connectedUnsnapBtn) {
                                            const connectedConnections = window.eprToolbarConnections.get(connectedId);
                                            if (!connectedConnections || connectedConnections.size === 0) {
                                                connectedUnsnapBtn.style.display = 'none';
                                            }
                                        }
                                    }
                                }
                            });
                            // Hide unsnap button if no connections
                            unsnapBtn.style.display = 'none';
                        }
                    });
                    // Only show if toolbar has connections
                    if (!toolbar.id || !window.eprToolbarConnections.has(toolbar.id)) {
                        unsnapBtn.style.display = 'none';
                    } else {
                        const connections = window.eprToolbarConnections.get(toolbar.id);
                        if (!connections || connections.size === 0) {
                            unsnapBtn.style.display = 'none';
                        }
                    }
                    try {
                        header.appendChild(unsnapBtn);
                    } catch (err) {
                        console.error('[EPR Actions] Error appending unsnap button:', err);
                    }
                }
                
                let isDragging = false;
                let startX = 0;
                let startY = 0;
                let initialX = 0;
                let initialY = 0;
                let connectedInitialPositions = new Map();
                
                const handleMouseDown = (e) => {
                    // Don't drag if clicking buttons or interactive elements
                    if (e.target.closest('button') || 
                        e.target.closest('input') ||
                        e.target.closest('select') ||
                        e.target.closest('.epr-btn-close-toolbar') || 
                        e.target.closest('.epr-btn-split-toolbar') ||
                        e.target.closest('i')) {
                        return;
                    }
                    
                    // Only drag from header area
                    if (!header.contains(e.target)) {
                        return;
                    }
                    
                    isDragging = true;
                    startX = e.clientX;
                    startY = e.clientY;
                    
                    const rect = toolbar.getBoundingClientRect();
                    initialX = rect.left;
                    initialY = rect.top;
                    
                    // Store initial positions of all connected toolbars
                    if (toolbar.id) {
                        try {
                            const connectedIds = getConnectedToolbars(toolbar.id);
                            connectedIds.forEach(connectedId => {
                                const connectedToolbar = document.getElementById(connectedId);
                                if (connectedToolbar) {
                                    const connectedRect = connectedToolbar.getBoundingClientRect();
                                    connectedInitialPositions.set(connectedId, {
                                        x: connectedRect.left,
                                        y: connectedRect.top
                                    });
                                }
                            });
                        } catch (err) {
                            console.error('[EPR Actions] Error getting connected toolbars:', err);
                        }
                    }
                    
                    toolbar.style.cursor = 'grabbing';
                    header.style.cursor = 'grabbing';
                    header.style.userSelect = 'none';
                    
                    document.addEventListener('mousemove', handleMouseMove);
                    document.addEventListener('mouseup', handleMouseUp);
                    e.preventDefault();
                    e.stopPropagation();
                };
                
                const handleMouseMove = (e) => {
                    if (!isDragging) return;
                    
                    const deltaX = e.clientX - startX;
                    const deltaY = e.clientY - startY;
                    
                    let newX = initialX + deltaX;
                    let newY = initialY + deltaY;
                    
                    // Check if this toolbar is locked
                    const isLocked = toolbar.getAttribute('data-toolbar-locked') === 'true';
                    
                    // Move all connected toolbars together ONLY if this toolbar is locked
                    if (toolbar.id && isLocked) {
                        try {
                            const connectedIds = getConnectedToolbars(toolbar.id);
                            connectedIds.forEach(connectedId => {
                                const connectedToolbar = document.getElementById(connectedId);
                                if (connectedToolbar && connectedId !== toolbar.id) {
                                    // Only move connected toolbar if it's also locked
                                    const connectedIsLocked = connectedToolbar.getAttribute('data-toolbar-locked') === 'true';
                                    if (connectedIsLocked) {
                                        const connectedInitial = connectedInitialPositions.get(connectedId);
                                        if (connectedInitial) {
                                            connectedToolbar.style.left = `${connectedInitial.x + deltaX}px`;
                                            connectedToolbar.style.top = `${connectedInitial.y + deltaY}px`;
                                            connectedToolbar.style.right = 'auto';
                                            connectedToolbar.style.bottom = 'auto';
                                        }
                                    }
                                }
                            });
                        } catch (err) {
                            console.error('[EPR Actions] Error moving connected toolbars:', err);
                        }
                    }
                    
                    // Snap to other toolbars/panels when close (within 20px) and auto-lock
                    // Snap works regardless of lock state, but auto-locks when snapped
                    let snappedTo = null;
                    let snapPosition = null; // 'left', 'right', 'top', 'bottom'
                    
                    const snapDistance = 20;
                    const allToolbars = Array.from(document.querySelectorAll('.epr-floating-toolbar, .epr-types-panel')).filter(t => {
                        if (t === toolbar || !t.id || !toolbar.id) return false;
                        // Don't snap to already connected toolbars
                        const tId = t.id;
                        const toolbarId = toolbar.id;
                        try {
                            const connections = window.eprToolbarConnections.get(toolbarId);
                            return !(connections && connections.has(tId));
                        } catch (err) {
                            console.error('[EPR Actions] Error checking toolbar connections:', err);
                            return false;
                        }
                    });
                        
                        allToolbars.forEach(otherToolbar => {
                            const otherRect = otherToolbar.getBoundingClientRect();
                            const currentRect = toolbar.getBoundingClientRect();
                            const otherId = otherToolbar.id;
                            
                            // Check for snapping LEFT of other toolbar (toolbar goes to the left)
                            if (Math.abs((newX + currentRect.width) - otherRect.left) < snapDistance &&
                                Math.abs(newY - otherRect.top) < Math.max(currentRect.height, otherRect.height)) {
                                newX = otherRect.left - currentRect.width;
                                newY = otherRect.top; // Align tops
                                snappedTo = otherId;
                                snapPosition = 'left';
                            }
                            // Check for snapping RIGHT of other toolbar (toolbar goes to the right)
                            else if (Math.abs(newX - (otherRect.left + otherRect.width)) < snapDistance &&
                                     Math.abs(newY - otherRect.top) < Math.max(currentRect.height, otherRect.height)) {
                                newX = otherRect.left + otherRect.width;
                                newY = otherRect.top; // Align tops
                                snappedTo = otherId;
                                snapPosition = 'right';
                            }
                            // Check for snapping ABOVE other toolbar (toolbar goes above)
                            else if (Math.abs((newY + currentRect.height) - otherRect.top) < snapDistance &&
                                     Math.abs(newX - otherRect.left) < Math.max(currentRect.width, otherRect.width)) {
                                newX = otherRect.left; // Align lefts
                                newY = otherRect.top - currentRect.height;
                                snappedTo = otherId;
                                snapPosition = 'top';
                            }
                            // Check for snapping BELOW other toolbar (toolbar goes below)
                            else if (Math.abs(newY - (otherRect.top + otherRect.height)) < snapDistance &&
                                     Math.abs(newX - otherRect.left) < Math.max(currentRect.width, otherRect.width)) {
                                newX = otherRect.left; // Align lefts
                                newY = otherRect.top + otherRect.height;
                                snappedTo = otherId;
                                snapPosition = 'bottom';
                            }
                        });
                        
                        // If snapped, auto-lock both toolbars and connect them
                        if (snappedTo && toolbar.id && snappedTo) {
                            const otherToolbar = document.getElementById(snappedTo);
                            if (otherToolbar) {
                                // Auto-lock both toolbars
                                toolbar.setAttribute('data-toolbar-locked', 'true');
                                otherToolbar.setAttribute('data-toolbar-locked', 'true');
                                
                                // Update lock button icons
                                const lockBtn = header.querySelector('.epr-btn-lock-toolbar');
                                if (lockBtn) {
                                    lockBtn.classList.add('locked');
                                    const icon = lockBtn.querySelector('i');
                                    if (icon) icon.className = 'bi bi-lock-fill';
                                }
                                
                                const otherHeader = otherToolbar.querySelector('.epr-toolbar-header');
                                if (otherHeader) {
                                    const otherLockBtn = otherHeader.querySelector('.epr-btn-lock-toolbar');
                                    if (otherLockBtn) {
                                        otherLockBtn.classList.add('locked');
                                        const icon = otherLockBtn.querySelector('i');
                                        if (icon) icon.className = 'bi bi-lock-fill';
                                    }
                                }
                                
                                try {
                                    connectToolbars(toolbar.id, snappedTo);
                                    // Show unsnap buttons
                                    const unsnapBtn = header.querySelector('.epr-btn-unsnap');
                                    if (unsnapBtn) unsnapBtn.style.display = '';
                                    if (otherHeader) {
                                        const otherUnsnapBtn = otherHeader.querySelector('.epr-btn-unsnap');
                                        if (otherUnsnapBtn) otherUnsnapBtn.style.display = '';
                                    }
                                } catch (err) {
                                    console.error('[EPR Actions] Error connecting toolbars:', err);
                                }
                            }
                        }
                    
                    // Ensure toolbars stay within viewport (but allow free movement)
                    const maxX = window.innerWidth - toolbar.offsetWidth;
                    const maxY = window.innerHeight - toolbar.offsetHeight;
                    const minX = 0;
                    const minY = 0; // Allow movement anywhere, no minimum Y constraint
                    
                    newX = Math.max(minX, Math.min(newX, maxX));
                    newY = Math.max(minY, Math.min(newY, maxY));
                    
                    toolbar.style.left = `${newX}px`;
                    toolbar.style.top = `${newY}px`;
                    toolbar.style.right = 'auto'; // Clear right positioning
                    toolbar.style.bottom = 'auto'; // Clear bottom positioning
                };
                
                const handleMouseUp = () => {
                    if (isDragging) {
                        isDragging = false;
                        toolbar.style.cursor = '';
                        header.style.cursor = 'move';
                        header.style.userSelect = '';
                        connectedInitialPositions.clear();
                    }
                    document.removeEventListener('mousemove', handleMouseMove);
                    document.removeEventListener('mouseup', handleMouseUp);
                };
                
                header.addEventListener('mousedown', handleMouseDown);
                header.style.cursor = 'move';
                console.log('[EPR Actions] Drag handler attached to:', toolbar.id || toolbar.className);
            };
            
            // Initialize all toolbars as unlocked (open lock icon = independent) by default
            try {
                document.querySelectorAll('.epr-floating-toolbar, .epr-types-panel').forEach(toolbar => {
                    if (!toolbar.hasAttribute('data-toolbar-locked')) {
                        toolbar.setAttribute('data-toolbar-locked', 'false');
                    }
                    const lockBtn = toolbar.querySelector('.epr-btn-lock-toolbar');
                    if (lockBtn) {
                        const isLocked = toolbar.getAttribute('data-toolbar-locked') === 'true';
                        if (isLocked) {
                            // Locked = closed lock icon = moves together
                            lockBtn.classList.add('locked');
                            const icon = lockBtn.querySelector('i');
                            if (icon) icon.className = 'bi bi-lock-fill';
                        } else {
                            // Unlocked = open lock icon = independent
                            lockBtn.classList.remove('locked');
                            const icon = lockBtn.querySelector('i');
                            if (icon) icon.className = 'bi bi-unlock';
                        }
                    }
                });
            } catch (err) {
                console.error('[EPR Actions] Error initializing toolbar lock states:', err);
            }
            
            // Setup for unified toolbar
            try {
                document.querySelectorAll('.epr-floating-toolbar').forEach(toolbar => {
                    try {
                        setupDrag(toolbar);
                    } catch (err) {
                        console.error('[EPR Actions] Error setting up drag for toolbar:', toolbar?.id || toolbar?.className, err);
                    }
                });
            } catch (err) {
                console.error('[EPR Actions] Error querying floating toolbars:', err);
            }
            
            // Setup for split panels
            try {
                document.querySelectorAll('.epr-types-panel').forEach(panel => {
                    try {
                        setupDrag(panel);
                    } catch (err) {
                        console.error('[EPR Actions] Error setting up drag for panel:', panel?.id || panel?.className, err);
                    }
                });
            } catch (err) {
                console.error('[EPR Actions] Error querying types panels:', err);
            }
            
            // Also setup for parameters panel specifically
            try {
                const paramsPanel = document.getElementById('eprParametersPanel');
                if (paramsPanel) {
                    setupDrag(paramsPanel);
                }
            } catch (err) {
                console.error('[EPR Actions] Error setting up parameters panel:', err);
            }
            
            // Setup dragging immediately and also retry to catch any dynamically added toolbars
            const setupAllToolbars = () => {
                try {
                    document.querySelectorAll('.epr-floating-toolbar').forEach(toolbar => {
                        try {
                            if (!toolbar.dataset.dragSetup) {
                                setupDrag(toolbar);
                                toolbar.dataset.dragSetup = 'true';
                            }
                        } catch (err) {
                            console.error('[EPR Actions] Error setting up drag for toolbar:', toolbar?.id || toolbar?.className, err);
                        }
                    });
                    
                    document.querySelectorAll('.epr-types-panel').forEach(panel => {
                        try {
                            if (!panel.dataset.dragSetup) {
                                setupDrag(panel);
                                panel.dataset.dragSetup = 'true';
                            }
                        } catch (err) {
                            console.error('[EPR Actions] Error setting up drag for panel:', panel?.id || panel?.className, err);
                        }
                    });
                } catch (err) {
                    console.error('[EPR Actions] Error in setupAllToolbars:', err);
                }
            };
            
            // Setup immediately
            setupAllToolbars();
            
            // Also setup after delays to catch any dynamically added toolbars
            setTimeout(setupAllToolbars, 100);
            setTimeout(setupAllToolbars, 500);
            setTimeout(setupAllToolbars, 1000);
            
            console.log('[EPR Actions] Toolbar dragging setup complete');
        }
        
        toggleTypesToolbarSplit() {
            console.log('[EPR Actions] toggleTypesToolbarSplit called');
            const unifiedToolbar = document.getElementById('eprTypesToolbar');
            const splitBtn = document.getElementById('eprSplitTypesBtn');
            
            if (!unifiedToolbar) {
                console.error('[EPR Actions] Unified toolbar not found!');
                return;
            }
            
            const isSplit = unifiedToolbar.style.display === 'none' || unifiedToolbar.classList.contains('split-mode');
            
            console.log('[EPR Actions] Is split:', isSplit);
            
            if (isSplit) {
                // Lock together - show unified, hide split panels
                unifiedToolbar.style.display = '';
                unifiedToolbar.classList.remove('split-mode');
                document.querySelectorAll('.epr-types-panel').forEach(panel => {
                    panel.style.display = 'none';
                });
                if (splitBtn) {
                    splitBtn.innerHTML = '<i class="bi bi-lock"></i>';
                    splitBtn.classList.remove('active');
                    splitBtn.title = 'Split Toolbars';
                }
                // Ensure raw materials are rendered in unified toolbar
                if (window.EPRVisualEditor && window.EPRVisualEditor.typesToolbar) {
                    const rawMaterialsContainer = document.getElementById('eprRawMaterialsButtons');
                    if (rawMaterialsContainer && rawMaterialsContainer.children.length === 0) {
                        console.log('[EPR Actions] Re-rendering raw materials in unified toolbar');
                        window.EPRVisualEditor.typesToolbar.renderRawMaterials();
                    }
                }
                console.log('[EPR Actions] Toolbars locked together');
            } else {
                // Split apart - hide unified, show split panels
                unifiedToolbar.style.display = 'none';
                unifiedToolbar.classList.add('split-mode');
                document.querySelectorAll('.epr-types-panel').forEach(panel => {
                    panel.style.display = '';
                });
                if (splitBtn) {
                    splitBtn.innerHTML = '<i class="bi bi-unlock"></i>';
                    splitBtn.classList.add('active');
                    splitBtn.title = 'Lock Toolbars';
                }
                
                // Sync content to split panels
                if (window.EPRVisualEditor && window.EPRVisualEditor.typesToolbar) {
                    window.EPRVisualEditor.typesToolbar.syncToSplitPanels();
                    // Also ensure raw materials are rendered in the split panel
                    window.EPRVisualEditor.typesToolbar.renderRawMaterials();
                }
                console.log('[EPR Actions] ✅ Toolbars split apart');
            }
        }
        
        toggleDragMode() {
            this.isDragMode = !this.isDragMode;
            const btn = document.getElementById('eprDragModeBtn');
            if (btn) {
                btn.classList.toggle('active', this.isDragMode);
            }
            
            const canvas = document.getElementById('eprCanvas');
            if (canvas) {
                canvas.style.cursor = this.isDragMode ? 'move' : 'default';
            }
        }
        
        toggleToolbarsVisibility() {
            // Get all toolbars and panels
            const toolbars = document.querySelectorAll('.epr-floating-toolbar');
            const panels = document.querySelectorAll('.epr-types-panel');
            const allPanels = [...Array.from(toolbars), ...Array.from(panels)];
            
            // Check if any are visible (not hidden)
            const isHidden = allPanels.length > 0 && allPanels[0] && 
                           (allPanels[0].style.display === 'none' || 
                            allPanels[0].classList.contains('toolbar-hidden'));
            
            console.log('[EPR Actions] Toggling toolbars visibility. Currently hidden:', isHidden);
            
            // Toggle all toolbars and panels
            allPanels.forEach(panel => {
                if (isHidden) {
                    // Show
                    panel.style.display = '';
                    panel.classList.remove('toolbar-hidden');
                } else {
                    // Hide
                    panel.style.display = 'none';
                    panel.classList.add('toolbar-hidden');
                }
            });
            
            const btn = document.getElementById('eprToggleToolbarsBtn');
            if (btn) {
                btn.classList.toggle('active', !isHidden);
                btn.title = isHidden ? 'Show Toolbars' : 'Hide Toolbars';
                const icon = btn.querySelector('i');
                const textSpan = btn.querySelector('span');
                if (icon) {
                    icon.className = isHidden ? 'bi bi-eye-slash' : 'bi bi-eye';
                }
                // Update text span if it exists (for project header button)
                if (textSpan) {
                    textSpan.textContent = isHidden ? 'Show Toolbars' : 'Hide Toolbars';
                }
            }
        }
        
        toggleTransportVisibility() {
            if (!window.EPRVisualEditor || !window.EPRVisualEditor.canvasManager) return;
            
            const canvasManager = window.EPRVisualEditor.canvasManager;
            canvasManager.showTransportNodes = !canvasManager.showTransportNodes;
            
            const btn = document.getElementById('eprToggleTransportBtn');
            if (btn) {
                btn.classList.toggle('active', canvasManager.showTransportNodes);
                const icon = btn.querySelector('i');
                const textSpan = btn.querySelector('span');
                if (icon) {
                    icon.className = canvasManager.showTransportNodes ? 'bi bi-truck' : 'bi bi-truck';
                }
                if (textSpan) {
                    textSpan.textContent = canvasManager.showTransportNodes ? 'Hide Transport' : 'Show Transport';
                }
            }
            
            // DON'T override transport node visibility here - let updateConnections() handle it
            // The updateConnections() method has logic to hide transport nodes that are between connected nodes
            // and show them as small boxes on connection lines
            
            // Update connections - this will show/hide transport boxes on connections
            // and properly hide/show transport nodes based on whether they're between connected nodes
            canvasManager.updateConnections();
        }
        
        enableTransportMode() {
            // REMOVED: This function is no longer used
            // Transport nodes can ONLY be added via connection context menu
            // This function is kept for compatibility but does nothing
            return;
        }
        
        showTransportPopup(x, y, nodeX, nodeY) {
            // CRITICAL: Only allow transport popup when called from connection context menu
            // Check if pendingConnectionInfo exists BEFORE creating modal
            if (!window.EPRVisualEditor || !window.EPRVisualEditor.pendingConnectionInfo) {
                alert('Transport nodes can only be added between two connected nodes.\n\nTo add a transport node:\n1. Right-click on a connection line between two nodes\n2. Select "Add Transport" from the context menu');
                return; // Don't create modal if no connection context
            }
            
            // Create popup modal ONLY when called from connection context menu
            const modal = document.createElement('div');
            modal.className = 'modal fade show';
            modal.style.display = 'block';
            modal.style.zIndex = '9999';
            modal.style.backgroundColor = 'rgba(0,0,0,0.5)';
            modal.innerHTML = `
                <div class="modal-dialog modal-dialog-centered">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Add Transport Node</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body">
                            <div id="transportTypesContainer">
                                <div class="transport-type-entry mb-3">
                                    <div class="row">
                                        <div class="col-md-6">
                                            <label class="form-label">Transport Type</label>
                                            <select class="form-select transport-type-select">
                                                <option value="">Select type...</option>
                                                <option value="Ship">Ship</option>
                                                <option value="Rail">Rail</option>
                                                <option value="Truck">Truck</option>
                                                <option value="Van">Van</option>
                                                <option value="Air">Air</option>
                                            </select>
                                        </div>
                                        <div class="col-md-6">
                                            <label class="form-label">Distance (km)</label>
                                            <input type="number" class="form-control transport-distance-input" placeholder="Distance" min="0" step="0.1">
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <button type="button" class="btn btn-sm btn-secondary" id="addTransportTypeBtn">
                                <i class="bi bi-plus"></i> Add Another Type
                            </button>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                            <button type="button" class="btn btn-primary" id="saveTransportBtn">Add Transport Node</button>
                        </div>
                    </div>
                </div>
            `;
            document.body.appendChild(modal);
            
            // Get canvas position for node placement
            const canvas = document.getElementById('eprCanvas');
            const rect = canvas.getBoundingClientRect();
            let finalNodeX, finalNodeY;
            if (nodeX !== undefined && nodeY !== undefined) {
                // Use provided coordinates (from connection modal)
                finalNodeX = nodeX;
                finalNodeY = nodeY;
            } else {
                // Calculate from screen coordinates
                finalNodeX = (x - rect.left) / (this.canvasManager.zoomLevel || 1) - (this.canvasManager.panOffset?.x || 0);
                finalNodeY = (y - rect.top) / (this.canvasManager.zoomLevel || 1) - (this.canvasManager.panOffset?.y || 0);
            }
            
            // Add another transport type
            document.getElementById('addTransportTypeBtn').addEventListener('click', () => {
                const container = document.getElementById('transportTypesContainer');
                const newEntry = document.createElement('div');
                newEntry.className = 'transport-type-entry mb-3';
                newEntry.innerHTML = `
                    <div class="row">
                        <div class="col-md-6">
                            <label class="form-label">Transport Type</label>
                            <select class="form-select transport-type-select">
                                <option value="">Select type...</option>
                                <option value="Ship">Ship</option>
                                <option value="Rail">Rail</option>
                                <option value="Truck">Truck</option>
                                <option value="Van">Van</option>
                                <option value="Air">Air</option>
                            </select>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Distance (km)</label>
                            <input type="number" class="form-control transport-distance-input" placeholder="Distance" min="0" step="0.1">
                            <button type="button" class="btn btn-sm btn-danger mt-2 remove-transport-type">
                                <i class="bi bi-trash"></i> Remove
                            </button>
                        </div>
                    </div>
                `;
                container.appendChild(newEntry);
                
                newEntry.querySelector('.remove-transport-type').addEventListener('click', () => {
                    newEntry.remove();
                });
            });
            
            // Remove transport type handlers
            modal.querySelectorAll('.remove-transport-type').forEach(btn => {
                btn.addEventListener('click', (e) => {
                    e.target.closest('.transport-type-entry').remove();
                });
            });
            
            // Save transport node
            document.getElementById('saveTransportBtn').addEventListener('click', () => {
                const transportTypes = [];
                modal.querySelectorAll('.transport-type-entry').forEach(entry => {
                    const type = entry.querySelector('.transport-type-select').value;
                    const distance = entry.querySelector('.transport-distance-input').value;
                    if (type && distance) {
                        transportTypes.push({ type, distance: parseFloat(distance) });
                    }
                });
                
                if (transportTypes.length === 0) {
                    alert('Please add at least one transport type with distance');
                    return;
                }
                
                // Create transport node
                const nodeData = {
                    type: 'transport',
                    name: `Transport (${transportTypes.map(t => t.type).join(', ')})`,
                    icon: 'bi-truck',
                    x: Math.max(0, finalNodeX),
                    y: Math.max(0, finalNodeY),
                    transportTypes: transportTypes
                };
                
                // CRITICAL: Transport nodes should ONLY be created between two nodes
                // This check should have already happened in showTransportPopup, but double-check here
                if (!window.EPRVisualEditor || !window.EPRVisualEditor.pendingConnectionInfo) {
                    alert('Transport nodes can only be added between two connected nodes.\n\nTo add a transport node:\n1. Right-click on a connection line between two nodes\n2. Select "Add Transport" from the context menu\n3. Enter transport details in the modal');
                    modal.remove();
                    // Clear any stale connection info
                    if (window.EPRVisualEditor) {
                        window.EPRVisualEditor.pendingConnectionInfo = null;
                    }
                    return;
                }
                
                const connInfo = window.EPRVisualEditor.pendingConnectionInfo;
                
                // Create transport node
                const transportNode = this.canvasManager.addNode(nodeData);
                
                // ALWAYS ensure the original direct connection exists (this is critical!)
                // This is what allows the transport to be displayed as a box on the connection line
                const originalConnExists = this.canvasManager.connections.some(c => 
                    c.from === connInfo.fromNode && c.to === connInfo.toNode
                );
                
                if (!originalConnExists) {
                    // Create the original direct connection - this is REQUIRED for transport box display
                    this.canvasManager.addConnection(connInfo.fromNode, connInfo.toNode, connInfo.fromPort, connInfo.toPort);
                }
                
                // Add connections through transport: fromNode -> transport -> toNode
                // These connections allow us to detect that transport is "between" the nodes
                this.canvasManager.addConnection(connInfo.fromNode, transportNode.id, connInfo.fromPort, 'left');
                this.canvasManager.addConnection(transportNode.id, connInfo.toNode, 'right', connInfo.toPort);
                
                // CRITICAL: Force hide the transport node IMMEDIATELY with multiple methods
                // Use multiple timeouts to ensure DOM is ready and node is hidden
                setTimeout(() => {
                    const transportNodeEl = document.querySelector(`[data-node-id="${transportNode.id}"]`);
                    if (transportNodeEl) {
                        // Apply ALL hiding methods
                        transportNodeEl.style.display = 'none';
                        transportNodeEl.style.setProperty('display', 'none', 'important');
                        transportNodeEl.style.visibility = 'hidden';
                        transportNodeEl.style.setProperty('visibility', 'hidden', 'important');
                        transportNodeEl.style.opacity = '0';
                        transportNodeEl.style.setProperty('opacity', '0', 'important');
                        transportNodeEl.style.height = '0';
                        transportNodeEl.style.setProperty('height', '0', 'important');
                        transportNodeEl.style.width = '0';
                        transportNodeEl.style.setProperty('width', '0', 'important');
                        transportNodeEl.style.position = 'absolute';
                        transportNodeEl.style.setProperty('position', 'absolute', 'important');
                        transportNodeEl.style.left = '-9999px';
                        transportNodeEl.style.setProperty('left', '-9999px', 'important');
                        transportNodeEl.style.pointerEvents = 'none';
                        transportNodeEl.style.setProperty('pointer-events', 'none', 'important');
                        transportNodeEl.setAttribute('data-transport-on-connection', 'true');
                        transportNodeEl.classList.add('epr-transport-hidden');
                        
                        // Force re-check after a short delay
                        setTimeout(() => {
                            if (transportNodeEl) {
                                transportNodeEl.style.display = 'none';
                                transportNodeEl.style.setProperty('display', 'none', 'important');
                            }
                        }, 10);
                    }
                }, 0);
                
                // CRITICAL: Force update connections IMMEDIATELY
                // This will hide the large transport node and render the small box on the connection line
                this.canvasManager.updateConnections();
                
                // Double-check after updateConnections
                setTimeout(() => {
                    const transportNodeEl = document.querySelector(`[data-node-id="${transportNode.id}"]`);
                    if (transportNodeEl) {
                        transportNodeEl.style.display = 'none';
                        transportNodeEl.style.setProperty('display', 'none', 'important');
                        transportNodeEl.style.visibility = 'hidden';
                        transportNodeEl.style.setProperty('visibility', 'hidden', 'important');
                    }
                    this.canvasManager.updateConnections();
                }, 50);
                
                window.EPRVisualEditor.pendingConnectionInfo = null;
                
                // Close modal
                modal.remove();
            });
            
            // Cancel handler
            modal.querySelectorAll('[data-bs-dismiss="modal"], .btn-close').forEach(btn => {
                btn.addEventListener('click', () => {
                    modal.remove();
                    // Clear pending connection info
                    if (window.EPRVisualEditor) {
                        window.EPRVisualEditor.pendingConnectionInfo = null;
                    }
                });
            });
        }
    }
    
    // ============================================================================
    // MAIN APPLICATION
    // ============================================================================
    class EPRVisualEditorApp {
        constructor() {
            console.log('[EPR Visual Editor] Creating EPRVisualEditorApp...');
            this.timestampUpdateInterval = null;
            this.lastAutosaveTime = null;
            
            // CRITICAL: Remove any blocking modal backdrops immediately
            document.querySelectorAll('.modal-backdrop:not(.show)').forEach(backdrop => {
                backdrop.remove();
            });
            document.querySelectorAll('.modal-backdrop').forEach(backdrop => {
                if (!backdrop.classList.contains('show')) {
                    backdrop.remove();
                }
            });
            
            // CRITICAL: Hide all modals that aren't shown
            document.querySelectorAll('.modal:not(.show)').forEach(modal => {
                modal.style.display = 'none';
                modal.style.pointerEvents = 'none';
                modal.style.visibility = 'hidden';
                modal.style.opacity = '0';
                modal.setAttribute('aria-hidden', 'true');
            });
            
            try {
                // Initialize canvas manager first
                this.canvasManager = new EPRCanvasManager();
                console.log('[EPR Visual Editor] CanvasManager created');
                
                // Initialize toolbars
                this.typesToolbar = new EPRTypesToolbar(this.canvasManager);
                console.log('[EPR Visual Editor] TypesToolbar created');
                
                this.parametersPanel = new EPRParametersPanel(this.canvasManager);
                console.log('[EPR Visual Editor] ParametersPanel created');
                
                // Setup parameter input handlers
                const paramsContent = document.getElementById('eprParametersContent');
                if (paramsContent) {
                    paramsContent.addEventListener('input', (e) => {
                        if (e.target.classList.contains('epr-parameter-input')) {
                            this.parametersPanel.saveParameter(e.target.dataset.paramKey, e.target.value);
                        }
                    });
                }
                
                this.actionsToolbar = new EPRActionsToolbar(this.canvasManager);
                console.log('[EPR Visual Editor] ActionsToolbar created');
                
                // Setup toolbar dragging immediately - make all toolbars draggable by default
                setTimeout(() => {
                    this.actionsToolbar.setupToolbarDragging();
                    console.log('[EPR Visual Editor] Toolbar dragging setup complete');
                    
                    // Lock 6 palettes horizontally in position on startup
                    this.lockPalettesHorizontally();
                }, 100);
                
                // Setup Sankey/Flow toggle button (in header)
                this.setupSankeyFlowToggle();
                
                // Setup project handlers
                this.setupProjectHandlers();
                
                // Setup connection creation
                this.setupConnectionCreation();
                
                // Expose setupConnectionPort for node rendering
                this.setupConnectionPort = this.setupConnectionPort.bind(this);
                
                console.log('[EPR Visual Editor] ✅✅✅ FULLY INITIALIZED! ✅✅✅');
                
                // Simple cleanup: Remove hidden modal backdrops
                setTimeout(() => {
                    document.querySelectorAll('.modal-backdrop:not(.show)').forEach(backdrop => {
                        backdrop.remove();
                    });
                }, 100);
                
                // Force a re-render after a short delay to ensure everything is ready
                setTimeout(async () => {
                    console.log('[EPR Visual Editor] Post-init check...');
                    if (this.typesToolbar) {
                        const rawMaterialsContainer = document.getElementById('eprRawMaterialsButtons');
                        if (!rawMaterialsContainer || rawMaterialsContainer.children.length === 0) {
                            console.log('[EPR Visual Editor] Re-rendering raw materials...');
                            this.typesToolbar.renderRawMaterials();
                        }
                        
                        // Ensure dropdowns are populated
                        const packagingDropdown = document.getElementById('eprPackagingLibraryDropdown');
                        if (packagingDropdown && packagingDropdown.options.length <= 1) {
                            console.log('[EPR Visual Editor] Re-rendering packaging dropdown...');
                            await this.typesToolbar.renderPackagingDropdown();
                        }
                        
                        const productDropdown = document.getElementById('eprProductLibraryDropdown');
                        if (productDropdown && productDropdown.options.length <= 1) {
                            console.log('[EPR Visual Editor] Re-rendering product dropdown...');
                            this.typesToolbar.renderProductDropdown();
                        }
                    }
                }, 500);
            } catch (error) {
                console.error('[EPR Visual Editor] ❌ ERROR during initialization:', error);
                console.error(error.stack);
                alert('Error initializing Visual Editor: ' + error.message);
            }
        }
        
        setupSankeyFlowToggle() {
            // Sankey/Flow toggle button removed
        }
        
        setupProjectHandlers() {
            const saveBtn = document.getElementById('eprSaveProjectBtn');
            if (saveBtn) {
                saveBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    this.saveProject();
                });
            }
            
            const loadBtn = document.getElementById('eprLoadProjectBtn');
            if (loadBtn) {
                loadBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    this.showLoadProjectModal();
                });
            }
            
            const newProjectBtn = document.getElementById('eprNewProjectBtn');
            if (newProjectBtn) {
                newProjectBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    this.newProject();
                });
            }
            
            // Set default project name to "Untitled" if empty
            const projectNameInput = document.getElementById('eprProjectName');
            if (projectNameInput && !projectNameInput.value.trim()) {
                projectNameInput.value = 'Untitled';
            }
            
            // Update browser tab title on project name change
            if (projectNameInput) {
                this.updateBrowserTabTitle(); // Set initial title
                projectNameInput.addEventListener('input', () => {
                    this.updateBrowserTabTitle();
                });
                projectNameInput.addEventListener('blur', () => {
                    this.updateBrowserTabTitle();
                });
            }
            
            // Setup Align/Distribute handlers
            this.setupAlignDistributeHandlers();
            
            const saveElementBtn = document.getElementById('eprSaveElementBtn');
            if (saveElementBtn) {
                saveElementBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    this.saveElement();
                });
            }
            
            const loadElementBtn = document.getElementById('eprLoadElementBtn');
            if (loadElementBtn) {
                loadElementBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    this.showLoadElementModal();
                });
            }
            
            const loadRelBtn = document.getElementById('eprLoadRelationshipsBtn');
            if (loadRelBtn) {
                loadRelBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    const modal = document.getElementById('eprLoadRelationshipsModal');
                    if (modal) {
                        const bsModal = new bootstrap.Modal(modal);
                        bsModal.show();
                    }
                });
            }
            const loadRelConfirmBtn = document.getElementById('eprLoadRelationshipsConfirmBtn');
            if (loadRelConfirmBtn) {
                loadRelConfirmBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    const datasetKey = document.getElementById('eprRelationshipDatasetSelect')?.value || '';
                    const modal = bootstrap.Modal.getInstance(document.getElementById('eprLoadRelationshipsModal'));
                    if (modal) modal.hide();
                    this.loadRelationshipGraph(datasetKey);
                });
            }
            
            const saveLayoutBtn = document.getElementById('eprSaveLayoutBtn');
            if (saveLayoutBtn) {
                saveLayoutBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    this.saveLayout();
                });
            }
            
            const resetLayoutBtn = document.getElementById('eprResetLayoutBtn');
            if (resetLayoutBtn) {
                resetLayoutBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    this.resetLayout();
                });
            }
            
            // Setup autosave checkbox
            const autosaveCheckbox = document.getElementById('eprAutosaveCheckbox');
            if (autosaveCheckbox) {
                // Prevent label click from opening tab
                const autosaveLabel = autosaveCheckbox.closest('label');
                if (autosaveLabel) {
                    autosaveLabel.addEventListener('click', (e) => {
                        e.stopPropagation();
                    });
                }
                
                // Load autosave state from localStorage (instance-specific)
                const instanceId = window.eprCurrentInstanceId || 'default';
                const autosaveEnabled = localStorage.getItem(`eprAutosaveEnabled-${instanceId}`) === 'true';
                autosaveCheckbox.checked = autosaveEnabled;
                this.autosaveEnabled = autosaveEnabled;
                
                // Update timestamp display on load
                if (autosaveEnabled) {
                    this.updateAutosaveTimestamp();
                }
                
                autosaveCheckbox.addEventListener('change', (e) => {
                    e.stopPropagation();
                    this.autosaveEnabled = e.target.checked;
                    localStorage.setItem(`eprAutosaveEnabled-${instanceId}`, e.target.checked.toString());
                    if (e.target.checked) {
                        console.log('[EPR Visual Editor] Autosave enabled for instance:', instanceId);
                        // Update timestamp display when enabled
                        this.updateAutosaveTimestamp();
                    } else {
                        console.log('[EPR Visual Editor] Autosave disabled for instance:', instanceId);
                        // Hide timestamp when disabled
                        this.updateAutosaveTimestamp();
                    }
                });
            }
            
            // Setup Align/Distribute dropdown button
            const alignDistributeBtn = document.querySelector('[data-bs-toggle="dropdown"][title="Align/Distribute Selected Nodes"]');
            if (alignDistributeBtn) {
                alignDistributeBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                });
            }
        }
        
        // Autosave method - called when autosave is enabled
        async autosave() {
            if (!this.autosaveEnabled) return;
            
            const projectName = document.getElementById('eprProjectName')?.value.trim();
            if (!projectName) return;
            
            try {
                const allNodes = Array.from(this.canvasManager.nodes.values());
                const allConnections = this.canvasManager.connections;
                
                const nodesToSave = allNodes.map(node => {
                    const cloned = JSON.parse(JSON.stringify(node));
                    if (node.groupId) cloned.groupId = node.groupId;
                    if (node.containedItems && Array.isArray(node.containedItems)) {
                        cloned.containedItems = [...node.containedItems];
                    }
                    if (node.parentItemId) cloned.parentItemId = node.parentItemId;
                    return cloned;
                });
                
                const projectData = {
                    projectName: projectName,
                    nodes: nodesToSave,
                    connections: JSON.parse(JSON.stringify(allConnections)),
                    savedAt: new Date().toISOString()
                };
                
                // Use instance-specific storage key
                const instanceId = window.eprCurrentInstanceId || 'default';
                const storageKey = `eprSavedProjects-${instanceId}`;
                const savedProjects = JSON.parse(localStorage.getItem(storageKey) || '[]');
                const projectIndex = savedProjects.findIndex(p => p.projectName === projectName);
                
                if (projectIndex >= 0) {
                    savedProjects[projectIndex] = projectData;
                } else {
                    savedProjects.push(projectData);
                }
                
                localStorage.setItem(storageKey, JSON.stringify(savedProjects));
                console.log('[EPR Visual Editor] Autosaved project:', projectName, 'to storage:', storageKey);
                
                // Store autosave time
                this.lastAutosaveTime = new Date();
                
                // Update autosave timestamp display (will schedule next update)
                this.updateAutosaveTimestamp();
            } catch (error) {
                console.error('[EPR Visual Editor] Autosave error:', error);
            }
        }
        
        updateAutosaveTimestamp() {
            const timestampEl = document.getElementById('eprAutosaveTimestamp');
            if (!timestampEl) return;
            
            if (!this.autosaveEnabled) {
                timestampEl.style.display = 'none';
                return;
            }
            
            const now = new Date();
            // Store last autosave time
            if (!this.lastAutosaveTime) {
                this.lastAutosaveTime = now;
            }
            
            // Calculate time ago
            const timeDiff = Math.floor((now - this.lastAutosaveTime) / 1000); // seconds
            let timeAgo = '';
            
            if (timeDiff < 60) {
                timeAgo = `${timeDiff} second${timeDiff !== 1 ? 's' : ''} ago`;
            } else if (timeDiff < 3600) {
                const minutes = Math.floor(timeDiff / 60);
                timeAgo = `${minutes} minute${minutes !== 1 ? 's' : ''} ago`;
            } else {
                const hours = Math.floor(timeDiff / 3600);
                timeAgo = `${hours} hour${hours !== 1 ? 's' : ''} ago`;
            }
            
            timestampEl.textContent = `(Last saved ${timeAgo})`;
            timestampEl.style.display = 'inline';
            
            // Schedule next update in 1 minute
            if (this.timestampUpdateInterval) {
                clearInterval(this.timestampUpdateInterval);
            }
            this.timestampUpdateInterval = setTimeout(() => {
                this.updateAutosaveTimestamp();
            }, 60000); // Update every minute
        }
        
        updateBrowserTabTitle() {
            const projectName = document.getElementById('eprProjectName')?.value.trim() || 'Untitled';
            document.title = `${projectName} - Visual Editor`;
            
            // Update browser tab title if browser tabs system is available
            if (window.browserTabs && window.browserTabs.updateTabTitle) {
                const currentUrl = window.location.pathname + window.location.search;
                // Try with full URL first, then just pathname
                window.browserTabs.updateTabTitle(currentUrl, projectName);
                // Also try with just pathname in case URL has query params
                if (currentUrl !== window.location.pathname) {
                    window.browserTabs.updateTabTitle(window.location.pathname, projectName);
                }
            }
        }
        
        async saveProject() {
            const projectName = document.getElementById('eprProjectName')?.value.trim();
            
            if (!projectName) {
                alert('Please enter a project name');
                return;
            }
            
            // Collect ALL nodes and connections from the canvas
            const allNodes = Array.from(this.canvasManager.nodes.values());
            const allConnections = this.canvasManager.connections;
            
            // Deep clone all data, ensuring groupId and containedItems are preserved
            const nodesToSave = allNodes.map(node => {
                const cloned = JSON.parse(JSON.stringify(node));
                // Ensure groupId and containedItems are preserved
                if (node.groupId) {
                    cloned.groupId = node.groupId;
                }
                if (node.containedItems && Array.isArray(node.containedItems)) {
                    cloned.containedItems = [...node.containedItems];
                }
                // Preserve parent-item relationship
                if (node.parentItemId) {
                    cloned.parentItemId = node.parentItemId;
                }
                return cloned;
            });
            
            const projectData = {
                projectName: projectName,
                nodes: nodesToSave,
                connections: JSON.parse(JSON.stringify(allConnections)),
                savedAt: new Date().toISOString()
            };
            
            // Save to localStorage (instance-specific)
            const instanceId = window.eprCurrentInstanceId || 'default';
            const storageKey = `eprSavedProjects-${instanceId}`;
            const savedProjects = JSON.parse(localStorage.getItem(storageKey) || '[]');
            const projectIndex = savedProjects.findIndex(p => p.projectName === projectName);
            
            if (projectIndex >= 0) {
                savedProjects[projectIndex] = projectData;
            } else {
                savedProjects.push(projectData);
            }
            
            localStorage.setItem(storageKey, JSON.stringify(savedProjects));
            alert(`Project "${projectName}" saved successfully!`);
        }
        
        showLoadProjectModal() {
            // Use instance-specific storage key
            const instanceId = window.eprCurrentInstanceId || 'default';
            const storageKey = `eprSavedProjects-${instanceId}`;
            const savedProjects = JSON.parse(localStorage.getItem(storageKey) || '[]');
            
            if (savedProjects.length === 0) {
                alert('No saved projects found');
                return;
            }
            
            const modal = document.getElementById('eprLoadProjectModal');
            if (!modal) {
                // Create modal if it doesn't exist
                const modalHtml = `
                    <div class="modal fade" id="eprLoadProjectModal" tabindex="-1">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h5 class="modal-title">Load Project</h5>
                                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                                </div>
                                <div class="modal-body">
                                    <div id="eprLoadProjectList"></div>
                                </div>
                            </div>
                        </div>
                    </div>
                `;
                document.body.insertAdjacentHTML('beforeend', modalHtml);
            }
            
            const listContainer = document.getElementById('eprLoadProjectList');
            if (!listContainer) return;
            
            listContainer.innerHTML = savedProjects.map((project, index) => {
                const savedDate = project.savedAt ? new Date(project.savedAt).toLocaleString() : 'Unknown date';
                const projectName = project.projectName || 'Unnamed Project';
                return `
                    <div class="card mb-2">
                        <div class="card-body">
                            <h6 class="card-title">${projectName.replace(/</g, '&lt;').replace(/>/g, '&gt;')}</h6>
                            <p class="card-text small text-muted">Saved: ${savedDate}</p>
                            <p class="card-text small">Nodes: ${project.nodes?.length || 0}, Connections: ${project.connections?.length || 0}</p>
                            <button class="btn btn-sm btn-primary load-project-btn" data-index="${index}">Load</button>
                        </div>
                    </div>
                `;
            }).join('');
            
            // Attach event listeners
            listContainer.querySelectorAll('.load-project-btn').forEach(btn => {
                btn.addEventListener('click', (e) => {
                    const index = parseInt(e.target.dataset.index);
                    this.loadProject(savedProjects[index]);
                });
            });
            
            const bsModal = new bootstrap.Modal(document.getElementById('eprLoadProjectModal'));
            bsModal.show();
        }
        
        _renderSupplyChainGraph(result, titlePrefix) {
            this.canvasManager.nodes.clear();
            this.canvasManager.connections = [];
            this.canvasManager.selectedNode = null;
            this.canvasManager.nodesLayer.innerHTML = '';
            this.canvasManager.connectionsLayer.innerHTML = '';

            const columnConfig = {
                'supplier':           { x: 50,   label: 'Suppliers' },
                'supplier-packaging': { x: 330,  label: 'Supplier Products' },
                'raw-material':       { x: 330,  label: 'Raw Materials' },
                'packaging':          { x: 660,  label: 'Packaging Items' },
                'packaging-group':    { x: 990,  label: 'Packaging Groups' },
                'product':            { x: 1320, label: 'Products' },
                'distribution':       { x: 1650, label: 'Distribution' }
            };
            const columnY = {};
            for (const t of Object.keys(columnConfig)) columnY[t] = 0;
            const ySpacing = 140;
            const headerY = 20;
            const nodeStartY = 70;

            const nodeIdMap = new Map();

            for (const n of result.nodes) {
                const cfg = columnConfig[n.type];
                const col = cfg ? cfg.x : 600;
                const yOff = columnY[n.type] ?? 0;

                const nodeData = {
                    id: n.id,
                    type: n.type,
                    entityId: n.entityId,
                    name: n.label || n.sku || 'Unknown',
                    x: col,
                    y: nodeStartY + yOff,
                    parameters: {}
                };

                if (n.sku) nodeData.parameters.sku = n.sku;
                if (n.imageUrl) nodeData.parameters.imageUrl = n.imageUrl;
                if (n.code) nodeData.parameters.code = n.code;
                if (n.city) nodeData.parameters.city = n.city;
                if (n.country) nodeData.parameters.country = n.country;
                if (n.supplierName) nodeData.parameters.supplierName = n.supplierName;
                if (n.taxonomyCode) nodeData.parameters.taxonomyCode = n.taxonomyCode;
                if (n.packId) nodeData.parameters.packId = n.packId;
                if (n.layer) nodeData.parameters.layer = n.layer;
                if (n.productCode) nodeData.parameters.productCode = n.productCode;

                this.canvasManager.nodes.set(n.id, nodeData);
                nodeIdMap.set(n.id, true);
                columnY[n.type] = (columnY[n.type] || 0) + ySpacing;
            }

            // Render column header labels
            const existingHeaders = this.canvasManager.nodesLayer.querySelectorAll('.epr-column-header');
            existingHeaders.forEach(h => h.remove());

            const renderedLabels = new Set();
            for (const [type, cfg] of Object.entries(columnConfig)) {
                if (columnY[type] === 0) continue;
                const key = `${cfg.x}-${cfg.label}`;
                if (renderedLabels.has(key)) continue;
                renderedLabels.add(key);
                const header = document.createElement('div');
                header.className = 'epr-column-header';
                header.style.cssText = `position:absolute;left:${cfg.x}px;top:${headerY}px;font-size:11px;font-weight:700;text-transform:uppercase;letter-spacing:0.5px;color:var(--bs-secondary,#6c757d);pointer-events:none;white-space:nowrap;z-index:1;`;
                header.textContent = cfg.label;
                this.canvasManager.nodesLayer.appendChild(header);
            }

            for (const [, nodeData] of this.canvasManager.nodes) {
                this.canvasManager.renderNode(nodeData);
            }
            this.canvasManager.updatePlaceholder();

            for (const e of result.edges) {
                if (nodeIdMap.has(e.from) && nodeIdMap.has(e.to)) {
                    this.canvasManager.addConnection(e.from, e.to);
                }
            }

            this.canvasManager.updateConnections();

            const projectNameInput = document.getElementById('eprProjectName');
            if (projectNameInput) {
                projectNameInput.value = titlePrefix;
                this.updateBrowserTabTitle();
            }

            console.log(`[EPR] Loaded supply chain graph: ${result.nodes.length} nodes, ${result.edges.length} edges`);
        }

        async loadRelationshipGraph(datasetKey) {
            if (this.canvasManager.nodes.size > 0 || this.canvasManager.connections.length > 0) {
                if (!confirm('Loading relationships will replace all current work on the canvas. Continue?')) return;
            }

            const url = datasetKey
                ? `/api/visual-editor/relationship-graph?datasetKey=${encodeURIComponent(datasetKey)}`
                : '/api/visual-editor/relationship-graph';

            try {
                const resp = await fetch(url);
                const result = await resp.json();
                if (!result.success || !result.nodes) {
                    alert('No relationship data found' + (result.message ? ': ' + result.message : ''));
                    return;
                }

                const title = datasetKey ? `${datasetKey} Supply Chain` : 'All Supply Chains';
                this._renderSupplyChainGraph(result, title);
            } catch (err) {
                console.error('[EPR] Failed to load relationship graph:', err);
                alert('Failed to load relationships: ' + err.message);
            }
        }

        async loadProject(projectData) {
            // Warn user about losing current work
            if (this.canvasManager.nodes.size > 0 || this.canvasManager.connections.length > 0) {
                const confirmed = confirm('Loading this project will replace all current work. Are you sure you want to continue?');
                if (!confirmed) {
                    return;
                }
            }
            
            // Clear current canvas
            this.canvasManager.nodes.clear();
            this.canvasManager.connections = [];
            this.canvasManager.selectedNode = null;
            this.canvasManager.nodesLayer.innerHTML = '';
            this.canvasManager.connectionsLayer.innerHTML = '';
            
            // Load project data
            if (projectData && projectData.nodes && projectData.connections) {
                // Deep clone to avoid reference issues
                const nodes = JSON.parse(JSON.stringify(projectData.nodes));
                const connections = JSON.parse(JSON.stringify(projectData.connections));
                
                // First, add all nodes to the map
                nodes.forEach(node => {
                    this.canvasManager.nodes.set(node.id, node);
                });
                
                // Then restore group relationships
                nodes.forEach(node => {
                    // Restore groupId for items in groups
                    if (node.groupId) {
                        const groupNode = this.canvasManager.nodes.get(node.groupId);
                        if (groupNode) {
                            if (groupNode.type === 'packaging-group' && (node.type === 'packaging' || node.type === 'raw-material')) {
                                // Ensure containedItems array exists
                                if (!groupNode.containedItems) {
                                    groupNode.containedItems = [];
                                }
                                // Add this node to the group's containedItems if not already present
                                if (!groupNode.containedItems.includes(node.id)) {
                                    groupNode.containedItems.push(node.id);
                                }
                            } else if (groupNode.type === 'distribution-group' && node.type === 'distribution') {
                                // Ensure containedItems array exists
                                if (!groupNode.containedItems) {
                                    groupNode.containedItems = [];
                                }
                                // Add this node to the group's containedItems if not already present
                                if (!groupNode.containedItems.includes(node.id)) {
                                    groupNode.containedItems.push(node.id);
                                }
                            }
                        }
                    }
                    
                    // Restore parent-item relationship
                    if (node.parentItemId) {
                        const parentNode = this.canvasManager.nodes.get(node.parentItemId);
                        if (parentNode) {
                            // This is handled by the data-parent-item attribute in renderNode
                        }
                    }
                });
                
                // Add all connections first (needed for renderNode to show nesting correctly)
                this.canvasManager.connections = connections;
                
                // Now render all nodes (this will apply group styling and nesting)
                // Render groups first, then their contained items
                const groupNodes = nodes.filter(n => n.type === 'packaging-group' || n.type === 'distribution-group');
                const itemNodes = nodes.filter(n => n.type !== 'packaging-group' && n.type !== 'distribution-group');
                
                groupNodes.forEach(node => {
                    this.canvasManager.renderNode(node);
                });
                itemNodes.forEach(node => {
                    this.canvasManager.renderNode(node);
                });
                
                // Update connections after rendering
                this.canvasManager.updateConnections();
                
                // Update project name
                const projectNameInput = document.getElementById('eprProjectName');
                if (projectNameInput && projectData.projectName) {
                    projectNameInput.value = projectData.projectName;
                }
                
                // Close modal
                const modal = bootstrap.Modal.getInstance(document.getElementById('eprLoadProjectModal'));
                if (modal) {
                    modal.hide();
                }
                
                alert('Project loaded successfully!');
            } else {
                alert('Invalid project data');
            }
        }
        
        newProject() {
            // Warn user about losing current work
            if (this.canvasManager.nodes.size > 0 || this.canvasManager.connections.length > 0) {
                const confirmed = confirm('Creating a new project will clear all current work. Are you sure you want to continue?');
                if (!confirmed) {
                    return;
                }
            }
            
            // Clear canvas
            this.canvasManager.nodes.clear();
            this.canvasManager.connections = [];
            this.canvasManager.selectedNode = null;
            this.canvasManager.nodesLayer.innerHTML = '';
            this.canvasManager.connectionsLayer.innerHTML = '';
            
            // Clear project name
            const projectNameInput = document.getElementById('eprProjectName');
            if (projectNameInput) {
                projectNameInput.value = '';
            }
            
            // Clear parameters panel
            const paramsContainer = document.getElementById('eprParametersContent');
            if (paramsContainer) {
                paramsContainer.innerHTML = '<p class="text-muted small">Select a node to view/edit parameters</p>';
            }
            
                alert('New project created. Canvas cleared.');
        }
        
        /**
         * Show modal to import M&S Network from Excel
         */
        showImportMSNetworkModal() {
            // Create modal if it doesn't exist
            let modal = document.getElementById('eprImportMSNetworkModal');
            if (!modal) {
                const modalHtml = `
                    <div class="modal fade" id="eprImportMSNetworkModal" tabindex="-1">
                        <div class="modal-dialog modal-dialog-centered">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h5 class="modal-title">Import M&S UK Network</h5>
                                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                                </div>
                                <div class="modal-body">
                                    <p>Upload the MSnetwork.xlsx file to create the Distribution Group.</p>
                                    <div class="mb-3">
                                        <label for="eprMSNetworkFile" class="form-label">Excel File</label>
                                        <input type="file" class="form-control" id="eprMSNetworkFile" accept=".xlsx,.xls">
                                    </div>
                                    <div id="eprImportMSNetworkStatus" class="alert" style="display: none;"></div>
                                </div>
                                <div class="modal-footer">
                                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                                    <button type="button" class="btn btn-primary" id="eprImportMSNetworkSubmitBtn">Import</button>
                                </div>
                            </div>
                        </div>
                    </div>
                `;
                document.body.insertAdjacentHTML('beforeend', modalHtml);
                modal = document.getElementById('eprImportMSNetworkModal');
            }
            
            // Reset form
            document.getElementById('eprMSNetworkFile').value = '';
            document.getElementById('eprImportMSNetworkStatus').style.display = 'none';
            
            // Attach submit handler
            const submitBtn = document.getElementById('eprImportMSNetworkSubmitBtn');
            submitBtn.onclick = async () => {
                const fileInput = document.getElementById('eprMSNetworkFile');
                const file = fileInput.files[0];
                
                if (!file) {
                    alert('Please select an Excel file');
                    return;
                }
                
                const formData = new FormData();
                formData.append('file', file);
                
                submitBtn.disabled = true;
                submitBtn.textContent = 'Importing...';
                const statusDiv = document.getElementById('eprImportMSNetworkStatus');
                statusDiv.style.display = 'block';
                statusDiv.className = 'alert alert-info';
                statusDiv.textContent = 'Uploading and processing file...';
                
                try {
                    const response = await fetch('/api/visual-editor/import-ms-network', {
                        method: 'POST',
                        body: formData
                    });
                    
                    const contentType = response.headers.get('content-type');
                    let result;
                    
                    if (contentType && contentType.includes('application/json')) {
                        result = await response.json();
                        
                        if (!result.success) {
                            statusDiv.className = 'alert alert-danger';
                            statusDiv.textContent = `Import failed: ${result.message || 'Unknown error'}`;
                            submitBtn.disabled = false;
                            submitBtn.textContent = 'Import';
                            return;
                        }
                    } else {
                        // Response is the JSON project data directly
                        const jsonText = await response.text();
                        result = JSON.parse(jsonText);
                    }
                    
                    // Handle the result (either from result.data or result itself)
                    const projectData = result.data || result;
                    
                    if (projectData && projectData.nodes && projectData.connections) {
                        statusDiv.className = 'alert alert-success';
                        statusDiv.textContent = `Import successful! ${projectData.nodes.length || 0} nodes and ${projectData.connections.length || 0} connections created.`;
                        
                        // Save to localStorage (instance-specific)
                        const instanceId = window.eprCurrentInstanceId || 'default';
                        const storageKey = `eprSavedProjects-${instanceId}`;
                        const savedProjects = JSON.parse(localStorage.getItem(storageKey) || '[]');
                        savedProjects.push(projectData);
                        localStorage.setItem(storageKey, JSON.stringify(savedProjects));
                        
                        // Auto-load the project after 2 seconds
                        setTimeout(() => {
                            const bsModal = bootstrap.Modal.getInstance(modal);
                            if (bsModal) bsModal.hide();
                            this.loadProject(projectData);
                        }, 2000);
                    } else {
                        statusDiv.className = 'alert alert-danger';
                        statusDiv.textContent = `Import failed: Invalid data structure`;
                        submitBtn.disabled = false;
                        submitBtn.textContent = 'Import';
                    }
                } catch (error) {
                    statusDiv.className = 'alert alert-danger';
                    statusDiv.textContent = `Error: ${error.message}`;
                    submitBtn.disabled = false;
                    submitBtn.textContent = 'Import';
                }
            };
            
            const bsModal = new bootstrap.Modal(modal);
            bsModal.show();
        }
        
        /**
         * Save current toolbar/palette layout
         */
        saveLayout() {
            const layout = {
                toolbars: [],
                panels: [],
                savedAt: new Date().toISOString()
            };
            
            // Save unified toolbar position
            const unifiedToolbar = document.getElementById('eprTypesToolbar');
            if (unifiedToolbar) {
                const rect = unifiedToolbar.getBoundingClientRect();
                layout.toolbars.push({
                    id: 'eprTypesToolbar',
                    left: rect.left,
                    top: rect.top,
                    display: unifiedToolbar.style.display || '',
                    isSplit: unifiedToolbar.classList.contains('split-mode')
                });
            }
            
            // Save split panels positions
            document.querySelectorAll('.epr-types-panel').forEach(panel => {
                const rect = panel.getBoundingClientRect();
                layout.panels.push({
                    id: panel.id,
                    left: rect.left,
                    top: rect.top,
                    display: panel.style.display || ''
                });
            });
            
            // Save actions toolbar
            const actionsToolbar = document.getElementById('eprActionsToolbar');
            if (actionsToolbar) {
                const rect = actionsToolbar.getBoundingClientRect();
                layout.toolbars.push({
                    id: 'eprActionsToolbar',
                    left: rect.left,
                    top: rect.top
                });
            }
            
            // Save parameters panel
            const paramsPanel = document.getElementById('eprParametersPanel');
            if (paramsPanel) {
                const rect = paramsPanel.getBoundingClientRect();
                layout.toolbars.push({
                    id: 'eprParametersPanel',
                    left: rect.left,
                    top: rect.top
                });
            }
            
            localStorage.setItem('eprToolbarLayout', JSON.stringify(layout));
            alert('Layout saved successfully!');
        }
        
        /**
         * Reset toolbar/palette layout to saved position
         */
        resetLayout() {
            const savedLayout = localStorage.getItem('eprToolbarLayout');
            if (!savedLayout) {
                alert('No saved layout found');
                return;
            }
            
            try {
                const layout = JSON.parse(savedLayout);
                
                // Restore unified toolbar
                if (layout.toolbars) {
                    layout.toolbars.forEach(toolbar => {
                        const el = document.getElementById(toolbar.id);
                        if (el) {
                            el.style.left = `${toolbar.left}px`;
                            el.style.top = `${toolbar.top}px`;
                            el.style.right = 'auto';
                            el.style.bottom = 'auto';
                            if (toolbar.display !== undefined) {
                                el.style.display = toolbar.display;
                            }
                            if (toolbar.isSplit !== undefined) {
                                if (toolbar.isSplit) {
                                    el.classList.add('split-mode');
                                } else {
                                    el.classList.remove('split-mode');
                                }
                            }
                        }
                    });
                }
                
                // Restore split panels
                if (layout.panels) {
                    layout.panels.forEach(panel => {
                        const el = document.getElementById(panel.id);
                        if (el) {
                            el.style.left = `${panel.left}px`;
                            el.style.top = `${panel.top}px`;
                            el.style.right = 'auto';
                            el.style.bottom = 'auto';
                            if (panel.display !== undefined) {
                                el.style.display = panel.display;
                            }
                        }
                    });
                }
                
                // Sync split panels if needed
                if (window.EPRVisualEditor && window.EPRVisualEditor.typesToolbar) {
                    const unifiedToolbar = document.getElementById('eprTypesToolbar');
                    if (unifiedToolbar && unifiedToolbar.classList.contains('split-mode')) {
                        window.EPRVisualEditor.typesToolbar.syncToSplitPanels();
                    }
                }
                
                alert('Layout reset successfully!');
            } catch (error) {
                console.error('Error resetting layout:', error);
                alert('Error resetting layout: ' + error.message);
            }
        }
        
        /**
         * Save selected element(s) and all related components
         */
        saveElement() {
            const selectedNodeId = this.canvasManager.selectedNode;
            
            if (!selectedNodeId) {
                alert('Please select an element to save');
                return;
            }
            
            // Collect all related nodes
            const elementData = this.canvasManager.collectRelatedNodes(selectedNodeId);
            
            if (elementData.nodes.length === 0) {
                alert('No elements to save');
                return;
            }
            
            // Prompt for name
            const elementName = prompt('Enter a name for this element(s):');
            if (!elementName || !elementName.trim()) {
                return;
            }
            
            // Create save data with metadata
            const saveData = {
                name: elementName.trim(),
                savedAt: new Date().toISOString(),
                nodeCount: elementData.nodes.length,
                connectionCount: elementData.connections.length,
                ...elementData
            };
            
            // Get existing saved elements from localStorage
            const savedElements = this.getSavedElements();
            
            // Check if name already exists
            const existingIndex = savedElements.findIndex(e => e.name === saveData.name);
            if (existingIndex >= 0) {
                if (!confirm(`An element named "${saveData.name}" already exists. Overwrite?`)) {
                    return;
                }
                savedElements[existingIndex] = saveData;
            } else {
                savedElements.push(saveData);
            }
            
            // Save to localStorage
            try {
                localStorage.setItem('epr_saved_elements', JSON.stringify(savedElements));
                alert(`Element(s) saved successfully as "${saveData.name}"!\n\nNodes: ${saveData.nodeCount}\nConnections: ${saveData.connectionCount}`);
            } catch (error) {
                console.error('[EPR Visual Editor] Error saving element:', error);
                alert('Error saving element: ' + error.message);
            }
        }
        
        /**
         * Get all saved elements from localStorage
         */
        getSavedElements() {
            try {
                const saved = localStorage.getItem('epr_saved_elements');
                return saved ? JSON.parse(saved) : [];
            } catch (error) {
                console.error('[EPR Visual Editor] Error reading saved elements:', error);
                return [];
            }
        }
        
        /**
         * Show modal with list of saved elements
         */
        showLoadElementModal() {
            const savedElements = this.getSavedElements();
            const modal = document.getElementById('eprLoadElementModal');
            const listContainer = document.getElementById('eprSavedElementsList');
            
            if (!modal || !listContainer) {
                alert('Load element modal not found');
                return;
            }
            
            if (savedElements.length === 0) {
                listContainer.innerHTML = '<p class="text-muted">No saved elements found.</p>';
            } else {
                listContainer.innerHTML = savedElements.map((element, index) => {
                    const savedDate = new Date(element.savedAt);
                    const elementName = this.escapeHtml(element.name);
                    return `
                        <div class="card mb-2" style="cursor: pointer;" data-element-index="${index}">
                            <div class="card-body">
                                <h6 class="card-title">${elementName}</h6>
                                <p class="card-text small text-muted mb-1">
                                    <span>Nodes: ${element.nodeCount}</span> | 
                                    <span>Connections: ${element.connectionCount}</span>
                                </p>
                                <p class="card-text small text-muted mb-0">
                                    Saved: ${savedDate.toLocaleDateString()} ${savedDate.toLocaleTimeString()}
                                </p>
                            </div>
                        </div>
                    `;
                }).join('');
                
                // Add click handlers
                listContainer.querySelectorAll('[data-element-index]').forEach(card => {
                    card.addEventListener('click', () => {
                        const index = parseInt(card.getAttribute('data-element-index'));
                        this.loadElement(index);
                    });
                });
            }
            
            // Show modal using Bootstrap
            const bsModal = new bootstrap.Modal(modal);
            bsModal.show();
        }
        
        /**
         * Load a saved element by index
         */
        loadElement(index) {
            const savedElements = this.getSavedElements();
            
            if (index < 0 || index >= savedElements.length) {
                alert('Invalid element index');
                return;
            }
            
            const elementData = savedElements[index];
            
            try {
                this.canvasManager.loadElement(elementData);
                
                // Close modal
                const modal = document.getElementById('eprLoadElementModal');
                if (modal) {
                    const bsModal = bootstrap.Modal.getInstance(modal);
                    if (bsModal) {
                        bsModal.hide();
                    }
                }
                
                alert(`Element "${elementData.name}" loaded successfully!`);
            } catch (error) {
                console.error('[EPR Visual Editor] Error loading element:', error);
                alert('Error loading element: ' + error.message);
            }
        }
        
        /**
         * Escape HTML to prevent XSS
         */
        escapeHtml(text) {
            const div = document.createElement('div');
            div.textContent = text;
            return div.innerHTML;
        }
        
        setupConnectionCreation() {
            this.connectionStartNode = null;
            this.connectionLine = null;
            this.isConnecting = false;
            
            // Setup connection ports for existing nodes
            this.canvasManager.nodes.forEach((node, nodeId) => {
                const nodeEl = document.querySelector(`[data-node-id="${nodeId}"]`);
                if (nodeEl) {
                    this.setupConnectionPort(nodeEl, nodeId);
                }
            });
            
            // Watch for new nodes and add connection ports
            const observer = new MutationObserver(() => {
                this.canvasManager.nodes.forEach((node, nodeId) => {
                    const nodeEl = document.querySelector(`[data-node-id="${nodeId}"]`);
                    if (nodeEl) {
                        const port = nodeEl.querySelector('.epr-connection-port');
                        if (port && !port.dataset.setup) {
                            this.setupConnectionPort(nodeEl, nodeId);
                        }
                    }
                });
            });
            
            observer.observe(document.getElementById('eprNodesLayer'), { childList: true, subtree: true });
            
            document.addEventListener('mousemove', (e) => {
                if (!this.connectionLine || !this.isConnecting) return;
                
                const canvas = document.getElementById('eprCanvas');
                if (!canvas) return;
                
                const canvasRect = canvas.getBoundingClientRect();
                const x = (e.clientX - canvasRect.left - this.canvasManager.panOffset.x) / this.canvasManager.zoomLevel;
                const y = (e.clientY - canvasRect.top - this.canvasManager.panOffset.y) / this.canvasManager.zoomLevel;
                
                this.connectionLine.setAttribute('x2', x);
                this.connectionLine.setAttribute('y2', y);
            });
            
            document.addEventListener('mouseup', (e) => {
                if (!this.connectionStartNode || !this.connectionLine || !this.isConnecting) {
                    // Clean up if connection was cancelled
                    if (this.connectionLine) {
                        this.connectionLine.remove();
                        this.connectionLine = null;
                    }
                    this.isConnecting = false;
                    this.connectionStartNode = null;
                    this.connectionStartPort = null;
                    return;
                }
                
                // Check if dropped on a connection port
                const portEl = e.target.closest('.epr-connection-port');
                if (portEl && portEl.dataset.nodeId && portEl.dataset.nodeId !== this.connectionStartNode) {
                    const toNodeId = portEl.dataset.nodeId;
                    const toPort = portEl.dataset.portSide || portEl.dataset.port || 'left';
                    const fromPort = this.connectionStartPort || 'right';
                    
                    console.log('[EPR Visual Editor] Creating connection from', this.connectionStartNode, '(', fromPort, ') to', toNodeId, '(', toPort, ')');
                    this.canvasManager.addConnection(this.connectionStartNode, toNodeId, fromPort, toPort);
                } else {
                    // Fallback: check if dropped on node
                    const nodeEl = e.target.closest('.epr-canvas-node');
                    if (nodeEl && nodeEl.dataset.nodeId && nodeEl.dataset.nodeId !== this.connectionStartNode) {
                        const toNodeId = nodeEl.dataset.nodeId;
                        const toNode = this.canvasManager.nodes.get(toNodeId);
                        const fromPort = this.connectionStartPort || 'right';
                        
                        // For distribution nodes, allow connection to either port
                        // Determine which port based on mouse position relative to node
                        let toPort = 'left';
                        if (toNode && toNode.type === 'distribution') {
                            const nodeElRect = nodeEl.getBoundingClientRect();
                            const nodeCenterX = nodeElRect.left + nodeElRect.width / 2;
                            // If mouse is on right side of node, connect to right port
                            toPort = e.clientX > nodeCenterX ? 'right' : 'left';
                        } else {
                            toPort = fromPort === 'right' ? 'left' : 'right'; // Default opposite port
                        }
                        
                        console.log('[EPR Visual Editor] Creating connection from', this.connectionStartNode, '(', fromPort, ') to', toNodeId, '(', toPort, ')');
                        this.canvasManager.addConnection(this.connectionStartNode, toNodeId, fromPort, toPort);
                    }
                }
                
                // Clean up
                if (this.connectionLine) {
                    this.connectionLine.remove();
                    this.connectionLine = null;
                }
                this.isConnecting = false;
                this.connectionStartNode = null;
                this.connectionStartPort = null;
            });
        }
        
        setupConnectionPort(portEl, nodeId, portSide) {
            if (!portEl || portEl.dataset.setup === 'true') return;
            
            portEl.dataset.setup = 'true';
            portEl.dataset.portSide = portSide;
            portEl.dataset.port = portSide; // Also set port for compatibility
            portEl.dataset.nodeId = nodeId;
            
            portEl.addEventListener('mousedown', (e) => {
                e.preventDefault();
                e.stopPropagation();
                this.isConnecting = true;
                this.connectionStartNode = nodeId;
                this.connectionStartPort = portSide;
                
                const svg = document.getElementById('eprConnectionsLayer');
                if (!svg) return;
                
                this.connectionLine = document.createElementNS('http://www.w3.org/2000/svg', 'line');
                this.connectionLine.setAttribute('stroke', '#28a745');
                this.connectionLine.setAttribute('stroke-width', '3');
                this.connectionLine.setAttribute('stroke-dasharray', '5,5');
                this.connectionLine.setAttribute('opacity', '0.7');
                svg.appendChild(this.connectionLine);
                
                const node = this.canvasManager.nodes.get(nodeId);
                if (node) {
                    const startX = portSide === 'right' ? node.x + 150 : node.x;
                    const startY = node.y + 40;
                    this.connectionLine.setAttribute('x1', startX);
                    this.connectionLine.setAttribute('y1', startY);
                }
            });
        }
        
        setupAlignDistributeHandlers() {
            // Align Left
            const alignLeftBtn = document.getElementById('eprAlignLeftBtn');
            if (alignLeftBtn) {
                alignLeftBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    this.canvasManager.alignNodes('left');
                });
            }
            
            // Align Right
            const alignRightBtn = document.getElementById('eprAlignRightBtn');
            if (alignRightBtn) {
                alignRightBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    this.canvasManager.alignNodes('right');
                });
            }
            
            // Align Center
            const alignCenterBtn = document.getElementById('eprAlignCenterBtn');
            if (alignCenterBtn) {
                alignCenterBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    this.canvasManager.alignNodes('center');
                });
            }
            
            // Align Top
            const alignTopBtn = document.getElementById('eprAlignTopBtn');
            if (alignTopBtn) {
                alignTopBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    this.canvasManager.alignNodes('top');
                });
            }
            
            // Align Bottom
            const alignBottomBtn = document.getElementById('eprAlignBottomBtn');
            if (alignBottomBtn) {
                alignBottomBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    this.canvasManager.alignNodes('bottom');
                });
            }
            
            // Distribute Horizontally
            const distributeHorizBtn = document.getElementById('eprDistributeHorizontallyBtn');
            if (distributeHorizBtn) {
                distributeHorizBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    this.canvasManager.distributeNodes('horizontal');
                });
            }
            
            // Distribute Vertically
            const distributeVertBtn = document.getElementById('eprDistributeVerticallyBtn');
            if (distributeVertBtn) {
                distributeVertBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    this.canvasManager.distributeNodes('vertical');
                });
            }
        }
        
        /**
         * Lock 6 palettes horizontally in position on startup
         * Order: Raw Materials, Packaging Library, Packaging Groups, Product Library, Distribution, Distribution Groups
         */
        lockPalettesHorizontally() {
            // Wait a bit for palettes to be rendered
            setTimeout(() => {
                // Force split mode on startup
                const typesToolbar = document.getElementById('eprTypesToolbar');
                const splitBtn = document.getElementById('eprSplitTypesBtn');
                
                // Ensure split mode is active
                if (typesToolbar && !typesToolbar.classList.contains('split-mode')) {
                    typesToolbar.style.display = 'none';
                    typesToolbar.classList.add('split-mode');
                    document.querySelectorAll('.epr-types-panel').forEach(panel => {
                        panel.style.display = '';
                    });
                    if (splitBtn) {
                        splitBtn.innerHTML = '<i class="bi bi-unlock"></i>';
                        splitBtn.classList.add('active');
                        splitBtn.title = 'Lock Toolbars';
                    }
                    
                    // Sync content to split panels
                    if (window.EPRVisualEditor && window.EPRVisualEditor.typesToolbar) {
                        window.EPRVisualEditor.typesToolbar.syncToSplitPanels();
                        window.EPRVisualEditor.typesToolbar.renderRawMaterials();
                    }
                }
                
                // Order: Raw Materials, Packaging Library, Packaging Groups, Product Library, Distribution, Distribution Groups
                // Also include Notes (Parameters) panel
                const palettes = [
                    { id: 'eprTypesPanelRawMaterials', selector: '.epr-types-panel-raw-materials' },
                    { id: 'eprTypesPanelPackaging', selector: '.epr-types-panel-packaging' },
                    { id: 'eprTypesPanelPackagingGroups', selector: '.epr-types-panel-packaging-groups' },
                    { id: 'eprTypesPanelProducts', selector: '.epr-types-panel-products' },
                    { id: 'eprTypesPanelDistribution', selector: '.epr-types-panel-distribution' },
                    { id: 'eprTypesPanelDistributionGroups', selector: '.epr-types-panel-distribution-groups' },
                    { id: 'eprParametersPanel', selector: '.epr-parameters-panel' }
                ];
                
                // Calculate horizontal positions (40px from left, 200px from bottom)
                const panelWidths = {
                    'eprTypesPanelRawMaterials': 280,
                    'eprTypesPanelPackaging': 220,
                    'eprTypesPanelPackagingGroups': 220,
                    'eprTypesPanelProducts': 220,
                    'eprTypesPanelDistribution': 220,
                    'eprTypesPanelDistributionGroups': 220,
                    'eprParametersPanel': 300
                };
                const panelSpacing = 20; // Spacing between panels
                const startLeft = 40;
                const bottomPosition = 200;
                
                // Lock split panels horizontally at bottom
                let currentLeft = startLeft;
                palettes.forEach((palette, index) => {
                    const panel = document.querySelector(palette.selector);
                    if (panel) {
                        // Ensure panel is visible (especially Notes panel)
                        panel.style.display = '';
                        panel.style.visibility = 'visible';
                        
                        // Get panel width
                        const panelWidth = panelWidths[palette.id] || 220;
                        
                        // Position horizontally at bottom
                        const leftPosition = currentLeft;
                        panel.style.left = `${leftPosition}px`;
                        panel.style.bottom = `${bottomPosition}px`;
                        panel.style.top = 'auto';
                        panel.style.right = 'auto';
                        
                        // Update currentLeft for next panel
                        currentLeft += panelWidth + panelSpacing;
                        
                        // Set locked state
                        panel.setAttribute('data-toolbar-locked', 'true');
                        
                        // Update lock button if it exists
                        const lockBtn = panel.querySelector('.epr-btn-lock-toolbar');
                        if (lockBtn) {
                            lockBtn.classList.add('locked');
                            const icon = lockBtn.querySelector('i');
                            if (icon) {
                                icon.className = 'bi bi-lock-fill';
                            }
                        }
                        
                        // Connect panels together
                        if (index > 0) {
                            const prevPanel = document.querySelector(palettes[index - 1].selector);
                            if (prevPanel && window.eprToolbarConnections) {
                                if (!window.eprToolbarConnections.has(palette.id)) {
                                    window.eprToolbarConnections.set(palette.id, new Set());
                                }
                                if (!window.eprToolbarConnections.has(palettes[index - 1].id)) {
                                    window.eprToolbarConnections.set(palettes[index - 1].id, new Set());
                                }
                                window.eprToolbarConnections.get(palette.id).add(palettes[index - 1].id);
                                window.eprToolbarConnections.get(palettes[index - 1].id).add(palette.id);
                            }
                        }
                        
                        console.log(`[EPR Visual Editor] Locked palette: ${palette.id} at left: ${leftPosition}px, bottom: ${bottomPosition}px`);
                    }
                });
                
                // Ensure Notes (Parameters) panel stays visible even when unlocked
                const notesPanel = document.getElementById('eprParametersPanel');
                if (notesPanel) {
                    notesPanel.style.display = '';
                    notesPanel.style.visibility = 'visible';
                    // Ensure it's not hidden by visibility toggle
                    const visibilityPalettes = document.getElementById('eprVisibilityPalettes');
                    if (visibilityPalettes && visibilityPalettes.checked) {
                        notesPanel.style.display = '';
                    }
                }
            }, 500);
        }
    }
    
    // Add saveParameter method to ParametersPanel
    EPRParametersPanel.prototype.saveParameter = function(key, value) {
        if (!this.currentNode) return;
        
        if (!this.currentNode.parameters) {
            this.currentNode.parameters = {};
        }
        
        if (key === 'height' || key === 'weight' || key === 'depth' || key === 'quantity') {
            this.currentNode.parameters[key] = value ? parseFloat(value) : null;
        } else {
            this.currentNode.parameters[key] = value;
        }
        
        if (key === 'name' && value) {
            this.currentNode.name = value;
            this.canvasManager.renderNode(this.currentNode);
        }
        
        this.canvasManager.saveState();
    };
    
    // Export classes globally
    window.EPRCanvasManager = EPRCanvasManager;
    window.EPRTypesToolbar = EPRTypesToolbar;
    window.EPRParametersPanel = EPRParametersPanel;
    window.EPRActionsToolbar = EPRActionsToolbar;
    window.EPRVisualEditorApp = EPRVisualEditorApp;
    
    console.log('%c[EPR Visual Editor] ✅✅✅ visual-editor-complete.js loaded, classes exported ✅✅✅', 'color: green; font-weight: bold; font-size: 16px; background: lightgreen; padding: 10px;');
    console.log('[EPR Visual Editor] Available classes:', {
        EPRVisualEditorApp: typeof window.EPRVisualEditorApp !== 'undefined',
        EPRCanvasManager: typeof window.EPRCanvasManager !== 'undefined',
        EPRTypesToolbar: typeof window.EPRTypesToolbar !== 'undefined',
        EPRParametersPanel: typeof window.EPRParametersPanel !== 'undefined',
        EPRActionsToolbar: typeof window.EPRActionsToolbar !== 'undefined'
    });
    
    // Signal that script is ready
    window.EPRVisualEditorScriptLoaded = true;
    console.log('%c[EPR Visual Editor] Script ready flag set: EPRVisualEditorScriptLoaded = true', 'color: green; font-weight: bold;');
})();