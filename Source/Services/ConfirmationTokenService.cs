namespace Comanda.WebApi.Services;

public sealed class ConfirmationTokenService : IConfirmationTokenService
{
    private readonly Random _random = new();

    private const int _tokenLength = 8;
    private const int _expirationMinutes = 30;

    public ConfirmationToken GenerateToken()
    {
        char[] token = new char[_tokenLength];
        for (int index = 0; index < _tokenLength; index ++)
        {
            /* Generates numbers from 0 to 9 (ASCII 48-58) */
            token[index] = (char)_random.Next(48, 58);
        }

        string tokenValue = new string(token);
        DateTime expirationDate = DateTime.UtcNow.AddMinutes(_expirationMinutes);

        return new ConfirmationToken
        {
            Token = tokenValue,
            ExpirationDate = expirationDate
        };
    }
}