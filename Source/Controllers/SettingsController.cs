namespace Comanda.WebApi.Controllers;

[ApiController]
[Route("api/settings")]
[Authorize(Roles = "Administrator")]
public sealed class SettingsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetSettingsAsync()
    {
        var response = await mediator.Send((SettingsDetailsRequest) new());
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateSettingsAsync(SettingsEditingRequest request)
    {
        var response = await mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }
}