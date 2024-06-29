#pragma warning disable CS8601

namespace Comanda.WebApi.Controllers;

[ApiController]
[Route("api/carts")]
public sealed class CartController(IMediator mediator) : ControllerBase
{
    [HttpPost("add-item")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> AddItemToCustomerCartAsync(AddProductToCartRequest request)
    {
        request.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var response = await mediator.Send(request);

        return StatusCode(response.StatusCode, response);
    }
}