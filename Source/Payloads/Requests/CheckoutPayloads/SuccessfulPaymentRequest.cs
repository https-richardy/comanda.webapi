namespace Comanda.WebApi.Payloads;

public sealed record SuccessfulPaymentRequest : IRequest<Response>
{
    [JsonIgnore] /* this property will be set from the query params. */
    public string SessionId { get; set; }
}