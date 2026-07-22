namespace NRN.Telegram.Features.Services.Contracts;

public enum ConnectedServiceStatus
{
    Active,
    Paused,
    Expired
}

public sealed record ConnectedServiceDto(
    string Id,
    string Name,
    string Description,
    ConnectedServiceStatus Status,
    string StatusDetail);
