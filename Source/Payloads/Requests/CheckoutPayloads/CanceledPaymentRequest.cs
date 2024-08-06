namespace Comanda.WebApi.Payloads;

public sealed record CanceledPaymentRequest : IRequest<Response>
{
    [JsonIgnore] /* this property will be set from the query params. */
    public int SessionId { get; set; }
}