namespace Comanda.WebApi.Payloads;

public sealed record AdditionalCreationRequest : IRequest<Response>
{
    public string Name { get; init; }
    public decimal Price { get; init; }
    public int CategoryId { get; init; }
}