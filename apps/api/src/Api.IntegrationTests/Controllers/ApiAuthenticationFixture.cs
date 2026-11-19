using System.Net;
using FluentAssertions;
using MyDisks.IntegrationTests.Api;

namespace MyDisks.Api.IntegrationTests.Controllers;

public sealed class ApiAuthenticationFixture : ControllerFixtureBase
{
    [Theory]
    [InlineData("GET", "/api/Disk/disks")]
    [InlineData("GET", "/api/Disk/disk-by-name?name=Test")]
    [InlineData("POST", "/api/Disk/create-disk")]
    [InlineData("POST", "/api/Disk/attach-author")]
    [InlineData("PUT", "/api/Disk/update-disk")]
    [InlineData("DELETE", "/api/Disk/delete-disk")]
    [InlineData("POST", "/api/Review")]
    [InlineData("POST", "/api/Review/Archive")]
    [InlineData("GET", "/api/Miscellaneous/disks")]
    [InlineData("GET", "/api/auth/me")]
    public async Task Business_routes_require_authentication(string method, string path)
    {
        using var client = _factory.CreateClient();
        using var request = new HttpRequestMessage(new HttpMethod(method), path);
        using var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData("/health/live")]
    [InlineData("/maintenance")]
    public async Task Operational_routes_allow_anonymous_access(string path)
    {
        using var client = _factory.CreateClient();
        using var response = await client.GetAsync(path);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
