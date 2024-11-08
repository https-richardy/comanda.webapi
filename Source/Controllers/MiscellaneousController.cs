namespace Comanda.WebApi.Controllers;

[ExcludeFromCodeCoverage]
[ApiController]
[Route("api/miscellaneous")]
public sealed class MiscellaneousController : ControllerBase
{
    private readonly ILogger<MiscellaneousController> _logger;
    private readonly SeedService _seedService;

    public MiscellaneousController(
        ILogger<MiscellaneousController> logger,
        SeedService seedService
    )
    {
        _logger = logger;
        _seedService = seedService;
    }

    [HttpPost("seed")]
    public async Task<IActionResult> SeedDatabaseAsync()
    {
        try
        {
            _logger.LogInformation("Starting database seeding...");
            await _seedService.SeedAsync();
            _logger.LogInformation("Database seeding completed successfully.");
            return Ok("Database seeding completed successfully.");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An error occurred while seeding the database.");
            return StatusCode(500, "An error occurred while seeding the database.");
        }
    }
}