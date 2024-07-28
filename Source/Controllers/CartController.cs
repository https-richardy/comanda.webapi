#pragma warning disable CS8601

namespace Comanda.WebApi.Controllers;

[ApiController]
[Route("api/cart")]
public sealed class CartController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetCustomerCartAsync()
    {
        var request = new GetCartDetailsRequest
        {
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        };

        var response = await mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("add-item")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> AddItemToCustomerCartAsync(InsertProductIntoCartRequest request)
    {
        request.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var response = await mediator.Send(request);

        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("modify-item-quantity")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> ModifyItemQuantityAsync(ModifyCartItemQuantityRequest request)
    {
        var response = await mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("update-item")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> UpdateItemQuantityAsync(UpdateItemQuantityInCartRequest request)
    {
        var response = await mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }
}