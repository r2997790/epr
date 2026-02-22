/**
 * Browser Tabs System for EPR
 * Simplified implementation based on EmpauerLocal browser tabs
 */
(function() {
    'use strict';
    
    const DEBUG_MODE = true;
    
    function debugLog(message, data = null) {
        if (DEBUG_MODE) {
            console.log('[Browser Tabs]', message, data || '');
        }
    }
    
    const TAB_STORAGE_KEY = 'eprBrowserTabs';
    const ACTIVE_TAB_KEY = 'eprActiveTabId';
    let tabs = [];
    let activeTabId = null;
    
    class BrowserTabsManager {
        constructor() {
            this.loadTabsFromStorage();
            this.init();
        }
        
        init() {
            debugLog('Initializing browser tabs');
            this.renderTabs();
            this.setupEventListeners();
            if (tabs.length > 0 && activeTabId && tabs.some(t => t.id === activeTabId)) {
                this.activateTab(activeTabId);
            }
            window.addEventListener('popstate', (e) => {
                if (e.state && e.state.tabId && tabs.some(t => t.id === e.state.tabId)) {
                    this.activateTab(e.state.tabId);
                }
            });
        }
        
        addTab(url, title, icon, category) {
            const tabId = 'tab-' + Date.now();
            const tab = {
                id: tabId,
                url: url,
                title: title,
                icon: icon || 'bi-file',
                category: category || 'General',
                isActive: false,
                content: null,
                isLoaded: false
            };
            
            tabs.push(tab);
            this.activateTab(tabId);
            this.saveTabsToStorage();
            this.renderTabs();
            
            this.loadTabContent(tabId, url);
            
            return tabId;
        }
        
        activateTab(tabId) {
            const tab = tabs.find(t => t.id === tabId);
            if (!tab) return;
            
            tabs.forEach(t => {
                t.isActive = t.id === tabId;
            });
            activeTabId = tabId;
            this.saveTabsToStorage();
            this.renderTabs();
            
            const contentContainer = document.getElementById('browserTabContent');
            const mainContent = document.querySelector('main');
            if (contentContainer && mainContent) {
                mainContent.style.display = 'none';
                contentContainer.style.display = 'block';
            }
            
            if (contentContainer) {
                if (tab.isLoaded && tab.content) {
                    contentContainer.innerHTML = tab.content;
                    if ((tab.scriptContents && tab.scriptContents.length) || (tab.scriptSrcs && tab.scriptSrcs.length)) {
                        const origGetElementById = document.getElementById.bind(document);
                        document.getElementById = function(id) {
                            const inContainer = contentContainer.querySelector('#' + id);
                            return inContainer || origGetElementById(id);
                        };
                        try {
                            (tab.scriptContents || []).forEach(text => {
                                try {
                                    const script = document.createElement('script');
                                    script.textContent = text;
                                    contentContainer.appendChild(script);
                                } catch (err) { debugLog('Script exec error on restore', err); }
                            });
                            (tab.scriptSrcs || []).forEach(src => {
                                const script = document.createElement('script');
                                script.src = src.startsWith('http') || src.startsWith('//') ? src : (src.startsWith('/') ? (window.location.origin + src) : (window.location.origin + '/' + src));
                                contentContainer.appendChild(script);
                            });
                        } finally {
                            document.getElementById = origGetElementById;
                        }
                    }
                    try {
                        window.history.pushState({ tabId: tabId }, '', tab.url);
                    } catch (e) { /* same-origin only */ }
                } else {
                    this.loadTabContent(tabId, tab.url);
                }
            }
        }
        
        closeTab(tabId) {
            const index = tabs.findIndex(t => t.id === tabId);
            if (index === -1) return;
            
            const wasActive = tabs[index].isActive;
            tabs.splice(index, 1);
            
            if (tabs.length > 0) {
                this.activateTab(tabs[0].id);
            } else {
                activeTabId = null;
                const container = document.getElementById('browserTabsContainer');
                const contentArea = document.getElementById('browserTabContent');
                const mainContent = document.querySelector('main');
                if (container) container.style.display = 'none';
                if (contentArea) contentArea.style.display = 'none';
                if (mainContent) mainContent.style.display = 'block';
                document.body.classList.remove('has-tabs');
                try {
                    window.history.pushState({}, '', window.location.pathname || '/');
                } catch (e) { }
            }
            
            this.saveTabsToStorage();
            this.renderTabs();
        }
        
        renderTabs() {
            const container = document.getElementById('browserTabsContainer');
            const scrollContainer = document.getElementById('browserTabsScroll');
            
            if (!container || !scrollContainer) return;
            
            if (tabs.length === 0) {
                container.style.display = 'none';
                document.body.classList.remove('has-tabs');
                return;
            }
            
            container.style.display = 'flex';
            document.body.classList.add('has-tabs');
            
            scrollContainer.innerHTML = tabs.map(tab => `
                <div class="browser-tab ${tab.isActive ? 'active' : ''}" data-tab-id="${tab.id}">
                    <i class="${tab.icon}"></i>
                    <span class="browser-tab-title">${tab.title}</span>
                    <button class="browser-tab-close" onclick="event.stopPropagation(); window.browserTabs.closeTab('${tab.id}')">
                        <i class="bi bi-x"></i>
                    </button>
                </div>
            `).join('');
            
            // Add click handlers
            scrollContainer.querySelectorAll('.browser-tab').forEach(el => {
                el.addEventListener('click', (e) => {
                    if (!e.target.closest('.browser-tab-close')) {
                        const tabId = el.dataset.tabId;
                        this.activateTab(tabId);
                    }
                });
            });

            const self = this;
            setTimeout(() => self.updateScrollButtons(), 0);
        }

        scrollTabs(direction) {
            const scrollContainer = document.getElementById('browserTabsScroll');
            if (!scrollContainer) return;
            const scrollAmount = 200;
            const currentScroll = scrollContainer.scrollLeft;
            const maxScroll = scrollContainer.scrollWidth - scrollContainer.clientWidth;
            let targetScroll = direction === 'left'
                ? Math.max(0, currentScroll - scrollAmount)
                : Math.min(maxScroll, currentScroll + scrollAmount);
            scrollContainer.scrollTo({ left: targetScroll, behavior: 'smooth' });
        }

        updateScrollButtons() {
            const scrollContainer = document.getElementById('browserTabsScroll');
            const leftBtn = document.getElementById('browserTabsScrollLeft');
            const rightBtn = document.getElementById('browserTabsScrollRight');
            if (!scrollContainer || !leftBtn || !rightBtn) return;
            void scrollContainer.offsetWidth;
            const scrollWidth = scrollContainer.scrollWidth;
            const clientWidth = scrollContainer.clientWidth;
            const needsScrolling = scrollWidth > clientWidth + 1;
            leftBtn.style.display = needsScrolling ? 'flex' : 'none';
            rightBtn.style.display = needsScrolling ? 'flex' : 'none';
            leftBtn.style.visibility = scrollContainer.scrollLeft <= 1 ? 'hidden' : 'visible';
            rightBtn.style.visibility = scrollContainer.scrollLeft >= scrollWidth - clientWidth - 1 ? 'hidden' : 'visible';
        }
        
        loadTabContent(tabId, url) {
            if (!url) return;
            
            const contentContainer = document.getElementById('browserTabContent');
            const mainContent = document.querySelector('main');
            if (!contentContainer) return;
            
            contentContainer.style.display = 'block';
            if (mainContent) mainContent.style.display = 'none';
            document.body.classList.add('has-tabs');
            contentContainer.innerHTML = '<div class="loading-spinner-container"><div class="spinner-border"></div><div class="spinner-text">Loading...</div></div>';
            
            if (url.includes('/VisualEditor') || url.includes('/visualeditor')) {
                // Already on Visual Editor page (e.g. full-page load with instance param). Do not redirect again or we cycle.
                const onVisualEditor = window.location.pathname.toLowerCase().includes('visualeditor');
                if (onVisualEditor) {
                    const tab = tabs.find(t => t.id === tabId);
                    if (tab) {
                        tab.url = window.location.href;
                        tab.isLoaded = true;
                        this.saveTabsToStorage();
                    }
                    if (contentContainer) contentContainer.style.display = 'none';
                    if (mainContent) mainContent.style.display = 'block';
                    return;
                }
                delete window.eprCurrentInstanceId;
                const separator = url.includes('?') ? '&' : '?';
                window.location.href = url + separator + 'instance=' + Date.now();
                return;
            }
            
            const self = this;
            fetch(url, {
                credentials: 'same-origin',
                headers: { 'X-Requested-With': 'XMLHttpRequest', 'Accept': 'text/html' }
            })
                .then(response => response.text())
                .then(html => {
                    const parser = new DOMParser();
                    const doc = parser.parseFromString(html, 'text/html');
                    let content = '';
                    const main = doc.querySelector('main');
                    if (main) {
                        content = main.innerHTML;
                    } else {
                        const body = doc.body;
                        const clone = body.cloneNode(true);
                        [].forEach.call(clone.querySelectorAll('header, nav, .navbar, .footer, footer, #browserTabsContainer, #browserTabContent'), function (el) {
                            if (el && el.parentNode) el.parentNode.removeChild(el);
                        });
                        const container = clone.querySelector('.container-fluid') || clone.querySelector('.container');
                        content = container ? container.innerHTML : clone.innerHTML;
                    }
                    contentContainer.innerHTML = content;
                    // Re-execute inline scripts so init code (e.g. setupTabSwitching, loadData) attaches
                    // to the injected DOM. Scoped getElementById ensures scripts find elements inside
                    // contentContainer when duplicates exist (e.g. main + tab both have Packaging).
                    const origGetElementById = document.getElementById.bind(document);
                    document.getElementById = function(id) {
                        const inContainer = contentContainer.querySelector('#' + id);
                        return inContainer || origGetElementById(id);
                    };
                    const scriptContents = [];
                    doc.querySelectorAll('script:not([src])').forEach(s => { if (s.textContent.trim()) scriptContents.push(s.textContent); });
                    const scriptSrcs = [];
                    doc.querySelectorAll('script[src]').forEach(s => {
                        const src = s.getAttribute('src');
                        if (src && !scriptSrcs.includes(src)) scriptSrcs.push(src);
                    });
                    try {
                        scriptContents.forEach(text => {
                            try {
                                const script = document.createElement('script');
                                script.textContent = text;
                                contentContainer.appendChild(script);
                            } catch (err) { debugLog('Script exec error', err); }
                        });
                        scriptSrcs.forEach(src => {
                            const script = document.createElement('script');
                            script.src = src.startsWith('http') || src.startsWith('//') ? src : (src.startsWith('/') ? (window.location.origin + src) : (window.location.origin + '/' + src));
                            contentContainer.appendChild(script);
                        });
                    } finally {
                        document.getElementById = origGetElementById;
                    }
                    const tab = tabs.find(t => t.id === tabId);
                    if (tab) {
                        tab.content = content;
                        tab.scriptContents = scriptContents;
                        tab.scriptSrcs = scriptSrcs;
                        tab.isLoaded = true;
                    }
                    window.history.pushState({ tabId: tabId }, '', url);
                })
                .catch(error => {
                    console.error('Error loading tab content:', error);
                    contentContainer.innerHTML = '<div class="alert alert-danger">Error loading content</div>';
                    const tab = tabs.find(t => t.id === tabId);
                    if (tab) tab.isLoaded = true;
                });
        }
        
        updateTabTitle(url, newTitle) {
            // Find tab by URL (try exact match first, then partial match)
            let tab = tabs.find(t => t.url === url);
            if (!tab) {
                tab = tabs.find(t => t.url.includes(url) || url.includes(t.url));
            }
            // Also check if URL contains VisualEditor (for tabs that might have query params)
            if (!tab && (url.includes('/VisualEditor') || url.includes('/visualeditor'))) {
                tab = tabs.find(t => (t.url.includes('/VisualEditor') || t.url.includes('/visualeditor')) && t.isActive);
            }
            if (!tab && (url.includes('/VisualEditor') || url.includes('/visualeditor'))) {
                tab = tabs.find(t => t.url.includes('/VisualEditor') || t.url.includes('/visualeditor'));
            }
            if (tab) {
                tab.title = newTitle;
                this.saveTabsToStorage();
                this.renderTabs();
                debugLog('Updated tab title:', { url, newTitle, tabId: tab.id });
            } else {
                debugLog('Tab not found for URL:', url);
            }
        }
        
        setupEventListeners() {
            const scrollContainer = document.getElementById('browserTabsScroll');
            if (scrollContainer) {
                scrollContainer.addEventListener('scroll', () => this.updateScrollButtons());
            }
            window.addEventListener('resize', () => this.updateScrollButtons());
            // Intercept navigation links
            document.addEventListener('click', (e) => {
                // Packaging Management sub-tabs (Suppliers, Raw Materials, etc.) - event delegation
                const packagingTabLink = e.target.closest('#packagingManagementTabs a[data-type]');
                if (packagingTabLink && document.getElementById('browserTabContent')?.contains(packagingTabLink)) {
                    const type = packagingTabLink.getAttribute('data-type');
                    if (type && window.switchPackagingTab) {
                        e.preventDefault();
                        e.stopPropagation();
                        window.switchPackagingTab(type);
                        return;
                    }
                }
                const link = e.target.closest('a[href]');
                if (!link || link.target === '_blank' || link.href.startsWith('mailto:') || link.href.startsWith('#')) {
                    return;
                }
                
                // Skip if link has data-no-tab="true" attribute - allow normal navigation
                if (link.getAttribute('data-no-tab') === 'true') {
                    return;
                }
                
                const href = link.getAttribute('href');
                if (!href || href.startsWith('http') || href.startsWith('//')) return;
                
                // Resolve path for full-navigation checks (must exit entire handler, not just try block)
                let allowFullNavigation = false;
                try {
                    const path = (href.indexOf('?') >= 0 ? href.split('?')[0] : href).replace(/\/$/, '') || '/';
                    const pathNorm = path.startsWith('http') ? new URL(path).pathname : path;
                    // Home: always full navigation
                    if (pathNorm === '/' || pathNorm === '/Home' || pathNorm === '/Home/Index') {
                        e.preventDefault();
                        window.location.href = href.startsWith('http') ? href : (window.location.origin + (href.startsWith('/') ? href : '/' + href));
                        return;
                    }
                    // Reporting: always full navigation so charts and scripts load, URL matches page
                    if (pathNorm.startsWith('/Reporting') || pathNorm.startsWith('Reporting')) {
                        allowFullNavigation = true;
                    }
                } catch (err) { }
                
                if (allowFullNavigation) return;
                
                if (href && !href.startsWith('http') && !href.startsWith('//')) {
                    if (href.includes('/VisualEditor') || href.includes('/visualeditor')) {
                        // Check if this is a menu item below browser tabs (has specific parent classes)
                        const isMenuBelowTabs = link.closest('.browser-tabs-menu') || link.closest('.menu-below-tabs');
                        if (isMenuBelowTabs) {
                            // Menu items below tabs should NOT add browser tabs
                            return; // Let normal navigation handle it
                        }
                        
                        // Main navigation VisualEditor link should add browser tab
                        e.preventDefault();
                        const title = link.textContent.trim() || 'Visual Editor';
                        const icon = link.querySelector('i')?.className || 'bi-diagram-3';
                        this.addTab(href, title, icon, 'VisualEditor');
                        return;
                    }
                    
                    e.preventDefault();
                    const title = link.textContent.trim() || 'New Tab';
                    const icon = link.querySelector('i')?.className || 'bi-file';
                    this.addTab(href, title, icon, 'Navigation');
                }
            });
        }
        
        saveTabsToStorage() {
            try {
                const toSave = tabs.map(t => ({
                    id: t.id, url: t.url, title: t.title, icon: t.icon, category: t.category,
                    isActive: t.isActive
                }));
                localStorage.setItem(TAB_STORAGE_KEY, JSON.stringify(toSave));
                if (activeTabId) {
                    localStorage.setItem(ACTIVE_TAB_KEY, activeTabId);
                }
            } catch (e) {
                debugLog('Error saving tabs to storage', e);
            }
        }
        
        loadTabsFromStorage() {
            try {
                const saved = localStorage.getItem(TAB_STORAGE_KEY);
                if (saved) {
                    tabs = JSON.parse(saved).map(t => ({
                        ...t,
                        content: null,
                        isLoaded: false
                    }));
                }
                activeTabId = localStorage.getItem(ACTIVE_TAB_KEY);
            } catch (e) {
                debugLog('Error loading tabs from storage', e);
                tabs = [];
            }
        }
    }
    
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', () => { window.browserTabs = new BrowserTabsManager(); });
    } else {
        window.browserTabs = new BrowserTabsManager();
    }
})();


