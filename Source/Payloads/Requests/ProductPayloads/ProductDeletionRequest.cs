namespace Comanda.WebApi.Payloads;

public sealed record ProductDeletionRequest : IRequest<Response>
{
    [JsonIgnore]
    public int ProductId { get; set; }
}