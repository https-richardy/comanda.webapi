namespace Comanda.WebApi.Extensions;

[ExcludeFromCodeCoverage]
internal static class HttpPipelineBuilderExtension
{
    public static async Task SetupPipeline(this IApplicationBuilder app, IWebHostEnvironment hostingEnvironment)
    {
        app.UseHttpsRedirection();

        if (hostingEnvironment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        await app.Bootstrap();

        app.UseRouting();
        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHub<NotificationHub>("/notification")
                     .RequireCors("RestrictedHubPolicy");
        });


        app.UseStaticFiles();
    }
}