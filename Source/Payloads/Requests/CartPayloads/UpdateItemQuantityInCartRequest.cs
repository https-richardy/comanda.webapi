namespace Comanda.WebApi.Payloads;

public sealed record UpdateItemQuantityInCartRequest : IRequest<Response>
{
    public int ProductId { get; init; }
    public int NewQuantity { get; init; }
}