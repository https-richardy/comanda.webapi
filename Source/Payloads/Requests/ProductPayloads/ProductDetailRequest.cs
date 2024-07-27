namespace Comanda.WebApi.Payloads;

public sealed record ProductDetailRequest : IRequest<Response<Product>>
{
    [JsonIgnore]
    public int ProductId { get; set; }
}