/* 
    using aliases because of the ambiguous reference between 
    FluentValidation.Results.ValidationFailure and Microsoft.IdentityModel.Tokens.ValidationFailure
*/

using ValidationFailure = FluentValidation.Results.ValidationFailure;

namespace Comanda.WebApi.Payloads;

/* To support a validation error response in api responses with payload (TData) */
public sealed class ValidationFailureResponse<TData> : Response<TData>
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<ValidationFailure> Errors { get; set; }

    public ValidationFailureResponse(IEnumerable<ValidationFailure> errors)
    {
        StatusCode = StatusCodes.Status400BadRequest;
        Message = "We couldn't process your request because some fields are missing or invalid. Please review the details and try again.";
        Errors = errors;
    }
}