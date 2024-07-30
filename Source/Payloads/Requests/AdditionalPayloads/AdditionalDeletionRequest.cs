namespace Comanda.WebApi.Payloads;

public sealed record AdditionalDeletionRequest : IRequest<Response>
{
    [JsonIgnore] /* this property will be set from the route. */
    public int AdditionalId { get; set; }
}