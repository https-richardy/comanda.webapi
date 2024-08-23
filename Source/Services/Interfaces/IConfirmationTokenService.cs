namespace Comanda.WebApi.Services;

public interface IConfirmationTokenService
{
    ConfirmationToken GenerateToken();
}