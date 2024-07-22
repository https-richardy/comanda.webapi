namespace Comanda.WebApi.Extensions;

public static class HttpPipelineBuilderExtension
{
    public static void SetupHttpPipeline(this IApplicationBuilder app, IWebHostEnvironment hostingEnvironment)
    {
        app.ConfigureCORS();
        app.UseHttpsRedirection();

        if (hostingEnvironment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.Bootstrap();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseStaticFiles();
    }
}