// Localization – 8 languages (en, tr, fr, de, es, zh, ja, it, ru)
(function() {
    'use strict';
    var currentLanguage = 'English';
    var translations = {};
    var defaultTranslations = {};
    var LANGUAGE_MAP = {
        'English': 'en',
        'Turkish': 'tr',
        'French': 'fr',
        'German': 'de',
        'Spanish': 'es',
        'Chinese': 'zh',
        'Japanese': 'ja',
        'Italian': 'it',
        'Russian': 'ru'
    };

    function getStoredLanguage() {
        try {
            var raw = localStorage.getItem('epr-app-settings');
            if (raw) {
                var s = JSON.parse(raw);
                if (s && s.language) return s.language;
            }
        } catch (e) {}
        return null;
    }

    function loadJson(url) {
        return fetch(url).then(function(r) { return r.ok ? r.json() : null; });
    }

    function loadDefault() {
        return loadJson('/locales/en.json').then(function(data) {
            if (data) {
                defaultTranslations = data;
                translations = Object.assign({}, data);
                return true;
            }
            return false;
        });
    }

    function loadLanguage(lang) {
        if (lang === 'English' || lang === 'en') {
            translations = Object.assign({}, defaultTranslations);
            currentLanguage = 'English';
            return Promise.resolve(true);
        }
        var code = LANGUAGE_MAP[lang] || (lang && lang.length === 2 ? lang : 'en');
        return loadJson('/locales/' + code + '.json').then(function(data) {
            if (data) {
                translations = Object.assign({}, defaultTranslations, data);
                currentLanguage = lang;
                return true;
            }
            translations = Object.assign({}, defaultTranslations);
            currentLanguage = 'English';
            return false;
        }).catch(function() {
            translations = Object.assign({}, defaultTranslations);
            currentLanguage = 'English';
            return false;
        });
    }

    function translate(key, fallback) {
        if (translations[key]) return translations[key];
        if (defaultTranslations[key]) return defaultTranslations[key];
        return fallback !== undefined ? fallback : key;
    }

    function apply(container) {
        var root = container || document;
        root.querySelectorAll('[data-translate]').forEach(function(el) {
            var key = el.getAttribute('data-translate');
            if (!key) return;
            var text = translate(key);
            if (!text) return;
            if (el.children.length > 0) {
                var first = el.firstChild;
                while (first && first.nodeType !== 3) first = first.nextSibling;
                if (first) first.textContent = text;
            } else {
                el.textContent = text;
            }
        });
        root.querySelectorAll('[data-translate-placeholder]').forEach(function(el) {
            var key = el.getAttribute('data-translate-placeholder');
            var text = translate(key);
            if (text !== key) el.setAttribute('placeholder', text);
        });
        root.querySelectorAll('[data-translate-title]').forEach(function(el) {
            var key = el.getAttribute('data-translate-title');
            var text = translate(key);
            if (text !== key) el.setAttribute('title', text);
        });
    }

    function init() {
        return loadDefault().then(function() {
            var stored = getStoredLanguage();
            if (stored) return loadLanguage(stored);
            return Promise.resolve();
        }).then(function() {
            function run() {
                setTimeout(function() { apply(); }, 50);
            }
            if (document.readyState === 'loading') {
                document.addEventListener('DOMContentLoaded', run);
            } else {
                run();
            }
        });
    }

    window.localization = {
        load: loadLanguage,
        translate: translate,
        apply: apply,
        getCurrentLanguage: function() { return currentLanguage; },
        init: init,
        LANGUAGE_MAP: LANGUAGE_MAP
    };
    window.applyTranslations = apply;
    init();
})();
