using System.Net.Http.Headers;

namespace MyDisks.IntegrationTests.Api;

public class ControllerFixtureBase : IDisposable
{
    protected readonly CustomWebApplicationFactory _factory;
    protected readonly HttpClient _client;

    public ControllerFixtureBase()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthenticationHandler.SchemeName);
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
