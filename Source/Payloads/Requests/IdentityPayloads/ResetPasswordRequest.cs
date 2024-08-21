namespace Comanda.WebApi.Payloads;

public sealed record ResetPasswordRequest : IRequest<Response>
{
    public string Email { get; init; }
    public string Token { get; init; }
    public string NewPassword { get; init; }
}