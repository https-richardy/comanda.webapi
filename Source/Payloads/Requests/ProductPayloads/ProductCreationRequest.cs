namespace Comanda.WebApi.Payloads;

public sealed record ProductCreationRequest : IRequest<Response>
{
    public string Title { get; init; }
    public string Description { get; init; }
    public decimal Price { get; init; }
    public int CategoryId { get; init; }
    public ICollection<IngredientAssociationScheme> Ingredients { get; init; }
}