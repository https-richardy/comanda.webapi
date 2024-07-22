namespace Comanda.WebApi;

internal static class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;

        builder.Services.ConfigureServices(configuration);

        var app = builder.Build();

        app.SetupPipeline(app.Environment);
        app.Run();
    }
}
