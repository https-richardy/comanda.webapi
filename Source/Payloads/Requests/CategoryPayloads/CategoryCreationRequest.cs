namespace Comanda.WebApi.Payloads;

public sealed class CategoryCreationRequest : IRequest<Response>
{
    public string Title { get; init; }
}