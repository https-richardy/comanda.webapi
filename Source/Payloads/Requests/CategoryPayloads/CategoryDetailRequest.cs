namespace Comanda.WebApi.Payloads;

public sealed record CategoryDetailRequest : IRequest<Response>
{
    [JsonIgnore]
    public int CategoryId { get; set; }
}