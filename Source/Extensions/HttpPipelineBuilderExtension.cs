namespace Comanda.WebApi.Extensions;

[ExcludeFromCodeCoverage]
internal static class HttpPipelineBuilderExtension
{
    public static void SetupPipeline(this IApplicationBuilder app, IWebHostEnvironment hostingEnvironment)
    {
        app.UseHttpsRedirection();

        if (hostingEnvironment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.Bootstrap();
        }

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