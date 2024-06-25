namespace Comanda.WebApi.Payloads;

public abstract record AuthenticatedRequest : IAuthenticatedRequest
{
    [JsonIgnore]
    public string UserId { get; set; }
}