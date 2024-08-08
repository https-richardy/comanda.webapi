namespace Comanda.WebApi.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize(Roles = "Administrator")]
public sealed class OrderController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetOrdersAsync([FromQuery] FetchOrdersRequest request)
    {
        var response = await mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }
}