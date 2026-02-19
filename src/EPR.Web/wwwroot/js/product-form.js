// Product Form Handler - GS1 Compliant Product Form
(function() {
    'use strict';

    let packagingComponents = [];
    let existingProducts = [];
    
    // Expose packagingComponents globally for visual view sync
    window.packagingComponents = packagingComponents;

    // Category to Sub-category mapping
    const categorySubCategories = {
        'Berries': ['Strawberries', 'Blueberries', 'Raspberries', 'Blackberries', 'Cranberries', 'Other'],
        'Citrus': ['Oranges', 'Lemons', 'Limes', 'Grapefruit', 'Mandarins', 'Other'],
        'Leafy Greens': ['Lettuce', 'Spinach', 'Kale', 'Arugula', 'Cabbage', 'Other'],
        'Root Vegetables': ['Carrots', 'Potatoes', 'Onions', 'Beets', 'Radishes', 'Other'],
        'Stone Fruit': ['Peaches', 'Plums', 'Cherries', 'Apricots', 'Nectarines', 'Other'],
        'Tropical Fruit': ['Bananas', 'Mangoes', 'Pineapples', 'Papayas', 'Avocados', 'Other'],
        'Other': ['Other']
    };

    document.addEventListener('DOMContentLoaded', function() {
        initializeForm();
        loadExistingProducts();
        setupQuickEntryDelegation();
        setupDynamicProductTitle();
        initializeViewToggle();
    });

    /**
     * Dynamic "Add Product" heading: show Product Name, or "Product Name | Brand" when both set.
     * Uses event delegation so it works when form is in main page or loaded in a tab.
     */
    function setupDynamicProductTitle() {
        function updateTitleFromInputs(nameEl, brandEl, titleSpan) {
            if (!titleSpan) return;
            var name = (nameEl && nameEl.value) ? nameEl.value.trim() : '';
            var brand = (brandEl && brandEl.value) ? brandEl.value.trim() : '';
            if (name && brand) titleSpan.textContent = name + ' | ' + brand;
            else if (name) titleSpan.textContent = name;
            else if (brand) titleSpan.textContent = brand;
            else titleSpan.textContent = 'Add Product';
        }
        document.body.addEventListener('input', function(e) {
            var el = e.target;
            if (el.id !== 'productName' && el.id !== 'brand') return;
            var root = el.closest('.container-fluid');
            if (!root || !root.querySelector('#formView')) return;
            var titleSpan = root.querySelector('#addProductTitleText');
            var nameEl = root.querySelector('#productName');
            var brandEl = root.querySelector('#brand');
            if (titleSpan && (nameEl || brandEl)) updateTitleFromInputs(nameEl, brandEl, titleSpan);
        });
        document.body.addEventListener('change', function(e) {
            var el = e.target;
            if (el.id !== 'productName' && el.id !== 'brand') return;
            var root = el.closest('.container-fluid');
            if (!root || !root.querySelector('#formView')) return;
            var titleSpan = root.querySelector('#addProductTitleText');
            var nameEl = root.querySelector('#productName');
            var brandEl = root.querySelector('#brand');
            if (titleSpan && (nameEl || brandEl)) updateTitleFromInputs(nameEl, brandEl, titleSpan);
        });
        // Initial update when Add Product is on page (full load)
        var titleSpan = document.getElementById('addProductTitleText');
        var nameEl = document.getElementById('productName');
        var brandEl = document.getElementById('brand');
        if (titleSpan && (nameEl || brandEl)) updateTitleFromInputs(nameEl, brandEl, titleSpan);
    }

    // Event delegation for quick-entry buttons so they work when form is loaded in a tab (injected HTML)
    function setupQuickEntryDelegation() {
        document.body.addEventListener('click', function(e) {
            const btn = e.target.closest('.quick-weight');
            if (btn) {
                e.preventDefault();
                const weightEl = document.getElementById('productWeight');
                const unitEl = document.getElementById('productWeightUnit');
                if (weightEl) weightEl.value = btn.dataset.value || '';
                if (unitEl) unitEl.value = (parseInt(btn.dataset.value, 10) >= 1000 ? 'kg' : 'g');
                btn.closest('.input-group-quick-btns')?.querySelectorAll('.quick-weight').forEach(function(b) { b.classList.remove('active'); });
                btn.classList.add('active');
                return;
            }
            const volBtn = e.target.closest('.quick-volume');
            if (volBtn) {
                e.preventDefault();
                const value = volBtn.dataset.value;
                const volEl = document.getElementById('productVolume');
                const volUnitEl = document.getElementById('productVolumeUnit');
                if (volEl) volEl.value = value || '';
                if (volUnitEl) volUnitEl.value = (parseInt(value, 10) >= 1000 ? 'L' : 'ml');
                volBtn.closest('.input-group-quick-btns')?.querySelectorAll('.quick-volume').forEach(function(b) { b.classList.remove('active'); });
                volBtn.classList.add('active');
                return;
            }
            const catBtn = e.target.closest('.quick-category');
            if (catBtn) {
                e.preventDefault();
                const catEl = document.getElementById('productCategory');
                if (catEl) {
                    catEl.value = catBtn.dataset.value || '';
                    if (typeof updateSubCategories === 'function') updateSubCategories(catEl.value);
                }
                return;
            }
        });
    }

    /**
     * Initialize View Toggle: Form Tabs | Form Single View | Visual View
     * Uses event delegation so it works when Add Product content is in main page or injected in a tab.
     */
    function initializeViewToggle() {
        function getViewRoot(btn) {
            if (!btn) return null;
            var root = btn.closest('.container-fluid');
            // Prefer the root that actually contains the form/visual views (works when nested or in tab)
            if (root && root.querySelector('#formView') && root.querySelector('#visualView')) return root;
            return root || document;
        }

        function getElements(root) {
            if (!root) return null;
            var formView = root.querySelector ? root.querySelector('#formView') : document.getElementById('formView');
            var visualView = root.querySelector ? root.querySelector('#visualView') : document.getElementById('visualView');
            if (!formView || !visualView) return null;
            return {
                formView: formView,
                visualView: visualView,
                sectionTabsBar: root.querySelector ? root.querySelector('#sectionTabsBar') : document.getElementById('sectionTabsBar'),
                tablist: root.querySelector ? root.querySelector('#sectionTabs[role="tablist"]') : document.querySelector('#sectionTabs[role="tablist"]'),
                sectionPanels: (root.querySelectorAll ? root.querySelectorAll('[id^="sectionPanel"]') : document.querySelectorAll('[id^="sectionPanel"]')) || [],
                viewFormTabsBtn: root.querySelector ? root.querySelector('#viewFormTabsBtn') : document.getElementById('viewFormTabsBtn'),
                viewFormSingleBtn: root.querySelector ? root.querySelector('#viewFormSingleBtn') : document.getElementById('viewFormSingleBtn'),
                viewVisualBtn: root.querySelector ? root.querySelector('#viewVisualBtn') : document.getElementById('viewVisualBtn')
            };
        }

        function updateButtonStates(els, activeBtn) {
            if (!els) return;
            [els.viewFormTabsBtn, els.viewFormSingleBtn, els.viewVisualBtn].forEach(function(btn) {
                if (btn) { btn.classList.remove('active'); btn.setAttribute('aria-pressed', 'false'); }
            });
            if (activeBtn) { activeBtn.classList.add('active'); activeBtn.setAttribute('aria-pressed', 'true'); }
        }

        function showFormTabsView(els) {
            if (!els) return;
            els.formView.style.display = '';
            els.formView.classList.remove('section-single-mode');
            els.formView.classList.add('section-tabbed-mode');
            els.visualView.style.display = 'none';
            els.visualView.classList.remove('show', 'active');
            document.body.classList.remove('visual-view-active');
            if (els.sectionTabsBar) els.sectionTabsBar.style.display = 'block';
            else if (els.tablist) els.tablist.style.display = '';
            var panels = els.sectionPanels;
            for (var i = 0; i < panels.length; i++) {
                if (i === 0) {
                    panels[i].style.display = 'block';
                    panels[i].setAttribute('aria-hidden', 'false');
                } else {
                    panels[i].style.display = 'none';
                    panels[i].setAttribute('aria-hidden', 'true');
                }
            }
            updateButtonStates(els, els.viewFormTabsBtn);
            try { localStorage.setItem('productFormViewMode', 'tabs'); } catch (err) {}
        }

        function showFormSingleView(els) {
            if (!els) return;
            els.formView.style.display = '';
            els.formView.classList.remove('section-tabbed-mode');
            els.formView.classList.add('section-single-mode');
            els.visualView.style.display = 'none';
            els.visualView.classList.remove('show', 'active');
            document.body.classList.remove('visual-view-active');
            if (els.sectionTabsBar) els.sectionTabsBar.style.display = 'none';
            else if (els.tablist) els.tablist.style.display = 'none';
            var panels = els.sectionPanels;
            for (var i = 0; i < panels.length; i++) {
                panels[i].style.setProperty('display', 'block', 'important');
                panels[i].setAttribute('aria-hidden', 'false');
            }
            updateButtonStates(els, els.viewFormSingleBtn);
            try { localStorage.setItem('productFormViewMode', 'single'); } catch (err) {}
        }

        function showVisualView(els) {
            if (!els) return;
            els.formView.style.display = 'none';
            els.visualView.style.display = 'block';
            els.visualView.classList.add('show', 'active');
            document.body.classList.add('visual-view-active');
            updateButtonStates(els, els.viewVisualBtn);
            document.dispatchEvent(new CustomEvent('visualViewShown'));
            try { localStorage.setItem('productFormViewMode', 'visual'); } catch (err) {}
        }

        document.body.addEventListener('click', function(e) {
            var btn = e.target.closest('#viewFormTabsBtn');
            if (btn) {
                e.preventDefault();
                e.stopPropagation();
                var root = getViewRoot(btn);
                var els = getElements(root);
                if (els) showFormTabsView(els);
                return;
            }
            btn = e.target.closest('#viewFormSingleBtn');
            if (btn) {
                e.preventDefault();
                e.stopPropagation();
                var root = getViewRoot(btn);
                var els = getElements(root);
                if (els) showFormSingleView(els);
                return;
            }
            btn = e.target.closest('#viewVisualBtn');
            if (btn) {
                e.preventDefault();
                e.stopPropagation();
                var root = getViewRoot(btn);
                var els = getElements(root);
                if (els) showVisualView(els);
                return;
            }
        });

        document.body.addEventListener('shown.bs.tab', function(ev) {
            var tab = ev.target;
            if (!tab || !tab.closest || !tab.closest('#sectionTabs')) return;
            var section = tab.getAttribute('data-section');
            if (!section) return;
            var root = getViewRoot(tab);
            var els = getElements(root);
            if (!els || !els.sectionPanels.length) return;
            els.sectionPanels.forEach(function(panel) {
                var isActive = panel.getAttribute('data-section') === section;
                panel.style.display = isActive ? 'block' : 'none';
                panel.setAttribute('aria-hidden', isActive ? 'false' : 'true');
            });
        });

        var savedMode = null;
        try { savedMode = localStorage.getItem('productFormViewMode'); } catch (err) {}
        if (savedMode === 'single' || savedMode === 'visual') {
            var formView = document.getElementById('formView');
            var visualView = document.getElementById('visualView');
            if (formView && visualView) {
                var root = formView.closest('.container-fluid') || document;
                var els = getElements(root);
                if (els) {
                    if (savedMode === 'single') showFormSingleView(els);
                    else showVisualView(els);
                }
            }
        }
    }

    function initializeForm() {
        // Category change handler
        const categorySelect = document.getElementById('productCategory');
        if (categorySelect) {
            categorySelect.addEventListener('change', function() {
                updateSubCategories(this.value);
            });
        }

        // Country of Origin "Other" handler
        const countrySelect = document.getElementById('countryOfOrigin');
        const countryOtherInput = document.getElementById('countryOfOriginOther');
        if (countrySelect && countryOtherInput) {
            countrySelect.addEventListener('change', function() {
                if (this.value === 'OTHER') {
                    countryOtherInput.style.display = 'block';
                    countryOtherInput.required = true;
                } else {
                    countryOtherInput.style.display = 'none';
                    countryOtherInput.required = false;
                    countryOtherInput.value = '';
                }
            });
        }

        // Quick entry buttons use event delegation so they work when form is loaded in a tab (AJAX content)
        // Handlers are attached once to document.body below

        // Adhesive type visibility
        document.querySelectorAll('input[name="closureTypes"]').forEach(cb => {
            cb.addEventListener('change', function() {
                const adhesiveGroup = document.getElementById('adhesiveTypeGroup');
                const adhesiveChecked = document.getElementById('closureAdhesive').checked;
                if (adhesiveGroup) {
                    adhesiveGroup.style.display = adhesiveChecked ? 'block' : 'none';
                }
            });
        });

        // EPR Category visibility
        const eprCheckbox = document.getElementById('eprSchemeApplicable');
        const eprCategoryGroup = document.getElementById('eprCategoryGroup');
        if (eprCheckbox && eprCategoryGroup) {
            eprCheckbox.addEventListener('change', function() {
                eprCategoryGroup.style.display = this.checked ? 'block' : 'none';
            });
        }

        // Packaging Component modal
        const btnAddComponent = document.getElementById('btnAddComponent');
        const btnSaveComponent = document.getElementById('btnSaveComponent');
        if (btnAddComponent) {
            btnAddComponent.addEventListener('click', function() {
                const modal = new bootstrap.Modal(document.getElementById('componentModal'));
                modal.show();
            });
        }

        if (btnSaveComponent) {
            btnSaveComponent.addEventListener('click', function() {
                const form = document.getElementById('componentForm');
                if (!form.checkValidity()) {
                    form.reportValidity();
                    return;
                }

                const component = {
                    materialType: document.getElementById('componentMaterialType').value,
                    weight: parseFloat(document.getElementById('componentWeight').value),
                    height: parseFloat(document.getElementById('componentHeight').value),
                    width: parseFloat(document.getElementById('componentWidth').value),
                    depth: parseFloat(document.getElementById('componentDepth').value),
                    volumeCapacity: document.getElementById('componentVolume').value ? parseFloat(document.getElementById('componentVolume').value) : null,
                    volumeUnit: document.getElementById('componentVolumeUnit').value
                };

                packagingComponents.push(component);
                // Update global reference
                window.packagingComponents = packagingComponents;
                updatePackagingComponentsList();
                updateTotalPackagingWeight();

                // Trigger sync to visual view
                if (window.syncFormToVisual) {
                    setTimeout(() => {
                        window.syncFormToVisual();
                    }, 100);
                }

                // Reset form and close modal
                form.reset();
                bootstrap.Modal.getInstance(document.getElementById('componentModal')).hide();
            });
        }

        // Photo preview
        const photoInput = document.getElementById('productPhotos');
        if (photoInput) {
            photoInput.addEventListener('change', function(e) {
                const preview = document.getElementById('photoPreview');
                preview.innerHTML = '';
                Array.from(e.target.files).forEach(file => {
                    if (file.size > 5 * 1024 * 1024) {
                        alert(`File ${file.name} exceeds 5MB limit`);
                        return;
                    }
                    const reader = new FileReader();
                    reader.onload = function(e) {
                        const img = document.createElement('img');
                        img.src = e.target.result;
                        img.style.maxWidth = '150px';
                        img.style.maxHeight = '150px';
                        img.className = 'me-2 mb-2';
                        preview.appendChild(img);
                    };
                    reader.readAsDataURL(file);
                });
            });
        }

        // Form submission
        const productForm = document.getElementById('productForm');
        if (productForm) {
            productForm.addEventListener('submit', async function(e) {
                e.preventDefault();
                
                if (!productForm.checkValidity()) {
                    productForm.reportValidity();
                    return;
                }

                // Validate GTIN
                const gtin = document.getElementById('gtin').value;
                if (!/^\d{8}$|^\d{12}$|^\d{13}$|^\d{14}$/.test(gtin)) {
                    alert('GTIN must be 8, 12, 13, or 14 digits');
                    return;
                }

                // Validate Parent Unit GTIN if Level 2 or 3
                const packagingLevel = document.getElementById('packagingLevel').value;
                const parentGtin = document.getElementById('parentUnitGtin').value;
                if ((packagingLevel === 'Retail Unit' || packagingLevel === 'Consignment Unit') && !parentGtin) {
                    if (!confirm('Parent Unit GTIN is recommended for Level 2 & 3 packaging. Continue without it?')) {
                        return;
                    }
                }

                await submitForm();
            });
        }

        // Clear form
        const btnClear = document.getElementById('btnClear');
        if (btnClear) {
            btnClear.addEventListener('click', function() {
                if (confirm('Are you sure you want to clear all form data?')) {
                    document.getElementById('productForm').reset();
                    packagingComponents = [];
                    window.packagingComponents = packagingComponents;
                    updatePackagingComponentsList();
                    updateTotalPackagingWeight();
                }
            });
        }

        // Collapsible sections
        document.querySelectorAll('.card-header[data-bs-toggle="collapse"]').forEach(header => {
            header.addEventListener('click', function() {
                const icon = this.querySelector('.bi-chevron-down, .bi-chevron-up');
                if (icon) {
                    const target = document.querySelector(this.dataset.bsTarget);
                    if (target && target.classList.contains('show')) {
                        icon.classList.remove('bi-chevron-down');
                        icon.classList.add('bi-chevron-up');
                    } else {
                        icon.classList.remove('bi-chevron-up');
                        icon.classList.add('bi-chevron-down');
                    }
                }
            });
        });
    }

    function updateSubCategories(category) {
        const subCategorySelect = document.getElementById('productSubCategory');
        if (!subCategorySelect) return;

        subCategorySelect.innerHTML = '<option value="">Select sub-category...</option>';
        
        if (category && categorySubCategories[category]) {
            categorySubCategories[category].forEach(sub => {
                const option = document.createElement('option');
                option.value = sub;
                option.textContent = sub;
                subCategorySelect.appendChild(option);
            });
        }
    }

    function updatePackagingComponentsList() {
        const list = document.getElementById('packagingComponentsList');
        if (!list) return;

        if (packagingComponents.length === 0) {
            list.innerHTML = '<p class="text-muted">No components added yet.</p>';
            return;
        }

        let html = '<div class="table-responsive"><table class="table table-sm table-bordered"><thead><tr><th>Material</th><th>Weight (g)</th><th>Dimensions (mm)</th><th>Volume</th><th>Actions</th></tr></thead><tbody>';
        packagingComponents.forEach((comp, index) => {
            html += `<tr>
                <td>${escapeHtml(comp.materialType)}</td>
                <td>${comp.weight.toFixed(2)}</td>
                <td>${comp.height} × ${comp.width} × ${comp.depth}</td>
                <td>${comp.volumeCapacity ? comp.volumeCapacity + ' ' + comp.volumeUnit : 'N/A'}</td>
                <td><button type="button" class="btn btn-sm btn-danger" onclick="removeComponent(${index})"><i class="bi bi-trash"></i></button></td>
            </tr>`;
        });
        html += '</tbody></table></div>';
        list.innerHTML = html;
    };
    
    // Also expose updateTotalPackagingWeight
    window.updateTotalPackagingWeight = updateTotalPackagingWeight;

    window.removeComponent = function(index) {
        packagingComponents.splice(index, 1);
        updatePackagingComponentsList();
        updateTotalPackagingWeight();
        
        // Trigger sync to visual view
        if (window.syncFormToVisual) {
            setTimeout(() => {
                window.syncFormToVisual();
            }, 100);
        }
    };

    function updateTotalPackagingWeight() {
        const total = packagingComponents.reduce((sum, comp) => sum + comp.weight, 0);
        const weightInput = document.getElementById('totalPackagingWeight');
        if (weightInput) {
            weightInput.value = total > 0 ? total.toFixed(2) : '';
        }
    }

    async function loadExistingProducts() {
        if (!document.getElementById('productForm')) return;
        try {
            const response = await fetch('/ProductForm/GetExistingProducts');
            const result = await response.json();
            
            if (result.success) {
                existingProducts = result.data;
                const select = document.getElementById('associatedSkus');
                if (select) {
                    select.innerHTML = '';
                    result.data.forEach(product => {
                        const option = document.createElement('option');
                        option.value = product.id;
                        option.textContent = `${product.name} (${product.sku || product.gtin || 'N/A'})`;
                        select.appendChild(option);
                    });
                }
            }
        } catch (error) {
            console.error('Error loading existing products:', error);
        }
    }

    async function submitForm() {
        const submitBtn = document.querySelector('button[type="submit"]');
        const originalText = submitBtn.innerHTML;
        submitBtn.disabled = true;
        submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Creating...';

        try {
            // Collect form data
            const formData = collectFormData();

            const response = await fetch('/ProductForm/SaveProductForm', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(formData)
            });

            const result = await response.json();

            if (result.success) {
                alert('Product created successfully!');
                window.location.href = '/Products';
            } else {
                alert('Failed to create product: ' + result.message);
            }
        } catch (error) {
            console.error('Error submitting form:', error);
            alert('Error creating product: ' + error.message);
        } finally {
            submitBtn.disabled = false;
            submitBtn.innerHTML = originalText;
        }
    }

    function collectFormData() {
        // Helper to get checked values
        function getCheckedValues(name) {
            return Array.from(document.querySelectorAll(`input[name="${name}"]:checked`)).map(cb => cb.value);
        }

        // Helper to get selected values from multi-select
        function getSelectedValues(id) {
            const select = document.getElementById(id);
            return select ? Array.from(select.selectedOptions).map(opt => opt.value) : [];
        }

        // Helper to get file names
        function getPhotoFiles() {
            const input = document.getElementById('productPhotos');
            if (!input || !input.files) return null;
            return Array.from(input.files).map(f => f.name);
        }

        return {
            // SECTION 1
            gtin: document.getElementById('gtin').value,
            productName: document.getElementById('productName').value,
            brand: document.getElementById('brand').value,
            productWeight: document.getElementById('productWeight').value ? parseFloat(document.getElementById('productWeight').value) : null,
            productWeightUnit: document.getElementById('productWeightUnit').value,
            productVolume: document.getElementById('productVolume').value ? parseFloat(document.getElementById('productVolume').value) : null,
            productVolumeUnit: document.getElementById('productVolumeUnit').value,
            skuCode: document.getElementById('skuCode').value || null,
            productCategory: document.getElementById('productCategory').value,
            productSubCategory: document.getElementById('productSubCategory').value,
            parentUnitGtin: document.getElementById('parentUnitGtin').value || null,
            unitsPerPackage: document.getElementById('unitsPerPackage').value ? parseInt(document.getElementById('unitsPerPackage').value) : null,
            countryOfOrigin: document.getElementById('countryOfOrigin').value === 'OTHER' 
                ? document.getElementById('countryOfOriginOther').value 
                : document.getElementById('countryOfOrigin').value,
            productPhotos: getPhotoFiles(),

            // SECTION 2
            packagingLevel: document.getElementById('packagingLevel').value,
            packagingType: document.getElementById('packagingType').value,
            packagingConfiguration: document.getElementById('packagingConfiguration').value,
            totalPackagingWeight: document.getElementById('totalPackagingWeight').value ? parseFloat(document.getElementById('totalPackagingWeight').value) : null,
            packagingComponents: packagingComponents.length > 0 ? packagingComponents : null,
            closureTypes: getCheckedValues('closureTypes'),
            adhesiveType: document.querySelector('input[name="adhesiveType"]:checked')?.value || null,
            tamperEvidence: document.getElementById('tamperEvidence').checked || null,
            resealable: document.getElementById('resealable').checked || null,
            labelTypes: getCheckedValues('labelTypes'),
            labelMaterial: document.querySelector('input[name="labelMaterial"]:checked')?.value || null,
            legalMarks: getCheckedValues('legalMarks'),

            // SECTION 3
            primaryRetailers: getCheckedValues('primaryRetailers'),
            retailFormat: getCheckedValues('retailFormat'),
            mostCommonPackSize: document.getElementById('mostCommonPackSize').value || null,
            isPrivateLabel: document.getElementById('isPrivateLabel').checked || null,
            geographicDistribution: getCheckedValues('geographicDistribution'),

            // SECTION 4
            shelfLifeExtension: document.getElementById('shelfLifeExtension').checked || null,
            estimatedShelfLifeDays: document.getElementById('estimatedShelfLifeDays').value ? parseInt(document.getElementById('estimatedShelfLifeDays').value) : null,
            foodWasteReductionImpact: document.getElementById('foodWasteReductionImpact').value || null,
            eprSchemeApplicable: document.getElementById('eprSchemeApplicable').checked || null,
            eprCategory: document.getElementById('eprCategory').value || null,
            apcoSignatory: document.getElementById('apcoSignatory').checked || null,
            sustainabilityCertifications: getCheckedValues('sustainabilityCertifications'),

            // SECTION 5
            productFamily: document.getElementById('productFamily').value || null,
            associatedSkus: getSelectedValues('associatedSkus'),
            variantReason: document.getElementById('variantReason').value || null,

            // SECTION 6
            generalNotes: document.getElementById('generalNotes').value || null,
            packagingRationale: document.getElementById('packagingRationale').value || null,
            knownIssues: document.getElementById('knownIssues').value || null,
            improvementPlans: document.getElementById('improvementPlans').value || null,

            status: 'submitted'
        };
    }

    function escapeHtml(text) {
        if (text === null || text === undefined) return '';
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }
})();
