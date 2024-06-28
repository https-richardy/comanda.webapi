namespace Comanda.WebApi.Payloads;

public record GetEstablishmentProductsRequest : IRequest<Response<PaginationHelper<Product>>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    [JsonIgnore]
    public int EstablishmentId { get; set; }
}