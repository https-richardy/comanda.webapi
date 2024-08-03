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

    [HttpPost("items/")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> AddItemToCustomerCartAsync(InsertProductIntoCartRequest request)
    {
        request.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var response = await mediator.Send(request);

        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("items/")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> UpdateItemQuantityAsync(UpdateItemQuantityInCartRequest request)
    {
        var response = await mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("items/{itemId}")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> DeleteItemAsync(int itemId)
    {
        var request = new CartItemDeletionRequest { ItemId = itemId };
        var response = await mediator.Send(request);

        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("items/{itemId}/increment")]
    public async Task<IActionResult> IncrementCartItemQuantityAsync(int itemId)
    {
        var request = new IncrementCartItemQuantityRequest
        {
            ItemId = itemId
        };

        var response = await mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("items/{itemId}/decrement")]
    public async Task<IActionResult> DecrementCartItemQuantityAsync(int itemId)
    {
        var request = new DecrementCartItemQuantityRequest
        {
            ItemId = itemId
        };

        var response = await mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }
}