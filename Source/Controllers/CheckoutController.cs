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

    [HttpGet("success")]
    public async Task<IActionResult> HandleSuccessfulPaymentAsync([FromQuery] SuccessfulPaymentRequest request)
    {
        var response = await mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("cancel")]
    public async Task<IActionResult> HandleCanceledPaymentAsync([FromQuery] CanceledPaymentRequest request)
    {
        var response = await mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }
}