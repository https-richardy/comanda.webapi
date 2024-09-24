namespace Comanda.WebApi.Payloads;

public sealed record ProfileExportData
{
    public string Name { get; init; }
    public string Email { get; init; }

    public IEnumerable<AddressExportFormatted> Addresses { get; init; } = [];
    public IEnumerable<OrderHistoryExportFormatted> Orders { get; init; } = [];
}