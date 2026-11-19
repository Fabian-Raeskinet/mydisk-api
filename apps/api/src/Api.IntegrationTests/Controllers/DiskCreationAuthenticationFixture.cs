using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Moq;
using MyDisks.Domain.Disks;
using MyDisks.IntegrationTests.Api;

namespace MyDisks.Api.IntegrationTests.Controllers;

public sealed class DiskCreationAuthenticationFixture : IDisposable
{
    private const string Issuer = "https://keycloak.test/realms/mydisk";
    private const string Audience = "mydisk-api";
    private static readonly SymmetricSecurityKey SigningKey = new(RandomNumberGenerator.GetBytes(32));
    private readonly JwtApplicationFactory _factory = new();
    private readonly HttpClient _client;

    public DiskCreationAuthenticationFixture()
    {
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
    }

    [Theory]
    [InlineData("account-a")]
    [InlineData("account-b")]
    public async Task Creates_disk_with_creator_from_validated_access_token(string subject)
    {
        using var response = await CreateDiskAsync(subject);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        _factory.DiskRepositoryMock.Verify(diskRepository => diskRepository.CreateDiskAsync(
            It.Is<Disk>(disk => disk.CreatedByIssuer == Issuer && disk.CreatedBySubject == subject)), Times.Once);
    }

    [Fact]
    public async Task Rejects_authenticated_creation_without_subject()
    {
        using var response = await CreateDiskAsync(subject: null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        _factory.DiskRepositoryMock.Verify(repository => repository.CreateDiskAsync(It.IsAny<Disk>()), Times.Never);
    }

    [Theory]
    [InlineData("https://keycloak.test/realms/another-realm", Audience)]
    [InlineData(Issuer, "another-api")]
    public async Task Rejects_access_token_for_wrong_issuer_or_audience(string issuer, string audience)
    {
        using var response = await CreateDiskAsync("account-a", issuer, audience);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        _factory.DiskRepositoryMock.Verify(repository => repository.CreateDiskAsync(It.IsAny<Disk>()), Times.Never);
    }

    private async Task<HttpResponseMessage> CreateDiskAsync(string? subject, string issuer = Issuer,
        string audience = Audience)
    {
        Claim[] claims = subject is null ? [] : [new Claim("sub", subject)];
        var jwt = new JwtSecurityToken(issuer, audience, claims, expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256));
        var token = new JwtSecurityTokenHandler().WriteToken(jwt);
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/disk/create-disk");
        request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
        request.Content = JsonContent.Create(new
        {
            name = "Test disk",
            releaseDate = new DateTime(2020, 1, 1),
            createdByIssuer = "https://example.test",
            createdBySubject = "from-request-body"
        });

        return await _client.SendAsync(request);
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    private sealed class JwtApplicationFactory : CustomWebApplicationFactory
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureHostConfiguration(configuration => configuration.AddInMemoryCollection(
                new Dictionary<string, string?> { ["Authentication:Authority"] = Issuer }));
            return base.CreateHost(builder);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
                services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    var metadata = new OpenIdConnectConfiguration { Issuer = Issuer };
                    metadata.SigningKeys.Add(SigningKey);
                    options.Configuration = metadata;
                });
            });
        }
    }
}
