// Types Toolbar Module - Handles Raw Materials, Packaging, Products, Distribution
console.log('[Visual Editor] types-toolbar.js loading...');

class TypesToolbar {
    constructor(canvasManager) {
        this.canvasManager = canvasManager;
        this.rawMaterials = [];
        this.packagingTypes = [];
        this.products = [];
        this.geographies = [];
        this.jurisdictions = [];
        
        this.init();
    }

    async init() {
        await this.loadData();
        this.setupEventListeners();
        this.renderRawMaterials();
        this.renderPackagingDropdown();
        this.renderProductDropdown();
        this.renderDistributionDropdowns();
        this.setupPackagingDragAndDrop();
        this.setupProductDragAndDrop();
    }

    async loadData() {
        try {
            // Load raw materials
            const materialsRes = await fetch('/api/visual-editor/raw-materials');
            if (materialsRes.ok) {
                this.rawMaterials = await materialsRes.json();
            } else {
                console.warn('Failed to load raw materials');
                this.rawMaterials = [];
            }

            // Load packaging types
            const packagingRes = await fetch('/api/visual-editor/packaging-types');
            if (packagingRes.ok) {
                this.packagingTypes = await packagingRes.json();
            } else {
                console.warn('Failed to load packaging types');
                this.packagingTypes = [];
            }

            // Load products
            const productsRes = await fetch('/api/visual-editor/products');
            if (productsRes.ok) {
                this.products = await productsRes.json();
            } else {
                console.warn('Failed to load products');
                this.products = [];
            }

            // Load geographies
            const geographiesRes = await fetch('/api/visual-editor/geographies');
            if (geographiesRes.ok) {
                this.geographies = await geographiesRes.json();
            } else {
                console.warn('Failed to load geographies');
                this.geographies = [];
            }

            // Load jurisdictions
            const jurisdictionsRes = await fetch('/api/visual-editor/jurisdictions');
            if (jurisdictionsRes.ok) {
                this.jurisdictions = await jurisdictionsRes.json();
            } else {
                console.warn('Failed to load jurisdictions');
                this.jurisdictions = [];
            }
        } catch (error) {
            console.error('Error loading data:', error);
            // Set empty arrays as fallback
            this.rawMaterials = [];
            this.packagingTypes = [];
            this.products = [];
            this.geographies = [];
            this.jurisdictions = [];
        }
    }

    setupEventListeners() {
        // Packaging buttons
        const addPackagingBtn = document.getElementById('addPackagingBtn');
        if (addPackagingBtn) {
            addPackagingBtn.addEventListener('click', () => {
                this.addSelectedPackaging();
            });
        }

        const newPackagingBtn = document.getElementById('newPackagingBtn');
        if (newPackagingBtn) {
            newPackagingBtn.addEventListener('click', () => {
                this.showNewPackagingModal();
            });
        }

        // Product buttons
        const addProductBtn = document.getElementById('addProductBtn');
        if (addProductBtn) {
            addProductBtn.addEventListener('click', () => {
                this.addSelectedProduct();
            });
        }

        const newProductBtn = document.getElementById('newProductBtn');
        if (newProductBtn) {
            newProductBtn.addEventListener('click', () => {
                this.showNewProductModal();
            });
        }

        // Distribution buttons
        const addDistributionBtn = document.getElementById('addDistributionBtn');
        if (addDistributionBtn) {
            addDistributionBtn.addEventListener('click', () => {
                this.addSelectedDistribution();
            });
        }

        const newLocationBtn = document.getElementById('newLocationBtn');
        if (newLocationBtn) {
            newLocationBtn.addEventListener('click', () => {
                this.showNewLocationModal();
            });
        }

        // Country dropdown change
        const countryDropdown = document.getElementById('countryDropdown');
        if (countryDropdown) {
            countryDropdown.addEventListener('change', (e) => {
                this.onCountryChange(e.target.value);
            });
        }

        // Save new packaging
        const saveNewPackagingBtn = document.getElementById('saveNewPackagingBtn');
        if (saveNewPackagingBtn) {
            saveNewPackagingBtn.addEventListener('click', () => {
                this.saveNewPackaging();
            });
        }

        // Save new product
        const saveNewProductBtn = document.getElementById('saveNewProductBtn');
        if (saveNewProductBtn) {
            saveNewProductBtn.addEventListener('click', () => {
                this.saveNewProduct();
            });
        }

        // Save new location
        const saveNewLocationBtn = document.getElementById('saveNewLocationBtn');
        if (saveNewLocationBtn) {
            saveNewLocationBtn.addEventListener('click', () => {
                this.saveNewLocation();
            });
        }
    }

    renderRawMaterials() {
        const container = document.getElementById('rawMaterialsButtons');
        container.innerHTML = '';

        // Common raw material types with icons
        const materialTypes = [
            { name: 'Glass', icon: 'bi-circle', class: 'material-icon-glass' },
            { name: 'Cardboard', icon: 'bi-box', class: 'material-icon-cardboard' },
            { name: 'Plastic', icon: 'bi-circle-fill', class: 'material-icon-plastic' },
            { name: 'Foil', icon: 'bi-square', class: 'material-icon-foil' },
            { name: 'Plastic Wrap', icon: 'bi-rectangle', class: 'material-icon-plastic-wrap' },
            { name: 'Sellotape', icon: 'bi-dash', class: 'material-icon-sellotape' }
        ];

        materialTypes.forEach(material => {
            const btn = document.createElement('button');
            btn.className = `material-btn ${material.class}`;
            btn.draggable = true;
            btn.innerHTML = `
                <i class="bi ${material.icon}"></i>
                <span>${material.name}</span>
            `;
            
            // Click handler
            btn.addEventListener('click', () => {
                this.addRawMaterial(material.name, material.icon);
            });

            // Drag handlers
            btn.addEventListener('dragstart', (e) => {
                const nodeData = {
                    type: 'raw-material',
                    name: material.name,
                    icon: material.icon
                };
                e.dataTransfer.setData('application/json', JSON.stringify(nodeData));
                e.dataTransfer.effectAllowed = 'copy';
                btn.classList.add('dragging');
            });

            btn.addEventListener('dragend', () => {
                btn.classList.remove('dragging');
            });

            container.appendChild(btn);
        });
    }

    addRawMaterial(name, icon) {
        const node = this.canvasManager.addNode({
            type: 'raw-material',
            name: name,
            icon: icon,
            x: Math.random() * 400 + 100,
            y: Math.random() * 400 + 100
        });

        // Auto-select the new node
        this.canvasManager.selectNode(node.id);
    }

    renderPackagingDropdown() {
        const dropdown = document.getElementById('packagingLibraryDropdown');
        dropdown.innerHTML = '<option value="">Select packaging...</option>';

        this.packagingTypes.forEach(packaging => {
            const option = document.createElement('option');
            option.value = packaging.id;
            option.textContent = packaging.name;
            dropdown.appendChild(option);
        });
    }

    async addSelectedPackaging() {
        const dropdown = document.getElementById('packagingLibraryDropdown');
        const packagingId = parseInt(dropdown.value);

        if (!packagingId) {
            alert('Please select a packaging item');
            return;
        }

        try {
            const res = await fetch(`/api/visual-editor/packaging-type/${packagingId}`);
            const packaging = await res.json();

            const node = this.canvasManager.addNode({
                type: 'packaging',
                entityId: packaging.id,
                name: packaging.name,
                icon: 'bi-box-seam',
                x: Math.random() * 400 + 100,
                y: Math.random() * 400 + 100,
                parameters: {
                    height: packaging.height,
                    weight: packaging.weight,
                    depth: packaging.depth,
                    volume: packaging.volume,
                    description: packaging.description
                }
            });

            // Load associated raw materials
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

                    // Connect raw material to packaging
                    this.canvasManager.addConnection(materialNode.id, node.id);
                });
            }

            this.canvasManager.selectNode(node.id);
        } catch (error) {
            console.error('Error adding packaging:', error);
            alert('Error loading packaging item');
        }
    }

    setupPackagingDragAndDrop() {
        const dropdown = document.getElementById('packagingLibraryDropdown');
        if (!dropdown) return;

        // Make dropdown items draggable
        dropdown.addEventListener('change', () => {
            const selectedOption = dropdown.options[dropdown.selectedIndex];
            if (selectedOption.value) {
                // Store selected packaging ID for drag
                selectedOption.dataset.packagingId = selectedOption.value;
            }
        });

        // Add drag handler to dropdown container
        const dropdownContainer = dropdown.closest('.toolbar-controls');
        if (dropdownContainer) {
            dropdown.addEventListener('mousedown', (e) => {
                if (dropdown.value) {
                    dropdown.draggable = true;
                } else {
                    dropdown.draggable = false;
                }
            });

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
                    console.error('Error loading packaging:', error);
                    e.preventDefault();
                }
            });
        }
    }

    showNewPackagingModal() {
        const modal = new bootstrap.Modal(document.getElementById('newPackagingModal'));
        document.getElementById('newPackagingName').value = '';
        document.getElementById('newPackagingDescription').value = '';
        modal.show();
    }

    async saveNewPackaging() {
        const name = document.getElementById('newPackagingName').value.trim();
        const description = document.getElementById('newPackagingDescription').value.trim();

        if (!name) {
            alert('Please enter a name');
            return;
        }

        try {
            // Create new packaging via API
            const response = await fetch('/PackagingTypes/Create', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    name: name,
                    description: description,
                    isUserCreated: true
                })
            });

            if (response.ok) {
                // Reload packaging list
                await this.loadData();
                this.renderPackagingDropdown();

                // Add to canvas
                const node = this.canvasManager.addNode({
                    type: 'packaging',
                    name: name,
                    icon: 'bi-box-seam',
                    x: Math.random() * 400 + 100,
                    y: Math.random() * 400 + 100,
                    parameters: {
                        description: description
                    }
                });

                this.canvasManager.selectNode(node.id);

                // Close modal
                const modal = bootstrap.Modal.getInstance(document.getElementById('newPackagingModal'));
                modal.hide();
            } else {
                alert('Error creating packaging item');
            }
        } catch (error) {
            console.error('Error saving packaging:', error);
            alert('Error creating packaging item');
        }
    }

    renderProductDropdown() {
        const dropdown = document.getElementById('productLibraryDropdown');
        dropdown.innerHTML = '<option value="">Select product...</option>';

        this.products.forEach(product => {
            const option = document.createElement('option');
            option.value = product.id;
            option.textContent = `${product.name} (${product.sku})`;
            dropdown.appendChild(option);
        });
    }

    setupProductDragAndDrop() {
        const dropdown = document.getElementById('productLibraryDropdown');
        if (!dropdown) return;

        dropdown.addEventListener('dragstart', async (e) => {
            const productId = parseInt(dropdown.value);
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
                console.error('Error loading product:', error);
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

    async addSelectedProduct() {
        const dropdown = document.getElementById('productLibraryDropdown');
        const productId = parseInt(dropdown.value);

        if (!productId) {
            alert('Please select a product');
            return;
        }

        try {
            const res = await fetch(`/api/visual-editor/product/${productId}`);
            const product = await res.json();

            const node = this.canvasManager.addNode({
                type: 'product',
                entityId: product.id,
                name: product.name,
                icon: 'bi-bag',
                x: Math.random() * 400 + 100,
                y: Math.random() * 400 + 100,
                parameters: {
                    sku: product.sku,
                    description: product.description,
                    size: product.size,
                    weight: product.weight,
                    height: product.height,
                    quantity: product.quantity
                }
            });

            // Load associated packaging and raw materials
            if (product.packagingUnits && product.packagingUnits.length > 0) {
                product.packagingUnits.forEach((packagingUnit, unitIndex) => {
                    packagingUnit.items.forEach((item, itemIndex) => {
                        const packagingNode = this.canvasManager.addNode({
                            type: 'packaging',
                            entityId: item.id,
                            name: item.name,
                            icon: 'bi-box-seam',
                            x: node.x - 200,
                            y: node.y + (unitIndex * 150) + (itemIndex * 100),
                        });

                        // Connect packaging to product
                        this.canvasManager.addConnection(packagingNode.id, node.id);

                        // Load raw materials for this packaging
                        if (item.materials && item.materials.length > 0) {
                            item.materials.forEach((material, matIndex) => {
                                const materialNode = this.canvasManager.addNode({
                                    type: 'raw-material',
                                    entityId: material.id,
                                    name: material.name,
                                    icon: 'bi-circle',
                                    x: packagingNode.x - 200,
                                    y: packagingNode.y + (matIndex * 80),
                                });

                                // Connect raw material to packaging
                                this.canvasManager.addConnection(materialNode.id, packagingNode.id);
                            });
                        }
                    });
                });
            }

            this.canvasManager.selectNode(node.id);
        } catch (error) {
            console.error('Error adding product:', error);
            alert('Error loading product');
        }
    }

    showNewProductModal() {
        const modal = new bootstrap.Modal(document.getElementById('newProductModal'));
        document.getElementById('newProductSku').value = '';
        document.getElementById('newProductName').value = '';
        document.getElementById('newProductDescription').value = '';
        modal.show();
    }

    async saveNewProduct() {
        const sku = document.getElementById('newProductSku').value.trim();
        const name = document.getElementById('newProductName').value.trim();
        const description = document.getElementById('newProductDescription').value.trim();

        if (!sku || !name) {
            alert('Please enter SKU and name');
            return;
        }

        try {
            // Create new product via API
            const response = await fetch('/Products/Create', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    sku: sku,
                    name: name,
                    description: description
                })
            });

            if (response.ok) {
                // Reload products list
                await this.loadData();
                this.renderProductDropdown();

                // Add to canvas
                const node = this.canvasManager.addNode({
                    type: 'product',
                    name: name,
                    icon: 'bi-bag',
                    x: Math.random() * 400 + 100,
                    y: Math.random() * 400 + 100,
                    parameters: {
                        sku: sku,
                        description: description
                    }
                });

                this.canvasManager.selectNode(node.id);

                // Close modal
                const modal = bootstrap.Modal.getInstance(document.getElementById('newProductModal'));
                modal.hide();
            } else {
                alert('Error creating product');
            }
        } catch (error) {
            console.error('Error saving product:', error);
            alert('Error creating product');
        }
    }

    renderDistributionDropdowns() {
        const countryDropdown = document.getElementById('countryDropdown');
        countryDropdown.innerHTML = '<option value="">Select country...</option>';

        // Group geographies by jurisdiction/country
        const countries = this.geographies.filter(g => !g.parentId);
        countries.forEach(geo => {
            const option = document.createElement('option');
            option.value = geo.id;
            option.textContent = geo.name;
            countryDropdown.appendChild(option);
        });
    }

    onCountryChange(countryId) {
        const regionDropdown = document.getElementById('regionDropdown');
        
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
        const countryDropdown = document.getElementById('countryDropdown');
        const regionDropdown = document.getElementById('regionDropdown');
        
        const countryId = countryDropdown.value;
        const regionId = regionDropdown.value || countryId;

        if (!countryId) {
            alert('Please select a location');
            return;
        }

        const selectedGeo = this.geographies.find(g => g.id == parseInt(regionId));
        const locationName = selectedGeo ? selectedGeo.name : 'Unknown Location';

        const node = this.canvasManager.addNode({
            type: 'distribution',
            name: locationName,
            icon: 'bi-geo-alt',
            x: Math.random() * 400 + 100,
            y: Math.random() * 400 + 100,
            parameters: {
                geographyId: parseInt(regionId),
                country: countryDropdown.options[countryDropdown.selectedIndex].text
            }
        });

        this.canvasManager.selectNode(node.id);
    }

    showNewLocationModal() {
        const modal = new bootstrap.Modal(document.getElementById('newLocationModal'));
        document.getElementById('newLocationCountry').value = '';
        document.getElementById('newLocationState').value = '';
        document.getElementById('newLocationCounty').value = '';
        modal.show();
    }

    async saveNewLocation() {
        const country = document.getElementById('newLocationCountry').value.trim();
        const state = document.getElementById('newLocationState').value.trim();
        const county = document.getElementById('newLocationCounty').value.trim();

        if (!country) {
            alert('Please enter a country');
            return;
        }

        try {
            // Create new location (simplified - you may want to create Geography entity)
            const locationName = [county, state, country].filter(Boolean).join(', ') || country;

            const node = this.canvasManager.addNode({
                type: 'distribution',
                name: locationName,
                icon: 'bi-geo-alt',
                x: Math.random() * 400 + 100,
                y: Math.random() * 400 + 100,
                parameters: {
                    country: country,
                    stateProvince: state,
                    county: county
                }
            });

            this.canvasManager.selectNode(node.id);

            // Close modal
            const modal = bootstrap.Modal.getInstance(document.getElementById('newLocationModal'));
            modal.hide();
        } catch (error) {
            console.error('Error saving location:', error);
            alert('Error creating location');
        }
    }
}

window.TypesToolbar = TypesToolbar;

