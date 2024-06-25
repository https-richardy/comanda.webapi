namespace Comanda.WebApi.Payloads;

public record CreateEstablishmentRequest : IRequest<Response>, IAuthenticatedRequest
{
    public string EstablishmentName { get; init; }
    public string UserId { get; set; } = string.Empty;
}