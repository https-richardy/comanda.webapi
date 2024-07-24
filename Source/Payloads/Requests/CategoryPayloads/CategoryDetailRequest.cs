namespace Comanda.WebApi.Payloads;

public sealed record CategoryDetailRequest : IRequest<Response<Category>>
{
    [JsonIgnore]
    public int CategoryId { get; set; }
}