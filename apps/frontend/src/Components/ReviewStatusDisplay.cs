using MudBlazor;
using MyDisks.Frontend.Models;

namespace MyDisks.Frontend.Components;

public static class ReviewStatusDisplay
{
    public static string Label(ReviewStatus status) => status switch
    {
        ReviewStatus.New => "Nouveau",
        ReviewStatus.Pending => "En cours de modération",
        ReviewStatus.Published => "Publié",
        ReviewStatus.Rejected => "Refusé",
        ReviewStatus.Archived => "Archivé",
        _ => "Statut inconnu"
    };

    public static Color Color(ReviewStatus status) => status switch
    {
        ReviewStatus.Published => MudBlazor.Color.Success,
        ReviewStatus.Pending => MudBlazor.Color.Warning,
        ReviewStatus.Rejected => MudBlazor.Color.Error,
        ReviewStatus.Archived => MudBlazor.Color.Default,
        _ => MudBlazor.Color.Info
    };
}
