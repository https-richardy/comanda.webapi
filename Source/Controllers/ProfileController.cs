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

    [HttpGet("orders")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetCurrentOrdersAsync()
    {
        var response = await mediator.Send((CustomerCurrentOrdersRequest)new());
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("orders/history")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetOrderHistoryAsync([FromQuery] CustomerOrderHistoryRequest request)
    {
        var response = await mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }
}