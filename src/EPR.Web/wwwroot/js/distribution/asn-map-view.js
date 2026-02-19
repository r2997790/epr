// ASN Map View - OpenStreetMap visualization
(function() {
    'use strict';

    let asnMap = null;
    let mapMarkers = [];
    let mapRoutes = [];
    let currentShipment = null;

    // Initialize map view when tab is shown
    document.addEventListener('DOMContentLoaded', function() {
        const mapTab = document.getElementById('map-tab');
        if (mapTab) {
            mapTab.addEventListener('shown.bs.tab', function() {
                if (!asnMap) {
                    initializeMap();
                }
                if (currentShipment) {
                    renderMapView(currentShipment);
                }
            });
        }
    });

    function initializeMap() {
        const mapContainer = document.getElementById('asnMap');
        if (!mapContainer) return;
        if (typeof L === 'undefined') {
            console.error('Leaflet (L) is not loaded. Map view requires Leaflet.');
            return;
        }

        // Initialize Leaflet map centered on UK
        asnMap = L.map('asnMap').setView([54.5, -2.0], 6);

        // Add OpenStreetMap tiles
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '© OpenStreetMap contributors',
            maxZoom: 19
        }).addTo(asnMap);
    }

    function geocodeAddress(address, city, country, callback) {
        // Simple geocoding - in production, use a proper geocoding service
        // For now, use approximate coordinates based on city/country
        const geocodeMap = {
            'Manchester': { lat: 53.4808, lng: -2.2426 },
            'Birmingham': { lat: 52.4862, lng: -1.8904 },
            'Leeds': { lat: 53.8008, lng: -1.5491 },
            'Southampton': { lat: 50.9097, lng: -1.4044 },
            'London': { lat: 51.5074, lng: -0.1278 },
            'Cambridge': { lat: 52.2053, lng: 0.1218 },
            'Milton Keynes': { lat: 52.0406, lng: -0.7594 },
            'Norwich': { lat: 52.6309, lng: 1.2974 }
        };

        const cityKey = city || '';
        const coords = geocodeMap[cityKey] || { lat: 54.5, lng: -2.0 };
        callback(coords);
    }

    function renderMapView(shipment) {
        if (!asnMap) {
            initializeMap();
        }

        currentShipment = shipment;

        // Clear existing markers and routes
        clearMap();

        const warnings = [];
        const locations = [];

        // Add shipper location
        if (shipment.shipperCity || shipment.shipperAddress) {
            geocodeAddress(shipment.shipperAddress, shipment.shipperCity, shipment.shipperCountryCode, (coords) => {
                const shipperMarker = L.marker([coords.lat, coords.lng])
                    .addTo(asnMap)
                    .bindPopup(`
                        <strong><i class="bi bi-building"></i> Shipper</strong><br>
                        ${escapeHtml(shipment.shipperName)}<br>
                        ${escapeHtml(shipment.shipperAddress || '')}<br>
                        ${escapeHtml(shipment.shipperCity || '')}, ${escapeHtml(shipment.shipperCountryCode || '')}<br>
                        GLN: ${escapeHtml(shipment.shipperGln)}
                    `);
                shipperMarker.options.type = 'shipper';
                mapMarkers.push(shipperMarker);
                locations.push({ type: 'shipper', coords: coords, name: shipment.shipperName });
            });
        }

        // Add receiver location (if different from shipper)
        if (shipment.receiverName) {
            // Try to get receiver city from pallets or use default
            const receiverCity = shipment.pallets && shipment.pallets.length > 0 
                ? shipment.pallets[0].destinationCity 
                : null;
            
            geocodeAddress(null, receiverCity || 'London', shipment.shipperCountryCode, (coords) => {
                const receiverMarker = L.marker([coords.lat, coords.lng], { icon: createCustomIcon('receiver') })
                    .addTo(asnMap)
                    .bindPopup(`
                        <strong><i class="bi bi-shop"></i> Receiver</strong><br>
                        ${escapeHtml(shipment.receiverName)}<br>
                        GLN: ${escapeHtml(shipment.receiverGln)}
                    `);
                receiverMarker.options.type = 'receiver';
                mapMarkers.push(receiverMarker);
                locations.push({ type: 'receiver', coords: coords, name: shipment.receiverName });
            });
        }

        // Add pallet destinations
        if (shipment.pallets && shipment.pallets.length > 0) {
            shipment.pallets.forEach((pallet, index) => {
                if (pallet.destinationGln || pallet.destinationName) {
                    if (!pallet.destinationCity && !pallet.destinationAddress) {
                        warnings.push({
                            type: 'pallet',
                            palletNumber: index + 1,
                            sscc: pallet.sscc,
                            message: `Pallet ${index + 1} (SSCC: ${pallet.sscc}) has destination name but missing address/city`
                        });
                    }

                    geocodeAddress(pallet.destinationAddress, pallet.destinationCity, pallet.destinationCountryCode, (coords) => {
                        const destMarker = L.marker([coords.lat, coords.lng], { icon: createCustomIcon('destination') })
                            .addTo(asnMap)
                            .bindPopup(`
                                <strong><i class="bi bi-geo-alt-fill"></i> Destination ${index + 1}</strong><br>
                                ${escapeHtml(pallet.destinationName || 'Unknown')}<br>
                                ${escapeHtml(pallet.destinationAddress || '')}<br>
                                ${escapeHtml(pallet.destinationCity || '')}, ${escapeHtml(pallet.destinationPostalCode || '')}<br>
                                ${escapeHtml(pallet.destinationCountryCode || '')}<br>
                                SSCC: ${escapeHtml(pallet.sscc || 'N/A')}
                            `);
                        destMarker.options.type = 'destination';
                        destMarker.options.palletIndex = index;
                        mapMarkers.push(destMarker);
                        locations.push({ type: 'destination', coords: coords, name: pallet.destinationName, palletIndex: index });
                    });
                } else {
                    warnings.push({
                        type: 'pallet',
                        palletNumber: index + 1,
                        sscc: pallet.sscc,
                        message: `Pallet ${index + 1} (SSCC: ${pallet.sscc}) has no destination information`
                    });
                }

                // Check line items for missing destinations
                if (pallet.lineItems && pallet.lineItems.length > 0) {
                    pallet.lineItems.forEach((item, itemIndex) => {
                        if (!pallet.destinationGln && !pallet.destinationName) {
                            warnings.push({
                                type: 'lineItem',
                                palletNumber: index + 1,
                                lineNumber: item.lineNumber,
                                gtin: item.gtin,
                                description: item.description,
                                message: `Line Item ${item.lineNumber} (${item.description}) on Pallet ${index + 1} has no destination`
                            });
                        }
                    });
                }
            });
        }

        // Draw routes between locations
        setTimeout(() => {
            drawRoutes(locations);
            displayWarnings(warnings);
            fitMapToMarkers();
        }, 500);
    }

    function createCustomIcon(type) {
        const colors = {
            shipper: '#0066cc',
            receiver: '#28a745',
            destination: '#ffc107',
            warning: '#dc3545'
        };

        return L.divIcon({
            className: 'custom-marker',
            html: `<div style="background-color: ${colors[type] || '#6c757d'}; width: 20px; height: 20px; border-radius: 50%; border: 2px solid white; box-shadow: 0 2px 4px rgba(0,0,0,0.3);"></div>`,
            iconSize: [20, 20],
            iconAnchor: [10, 10]
        });
    }

    function drawRoutes(locations) {
        if (locations.length < 2) return;

        const shipper = locations.find(l => l.type === 'shipper');
        const receiver = locations.find(l => l.type === 'receiver');
        const destinations = locations.filter(l => l.type === 'destination');

        // Draw route from shipper to receiver (if both exist)
        if (shipper && receiver) {
            const route = L.polyline(
                [[shipper.coords.lat, shipper.coords.lng], [receiver.coords.lat, receiver.coords.lng]],
                { color: '#0066cc', weight: 3, opacity: 0.7 }
            ).addTo(asnMap);
            mapRoutes.push(route);
        }

        // Draw routes from receiver to each destination (or shipper to destination if no receiver)
        const source = receiver || shipper;
        if (source) {
            destinations.forEach(dest => {
                const route = L.polyline(
                    [[source.coords.lat, source.coords.lng], [dest.coords.lat, dest.coords.lng]],
                    { color: '#28a745', weight: 2, opacity: 0.5, dashArray: '5, 5' }
                ).addTo(asnMap);
                mapRoutes.push(route);
            });
        }
    }

    function fitMapToMarkers() {
        if (mapMarkers.length === 0) return;

        const group = new L.featureGroup(mapMarkers);
        asnMap.fitBounds(group.getBounds().pad(0.1));
    }

    function clearMap() {
        mapMarkers.forEach(marker => asnMap.removeLayer(marker));
        mapRoutes.forEach(route => asnMap.removeLayer(route));
        mapMarkers = [];
        mapRoutes = [];
    }

    function displayWarnings(warnings) {
        const warningsContainer = document.getElementById('mapWarnings');
        if (!warningsContainer) return;

        if (warnings.length === 0) {
            warningsContainer.innerHTML = '<div class="alert alert-success"><i class="bi bi-check-circle"></i> All destinations have complete location information.</div>';
            return;
        }

        let html = '<div class="alert alert-warning"><h6><i class="bi bi-exclamation-triangle"></i> Missing Destination Information</h6><ul class="mb-0">';
        warnings.forEach(warning => {
            html += `<li>${escapeHtml(warning.message)}</li>`;
        });
        html += '</ul></div>';

        warningsContainer.innerHTML = html;
    }

    function escapeHtml(text) {
        if (text === null || text === undefined) return '';
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    // Export function to be called from asn-management.js
    window.renderAsnMapView = function(shipment) {
        if (!shipment) return;
        currentShipment = shipment;
        if (!asnMap) {
            initializeMap();
        }
        if (!asnMap) return;
        // Small delay so map container is visible (e.g. after tab switch)
        setTimeout(() => {
            renderMapView(shipment);
        }, 150);
    };

})();
