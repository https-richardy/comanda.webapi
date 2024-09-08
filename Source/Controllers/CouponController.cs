namespace Comanda.WebApi.Controllers;

[ApiController]
[Route("api/coupons")]
[Authorize(Roles = "Administrator")]
public sealed class CouponController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateCouponAsync(CouponCreationRequest request)
    {
        var response = await mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }
}