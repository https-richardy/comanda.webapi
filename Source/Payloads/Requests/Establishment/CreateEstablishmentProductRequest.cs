namespace Comanda.WebApi.Payloads;

public sealed record CreateEstablishmentProductRequest : IRequest<Response>, IAuthenticatedRequest
{
    public string Title { get; set; }
    public string Description { get; init; }
    public decimal Price { get; init; }
    public IFormFile Image { get; init; }

    [JsonIgnore]
    public string UserId { get; set; } = string.Empty;

    [JsonIgnore]
    public int EstablishmentId { get; set; }
}