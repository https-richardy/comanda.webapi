namespace Comanda.WebApi.Helpers;

public interface IHostInformationProvider
{
    string HttpProtocol { get; }
    string HostAddress { get; }
}