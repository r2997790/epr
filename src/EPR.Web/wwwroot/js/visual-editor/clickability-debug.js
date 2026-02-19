/**
 * Clickability Debug Tool
 * Helps identify what's blocking clicks and drags on the Visual Editor
 */

class ClickabilityDebugger {
    constructor() {
        this.init();
    }

    init() {
        console.log('[Clickability Debugger] Initializing...');
        this.setupClickListener();
        this.analyzePage();
    }

    setupClickListener() {
        // Listen to all clicks and log what element is clicked
        document.addEventListener('click', (e) => {
            const target = e.target;
            const path = e.composedPath();
            
            console.group('[Clickability Debugger] Click detected');
            console.log('Target element:', target);
            console.log('Target tag:', target.tagName);
            console.log('Target classes:', target.className);
            console.log('Target ID:', target.id);
            console.log('Computed pointer-events:', window.getComputedStyle(target).pointerEvents);
            console.log('Computed z-index:', window.getComputedStyle(target).zIndex);
            console.log('Computed position:', window.getComputedStyle(target).position);
            console.log('Click path:', path);
            
            // Check if click was prevented
            if (e.defaultPrevented) {
                console.warn('⚠️ Click was prevented!');
            }
            
            // Check for blocking elements
            const blockingElements = this.findBlockingElements(e.clientX, e.clientY);
            if (blockingElements.length > 0) {
                console.warn('⚠️ Found potentially blocking elements:', blockingElements);
            }
            
            console.groupEnd();
        }, true); // Use capture phase

        // Listen to mousedown for drag detection
        document.addEventListener('mousedown', (e) => {
            const target = e.target;
            console.log('[Clickability Debugger] Mousedown on:', target.tagName, target.className, 'pointer-events:', window.getComputedStyle(target).pointerEvents);
        }, true);
    }

    findBlockingElements(x, y) {
        const elements = document.elementsFromPoint(x, y);
        const blocking = [];
        
        elements.forEach((el, index) => {
            const style = window.getComputedStyle(el);
            const zIndex = parseInt(style.zIndex) || 0;
            const pointerEvents = style.pointerEvents;
            const position = style.position;
            
            // Check if element might be blocking
            if (pointerEvents === 'none' && index === 0) {
                blocking.push({
                    element: el,
                    reason: 'pointer-events: none on top element',
                    zIndex: zIndex,
                    position: position
                });
            }
            
            // Check for hidden modals that are blocking
            if (el.classList && el.classList.contains('modal') && !el.classList.contains('show')) {
                const display = window.getComputedStyle(el).display;
                if (display !== 'none') {
                    blocking.push({
                        element: el,
                        reason: 'Hidden modal not properly hidden (display !== none)',
                        zIndex: zIndex,
                        display: display
                    });
                }
            }
            
            if (zIndex > 1000 && position === 'fixed' && el.id !== 'epr-visual-editor') {
                const rect = el.getBoundingClientRect();
                const display = window.getComputedStyle(el).display;
                const visibility = window.getComputedStyle(el).visibility;
                const opacity = window.getComputedStyle(el).opacity;
                
                // Only flag as blocking if actually visible
                if (rect.width > window.innerWidth * 0.9 && 
                    rect.height > window.innerHeight * 0.9 &&
                    display !== 'none' &&
                    visibility !== 'hidden' &&
                    opacity !== '0') {
                    blocking.push({
                        element: el,
                        reason: 'Large fixed element covering viewport',
                        zIndex: zIndex,
                        size: { width: rect.width, height: rect.height },
                        display: display,
                        visibility: visibility,
                        opacity: opacity
                    });
                }
            }
        });
        
        return blocking;
    }

    analyzePage() {
        console.group('[Clickability Debugger] Page Analysis');
        
        // Check for modal backdrops
        const backdrops = document.querySelectorAll('.modal-backdrop');
        console.log('Modal backdrops:', backdrops.length);
        backdrops.forEach((backdrop, i) => {
            const style = window.getComputedStyle(backdrop);
            console.log(`Backdrop ${i}:`, {
                display: style.display,
                visibility: style.visibility,
                opacity: style.opacity,
                zIndex: style.zIndex,
                pointerEvents: style.pointerEvents,
                hasShowClass: backdrop.classList.contains('show')
            });
        });
        
        // Check canvas z-index vs toolbars
        const canvas = document.getElementById('eprCanvas');
        const canvasContainer = document.getElementById('eprCanvasContainer');
        const toolbars = document.querySelectorAll('.epr-floating-toolbar, .epr-types-panel');
        
        if (canvas) {
            const canvasZ = parseInt(window.getComputedStyle(canvas).zIndex) || 0;
            console.log('Canvas z-index:', canvasZ);
        }
        
        if (canvasContainer) {
            const containerZ = parseInt(window.getComputedStyle(canvasContainer).zIndex) || 0;
            console.log('Canvas container z-index:', containerZ);
        }
        
        toolbars.forEach((toolbar, i) => {
            const toolbarZ = parseInt(window.getComputedStyle(toolbar).zIndex) || 0;
            console.log(`Toolbar ${i} z-index:`, toolbarZ, 'ID:', toolbar.id);
        });
        
        // Check for grid overlay
        if (canvas && canvas.classList.contains('grid-visible')) {
            console.warn('⚠️ Grid is visible - checking for overlay');
            const gridStyle = window.getComputedStyle(canvas, '::before');
            console.log('Grid ::before pseudo-element:', gridStyle);
        }
        
        // Check for elements with high z-index covering everything
        const allElements = document.querySelectorAll('*');
        const highZElements = [];
        allElements.forEach(el => {
            const style = window.getComputedStyle(el);
            const zIndex = parseInt(style.zIndex) || 0;
            if (zIndex > 5000 && style.position !== 'static') {
                const rect = el.getBoundingClientRect();
                if (rect.width > window.innerWidth * 0.8 && rect.height > window.innerHeight * 0.8) {
                    highZElements.push({
                        element: el,
                        zIndex: zIndex,
                        position: style.position,
                        pointerEvents: style.pointerEvents,
                        size: { width: rect.width, height: rect.height }
                    });
                }
            }
        });
        
        if (highZElements.length > 0) {
            console.warn('⚠️ Found high z-index elements covering viewport:', highZElements);
        }
        
        console.groupEnd();
    }
}

// Auto-initialize if on visual editor page
if (document.body && document.body.classList.contains('visual-editor-page')) {
    window.clickabilityDebugger = new ClickabilityDebugger();
    console.log('[Clickability Debugger] ✅ Debugger initialized. Check console for click analysis.');
}

