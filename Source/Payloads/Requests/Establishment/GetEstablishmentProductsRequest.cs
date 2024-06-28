namespace Comanda.WebApi.Payloads;

public record GetEstablishmentProductsRequest : IRequest<Response<PaginationHelper<Product>>>
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }

    [JsonIgnore]
    public int EstablishmentId { get; set; }
}