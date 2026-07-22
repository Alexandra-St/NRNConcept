using System.Globalization;
using NRN.Telegram.Features.Services.Contracts;
using NRN.Telegram.Telegram;

namespace NRN.Telegram.Localization;

public sealed class MiniAppLocalizer(MiniAppPreferencesState preferences)
{
    private static IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Resources =>
        new Dictionary<string, IReadOnlyDictionary<string, string>>(StringComparer.Ordinal)
        {
            ["ru"] = Russian,
            ["en"] = English
        };


    private string Language => preferences.Language;
    private bool IsEnglish => Language == "en";
    private static IReadOnlyCollection<string> Keys => Russian.Keys.ToArray();

    public string this[string key] => Resources[Language].TryGetValue(key, out var value) ? value : key;

    public string Format(string key, params object?[] arguments) =>
        string.Format(CultureInfo.InvariantCulture, this[key], arguments);

    public string ProductName(string productId, string fallback) =>
        TranslateProduct(productId, "name", fallback);

    public string ProductDescription(string productId, string fallback) =>
        TranslateProduct(productId, "description", fallback);

    public string ServiceStatus(ConnectedServiceStatus status) => this[status switch
    {
        ConnectedServiceStatus.Active => "status.active",
        ConnectedServiceStatus.Paused => "status.paused",
        ConnectedServiceStatus.Expired => "status.expired",
        _ => "status.unknown"
    }];

    public string ServiceStatusDetail(string detail)
    {
        if (!IsEnglish)
        {
            return detail;
        }

        if (detail.StartsWith("баланс ", StringComparison.OrdinalIgnoreCase))
        {
            return $"balance {detail[7..]}";
        }

        if (detail.StartsWith("оплачено ", StringComparison.OrdinalIgnoreCase))
        {
            return $"paid with {detail[9..]}";
        }

        return detail;
    }

    public static IReadOnlyCollection<string> MissingKeys(string language) =>
        !Resources.TryGetValue(language, out var resource)
            ? Keys.ToArray()
            : Keys.Where(key => !resource.ContainsKey(key)).ToArray();

    private string TranslateProduct(string productId, string field, string fallback)
    {
        var key = $"product.{productId}.{field}";
        return Resources[Language].TryGetValue(key, out var value) ? value : fallback;
    }

    private static readonly IReadOnlyDictionary<string, string> Russian = new Dictionary<string, string>
    {
        ["common.loading"] = "Загружаем данные…",
        ["common.retry"] = "Повторить",
        ["common.cancel"] = "Отмена",
        ["common.close"] = "Закрыть",
        ["common.month"] = "мес",
        ["common.minutes"] = "3 мин",
        ["data.errorOffline"] = "Похоже, нет подключения к интернету. Проверьте сеть и попробуйте снова.",
        ["data.errorTimeout"] = "Сервер отвечает слишком долго. Попробуйте ещё раз.",
        ["data.errorUnauthorized"] = "Telegram-сессия больше не действует. Закройте Mini App и откройте его снова.",
        ["data.errorServer"] = "Сервис временно недоступен. Попробуйте ещё раз чуть позже.",
        ["data.errorUnknown"] = "Не удалось загрузить данные. Попробуйте ещё раз.",
        ["nav.label"] = "Основная навигация",
        ["nav.home"] = "Главная",
        ["nav.services"] = "Услуги",
        ["nav.buy"] = "Купить",
        ["nav.learn"] = "Узнать",
        ["nav.help"] = "Помощь",
        ["layout.skip"] = "Перейти к содержанию",
        ["layout.sessionRejected"] = "Не удалось подтвердить Telegram-сессию. Закройте и снова откройте Mini App.",
        ["layout.error"] = "Не удалось продолжить работу.",
        ["layout.reload"] = "Обновить",
        ["layout.dismiss"] = "Закрыть",
        ["home.title"] = "NRN — Главная",
        ["home.account"] = "Мой аккаунт",
        ["home.connected"] = "Подключённые услуги",
        ["home.checkingConnected"] = "Проверяем подключённые услуги…",
        ["home.myServices"] = "Мои услуги",
        ["home.all"] = "Все",
        ["home.empty"] = "У вас пока нет подключённых услуг. Выберите подходящий план в каталоге.",
        ["home.unauthorized"] = "Услуги доступны после подтверждения Telegram-сессии.",
        ["home.error"] = "Не удалось загрузить услуги. Попробуйте открыть Mini App снова.",
        ["home.labTitle"] = "Безопасен ли публичный Wi-Fi?",
        ["home.labDescription"] = "Пройдите короткую интерактивную симуляцию",
        ["home.labAction"] = "Начать · 3 мин",
        ["home.summary.none"] = "Услуг пока нет",
        ["home.summary.working"] = "Все услуги активны",
        ["home.summary.check"] = "Проверьте услуги",
        ["home.summary.telegram"] = "Требуется Telegram",
        ["home.summary.unavailable"] = "Статус недоступен",
        ["home.summary.checking"] = "Проверяем услуги",
        ["home.summary.openCatalog"] = "Откройте каталог, чтобы выбрать план",
        ["home.summary.oneActive"] = "1 услуга активна",
        ["home.summary.oneAttention"] = "1 услуга требует внимания",
        ["home.summary.many"] = "{0} из {1} услуг активны",
        ["home.greeting"] = "Здравствуйте",
        ["home.greetingNamed"] = "Здравствуйте, {0}",
        ["status.active"] = "Активна",
        ["status.paused"] = "Приостановлена",
        ["status.expired"] = "Истекла",
        ["status.unknown"] = "Неизвестно",
        ["account.title"] = "NRN — Мой аккаунт",
        ["account.eyebrow"] = "МОЙ АККАУНТ",
        ["account.description"] = "Данные аккаунта из подтверждённой Telegram-сессии.",
        ["account.heading"] = "Мой аккаунт",
        ["account.previewDescription"] = "Демонстрационные данные показывают, как выглядит аккаунт внутри Telegram.",
        ["account.loadingDescription"] = "Подтверждаем Telegram-сессию и загружаем данные аккаунта.",
        ["account.loading"] = "Загружаем аккаунт…",
        ["account.unavailableHeading"] = "Аккаунт недоступен",
        ["account.unavailableDescription"] = "Закройте Mini App и откройте его снова из Narayana Bot.",
        ["account.demoUser"] = "Demo User",
        ["account.demoMode"] = "Демо-режим",
        ["account.verifiedMode"] = "Telegram подтверждён",
        ["account.telegramUser"] = "Пользователь Telegram",
        ["account.balance"] = "Баланс",
        ["account.balanceHint"] = "Баланс используется для подключения услуг Narayana.",
        ["account.details"] = "Данные аккаунта",
        ["account.connectedServices"] = "Подключённые услуги",
        ["account.telegramId"] = "Telegram ID",
        ["account.actions"] = "Действия аккаунта",
        ["account.openServices"] = "Мои услуги",
        ["account.openServicesHint"] = "Статусы подключённых услуг",
        ["account.languageHint"] = "Язык интерфейса",
        ["account.usernameNotSet"] = "Username не указан",
        ["account.notAvailable"] = "Недоступно",
        ["account.demoEnvironment"] = "Демо-окружение",
        ["account.language"] = "Язык",
        ["account.currentLanguage"] = "Русский →",
        ["language.title"] = "NRN — Язык",
        ["language.eyebrow"] = "ЯЗЫК",
        ["language.heading"] = "Язык приложения",
        ["language.description"] = "Выбор сохранится для следующих запусков Mini App.",
        ["services.title"] = "NRN — Мои услуги",
        ["services.heading"] = "Мои услуги",
        ["services.description"] = "Подключённые услуги, их статус и сроки действия.",
        ["services.validity"] = "Состояние услуги",
        ["services.add"] = "Добавить услугу",
        ["services.emptyTitle"] = "У вас пока нет подключённых услуг",
        ["services.empty"] = "Выберите подходящую услугу в каталоге — она появится здесь после подключения.",
        ["services.choose"] = "Выбрать услугу",
        ["services.unauthorized"] = "Откройте приложение из Telegram, чтобы увидеть подключённые услуги.",
        ["services.errorTitle"] = "Не удалось загрузить данные",
        ["services.error"] = "Проверьте подключение и попробуйте снова.",
        ["services.loading"] = "Загружаем подключённые услуги…",
        ["services.refreshError"] = "Не удалось обновить услуги. Показаны ранее загруженные данные.",
        ["catalog.title"] = "NRN — Каталог услуг",
        ["catalog.eyebrow"] = "УСЛУГИ",
        ["catalog.heading"] = "Выберите план",
        ["catalog.description"] = "Продукты и публичные цены Narayana в Mini App.",
        ["catalog.popular"] = "В фокусе",
        ["catalog.connection"] = "Подключение",
        ["catalog.monthly"] = "Абонентская плата",
        ["catalog.oneTime"] = "Единоразово",
        ["catalog.from"] = "от",
        ["catalog.sourceLabel"] = "Источник цен",
        ["catalog.source"] = "Публичные цены Narayana · проверено {0}",
        ["catalog.checkPrice"] = "Проверить актуальную цену",
        ["catalog.error"] = "Не удалось загрузить каталог. Попробуйте ещё раз позже.",
        ["catalog.unauthorized"] = "Откройте приложение из Telegram, чтобы перейти к покупке.",
        ["catalog.loading"] = "Загружаем каталог…",
        ["catalog.emptyTitle"] = "Каталог пока пуст",
        ["catalog.emptyDescription"] = "Сейчас нет доступных предложений. Обновите каталог через несколько минут.",
        ["catalog.errorTitle"] = "Не удалось загрузить каталог",
        ["catalog.refreshError"] = "Не удалось обновить каталог. Показаны ранее загруженные данные.",
        ["details.serviceFallback"] = "Услуга",
        ["details.priceLabel"] = "Стоимость услуги",
        ["details.once"] = "единоразово",
        ["details.perMonth"] = "в месяц",
        ["details.includes"] = "Что входит",
        ["details.mobileInternet"] = "Мобильный интернет без номера телефона",
        ["details.mobileNumber"] = "Мобильный номер",
        ["details.smsCalls"] = "SMS и звонки",
        ["details.noMonthlyFee"] = "Без абонентской платы",
        ["details.manage"] = "Управление услугой в Mini App",
        ["details.connected"] = "Услуга подключена",
        ["details.appeared"] = "{0} появилась в «Моих услугах».",
        ["details.openServices"] = "Открыть мои услуги →",
        ["details.insufficient"] = "Недостаточно средств",
        ["details.balanceRequired"] = "На балансе {0}, для подключения нужно {1}.",
        ["details.topUp"] = "Пополните баланс через Narayana Bot и повторите покупку.",
        ["details.openingTelegram"] = "Открываем Telegram…",
        ["details.payStars"] = "Оплатить Telegram Stars",
        ["details.notFound"] = "Такого плана нет в каталоге.",
        ["details.loading"] = "Загружаем план…",
        ["details.cancelPurchase"] = "Отменить покупку",
        ["details.confirmation"] = "ПОДТВЕРЖДЕНИЕ",
        ["details.connectQuestion"] = "Подключить {0}?",
        ["details.yourBalance"] = "Ваш баланс",
        ["details.chargeAfterConfirm"] = "Средства спишутся только после подтверждения.",
        ["details.connecting"] = "Подключаем…",
        ["details.confirm"] = "Подтвердить · {0}",
        ["details.connectedButton"] = "Подключено",
        ["details.connectFor"] = "Подключить за {0}",
        ["details.kindData"] = "ESIM · ТОЛЬКО ДАННЫЕ",
        ["details.kindMobile"] = "МОБИЛЬНЫЙ НОМЕР",
        ["details.kindVirtual"] = "ВИРТУАЛЬНЫЕ НОМЕРА",
        ["details.kindEsim"] = "ESIM",
        ["details.kindPhysical"] = "ФИЗИЧЕСКАЯ SIM",
        ["details.virtualTypes"] = "Мобильные и бесплатные номера",
        ["details.voiceSms"] = "Поддержка голосовой связи и SMS",
        ["details.countries"] = "Более 20 доступных стран",
        ["details.dataVoice"] = "eSIM для мобильного интернета и голосовой связи",
        ["details.noPhysicalCard"] = "Без физической SIM-карты",
        ["details.instantActivation"] = "Цифровая доставка и быстрая активация",
        ["details.fullSim"] = "Полнофункциональная физическая SIM-карта",
        ["details.dataCallsSms"] = "Интернет, звонки и SMS",
        ["details.deliverySeparate"] = "Стоимость доставки не включена",
        ["details.demoPurchase"] = "Демонстрационная покупка использует тестовый баланс и не создаёт реальную услугу Narayana.",
        ["details.livePurchasePending"] = "Покупка внутри Telegram пока недоступна. Проверьте актуальные условия в официальном прайсе.",
        ["details.demoConnectFor"] = "Демо-покупка · {0}",
        ["stars.priceMissing"] = "Цена в Stars пока не настроена для этого тарифа.",
        ["stars.webhookPending"] = "Платёж принят. Услуга появится после подтверждения webhook.",
        ["stars.cancelled"] = "Оплата отменена — услуга не подключена.",
        ["stars.failed"] = "Платёж не завершён. Попробуйте ещё раз.",
        ["stars.openFailed"] = "Не удалось открыть оплату Stars.",
        ["purchase.telegramRequired"] = "Для покупки требуется подтверждённая Telegram-сессия.",
        ["purchase.unavailable"] = "Услуга недоступна.",
        ["purchase.failed"] = "Не удалось подключить услугу.",
        ["purchase.offline"] = "Нет подключения к интернету. Проверьте сеть и попробуйте снова.",
        ["purchase.timeout"] = "Подключение занимает слишком много времени. Проверьте статус услуги перед повторной попыткой.",
        ["purchase.conflict"] = "Это действие уже выполнено. Обновите список услуг.",
        ["payment.orderInvalid"] = "Заказ устарел или уже оплачен.",
        ["learn.title"] = "NRN — Privacy Lab",
        ["learn.heading"] = "Приватность — это навык",
        ["learn.description"] = "Учитесь распознавать цифровые риски через короткие интерактивные сценарии.",
        ["learn.first"] = "ПЕРВАЯ СИМУЛЯЦИЯ",
        ["learn.wifi"] = "Публичный Wi-Fi",
        ["learn.scenario"] = "Вы в аэропорту, телефон почти разряжен, а нужно срочно отправить документ. Как поступить безопасно?",
        ["learn.start"] = "Начать симуляцию",
        ["learn.coming"] = "Новые сценарии появятся после запуска первого идеального опыта.",
        ["help.title"] = "NRN — Помощь",
        ["help.eyebrow"] = "ПОДДЕРЖКА",
        ["help.heading"] = "Чем помочь?",
        ["help.description"] = "Ответы на частые вопросы и официальные контакты Narayana.",
        ["help.faq"] = "Частые вопросы",
        ["help.connectQ"] = "Как подключить услугу?",
        ["help.connectA"] = "Откройте раздел «Купить», выберите услугу и подтвердите подключение.",
        ["help.statusQ"] = "Где проверить статус услуги?",
        ["help.statusA"] = "Все активные услуги и их состояние находятся в разделе «Мои услуги».",
        ["help.cancelQ"] = "Как отменить подписку?",
        ["help.cancelA"] = "Напишите в поддержку — команда поможет управлять подпиской.",
        ["help.need"] = "Нужна помощь?",
        ["help.supportText"] = "Откройте официальную страницу Narayana в Telegram или напишите на support@narayana.im.",
        ["help.openSupport"] = "Открыть страницу в Telegram",
        ["help.feedback"] = "Обратная связь",
        ["help.opensTelegram"] = "Откроется в Telegram",
        ["help.opensBrowser"] = "Откроется во внешнем браузере",
        ["help.notConfigured"] = "Ссылка пока не настроена",
        ["help.linksPending"] = "Ссылка для обратной связи пока недоступна. Для помощи напишите на support@narayana.im.",
        ["error.title"] = "Ошибка",
        ["error.heading"] = "Что-то пошло не так",
        ["error.description"] = "Обновите Mini App и попробуйте ещё раз.",
        ["error.back"] = "Вернуться в приложение",
        ["demo.title"] = "Narayana Bot — Демо-окружение",
        ["demo.label"] = "Демо-окружение",
        ["demo.heading"] = "От Narayana Bot к Mini App",
        ["demo.description"] = "Симуляция существующего Telegram-сценария. Это не интерфейс Telegram и не настоящий чат.",
        ["demo.openDirect"] = "Открыть Mini App напрямую",
        ["demo.phoneLabel"] = "Симуляция Narayana Bot",
        ["demo.chatLabel"] = "Демонстрационный чат",
        ["demo.today"] = "Сегодня",
        ["demo.welcome"] = "👋 Добро пожаловать в Narayana!",
        ["demo.logged"] = "🔑 Выполнен вход как",
        ["demo.balance"] = "💰 Демо-баланс: 12 EUR",
        ["demo.private"] = "🔐 Продукты из публичного каталога Narayana:",
        ["demo.products"] = "eSIM — интернет и голосовая связь; доступность зависит от направления.<br />Виртуальные номера — голосовая связь и SMS более чем в 20 странах.",
        ["demo.productsEsim"] = "eSIM — интернет и голосовая связь; доступность зависит от направления.",
        ["demo.productsMobile"] = "Виртуальные номера — голосовая связь и SMS более чем в 20 странах.",
        ["demo.choose"] = "Выберите план.",
        ["demo.data"] = "🌐 eSIM — от 9 EUR, цифровая доставка.",
        ["demo.mobile"] = "☎️ Виртуальные номера — от 10 EUR в месяц; цена зависит от страны.",
        ["demo.commands"] = "Команды бота",
        ["demo.account"] = "👤 Мой аккаунт",
        ["demo.services"] = "📦 Услуги",
        ["demo.prices"] = "💰 Цены",
        ["demo.help"] = "❓ Помощь",
        ["demo.openApp"] = "Открыть Narayana App",
        ["demo.openAppHint"] = "Каталог, аккаунт и подключённые услуги",
        ["demo.message"] = "Сообщение",
        ["demo.dialog"] = "Narayana Mini App",
        ["demo.close"] = "Закрыть Mini App",
        ["demo.frame"] = "Демо Narayana Mini App",
        ["product.virtual-numbers.name"] = "Виртуальные номера",
        ["product.virtual-numbers.description"] = "Мобильные и бесплатные номера с голосовой связью и SMS более чем в 20 странах.",
        ["product.virtual-numbers.priceNote"] = "Цена зависит от страны",
        ["product.esim.name"] = "eSIM",
        ["product.esim.description"] = "eSIM для интернета и голосовой связи с мгновенной цифровой доставкой.",
        ["product.esim.priceNote"] = "Мгновенная цифровая доставка",
        ["product.physical-sim.name"] = "Физическая SIM",
        ["product.physical-sim.description"] = "Полнофункциональная SIM-карта с интернетом, звонками и SMS. Доставка оплачивается отдельно.",
        ["product.physical-sim.priceNote"] = "Доставка не включена"
    };

    private static readonly IReadOnlyDictionary<string, string> English = new Dictionary<string, string>
    {
        ["common.loading"] = "Loading data…", ["common.retry"] = "Try again", ["common.cancel"] = "Cancel", ["common.close"] = "Close", ["common.month"] = "mo", ["common.minutes"] = "3 min", ["data.errorOffline"] = "You appear to be offline. Check your connection and try again.", ["data.errorTimeout"] = "The server is taking too long to respond. Try again.", ["data.errorUnauthorized"] = "Your Telegram session has expired. Close and reopen the Mini App.", ["data.errorServer"] = "The service is temporarily unavailable. Try again a little later.", ["data.errorUnknown"] = "We couldn't load the data. Try again.",
        ["nav.label"] = "Main navigation", ["nav.home"] = "Home", ["nav.services"] = "Services", ["nav.buy"] = "Buy", ["nav.learn"] = "Learn", ["nav.help"] = "Help",
        ["layout.skip"] = "Skip to content", ["layout.sessionRejected"] = "We couldn't verify your Telegram session. Close and reopen the Mini App.", ["layout.error"] = "We couldn't continue.", ["layout.reload"] = "Reload", ["layout.dismiss"] = "Dismiss",
        ["home.title"] = "NRN — Home", ["home.account"] = "My account", ["home.connected"] = "Connected services", ["home.checkingConnected"] = "Checking connected services…", ["home.myServices"] = "My services", ["home.all"] = "All", ["home.empty"] = "You don't have any connected services yet. Choose a plan in the catalog.", ["home.unauthorized"] = "Services are available after your Telegram session is verified.", ["home.error"] = "We couldn't load your services. Try reopening the Mini App.", ["home.labTitle"] = "Is public Wi-Fi safe?", ["home.labDescription"] = "Try a short interactive simulation", ["home.labAction"] = "Start · 3 min", ["home.summary.none"] = "No services yet", ["home.summary.working"] = "All services are active", ["home.summary.check"] = "Check your services", ["home.summary.telegram"] = "Telegram required", ["home.summary.unavailable"] = "Status unavailable", ["home.summary.checking"] = "Checking services", ["home.summary.openCatalog"] = "Open the catalog to choose a plan", ["home.summary.oneActive"] = "1 service is active", ["home.summary.oneAttention"] = "1 service needs attention", ["home.summary.many"] = "{0} of {1} services are active", ["home.greeting"] = "Hello", ["home.greetingNamed"] = "Hello, {0}",
        ["status.active"] = "Active", ["status.paused"] = "Paused", ["status.expired"] = "Expired", ["status.unknown"] = "Unknown",
        ["account.title"] = "NRN — My account", ["account.eyebrow"] = "MY ACCOUNT", ["account.description"] = "Account data from your verified Telegram session.", ["account.heading"] = "My account", ["account.previewDescription"] = "Demo data shows how your account will look inside Telegram.", ["account.loadingDescription"] = "Verifying your Telegram session and loading account data.", ["account.loading"] = "Loading account…", ["account.unavailableHeading"] = "Account unavailable", ["account.unavailableDescription"] = "Close the Mini App and reopen it from Narayana Bot.", ["account.demoUser"] = "Demo User", ["account.demoMode"] = "Demo mode", ["account.verifiedMode"] = "Telegram verified", ["account.telegramUser"] = "Telegram user", ["account.balance"] = "Balance", ["account.balanceHint"] = "Your balance is used to connect Narayana services.", ["account.details"] = "Account details", ["account.connectedServices"] = "Connected services", ["account.telegramId"] = "Telegram ID", ["account.actions"] = "Account actions", ["account.openServices"] = "My services", ["account.openServicesHint"] = "Connected service status", ["account.languageHint"] = "Interface language", ["account.usernameNotSet"] = "Username not set", ["account.notAvailable"] = "Not available", ["account.demoEnvironment"] = "Demo environment", ["account.language"] = "Language", ["account.currentLanguage"] = "English →",
        ["language.title"] = "NRN — Language", ["language.eyebrow"] = "LANGUAGE", ["language.heading"] = "App language", ["language.description"] = "Your choice will be saved for future Mini App sessions.",
        ["services.title"] = "NRN — My services", ["services.heading"] = "My services", ["services.description"] = "Your connected services, their status and validity.", ["services.validity"] = "Service status", ["services.add"] = "Add a service", ["services.emptyTitle"] = "You don't have any connected services yet", ["services.empty"] = "Choose a suitable service in the catalog — it will appear here after connection.", ["services.choose"] = "Choose a service", ["services.unauthorized"] = "Open the app from Telegram to see your connected services.", ["services.errorTitle"] = "We couldn't load the data", ["services.error"] = "Check your connection and try again.", ["services.loading"] = "Loading connected services…", ["services.refreshError"] = "We couldn't refresh your services. Previously loaded data is still shown.",
        ["catalog.title"] = "NRN — Services catalog", ["catalog.eyebrow"] = "SERVICES", ["catalog.heading"] = "Choose your plan", ["catalog.description"] = "Narayana products and public prices in the Mini App.", ["catalog.popular"] = "Featured", ["catalog.connection"] = "Connection", ["catalog.monthly"] = "Monthly fee", ["catalog.oneTime"] = "One-time", ["catalog.from"] = "from", ["catalog.sourceLabel"] = "Price source", ["catalog.source"] = "Public Narayana prices · verified {0}", ["catalog.checkPrice"] = "Check current price", ["catalog.error"] = "Check your connection and try again.", ["catalog.unauthorized"] = "Open the app from Telegram to continue to purchase.", ["catalog.loading"] = "Loading catalog…", ["catalog.emptyTitle"] = "The catalog is empty", ["catalog.emptyDescription"] = "No offers are available right now. Refresh the catalog in a few minutes.", ["catalog.errorTitle"] = "We couldn't load the catalog", ["catalog.refreshError"] = "We couldn't refresh the catalog. Previously loaded data is still shown.",
        ["details.serviceFallback"] = "Service", ["details.priceLabel"] = "Service price", ["details.once"] = "one-time", ["details.perMonth"] = "per month", ["details.includes"] = "What's included", ["details.mobileInternet"] = "Mobile internet without a phone number", ["details.mobileNumber"] = "Mobile number", ["details.smsCalls"] = "SMS and calls", ["details.noMonthlyFee"] = "No monthly fee", ["details.manage"] = "Manage your service in the Mini App", ["details.connected"] = "Service connected", ["details.appeared"] = "{0} is now in My services.", ["details.openServices"] = "Open my services →", ["details.insufficient"] = "Insufficient balance", ["details.balanceRequired"] = "Your balance is {0}; you need {1} to connect.", ["details.topUp"] = "Top up through Narayana Bot and try again.", ["details.openingTelegram"] = "Opening Telegram…", ["details.payStars"] = "Pay with Telegram Stars", ["details.notFound"] = "This plan isn't in the catalog.", ["details.loading"] = "Loading plan…", ["details.cancelPurchase"] = "Cancel purchase", ["details.confirmation"] = "CONFIRMATION", ["details.connectQuestion"] = "Connect {0}?", ["details.yourBalance"] = "Your balance", ["details.chargeAfterConfirm"] = "Funds will only be charged after confirmation.", ["details.connecting"] = "Connecting…", ["details.confirm"] = "Confirm · {0}", ["details.connectedButton"] = "Connected", ["details.connectFor"] = "Connect for {0}", ["details.kindData"] = "ESIM · DATA ONLY", ["details.kindMobile"] = "MOBILE NUMBER", ["details.kindVirtual"] = "VIRTUAL NUMBERS", ["details.kindEsim"] = "ESIM", ["details.kindPhysical"] = "PHYSICAL SIM", ["details.virtualTypes"] = "Mobile and toll-free numbers", ["details.voiceSms"] = "Voice and SMS support", ["details.countries"] = "More than 20 available countries", ["details.dataVoice"] = "Data and voice eSIM", ["details.noPhysicalCard"] = "No physical SIM card required", ["details.instantActivation"] = "Digital delivery and quick activation", ["details.fullSim"] = "Full-featured physical SIM card", ["details.dataCallsSms"] = "Data, calls and SMS", ["details.deliverySeparate"] = "Delivery cost is not included", ["details.demoPurchase"] = "The demo purchase uses a test balance and does not create a real Narayana service.", ["details.livePurchasePending"] = "Purchases inside Telegram are not available yet. Check the official price list for current terms.", ["details.demoConnectFor"] = "Demo purchase · {0}",
        ["stars.priceMissing"] = "A Stars price hasn't been configured for this plan yet.", ["stars.webhookPending"] = "Payment accepted. The service will appear after webhook confirmation.", ["stars.cancelled"] = "Payment cancelled — the service wasn't connected.", ["stars.failed"] = "Payment wasn't completed. Try again.", ["stars.openFailed"] = "We couldn't open the Stars payment.", ["purchase.telegramRequired"] = "A verified Telegram session is required to purchase.", ["purchase.unavailable"] = "This service is unavailable.", ["purchase.failed"] = "We couldn't connect the service.", ["purchase.offline"] = "You're offline. Check your connection and try again.", ["purchase.timeout"] = "Connection is taking too long. Check the service status before trying again.", ["purchase.conflict"] = "This action has already been completed. Refresh your services.", ["payment.orderInvalid"] = "This order has expired or has already been paid.",
        ["learn.title"] = "NRN — Privacy Lab", ["learn.heading"] = "Privacy is a skill", ["learn.description"] = "Learn to recognize digital risks through short interactive scenarios.", ["learn.first"] = "FIRST SIMULATION", ["learn.wifi"] = "Public Wi-Fi", ["learn.scenario"] = "You're at the airport, your phone is almost out of battery, and you urgently need to send a document. What is the safe choice?", ["learn.start"] = "Start simulation", ["learn.coming"] = "New scenarios will follow after the first polished experience launches.",
        ["help.title"] = "NRN — Help", ["help.eyebrow"] = "SUPPORT", ["help.heading"] = "How can we help?", ["help.description"] = "Frequently asked questions and official Narayana contact details.", ["help.faq"] = "Frequently asked questions", ["help.connectQ"] = "How do I connect a service?", ["help.connectA"] = "Open Buy, choose a service, and confirm the connection.", ["help.statusQ"] = "Where can I check a service status?", ["help.statusA"] = "All active services and their status are available in My services.", ["help.cancelQ"] = "How do I cancel a subscription?", ["help.cancelA"] = "Contact support and the team will help you manage your subscription.", ["help.need"] = "Need help?", ["help.supportText"] = "Open the official Narayana page in Telegram or email support@narayana.im.", ["help.openSupport"] = "Open the Telegram page", ["help.feedback"] = "Feedback", ["help.opensTelegram"] = "Opens in Telegram", ["help.opensBrowser"] = "Opens in your external browser", ["help.notConfigured"] = "Link not configured yet", ["help.linksPending"] = "The feedback link is not available yet. For help, email support@narayana.im.",
        ["error.title"] = "Error", ["error.heading"] = "Something went wrong", ["error.description"] = "Reload the Mini App and try again.", ["error.back"] = "Return to the app",
        ["demo.title"] = "Narayana Bot — Demo environment", ["demo.label"] = "Demo environment", ["demo.heading"] = "From Narayana Bot to Mini App", ["demo.description"] = "A simulation of the existing Telegram journey. This is not the Telegram interface or a real chat.", ["demo.openDirect"] = "Open the Mini App directly", ["demo.phoneLabel"] = "Narayana Bot simulation", ["demo.chatLabel"] = "Demo chat", ["demo.today"] = "Today", ["demo.welcome"] = "👋 Welcome to Narayana!", ["demo.logged"] = "🔑 Logged in as", ["demo.balance"] = "💰 Demo balance: 12 EUR", ["demo.private"] = "🔐 Products from Narayana's public catalog:", ["demo.products"] = "eSIM — data and voice; availability depends on destination.<br />Virtual numbers — voice and SMS in more than 20 countries.", ["demo.productsEsim"] = "eSIM — data and voice; availability depends on destination.", ["demo.productsMobile"] = "Virtual numbers — voice and SMS in more than 20 countries.", ["demo.choose"] = "Choose your plan.", ["demo.data"] = "🌐 eSIM — from EUR 9 with digital delivery.", ["demo.mobile"] = "☎️ Virtual numbers — from EUR 10 per month; price varies by country.", ["demo.commands"] = "Bot commands", ["demo.account"] = "👤 My Account", ["demo.services"] = "📦 Services", ["demo.prices"] = "💰 Prices", ["demo.help"] = "❓ Help", ["demo.openApp"] = "Open Narayana App", ["demo.openAppHint"] = "Catalog, account and connected services", ["demo.message"] = "Message", ["demo.dialog"] = "Narayana Mini App", ["demo.close"] = "Close Mini App", ["demo.frame"] = "Narayana Mini App demo",
        ["product.virtual-numbers.name"] = "Virtual Numbers", ["product.virtual-numbers.description"] = "Mobile and toll-free numbers with voice and SMS support in more than 20 countries.", ["product.virtual-numbers.priceNote"] = "Price varies by country", ["product.esim.name"] = "eSIM", ["product.esim.description"] = "Data and voice eSIM with instant digital delivery and no physical card.", ["product.esim.priceNote"] = "Instant digital delivery", ["product.physical-sim.name"] = "Physical SIM", ["product.physical-sim.description"] = "A full-featured SIM with data, calls and SMS. Delivery is charged separately.", ["product.physical-sim.priceNote"] = "Delivery not included"
    };
}
