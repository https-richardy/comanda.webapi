namespace Comanda.WebApi.Services;

public sealed class GeminiService(IGeminiClient geminiClient) : IGeminiService
{
    public async Task<string> GenerateContent(string prompt)
    {
        var response = await geminiClient.TextPrompt(prompt);
        var firstCandidate = response!.Candidates.First();
        var content = firstCandidate.Content.Parts.First();

        return content.Text;
    }
}