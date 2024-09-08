namespace Comanda.WebApi.Controllers;

[ApiController]
[Route("api/coupons")]
public sealed class CouponController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateCouponAsync(CouponCreationRequest request)
    {
        var response = await mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }
}