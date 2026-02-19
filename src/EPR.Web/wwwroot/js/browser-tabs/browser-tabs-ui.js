/**
 * Browser Tabs UI Rendering Module
 * Handles tab rendering, drag-and-drop, and UI interactions
 * 
 * This module provides:
 * - Tab HTML generation
 * - Tab rendering
 * - Drag-and-drop functionality
 * - Scroll button management
 * 
 * Dependencies: browser-tabs-core.js
 */

(function() {
    'use strict';
    
    const DEBUG_MODE = true;
    
    function debugLog(message, data = null) {
        if (DEBUG_MODE) {
            if (data) {
                console.log('[Browser Tabs UI]', message, data);
            } else {
                console.log('[Browser Tabs UI]', message);
            }
        }
    }
    
    function debugWarn(message, data = null) {
        if (DEBUG_MODE) {
            if (data) {
                console.warn('[Browser Tabs UI WARN]', message, data);
            } else {
                console.warn('[Browser Tabs UI WARN]', message);
            }
        }
    }
    
    /**
     * Browser Tabs UI Manager Class
     */
    class BrowserTabsUIManager {
        constructor(core) {
            this.core = core;
            this.isDragging = false;
            this.dragTabId = null;
            this.dragOffset = 0;
        }
        
        /**
         * Escape HTML to prevent XSS
         */
        escapeHtml(text) {
            const div = document.createElement('div');
            div.textContent = text;
            return div.innerHTML;
        }
        
        /**
         * Create tab HTML
         */
        createTabHTML(tab) {
            const isActive = tab.id === this.core.activeTabId;
            return `
                <div class="browser-tab ${isActive ? 'active' : ''}" 
                     data-tab-id="${tab.id}" 
                     draggable="true">
                    <span class="browser-tab-icon"><i class="bi ${tab.icon}"></i></span>
                    <span class="browser-tab-title">${this.escapeHtml(tab.title)}</span>
                    <button class="browser-tab-close" onclick="window.browserTabs?.closeTab(${tab.id}); event.stopPropagation();" title="Close">
                        <i class="bi bi-x"></i>
                    </button>
                </div>
            `;
        }
        
        /**
         * Render tabs
         */
        renderTabs() {
            debugLog('renderTabs() called', { tabCount: this.core.tabs.length, activeTab: this.core.activeTabId });
            
            const container = document.getElementById('browserTabsContainer');
            const scrollContainer = document.getElementById('browserTabsScroll');
            
            if (!container || !scrollContainer) {
                debugWarn('Containers not found, cannot render tabs');
                return;
            }
            
            const tabs = this.core.getTabs();
            
            // Show/hide tab container based on tabs
            if (tabs.length === 0) {
                debugLog('No tabs, hiding container');
                container.style.display = 'none';
                document.body.classList.remove('has-tabs');
                const mainContent = document.querySelector('main');
                if (mainContent) {
                    mainContent.style.display = 'block';
                }
                const contentArea = document.getElementById('browserTabContent');
                if (contentArea) {
                    contentArea.style.display = 'none';
                }
                return;
            }
            
            debugLog('Rendering', tabs.length, 'tabs');
            
            // Show tabs
            container.style.display = 'flex';
            container.style.visibility = 'visible';
            container.style.opacity = '1';
            document.body.classList.add('has-tabs');
            const mainContent = document.querySelector('main');
            if (mainContent) {
                mainContent.style.display = 'none';
            }
            const contentArea = document.getElementById('browserTabContent');
            if (contentArea) {
                contentArea.style.display = 'block';
            }
            
            // Render tab HTML
            const tabHTML = tabs.map(tab => this.createTabHTML(tab)).join('');
            scrollContainer.innerHTML = tabHTML;
            debugLog('Tabs rendered to DOM');
            
            // Attach event listeners
            this.attachTabListeners();
            
            // Update scroll buttons
            this.updateScrollButtons();
            
            // Ensure active tab is visible
            setTimeout(() => {
                if (this.core.activeTabId) {
                    const activeTabEl = document.querySelector(`[data-tab-id="${this.core.activeTabId}"]`);
                    if (activeTabEl) {
                        activeTabEl.scrollIntoView({ behavior: 'auto', block: 'nearest', inline: 'nearest' });
                    }
                }
                this.updateScrollButtons();
            }, 50);
            
            debugLog('renderTabs() complete');
        }
        
        /**
         * Attach event listeners to tabs
         */
        attachTabListeners() {
            document.querySelectorAll('.browser-tab').forEach(tabEl => {
                const tabId = parseInt(tabEl.getAttribute('data-tab-id'));
                
                // Click to activate
                tabEl.addEventListener('click', (e) => {
                    if (!e.target.closest('.browser-tab-close')) {
                        if (window.browserTabs) {
                            window.browserTabs.activateTab(tabId);
                        }
                    }
                });
                
                // Drag and drop
                tabEl.addEventListener('dragstart', (e) => {
                    this.isDragging = true;
                    this.dragTabId = tabId;
                    e.dataTransfer.effectAllowed = 'move';
                    e.dataTransfer.setData('text/html', tabEl.innerHTML);
                    tabEl.style.opacity = '0.5';
                });
                
                tabEl.addEventListener('dragend', (e) => {
                    tabEl.style.opacity = '';
                    this.isDragging = false;
                    this.dragTabId = null;
                });
                
                tabEl.addEventListener('dragover', (e) => {
                    e.preventDefault();
                    e.dataTransfer.dropEffect = 'move';
                    
                    const afterElement = this.getDragAfterElement(e.clientX);
                    const scrollContainer = document.getElementById('browserTabsScroll');
                    if (afterElement == null) {
                        scrollContainer.appendChild(tabEl);
                    } else {
                        scrollContainer.insertBefore(tabEl, afterElement);
                    }
                });
                
                tabEl.addEventListener('drop', (e) => {
                    e.preventDefault();
                    if (this.dragTabId && this.dragTabId !== tabId && window.browserTabs) {
                        window.browserTabs.reorderTabs(this.dragTabId, tabId);
                    }
                });
            });
        }
        
        /**
         * Get element after which to insert dragged tab
         */
        getDragAfterElement(x) {
            const tabs = Array.from(document.querySelectorAll('.browser-tab:not(.dragging)'));
            return tabs.reduce((closest, child) => {
                const box = child.getBoundingClientRect();
                const offset = x - box.left - box.width / 2;
                if (offset < 0 && offset > closest.offset) {
                    return { offset: offset, element: child };
                } else {
                    return closest;
                }
            }, { offset: Number.NEGATIVE_INFINITY }).element;
        }
        
        /**
         * Update scroll buttons visibility
         */
        updateScrollButtons() {
            const scrollContainer = document.getElementById('browserTabsScroll');
            const leftBtn = document.getElementById('browserTabsScrollLeft');
            const rightBtn = document.getElementById('browserTabsScrollRight');
            
            if (!scrollContainer || !leftBtn || !rightBtn) {
                debugWarn('Scroll buttons or container not found');
                return;
            }
            
            // Force a reflow
            void scrollContainer.offsetWidth;
            
            const scrollWidth = scrollContainer.scrollWidth;
            const clientWidth = scrollContainer.clientWidth;
            const needsScrolling = scrollWidth > clientWidth + 1;
            
            if (!needsScrolling) {
                leftBtn.style.display = 'none';
                rightBtn.style.display = 'none';
                return;
            }
            
            const scrollLeft = scrollContainer.scrollLeft;
            const maxScroll = scrollWidth - clientWidth;
            
            const canScrollLeft = scrollLeft > 5;
            const canScrollRight = scrollLeft < (maxScroll - 5);
            
            leftBtn.style.display = canScrollLeft ? 'block' : 'none';
            rightBtn.style.display = canScrollRight ? 'block' : 'none';
        }
        
        /**
         * Setup scroll button event listeners
         */
        setupScrollButtons() {
            const scrollContainer = document.getElementById('browserTabsScroll');
            const leftBtn = document.getElementById('browserTabsScrollLeft');
            const rightBtn = document.getElementById('browserTabsScrollRight');
            
            if (!scrollContainer || !leftBtn || !rightBtn) {
                return;
            }
            
            leftBtn.addEventListener('click', () => {
                scrollContainer.scrollBy({ left: -200, behavior: 'smooth' });
                setTimeout(() => this.updateScrollButtons(), 100);
            });
            
            rightBtn.addEventListener('click', () => {
                scrollContainer.scrollBy({ left: 200, behavior: 'smooth' });
                setTimeout(() => this.updateScrollButtons(), 100);
            });
            
            scrollContainer.addEventListener('scroll', () => {
                this.updateScrollButtons();
            });
        }
        
        /**
         * Setup real-time observers for scroll button updates
         */
        setupRealTimeObservers() {
            const scrollContainer = document.getElementById('browserTabsScroll');
            if (!scrollContainer) return;
            
            // Use ResizeObserver to detect tab width changes
            if (typeof ResizeObserver !== 'undefined') {
                const resizeObserver = new ResizeObserver(() => {
                    this.updateScrollButtons();
                });
                resizeObserver.observe(scrollContainer);
            }
            
            // Use MutationObserver to detect tab additions/removals
            const mutationObserver = new MutationObserver(() => {
                this.updateScrollButtons();
            });
            mutationObserver.observe(scrollContainer, {
                childList: true,
                subtree: true
            });
        }
    }
    
    // Export to global scope
    if (typeof window !== 'undefined') {
        window.BrowserTabsUIManager = BrowserTabsUIManager;
        debugLog('BrowserTabsUIManager exported to window');
    }
    
    // Also export for module systems
    if (typeof module !== 'undefined' && module.exports) {
        module.exports = BrowserTabsUIManager;
    }
})();

