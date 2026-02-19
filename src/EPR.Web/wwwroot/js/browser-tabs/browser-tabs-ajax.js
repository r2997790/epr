/**
 * Browser Tabs AJAX Content Loading Module
 * Handles loading tab content via AJAX
 * 
 * This module provides:
 * - AJAX content loading
 * - Content caching
 * - Script extraction and execution
 * - Error handling
 * 
 * Dependencies: browser-tabs-core.js
 */

(function() {
    'use strict';
    
    if (typeof window.BrowserTabsCore === 'undefined') {
        console.error('[Browser Tabs AJAX] BrowserTabsCore is required');
        return;
    }
    
    const DEBUG_MODE = true;
    
    function debugLog(message, data = null) {
        if (DEBUG_MODE) {
            if (data) {
                console.log('[Browser Tabs AJAX]', message, data);
            } else {
                console.log('[Browser Tabs AJAX]', message);
            }
        }
    }
    
    function debugError(message, error = null) {
        if (DEBUG_MODE) {
            if (error) {
                console.error('[Browser Tabs AJAX ERROR]', message, error);
            } else {
                console.error('[Browser Tabs AJAX ERROR]', message);
            }
        }
    }
    
    /**
     * AJAX Content Loader Class
     */
    class BrowserTabsAjaxLoader {
        constructor(core) {
            this.core = core;
            this.contentCache = new Map();
            this.loadingPromises = new Map();
            this.options = {
                cacheEnabled: true,
                cacheTimeout: 5 * 60 * 1000, // 5 minutes
                retryAttempts: 3,
                retryDelay: 1000
            };
        }
        
        /**
         * Load tab content via AJAX
         */
        async loadTabContent(tabId, url) {
            // Check cache first
            if (this.options.cacheEnabled) {
                const cached = this.contentCache.get(url);
                if (cached && Date.now() - cached.timestamp < this.options.cacheTimeout) {
                    debugLog('Using cached content', url);
                    return cached.content;
                }
            }
            
            // Check if already loading
            if (this.loadingPromises.has(url)) {
                debugLog('Content already loading, waiting...', url);
                return await this.loadingPromises.get(url);
            }
            
            // Start loading
            const loadPromise = this._loadContent(url);
            this.loadingPromises.set(url, loadPromise);
            
            try {
                const content = await loadPromise;
                
                // Cache content
                if (this.options.cacheEnabled) {
                    this.contentCache.set(url, {
                        content: content,
                        timestamp: Date.now()
                    });
                }
                
                return content;
            } finally {
                this.loadingPromises.delete(url);
            }
        }
        
        /**
         * Internal content loading with retry logic
         */
        async _loadContent(url, attempt = 1) {
            try {
                debugLog(`Loading content (attempt ${attempt})`, url);
                
                const response = await fetch(url, {
                    method: 'GET',
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest',
                        'Accept': 'text/html'
                    },
                    cache: 'no-cache'
                });
                
                if (!response.ok) {
                    throw new Error(`HTTP ${response.status}: ${response.statusText}`);
                }
                
                const html = await response.text();
                debugLog('Content loaded successfully', { url, size: html.length });
                
                return html;
            } catch (error) {
                debugError(`Load attempt ${attempt} failed`, error);
                
                if (attempt < this.options.retryAttempts) {
                    await new Promise(resolve => setTimeout(resolve, this.options.retryDelay * attempt));
                    return await this._loadContent(url, attempt + 1);
                }
                
                throw error;
            }
        }
        
        /**
         * Clear cache for a specific URL
         */
        clearCache(url) {
            this.contentCache.delete(url);
            debugLog('Cache cleared', url);
        }
        
        /**
         * Clear all cache
         */
        clearAllCache() {
            this.contentCache.clear();
            debugLog('All cache cleared');
        }
        
        /**
         * Extract scripts from HTML content
         */
        extractScripts(html) {
            const parser = new DOMParser();
            const doc = parser.parseFromString(html, 'text/html');
            const scripts = doc.querySelectorAll('script');
            
            const scriptData = [];
            scripts.forEach(script => {
                scriptData.push({
                    src: script.getAttribute('src'),
                    textContent: script.textContent || '',
                    type: script.getAttribute('type') || 'text/javascript',
                    attributes: Array.from(script.attributes).reduce((acc, attr) => {
                        acc[attr.name] = attr.value;
                        return acc;
                    }, {})
                });
            });
            
            debugLog('Scripts extracted', { count: scriptData.length });
            return scriptData;
        }
    }
    
    // Export to global scope
    if (typeof window !== 'undefined') {
        window.BrowserTabsAjaxLoader = BrowserTabsAjaxLoader;
        debugLog('BrowserTabsAjaxLoader exported to window');
    }
    
    // Also export for module systems
    if (typeof module !== 'undefined' && module.exports) {
        module.exports = BrowserTabsAjaxLoader;
    }
})();

