namespace Comanda.WebApi.Payloads;

public sealed record BasicIngredientDetail
{
    public int Id { get; init; }
    public string Name { get; init; }
}