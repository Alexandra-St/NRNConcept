namespace NRN.Telegram.Features.Catalog;

public sealed class NarayanaPublicCatalogOptions
{
    public const string SectionName = "NarayanaPublicCatalog";

    public string PricesUrl { get; init; } = "https://narayana.im/about/prices";
    public DateOnly VerifiedOn { get; init; } = new(2026, 7, 16);
}
