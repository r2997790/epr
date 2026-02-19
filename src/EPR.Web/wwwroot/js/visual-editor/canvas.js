// Canvas Module - Handles canvas operations, drag-drop, grid, connections
console.log('[Visual Editor] canvas.js loading...');

class CanvasManager {
    constructor() {
        this.canvas = document.getElementById('canvas');
        this.nodesLayer = document.getElementById('nodesLayer');
        this.connectionsLayer = document.getElementById('connectionsLayer');
        
        if (!this.canvas || !this.nodesLayer || !this.connectionsLayer) {
            console.error('Canvas elements not found');
            return;
        }
        
        this.nodes = new Map();
        this.connections = [];
        this.selectedNode = null;
        this.dragNode = null;
        this.dragOffset = { x: 0, y: 0 };
        this.isDragging = false;
        this.gridSize = 20;
        this.showGrid = false;
        this.snapToGrid = false;
        this.viewMode = 'simple'; // 'simple' or 'sankey'
        this.zoomLevel = 1;
        this.panOffset = { x: 0, y: 0 };
        this.history = [];
        this.historyIndex = -1;
        
        this.init();
    }

    init() {
        if (!this.canvas || !this.nodesLayer || !this.connectionsLayer) {
            console.error('Canvas elements not found during init');
            console.error('Canvas:', this.canvas);
            console.error('Nodes Layer:', this.nodesLayer);
            console.error('Connections Layer:', this.connectionsLayer);
            return;
        }
        
        console.log('Initializing canvas with elements:', {
            canvas: this.canvas.id,
            nodesLayer: this.nodesLayer.id,
            connectionsLayer: this.connectionsLayer.id
        });
        
        this.setupEventListeners();
        this.setupDragAndDrop();
        this.updateGridDisplay();
        this.resizeConnectionsLayer();
        this.updatePlaceholder();
        
        // Resize connections layer on window resize
        window.addEventListener('resize', () => {
            setTimeout(() => this.resizeConnectionsLayer(), 100);
        });
        
        // Force initial resize after a short delay to ensure layout is complete
        setTimeout(() => {
            this.resizeConnectionsLayer();
            console.log('Canvas fully initialized. Canvas size:', {
                width: this.canvas.offsetWidth,
                height: this.canvas.offsetHeight
            });
        }, 200);
        
        console.log('Canvas initialized successfully');
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
        // Canvas click to deselect
        this.canvas.addEventListener('click', (e) => {
            if (e.target === this.canvas || e.target === this.nodesLayer || e.target === this.connectionsLayer) {
                this.deselectNode();
            }
        });

        // Canvas panning (only when not dragging a node)
        let isPanning = false;
        let panStart = { x: 0, y: 0 };

        this.canvas.addEventListener('mousedown', (e) => {
            // Don't pan if clicking on a node
            if (e.target.closest('.canvas-node')) return;
            
            if (e.target === this.canvas || e.target === this.nodesLayer || e.target === this.connectionsLayer) {
                isPanning = true;
                panStart.x = e.clientX - this.panOffset.x;
                panStart.y = e.clientY - this.panOffset.y;
                this.canvas.style.cursor = 'grabbing';
            }
        });

        this.canvas.addEventListener('mousemove', (e) => {
            if (isPanning) {
                this.panOffset.x = e.clientX - panStart.x;
                this.panOffset.y = e.clientY - panStart.y;
                this.updateTransform();
            }
        });

        this.canvas.addEventListener('mouseup', () => {
            isPanning = false;
            this.canvas.style.cursor = 'default';
        });

        // Prevent context menu
        this.canvas.addEventListener('contextmenu', (e) => {
            e.preventDefault();
        });
    }

    setupDragAndDrop() {
        if (!this.canvas) {
            console.error('Canvas not available for drag and drop setup');
            return;
        }
        
        console.log('Setting up drag and drop on canvas');
        
        // Allow drops on canvas
        this.canvas.addEventListener('dragover', (e) => {
            e.preventDefault();
            e.stopPropagation();
            this.canvas.classList.add('drag-over');
            e.dataTransfer.dropEffect = 'copy';
        });

        this.canvas.addEventListener('dragleave', (e) => {
            e.preventDefault();
            e.stopPropagation();
            // Only remove drag-over if we're actually leaving the canvas
            if (!this.canvas.contains(e.relatedTarget)) {
                this.canvas.classList.remove('drag-over');
            }
        });

        this.canvas.addEventListener('drop', (e) => {
            e.preventDefault();
            e.stopPropagation();
            this.canvas.classList.remove('drag-over');

            const data = e.dataTransfer.getData('application/json');
            console.log('Drop event received, data:', data);
            
            if (!data) {
                console.warn('No drop data found');
                return;
            }

            try {
                const nodeData = JSON.parse(data);
                const rect = this.canvas.getBoundingClientRect();
                const x = e.clientX - rect.left;
                const y = e.clientY - rect.top;

                console.log('Drop position:', { x, y, rect });

                // Apply snap to grid if enabled
                let finalX = x;
                let finalY = y;
                if (this.snapToGrid) {
                    finalX = Math.round(x / this.gridSize) * this.gridSize;
                    finalY = Math.round(y / this.gridSize) * this.gridSize;
                }

                nodeData.x = finalX;
                nodeData.y = finalY;
                const node = this.addNode(nodeData);
                console.log('Node added successfully:', node);
            } catch (error) {
                console.error('Error parsing drop data:', error);
                alert('Error adding item: ' + error.message);
            }
        });
        
        console.log('Drag and drop handlers attached');
    }

    updateTransform() {
        const transform = `translate(${this.panOffset.x}px, ${this.panOffset.y}px) scale(${this.zoomLevel})`;
        this.nodesLayer.style.transform = transform;
        this.connectionsLayer.style.transform = transform;
    }

    addNode(nodeData) {
        const node = {
            id: nodeData.id || `node-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`,
            type: nodeData.type,
            entityId: nodeData.entityId,
            x: nodeData.x || 100,
            y: nodeData.y || 100,
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

    renderNode(node) {
        // Remove existing node element if present
        const existing = document.querySelector(`[data-node-id="${node.id}"]`);
        if (existing) {
            existing.remove();
        }

        const nodeEl = document.createElement('div');
        nodeEl.className = 'canvas-node';
        nodeEl.setAttribute('data-node-id', node.id);
        nodeEl.setAttribute('data-type', node.type);
        nodeEl.style.left = `${node.x}px`;
        nodeEl.style.top = `${node.y}px`;

        nodeEl.innerHTML = `
            <div class="node-header">
                <div style="display: flex; align-items: center;">
                    <i class="bi ${node.icon} node-icon"></i>
                    <span class="node-title">${this.escapeHtml(node.name)}</span>
                </div>
                <button class="node-delete" onclick="canvasManager.deleteNode('${node.id}')" title="Delete">
                    <i class="bi bi-x-circle"></i>
                </button>
            </div>
            <div class="node-type">${this.getTypeLabel(node.type)}</div>
        `;

        // Make node draggable
        this.makeNodeDraggable(nodeEl, node);

        // Select on click
        nodeEl.addEventListener('click', (e) => {
            if (!e.target.closest('.node-delete')) {
                this.selectNode(node.id);
            }
        });

        this.nodesLayer.appendChild(nodeEl);
        this.updateConnections();
    }

    makeNodeDraggable(nodeEl, node) {
        let isDragging = false;
        let startX = 0;
        let startY = 0;
        let initialX = node.x;
        let initialY = node.y;

        nodeEl.addEventListener('mousedown', (e) => {
            if (e.target.closest('.node-delete')) return;
            
            isDragging = true;
            this.dragNode = node.id;
            nodeEl.classList.add('dragging');
            
            const rect = nodeEl.getBoundingClientRect();
            const canvasRect = this.canvas.getBoundingClientRect();
            startX = e.clientX - rect.left;
            startY = e.clientY - rect.top;

            document.addEventListener('mousemove', handleMouseMove);
            document.addEventListener('mouseup', handleMouseUp);
        });

        const handleMouseMove = (e) => {
            if (!isDragging) return;

            const canvasRect = this.canvas.getBoundingClientRect();
            let newX = e.clientX - canvasRect.left - startX;
            let newY = e.clientY - canvasRect.top - startY;

            // Apply snap to grid
            if (this.snapToGrid) {
                newX = Math.round(newX / this.gridSize) * this.gridSize;
                newY = Math.round(newY / this.gridSize) * this.gridSize;
            }

            node.x = newX;
            node.y = newY;
            nodeEl.style.left = `${newX}px`;
            nodeEl.style.top = `${newY}px`;

            this.updateConnections();
        };

        const handleMouseUp = () => {
            if (isDragging) {
                isDragging = false;
                this.dragNode = null;
                nodeEl.classList.remove('dragging');
                this.saveState();
            }
            document.removeEventListener('mousemove', handleMouseMove);
            document.removeEventListener('mouseup', handleMouseUp);
        };
    }

    deleteNode(nodeId) {
        if (confirm('Delete this node?')) {
            this.nodes.delete(nodeId);
            this.connections = this.connections.filter(c => c.from !== nodeId && c.to !== nodeId);
            
            const nodeEl = document.querySelector(`[data-node-id="${nodeId}"]`);
            if (nodeEl) {
                nodeEl.remove();
            }

            if (this.selectedNode === nodeId) {
                this.deselectNode();
            }

            this.updateConnections();
            this.updatePlaceholder();
            this.saveState();
        }
    }

    selectNode(nodeId) {
        this.selectedNode = nodeId;
        this.updateNodeSelection();
        
        // Notify parameters panel
        if (window.parametersPanel) {
            window.parametersPanel.loadNodeParameters(this.nodes.get(nodeId));
        }
    }

    deselectNode() {
        this.selectedNode = null;
        this.updateNodeSelection();
        
        if (window.parametersPanel) {
            window.parametersPanel.clearParameters();
        }
    }

    updateNodeSelection() {
        document.querySelectorAll('.canvas-node').forEach(el => {
            el.classList.remove('selected');
        });

        if (this.selectedNode) {
            const nodeEl = document.querySelector(`[data-node-id="${this.selectedNode}"]`);
            if (nodeEl) {
                nodeEl.classList.add('selected');
            }
        }
    }

    addConnection(fromNodeId, toNodeId, properties = {}) {
        // Check if connection already exists
        if (this.connections.some(c => c.from === fromNodeId && c.to === toNodeId)) {
            return;
        }

        this.connections.push({
            from: fromNodeId,
            to: toNodeId,
            properties: properties
        });

        this.updateConnections();
        this.saveState();
    }

    removeConnection(fromNodeId, toNodeId) {
        this.connections = this.connections.filter(
            c => !(c.from === fromNodeId && c.to === toNodeId)
        );
        this.updateConnections();
        this.saveState();
    }

    updateConnections() {
        // Clear existing connections
        this.connectionsLayer.innerHTML = '';

        if (this.viewMode === 'sankey') {
            this.renderSankeyConnections();
        } else {
            this.renderSimpleConnections();
        }
    }

    renderSimpleConnections() {
        // Clear existing connections
        const existingLines = this.connectionsLayer.querySelectorAll('.connection-line');
        existingLines.forEach(line => line.remove());

        this.connections.forEach(conn => {
            const fromNode = this.nodes.get(conn.from);
            const toNode = this.nodes.get(conn.to);

            if (!fromNode || !toNode) return;

            // Get node elements to calculate center positions
            const fromEl = document.querySelector(`[data-node-id="${conn.from}"]`);
            const toEl = document.querySelector(`[data-node-id="${conn.to}"]`);

            if (!fromEl || !toEl) return;

            const fromRect = fromEl.getBoundingClientRect();
            const toRect = toEl.getBoundingClientRect();
            const canvasRect = this.canvas.getBoundingClientRect();

            // Calculate positions relative to canvas
            const x1 = fromNode.x + 75; // Center of node horizontally
            const y1 = fromNode.y + 40; // Center of node vertically
            const x2 = toNode.x + 75;
            const y2 = toNode.y + 40;

            const line = document.createElementNS('http://www.w3.org/2000/svg', 'line');
            line.setAttribute('x1', x1);
            line.setAttribute('y1', y1);
            line.setAttribute('x2', x2);
            line.setAttribute('y2', y2);
            line.setAttribute('class', 'connection-line');
            line.setAttribute('data-from', conn.from);
            line.setAttribute('data-to', conn.to);
            line.setAttribute('stroke', '#6c757d');
            line.setAttribute('stroke-width', '2');
            line.setAttribute('fill', 'none');

            // Make connections deletable on right-click
            line.addEventListener('contextmenu', (e) => {
                e.preventDefault();
                e.stopPropagation();
                if (confirm('Delete this connection?')) {
                    this.removeConnection(conn.from, conn.to);
                }
            });

            this.connectionsLayer.appendChild(line);
        });
    }

    renderSankeyConnections() {
        // Sankey rendering will use Plotly.js
        // This is a placeholder - will be implemented with actual Sankey diagram
        this.renderSimpleConnections(); // Fallback to simple for now
    }

    toggleGrid() {
        this.showGrid = !this.showGrid;
        this.updateGridDisplay();
    }

    toggleSnapToGrid() {
        this.snapToGrid = !this.snapToGrid;
        document.getElementById('snapToGridBtn').classList.toggle('active', this.snapToGrid);
    }

    updateGridDisplay() {
        if (this.showGrid) {
            this.canvas.classList.add('grid-visible');
        } else {
            this.canvas.classList.remove('grid-visible');
        }
        document.getElementById('toggleGridBtn').classList.toggle('active', this.showGrid);
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
        // Arrange nodes in left-to-right flow
        const nodesByType = {
            'raw-material': [],
            'packaging': [],
            'product': [],
            'distribution': []
        };

        this.nodes.forEach(node => {
            if (nodesByType[node.type]) {
                nodesByType[node.type].push(node);
            }
        });

        const startX = 100;
        const startY = 100;
        const columnSpacing = 300;
        const rowSpacing = 120;

        let currentX = startX;
        
        ['raw-material', 'packaging', 'product', 'distribution'].forEach(type => {
            const nodes = nodesByType[type];
            if (nodes.length === 0) return;

            let currentY = startY;
            nodes.forEach((node, index) => {
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

    toggleViewMode() {
        this.viewMode = this.viewMode === 'simple' ? 'sankey' : 'simple';
        this.canvas.classList.toggle('sankey-view', this.viewMode === 'sankey');
        this.updateConnections();
    }

    saveState() {
        // Save to history for undo
        const state = {
            nodes: Array.from(this.nodes.values()),
            connections: [...this.connections]
        };
        
        this.history = this.history.slice(0, this.historyIndex + 1);
        this.history.push(JSON.parse(JSON.stringify(state)));
        this.historyIndex = this.history.length - 1;

        // Limit history size
        if (this.history.length > 50) {
            this.history.shift();
            this.historyIndex--;
        }
    }

    undo() {
        if (this.historyIndex > 0) {
            this.historyIndex--;
            const state = this.history[this.historyIndex];
            this.restoreState(state);
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

    loadState(state) {
        this.restoreState(state);
    }

    escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    getTypeLabel(type) {
        const labels = {
            'raw-material': 'Raw Material',
            'packaging': 'Packaging',
            'product': 'Product',
            'distribution': 'Distribution'
        };
        return labels[type] || type;
    }

    updatePlaceholder() {
        const placeholder = document.getElementById('canvasPlaceholder');
        if (placeholder) {
            if (this.nodes.size > 0) {
                placeholder.classList.add('hidden');
            } else {
                placeholder.classList.remove('hidden');
            }
        }
    }
}

// Export for use in other modules
window.CanvasManager = CanvasManager;

