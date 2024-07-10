namespace Comanda.TestingSuite.ServicesTestSuite.JwtServiceTest;

public sealed class JwtServiceTest
{
    private readonly IJwtService _jwtService;
    private readonly Mock<IConfiguration> _configuration;
    private readonly IFixture _fixture;

    public JwtServiceTest()
    {
        _configuration = new Mock<IConfiguration>();
        _configuration.Setup(configuration => configuration["JwtSettings:SecretKey"])
            .Returns(Guid.NewGuid().ToString());

        _jwtService = new JwtService(_configuration.Object);

        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact(DisplayName = "JwtService.GenerateToken() should generate a token")]
    public void GenerateToken_ShouldGenerateToken()
    {
        var claimsIdentity = _fixture.Create<ClaimsIdentity>();
        var token = _jwtService.GenerateToken(claimsIdentity);

        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact(DisplayName = "JwtService.JwtOptions custom security algorithm should set correctly")]
    public void JwtOptions_CustomSecurityAlgorithm_ShouldSetCorrectly()
    {
        var customSecurityAlgorithm = SecurityAlgorithms.Sha512;
        var options = new JwtOptions { SecurityAlgorithm = customSecurityAlgorithm };

        var jwtService = new JwtService(_configuration.Object, options);
        var actualOptions = GetOptions(jwtService);

        Assert.Equal(customSecurityAlgorithm, actualOptions.SecurityAlgorithm);
    }

    [Fact(DisplayName = "JwtService.JwtOptions custom expires days should set correctly")]
    public void JwtOptions_CustomExpiresDays_ShouldSetCorrectly()
    {
        var customExpires = DateTime.UtcNow.AddDays(15);
        var options = new JwtOptions { Expires = customExpires };

        var jwtService = new JwtService(_configuration.Object, options);
        var actualOptions = GetOptions(jwtService);

        Assert.Equal(customExpires, actualOptions.Expires);
    }

    [Fact(DisplayName = "Constructor with custom options should initialize options correctly")]
    public void Constructor_CustomOptions_ShouldInitializeOptionsCorrectly()
    {
        var customOptions = new JwtOptions
        {
            Key = Encoding.ASCII.GetBytes(Guid.NewGuid().ToString()),
            SecurityAlgorithm = SecurityAlgorithms.Sha512,
            Expires = DateTime.UtcNow.AddDays(3)
        };

        var jwtService = new JwtService(_configuration.Object, customOptions);
        var options = GetOptions(jwtService);

        Assert.Equal(customOptions.SecurityAlgorithm, options.SecurityAlgorithm);
        Assert.Equal(customOptions.Expires, options.Expires);
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