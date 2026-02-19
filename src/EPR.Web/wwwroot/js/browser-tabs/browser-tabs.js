/**
 * Browser Tabs Main Integration File
 * Brings together all browser tabs modules
 * 
 * This is the main entry point that:
 * - Initializes all modules
 * - Provides unified API
 * - Handles content loading and display
 * 
 * Dependencies:
 * - browser-tabs-core.js
 * - browser-tabs-ajax.js
 * - browser-tabs-scripts.js
 * - browser-tabs-ui.js
 * - browser-tabs-navigation.js
 */

(function() {
    'use strict';
    
    const DEBUG_MODE = true;
    
    function debugLog(message, data = null) {
        if (DEBUG_MODE) {
            if (data) {
                console.log('[Browser Tabs]', message, data);
            } else {
                console.log('[Browser Tabs]', message);
            }
        }
    }
    
    function debugError(message, error = null) {
        if (DEBUG_MODE) {
            if (error) {
                console.error('[Browser Tabs ERROR]', message, error);
            } else {
                console.error('[Browser Tabs ERROR]', message);
            }
        }
    }
    
    /**
     * Main Browser Tabs Manager
     * Coordinates all browser tabs functionality
     */
    class BrowserTabsManager {
        constructor(options = {}) {
            // Initialize core
            this.core = new BrowserTabsCore(options);
            
            // Initialize AJAX loader
            this.ajaxLoader = new BrowserTabsAjaxLoader(this.core);
            
            // Initialize script executor
            this.scriptExecutor = new BrowserTabsScriptExecutor();
            
            // Initialize UI manager
            this.uiManager = new BrowserTabsUIManager(this.core);
            
            // Initialize navigation manager
            this.navigationManager = new BrowserTabsNavigationManager(this.core, this);
            
            // Content cache
            this.contentCache = new Map();
        }
        
        /**
         * Initialize browser tabs system
         */
        init() {
            debugLog('Initializing Browser Tabs system...');
            
            try {
                // Setup scroll buttons
                this.uiManager.setupScrollButtons();
                
                // Setup real-time observers
                this.uiManager.setupRealTimeObservers();
                
                // Intercept navigation
                this.navigationManager.interceptNavigation();
                
                // Render initial tabs
                this.renderTabs();
                
                debugLog('Browser Tabs initialization complete');
            } catch (error) {
                debugError('Error during initialization', error);
                console.error('Browser tabs initialization error:', error);
            }
        }
        
        /**
         * Add a new tab
         */
        addTab(url, title, icon, type) {
            debugLog('addTab() called', { url, title, icon, type });
            
            // Normalize URL
            let normalizedUrl = url;
            if (!normalizedUrl.startsWith('http')) {
                try {
                    if (normalizedUrl.startsWith('/')) {
                        normalizedUrl = window.location.origin + normalizedUrl;
                    } else {
                        normalizedUrl = new URL(normalizedUrl, window.location.href).href;
                    }
                } catch (e) {
                    debugError('Error normalizing URL', e);
                    normalizedUrl = window.location.origin + (normalizedUrl.startsWith('/') ? normalizedUrl : '/' + normalizedUrl);
                }
            }
            
            // Check if it's a Print URL
            if (normalizedUrl.includes('/Print/Print') || 
                normalizedUrl.includes('/Print?') ||
                normalizedUrl.match(/\/Print/i)) {
                debugError('Attempted to add Print page as tab - rejecting');
                return null;
            }
            
            // Add tab via core
            const tabId = this.core.addTab(normalizedUrl, title, icon, type || 'default');
            
            if (tabId) {
                // Render tabs
                this.renderTabs();
                
                // Load content
                this.loadTabContent(tabId);
            }
            
            return tabId;
        }
        
        /**
         * Activate a tab
         */
        activateTab(tabId) {
            if (this.core.activateTab(tabId)) {
                // Update UI
                document.querySelectorAll('.browser-tab').forEach(el => {
                    el.classList.remove('active');
                    if (parseInt(el.getAttribute('data-tab-id')) === tabId) {
                        el.classList.add('active');
                    }
                });
                
                // Ensure active tab is visible
                setTimeout(() => {
                    const activeTabEl = document.querySelector(`[data-tab-id="${tabId}"]`);
                    if (activeTabEl) {
                        activeTabEl.scrollIntoView({ behavior: 'smooth', block: 'nearest', inline: 'nearest' });
                    }
                    this.uiManager.updateScrollButtons();
                }, 100);
                
                // Load content if not loaded
                const tab = this.core.findTabById(tabId);
                if (tab && !tab.isLoaded) {
                    this.loadTabContent(tabId);
                } else if (tab && tab.content) {
                    this.showTabContent(tabId);
                }
            }
        }
        
        /**
         * Close a tab
         */
        closeTab(tabId) {
            if (this.core.closeTab(tabId)) {
                this.renderTabs();
                
                // If all tabs closed, redirect to home
                if (this.core.getTabs().length === 0) {
                    const contentArea = document.getElementById('browserTabContent');
                    if (contentArea) {
                        contentArea.style.display = 'none';
                    }
                    document.body.classList.remove('has-tabs');
                    const mainContent = document.querySelector('main');
                    if (mainContent) {
                        mainContent.style.display = 'block';
                    }
                    
                    if (!this.navigationManager.isHomePage(window.location.href)) {
                        window.location.href = '/';
                    }
                }
            }
        }
        
        /**
         * Reorder tabs
         */
        reorderTabs(dragId, targetId) {
            const tabs = this.core.getTabs();
            const dragIndex = tabs.findIndex(t => t.id === dragId);
            const targetIndex = tabs.findIndex(t => t.id === targetId);
            
            if (dragIndex === -1 || targetIndex === -1) return;
            
            const [removed] = tabs.splice(dragIndex, 1);
            tabs.splice(targetIndex, 0, removed);
            
            this.core.tabs = tabs;
            this.core.saveState();
            this.renderTabs();
        }
        
        /**
         * Load tab content via AJAX
         */
        async loadTabContent(tabId) {
            const tab = this.core.findTabById(tabId);
            if (!tab) return;
            
            const contentArea = document.getElementById('browserTabContent');
            if (!contentArea) return;
            
            // Show loading indicator
            contentArea.innerHTML = '<div class="loading-spinner-container-relative"><div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div><p class="spinner-text">Loading...</p></div>';
            
            try {
                // Load content via AJAX loader
                const html = await this.ajaxLoader.loadTabContent(tabId, tab.url);
                
                if (!html || html.trim().length === 0) {
                    throw new Error('Empty response received');
                }
                
                // Check if it's a Print page
                const hasAutoPrintScript = html.includes('window.onload') && 
                                          html.includes('window.print()') &&
                                          html.includes('setTimeout(function()');
                const hasPrintTitle = html.match(/<title>\s*Print\s*-\s*/i);
                const isFullHtmlDocument = html.includes('<!DOCTYPE html>') || html.includes('<html');
                
                const isPrintPage = hasAutoPrintScript && hasPrintTitle && isFullHtmlDocument;
                
                if (isPrintPage) {
                    debugError('Attempted to load Print page in tab - rejecting');
                    throw new Error('Print pages cannot be loaded in tabs. Please use the Print button instead.');
                }
                
                // Store content
                tab.content = html;
                tab.isLoaded = true;
                this.core.saveState();
                
                // Show content
                this.showTabContent(tabId);
            } catch (error) {
                debugError('Error loading tab content', error);
                contentArea.innerHTML = `<div class="alert alert-danger p-3">
                    <strong>Error loading content</strong><br/>
                    ${error.message}<br/>
                    <small>URL: ${tab.url}</small>
                </div>`;
                tab.isLoaded = true;
                tab.content = '<div class="alert alert-danger">Error loading content: ' + error.message + '</div>';
                this.core.saveState();
            }
        }
        
        /**
         * Show tab content
         */
        showTabContent(tabId) {
            const tab = this.core.findTabById(tabId);
            if (!tab || !tab.content) {
                debugWarn('Tab not found or no content', { tabId, tabExists: !!tab, hasContent: !!tab?.content });
                return;
            }
            
            const contentArea = document.getElementById('browserTabContent');
            if (!contentArea) {
                debugError('Content area not found!');
                return;
            }
            
            // Parse HTML and extract main content
            const parser = new DOMParser();
            const doc = parser.parseFromString(tab.content, 'text/html');
            const mainContent = doc.querySelector('main') || doc.body;
            const bodyContent = mainContent ? mainContent.innerHTML : tab.content;
            
            contentArea.innerHTML = bodyContent;
            
            // Ensure containers are visible
            const tabContainer = document.getElementById('browserTabsContainer');
            if (tabContainer && this.core.getTabs().length > 0) {
                tabContainer.style.display = 'flex';
            }
            contentArea.style.display = 'block';
            
            // Hide main content
            const mainContentEl = document.querySelector('main');
            if (mainContentEl) {
                mainContentEl.style.display = 'none';
            }
            document.body.classList.add('has-tabs');
            
            // Process scripts
            setTimeout(() => {
                this.scriptExecutor.processScripts(tab.content, contentArea);
                
                // Apply translations
                setTimeout(() => {
                    if (window.applyTranslations) {
                        window.applyTranslations(contentArea);
                    } else if (window.localization && window.localization.apply) {
                        window.localization.apply(contentArea);
                    }
                }, 100);
            }, 50);
            
            // Update URL without reload
            window.history.pushState({ tabId: tabId }, '', tab.url);
            
            // Scroll to top
            contentArea.scrollTop = 0;
        }
        
        /**
         * Render tabs
         */
        renderTabs() {
            this.uiManager.renderTabs();
        }
        
        /**
         * Navigate to assessment
         */
        navigateToAssessment(code) {
            const url = `/AssessmentNavigator?code=${encodeURIComponent(code)}`;
            const title = `Assessment: ${code}`;
            return this.addTab(url, title, 'bi-file-text', 'Assessment');
        }
    }
    
    // Initialize on DOM ready
    function init() {
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', () => {
                initializeBrowserTabs();
            });
        } else {
            initializeBrowserTabs();
        }
    }
    
    function initializeBrowserTabs() {
        debugLog('Initializing Browser Tabs...');
        
        // Create global instance
        window.browserTabs = new BrowserTabsManager({
            maxTabs: 20,
            persistState: true,
            enableDragDrop: true
        });
        
        // Initialize
        window.browserTabs.init();
        
        debugLog('Browser Tabs initialized');
    }
    
    // Auto-initialize
    init();
    
    // Export for module systems
    if (typeof module !== 'undefined' && module.exports) {
        module.exports = BrowserTabsManager;
    }
})();

