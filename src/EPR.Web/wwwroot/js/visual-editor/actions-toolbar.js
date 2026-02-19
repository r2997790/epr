// Actions Toolbar Module - Handles toolbar actions and interactions
console.log('[Visual Editor] actions-toolbar.js loading...');

class ActionsToolbar {
    constructor(canvasManager) {
        this.canvasManager = canvasManager;
        this.isDragMode = false;
        this.init();
    }

    init() {
        this.setupEventListeners();
        this.setupToolbarDragging();
    }

    setupEventListeners() {
        // Drag mode toggle
        document.getElementById('dragModeBtn').addEventListener('click', () => {
            this.toggleDragMode();
        });

        // Zoom controls
        document.getElementById('zoomInBtn').addEventListener('click', () => {
            this.canvasManager.zoomIn();
        });

        document.getElementById('zoomOutBtn').addEventListener('click', () => {
            this.canvasManager.zoomOut();
        });

        // Undo
        document.getElementById('undoBtn').addEventListener('click', () => {
            this.canvasManager.undo();
        });

        // Fit to canvas
        document.getElementById('fitToCanvasBtn').addEventListener('click', () => {
            this.canvasManager.fitToCanvas();
        });

        // Arrange nodes
        document.getElementById('arrangeBtn').addEventListener('click', () => {
            this.canvasManager.arrangeNodes();
        });

        // Grid toggle
        document.getElementById('toggleGridBtn').addEventListener('click', () => {
            this.canvasManager.toggleGrid();
        });

        // Snap to grid toggle
        document.getElementById('snapToGridBtn').addEventListener('click', () => {
            this.canvasManager.toggleSnapToGrid();
        });

        // View mode toggle
        document.getElementById('toggleViewBtn').addEventListener('click', () => {
            this.canvasManager.toggleViewMode();
            document.getElementById('toggleViewBtn').classList.toggle('active', 
                this.canvasManager.viewMode === 'sankey');
        });

        // Toggle toolbars visibility
        document.getElementById('toggleToolbarsBtn').addEventListener('click', () => {
            this.toggleToolbarsVisibility();
        });

        // Toolbar minimize buttons
        document.querySelectorAll('.btn-close-toolbar').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const toolbar = e.target.closest('.floating-toolbar');
                if (toolbar) {
                    toolbar.classList.toggle('minimized');
                }
            });
        });
    }

    setupToolbarDragging() {
        document.querySelectorAll('.floating-toolbar').forEach(toolbar => {
            const header = toolbar.querySelector('.toolbar-header');
            if (!header) return;

            let isDragging = false;
            let startX = 0;
            let startY = 0;
            let initialX = 0;
            let initialY = 0;

            header.addEventListener('mousedown', (e) => {
                if (e.target.closest('.btn-close-toolbar')) return;
                
                isDragging = true;
                startX = e.clientX;
                startY = e.clientY;
                
                const rect = toolbar.getBoundingClientRect();
                initialX = rect.left;
                initialY = rect.top;

                document.addEventListener('mousemove', handleMouseMove);
                document.addEventListener('mouseup', handleMouseUp);
            });

            const handleMouseMove = (e) => {
                if (!isDragging) return;

                const deltaX = e.clientX - startX;
                const deltaY = e.clientY - startY;

                let newX = initialX + deltaX;
                let newY = initialY + deltaY;

                // Keep toolbar within viewport
                const maxX = window.innerWidth - toolbar.offsetWidth;
                const maxY = window.innerHeight - toolbar.offsetHeight;

                newX = Math.max(0, Math.min(newX, maxX));
                newY = Math.max(0, Math.min(newY, maxY));

                toolbar.style.left = `${newX}px`;
                toolbar.style.top = `${newY}px`;
            };

            const handleMouseUp = () => {
                isDragging = false;
                document.removeEventListener('mousemove', handleMouseMove);
                document.removeEventListener('mouseup', handleMouseUp);
            };
        });
    }

    toggleDragMode() {
        this.isDragMode = !this.isDragMode;
        document.getElementById('dragModeBtn').classList.toggle('active', this.isDragMode);
        
        // Change cursor based on drag mode
        const canvas = document.getElementById('canvas');
        if (this.isDragMode) {
            canvas.style.cursor = 'move';
        } else {
            canvas.style.cursor = 'default';
        }
    }

    toggleToolbarsVisibility() {
        const toolbars = document.querySelectorAll('.floating-toolbar');
        const isHidden = toolbars[0].style.display === 'none';
        
        toolbars.forEach(toolbar => {
            toolbar.style.display = isHidden ? '' : 'none';
        });

        document.getElementById('toggleToolbarsBtn').classList.toggle('active', !isHidden);
        document.getElementById('toggleToolbarsBtn').innerHTML = isHidden 
            ? '<i class="bi bi-eye"></i>' 
            : '<i class="bi bi-eye-slash"></i>';
    }
}

window.ActionsToolbar = ActionsToolbar;

