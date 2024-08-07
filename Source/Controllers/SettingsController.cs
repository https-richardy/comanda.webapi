namespace Comanda.WebApi.Controllers;

[ApiController]
[Route("api/settings")]
public sealed class SettingsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetSettingsAsync()
    {
        var response = await mediator.Send((SettingsDetailsRequest) new());
        return StatusCode(response.StatusCode, response);
    }
}