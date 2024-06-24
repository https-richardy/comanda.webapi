namespace Comanda.WebApi.Payloads;

public class Response<TData>
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TData? Data { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; set; }

    public int StatusCode { get; set; } = 200;
    public bool IsSuccess => StatusCode >= 200 && StatusCode < 300;

    public Response() { }

    public Response(TData? data, int statusCode, string? message)
    {
        Data = data;
        StatusCode = statusCode;
        Message = message;
    }

    public static Response<TData> Success(TData data, string message = "")
    {
        return new Response<TData>
        {
            Data = data,
            Message = message,
            StatusCode = 200
        };
    }

    public static Response<TData> Error(int statusCode, string message)
    {
        return new Response<TData>
        {
            StatusCode = statusCode,
            Message = message
        };
    }
}

public class Response
{
    public int StatusCode { get; set; }
    public bool IsSuccess => StatusCode >= 200 && StatusCode < 300;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; set; }

    public Response() { }

    public Response(int statusCode, string? message)
    {
        StatusCode = statusCode;
        Message = message;
    }
}