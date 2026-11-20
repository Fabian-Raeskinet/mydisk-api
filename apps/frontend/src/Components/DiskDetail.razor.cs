using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MyDisks.Frontend.Models;

namespace MyDisks.Frontend.Components;

public partial class DiskDetail : IAsyncDisposable
{
    [Parameter, EditorRequired] public DiskResult Disk { get; set; } = default!;
    [Parameter] public bool Loading { get; set; }
    [Parameter] public bool Busy { get; set; }
    [Parameter] public string? Error { get; set; }
    [Parameter] public IEnumerable<AuthorResult> Authors { get; set; } = [];
    [Parameter] public EventCallback OnClose { get; set; }
    [Parameter] public EventCallback OnEdit { get; set; }
    [Parameter] public EventCallback OnDelete { get; set; }
    [Parameter] public EventCallback<Guid> OnAttach { get; set; }
    [Parameter] public EventCallback<ReviewForm> OnReview { get; set; }
    [Parameter] public EventCallback<Guid> OnArchive { get; set; }

    private static readonly CultureInfo FrenchCulture = CultureInfo.GetCultureInfo("fr-FR");
    private static readonly DialogOptions DialogOptions = new()
    {
        FullWidth = true,
        MaxWidth = MaxWidth.Medium,
        BackdropClick = false,
        CloseOnEscapeKey = false,
        CloseButton = false
    };

    private MudDialog? _dialog;
    private bool _disposed;
    private bool _confirmDelete;
    private ReviewForm _review = new();
    private readonly AuthorForm _author = new();
    private string? _knownAuthorId;
    private int _reviewCount = -1;

    protected override void OnParametersSet()
    {
        var count = Disk.Reviews?.Count ?? 0;
        if (_reviewCount >= 0 && count > _reviewCount)
        {
            _review = new ReviewForm();
        }

        _reviewCount = count;
    }

    private void ToggleDeleteConfirmation() => _confirmDelete = !_confirmDelete;

    private void ChooseAuthor(string? authorId)
    {
        _knownAuthorId = authorId;
        _author.AuthorId = authorId ?? "";
    }

    private Task AttachAuthorAsync() =>
        Busy ? Task.CompletedTask : OnAttach.InvokeAsync(Guid.Parse(_author.AuthorId));

    private Task SubmitReviewAsync() =>
        Busy ? Task.CompletedTask : OnReview.InvokeAsync(_review);

    private Task RequestCloseAsync() => Busy ? Task.CompletedTask : OnClose.InvokeAsync();

    private Task OnVisibleChangedAsync(bool visible) =>
        !visible && !_disposed ? RequestCloseAsync() : Task.CompletedTask;

    public async ValueTask DisposeAsync()
    {
        _disposed = true;
        if (_dialog is not null)
        {
            await _dialog.CloseAsync();
        }
    }

    private sealed class AuthorForm : IValidatableObject
    {
        public string AuthorId { get; set; } = "";

        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            if (!Guid.TryParse(AuthorId, out var id) || id == Guid.Empty)
            {
                yield return new ValidationResult(
                    "Indiquez un UUID d’artiste valide et non vide.",
                    [nameof(AuthorId)]);
            }
        }
    }

    public sealed class ReviewForm
    {
        [Required(ErrorMessage = "Indiquez un titre.")]
        [StringLength(30, ErrorMessage = "Le titre est limité à 30 caractères.")]
        [RegularExpression(@".*\S.*", ErrorMessage = "Le titre ne peut pas être vide.")]
        public string Title { get; set; } = "";

        [Required(ErrorMessage = "Écrivez quelques mots sur ce disque.")]
        [RegularExpression(@"[\s\S]*\S[\s\S]*", ErrorMessage = "L’avis ne peut pas être vide.")]
        public string Content { get; set; } = "";

        [Range(0, 5, ErrorMessage = "La note doit être comprise entre 0 et 5.")]
        public double Note { get; set; } = 3;
    }
}
