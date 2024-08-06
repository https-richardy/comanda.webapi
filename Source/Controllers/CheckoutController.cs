namespace Comanda.WebApi.Controllers;

[ApiController]
[Route("api/checkout")]
public sealed class CheckoutController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> CreateCheckoutSessionAsync()
    {
        var response = await mediator.Send((CheckoutRequest) new());
        return StatusCode(response.StatusCode, response);
    }
}