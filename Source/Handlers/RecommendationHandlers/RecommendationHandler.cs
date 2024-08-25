namespace Comanda.WebApi.Handlers;

public sealed class RecommendationHandler(
    IRecommendationService recommendationService,
    IUserContextService userContextService,
    ICustomerRepository customerRepository
) : IRequestHandler<RecommendationRequest, Response<RecommendationResponse>>
{
    public async Task<Response<RecommendationResponse>> Handle(
        RecommendationRequest request,
        CancellationToken cancellationToken
    )
    {
        var userIdentifier = userContextService.GetCurrentUserIdentifier();
        var customer = await customerRepository.FindCustomerByUserIdAsync(userIdentifier!);

        var suggestion = await recommendationService.RecommendAsync(customer!);
        var payload = new RecommendationResponse { Suggestion = suggestion };

        return new Response<RecommendationResponse>(
            data: payload,
            statusCode: StatusCodes.Status200OK,
            message: "Recommendation generated successfully."
        );
    }
}
