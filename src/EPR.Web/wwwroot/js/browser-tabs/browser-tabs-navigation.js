/**
 * Browser Tabs Navigation Interception Module
 * Handles intercepting navigation links and opening them in tabs
 * 
 * This module provides:
 * - Link click interception
 * - URL pattern matching
 * - Tab creation from navigation
 * 
 * Dependencies: browser-tabs-core.js
 */

(function() {
    'use strict';
    
    const DEBUG_MODE = true;
    
    function debugLog(message, data = null) {
        if (DEBUG_MODE) {
            if (data) {
                console.log('[Browser Tabs Navigation]', message, data);
            } else {
                console.log('[Browser Tabs Navigation]', message);
            }
        }
    }
    
    function debugWarn(message, data = null) {
        if (DEBUG_MODE) {
            if (data) {
                console.warn('[Browser Tabs Navigation WARN]', message, data);
            } else {
                console.warn('[Browser Tabs Navigation WARN]', message);
            }
        }
    }
    
    /**
     * Tab configuration - defines which URLs should open in tabs
     * IMPORTANT: Order matters! More specific patterns must come BEFORE general patterns
     */
    const TABBED_PAGES = {
        'Assessment': {
            pattern: /^\/AssessmentNavigator(\/Home\/Index)?(\?.*code=|&code=)/i,
            title: null, // Will be set dynamically
            icon: 'bi-file-text'
        },
        'AssessmentNavigator': {
            pattern: /^\/AssessmentNavigator(\/Home\/Index)?\/?$/i,
            title: 'Assessment Navigator',
            icon: 'bi-compass'
        },
        'AssessmentSearch': {
            pattern: /^\/AssessmentSearch(\/Home\/Index)?\/?$/i,
            title: 'Assessment Search',
            icon: 'bi-search'
        },
        'NewAssessment': {
            pattern: /^\/AssessmentNavigator\/Home\/Wizard\/?$/i,
            title: 'New Assessment',
            icon: 'bi-magic'
        },
        'NewMaterial': {
            pattern: /^\/MaterialSearch\/Home\/New\/?$/i,
            title: 'New Material',
            icon: 'bi-plus-square'
        },
        'MaterialNavigator': {
            pattern: /^\/MaterialSearch(\/Home\/Index)?\/?$/i,
            title: 'Material Navigator',
            icon: 'bi-box-seam'
        },
        'AdminTools': {
            pattern: /^\/AdminTools/i,
            title: 'Data Types',
            icon: 'bi-gear'
        },
        'SupportTicket': {
            pattern: /^\/Help\/SupportTicket/i,
            title: 'New Support Ticket',
            icon: 'bi-ticket-perforated'
        },
        'Material': {
            pattern: /^\/MaterialSearch\/Home\/Detail/i,
            title: null, // Will be set dynamically
            icon: 'bi-box-seam'
        },
        'SysOpUsers': {
            pattern: /^\/SysOp\/Users(\/.*)?$/i,
            title: 'User Management',
            icon: 'bi-people'
        },
        'SysOpActivityLogs': {
            pattern: /^\/SysOp\/ActivityLogs/i,
            title: 'Activity Logs',
            icon: 'bi-list-check'
        },
        'SysOpSettings': {
            pattern: /^\/SysOp\/Settings/i,
            title: 'Site Settings',
            icon: 'bi-sliders'
        },
        'SysOpWhatsNew': {
            pattern: /^\/SysOp\/WhatsNew(\/.*)?$/i,
            title: "What's New",
            icon: 'bi-star'
        },
        'SysOpWhatsNewCreate': {
            pattern: /^\/SysOp\/WhatsNew\/Create/i,
            title: "Create What's New Item",
            icon: 'bi-star'
        },
        'SysOpImport': {
            pattern: /^\/SysOp\/Import/i,
            title: 'Import Data',
            icon: 'bi-upload'
        },
        'SysOpLegalNotices': {
            pattern: /^\/SysOp\/LegalNotices/i,
            title: 'Legal Notices',
            icon: 'bi-file-text'
        },
        'SysOpTheme': {
            pattern: /^\/SysOp\/Theme/i,
            title: 'Themes',
            icon: 'bi-palette'
        },
        'SysOpLicense': {
            pattern: /^\/SysOp\/License/i,
            title: 'License Management',
            icon: 'bi-key'
        },
        'VisualEditor': {
            pattern: /^\/VisualEditor(\/.*)?$/i,
            title: 'Visual Editor',
            icon: 'bi-diagram-3'
        }
    };
    
    /**
     * Browser Tabs Navigation Manager Class
     */
    class BrowserTabsNavigationManager {
        constructor(core, tabsManager) {
            this.core = core;
            this.tabsManager = tabsManager;
            this.navigationIntercepted = false;
        }
        
        /**
         * Check if URL is home page
         */
        isHomePage(url) {
            if (!url) return false;
            try {
                const urlObj = typeof url === 'string' ? new URL(url, window.location.origin) : url;
                return urlObj.pathname === '/' || 
                       (urlObj.pathname === '/Home' && !urlObj.search);
            } catch {
                return url === '/' || url === '/Home';
            }
        }
        
        /**
         * Determine if URL should open in a tab
         */
        shouldOpenInTab(url) {
            if (!url) {
                return null;
            }
            
            // Normalize URL for matching
            let urlPath = url;
            try {
                if (typeof url === 'string') {
                    if (!url.startsWith('http')) {
                        const urlObj = new URL(url, window.location.origin);
                        urlPath = urlObj.pathname + urlObj.search;
                    } else {
                        const urlObj = new URL(url);
                        urlPath = urlObj.pathname + urlObj.search;
                    }
                } else {
                    urlPath = url.pathname + url.search;
                }
            } catch (e) {
                urlPath = url;
            }
            
            // Ensure urlPath starts with /
            if (!urlPath.startsWith('/')) {
                urlPath = '/' + urlPath;
            }
            
            // Exclude print URLs
            if (urlPath.includes('/Print/Print') || 
                urlPath.includes('/Print?') || 
                urlPath.match(/\/Print\/Print/i) ||
                urlPath.match(/\/AssessmentNavigator\/Print/i)) {
                return null;
            }
            
            // Check against tabbed pages patterns
            for (const [key, config] of Object.entries(TABBED_PAGES)) {
                if (config.pattern.test(urlPath)) {
                    return { type: key, config };
                }
            }
            
            return null;
        }
        
        /**
         * Get tab title from URL or content
         */
        getTabTitle(url, type) {
            if (type === 'Assessment') {
                const match = url.match(/code=([^&]+)/);
                return match ? `Assessment: ${match[1]}` : 'Assessment';
            }
            if (type === 'Material') {
                const match = url.match(/code=([^&]+)/);
                return match ? `Material: ${match[1]}` : 'Material';
            }
            return TABBED_PAGES[type]?.title || 'Tab';
        }
        
        /**
         * Intercept navigation links
         */
        interceptNavigation() {
            if (this.navigationIntercepted) {
                debugWarn('Navigation already intercepted');
                return;
            }
            
            this.navigationIntercepted = true;
            debugLog('Setting up navigation interception');
            
            // Intercept link clicks
            document.addEventListener('click', (e) => {
                const link = e.target.closest('a[href]');
                if (!link) return;
                
                // Skip if modifier keys are pressed (Ctrl/Cmd for new tab, Shift for new window)
                if (e.ctrlKey || e.metaKey || e.shiftKey) {
                    return;
                }
                
                // Skip if target is _blank
                if (link.target === '_blank') {
                    return;
                }
                
                // Skip if link has data-no-tab attribute
                if (link.hasAttribute('data-no-tab')) {
                    return;
                }
                
                const href = link.getAttribute('href');
                if (!href) return;
                
                // Skip anchors and javascript: links
                if (href.startsWith('#') || href.startsWith('javascript:')) {
                    return;
                }
                
                // Check if URL should open in tab
                const tabConfig = this.shouldOpenInTab(href);
                if (!tabConfig) {
                    return; // Let normal navigation handle it
                }
                
                // Prevent default navigation
                e.preventDefault();
                e.stopPropagation();
                
                debugLog('Intercepted navigation', { href, type: tabConfig.type });
                
                // Normalize URL
                let normalizedUrl = href;
                if (!normalizedUrl.startsWith('http')) {
                    try {
                        normalizedUrl = new URL(href, window.location.origin).href;
                    } catch (e) {
                        normalizedUrl = window.location.origin + (href.startsWith('/') ? href : '/' + href);
                    }
                }
                
                // For Visual Editor, add unique instance parameter to ensure each tab has its own instance
                if (normalizedUrl.includes('/VisualEditor') || normalizedUrl.includes('/visualeditor')) {
                    try {
                        const urlObj = new URL(normalizedUrl);
                        // Only add instance parameter if not already present
                        if (!urlObj.searchParams.has('instance')) {
                            urlObj.searchParams.set('instance', Date.now() + '-' + Math.random().toString(36).substr(2, 9));
                            normalizedUrl = urlObj.href;
                        }
                    } catch (e) {
                        // If URL parsing fails, append instance parameter manually
                        const separator = normalizedUrl.includes('?') ? '&' : '?';
                        normalizedUrl = normalizedUrl + separator + 'instance=' + Date.now() + '-' + Math.random().toString(36).substr(2, 9);
                    }
                }
                
                // Get tab title
                const title = this.getTabTitle(normalizedUrl, tabConfig.type);
                
                // Add tab
                if (this.tabsManager && typeof this.tabsManager.addTab === 'function') {
                    this.tabsManager.addTab(normalizedUrl, title, tabConfig.config.icon, tabConfig.type);
                } else if (window.browserTabs && typeof window.browserTabs.addTab === 'function') {
                    window.browserTabs.addTab(normalizedUrl, title, tabConfig.config.icon, tabConfig.type);
                } else {
                    debugWarn('Tabs manager not available, falling back to normal navigation');
                    window.location.href = normalizedUrl;
                }
            }, true); // Use capture phase
            
            // Intercept browser back/forward buttons
            window.addEventListener('popstate', (e) => {
                if (e.state && e.state.tabId) {
                    debugLog('Popstate event', e.state);
                    if (this.tabsManager && typeof this.tabsManager.activateTab === 'function') {
                        this.tabsManager.activateTab(e.state.tabId);
                    } else if (window.browserTabs && typeof window.browserTabs.activateTab === 'function') {
                        window.browserTabs.activateTab(e.state.tabId);
                    }
                }
            });
            
            debugLog('Navigation interception setup complete');
        }
        
        /**
         * Stop intercepting navigation
         */
        stopIntercepting() {
            this.navigationIntercepted = false;
            debugLog('Navigation interception stopped');
        }
    }
    
    // Export to global scope
    if (typeof window !== 'undefined') {
        window.BrowserTabsNavigationManager = BrowserTabsNavigationManager;
        window.TABBED_PAGES = TABBED_PAGES; // Export for configuration
        debugLog('BrowserTabsNavigationManager exported to window');
    }
    
    // Also export for module systems
    if (typeof module !== 'undefined' && module.exports) {
        module.exports = BrowserTabsNavigationManager;
    }
})();

