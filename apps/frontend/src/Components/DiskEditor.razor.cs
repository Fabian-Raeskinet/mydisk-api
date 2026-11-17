using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MyDisks.Frontend.Models;

namespace MyDisks.Frontend.Components;

public partial class DiskEditor : IAsyncDisposable
{
    [Parameter] public DiskResult? Disk { get; set; }
    [Parameter] public bool Busy { get; set; }
    [Parameter] public string? Error { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }
    [Parameter] public EventCallback<DiskForm> OnSave { get; set; }

    private static readonly CultureInfo FrenchCulture = CultureInfo.GetCultureInfo("fr-FR");
    private static readonly DialogOptions DialogOptions = new()
    {
        FullWidth = true,
        MaxWidth = MaxWidth.Small,
        BackdropClick = false,
        CloseOnEscapeKey = false,
        CloseButton = false
    };

    private MudDialog? _dialog;
    private DiskForm _form = new();
    private bool _disposed;

    protected override void OnInitialized()
    {
        _form = new DiskForm
        {
            Name = Disk?.Name ?? "",
            ReleaseDate = Disk?.ReleaseDate ?? DateTime.Today
        };
    }

    private Task SubmitAsync() => Busy ? Task.CompletedTask : OnSave.InvokeAsync(_form);

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

    public sealed class DiskForm
    {
        [Required(ErrorMessage = "Indiquez un titre.")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "Le titre doit contenir entre 1 et 30 caractères.")]
        [RegularExpression(@".*\S.*", ErrorMessage = "Le titre ne peut pas être vide.")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Choisissez une date de sortie.")]
        public DateTime? ReleaseDate { get; set; }
    }
}
