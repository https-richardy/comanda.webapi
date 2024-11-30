namespace Comanda.WebApi.Payloads;

public sealed record AdditionalsListingByCategoryRequest
    : IRequest<Response<IEnumerable<FormattedAdditional>>>
{
    public int CategoryId { get; init; }
}