using NRN.Web.Features.SolutionFinder.Models;

namespace NRN.Web.Features.SolutionFinder.Services;

public sealed class SolutionFinderService : ISolutionFinderService
{
    public IReadOnlyList<FinderQuestion> GetQuestions(string locale, SolutionFinderState state)
    {
        var ru = IsRussian(locale);
        var questions = new List<FinderQuestion>
        {
            new(FinderQuestionIds.Goals, ru ? "ВАША ЗАДАЧА" : "YOUR GOAL",
                ru ? "Что вы хотите улучшить?" : "What would you like to improve?",
                ru ? "Можно выбрать несколько вариантов." : "Choose as many as apply.", true,
                [
                    O(FinderAnswerIds.SaferInternet, ru ? "Безопаснее подключаться к интернету вне дома" : "Use the internet more safely away from home"),
                    O(FinderAnswerIds.SeparateNumber, ru ? "Не использовать личный номер повсюду" : "Avoid using my personal number everywhere"),
                    O(FinderAnswerIds.TravelConnectivity, ru ? "Оставаться на связи во время поездок" : "Stay connected while travelling"),
                    O(FinderAnswerIds.SeparateWork, ru ? "Разделить личное и рабочее общение" : "Separate personal and work communication"),
                    O(FinderAnswerIds.ProtectAccounts, ru ? "Лучше защитить аккаунты и пароли" : "Protect accounts and passwords better"),
                    O(FinderAnswerIds.Explore, ru ? "Пока не знаю — хочу разобраться" : "I am not sure yet — help me explore")
                ]),
            new(FinderQuestionIds.NumberUse, ru ? "ВАШ НОМЕР" : "YOUR NUMBER",
                ru ? "Где вам приходится указывать основной номер?" : "Where do you use your primary number?",
                ru ? "Выберите все подходящие ситуации." : "Choose every situation that applies.", true,
                [
                    O(FinderAnswerIds.Registrations, ru ? "При регистрациях в онлайн-сервисах" : "For online service registrations"),
                    O(FinderAnswerIds.Classifieds, ru ? "В объявлениях и разовых контактах" : "For listings and one-off contacts"),
                    O(FinderAnswerIds.Work, ru ? "Для рабочего общения" : "For work communication"),
                    O(FinderAnswerIds.CloseCircleOnly, ru ? "Только для близких и знакомых" : "Only with people I know"),
                    O(FinderAnswerIds.AvoidSharing, ru ? "Я стараюсь его не указывать" : "I try not to share it")
                ]),
            new(FinderQuestionIds.Travel, ru ? "ПОЕЗДКИ" : "TRAVEL",
                ru ? "Как часто вы путешествуете или живёте в другой стране?" : "How often do you travel or live abroad?",
                string.Empty, false,
                [
                    O(FinderAnswerIds.TravelOften, ru ? "Часто" : "Often"),
                    O(FinderAnswerIds.TravelSometimes, ru ? "Несколько раз в год" : "A few times a year"),
                    O(FinderAnswerIds.TravelRarely, ru ? "Редко или никогда" : "Rarely or never")
                ]),
            new(FinderQuestionIds.Calls, ru ? "ЗВОНКИ" : "CALLS",
                ru ? "Как вы используете телефонную связь?" : "How do you use phone calls?",
                ru ? "Можно выбрать несколько вариантов." : "Choose as many as apply.", true,
                [
                    O(FinderAnswerIds.PersonalCalls, ru ? "В основном принимаю личные звонки" : "Mostly personal calls"),
                    O(FinderAnswerIds.InternationalCalls, ru ? "Часто звоню в другие страны" : "I often call other countries"),
                    O(FinderAnswerIds.WorkNumber, ru ? "Нужен отдельный рабочий номер" : "I need a separate work number"),
                    O(FinderAnswerIds.InternetCalls, ru ? "Хочу звонить через приложение по интернету" : "I want to call through an internet app"),
                    O(FinderAnswerIds.RareCalls, ru ? "Почти не пользуюсь обычными звонками" : "I rarely use traditional calls")
                ]),
            new(FinderQuestionIds.Priority, ru ? "ПРИОРИТЕТ" : "PRIORITY",
                ru ? "Что для вас важнее всего?" : "What matters most to you?",
                string.Empty, false,
                [
                    O(FinderAnswerIds.Privacy, ru ? "Приватность" : "Privacy"),
                    O(FinderAnswerIds.Simplicity, ru ? "Простота" : "Simplicity"),
                    O(FinderAnswerIds.International, ru ? "Связь в разных странах" : "Connectivity across countries"),
                    O(FinderAnswerIds.Separation, ru ? "Разделение личной и рабочей жизни" : "Separating personal and work life")
                ])
        };

        if (NeedsNumberFollowUp(state))
        {
            questions.Add(new(FinderQuestionIds.NumberPurpose, ru ? "УТОЧНЕНИЕ" : "A LITTLE MORE",
                ru ? "Для чего вам нужен отдельный номер?" : "What would you use a separate number for?",
                string.Empty, false,
                [
                    O(FinderAnswerIds.TemporaryContacts, ru ? "Для регистраций и временных контактов" : "Registrations and temporary contacts"),
                    O(FinderAnswerIds.PersonalCommunication, ru ? "Для постоянного личного общения" : "Ongoing personal communication"),
                    O(FinderAnswerIds.Business, ru ? "Для работы или бизнеса" : "Work or business"),
                    O(FinderAnswerIds.SeveralPurposes, ru ? "Для нескольких задач" : "Several purposes")
                ]));
        }

        if (NeedsTravelFollowUp(state))
        {
            questions.Add(new(FinderQuestionIds.TravelChallenge, ru ? "В ПОЕЗДКЕ" : "WHILE TRAVELLING",
                ru ? "Что обычно сложнее всего в поездках?" : "What is usually hardest while travelling?",
                string.Empty, false,
                [
                    O(FinderAnswerIds.MobileInternet, ru ? "Найти подходящий мобильный интернет" : "Finding suitable mobile internet"),
                    O(FinderAnswerIds.PublicWifi, ru ? "Пользоваться публичным Wi-Fi" : "Using public Wi-Fi"),
                    O(FinderAnswerIds.KeepNumber, ru ? "Сохранять доступ к привычному номеру" : "Keeping access to my usual number"),
                    O(FinderAnswerIds.NoTravelChallenge, ru ? "Ничего из перечисленного" : "None of these")
                ]));
        }

        return questions;
    }

    public FinderResult Evaluate(string locale, SolutionFinderState state)
    {
        var ru = IsRussian(locale);
        var recommendations = new List<FinderRecommendation>();
        var personalNumberPurpose = state.HasAny(FinderQuestionIds.NumberPurpose, FinderAnswerIds.PersonalCommunication);
        var businessNumberPurpose = state.HasAny(FinderQuestionIds.NumberPurpose, FinderAnswerIds.Business);
        var highPriority = state.HasAny(FinderQuestionIds.NumberUse,
            FinderAnswerIds.Registrations, FinderAnswerIds.Classifieds) ||
            NeedsNumberFollowUp(state) && state.HasAny(FinderQuestionIds.NumberPurpose,
                FinderAnswerIds.TemporaryContacts, FinderAnswerIds.SeveralPurposes);
        var relevant = highPriority ||
            state.HasAny(FinderQuestionIds.NumberUse, FinderAnswerIds.Work) ||
            state.HasAny(FinderQuestionIds.Goals, FinderAnswerIds.SeparateNumber, FinderAnswerIds.SeparateWork) ||
            state.HasAny(FinderQuestionIds.Calls, FinderAnswerIds.WorkNumber);

        if (relevant)
        {
            var product = GetProduct("virtual-number", locale)!;
            var need = highPriority
                ? (ru ? "Не раскрывать основной номер при регистрациях и разовых контактах." : "Keep your primary number away from registrations and one-off contacts.")
                : businessNumberPurpose
                    ? (ru ? "Отделить рабочие контакты от личного номера." : "Keep work contacts separate from your personal number.")
                    : personalNumberPurpose
                        ? (ru ? "Использовать отдельный номер для постоянного общения." : "Use a separate number for ongoing communication.")
                        : (ru ? "Разделить личное и рабочее общение." : "Separate personal and work communication.");
            var why = highPriority
                ? (ru ? "Вы указали, что используете основной номер в контекстах, где постоянная личная связь не нужна." : "You use your primary number in situations that do not require a permanent personal connection.")
                : businessNumberPurpose
                    ? (ru ? "Вы указали, что отдельный номер нужен для работы или бизнеса." : "You said the separate number is for work or business.")
                    : personalNumberPurpose
                        ? (ru ? "Вы указали, что хотите отдельный номер для постоянного личного общения." : "You said you want a separate number for ongoing personal communication.")
                        : (ru ? "Отдельный номер поможет не смешивать разные способы общения." : "A separate number can keep different communication contexts apart.");
            why += PriorityContext(ru, state, product.Id);
            recommendations.Add(new(product, need, why, highPriority));
        }

        var esimRelevant = state.HasAny(FinderQuestionIds.TravelChallenge, FinderAnswerIds.MobileInternet);
        var esimHighPriority = esimRelevant &&
            state.HasAny(FinderQuestionIds.Travel, FinderAnswerIds.TravelOften);
        if (esimRelevant)
        {
            var product = GetProduct("esim", locale)!;
            var need = ru
                ? "Получить мобильный интернет во время поездок."
                : "Get mobile internet while travelling.";
            var why = esimHighPriority
                ? (ru ? "Вы указали, что в поездках вам сложнее всего найти подходящий мобильный интернет." : "You said that finding suitable mobile internet is your main travel challenge.")
                : (ru ? "Вы путешествуете несколько раз в год и указали, что вам нужен подходящий мобильный интернет." : "You travel a few times a year and said that you need suitable mobile internet.");
            why += PriorityContext(ru, state, product.Id);
            recommendations.Add(new(product, need, why, esimHighPriority));
        }

        var sipHighPriority = state.HasAny(FinderQuestionIds.Calls, FinderAnswerIds.InternetCalls);
        var sipRelevant = sipHighPriority ||
            state.HasAny(FinderQuestionIds.Calls, FinderAnswerIds.InternationalCalls);
        if (sipRelevant)
        {
            var product = GetProduct("sip", locale)!;
            var need = sipHighPriority
                ? (ru ? "Совершать звонки через интернет-приложение." : "Make calls through an internet application.")
                : (ru ? "Регулярно звонить в другие страны." : "Make regular international calls.");
            var why = sipHighPriority
                ? (ru ? "Это решение позволяет звонить через интернет из совместимого приложения или устройства." : "This solution lets you make calls over the internet from a compatible app or device.")
                : (ru ? "Это решение поддерживает международные звонки через интернет с поминутной оплатой." : "This solution supports international calls over the internet with per-minute pricing.");
            why += PriorityContext(ru, state, product.Id);
            recommendations.Add(new(product, need, why, sipHighPriority));
        }

        var learningLinks = new List<FinderLearningLink>();
        if (state.HasAny(FinderQuestionIds.Goals, FinderAnswerIds.SaferInternet) ||
            state.HasAny(FinderQuestionIds.TravelChallenge, FinderAnswerIds.PublicWifi))
        {
            learningLinks.Add(new(
                ru ? "Безопасность публичного Wi-Fi" : "Public Wi-Fi safety",
                ru ? "Разберитесь, что действительно защищает соединение в чужой сети." : "Learn what actually protects a connection on a network you do not control.",
                "/learn/everyday-situations/public-wifi"));
        }
        if (state.HasAny(FinderQuestionIds.Goals, FinderAnswerIds.ProtectAccounts))
        {
            learningLinks.Add(new(
                ru ? "Пароли и защита аккаунтов" : "Passwords and account safety",
                ru ? "Начните с уникальных паролей и подходящего второго фактора." : "Start with unique passwords and an appropriate second factor.",
                "/learn/everyday-situations/passwords"));
        }
        if (state.HasAny(FinderQuestionIds.Goals, FinderAnswerIds.Explore) && learningLinks.Count == 0)
        {
            learningLinks.Add(new(
                ru ? "Что такое цифровая приватность?" : "What is digital privacy?",
                ru ? "Короткая отправная точка без технических терминов." : "A short starting point without technical jargon.",
                "/learn/foundations/what-is-digital-privacy"));
        }

        if (recommendations.Count == 0 && learningLinks.Count == 0)
        {
            learningLinks.Add(new(
                ru ? "Соберите свой набор приватности" : "Build your privacy toolkit",
                ru ? "Посмотрите, какие базовые шаги полезны до выбора специального инструмента." : "See which foundational steps help before choosing a specialised tool.",
                "/learn/taking-control/building-your-privacy-toolkit"));
        }

        var orderedRecommendations = OrderRecommendations(recommendations, state);
        return new(orderedRecommendations, learningLinks);
    }

    public FinderProduct? GetProduct(string productId, string locale)
    {
        var ru = IsRussian(locale);
        return productId.ToLowerInvariant() switch
        {
            "virtual-number" => new(
                "virtual-number",
                ru ? "Виртуальный номер" : "Virtual number",
                ru ? "Отделите основной номер от отдельных онлайн-контекстов." : "Separate your primary number from specific online contexts.",
                ru ? "Дополнительный номер для входящих звонков и/или SMS помогает не использовать основной номер для каждой регистрации, объявления или рабочего контакта." : "An additional number for incoming calls and/or SMS helps you avoid using your primary number for every registration, listing or work contact.",
                ru ? "Подходит для регистраций, разовых контактов и разделения личного и рабочего общения." : "A good fit for registrations, one-off contacts and separating personal from work communication.",
                ru ? "Не подходит как замена защищённому мессенджеру и не усиливает безопасность паролей или аккаунтов." : "It is not a replacement for a secure messenger and does not strengthen passwords or account security.",
                ru ? "Отдельный номер не защищает содержание сообщений и все связанные метаданные. Цена, звонки, SMS и доступные страны зависят от выбранного номера." : "A separate number does not protect message content or all related metadata. Price, calls, SMS and available countries depend on the selected number.",
                "https://narayana.im/register",
                "/learn/taking-control/messaging-apps"),
            "esim" => new(
                "esim",
                "eSIM",
                ru ? "Мобильный интернет для поездок без физической SIM-карты." : "Mobile internet for travel without a physical SIM card.",
                ru ? "Narayana предлагает eSIM с доступом в интернет. После регистрации и пополнения баланса доступные услуги выбираются в личном кабинете." : "Narayana offers eSIMs with internet access. After registration and account funding, available services are selected in the dashboard.",
                ru ? "Подходит путешественникам, которым нужен мобильный интернет за границей и чьё устройство поддерживает eSIM." : "A good fit for travellers who need mobile internet abroad and have an eSIM-compatible device.",
                ru ? "Не подходит, если устройство не поддерживает eSIM, нужен гарантированный доступ в конкретной сети или требуется сохранить привычный телефонный номер." : "It does not fit when the device lacks eSIM support, a specific network must be guaranteed, or the usual phone number must be preserved.",
                ru ? "Перед покупкой нужно проверить совместимость устройства, покрытие и актуальную стоимость передачи данных. Публичная страница тарифов указывает оплату мобильных данных за мегабайт." : "Check device compatibility, coverage and the current data rate before purchase. The public pricing page lists mobile data charges per megabyte.",
                "https://narayana.im/register",
                "/learn/everyday-situations/public-wifi"),
            "sip" => new(
                "sip",
                ru ? "Интернет-звонки (SIP)" : "Internet calls (SIP)",
                ru ? "Звоните через интернет независимо от обычной мобильной линии." : "Make calls over the internet without relying on a standard mobile line.",
                ru ? "Аккаунт Narayana для интернет-звонков позволяет звонить из совместимого приложения или устройства, в том числе на международные направления. Сервис требует защищённое соединение для таких звонков." : "A Narayana internet-calling account lets you call from a compatible app or device, including international destinations. The service requires a protected connection for these calls.",
                ru ? "Подходит тем, кто регулярно звонит за границу или хочет совершать интернет-звонки из отдельного приложения." : "A good fit for people who call abroad regularly or want to make internet calls from a dedicated app.",
                ru ? "Не подходит, если нужен только дополнительный номер для SMS и регистраций или звонки должны работать без интернет-соединения." : "It does not fit when only an additional number for SMS and registrations is needed, or calls must work without an internet connection.",
                ru ? "Требуются интернет и настройка совместимого приложения; звонки оплачиваются поминутно. Публичные материалы не подтверждают поддержку экстренных вызовов, поэтому это не замена обычной мобильной связи." : "An internet connection and compatible app setup are required; calls are billed per minute. Public materials do not confirm emergency calling, so this is not a replacement for ordinary mobile service.",
                "https://narayana.im/register",
                null),
            _ => null
        };
    }

    private static FinderOption O(string id, string label) => new(id, label);
    private static bool IsRussian(string locale) => string.Equals(locale, "ru", StringComparison.OrdinalIgnoreCase);
    private static string PriorityContext(bool ru, SolutionFinderState state, string productId)
    {
        if (state.HasAny(FinderQuestionIds.Priority, FinderAnswerIds.Privacy))
            return productId switch
            {
                "virtual-number" => ru ? " Это также уменьшает число контекстов, в которых раскрывается ваш основной номер." : " It also reduces the number of contexts where your primary number is exposed.",
                "esim" => ru ? " Важно: eSIM решает задачу подключения, но сама по себе не скрывает вашу сетевую активность." : " Keep in mind: an eSIM solves connectivity, but does not hide your network activity by itself.",
                _ => ru ? " Защищённое соединение не следует считать полной анонимностью звонков." : " A protected connection should not be treated as complete call anonymity."
            };
        if (state.HasAny(FinderQuestionIds.Priority, FinderAnswerIds.Simplicity))
            return productId switch
            {
                "virtual-number" => ru ? " Это сравнительно простой способ разделить контакты без полной перестройки привычного общения." : " It is a relatively simple way to separate contacts without rebuilding your communication habits.",
                "esim" => ru ? " Потребуется проверить совместимость устройства и активировать eSIM перед поездкой." : " You will need to check device compatibility and activate the eSIM before travelling.",
                _ => ru ? " Потребуется отдельно настроить совместимое приложение, поэтому это не самый простой вариант." : " A compatible app must be configured separately, so this is not the simplest option."
            };
        if (state.HasAny(FinderQuestionIds.Priority, FinderAnswerIds.International))
            return productId switch
            {
                "esim" => ru ? " Это напрямую отвечает выбранному приоритету связи в разных странах." : " This directly supports your priority of staying connected across countries.",
                "sip" => ru ? " Международные звонки напрямую отвечают выбранному вами приоритету." : " International calling directly supports the priority you selected.",
                _ => ru ? " Доступность номеров по странам нужно проверить перед выбором." : " Check number availability by country before choosing."
            };
        if (state.HasAny(FinderQuestionIds.Priority, FinderAnswerIds.Separation))
            return productId switch
            {
                "virtual-number" => ru ? " Это напрямую поддерживает выбранное вами разделение личной и рабочей жизни." : " This directly supports your priority of separating personal and work life.",
                "esim" => ru ? " eSIM создаёт отдельное подключение для поездки, но не разделяет личный и рабочий номер." : " An eSIM creates a separate travel connection, but does not separate personal and work phone numbers.",
                _ => ru ? " Отдельное приложение разделит способ звонков, но не заменит отдельный номер для регистраций." : " A dedicated app separates how you make calls, but does not replace a separate number for registrations."
            };
        return string.Empty;
    }
    private static IReadOnlyList<FinderRecommendation> OrderRecommendations(
        IEnumerable<FinderRecommendation> recommendations,
        SolutionFinderState state)
    {
        var internationalFirst = state.HasAny(FinderQuestionIds.Priority, FinderAnswerIds.International);
        var separationFirst = state.HasAny(FinderQuestionIds.Priority, FinderAnswerIds.Separation);
        return recommendations
            .OrderByDescending(item => item.IsHighPriority)
            .ThenBy(item => internationalFirst && (item.Product.Id is "esim" or "sip") ? 0
                : separationFirst && item.Product.Id == "virtual-number" ? 0 : 1)
            .ThenBy(item => item.Product.Id, StringComparer.Ordinal)
            .Take(3)
            .ToList();
    }
    private static bool NeedsNumberFollowUp(SolutionFinderState state) => state.HasAny(FinderQuestionIds.NumberUse,
        FinderAnswerIds.Registrations, FinderAnswerIds.Classifieds, FinderAnswerIds.Work);
    private static bool NeedsTravelFollowUp(SolutionFinderState state) => state.HasAny(FinderQuestionIds.Travel,
        FinderAnswerIds.TravelOften, FinderAnswerIds.TravelSometimes);
}
