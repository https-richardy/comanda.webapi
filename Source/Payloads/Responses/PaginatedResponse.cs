namespace Comanda.WebApi.Payloads;

public class PaginatedResponse<TData> : Response
    where TData : PaginationHelper<TData>
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TData? Data { get; set; }

    public PaginatedResponse(TData data)
    {
        Data = data;
    }
}