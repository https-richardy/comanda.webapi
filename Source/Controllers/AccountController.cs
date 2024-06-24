namespace Comanda.WebApi.Controllers;

[Route("api/accounts")]
[ApiController]
public sealed class AccountController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAccountAsync(AccountRegistrationRequest request)
    {
        var response = await mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }
}