namespace Comanda.WebApi.Handlers;

public sealed class ProfileExportDataHandler(
    IProfileDataExportService profileDataExportService,
    IUserContextService userContextService
) : IRequestHandler<ProfileDataExportRequest, Response<ProfileExportData>>
{
    public async Task<Response<ProfileExportData>> Handle(
        ProfileDataExportRequest request,
        CancellationToken cancellationToken
    )
    {
        var userIdentifier = userContextService.GetCurrentUserIdentifier();
        var profileData = await profileDataExportService.ExportDataAsync(userIdentifier!);

        return new Response<ProfileExportData>(
            data: profileData,
            statusCode: StatusCodes.Status200OK,
            message: "Profile data exported successfully."
        );
    }
}
