namespace Comanda.WebApi.Payloads;

public sealed class AddressDeletionRequest : IRequest<Response>
{
    [JsonIgnore] /* this property will be set from the route. */
    public int AddressId { get; set; }
}