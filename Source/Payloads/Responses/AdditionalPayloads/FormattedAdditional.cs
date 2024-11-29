namespace Comanda.WebApi.Payloads;

public sealed class FormattedAdditional
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public decimal Price { get; init; }
    public FormattedCategory Category { get; init; }
}