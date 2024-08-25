namespace Comanda.WebApi.Services;

public interface IGeminiService
{
    Task<string> GenerateContent(string prompt);
}