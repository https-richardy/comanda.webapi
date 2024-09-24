namespace Comanda.WebApi.Handlers;

public sealed class ProfileInformationHandler(
    UserManager<Account> userManager,
    IUserContextService userContextService
) : IRequestHandler<ProfileInformationRequest, Response<ProfileInformation>>
{
    public async Task<Response<ProfileInformation>> Handle(
        ProfileInformationRequest request,
        CancellationToken cancellationToken
    )
    {
        var userIdentifier = userContextService.GetCurrentUserIdentifier();
        var profile = await userManager.FindByIdAsync(userIdentifier!);

        return new Response<ProfileInformation>(
            data: (ProfileInformation) profile!,
            statusCode: StatusCodes.Status200OK,
            message: "Profile information retrieved successfully."
        );
    }
}