namespace Comanda.WebApi.Controllers;

[ApiController]
[Route("api/checkout")]
public sealed class CheckoutController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> CreateCheckoutSessionAsync(CheckoutRequest request)
    {
        var response = await mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }
}