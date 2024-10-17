namespace Comanda.WebApi;

public partial class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;

        builder.Services.ConfigureServices(configuration);

        var app = builder.Build();

        await app.SetupPipeline(app.Environment);
        app.Run();
    }
}
