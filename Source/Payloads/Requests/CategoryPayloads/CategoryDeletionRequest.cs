namespace Comanda.WebApi.Payloads;

public sealed record CategoryDeletionRequest : IRequest<Response>
{
    [JsonIgnore]
    public int CategoryId { get; set; }
}