// ==========================================================================
// Employee Management System - Global Theme Switcher & Client Utilities
// ==========================================================================

function updateThemeUI(theme) {
    const icon = document.getElementById('themeIcon');
    const btn = document.getElementById('themeToggleBtn');
    if (!icon || !btn) return;

    if (theme === 'dark') {
        icon.className = 'bi bi-sun-fill text-warning fs-5';
        btn.title = 'Switch to Light Mode';
        btn.setAttribute('aria-label', 'Switch to Light Mode');
    } else {
        icon.className = 'bi bi-moon-stars text-secondary fs-5';
        btn.title = 'Switch to Dark Mode';
        btn.setAttribute('aria-label', 'Switch to Dark Mode');
    }
}

function toggleAppTheme() {
    const currentTheme = document.documentElement.getAttribute('data-bs-theme') === 'dark' ? 'dark' : 'light';
    const newTheme = currentTheme === 'dark' ? 'light' : 'dark';

    document.documentElement.setAttribute('data-bs-theme', newTheme);
    if (newTheme === 'dark') {
        document.documentElement.classList.add('dark-theme');
        if (document.body) document.body.classList.add('dark-theme');
    } else {
        document.documentElement.classList.remove('dark-theme');
        if (document.body) document.body.classList.remove('dark-theme');
    }

    try {
        localStorage.setItem('app-theme', newTheme);
    } catch (e) {
        console.warn('localStorage not accessible for theme persistence', e);
    }

    updateThemeUI(newTheme);

    if (window.applyChartTheme) {
        window.applyChartTheme(newTheme === 'dark');
    }

    window.dispatchEvent(new CustomEvent('themeChanged', { detail: { theme: newTheme } }));
}

// Chart.js Theme Synchronization
window.applyChartTheme = function (isDark) {
    if (typeof Chart !== 'undefined' && Chart.defaults) {
        Chart.defaults.color = isDark ? '#94a3b8' : '#64748b';
        if (Chart.defaults.scale && Chart.defaults.scale.grid) {
            Chart.defaults.scale.grid.color = isDark ? 'rgba(255, 255, 255, 0.08)' : 'rgba(0, 0, 0, 0.06)';
        }
        if (typeof Chart.instances !== 'undefined') {
            Object.values(Chart.instances).forEach(chart => {
                try {
                    if (chart && chart.options) {
                        if (chart.options.scales) {
                            Object.values(chart.options.scales).forEach(scale => {
                                if (scale.ticks) scale.ticks.color = isDark ? '#94a3b8' : '#64748b';
                                if (scale.grid) scale.grid.color = isDark ? 'rgba(255, 255, 255, 0.08)' : 'rgba(0, 0, 0, 0.06)';
                            });
                        }
                        chart.update();
                    }
                } catch (err) {
                    // silently handle unmounted chart
                }
            });
        }
    }
};

// Initialize Theme on Page Load
document.addEventListener('DOMContentLoaded', () => {
    const activeTheme = document.documentElement.getAttribute('data-bs-theme') || 'light';
    if (activeTheme === 'dark' && document.body) {
        document.body.classList.add('dark-theme');
    }
    updateThemeUI(activeTheme);
    if (window.applyChartTheme) {
        window.applyChartTheme(activeTheme === 'dark');
    }
});
