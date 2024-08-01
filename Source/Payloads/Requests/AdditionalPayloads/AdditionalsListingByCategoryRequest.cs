namespace Comanda.WebApi.Payloads;

public sealed record AdditionalsListingByCategoryRequest
{
    public int CategoryId { get; init; }
}