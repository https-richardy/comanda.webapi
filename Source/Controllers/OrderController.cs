namespace Comanda.WebApi.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrderController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetOrdersAsync([FromQuery] FetchOrdersRequest request)
    {
        var response = await mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{orderId}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetOrderDetailsAsync(int orderId)
    {
        var request = new OrderDetailsRequest { OrderId = orderId };
        var response = await mediator.Send(request);

        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("{orderId}/set-status")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> SetOrderStatus(SetOrderStatusRequest request, int orderId)
    {
        request.OrderId = orderId;

        var response = await mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("orderId/cancel")]
    [Authorize(Roles = "Administrator, Customer")]
    public async Task<IActionResult> CancelOrderAsync(OrderCancellationRequest request, [FromRoute] int orderId)
    {
        request.OrderId = orderId;

        var response = await mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }
}