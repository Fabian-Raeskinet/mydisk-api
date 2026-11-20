using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MyDisks.Domain.Disks;
using MyDisks.Domain.Reviews;
using MyDisks.IntegrationTests.Api;

namespace MyDisks.Api.IntegrationTests.Controllers;

public sealed class AdminReviewsFixture : IDisposable
{
    private const string ReviewsPath = "/api/admin/reviews";
    private readonly AdminApplicationFactory _factory = new();
    private readonly HttpClient _client;

    public AdminReviewsFixture()
    {
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
    }

    [Theory]
    [InlineData(false, false, HttpStatusCode.Unauthorized)]
    [InlineData(true, false, HttpStatusCode.Unauthorized)]
    [InlineData(false, true, HttpStatusCode.Forbidden)]
    [InlineData(true, true, HttpStatusCode.Forbidden)]
    public async Task Moderation_requires_the_admin_role(bool archive, bool authenticated, HttpStatusCode status)
    {
        if (authenticated)
            Authenticate("member");

        using var response = archive
            ? await _client.PostAsync(ArchivePath(Guid.NewGuid()), null)
            : await _client.GetAsync(ReviewsPath);

        response.StatusCode.Should().Be(status);
        _factory.Reviews.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Admin_can_list_reviews_for_another_creators_disk()
    {
        Authenticate("member", "admin");
        var review = CreateReview();
        _factory.Reviews.Setup(repository => repository.GetReviewsWithDisksAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([review]);

        using var response = await _client.GetAsync(ReviewsPath);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (response.Headers.CacheControl?.NoStore).Should().BeTrue();
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var result = json.RootElement.EnumerateArray().Single();
        result.GetProperty("id").GetGuid().Should().Be(review.Id);
        result.GetProperty("diskId").GetGuid().Should().Be(review.DiskId);
        result.GetProperty("diskName").GetString().Should().Be(review.Disk!.Name.Value);
        result.GetProperty("title").GetString().Should().Be(review.Title);
        result.GetProperty("content").GetString().Should().Be(review.Content);
        result.GetProperty("note").GetDouble().Should().Be(review.Note);
        result.GetProperty("status").GetString().Should().Be(nameof(ReviewStatus.Pending));
    }

    [Fact]
    public async Task Admin_can_archive_a_review_and_repeating_the_request_does_not_update_it_again()
    {
        Authenticate("admin");
        var review = CreateReview();
        _factory.Reviews.Setup(repository => repository.GetReviewByFilterAsync(
                It.IsAny<Expression<Func<Review, bool>>>()))
            .ReturnsAsync((Expression<Func<Review, bool>> filter) => filter.Compile()(review) ? review : null);

        using var first = await _client.PostAsync(ArchivePath(review.Id), null);
        using var second = await _client.PostAsync(ArchivePath(review.Id), null);

        first.StatusCode.Should().Be(HttpStatusCode.NoContent);
        second.StatusCode.Should().Be(HttpStatusCode.NoContent);
        review.Status.Should().Be(ReviewStatus.Archived);
        _factory.Reviews.Verify(repository => repository.UpdateReviewAsync(review), Times.Once);
    }

    [Fact]
    public async Task Archiving_an_unknown_review_returns_not_found()
    {
        Authenticate("admin");

        using var response = await _client.PostAsync(ArchivePath(Guid.NewGuid()), null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        _factory.Reviews.Verify(repository => repository.UpdateReviewAsync(It.IsAny<Review>()), Times.Never);
    }

    private void Authenticate(params string[] roles) => _client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme,
            JwtApplicationFactory.CreateAccessToken("viewer", roles: roles));

    private static string ArchivePath(Guid id) => $"{ReviewsPath}/{id}/archive";

    private static Review CreateReview()
    {
        var disk = new Disk
        {
            Id = Guid.NewGuid(),
            Name = new Name("A private disk"),
            CreatedByIssuer = JwtApplicationFactory.Issuer,
            CreatedBySubject = "creator"
        };
        return new Review
        {
            Id = Guid.NewGuid(),
            DiskId = disk.Id,
            Disk = disk,
            Title = "An opinion",
            Content = "This is a review.",
            Note = 3.5
        };
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    private sealed class AdminApplicationFactory : JwtApplicationFactory
    {
        public Mock<IReviewRepository> Reviews { get; } = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureTestServices(services => services.AddSingleton(Reviews.Object));
        }
    }
}
