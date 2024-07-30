namespace Comanda.WebApi.Payloads;

public sealed class IngredientEditingRequest : IRequest<Response>
{
    public int IngredientId { get; set; }
    public string Name { get; init; }
}