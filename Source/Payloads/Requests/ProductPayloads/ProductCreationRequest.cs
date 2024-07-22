namespace Comanda.WebApi.Payloads;

public sealed record ProductCreationRequest
{
    public string Title { get; init; }
    public string Description { get; init; }
    public decimal Price { get; init; }
    public int CategoryId { get; init; }
    public IFormFile Image { get; init; }
}