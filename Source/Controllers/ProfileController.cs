namespace Comanda.WebApi.Controllers;

[ApiController]
[Route("api/profile")]
public sealed class ProfileController(IMediator mediator) : ControllerBase
{
    [HttpPost("addresses")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> RegisterNewAddressAsync(NewAddressRegistrationRequest request)
    {
        var response = await mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }
}