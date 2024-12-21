namespace Comanda.WebApi.Services;

public interface ISummaryService
{
    Task<DailySummary> GetDailySummaryAsync();
}