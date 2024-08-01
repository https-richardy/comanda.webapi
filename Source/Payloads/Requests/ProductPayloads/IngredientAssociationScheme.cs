namespace Comanda.WebApi.Payloads;

public sealed record IngredientAssociationScheme
{
    public int StandardQuantity { get; init; }
    public int IngredientId { get; init; }
    public bool IsMandatory { get; init; }
}