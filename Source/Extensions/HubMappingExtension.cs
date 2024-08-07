namespace Comanda.WebApi.Extensions;

public static class HubMappingExtension
{
    public static void MapHubs(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHub<NotificationHub>("/notification");
        });
    }
}