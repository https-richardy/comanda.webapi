using Microsoft.Data.SqlClient;

namespace Comanda.WebApi.Services;

public sealed class SeedService
{
    private readonly ILogger<SeedService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly string _seedFolderPath = string.Empty;

    public SeedService(
        ILogger<SeedService> logger,
        IWebHostEnvironment hostEnvironment,
        IConfiguration configuration
    )
    {
        _logger = logger;
        _hostEnvironment = hostEnvironment;
        _configuration = configuration;

        _seedFolderPath = Path.Combine(_hostEnvironment.ContentRootPath, "Data/Scripts/Seeds");
    }

    public async Task SeedAsync()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        var files = Directory.GetFiles(_seedFolderPath, "*.sql");

        foreach (var file in files)
        {
            var scriptName = Path.GetFileName(file);
            _logger.LogInformation("Executing seed script: {scriptName}", scriptName);

            await ExecuteScriptAsync(connectionString!, File.ReadAllText(file));
        }
    }

    private async Task ExecuteScriptAsync(string connectionString, string script)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(script, connection))
            {
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}