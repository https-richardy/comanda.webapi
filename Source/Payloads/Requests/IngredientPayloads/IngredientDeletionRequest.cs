namespace Comanda.WebApi.Payloads;

public sealed record IngredientDeletionRequest : IRequest<Response>
{
    public int IngredientId { get; set; }
}