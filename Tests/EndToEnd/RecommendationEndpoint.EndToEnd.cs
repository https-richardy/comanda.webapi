using DotnetGeminiSDK.Client.Interfaces;
using DotnetGeminiSDK.Model.Response;

namespace Comanda.TestingSuite.EndToEnd;

public sealed class RecommendationEndpointEndToEnd :
    IClassFixture<ApiIntegrationBase<Program, ComandaDbContext>>,
    IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private readonly IFixture _fixture;
    private readonly ApiIntegrationBase<Program, ComandaDbContext> _factory;

    public RecommendationEndpointEndToEnd(ApiIntegrationBase<Program, ComandaDbContext> factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();


        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact(DisplayName = "Given a valid request, when requesting a recommendation, it should return 200 OK with a suggestion")]
    public async Task GivenAValidRequest_WhenRequestingRecommendation_ThenItShouldReturnOkWithSuggestion()
    {
        // arrange: Mock the recommendation service
        var recommendationServiceMock = new Mock<IRecommendationService>();
        recommendationServiceMock
            .Setup(service => service.RecommendAsync(It.IsAny<Customer>()))
            .ReturnsAsync("Hey John, how about trying a Cheeseburger today?");

        // arrange: Mock the Gemini service
        var geminiServiceMock = new Mock<IGeminiService>();
        geminiServiceMock
            .Setup(service => service.GenerateContent(It.IsAny<string>()))
            .ReturnsAsync("Hey John, how about trying a Cheeseburger today?");

        var geminiMessageResponse = _fixture.Create<GeminiMessageResponse>();

        var geminiClientMock = new Mock<IGeminiClient>();
        geminiClientMock
            .Setup(client => client.TextPrompt(It.IsAny<string>(), null, null))
            .ReturnsAsync(geminiMessageResponse);

        // arrange: Remove the real services and add the mocks
        var scopedClient = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // remove the real GeminiClient
                var geminiClientDescriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IGeminiClient));
                if (geminiClientDescriptor is not null)
                    services.Remove(geminiClientDescriptor);

                services.AddScoped(provider => geminiClientMock.Object);

                // remove the real RecommendationService
                var recommendationServiceDescriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IRecommendationService));
                if (recommendationServiceDescriptor is not null)
                    services.Remove(recommendationServiceDescriptor);

                services.AddScoped(provider => recommendationServiceMock.Object);

                // remove the real GeminiService
                var geminiServiceDescriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IGeminiService));
                if (geminiServiceDescriptor is not null)
                    services.Remove(geminiServiceDescriptor);

                services.AddScoped(provider => geminiServiceMock.Object);
            });
        })
        .CreateClient();

        // Act: authenticate with the new password
        var authenticationRequest = new AuthenticationCredentials
        {
            Email = "john.doe@email.com",
            Password = "JohnDoe1234*"
        };

        var authenticationResponse = await scopedClient.PostAsJsonAsync("api/identity/authenticate", authenticationRequest);
        var authenticationResponseContent = await authenticationResponse.Content.ReadFromJsonAsync<Response<AuthenticationResponse>>();

        // Assert: verify successful authentication with the new password
        authenticationResponse.EnsureSuccessStatusCode();

        Assert.NotNull(authenticationResponseContent);
        Assert.NotNull(authenticationResponseContent.Data);
        Assert.NotNull(authenticationResponseContent.Data.Token);

        // set the authentication token in the request header
        scopedClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResponseContent.Data.Token);

        // act: Request a recommendation
        var response = await scopedClient.GetAsync("api/recommendation");
        var responseContent = await response.Content.ReadFromJsonAsync<Response<RecommendationResponse>>();

        // assert: Verify that the response is 200 OK and contains the suggestion
        response.EnsureSuccessStatusCode();

        Assert.NotNull(responseContent);
        Assert.NotNull(responseContent.Data);
        Assert.Equal("Hey John, how about trying a Cheeseburger today?", responseContent.Data.Suggestion);
    }

    public async Task DisposeAsync() => await Task.CompletedTask;
    public async Task InitializeAsync()
    {
        _factory.CleanUp();

        var signupCredentials = _fixture.Build<AccountRegistrationRequest>()
            .With(credential => credential.Name, "John Doe")
            .With(credential => credential.Email, "john.doe@email.com")
            .With(credential => credential.Password, "JohnDoe1234*")
            .Create();

        var response = await _httpClient.PostAsJsonAsync("api/identity/register", signupCredentials);
        response.EnsureSuccessStatusCode();
    }
}