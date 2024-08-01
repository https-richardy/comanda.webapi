namespace Comanda.WebApi.Payloads;

public sealed record AdditionalsListingByCategoryRequest
    : IRequest<Response<IEnumerable<Additional>>>
{
    public int CategoryId { get; init; }
}