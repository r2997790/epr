// Parameters Panel Module - Handles node parameter editing
console.log('[Visual Editor] parameters-panel.js loading...');

class ParametersPanel {
    constructor(canvasManager) {
        this.canvasManager = canvasManager;
        this.currentNode = null;
        this.init();
    }

    init() {
        this.setupEventListeners();
    }

    setupEventListeners() {
        // Auto-save on input change
        document.getElementById('parametersContent').addEventListener('input', (e) => {
            if (e.target.classList.contains('parameter-input')) {
                this.saveParameter(e.target.dataset.paramKey, e.target.value);
            }
        });
    }

    loadNodeParameters(node) {
        this.currentNode = node;
        const container = document.getElementById('parametersContent');
        
        if (!node) {
            container.innerHTML = '<div class="epr-parameters-empty-state"><i class="bi bi-cursor"></i><p>Click a node on the canvas to edit its properties</p></div>';
            return;
        }

        let html = `<h6 class="mb-3">${this.escapeHtml(node.name)}</h6>`;

        // Common parameters based on node type
        const commonParams = this.getCommonParameters(node.type);
        
        commonParams.forEach(param => {
            const value = node.parameters?.[param.key] || '';
            html += this.renderParameterField(param, value);
        });

        // Type-specific parameters
        const typeParams = this.getTypeSpecificParameters(node.type, node.parameters || {});
        typeParams.forEach(param => {
            html += this.renderParameterField(param, param.value);
        });

        container.innerHTML = html;
    }

    getCommonParameters(nodeType) {
        const common = [
            { key: 'name', label: 'Name', type: 'text' },
            { key: 'description', label: 'Description', type: 'textarea' }
        ];

        // Add type-specific common params
        if (nodeType === 'packaging' || nodeType === 'product') {
            common.push(
                { key: 'height', label: 'Height', type: 'number' },
                { key: 'weight', label: 'Weight', type: 'number' },
                { key: 'depth', label: 'Depth', type: 'number' }
            );
        }

        if (nodeType === 'product') {
            common.push(
                { key: 'sku', label: 'SKU', type: 'text' },
                { key: 'quantity', label: 'Quantity', type: 'number' }
            );
        }

        if (nodeType === 'distribution') {
            common.push(
                { key: 'country', label: 'Country', type: 'text' },
                { key: 'stateProvince', label: 'State/Province', type: 'text' },
                { key: 'county', label: 'County', type: 'text' }
            );
        }

        return common;
    }

    getTypeSpecificParameters(nodeType, parameters) {
        const params = [];

        // Add any additional parameters from the node
        Object.keys(parameters).forEach(key => {
            if (!['name', 'description', 'height', 'weight', 'depth', 'sku', 'quantity', 
                  'country', 'stateProvince', 'county'].includes(key)) {
                params.push({
                    key: key,
                    label: this.formatLabel(key),
                    type: typeof parameters[key] === 'number' ? 'number' : 'text',
                    value: parameters[key]
                });
            }
        });

        return params;
    }

    renderParameterField(param, value) {
        const inputId = `param-${param.key}`;
        
        if (param.type === 'textarea') {
            return `
                <div class="parameter-group">
                    <label for="${inputId}">${param.label}</label>
                    <textarea 
                        id="${inputId}" 
                        class="form-control parameter-input" 
                        data-param-key="${param.key}"
                        rows="3">${this.escapeHtml(value)}</textarea>
                </div>
            `;
        } else if (param.type === 'number') {
            return `
                <div class="parameter-group">
                    <label for="${inputId}">${param.label}</label>
                    <input 
                        type="number" 
                        id="${inputId}" 
                        class="form-control parameter-input" 
                        data-param-key="${param.key}"
                        value="${value}"
                        step="0.01" />
                </div>
            `;
        } else {
            return `
                <div class="parameter-group">
                    <label for="${inputId}">${param.label}</label>
                    <input 
                        type="text" 
                        id="${inputId}" 
                        class="form-control parameter-input" 
                        data-param-key="${param.key}"
                        value="${this.escapeHtml(value)}" />
                </div>
            `;
        }
    }

    saveParameter(key, value) {
        if (!this.currentNode) return;

        // Update node parameters
        if (!this.currentNode.parameters) {
            this.currentNode.parameters = {};
        }

        // Convert value based on type
        if (key === 'height' || key === 'weight' || key === 'depth' || key === 'quantity') {
            this.currentNode.parameters[key] = value ? parseFloat(value) : null;
        } else {
            this.currentNode.parameters[key] = value;
        }

        // Update node name if name parameter changed
        if (key === 'name' && value) {
            this.currentNode.name = value;
            this.canvasManager.renderNode(this.currentNode);
        }

        // Save state
        this.canvasManager.saveState();
    }

    clearParameters() {
        this.currentNode = null;
        const container = document.getElementById('parametersContent');
        container.innerHTML = '<div class="epr-parameters-empty-state"><i class="bi bi-cursor"></i><p>Click a node on the canvas to edit its properties</p></div>';
    }

    formatLabel(key) {
        // Convert camelCase to Title Case
        return key
            .replace(/([A-Z])/g, ' $1')
            .replace(/^./, str => str.toUpperCase())
            .trim();
    }

    escapeHtml(text) {
        if (text == null) return '';
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }
}

window.ParametersPanel = ParametersPanel;

