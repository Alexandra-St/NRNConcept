namespace NRN.Web.Features.SolutionFinder.Models;

public sealed record FinderOption(string Id, string Label);

public sealed record FinderQuestion(
    string Id,
    string Kicker,
    string Title,
    string Description,
    bool AllowsMultiple,
    IReadOnlyList<FinderOption> Options);

public sealed record FinderProduct(
    string Id,
    string Name,
    string Tagline,
    string Description,
    string Fits,
    string DoesNotFit,
    string Limitation,
    string NarayanaUrl,
    string? LearningUrl);

public sealed record FinderRecommendation(
    FinderProduct Product,
    string Need,
    string Why,
    bool IsHighPriority);

public sealed record FinderLearningLink(
    string Title,
    string Description,
    string Url);

public sealed record FinderResult(
    IReadOnlyList<FinderRecommendation> Recommendations,
    IReadOnlyList<FinderLearningLink> LearningLinks);

public static class FinderQuestionIds
{
    public const string Goals = "goals";
    public const string NumberUse = "number-use";
    public const string Travel = "travel";
    public const string Calls = "calls";
    public const string Priority = "priority";
    public const string NumberPurpose = "number-purpose";
    public const string TravelChallenge = "travel-challenge";
}

public static class FinderAnswerIds
{
    public const string SaferInternet = "safer-internet";
    public const string SeparateNumber = "separate-number";
    public const string TravelConnectivity = "travel-connectivity";
    public const string SeparateWork = "separate-work";
    public const string ProtectAccounts = "protect-accounts";
    public const string Explore = "explore";

    public const string Registrations = "registrations";
    public const string Classifieds = "classifieds";
    public const string Work = "work";
    public const string CloseCircleOnly = "close-circle-only";
    public const string AvoidSharing = "avoid-sharing";

    public const string TravelOften = "travel-often";
    public const string TravelSometimes = "travel-sometimes";
    public const string TravelRarely = "travel-rarely";

    public const string PersonalCalls = "personal-calls";
    public const string InternationalCalls = "international-calls";
    public const string WorkNumber = "work-number";
    public const string InternetCalls = "internet-calls";
    public const string RareCalls = "rare-calls";

    public const string Privacy = "privacy";
    public const string Simplicity = "simplicity";
    public const string International = "international";
    public const string Separation = "separation";

    public const string TemporaryContacts = "temporary-contacts";
    public const string PersonalCommunication = "personal-communication";
    public const string Business = "business";
    public const string SeveralPurposes = "several-purposes";

    public const string MobileInternet = "mobile-internet";
    public const string PublicWifi = "public-wifi";
    public const string KeepNumber = "keep-number";
    public const string NoTravelChallenge = "no-travel-challenge";
}
