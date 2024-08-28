namespace Comanda.TestingSuite.UnitTests.Helpers;

public sealed class HostInformationProviderHelperTests
{
    private readonly IHostInformationProvider _hostInformation;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;

    public HostInformationProviderHelperTests()
    {
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _hostInformation = new HostInformationProvider(
            contextAccessor: _httpContextAccessorMock.Object
        );
    }

    [Fact(DisplayName = "Should return http protocol when request is not null")]
    public void ShouldReturnEmptyStringForHttpProtocolWhenRequestIsNull()
    {
        _httpContextAccessorMock.Setup(accessor => accessor.HttpContext)
            .Returns((HttpContext)null!);

        var result = _hostInformation.HttpProtocol;
        Assert.Equal(string.Empty, result); 
    }

    [Fact(DisplayName = "Should return correct http protocol when request is not null and scheme is present")]
    public void ShouldReturnCorrectHttpProtocolWhenRequestIsNotNullAndSchemeIsPresent()
    {
        var requestMock = new Mock<HttpRequest>();
        var httpContextMock = new Mock<HttpContext>();

        requestMock
            .Setup(request => request.Scheme)
            .Returns("https");

        httpContextMock.
            Setup(context => context.Request)
            .Returns(requestMock.Object);

        _httpContextAccessorMock
            .Setup(accessor => accessor.HttpContext)
            .Returns(httpContextMock.Object);

        var hostInformation = new HostInformationProvider(_httpContextAccessorMock.Object);

        var result = hostInformation.HttpProtocol;
        Assert.Equal("https", result);
    }

    [Fact(DisplayName = "Should return correct host address when request is not null and host is present")]
    public void ShouldReturnCorrectHostAddressWhenRequestIsNotNullAndHostIsPresent()
    {
        var requestMock = new Mock<HttpRequest>();
        var httpContextMock = new Mock<HttpContext>();

        requestMock
            .Setup(request => request.Scheme)
            .Returns("https");

        requestMock
            .Setup(request => request.Host)
            .Returns(new HostString("example.com"));

        httpContextMock.
            Setup(context => context.Request)
            .Returns(requestMock.Object);


        _httpContextAccessorMock
            .Setup(context => context.HttpContext!.Request)
            .Returns(requestMock.Object);

        _httpContextAccessorMock
            .Setup(accessor => accessor.HttpContext)
            .Returns(httpContextMock.Object);

        var result = _hostInformation.HostAddress;
        Assert.Equal("https://example.com", result);
    }
}