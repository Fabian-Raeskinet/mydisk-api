using System.Globalization;
using System.Text;
using Microsoft.Extensions.Options;

namespace MyDisks.Services.Reviews.Moderation;

public sealed class KeywordReviewModerator(IOptions<ReviewModerationSettings> settings) : IReviewModerator
{
    private static readonly string[] ForbiddenPhrases =
    [
        "nincompoop",
        "cabbage brain",
        "kazoo goblin",
        "trombone gremlin",
        "tone deaf walrus",
        "off key pigeon",
        "son of a motherboard",
        "soggy waffle",
        "lukewarm potato",
        "unseasoned tofu",
        "buffering human",
        "walking elevator music"
    ];

    public async Task<ReviewModerationResult> ModerateAsync(string title, string? content,
        CancellationToken cancellationToken)
    {
        if (settings.Value.SimulatedLatency > TimeSpan.Zero)
            await Task.Delay(settings.Value.SimulatedLatency, cancellationToken);

        var text = $" {Normalize(title)} {Normalize(content)} ";
        var forbidden = ForbiddenPhrases.FirstOrDefault(phrase => text.Contains($" {phrase} ", StringComparison.Ordinal));

        return forbidden is null
            ? ReviewModerationResult.Approve()
            : ReviewModerationResult.Reject($"Forbidden phrase: {forbidden}");
    }

    private static string Normalize(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        var normalized = new StringBuilder(text.Length);
        foreach (var character in text.Normalize(NormalizationForm.FormD))
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) == UnicodeCategory.NonSpacingMark)
                continue;

            if (char.IsLetter(character))
                normalized.Append(char.ToLowerInvariant(character));
            else if (normalized.Length > 0 && normalized[^1] != ' ')
                normalized.Append(' ');
        }

        return normalized.ToString().TrimEnd();
    }
}
