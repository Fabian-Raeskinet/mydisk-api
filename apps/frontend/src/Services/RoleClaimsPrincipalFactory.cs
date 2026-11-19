using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;

namespace MyDisks.Frontend.Services;

internal sealed class RoleClaimsPrincipalFactory(IAccessTokenProviderAccessor tokenProvider)
    : AccountClaimsPrincipalFactory<RemoteUserAccount>(tokenProvider)
{
    public override async ValueTask<ClaimsPrincipal> CreateUserAsync(
        RemoteUserAccount account, RemoteAuthenticationUserOptions options)
    {
        var user = await base.CreateUserAsync(account, options);
        if (user.Identity is not ClaimsIdentity { IsAuthenticated: true } identity ||
            options.RoleClaim is not { } roleClaim ||
            !account.AdditionalProperties.TryGetValue(roleClaim, out var value) ||
            value is not JsonElement { ValueKind: JsonValueKind.Array } roles)
        {
            return user;
        }

        // The OIDC account stores role arrays as a single property. IsInRole uses individual claims.
        foreach (var claim in identity.FindAll(roleClaim).ToArray())
            identity.RemoveClaim(claim);

        foreach (var role in roles.EnumerateArray())
        {
            if (role.ValueKind == JsonValueKind.String && role.GetString() is { Length: > 0 } name)
                identity.AddClaim(new Claim(roleClaim, name));
        }

        return user;
    }
}
