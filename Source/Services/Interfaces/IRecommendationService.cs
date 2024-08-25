namespace Comanda.WebApi.Services;

public interface IRecommendationService
{
    Task<string> RecommendAsync(Customer customer);
}