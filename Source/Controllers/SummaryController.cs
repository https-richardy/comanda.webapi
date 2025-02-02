namespace Comanda.WebApi.Controllers;

[ApiController]
[Route("api/summary")]
public sealed class SummaryController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetDailySummaryAsync()
    {
        var response = await mediator.Send((DailySummaryRequest) new());
        return StatusCode(response.StatusCode, response);
    }
}