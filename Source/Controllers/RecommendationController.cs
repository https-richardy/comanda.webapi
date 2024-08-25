namespace Comanda.WebApi.Controllers;

[ApiController]
[Route("api/recommendation")]
public sealed class RecommendationController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetRecommendation()
    {
        var response = await mediator.Send((RecommendationRequest) new());
        return StatusCode(response.StatusCode, response);
    }
}