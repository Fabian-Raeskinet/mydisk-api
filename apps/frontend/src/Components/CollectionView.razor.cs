using System.Globalization;
using Microsoft.AspNetCore.Components;
using MyDisks.Frontend.Models;

namespace MyDisks.Frontend.Components;

public partial class CollectionView
{
    [Parameter, EditorRequired] public IReadOnlyList<DiskResult> Disks { get; set; } = [];
    [Parameter] public bool Loading { get; set; }
    [Parameter] public bool LoadFailed { get; set; }
    [Parameter] public bool Busy { get; set; }
    [Parameter] public EventCallback OnCreate { get; set; }
    [Parameter] public EventCallback OnRefresh { get; set; }
    [Parameter] public EventCallback<DiskResult> OnSelect { get; set; }

    private string? _search;
    private string _sort = "name";

    private int ArtistCount => Disks
        .Where(disk => disk.Author is not null)
        .Select(disk => disk.Author!.Id)
        .Distinct()
        .Count();

    private int PublishedReviewCount => Disks.Sum(CountReviews);

    private IEnumerable<DiskResult> FilteredDisks
    {
        get
        {
            var search = _search?.Trim() ?? "";
            var disks = Disks.Where(disk =>
                disk.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                || (disk.Author?.Pseudonym.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false));

            return _sort switch
            {
                "newest" => disks.OrderByDescending(disk => disk.ReleaseDate),
                "oldest" => disks.OrderBy(disk => disk.ReleaseDate),
                _ => disks.OrderBy(disk => disk.Name, StringComparer.CurrentCultureIgnoreCase)
            };
        }
    }

    private static int CountReviews(DiskResult disk) =>
        disk.Reviews?.Count(review => review.Status == ReviewStatus.Published) ?? 0;

    private static string FormatAverage(DiskResult disk)
    {
        var reviews = disk.Reviews?.Where(review => review.Status == ReviewStatus.Published).ToArray() ?? [];
        return reviews.Length == 0
            ? "Aucune note"
            : $"{reviews.Average(review => review.Note).ToString("0.0", CultureInfo.GetCultureInfo("fr-FR"))} / 5";
    }

    private void ClearSearch() => _search = null;
}
