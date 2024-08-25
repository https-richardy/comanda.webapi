namespace Comanda.WebApi.Payloads;

public sealed record RecommendationResponse
{
    public string Suggestion { get; init; }
}