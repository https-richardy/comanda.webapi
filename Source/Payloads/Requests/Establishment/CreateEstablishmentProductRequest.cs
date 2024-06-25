namespace Comanda.WebApi.Payloads;

public sealed record CreateEstablishmentProductRequest : AuthenticatedRequest, IRequest<Response>
{
    public string Title { get; set; }
    public string Description { get; init; }
    public decimal Price { get; init; }
    public int CategoryId { get; init; }
    public IFormFile Image { get; init; }

    [JsonIgnore]
    public int EstablishmentId { get; set; }
}