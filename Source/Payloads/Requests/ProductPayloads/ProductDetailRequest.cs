namespace Comanda.WebApi.Payloads;

public sealed record ProductDetailRequest : IRequest<Response>
{
    [JsonIgnore]
    public int ProductId { get; set; }
}