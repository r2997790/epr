/**
 * Browser Tabs Core Engine
 * Shared module for browser-style tab management
 * 
 * This module provides:
 * - Tab creation and management
 * - Tab state persistence
 * - Tab navigation
 * - Tab closing and cleanup
 * 
 * Usage:
 *   const tabs = new BrowserTabsCore();
 *   tabs.addTab(url, title, icon, category);
 */

(function() {
    'use strict';
    
    // Debug mode - set to false in production
    const DEBUG_MODE = true;
    
    // Debug logging functions
    function debugLog(message, data = null) {
        if (DEBUG_MODE) {
            if (data) {
                console.log('[Browser Tabs Core]', message, data);
            } else {
                console.log('[Browser Tabs Core]', message);
            }
        }
    }
    
    function debugError(message, error = null) {
        if (DEBUG_MODE) {
            if (error) {
                console.error('[Browser Tabs Core ERROR]', message, error);
            } else {
                console.error('[Browser Tabs Core ERROR]', message);
            }
        }
    }
    
    function debugWarn(message, data = null) {
        if (DEBUG_MODE) {
            if (data) {
                console.warn('[Browser Tabs Core WARN]', message, data);
            } else {
                console.warn('[Browser Tabs Core WARN]', message);
            }
        }
    }
    
    /**
     * Browser Tabs Core Class
     * Manages tab state and operations
     */
    class BrowserTabsCore {
        constructor(options = {}) {
            this.TAB_STORAGE_KEY = options.storageKey || 'browserTabs';
            this.ACTIVE_TAB_KEY = options.activeTabKey || 'activeTabId';
            this.tabs = [];
            this.activeTabId = null;
            this.tabCounter = 0;
            this.options = {
                maxTabs: options.maxTabs || 20,
                persistState: options.persistState !== false,
                enableDragDrop: options.enableDragDrop !== false,
                ...options
            };
            
            this.loadState();
            debugLog('BrowserTabsCore initialized', { tabCount: this.tabs.length, activeTab: this.activeTabId });
        }
        
        /**
         * Load tab state from localStorage
         */
        loadState() {
            try {
                const storedTabs = localStorage.getItem(this.TAB_STORAGE_KEY);
                if (storedTabs) {
                    this.tabs = JSON.parse(storedTabs);
                    // Validate tabs
                    this.tabs = this.tabs.filter(tab => tab && tab.id && tab.url);
                }
                
                const storedActiveTab = localStorage.getItem(this.ACTIVE_TAB_KEY);
                if (storedActiveTab && this.tabs.some(t => t.id === storedActiveTab)) {
                    this.activeTabId = storedActiveTab;
                }
                
                // Update tab counter
                if (this.tabs.length > 0) {
                    const maxId = Math.max(...this.tabs.map(t => parseInt(t.id.replace('tab-', '')) || 0));
                    this.tabCounter = maxId;
                }
            } catch (e) {
                debugError('Error loading tab state', e);
                this.tabs = [];
                this.activeTabId = null;
            }
        }
        
        /**
         * Save tab state to localStorage
         */
        saveState() {
            if (!this.options.persistState) return;
            
            try {
                localStorage.setItem(this.TAB_STORAGE_KEY, JSON.stringify(this.tabs));
                if (this.activeTabId) {
                    localStorage.setItem(this.ACTIVE_TAB_KEY, this.activeTabId);
                }
            } catch (e) {
                debugError('Error saving tab state', e);
            }
        }
        
        /**
         * Generate unique tab ID
         */
        generateTabId() {
            this.tabCounter++;
            return `tab-${this.tabCounter}`;
        }
        
        /**
         * Find tab by URL
         */
        findTabByUrl(url) {
            return this.tabs.find(tab => tab.url === url);
        }
        
        /**
         * Find tab by ID
         */
        findTabById(tabId) {
            return this.tabs.find(tab => tab.id === tabId);
        }
        
        /**
         * Add a new tab
         */
        addTab(url, title, icon, category) {
            // Check if tab already exists
            const existingTab = this.findTabByUrl(url);
            if (existingTab) {
                debugLog('Tab already exists, activating', existingTab.id);
                this.activateTab(existingTab.id);
                return existingTab.id;
            }
            
            // Check max tabs limit
            if (this.tabs.length >= this.options.maxTabs) {
                // Remove oldest inactive tab
                const inactiveTabs = this.tabs.filter(t => t.id !== this.activeTabId);
                if (inactiveTabs.length > 0) {
                    this.closeTab(inactiveTabs[0].id);
                } else {
                    debugWarn('Cannot add tab: maximum tabs reached');
                    return null;
                }
            }
            
            // Create new tab
            const tabId = this.generateTabId();
            const newTab = {
                id: tabId,
                url: url,
                title: title || 'New Tab',
                icon: icon || 'bi-file-text',
                category: category || 'default',
                isActive: false,
                isLoaded: false,
                createdAt: new Date().toISOString()
            };
            
            this.tabs.push(newTab);
            this.activateTab(tabId);
            this.saveState();
            
            debugLog('Tab added', newTab);
            return tabId;
        }
        
        /**
         * Activate a tab
         */
        activateTab(tabId) {
            const tab = this.findTabById(tabId);
            if (!tab) {
                debugWarn('Tab not found', tabId);
                return false;
            }
            
            // Deactivate all tabs
            this.tabs.forEach(t => t.isActive = false);
            
            // Activate selected tab
            tab.isActive = true;
            this.activeTabId = tabId;
            this.saveState();
            
            debugLog('Tab activated', tabId);
            return true;
        }
        
        /**
         * Close a tab
         */
        closeTab(tabId) {
            const tabIndex = this.tabs.findIndex(t => t.id === tabId);
            if (tabIndex === -1) {
                debugWarn('Tab not found for closing', tabId);
                return false;
            }
            
            const wasActive = this.tabs[tabIndex].id === this.activeTabId;
            this.tabs.splice(tabIndex, 1);
            
            // If closed tab was active, activate another tab
            if (wasActive && this.tabs.length > 0) {
                // Try to activate tab to the right, or left if none
                const newActiveIndex = Math.min(tabIndex, this.tabs.length - 1);
                this.activateTab(this.tabs[newActiveIndex].id);
            } else if (this.tabs.length === 0) {
                this.activeTabId = null;
            }
            
            this.saveState();
            debugLog('Tab closed', tabId);
            return true;
        }
        
        /**
         * Get all tabs
         */
        getTabs() {
            return [...this.tabs];
        }
        
        /**
         * Get active tab
         */
        getActiveTab() {
            return this.tabs.find(t => t.id === this.activeTabId) || null;
        }
        
        /**
         * Check if tab exists
         */
        tabExists(tabId) {
            return this.tabs.some(t => t.id === tabId);
        }
        
        /**
         * Update tab title
         */
        updateTabTitle(tabId, title) {
            const tab = this.findTabById(tabId);
            if (tab) {
                tab.title = title;
                this.saveState();
                return true;
            }
            return false;
        }
        
        /**
         * Mark tab as loaded
         */
        markTabLoaded(tabId) {
            const tab = this.findTabById(tabId);
            if (tab) {
                tab.isLoaded = true;
                this.saveState();
                return true;
            }
            return false;
        }
    }
    
    // Export to global scope
    if (typeof window !== 'undefined') {
        window.BrowserTabsCore = BrowserTabsCore;
        debugLog('BrowserTabsCore exported to window');
    }
    
    // Also export for module systems
    if (typeof module !== 'undefined' && module.exports) {
        module.exports = BrowserTabsCore;
    }
})();

