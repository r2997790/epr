/**
 * Browser Tabs Script Execution Module
 * Handles script extraction, validation, and execution from AJAX-loaded content
 * 
 * This module provides:
 * - Script extraction from HTML
 * - Syntax validation and error fixing
 * - Safe script execution
 * - Critical function detection
 * 
 * Dependencies: browser-tabs-core.js
 */

(function() {
    'use strict';
    
    const DEBUG_MODE = true;
    
    function debugLog(message, data = null) {
        if (DEBUG_MODE) {
            if (data) {
                console.log('[Browser Tabs Scripts]', message, data);
            } else {
                console.log('[Browser Tabs Scripts]', message);
            }
        }
    }
    
    function debugError(message, error = null) {
        if (DEBUG_MODE) {
            if (error) {
                console.error('[Browser Tabs Scripts ERROR]', message, error);
            } else {
                console.error('[Browser Tabs Scripts ERROR]', message);
            }
        }
    }
    
    /**
     * Script Execution Manager Class
     */
    class BrowserTabsScriptExecutor {
        constructor() {
            this.executedScripts = new Set();
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
        
        /**
         * Check if script defines critical functions
         */
        definesCriticalFunctions(scriptContent) {
            if (!scriptContent) return false;
            
            const criticalPatterns = [
                'chartDataConfig',
                'initializeAllCharts',
                'createChart',
                'waitForChartJS',
                'loadDashboardContent',
                'loadAllResourceManagementTablesForStage'
            ];
            
            return criticalPatterns.some(pattern => scriptContent.includes(pattern));
        }
        
        /**
         * Fix missing commas between object properties
         */
        fixMissingCommas(code) {
            let fixed = code;
            let previousFixed = '';
            let iterations = 0;
            const maxIterations = 5;
            
            while (fixed !== previousFixed && iterations < maxIterations) {
                previousFixed = fixed;
                iterations++;
                
                // Pattern 1: Missing comma after closing brace before identifier with colon
                fixed = fixed.replace(/([}\]\)])\s+([a-zA-Z_$][a-zA-Z0-9_$]*\s*:)/g, '$1, $2');
                
                // Pattern 2: Missing comma with newline
                fixed = fixed.replace(/([}\]\)])\s*\r?\n\s*([a-zA-Z_$][a-zA-Z0-9_$]*\s*:)/g, '$1,\n$2');
                
                // Pattern 3: Missing comma after array literal
                fixed = fixed.replace(/(\]\s*\}\s*)([a-zA-Z_$][a-zA-Z0-9_$]*\s*:)/g, '$1, $2');
                
                // Pattern 4: Missing comma after object literal
                fixed = fixed.replace(/(\}\s*)([a-zA-Z_$][a-zA-Z0-9_$]*\s*:)/g, '$1, $2');
                
                // Pattern 5: Missing comma after closing parenthesis
                fixed = fixed.replace(/(\)\s+)([a-zA-Z_$][a-zA-Z0-9_$]*\s*:)/g, '$1, $2');
            }
            
            if (iterations > 1) {
                debugLog(`fixMissingCommas: Applied ${iterations} iterations`);
            }
            
            return fixed;
        }
        
        /**
         * Remove trailing commas
         */
        removeTrailingCommas(code) {
            let cleaned = code;
            let previousCleaned = '';
            let iterations = 0;
            const maxIterations = 10;
            
            while (cleaned !== previousCleaned && iterations < maxIterations) {
                previousCleaned = cleaned;
                iterations++;
                
                // Pattern 1: Remove trailing commas before closing braces
                cleaned = cleaned.replace(/,\s*([}])/g, '$1');
                
                // Pattern 2: Remove trailing commas before closing brackets
                cleaned = cleaned.replace(/,\s*(\])/g, '$1');
                
                // Pattern 3: Remove trailing commas with newline before closing braces
                cleaned = cleaned.replace(/,\s*\r?\n\s*([}])/g, '\n$1');
                
                // Pattern 4: Remove trailing commas with newline before closing brackets
                cleaned = cleaned.replace(/,\s*\r?\n\s*(\])/g, '\n$1');
                
                // Pattern 5: Remove trailing commas before closing parentheses
                cleaned = cleaned.replace(/,\s*\)\s*(\{|=>|;|\r?\n|$)/g, ')$1');
                
                // Pattern 6: Remove trailing commas in destructuring
                cleaned = cleaned.replace(/,\s*([}\]])/g, '$1');
                
                // Pattern 7: Remove trailing commas after last property
                cleaned = cleaned.replace(/([^,}\]]+)\s*,\s*([}\]])/g, '$1$2');
            }
            
            if (iterations > 1) {
                debugLog(`removeTrailingCommas: Applied ${iterations} iterations`);
            }
            
            // Safety check
            const missingCommaPattern = /}\s+[a-zA-Z_$][a-zA-Z0-9_$]*\s*:/;
            if (missingCommaPattern.test(cleaned)) {
                debugError('Trailing comma removal may have removed a comma between properties. Reverting.');
                return code;
            }
            
            return cleaned;
        }
        
        /**
         * Comprehensive syntax fixer
         */
        comprehensiveSyntaxFix(code) {
            let fixed = code;
            
            // Fix "Unexpected token 'else'" - add missing closing brace
            const segments = fixed.split(/\belse\b/);
            if (segments.length > 1) {
                let rebuilt = segments[0];
                for (let i = 1; i < segments.length; i++) {
                    const beforeElse = segments[i - 1];
                    const afterElse = segments[i];
                    
                    const openBraces = (beforeElse.match(/\{/g) || []).length;
                    const closeBraces = (beforeElse.match(/\}/g) || []).length;
                    
                    if (openBraces > closeBraces) {
                        const lastNonWhitespace = beforeElse.trimEnd();
                        if (!lastNonWhitespace.endsWith('}') && !lastNonWhitespace.endsWith(';')) {
                            rebuilt += '}';
                        }
                    }
                    
                    rebuilt += ' else' + afterElse;
                }
                fixed = rebuilt;
            }
            
            // Fix double commas
            fixed = fixed.replace(/,,+/g, ',');
            
            // Fix commas before semicolons
            fixed = fixed.replace(/,\s*;/g, ';');
            
            // Remove trailing commas
            fixed = fixed.replace(/,\s*([}\]\)])/g, '$1');
            fixed = fixed.replace(/,\s*\r?\n\s*([}\]\)])/g, '\n$1');
            
            return fixed;
        }
        
        /**
         * Validate and fix script syntax
         */
        validateAndFixScript(scriptContent, isCritical = false) {
            let cleanedScript = scriptContent;
            let syntaxValid = false;
            const originalScript = scriptContent;
            
            // STEP 1: Validate original script
            try {
                new Function(originalScript);
                syntaxValid = true;
                cleanedScript = originalScript;
                debugLog('Script syntax is valid - using original without modification');
                return { valid: true, script: cleanedScript };
            } catch (originalError) {
                debugError('Script validation failed', originalError.message);
                
                if (!isCritical) {
                    return { valid: false, script: originalScript, error: originalError.message };
                }
                
                const isTrailingCommaError = originalError.message.includes("Unexpected token ','");
                const isMissingCommaError = originalError.message.includes("Unexpected identifier");
                const isElseError = originalError.message.includes("Unexpected token 'else'");
                
                // Try to fix based on error type
                if (isTrailingCommaError) {
                    debugLog('Detected trailing comma error - removing trailing commas');
                    cleanedScript = this.removeTrailingCommas(originalScript);
                    
                    try {
                        new Function(cleanedScript);
                        syntaxValid = true;
                        debugLog('✓ Successfully removed trailing commas');
                        return { valid: true, script: cleanedScript };
                    } catch (fixedError) {
                        debugError('Trailing comma removal failed', fixedError.message);
                    }
                } else if (isMissingCommaError) {
                    debugLog('Detected missing comma error - adding missing commas');
                    cleanedScript = this.fixMissingCommas(originalScript);
                    
                    try {
                        new Function(cleanedScript);
                        syntaxValid = true;
                        debugLog('✓ Successfully added missing commas');
                        return { valid: true, script: cleanedScript };
                    } catch (fixedError) {
                        debugError('Missing comma fix failed', fixedError.message);
                    }
                } else if (isElseError) {
                    debugLog('Detected else error - attempting comprehensive fix');
                    cleanedScript = this.comprehensiveSyntaxFix(originalScript);
                    cleanedScript = this.removeTrailingCommas(cleanedScript);
                    
                    try {
                        new Function(cleanedScript);
                        syntaxValid = true;
                        debugLog('✓ Comprehensive syntax fix succeeded');
                        return { valid: true, script: cleanedScript };
                    } catch (comprehensiveError) {
                        debugError('Comprehensive syntax fix failed', comprehensiveError.message);
                    }
                }
                
                // Last resort for initializeAllCharts
                if (scriptContent.includes('initializeAllCharts')) {
                    debugLog('CRITICAL: Script contains initializeAllCharts - attempting execution with error suppression');
                    try {
                        const wrappedScript = `(function() {
                            try {
                                ${cleanedScript}
                            } catch(e) {
                                console.error('[Browser Tabs Scripts] Error in initializeAllCharts script (suppressed):', e.message);
                            }
                        })();`;
                        eval(wrappedScript);
                        syntaxValid = true;
                        debugLog('✓ initializeAllCharts script executed with error suppression');
                        return { valid: true, script: cleanedScript, suppressed: true };
                    } catch (wrappedError) {
                        debugError('Even wrapped execution failed', wrappedError.message);
                    }
                }
                
                return { valid: false, script: originalScript, error: originalError.message };
            }
        }
        
        /**
         * Execute script safely
         */
        executeScript(scriptContent, isCritical = false) {
            const definesCritical = this.definesCriticalFunctions(scriptContent);
            const shouldFix = isCritical || definesCritical;
            
            const result = this.validateAndFixScript(scriptContent, shouldFix);
            
            if (!result.valid && shouldFix) {
                debugError('Cannot execute script - syntax is invalid', result.error);
                return { success: false, error: result.error };
            }
            
            try {
                if (result.suppressed) {
                    // Already executed in wrapped form
                    debugLog('Script executed successfully (suppressed errors)');
                } else {
                    eval(result.script);
                    debugLog('Script executed successfully' + (definesCritical ? ' (critical functions)' : ''));
                }
                
                // Verify critical functions were defined
                if (definesCritical) {
                    if (scriptContent.includes('chartDataConfig') && typeof window.chartDataConfig === 'undefined') {
                        console.warn('[Browser Tabs Scripts] WARNING: chartDataConfig was not set after script execution');
                    }
                    if (scriptContent.includes('initializeAllCharts') && typeof window.initializeAllCharts !== 'function') {
                        console.warn('[Browser Tabs Scripts] WARNING: initializeAllCharts was not defined after script execution');
                    }
                }
                
                return { success: true };
            } catch (evalError) {
                debugError('Error executing script after validation/fix', evalError);
                return { success: false, error: evalError.message };
            }
        }
        
        /**
         * Process and execute all scripts from HTML content
         */
        processScripts(html, contentArea) {
            const scripts = this.extractScripts(html);
            const results = [];
            
            scripts.forEach((scriptData, index) => {
                if (scriptData.src) {
                    // External script - handle separately
                    debugLog(`Skipping external script: ${scriptData.src}`);
                    return;
                }
                
                if (!scriptData.textContent || !scriptData.textContent.trim()) {
                    debugLog(`Skipping empty script ${index}`);
                    return;
                }
                
                const definesCritical = this.definesCriticalFunctions(scriptData.textContent);
                const result = this.executeScript(scriptData.textContent, definesCritical);
                
                results.push({
                    index,
                    success: result.success,
                    error: result.error,
                    critical: definesCritical
                });
                
                if (result.success) {
                    // Create new script element and replace old one
                    const newScript = document.createElement('script');
                    Object.keys(scriptData.attributes).forEach(key => {
                        newScript.setAttribute(key, scriptData.attributes[key]);
                    });
                    newScript.textContent = scriptData.textContent;
                    
                    // Find and replace old script in contentArea
                    const oldScripts = contentArea.querySelectorAll('script');
                    if (oldScripts[index]) {
                        try {
                            oldScripts[index].parentNode?.replaceChild(newScript, oldScripts[index]);
                        } catch (e) {
                            debugError('Error replacing script', e);
                        }
                    }
                }
            });
            
            return results;
        }
    }
    
    // Export to global scope
    if (typeof window !== 'undefined') {
        window.BrowserTabsScriptExecutor = BrowserTabsScriptExecutor;
        debugLog('BrowserTabsScriptExecutor exported to window');
    }
    
    // Also export for module systems
    if (typeof module !== 'undefined' && module.exports) {
        module.exports = BrowserTabsScriptExecutor;
    }
})();

