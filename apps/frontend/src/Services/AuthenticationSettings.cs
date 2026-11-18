namespace MyDisks.Frontend.Services;

public sealed record AuthenticationSettings(string? Authority, string? ClientId)
{
    public bool IsConfigured => !string.IsNullOrWhiteSpace(Authority) && !string.IsNullOrWhiteSpace(ClientId);
}
