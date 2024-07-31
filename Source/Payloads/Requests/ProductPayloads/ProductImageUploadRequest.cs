namespace Comanda.WebApi.Payloads;

public sealed record ProductImageUploadRequest : IRequest<Response>
{
    [JsonIgnore] /* this property will be set from the route. */
    public int ProductId { get; set; }

    public IFormFile Image { get; init; }
}