namespace Comanda.WebApi.Payloads;

public sealed record CategoryEditingRequest : IRequest<Response>
{
    public string Title { get; init; }
    public int CategoryId { get; set; }
}