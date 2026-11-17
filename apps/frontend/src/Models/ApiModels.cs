using System.Text.Json.Serialization;

namespace MyDisks.Frontend.Models;

public sealed class DiskResult
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public DateTime? ReleaseDate { get; set; }
    public string? ImageUrl { get; set; }
    public AuthorResult? Author { get; set; }
    public List<ReviewResult>? Reviews { get; set; }
}

public sealed class AuthorResult
{
    public Guid Id { get; set; }
    public string Pseudonym { get; set; } = "";
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReviewStatus { New = 1, Pending = 2, Published = 3, Archived = 4 }

public sealed class ReviewResult
{
    public Guid Id { get; set; }
    public DateTimeOffset PublishedDate { get; set; }
    public ReviewStatus Status { get; set; }
    public string? Content { get; set; }
    public string? Title { get; set; }
    public double Note { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MaintenanceStatus { Active = 1, Inactive = 2 }

public sealed class MaintenanceResult
{
    public MaintenanceStatus Status { get; set; }
}
