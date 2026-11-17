using Microsoft.AspNetCore.Components;
using MyDisks.Frontend.Models;

namespace MyDisks.Frontend.Components;

public partial class PublishedReviewsView
{
    [Parameter, EditorRequired] public IReadOnlyList<DiskResult> Disks { get; set; } = [];
    [Parameter] public bool Loading { get; set; }
    [Parameter] public EventCallback<DiskResult> OnSelect { get; set; }
    [Parameter] public EventCallback OnBrowse { get; set; }

    private IEnumerable<(DiskResult Disk, ReviewResult Review)> PublishedReviews => Disks
        .SelectMany(disk => (disk.Reviews ?? [])
            .Where(review => review.Status == ReviewStatus.Published)
            .Select(review => (disk, review)));
}
