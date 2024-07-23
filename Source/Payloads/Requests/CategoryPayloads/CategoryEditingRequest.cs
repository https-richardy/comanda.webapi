namespace Comanda.WebApi.Payloads;

public sealed record CategoryEditingRequest : IRequest<Response>
{
    public string Title { get; init; }

    [JsonIgnore]
    public int CategoryId { get; set; }
}