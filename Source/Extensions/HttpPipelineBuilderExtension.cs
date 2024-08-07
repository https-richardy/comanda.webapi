namespace Comanda.WebApi.Extensions;

public static class HttpPipelineBuilderExtension
{
    public static void SetupPipeline(this IApplicationBuilder app, IWebHostEnvironment hostingEnvironment)
    {
        app.ConfigureCORS();
        app.UseHttpsRedirection();

        if (hostingEnvironment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.Bootstrap();
        }

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHub<NotificationHub>("/notification");
        });


        app.UseStaticFiles();
    }
}