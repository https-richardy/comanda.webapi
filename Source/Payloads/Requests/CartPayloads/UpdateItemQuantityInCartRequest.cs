namespace Comanda.WebApi.Payloads;

public sealed record UpdateItemQuantityInCartRequest : IRequest<Response>
{
    public int ItemId { get; init; }
    public int NewQuantity { get; init; }
}