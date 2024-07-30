namespace Comanda.WebApi.Payloads;

public sealed record IngredientCreationRequest : IRequest<Response>
{
    public string Name { get; init; }
}