namespace Comanda.WebApi.Payloads;

public sealed record AdditionalEditingRequest : IRequest<Response>
{
    public string Name { get; init; }
    public decimal Price { get; init; }
    public int CategoryId { get; init; }

    [JsonIgnore] /* this property will be set from the route. */
    public int AdditionalId { get; set; }
}