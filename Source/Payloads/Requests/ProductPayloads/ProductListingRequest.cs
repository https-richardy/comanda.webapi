namespace Comanda.WebApi.Payloads;

public sealed record ProductListingRequest :
    IRequest<Response<PaginationHelper<Product>>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public string? Title { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
}