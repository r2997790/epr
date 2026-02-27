// Settings (Empauer-style) - client-side preferences
(function() {
    'use strict';
    const SETTINGS_STORAGE_KEY = 'epr-app-settings';
    const defaultSettings = {
        textSize: 14,
        dateFormat: 'YYYY-MM-DD',
        language: 'English'
    };

    function getSettings() {
        try {
            var saved = localStorage.getItem(SETTINGS_STORAGE_KEY);
            if (saved) return Object.assign({}, defaultSettings, JSON.parse(saved));
        } catch (e) {}
        return Object.assign({}, defaultSettings);
    }

    function saveSettings(settings) {
        try {
            localStorage.setItem(SETTINGS_STORAGE_KEY, JSON.stringify(settings));
        } catch (e) {}
    }

    window.showSettingsDialog = function() {
        var settings = getSettings();
        var modalHtml =
            '<div class="modal fade" id="settingsModal" tabindex="-1">' +
            '  <div class="modal-dialog modal-lg modal-dialog-centered">' +
            '    <div class="modal-content">' +
            '      <div class="modal-header">' +
            '        <h5 class="modal-title">Settings</h5>' +
            '        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>' +
            '      </div>' +
            '      <div class="modal-body">' +
            '        <ul class="nav nav-tabs mb-3" role="tablist">' +
            '          <li class="nav-item"><button class="nav-link active" data-bs-toggle="tab" data-bs-target="#settings-appearance-panel" type="button">Appearance</button></li>' +
            '          <li class="nav-item"><button class="nav-link" data-bs-toggle="tab" data-bs-target="#settings-formatting-panel" type="button">Formatting</button></li>' +
            '        </ul>' +
            '        <div class="tab-content">' +
            '          <div class="tab-pane fade show active" id="settings-appearance-panel">' +
            '            <div class="mb-3">' +
            '              <label class="form-label">Text size</label>' +
            '              <input type="range" class="form-range" id="settingsTextSize" min="10" max="18" step="1" value="' + (settings.textSize || 12) + '">' +
            '              <div class="d-flex justify-content-between small text-muted"><span>10px</span><span id="settingsTextSizeValue">' + (settings.textSize || 12) + 'px</span><span>18px</span></div>' +
            '            </div>' +
            '          </div>' +
            '          <div class="tab-pane fade" id="settings-formatting-panel">' +
            '            <div class="mb-3">' +
            '              <label class="form-label">Date format</label>' +
            '              <select class="form-select" id="settingsDateFormat">' +
            '                <option value="YYYY-MM-DD"' + ((settings.dateFormat || 'YYYY-MM-DD') === 'YYYY-MM-DD' ? ' selected' : '') + '>YYYY-MM-DD</option>' +
            '                <option value="DD-MM-YYYY"' + (settings.dateFormat === 'DD-MM-YYYY' ? ' selected' : '') + '>DD-MM-YYYY</option>' +
            '                <option value="DD MMM YYYY"' + (settings.dateFormat === 'DD MMM YYYY' ? ' selected' : '') + '>DD MMM YYYY</option>' +
            '              </select>' +
            '            </div>' +
            '            <div class="mb-3">' +
            '              <label class="form-label" data-translate="Language">Language</label>' +
            '              <select class="form-select" id="settingsLanguage">' +
            '                <option value="English"' + ((settings.language || 'English') === 'English' ? ' selected' : '') + '>English</option>' +
            '                <option value="Turkish"' + (settings.language === 'Turkish' ? ' selected' : '') + '>Türkçe</option>' +
            '                <option value="French"' + (settings.language === 'French' ? ' selected' : '') + '>Français</option>' +
            '                <option value="German"' + (settings.language === 'German' ? ' selected' : '') + '>Deutsch</option>' +
            '                <option value="Spanish"' + (settings.language === 'Spanish' ? ' selected' : '') + '>Español</option>' +
            '                <option value="Chinese"' + (settings.language === 'Chinese' ? ' selected' : '') + '>中文</option>' +
            '                <option value="Japanese"' + (settings.language === 'Japanese' ? ' selected' : '') + '>日本語</option>' +
            '                <option value="Italian"' + (settings.language === 'Italian' ? ' selected' : '') + '>Italiano</option>' +
            '                <option value="Russian"' + (settings.language === 'Russian' ? ' selected' : '') + '>Русский</option>' +
            '              </select>' +
            '            </div>' +
            '          </div>' +
            '        </div>' +
            '      </div>' +
            '      <div class="modal-footer">' +
            '        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>' +
            '        <button type="button" class="btn btn-primary" id="settingsSaveBtn">Save</button>' +
            '      </div>' +
            '    </div>' +
            '  </div>' +
            '</div>';
        var existing = document.getElementById('settingsModal');
        if (existing) existing.remove();
        document.body.insertAdjacentHTML('beforeend', modalHtml);

        var modalEl = document.getElementById('settingsModal');
        var textSizeEl = document.getElementById('settingsTextSize');
        var textSizeValueEl = document.getElementById('settingsTextSizeValue');
        if (textSizeEl && textSizeValueEl) {
            textSizeEl.addEventListener('input', function() { textSizeValueEl.textContent = this.value + 'px'; });
        }
        document.getElementById('settingsSaveBtn').addEventListener('click', function() {
            var s = {
                textSize: parseInt(document.getElementById('settingsTextSize').value, 10) || 14,
                dateFormat: document.getElementById('settingsDateFormat').value || 'YYYY-MM-DD',
                language: document.getElementById('settingsLanguage').value || 'English'
            };
            saveSettings(s);
            document.documentElement.style.fontSize = s.textSize + 'px';
            if (window.localization && window.localization.load) {
                window.localization.load(s.language).then(function() {
                    if (window.localization.apply) window.localization.apply();
                });
            }
            var modal = bootstrap.Modal.getInstance(modalEl);
            if (modal) modal.hide();
        });
        var modal = new bootstrap.Modal(modalEl);
        modal.show();
    };

    // Apply saved text size on load
    (function applySavedSettings() {
        var s = getSettings();
        if (s.textSize) document.documentElement.style.fontSize = s.textSize + 'px';
    })();
})();
