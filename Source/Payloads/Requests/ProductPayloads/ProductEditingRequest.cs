namespace Comanda.WebApi.Payloads;

public sealed record ProductEditingRequest : IRequest<Response>
{
    [JsonIgnore]
    public int ProductId { get; set; }

    public string Title { get; init; }
    public string Description { get; init; }
    public decimal Price { get; init; }
    public int CategoryId { get; init; }
    public IFormFile? Image { get; init; }
}