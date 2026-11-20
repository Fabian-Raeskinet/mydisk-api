using FluentAssertions;
using Microsoft.Extensions.Options;
using MyDisks.Services.Reviews.Moderation;

namespace MyDisks.Services.Tests.Reviews;

public class KeywordReviewModeratorFixture
{
    private static readonly KeywordReviewModerator Sut = new(Options.Create(new ReviewModerationSettings()));

    [Theory]
    [InlineData("Instant classic", "The bassline alone is worth the price of the vinyl.")]
    [InlineData("Brave choice", "A kazoo solo in track 3? Bold, and it works.")]
    [InlineData("Late night spin", "Smooth like a waffle fresh out of the iron.")]
    [InlineData("No words", null)]
    public async Task Should_Approve_CleanReview(string title, string? content)
    {
        var result = await Sut.ModerateAsync(title, content, CancellationToken.None);

        result.Approved.Should().BeTrue();
    }

    [Theory]
    [InlineData("Meh", "The singer is a NINCOMPOOP.")]
    [InlineData("Son-of-a-Motherboard!", "I wanted my money back.")]
    [InlineData("Ouch", "Whoever mixed this is a tone-deaf walrus.")]
    [InlineData("Skip it", "Produced by a  kazoo   goblin, clearly.")]
    public async Task Should_Reject_ReviewContainingForbiddenPhrase(string title, string content)
    {
        var result = await Sut.ModerateAsync(title, content, CancellationToken.None);

        result.Approved.Should().BeFalse();
        result.Reason.Should().StartWith("Forbidden phrase:");
    }
}
