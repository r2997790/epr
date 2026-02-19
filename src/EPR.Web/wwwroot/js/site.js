// Product table row click: use delegation so it works when Products page is loaded in a tab (no Scripts run there)
(function () {
    document.body.addEventListener('click', function (e) {
        var row = e.target && e.target.closest && e.target.closest('.product-row[data-href]');
        if (!row) return;
        var href = row.getAttribute('data-href');
        if (!href) return;
        e.preventDefault();
        if (window.browserTabs && typeof window.browserTabs.addTab === 'function') {
            var title = (row.querySelector('td strong') && row.querySelector('td strong').textContent) ? row.querySelector('td strong').textContent.trim() : 'Product';
            window.browserTabs.addTab(href, title, 'bi-bag', 'Products');
        } else {
            window.location.href = href;
        }
    });
})();







