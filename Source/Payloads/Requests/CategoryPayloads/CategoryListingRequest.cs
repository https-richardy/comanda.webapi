namespace Comanda.WebApi.Payloads;

public sealed record CategoryListingRequest :
    IRequest<Response<IEnumerable<Category>>>
{
    public string? Title { get; init; }
}