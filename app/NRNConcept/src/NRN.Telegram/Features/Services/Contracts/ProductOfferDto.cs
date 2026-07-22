namespace NRN.Telegram.Features.Services.Contracts;

public enum ServicePlanKind
{
    VirtualNumber,
    Esim,
    PhysicalSim
}

public sealed record ProductOfferDto(
    string Id,
    string Name,
    string Description,
    decimal ConnectionPrice,
    decimal MonthlyPrice,
    string Currency,
    ServicePlanKind Kind,
    string? CountryCode,
    string FlagEmoji,
    bool IncludesPhoneNumber,
    string Accent,
    bool IsPopular = false,
    bool ConnectionPriceIsFrom = false,
    bool MonthlyPriceIsFrom = false);
