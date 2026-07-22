(() => {
    const getApp = () => window.Telegram?.WebApp;
    let backHandler = null;
    let mainHandler = null;

    const initialize = () => {
        const app = getApp();

        if (!app) {
            document.documentElement.dataset.telegram = "preview";
            return;
        }

        document.documentElement.dataset.telegram = "connected";
        document.documentElement.dataset.telegramTheme = app.colorScheme ?? "light";

        app.onEvent?.("themeChanged", () => {
            document.documentElement.dataset.telegramTheme = app.colorScheme ?? "light";
        });

        app.ready();
        app.expand();
    };

    window.nrnTelegram = Object.freeze({
        getInitData: () => getApp()?.initData ?? "",
        getSavedLanguage: () => window.localStorage.getItem("nrn.language"),
        setLanguage: (language) => {
            const normalized = language === "en" ? "en" : "ru";
            window.localStorage.setItem("nrn.language", normalized);
            document.cookie = `nrn.language=${normalized}; Path=/; Max-Age=31536000; SameSite=Lax`;
            document.documentElement.lang = normalized;
        },
        bindChrome: (dotNet, mainButtonText, enableMainButton) => {
            const app = getApp();
            if (!app?.initData) {
                return false;
            }

            if (backHandler) {
                app.BackButton.offClick(backHandler);
            }
            if (mainHandler) {
                app.MainButton.offClick(mainHandler);
            }

            backHandler = () => dotNet.invokeMethodAsync("OnTelegramBack");
            app.BackButton.onClick(backHandler);
            app.BackButton.show();

            if (enableMainButton) {
                mainHandler = () => dotNet.invokeMethodAsync("OnTelegramMain");
                app.MainButton.setParams({
                    text: mainButtonText,
                    is_active: true,
                    is_visible: true
                });
                app.MainButton.onClick(mainHandler);
                app.MainButton.show();
            } else {
                app.MainButton.hide();
            }

            return true;
        },
        unbindChrome: () => {
            const app = getApp();
            if (!app) {
                return;
            }
            if (backHandler) {
                app.BackButton.offClick(backHandler);
                backHandler = null;
            }
            if (mainHandler) {
                app.MainButton.offClick(mainHandler);
                mainHandler = null;
            }
            app.BackButton.hide();
            app.MainButton.hide();
        },
        hapticSuccess: () => getApp()?.HapticFeedback?.notificationOccurred("success"),
        hapticError: () => getApp()?.HapticFeedback?.notificationOccurred("error"),
        openInvoice: (url) => new Promise((resolve) => {
            const app = getApp();
            if (!app?.initData || typeof app.openInvoice !== "function") {
                resolve("unsupported");
                return;
            }

            app.openInvoice(url, (status) => resolve(status ?? "failed"));
        }),
        openLink: (url, isTelegram) => {
            const app = getApp();
            if (app?.initData) {
                if (isTelegram) {
                    app.openTelegramLink(url);
                } else {
                    app.openLink(url);
                }
            } else {
                window.open(url, "_blank", "noopener,noreferrer");
            }
        }
    });

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", initialize, { once: true });
    } else {
        initialize();
    }
})();
