using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MyDisks.Contracts.Disks;
using MyDisks.Domain.Authors;
using MyDisks.Domain.Disks;
using MyDisks.Domain.Reviews;
using MyDisks.IntegrationTests.Api;

namespace MyDisks.Api.IntegrationTests.Controllers;

public sealed class PrivateCollectionFixture : IDisposable
{
    private readonly Disk _aliceDisk = CreateDisk("Alice record", "alice");
    private readonly Disk _bobDisk = CreateDisk("Bob record", "bob");
    private readonly CollectionApplicationFactory _factory = new();

    public PrivateCollectionFixture()
    {
        var disks = new[]
        {
            _aliceDisk,
            _bobDisk,
            new Disk { Id = Guid.NewGuid(), Name = new Name("Legacy record") },
            CreateDisk("Other realm", "alice", "https://another.test/realms/mydisk")
        };
        _factory.DiskRepositoryMock
            .Setup(repository => repository.GetDisksAsync(It.IsAny<Expression<Func<Disk, bool>>>()))
            .ReturnsAsync((Expression<Func<Disk, bool>> filter) => disks.Where(filter.Compile()).ToList());
        _factory.DiskRepositoryMock
            .Setup(repository => repository.GetDiskByFilterAsync(It.IsAny<Expression<Func<Disk, bool>>>()))
            .ReturnsAsync((Expression<Func<Disk, bool>> filter) => disks.FirstOrDefault(filter.Compile()));
    }

    [Theory]
    [InlineData("alice", false, "Alice record")]
    [InlineData("bob", false, "Bob record")]
    [InlineData("alice", true, "Alice record")]
    public async Task Lists_only_disks_owned_by_the_current_identity(string subject, bool admin, string expectedName)
    {
        using var client = CreateClient(subject, admin);
        using var response = await client.GetAsync("/api/disk/disks");
        var disks = await response.Content.ReadFromJsonAsync<List<DiskResult>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.CacheControl!.NoStore.Should().BeTrue();
        disks.Should().ContainSingle().Which.Name.Should().Be(expectedName);
    }

    [Fact]
    public async Task Finds_owned_disk_by_name_without_archived_reviews()
    {
        _aliceDisk.AddReview(new Review { Id = Guid.NewGuid(), Title = "Visible", Content = "Content" });
        var archived = new Review { Id = Guid.NewGuid(), Title = "Archived", Content = "Content" };
        archived.Archive();
        _aliceDisk.AddReview(archived);

        using var client = CreateClient("alice", admin: false);
        using var response = await client.GetAsync("/api/disk/disk-by-name?name=Alice%20record");
        var disk = await response.Content.ReadFromJsonAsync<DiskResult>(new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() }
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.CacheControl!.NoStore.Should().BeTrue();
        disk!.Reviews.Should().ContainSingle().Which.Title.Should().Be("Visible");
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Rejects_access_to_another_accounts_disk_and_reviews(bool admin)
    {
        var bobsReview = new Review { Id = Guid.NewGuid(), Title = "Bob review", Content = "Content", Disk = _bobDisk };
        _factory.ReviewRepositoryMock
            .Setup(repository => repository.GetReviewByFilterAsync(It.IsAny<Expression<Func<Review, bool>>>()))
            .ReturnsAsync((Expression<Func<Review, bool>> filter) => filter.Compile()(bobsReview) ? bobsReview : null);

        using var client = CreateClient("alice", admin);
        using var detail = await client.GetAsync("/api/disk/disk-by-name?name=Bob%20record");
        using var update = await client.PutAsJsonAsync("/api/disk/update-disk", new { id = _bobDisk.Id, name = "Updated" });
        using var delete = await client.DeleteAsync($"/api/disk/delete-disk?diskId={_bobDisk.Id}");
        using var attach = await client.PostAsJsonAsync("/api/disk/attach-author",
            new { diskId = _bobDisk.Id, authorId = Guid.NewGuid() });
        using var createReview = await client.PostAsJsonAsync("/api/review",
            new { diskId = _bobDisk.Id, title = "New review", content = "Content", note = 4 });
        using var archiveReview = await client.PostAsJsonAsync("/api/review/archive", new { reviewId = bobsReview.Id });

        new[] { detail, update, delete, attach, createReview, archiveReview }
            .Should().OnlyContain(response => response.StatusCode == HttpStatusCode.NotFound);
        _factory.DiskRepositoryMock.Verify(repository => repository.UpdateDiskAsync(It.IsAny<Disk>()), Times.Never);
        _factory.DiskRepositoryMock.Verify(repository => repository.DeleteDiskAsync(It.IsAny<Disk>()), Times.Never);
        _factory.ReviewRepositoryMock.Verify(repository => repository.UpdateReviewAsync(It.IsAny<Review>()), Times.Never);
        _factory.AuthorRepositoryMock.Verify(repository => repository.GetAuthorByFilterAsync(
            It.IsAny<Expression<Func<Author, bool>>>()), Times.Never);
    }

    public void Dispose() => _factory.Dispose();

    private HttpClient CreateClient(string subject, bool admin)
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme,
            JwtApplicationFactory.CreateAccessToken(subject, roles: admin ? ["admin"] : []));
        return client;
    }

    private static Disk CreateDisk(string name, string owner, string issuer = JwtApplicationFactory.Issuer)
        => new()
        {
            Id = Guid.NewGuid(),
            Name = new Name(name),
            CreatedByIssuer = issuer,
            CreatedBySubject = owner
        };

    private sealed class CollectionApplicationFactory : JwtApplicationFactory
    {
        public Mock<IReviewRepository> ReviewRepositoryMock { get; } = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureTestServices(services => services.AddSingleton(ReviewRepositoryMock.Object));
        }
    }
}
