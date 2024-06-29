#pragma warning disable CS8601

namespace Comanda.WebApi.Controllers;

[ApiController]
[Route("api/carts")]
public sealed class CartController(IMediator mediator) : ControllerBase
{

}