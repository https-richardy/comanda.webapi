namespace Comanda.WebApi.Controllers;

[ApiController]
[Route("api/profile")]
public sealed class ProfileController(IMediator mediator) : ControllerBase
{
    [HttpGet("addresses")]
    public async Task<IActionResult> GetCustomerAddressesAsync()
    {
        var response = await mediator.Send((FetchCustomerAddressesRequest) new());
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("addresses")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> RegisterNewAddressAsync(NewAddressRegistrationRequest request)
    {
        var response = await mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("addresses/{addressId}")]
    public async Task<IActionResult> UpdateAddressAsync(AddressEditingRequest request, [FromRoute] int addressId)
    {
        request.AddressId = addressId;

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

    [HttpGet("orders/{orderId}")]
    public async Task<IActionResult> GetOrderDetailsAsync(int orderId)
    {
        var request = new CustomerOrderDetailsRequest { OrderId = orderId };
        var response = await mediator.Send(request);

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