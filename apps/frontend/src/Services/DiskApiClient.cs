using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using MyDisks.Frontend.Models;

namespace MyDisks.Frontend.Services;

public sealed class ApiException(HttpStatusCode statusCode, string message) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}

public sealed class DiskApiClient(HttpClient http, IAccessTokenProvider accessTokenProvider)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<List<DiskResult>> GetDisksAsync(CancellationToken ct = default) =>
        await GetAsync<List<DiskResult>>("api/Disk/disks", ct);

    public Task<DiskResult> GetDiskByNameAsync(string name, CancellationToken ct = default) =>
        GetAsync<DiskResult>($"api/Disk/disk-by-name?name={Uri.EscapeDataString(name)}", ct);

    public Task CreateDiskAsync(string name, DateTime releaseDate, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Post, "api/Disk/create-disk", new { name, releaseDate = CalendarDate(releaseDate) }, ct);

    public Task UpdateDiskAsync(Guid id, string name, DateTime releaseDate, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Put, "api/Disk/update-disk", new { id, name, releaseDate = CalendarDate(releaseDate) }, ct);

    public Task DeleteDiskAsync(Guid id, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Delete, $"api/Disk/delete-disk?diskId={id}", null, ct);

    public Task AttachAuthorAsync(Guid diskId, Guid authorId, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Post, "api/Disk/attach-author", new { diskId, authorId }, ct);

    public Task CreateReviewAsync(Guid diskId, string title, string content, double note, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Post, "api/Review", new { diskId, title, content, note }, ct);

    public Task ArchiveReviewAsync(Guid reviewId, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Post, "api/Review/Archive", new { reviewId }, ct);

    public Task<MaintenanceResult> GetMaintenanceAsync(CancellationToken ct = default) =>
        GetAsync<MaintenanceResult>("maintenance", ct);

    public Task<List<ClaimResult>> GetSessionClaimsAsync(CancellationToken ct = default) =>
        GetAsync<List<ClaimResult>>("api/auth/me", ct);

    public async Task<string> GetHealthAsync(string probe = "ready", CancellationToken ct = default)
    {
        if (probe is not ("live" or "ready" or ""))
            throw new ArgumentException("Sonde inconnue.", nameof(probe));
        using var response = await http.GetAsync(probe.Length == 0 ? "health" : $"health/{probe}", ct);
        await EnsureSuccessAsync(response, ct);
        return await response.Content.ReadAsStringAsync(ct);
    }

    public Task RunRetryAsync(CancellationToken ct = default) =>
        SendAsync(HttpMethod.Get, "api/Miscellaneous/disks", null, ct);

    // A release date is a calendar date, not a time-zone-dependent instant.
    private static DateTime CalendarDate(DateTime date) => DateTime.SpecifyKind(date.Date, DateTimeKind.Unspecified);

    private async Task<T> GetAsync<T>(string path, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, path);
        using var response = await SendRequestAsync(request, ct);
        return await ReadResponseAsync<T>(response, ct);
    }

    private static async Task<T> ReadResponseAsync<T>(HttpResponseMessage response, CancellationToken ct)
    {
        await EnsureSuccessAsync(response, ct);
        try
        {
            return await response.Content.ReadFromJsonAsync<T>(JsonOptions, ct)
                ?? throw new ApiException(response.StatusCode, "L’API a renvoyé une réponse vide.");
        }
        catch (JsonException)
        {
            throw new ApiException(response.StatusCode, "Réponse API illisible. Vérifiez l’adresse et le proxy de l’API.");
        }
    }

    private async Task SendAsync(HttpMethod method, string path, object? body, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(method, path);
        if (body is not null) request.Content = JsonContent.Create(body, options: JsonOptions);
        using var response = await SendRequestAsync(request, ct);
        await EnsureSuccessAsync(response, ct);
    }

    private async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request, CancellationToken ct)
    {
        if (request.RequestUri is { IsAbsoluteUri: false } uri &&
            uri.OriginalString.StartsWith("api/", StringComparison.Ordinal))
        {
            ct.ThrowIfCancellationRequested();
            var result = await accessTokenProvider.RequestAccessToken();
            ct.ThrowIfCancellationRequested();
            if (!result.TryGetToken(out var token))
            {
                throw new ApiException(HttpStatusCode.Unauthorized,
                    "Aucun jeton d’accès disponible. Reconnectez-vous puis réessayez.");
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
        }

        return await http.SendAsync(request, ct);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode) return;
        var text = await response.Content.ReadAsStringAsync(ct);
        var message = response.StatusCode switch
        {
            HttpStatusCode.ServiceUnavailable => "L’API est en maintenance ou temporairement indisponible.",
            HttpStatusCode.NotFound => "L’élément demandé est introuvable.",
            HttpStatusCode.Unauthorized => "L’API a refusé la session (401). Reconnectez-vous puis réessayez.",
            HttpStatusCode.Forbidden => "Cette action n’est pas autorisée.",
            _ => $"L’API a renvoyé une erreur ({(int)response.StatusCode})."
        };
        try
        {
            using var document = JsonDocument.Parse(text);
            var root = document.RootElement;
            if (root.ValueKind == JsonValueKind.Array)
                message = string.Join(" · ", root.EnumerateArray().Select(x => x.ToString()));
            else if (root.ValueKind == JsonValueKind.Object)
            {
                if (root.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Object)
                    message = string.Join(" · ", errors.EnumerateObject().SelectMany(x =>
                        x.Value.ValueKind == JsonValueKind.Array ? x.Value.EnumerateArray().Select(v => v.ToString()) : [x.Value.ToString()]));
                else if (root.TryGetProperty("detail", out var detail) && detail.ValueKind == JsonValueKind.String)
                    message = detail.GetString()!;
                else if (root.TryGetProperty("title", out var title) && title.ValueKind == JsonValueKind.String)
                    message = title.GetString()!;
            }
        }
        catch (JsonException)
        {
            // Do not surface server stack traces or HTML error pages to the interface.
        }
        throw new ApiException(response.StatusCode, message);
    }
}
