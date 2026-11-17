using Microsoft.AspNetCore.Components;
using MudBlazor;
using MyDisks.Frontend.Models;
using MyDisks.Frontend.Services;

namespace MyDisks.Frontend.Components;

public partial class Diagnostics : IDisposable
{
    [Inject] private DiskApiClient Api { get; set; } = default!;

    private readonly CancellationTokenSource _lifetime = new();
    private readonly ServiceCheck[] _checks =
    [
        new("Processus API", "/health/live", Icons.Material.Filled.MonitorHeart),
        new("Base de données", "/health/ready", Icons.Material.Filled.Storage),
        new("Santé globale", "/health", Icons.Material.Filled.HealthAndSafety),
        new("Maintenance", "/maintenance", Icons.Material.Filled.Build)
    ];

    private bool _checking;
    private bool _retrying;
    private bool _retrySuccess;
    private string? _retryResult;

    protected override Task OnInitializedAsync() => CheckAsync();

    private async Task CheckAsync()
    {
        if (_checking) return;
        _checking = true;
        try
        {
            await Task.WhenAll(_checks.Select(CheckServiceAsync));
        }
        finally
        {
            _checking = false;
        }
    }

    private async Task CheckServiceAsync(ServiceCheck check, int index)
    {
        try
        {
            if (index == 3)
            {
                var maintenance = await Api.GetMaintenanceAsync(_lifetime.Token);
                check.Success = maintenance.Status == MaintenanceStatus.Inactive;
                check.Message = check.Success.Value
                    ? "L’API accepte les actions."
                    : "Le mode maintenance est activé.";
                return;
            }

            var probe = index switch { 0 => "live", 1 => "ready", _ => "" };
            check.Message = await Api.GetHealthAsync(probe, _lifetime.Token);
            check.Success = true;
        }
        catch (OperationCanceledException) when (_lifetime.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            check.Success = false;
            check.Message = ex is ApiException ? ex.Message : "Impossible de joindre cette sonde.";
        }
    }

    private async Task RetryAsync()
    {
        if (_retrying) return;
        _retrying = true;
        _retryResult = null;
        try
        {
            await Api.RunRetryAsync(_lifetime.Token);
            _retrySuccess = true;
            _retryResult = "L’action du service de retry s’est terminée avec succès.";
        }
        catch (OperationCanceledException) when (_lifetime.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            _retrySuccess = false;
            _retryResult = ex is ApiException
                ? ex.Message
                : "L’action a échoué ou dépassé le délai de réponse.";
        }
        finally
        {
            _retrying = false;
        }
    }

    private string StatusLabel(ServiceCheck check) => _checking
        ? "Vérification…"
        : check.Success switch { true => "Disponible", false => "Indisponible", _ => "Non vérifié" };

    private Color StatusColor(ServiceCheck check) => _checking
        ? Color.Default
        : check.Success switch { true => Color.Success, false => Color.Error, _ => Color.Default };

    private sealed class ServiceCheck(string title, string path, string icon)
    {
        public string Title { get; } = title;
        public string Path { get; } = path;
        public string Icon { get; } = icon;
        public string Message { get; set; } = "En attente de vérification.";
        public bool? Success { get; set; }
    }

    public void Dispose()
    {
        _lifetime.Cancel();
        _lifetime.Dispose();
    }
}
