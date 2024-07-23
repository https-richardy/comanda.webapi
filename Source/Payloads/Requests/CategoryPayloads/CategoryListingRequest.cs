namespace Comanda.WebApi.Payloads;

public sealed record CategoryListingRequest :
    IRequest<Response<PaginationHelper<Category>>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public string? Title { get; init; }
}