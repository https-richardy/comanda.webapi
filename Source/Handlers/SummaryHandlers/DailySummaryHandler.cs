namespace Comanda.WebApi.Handlers;

public sealed class DailySummaryHandler :
    IRequestHandler<DailySummaryRequest, Response<DailySummary>>
{
    private readonly ISummaryService _summaryService;

    public DailySummaryHandler(ISummaryService summaryService)
    {
        _summaryService = summaryService;
    }

    public async Task<Response<DailySummary>> Handle(DailySummaryRequest request, CancellationToken cancellationToken)
    {
        var summary = await _summaryService.GetDailySummaryAsync();

        return new Response<DailySummary>(
            data: summary,
            statusCode: StatusCodes.Status200OK,
            message: "here's a summary of the day."
        );
    }
}