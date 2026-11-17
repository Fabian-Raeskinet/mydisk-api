using Microsoft.AspNetCore.Components;
using MyDisks.Frontend.Models;
using MyDisks.Frontend.Services;

namespace MyDisks.Frontend.Components;

/// <summary>Coordinates API operations; presentation and form validation live in the views.</summary>
public partial class CollectionWorkspace
{
    [Inject] private DiskApiClient Api { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Parameter] public bool ShowReviews { get; set; }

    private readonly CancellationTokenSource _lifetime = new();
    private List<DiskResult> _disks = [];
    private bool _loading = true;
    private bool _busy;
    private bool _loadFailed;
    private bool _error;
    private bool _detailLoading;
    private bool _editorOpen;
    private string? _notice;
    private string? _dialogError;
    private DiskResult? _selected;
    private DiskResult? _editing;
    private long _selectionVersion;

    private IEnumerable<AuthorResult> KnownAuthors => _disks
        .Where(disk => disk.Author is not null)
        .Select(disk => disk.Author!)
        .DistinctBy(author => author.Id);

    protected override Task OnInitializedAsync() => RefreshAsync();

    private async Task RefreshAsync()
    {
        _loading = true;
        try
        {
            _disks = await Api.GetDisksAsync(_lifetime.Token);
            _loadFailed = false;
        }
        catch (Exception error) when (ShouldDisplay(error))
        {
            _loadFailed = true;
            ShowError(error);
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task SelectDiskAsync(DiskResult disk)
    {
        _dialogError = null;
        _selected = disk;
        _detailLoading = true;
        var version = ++_selectionVersion;

        try
        {
            var detail = await Api.GetDiskByNameAsync(disk.Name, _lifetime.Token);
            if (version != _selectionVersion)
                return;

            // The API's only detail lookup is by name, which is not unique.
            if (detail.Id != disk.Id)
            {
                _selected = null;
                Notice("Plusieurs disques portent ce nom. L’API ne peut pas identifier ce disque précisément.", true);
                return;
            }

            _selected = detail;
        }
        catch (Exception error) when (ShouldDisplay(error))
        {
            if (version == _selectionVersion)
            {
                _selected = null;
                ShowError(error);
            }
        }
        finally
        {
            if (version == _selectionVersion)
                _detailLoading = false;
        }
    }

    private void OpenCreate()
    {
        _dialogError = null;
        _editing = null;
        _editorOpen = true;
    }

    private void OpenEdit()
    {
        _dialogError = null;
        _editing = _selected;
        _editorOpen = true;
    }

    private void CloseEditor()
    {
        if (!_busy)
            _editorOpen = false;
    }

    private void CloseDetail()
    {
        if (_busy)
            return;

        _selectionVersion++;
        _selected = null;
    }

    private void BrowseCollection() => Navigation.NavigateTo("/");
    private void ClearNotice() => _notice = null;

    private Task SaveDiskAsync(DiskEditor.DiskForm form)
    {
        var editing = _editing;
        return MutateAsync(async () =>
        {
            if (editing is null)
                await Api.CreateDiskAsync(form.Name.Trim(), form.ReleaseDate!.Value, _lifetime.Token);
            else
                await Api.UpdateDiskAsync(editing.Id, form.Name.Trim(), form.ReleaseDate!.Value, _lifetime.Token);

            _editorOpen = false;
            // Preserve the updated lookup key before fetching details again.
            if (_selected is not null && editing?.Id == _selected.Id)
                _selected.Name = form.Name.Trim();
        }, editing is null ? "Le disque a été ajouté." : "Le disque a été mis à jour.");
    }

    private Task DeleteAsync() => MutateAsync(async () =>
    {
        await Api.DeleteDiskAsync(_selected!.Id, _lifetime.Token);
        _selected = null;
    }, "Le disque a été supprimé.");

    private Task AttachAsync(Guid authorId) => MutateAsync(
        () => Api.AttachAuthorAsync(_selected!.Id, authorId, _lifetime.Token),
        "L’artiste a été associé au disque.");

    private Task CreateReviewAsync(DiskDetail.ReviewForm form) => MutateAsync(
        () => Api.CreateReviewAsync(_selected!.Id, form.Title.Trim(), form.Content.Trim(), form.Note, _lifetime.Token),
        "Votre avis a été ajouté.");

    private Task ArchiveAsync(Guid reviewId) => MutateAsync(
        () => Api.ArchiveReviewAsync(reviewId, _lifetime.Token),
        "L’avis a été archivé.");

    private async Task MutateAsync(Func<Task> action, string success)
    {
        if (_busy)
            return;

        _busy = true;
        _dialogError = null;

        try
        {
            await action();
            Notice(success);
            // A refresh failure must not report a committed write as failed.
            await RefreshAsync();
            if (_selected is not null && !_loadFailed)
                await SelectDiskAsync(_disks.FirstOrDefault(disk => disk.Id == _selected.Id) ?? _selected);
        }
        catch (Exception error) when (ShouldDisplay(error))
        {
            ShowError(error);
        }
        finally
        {
            _busy = false;
        }
    }

    private bool ShouldDisplay(Exception error) =>
        error is not OperationCanceledException || !_lifetime.IsCancellationRequested;

    private void Notice(string message, bool error = false)
    {
        _notice = message;
        _error = error;
    }

    private void ShowError(Exception error)
    {
        var message = error switch
        {
            ApiException => error.Message,
            OperationCanceledException => "L’API a mis trop de temps à répondre. Réessayez.",
            _ => "Impossible de contacter l’API. Vérifiez sa disponibilité et la configuration de son adresse."
        };

        Notice(message, true);
        if (_editorOpen || _selected is not null)
            _dialogError = message;
    }

    public void Dispose()
    {
        _lifetime.Cancel();
        _lifetime.Dispose();
    }
}
