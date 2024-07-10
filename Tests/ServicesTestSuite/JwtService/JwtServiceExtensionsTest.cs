namespace Comanda.TestingSuite.ServicesTestSuite.JwtServiceTest;

public sealed class JwtServiceExtensionsTest
{
    private readonly IServiceCollection _services;
    private readonly Mock<IConfiguration> _configuration;
    private ServiceProvider _serviceProvider;

    #pragma warning disable CS8618
    public JwtServiceExtensionsTest()
    {
        _services = new ServiceCollection();

        _configuration = new Mock<IConfiguration>();
        _configuration.Setup(configuration => configuration["JwtSettings:SecretKey"])
            .Returns(Guid.NewGuid().ToString());

        _services.AddScoped<IConfiguration>(_ => _configuration.Object);
    }

    [Fact(DisplayName = "services.AddJwtBearer should ad JwtService to the service collection")]
    public void AddJwtBearer_ShouldAddJwtServicesToCollection()
    {
        _services.AddJwtBearer(_configuration.Object);
        _serviceProvider = _services.BuildServiceProvider();

        var jwtService = _serviceProvider.GetRequiredService<IJwtService>();
        Assert.NotNull(jwtService);
    }

    [Fact(DisplayName = "services.AddJwtBearer with custom options should configure options correctly")]
    public void AddJwtBearer_WithCustomOptions_ShouldConfigureOptionsCorrectly()
    {
        var customExpiresDays = DateTime.UtcNow.AddDays(15);
        var customSecurityAlghorithm = SecurityAlgorithms.Sha256;

        _services.AddJwtBearer(_configuration.Object, options =>
        {
            options.Expires = customExpiresDays;
            options.SecurityAlgorithm = customSecurityAlghorithm;
        });

        _serviceProvider = _services.BuildServiceProvider();


        var jwtService = (JwtService)_serviceProvider.GetRequiredService<IJwtService>();
        var actualOptions = GetOptions(jwtService);

        Assert.Equal(customExpiresDays, actualOptions.Expires);
        Assert.Equal(customSecurityAlghorithm, actualOptions.SecurityAlgorithm);
    }

    #pragma warning disable CS8600, CS8603
    /*
        Private method to access the _options field using reflection.
        This method is created to test if the JwtService initializes its options correctly without directly exposing
        the private field _options in the test code. It allows us to maintain encapsulation while enabling
        unit testing of the initialization logic.
    */
    private JwtOptions GetOptions(JwtService jwtService)
    {
        var optionsField = typeof(JwtService).GetField("_options", BindingFlags.NonPublic | BindingFlags.Instance);
        return (JwtOptions)optionsField?.GetValue(jwtService);
    }
    #pragma warning restore CS8600, CS8603
}