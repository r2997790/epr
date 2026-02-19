// ASN Visual View - Packaging flow visualization using Visual Editor structure
// Matches the example: Raw Materials → Packaging → Packaging Groups → Products → Packaging Groups → Distribution Groups
(function() {
    'use strict';

    let visualSvg = null;
    let visualNodesLayer = null;
    let visualConnectionsLayer = null;
    let visualCanvas = null;
    let visualNodes = [];
    let visualConnections = [];
    let currentShipment = null;
    let productPackagingData = null;

    // Initialize visual view when tab is shown
    document.addEventListener('DOMContentLoaded', function() {
        // Button handlers
        const btnZoomFit = document.getElementById('btnZoomFit');
        if (btnZoomFit) {
            btnZoomFit.addEventListener('click', fitToView);
        }

        const btnResetView = document.getElementById('btnResetView');
        if (btnResetView) {
            btnResetView.addEventListener('click', resetView);
        }
    });

    function initializeVisualView() {
        visualCanvas = document.getElementById('visualViewCanvas');
        visualSvg = document.getElementById('visualViewSvg');
        visualNodesLayer = document.getElementById('visualViewNodesLayer');
        visualConnectionsLayer = document.getElementById('visualViewConnectionsLayer');

        if (!visualCanvas || !visualSvg || !visualNodesLayer || !visualConnectionsLayer) {
            console.error('[Visual View] Elements not found - canvas:', !!visualCanvas, 'svg:', !!visualSvg, 'nodesLayer:', !!visualNodesLayer, 'connectionsLayer:', !!visualConnectionsLayer);
            return false;
        }

        // Clear existing content
        visualNodes = [];
        visualConnections = [];

        // Clear nodes layer
        visualNodesLayer.innerHTML = '';

        // Clear connections SVG except defs
        const defs = visualSvg.querySelector('defs');
        visualSvg.innerHTML = '';
        if (defs) {
            visualSvg.appendChild(defs);
        } else {
            // Create defs if missing
            const newDefs = document.createElementNS('http://www.w3.org/2000/svg', 'defs');
            const marker = document.createElementNS('http://www.w3.org/2000/svg', 'marker');
            marker.setAttribute('id', 'visualArrow');
            marker.setAttribute('markerWidth', '10');
            marker.setAttribute('markerHeight', '10');
            marker.setAttribute('refX', '9');
            marker.setAttribute('refY', '3');
            marker.setAttribute('orient', 'auto');
            marker.setAttribute('markerUnits', 'strokeWidth');
            const path = document.createElementNS('http://www.w3.org/2000/svg', 'path');
            path.setAttribute('d', 'M0,0 L0,6 L9,3 z');
            path.setAttribute('fill', '#6c757d');
            marker.appendChild(path);
            newDefs.appendChild(marker);
            visualSvg.appendChild(newDefs);
        }
        return true;
    }

    async function loadProductPackagingData(asnId) {
        try {
            const response = await fetch(`/Distribution/GetAsnProductPackaging?asnId=${asnId}`);
            const result = await response.json();
            
            if (result.success) {
                productPackagingData = result.data;
            } else {
                console.warn('Failed to load product packaging data:', result.message);
                productPackagingData = [];
            }
        } catch (error) {
            console.error('Error loading product packaging data:', error);
            productPackagingData = [];
        }
    }

    function getTypeLabel(type) {
        const labels = {
            'raw-material': 'Raw Material',
            'packaging': 'Packaging',
            'product': 'Product',
            'distribution': 'Distribution',
            'packaging-group': 'Packaging Group',
            'distribution-group': 'Distribution Group'
        };
        return labels[type] || type;
    }

    function getIconForType(type) {
        const icons = {
            'raw-material': 'bi-circle-fill',
            'packaging': 'bi-box-seam',
            'product': 'bi-box',
            'distribution': 'bi-building',
            'packaging-group': 'bi-folder',
            'distribution-group': 'bi-diagram-3'
        };
        return icons[type] || 'bi-circle';
    }

    function escapeHtml(text) {
        if (text === null || text === undefined) return '';
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    function renderNode(node) {
        const nodeEl = document.createElement('div');
        nodeEl.className = 'epr-canvas-node';
        
        if (node.type === 'packaging-group') {
            nodeEl.classList.add('epr-packaging-group-node');
        }
        if (node.type === 'distribution-group') {
            nodeEl.classList.add('epr-distribution-group-node');
        }
        
        nodeEl.setAttribute('data-node-id', node.id);
        nodeEl.setAttribute('data-type', node.type);
        nodeEl.style.position = 'absolute';
        nodeEl.style.left = `${node.x}px`;
        nodeEl.style.top = `${node.y}px`;
        nodeEl.style.pointerEvents = 'all';
        nodeEl.style.zIndex = '10';

        // Determine connection ports
        let leftPort = '';
        let rightPort = '';
        
        if (node.type !== 'raw-material') {
            leftPort = `<div class="epr-connection-port epr-port-left" data-node-id="${node.id}" data-port="left"></div>`;
        }
        if (node.type !== 'distribution' && node.type !== 'distribution-group') {
            rightPort = `<div class="epr-connection-port epr-port-right" data-node-id="${node.id}" data-port="right"></div>`;
        }

        // Build group content for packaging groups
        let groupContent = '';
        if (node.type === 'packaging-group' && node.containedItems && node.containedItems.length > 0) {
            const packagingItems = node.containedItems.filter(item => item.type === 'packaging');
            const rawMaterials = node.containedItems.filter(item => item.type === 'raw-material');
            
            let itemsList = '';
            if (node.containedItems.length > 0) {
                itemsList = '<div class="epr-group-items-list" style="margin-top: 0.5rem; padding-top: 0.5rem; border-top: 1px solid #dee2e6;">';
                
                // Show packaging items with nested raw materials (matching Visual Editor structure)
                packagingItems.forEach(packagingItem => {
                    itemsList += `<div class="epr-group-item epr-group-packaging-item" data-item-id="${packagingItem.id}" style="font-size: 0.7rem; padding: 0.25rem 0.5rem; margin: 0.25rem 0; background: white; border-radius: 3px; border-left: 3px solid #007bff; cursor: pointer;">
                        📦 ${escapeHtml(packagingItem.name)}
                    </div>`;
                    
                    // Show nested raw materials (indented under packaging)
                    const nestedRawMaterials = rawMaterials.filter(raw => raw.parentItemId === packagingItem.id);
                    nestedRawMaterials.forEach(rawMat => {
                        itemsList += `<div class="epr-group-item epr-group-raw-material-item" data-item-id="${rawMat.id}" style="font-size: 0.65rem; padding: 0.2rem 0.5rem 0.2rem 1.5rem; margin: 0.1rem 0; background: #f8f9fa; border-radius: 3px; border-left: 3px solid #6c757d; cursor: pointer;">
                            🔵 ${escapeHtml(rawMat.name)}
                        </div>`;
                    });
                });
                
                // Show orphan raw materials (not nested under any packaging)
                const orphanRawMaterials = rawMaterials.filter(raw => !raw.parentItemId);
                orphanRawMaterials.forEach(rawMat => {
                    itemsList += `<div class="epr-group-item epr-group-raw-material-item" data-item-id="${rawMat.id}" style="font-size: 0.7rem; padding: 0.25rem 0.5rem; margin: 0.25rem 0; background: white; border-radius: 3px; border-left: 3px solid #6c757d; cursor: pointer;">
                        🔵 ${escapeHtml(rawMat.name)}
                    </div>`;
                });
                
                itemsList += '</div>';
            }
            
            const packagingCount = packagingItems.length;
            const rawMaterialsCount = rawMaterials.length;
            groupContent = `<div class="epr-group-content" style="font-size: 0.75rem; color: #495057; margin-top: 0.5rem;">
                <div style="font-weight: 600; margin-bottom: 0.25rem; padding: 0.5rem; background: #f8f9fa; border-radius: 4px;">
                    <div>Packaging Items: ${packagingCount}</div>
                    <div>Raw Materials: ${rawMaterialsCount}</div>
                </div>
                ${itemsList}
            </div>`;
        }

        // Build group content for distribution groups
        if (node.type === 'distribution-group' && node.containedItems && node.containedItems.length > 0) {
            const distributionItems = node.containedItems.filter(item => item.type === 'distribution');
            
            let itemsList = '';
            if (distributionItems.length > 0) {
                itemsList = '<div class="epr-group-items-list" style="margin-top: 0.5rem; padding-top: 0.5rem; border-top: 1px solid #dee2e6;">';
                distributionItems.forEach(distItem => {
                    itemsList += `<div class="epr-group-item epr-group-distribution-item" data-item-id="${distItem.id}" style="font-size: 0.7rem; padding: 0.25rem 0.5rem; margin: 0.25rem 0; background: white; border-radius: 3px; border-left: 3px solid #6f42c1; cursor: pointer;">
                        🏢 ${escapeHtml(distItem.name)}
                    </div>`;
                });
                itemsList += '</div>';
            }
            
            groupContent = `<div class="epr-group-content" style="font-size: 0.75rem; color: #495057; margin-top: 0.5rem;">
                <div style="font-weight: 600; margin-bottom: 0.25rem; padding: 0.5rem; background: #f8f9fa; border-radius: 4px;">
                    <div>Distribution Nodes: ${distributionItems.length}</div>
                </div>
                ${itemsList}
            </div>`;
        }

        // Build node HTML
        const icon = getIconForType(node.type);
        const badge = node.badge ? `<span style="position: absolute; top: -8px; left: -8px; background: ${node.badgeColor || '#007bff'}; color: white; padding: 2px 6px; border-radius: 3px; font-size: 0.65rem; font-weight: 600;">${escapeHtml(node.badge)}</span>` : '';
        
        nodeEl.innerHTML = `
            <div class="epr-node-header" style="position: relative;">
                ${badge}
                <div style="display: flex; align-items: center;">
                    <i class="bi ${icon} epr-node-icon"></i>
                    <div style="display: flex; flex-direction: column; flex: 1;">
                        <span class="epr-node-title">${escapeHtml(node.name)}</span>
                    </div>
                </div>
            </div>
            <div class="epr-node-type">${getTypeLabel(node.type)}</div>
            ${node.description ? `<div style="font-size: 0.75rem; color: #6c757d; margin-top: 0.25rem;">${escapeHtml(node.description)}</div>` : ''}
            ${groupContent}
            ${leftPort}
            ${rightPort}
        `;

        visualNodesLayer.appendChild(nodeEl);
    }

    function renderConnections() {
        console.log('[Visual View] Rendering connections:', visualConnections.length);
        
        // Ensure SVG is sized correctly (use fallback when tab just shown and layout not yet complete)
        const canvasRect = visualCanvas.getBoundingClientRect();
        const w = canvasRect.width > 0 ? canvasRect.width : (visualCanvas.clientWidth || 800);
        const h = canvasRect.height > 0 ? canvasRect.height : (visualCanvas.clientHeight || 700);
        visualSvg.setAttribute('width', w);
        visualSvg.setAttribute('height', h);

        visualConnections.forEach((conn, index) => {
            const fromNode = visualNodes.find(n => n.id === conn.from);
            const toNode = visualNodes.find(n => n.id === conn.to);

            if (!fromNode || !toNode) {
                console.warn(`[Visual View] Connection ${index}: Missing node (from: ${conn.from}, to: ${conn.to})`);
                return;
            }

            // Get node element positions
            const fromEl = visualNodesLayer.querySelector(`[data-node-id="${conn.from}"]`);
            const toEl = visualNodesLayer.querySelector(`[data-node-id="${conn.to}"]`);
            
            if (!fromEl || !toEl) {
                console.warn(`[Visual View] Connection ${index}: Missing element (from: ${conn.from}, to: ${conn.to})`);
                return;
            }

            const fromX = fromNode.x + (fromEl.offsetWidth || 150);
            const fromY = fromNode.y + (fromEl.offsetHeight || 100) / 2;
            const toX = toNode.x;
            const toY = toNode.y + (toEl.offsetHeight || 100) / 2;

            const line = document.createElementNS('http://www.w3.org/2000/svg', 'line');
            line.setAttribute('x1', fromX);
            line.setAttribute('y1', fromY);
            line.setAttribute('x2', toX);
            line.setAttribute('y2', toY);
            line.setAttribute('class', 'epr-connection-line');
            line.setAttribute('stroke', '#6c757d');
            line.setAttribute('stroke-width', '2');
            line.setAttribute('marker-end', 'url(#visualArrow)');
            visualSvg.appendChild(line);

            // Add quantity label if present
            if (conn.quantity) {
                const midX = (fromX + toX) / 2;
                const midY = (fromY + toY) / 2;
                
                const labelBg = document.createElementNS('http://www.w3.org/2000/svg', 'circle');
                labelBg.setAttribute('cx', midX);
                labelBg.setAttribute('cy', midY);
                labelBg.setAttribute('r', '12');
                labelBg.setAttribute('fill', '#007bff');
                visualSvg.appendChild(labelBg);
                
                const labelText = document.createElementNS('http://www.w3.org/2000/svg', 'text');
                labelText.setAttribute('x', midX);
                labelText.setAttribute('y', midY + 4);
                labelText.setAttribute('text-anchor', 'middle');
                labelText.setAttribute('fill', 'white');
                labelText.setAttribute('font-size', '10');
                labelText.setAttribute('font-weight', 'bold');
                labelText.textContent = conn.quantity.toString();
                visualSvg.appendChild(labelText);
            }
        });
        
        console.log('[Visual View] Connections rendered');
    }

    function renderVisualView(shipment) {
        if (!visualSvg || !visualNodesLayer) {
            if (!initializeVisualView()) return;
        }

        currentShipment = shipment;
        visualNodes = [];
        visualConnections = [];

        const warnings = [];
        let xPos = 100;
        const yStart = 150;
        const xSpacing = 280;
        const ySpacing = 180;

        // Collect all unique raw materials and packaging types from product packaging data
        const rawMaterialMap = new Map(); // id -> {id, name, description}
        const packagingTypeMap = new Map(); // id -> {id, name, description, rawMaterials: []}
        const packagingGroups = [];
        const productNodes = [];

        if (productPackagingData && productPackagingData.length > 0) {
            productPackagingData.forEach((productData, productIndex) => {
                if (productData.packagingFlow) {
                    // Collect raw materials
                    if (productData.packagingFlow.rawMaterials) {
                        productData.packagingFlow.rawMaterials.forEach(rm => {
                            if (!rawMaterialMap.has(rm.id)) {
                                rawMaterialMap.set(rm.id, {
                                    id: rm.id,
                                    name: rm.name,
                                    description: rm.description
                                });
                            }
                        });
                    }

                    // Process packaging units (Primary, Secondary, Tertiary)
                    if (productData.packagingFlow.packagingUnits) {
                        productData.packagingFlow.packagingUnits.forEach((packagingUnit, unitIndex) => {
                            const unitLevel = packagingUnit.unitLevel || 'Primary';
                            
                            // For each packaging type in this unit
                            if (packagingUnit.packagingTypes) {
                                packagingUnit.packagingTypes.forEach((packagingType, ptIndex) => {
                                    const ptId = `pt-${productIndex}-${unitIndex}-${ptIndex}`;
                                    
                                    // Store packaging type with its raw materials
                                    if (!packagingTypeMap.has(ptId)) {
                                        packagingTypeMap.set(ptId, {
                                            id: ptId,
                                            name: packagingType.name,
                                            description: packagingType.description,
                                            unitLevel: unitLevel,
                                            quantity: packagingType.quantity || 1,
                                            rawMaterials: packagingType.rawMaterials || []
                                        });
                                    }
                                });
                            }
                        });
                    }
                }
            });
        }

        // Layer 1: Raw Material nodes (individual nodes, leftmost)
        const rawMaterialNodes = [];
        let rawYPos = yStart;
        rawMaterialMap.forEach((rm, rmId) => {
            rawMaterialNodes.push({
                id: `raw-${rmId}`,
                type: 'raw-material',
                name: rm.name,
                description: rm.description,
                x: xPos,
                y: rawYPos,
                rawMaterialId: rmId
            });
            rawYPos += ySpacing;
        });

        xPos += xSpacing;

        // Layer 2: Packaging nodes (created from raw materials)
        const packagingNodes = [];
        let packagingYPos = yStart;
        packagingTypeMap.forEach((pt, ptId) => {
            // Only show individual packaging nodes for Primary/Secondary level
            if (pt.unitLevel === 'Primary' || pt.unitLevel === 'Secondary') {
                const packagingId = `packaging-${ptId}`;
                packagingNodes.push({
                    id: packagingId,
                    type: 'packaging',
                    name: pt.name,
                    description: pt.description,
                    x: xPos,
                    y: packagingYPos,
                    packagingTypeId: ptId,
                    rawMaterials: pt.rawMaterials
                });

                // Connect raw materials to this packaging
                pt.rawMaterials.forEach(rm => {
                    const rawNodeId = `raw-${rm.id}`;
                    const rawNode = rawMaterialNodes.find(rn => rn.rawMaterialId === rm.id);
                    if (rawNode) {
                        visualConnections.push({
                            from: rawNode.id,
                            to: packagingId,
                            type: 'rawToPackaging'
                        });
                    }
                });

                packagingYPos += ySpacing;
            }
        });

        xPos += xSpacing;

        // Layer 3: Primary Packaging Groups (containing packaging items and nested raw materials)
        // Connect packaging nodes and raw materials to primary packaging groups
        if (productPackagingData && productPackagingData.length > 0) {
            productPackagingData.forEach((productData, productIndex) => {
                if (productData.packagingFlow && productData.packagingFlow.packagingUnits) {
                    productData.packagingFlow.packagingUnits.forEach((packagingUnit, unitIndex) => {
                        const unitLevel = packagingUnit.unitLevel || 'Primary';
                        
                        // Only create primary packaging groups here
                        if (unitLevel === 'Primary') {
                            const groupId = `packaging-group-primary-${productIndex}-${unitIndex}`;
                            const containedItems = [];
                            
                            // Add packaging types as items (reference to packaging nodes)
                            if (packagingUnit.packagingTypes) {
                                packagingUnit.packagingTypes.forEach((packagingType, ptIndex) => {
                                    // Find corresponding packaging node
                                    const packagingNode = packagingNodes.find(pn => 
                                        pn.packagingTypeId === `pt-${productIndex}-${unitIndex}-${ptIndex}`
                                    );
                                    
                                    if (packagingNode) {
                                        const packagingItemId = `packaging-item-${productIndex}-${unitIndex}-${ptIndex}`;
                                        containedItems.push({
                                            id: packagingItemId,
                                            type: 'packaging',
                                            name: packagingType.name,
                                            parentItemId: null,
                                            packagingNodeId: packagingNode.id
                                        });
                                        
                                        // Connect packaging node to this group
                                        visualConnections.push({
                                            from: packagingNode.id,
                                            to: groupId,
                                            type: 'packagingToPackagingGroup'
                                        });
                                    }
                                    
                                    // Add raw materials nested under this packaging
                                    if (packagingType.rawMaterials) {
                                        packagingType.rawMaterials.forEach((rm, rmIndex) => {
                                            const rawId = `raw-nested-${productIndex}-${unitIndex}-${ptIndex}-${rmIndex}`;
                                            containedItems.push({
                                                id: rawId,
                                                type: 'raw-material',
                                                name: rm.name,
                                                parentItemId: packagingItemId || null
                                            });
                                        });
                                    }
                                });
                            }

                            // Also add any standalone raw materials (like Paper Label in the example)
                            // These would be raw materials not associated with a packaging type
                            if (productData.packagingFlow.rawMaterials) {
                                productData.packagingFlow.rawMaterials.forEach((rm, rmIndex) => {
                                    // Check if this raw material is already nested under a packaging item
                                    const alreadyNested = containedItems.some(ci => 
                                        ci.type === 'raw-material' && ci.name === rm.name
                                    );
                                    if (!alreadyNested) {
                                        const rawId = `raw-standalone-${productIndex}-${rmIndex}`;
                                        containedItems.push({
                                            id: rawId,
                                            type: 'raw-material',
                                            name: rm.name,
                                            parentItemId: null
                                        });
                                        
                                        // Connect raw material node to this group
                                        const rawNode = rawMaterialNodes.find(rn => rn.rawMaterialId === rm.id);
                                        if (rawNode) {
                                            visualConnections.push({
                                                from: rawNode.id,
                                                to: groupId,
                                                type: 'rawToPackagingGroup'
                                            });
                                        }
                                    }
                                });
                            }

                            if (containedItems.length > 0) {
                                packagingGroups.push({
                                    id: groupId,
                                    type: 'packaging-group',
                                    name: 'Bag',
                                    badge: 'Primary',
                                    badgeColor: '#007bff',
                                    x: xPos,
                                    y: yStart + (packagingGroups.length * ySpacing),
                                    containedItems: containedItems,
                                    unitLevel: unitLevel,
                                    productIndex: productIndex
                                });
                            }
                        }
                    });
                }
            });
        }

        xPos += xSpacing;

        // Layer 4: Product nodes
        if (shipment.pallets) {
            shipment.pallets.forEach((pallet, palletIndex) => {
                if (pallet.lineItems && pallet.lineItems.length > 0) {
                    pallet.lineItems.forEach((item, itemIndex) => {
                        const nodeId = `product-${palletIndex}-${itemIndex}`;
                        const productData = productPackagingData && productPackagingData.length > 0 
                            ? productPackagingData.find(pd => pd.gtin === item.gtin) 
                            : null;
                        const productDataIndex = productData ? productPackagingData.indexOf(productData) : -1;
                        
                        productNodes.push({
                            id: nodeId,
                            type: 'product',
                            name: item.description || `Product ${item.gtin}`,
                            description: productData?.productDescription || null,
                            gtin: item.gtin,
                            quantity: item.quantity,
                            x: xPos,
                            y: yStart + (productNodes.length * ySpacing),
                            productIndex: productDataIndex >= 0 ? productDataIndex : productNodes.length
                        });

                        // Connect primary packaging group to product
                        const primaryGroup = packagingGroups.find(pg => 
                            pg.productIndex === productDataIndex &&
                            pg.unitLevel === 'Primary'
                        );
                        if (primaryGroup) {
                            visualConnections.push({
                                from: primaryGroup.id,
                                to: nodeId,
                                type: 'packagingGroupToProduct'
                            });
                        }
                    });
                }
            });
        }

        xPos += xSpacing;

        // Layer 5: Secondary and Tertiary Packaging Groups
        const higherLevelGroups = [];
        if (productPackagingData && productPackagingData.length > 0) {
            productPackagingData.forEach((productData, productIndex) => {
                if (productData.packagingFlow && productData.packagingFlow.packagingUnits) {
                    productData.packagingFlow.packagingUnits.forEach((packagingUnit, unitIndex) => {
                        const unitLevel = packagingUnit.unitLevel || 'Primary';
                        
                        // Only show Secondary and Tertiary as separate groups
                        if (unitLevel === 'Secondary' || unitLevel === 'Tertiary') {
                            const groupId = `packaging-group-${unitLevel.toLowerCase()}-${productIndex}-${unitIndex}`;
                            const containedItems = [];
                            
                            if (packagingUnit.packagingTypes) {
                                packagingUnit.packagingTypes.forEach((packagingType, ptIndex) => {
                                    const packagingItemId = `packaging-item-${unitLevel.toLowerCase()}-${productIndex}-${unitIndex}-${ptIndex}`;
                                    containedItems.push({
                                        id: packagingItemId,
                                        type: 'packaging',
                                        name: packagingType.name,
                                        parentItemId: null
                                    });
                                    
                                    // Add raw materials nested under this packaging
                                    if (packagingType.rawMaterials) {
                                        packagingType.rawMaterials.forEach((rm, rmIndex) => {
                                            const rawId = `raw-nested-${unitLevel.toLowerCase()}-${productIndex}-${unitIndex}-${ptIndex}-${rmIndex}`;
                                            containedItems.push({
                                                id: rawId,
                                                type: 'raw-material',
                                                name: rm.name,
                                                parentItemId: packagingItemId
                                            });
                                        });
                                    }
                                });
                            }

                            if (containedItems.length > 0) {
                                const groupName = unitLevel === 'Secondary' ? 'Box' : 'Palette';
                                
                                higherLevelGroups.push({
                                    id: groupId,
                                    type: 'packaging-group',
                                    name: groupName,
                                    badge: unitLevel === 'Secondary' ? 'Primary' : 'Secondary',
                                    badgeColor: unitLevel === 'Secondary' ? '#007bff' : '#28a745',
                                    x: xPos,
                                    y: yStart + (higherLevelGroups.length * ySpacing),
                                    containedItems: containedItems,
                                    unitLevel: unitLevel,
                                    productIndex: productIndex
                                });

                                // Connect products to secondary groups, secondary to tertiary
                                if (unitLevel === 'Secondary') {
                                    productNodes.forEach(product => {
                                        if (product.productIndex === productIndex) {
                                            visualConnections.push({
                                                from: product.id,
                                                to: groupId,
                                                type: 'productToPackagingGroup',
                                                quantity: packagingUnit.packagingTypes?.[0]?.quantity || 10
                                            });
                                        }
                                    });
                                } else if (unitLevel === 'Tertiary') {
                                    // Connect secondary groups to tertiary
                                    const secondaryGroup = higherLevelGroups.find(hg => 
                                        hg.unitLevel === 'Secondary' && hg.productIndex === productIndex
                                    );
                                    if (secondaryGroup) {
                                        visualConnections.push({
                                            from: secondaryGroup.id,
                                            to: groupId,
                                            type: 'packagingGroupToPackagingGroup',
                                            quantity: packagingUnit.packagingTypes?.[0]?.quantity || 24
                                        });
                                    }
                                }
                            }
                        }
                    });
                }
            });
        }

        xPos += xSpacing;

        // Layer 6: Distribution Group
        const distributionGroup = {
            id: 'distribution-group-1',
            type: 'distribution-group',
            name: shipment.shipperName || 'Distribution Points',
            x: xPos,
            y: yStart,
            containedItems: []
        };

        // Add distribution points
        if (shipment.pallets) {
            shipment.pallets.forEach((pallet, palletIndex) => {
                if (pallet.destinationName || pallet.destinationGln) {
                    const distId = `distribution-${palletIndex}`;
                    distributionGroup.containedItems.push({
                        id: distId,
                        type: 'distribution',
                        name: pallet.destinationName || 'Unknown Destination',
                        gln: pallet.destinationGln,
                        address: pallet.destinationAddress,
                        city: pallet.destinationCity
                    });
                } else {
                    warnings.push({
                        type: 'distribution',
                        palletNumber: palletIndex + 1,
                        message: `Pallet ${palletIndex + 1} has no destination`
                    });
                }
            });
        }

        // Add shipper and receiver
        if (shipment.shipperName) {
            distributionGroup.containedItems.push({
                id: 'distribution-shipper',
                type: 'distribution',
                name: `Shipper: ${shipment.shipperName}`,
                gln: shipment.shipperGln,
                address: shipment.shipperAddress,
                city: shipment.shipperCity
            });
        }

        if (shipment.receiverName) {
            distributionGroup.containedItems.push({
                id: 'distribution-receiver',
                type: 'distribution',
                name: `Receiver: ${shipment.receiverName}`,
                gln: shipment.receiverGln,
                address: shipment.receiverAddress,
                city: shipment.receiverCity
            });
        }

        // Connect tertiary packaging groups to distribution group
        const tertiaryGroups = higherLevelGroups.filter(hg => hg.unitLevel === 'Tertiary');
        if (tertiaryGroups.length > 0) {
            tertiaryGroups.forEach(tertiaryGroup => {
                visualConnections.push({
                    from: tertiaryGroup.id,
                    to: distributionGroup.id,
                    type: 'packagingGroupToDistribution'
                });
            });
        } else if (higherLevelGroups.length > 0) {
            // If no tertiary groups, connect secondary groups
            higherLevelGroups.filter(hg => hg.unitLevel === 'Secondary').forEach(secondaryGroup => {
                visualConnections.push({
                    from: secondaryGroup.id,
                    to: distributionGroup.id,
                    type: 'packagingGroupToDistribution'
                });
            });
        } else if (productNodes.length > 0) {
            // If no packaging groups, connect products directly to distribution
            productNodes.forEach(product => {
                visualConnections.push({
                    from: product.id,
                    to: distributionGroup.id,
                    type: 'productToDistribution'
                });
            });
        }

        // Ensure we always have at least products and distribution group
        if (productNodes.length === 0 && shipment.pallets) {
            // Create placeholder product nodes from line items
            shipment.pallets.forEach((pallet, palletIndex) => {
                if (pallet.lineItems && pallet.lineItems.length > 0) {
                    pallet.lineItems.forEach((item, itemIndex) => {
                        const nodeId = `product-${palletIndex}-${itemIndex}`;
                        productNodes.push({
                            id: nodeId,
                            type: 'product',
                            name: item.description || `Product ${item.gtin}`,
                            gtin: item.gtin,
                            quantity: item.quantity,
                            x: xPos,
                            y: yStart + (productNodes.length * ySpacing),
                            productIndex: productNodes.length
                        });
                    });
                }
            });
            xPos += xSpacing;
        }

        // Combine all nodes in order
        visualNodes = [
            ...rawMaterialNodes,
            ...packagingNodes,
            ...packagingGroups,
            ...productNodes,
            ...higherLevelGroups,
            distributionGroup
        ];

        console.log('[Visual View] Total nodes to render:', visualNodes.length);
        console.log('[Visual View] Raw materials:', rawMaterialNodes.length);
        console.log('[Visual View] Packaging nodes:', packagingNodes.length);
        console.log('[Visual View] Packaging groups:', packagingGroups.length);
        console.log('[Visual View] Products:', productNodes.length);
        console.log('[Visual View] Higher level groups:', higherLevelGroups.length);
        console.log('[Visual View] Connections:', visualConnections.length);

        // If no nodes at all, show a message
        if (visualNodes.length === 0) {
            displayWarnings([{
                type: 'info',
                message: 'No data available to display. Please ensure the ASN has pallets and line items.'
            }]);
            return;
        }

        // If no packaging data, show info message
        if (packagingGroups.length === 0 && higherLevelGroups.length === 0 && productPackagingData && productPackagingData.length > 0) {
            warnings.push({
                type: 'info',
                message: 'No packaging data found. Products will be displayed without packaging hierarchy.'
            });
        }

        // Render nodes and connections
        try {
            visualNodes.forEach(node => {
                try {
                    renderNode(node);
                } catch (error) {
                    console.error('[Visual View] Error rendering node:', node, error);
                }
            });
            renderConnections();
            displayWarnings(warnings);
            fitToView();
            
            console.log('[Visual View] Rendering complete');
        } catch (error) {
            console.error('[Visual View] Error during rendering:', error);
            displayError('Error rendering nodes: ' + error.message);
        }
    }

    function fitToView() {
        if (visualNodes.length === 0) return;

        // Wait for nodes to be rendered
        setTimeout(() => {
            const nodeElements = Array.from(visualNodesLayer.querySelectorAll('[data-node-id]'));
            if (nodeElements.length === 0) {
                console.warn('[Visual View] No node elements found for fitToView');
                return;
            }

            let minX = Infinity, maxX = -Infinity, minY = Infinity, maxY = -Infinity;

            nodeElements.forEach(el => {
                const rect = el.getBoundingClientRect();
                const canvasRect = visualCanvas.getBoundingClientRect();
                const relativeX = parseInt(el.style.left) || 0;
                const relativeY = parseInt(el.style.top) || 0;
                
                minX = Math.min(minX, relativeX);
                maxX = Math.max(maxX, relativeX + (el.offsetWidth || 200));
                minY = Math.min(minY, relativeY);
                maxY = Math.max(maxY, relativeY + (el.offsetHeight || 150));
            });

            const padding = 50;
            const width = maxX - minX + (padding * 2);
            const height = maxY - minY + (padding * 2);

            // Set canvas size to accommodate all nodes
            visualNodesLayer.style.width = `${width}px`;
            visualNodesLayer.style.height = `${height}px`;
            visualSvg.setAttribute('width', width);
            visualSvg.setAttribute('height', height);

            // Scroll to show all nodes
            visualCanvas.scrollLeft = Math.max(0, minX - padding);
            visualCanvas.scrollTop = Math.max(0, minY - padding);
        }, 100);
    }

    function resetView() {
        if (visualCanvas) {
            visualCanvas.scrollLeft = 0;
            visualCanvas.scrollTop = 0;
        }
    }

    function displayWarnings(warnings) {
        const warningsContainer = document.getElementById('visualViewWarnings');
        if (!warningsContainer) return;

        if (warnings.length === 0) {
            warningsContainer.innerHTML = '<div class="alert alert-success"><i class="bi bi-check-circle"></i> All products have complete packaging data.</div>';
            return;
        }

        const hasInfo = warnings.some(w => w.type === 'info');
        const alertClass = hasInfo ? 'alert-info' : 'alert-warning';
        const icon = hasInfo ? 'bi-info-circle' : 'bi-exclamation-triangle';
        const title = hasInfo ? 'Information' : 'Missing Packaging or Distribution Data';

        let html = `<div class="alert ${alertClass}"><h6><i class="bi ${icon}"></i> ${title}</h6><ul class="mb-0">`;
        warnings.forEach(warning => {
            html += `<li>${escapeHtml(warning.message)}</li>`;
        });
        html += '</ul></div>';

        warningsContainer.innerHTML = html;
    }

    function displayError(message) {
        const warningsContainer = document.getElementById('visualViewWarnings');
        if (warningsContainer) {
            warningsContainer.innerHTML = `<div class="alert alert-danger"><i class="bi bi-exclamation-triangle"></i> ${escapeHtml(message)}</div>`;
        }
    }

    // Export function to be called from asn-management.js
    window.renderAsnVisualView = async function(shipment) {
        console.log('[Visual View] renderAsnVisualView called with shipment:', shipment);

        if (!shipment || !shipment.id) {
            console.error('[Visual View] Invalid shipment data');
            displayError('Invalid shipment data');
            return;
        }

        currentShipment = shipment;
        if (!initializeVisualView()) {
            displayError('Visual view container not found. Please ensure the Visual tab is visible.');
            return;
        }

        try {
            await loadProductPackagingData(shipment.id);
            console.log('[Visual View] Product packaging data loaded:', productPackagingData);
            // Defer render so the tab pane has been laid out and canvas has dimensions
            setTimeout(function() {
                if (!visualCanvas || !visualNodesLayer) return;
                renderVisualView(shipment);
            }, 50);
        } catch (error) {
            console.error('[Visual View] Error rendering visual view:', error);
            displayError('Error rendering visual view: ' + error.message);
        }
    };

})();
