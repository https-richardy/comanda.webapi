namespace Comanda.WebApi.Payloads;

public sealed class ValidationFailureResponse : Response
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ValidationResult Errors { get; set; }

    public ValidationFailureResponse(ValidationResult errors)
    {
        StatusCode = StatusCodes.Status400BadRequest;
        Message = "We couldn't process your request because some fields are missing or invalid. Please review the details and try again.";
        Errors = errors;
    }
}