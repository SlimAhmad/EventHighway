// Color-mode (light/dark/auto) helper for the EventHighway portal.
// Mirrors the CoreUI template behaviour (data-coreui-theme on <html>, same localStorage key)
// but exposes an API the Blazor ColorModeSwitcher component calls directly, because the
// interactively-rendered header is not present when the template's DOMContentLoaded wiring runs.
(() => {
    const THEME_KEY = 'coreui-free-bootstrap-admin-template-theme';

    const prefersDark = () =>
        window.matchMedia('(prefers-color-scheme: dark)').matches;

    const storedTheme = () => localStorage.getItem(THEME_KEY);

    const apply = theme => {
        const effective =
            (theme === 'auto' && prefersDark()) ? 'dark' : theme;

        document.documentElement.setAttribute('data-coreui-theme', effective);
        document.documentElement.dispatchEvent(new Event('ColorSchemeChange'));
    };

    // Apply the saved (or system) theme as early as possible to avoid a flash.
    apply(storedTheme() || 'auto');

    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', () => {
        const current = storedTheme();
        if (!current || current === 'auto') {
            apply('auto');
        }
    });

    window.eventHighwayTheme = {
        // The stored MODE ('light' | 'dark' | 'auto'), defaulting to 'auto'.
        getMode: () => storedTheme() || 'auto',
        setMode: theme => {
            localStorage.setItem(THEME_KEY, theme);
            apply(theme);
        }
    };
})();
