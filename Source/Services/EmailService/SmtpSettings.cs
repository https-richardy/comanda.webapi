namespace Comanda.WebApi.Services;

public sealed record SmtpSettings
{
    public string Host { get; init; } = null!;
    public string UserName { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string FromAddress { get; init; } = null!;
    public string DisplayName { get; init; } = null!;
    public int Port { get; init; }
    public bool UseSsl { get; init; }
}