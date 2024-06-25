namespace Comanda.WebApi.Payloads;

public sealed record EstablishmentCategoryRegistrationRequest : AuthenticatedRequest, IRequest<Response>
{
    public string Name { get; set; }

    [JsonIgnore]
    public int EstablishmentId { get; set; }
}
