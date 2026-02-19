// Main Visual Editor Application
console.log('[Visual Editor] main.js executing...');
console.log('[Visual Editor] CanvasManager available:', typeof CanvasManager !== 'undefined');
console.log('[Visual Editor] TypesToolbar available:', typeof TypesToolbar !== 'undefined');

class VisualEditorApp {
    constructor() {
        console.log('[Visual Editor] VisualEditorApp constructor called');
        this.canvasManager = null;
        this.typesToolbar = null;
        this.parametersPanel = null;
        this.actionsToolbar = null;
        
        // Initialize immediately
        this.init();
    }

    async init() {
        try {
            console.log('Initializing Visual Editor...');
            
            // Check if Bootstrap is available
            if (typeof bootstrap === 'undefined') {
                console.error('Bootstrap not loaded!');
                alert('Bootstrap is required but not loaded. Please refresh the page.');
                return;
            }

            // Initialize canvas manager first
            this.canvasManager = new CanvasManager();
            if (!this.canvasManager.canvas) {
                console.error('Canvas not found!');
                return;
            }
            window.canvasManager = this.canvasManager;

            // Initialize toolbars
            this.typesToolbar = new TypesToolbar(this.canvasManager);
            this.parametersPanel = new ParametersPanel(this.canvasManager);
            this.actionsToolbar = new ActionsToolbar(this.canvasManager);

            // Make parameters panel globally accessible
            window.parametersPanel = this.parametersPanel;

            // Setup project save/load
            this.setupProjectHandlers();

            // Setup connection creation (click and drag between nodes)
            this.setupConnectionCreation();

            console.log('Visual Editor initialized successfully');
        } catch (error) {
            console.error('Error initializing Visual Editor:', error);
            alert('Error initializing Visual Editor: ' + error.message);
        }
    }

    setupProjectHandlers() {
        const saveBtn = document.getElementById('saveProjectBtn');
        const loadBtn = document.getElementById('loadProjectBtn');
        
        if (saveBtn) {
            saveBtn.addEventListener('click', () => {
                this.saveProject();
            });
        }
        
        if (loadBtn) {
            loadBtn.addEventListener('click', () => {
                this.loadProject();
            });
        }
    }

    async saveProject() {
        const projectName = document.getElementById('projectName')?.value.trim();
        
        if (!projectName) {
            alert('Please enter a project name');
            return;
        }

        const projectData = {
            projectName: projectName,
            nodes: this.canvasManager.getState().nodes,
            connections: this.canvasManager.getState().connections
        };

        try {
            const response = await fetch('/api/visual-editor/project', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(projectData)
            });

            if (response.ok) {
                alert('Project saved successfully!');
            } else {
                const errorText = await response.text();
                console.error('Save error:', errorText);
                alert('Error saving project');
            }
        } catch (error) {
            console.error('Error saving project:', error);
            alert('Error saving project: ' + error.message);
        }
    }

    async loadProject() {
        const projectId = prompt('Enter project ID to load:');
        if (!projectId) return;

        try {
            const response = await fetch(`/api/visual-editor/project/${projectId}`);
            const result = await response.json();

            if (result.success && result.data) {
                this.canvasManager.loadState(result.data);
                const projectNameInput = document.getElementById('projectName');
                if (projectNameInput) {
                    projectNameInput.value = result.data.projectName || '';
                }
                alert('Project loaded successfully!');
            } else {
                alert('Project not found');
            }
        } catch (error) {
            console.error('Error loading project:', error);
            alert('Error loading project: ' + error.message);
        }
    }

    setupConnectionCreation() {
        let connectionStartNode = null;
        let connectionLine = null;

        // Click on node to start connection
        document.addEventListener('mousedown', (e) => {
            const nodeEl = e.target.closest('.canvas-node');
            if (!nodeEl) return;

            const nodeId = nodeEl.dataset.nodeId;
            if (!nodeId) return;

            // Start connection on Ctrl+Click or right-click
            if (e.ctrlKey || e.button === 2) {
                e.preventDefault();
                connectionStartNode = nodeId;
                
                // Create temporary connection line
                const svg = document.getElementById('connectionsLayer');
                if (!svg) return;
                
                connectionLine = document.createElementNS('http://www.w3.org/2000/svg', 'line');
                connectionLine.setAttribute('stroke', '#28a745');
                connectionLine.setAttribute('stroke-width', '2');
                connectionLine.setAttribute('stroke-dasharray', '5,5');
                svg.appendChild(connectionLine);

                const node = this.canvasManager.nodes.get(nodeId);
                if (node) {
                    connectionLine.setAttribute('x1', node.x + 75);
                    connectionLine.setAttribute('y1', node.y + 40);
                }
            }
        });

        // Update connection line while dragging
        document.addEventListener('mousemove', (e) => {
            if (!connectionLine) return;

            const canvas = document.getElementById('canvas');
            if (!canvas) return;
            
            const canvasRect = canvas.getBoundingClientRect();
            const x = e.clientX - canvasRect.left;
            const y = e.clientY - canvasRect.top;

            connectionLine.setAttribute('x2', x);
            connectionLine.setAttribute('y2', y);
        });

        // Complete connection on mouse up
        document.addEventListener('mouseup', (e) => {
            if (!connectionStartNode || !connectionLine) return;

            const nodeEl = e.target.closest('.canvas-node');
            if (nodeEl && nodeEl.dataset.nodeId !== connectionStartNode) {
                const toNodeId = nodeEl.dataset.nodeId;
                this.canvasManager.addConnection(connectionStartNode, toNodeId);
            }

            // Clean up
            if (connectionLine && connectionLine.parentNode) {
                connectionLine.parentNode.removeChild(connectionLine);
            }
            connectionStartNode = null;
            connectionLine = null;
        });
    }
}

// Export for manual initialization
console.log('[Visual Editor] main.js loaded, VisualEditorApp class defined');
