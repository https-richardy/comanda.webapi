namespace Comanda.WebApi.Payloads;

public sealed record ModifyCartItemQuantityRequest : IRequest<Response>
{
    public int ProductId { get; init; }
    public ChangeItemQuantityAction Action { get; init; }
}