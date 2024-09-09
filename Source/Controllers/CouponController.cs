namespace Comanda.WebApi.Controllers;

[ApiController]
[Route("api/coupons")]
[Authorize(Roles = "Administrator")]
public sealed class CouponController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCouponsAsync()
    {
        var response = await mediator.Send((CouponListingRequest) new());
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCouponByIdAsync(int id)
    {
        var request = new FetchCouponByIdentifier { Id = id };
        var response = await mediator.Send(request);

        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCouponAsync(CouponCreationRequest request)
    {
        var response = await mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }
}