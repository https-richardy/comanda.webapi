namespace Comanda.WebApi.Payloads;

public sealed record EstablishmentCategoryRegistrationRequest : IRequest<Response>, IAuthenticatedRequest
{
    public string Name { get; set; }

    [JsonIgnore]
    public string UserId { get; set; } = string.Empty;
}
