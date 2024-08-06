namespace Comanda.WebApi.Helpers;

public sealed class HostInformationProvider : IHostInformationProvider
{
    private readonly IHttpContextAccessor _contextAccessor;
    public string HttpProtocol => _contextAccessor?.HttpContext?.Request?.Scheme ?? string.Empty;
    public string HostAddress
    {
        get
        {
            var request = _contextAccessor?.HttpContext?.Request;
            return request == null ? string.Empty : $"{request?.Scheme}://{request?.Host}";
        }
    }

    public HostInformationProvider(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }
}