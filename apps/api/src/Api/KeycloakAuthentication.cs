using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace MyDisks.Api;

internal static class KeycloakAuthentication
{
    internal static void AddKeycloakAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var authentication = configuration.GetSection("Authentication");
        var authority = authentication["Authority"]?.TrimEnd('/');

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.Authority = authority;
            options.Audience = authentication["Audience"] ?? "mydisk-api";
            options.RequireHttpsMetadata = true;
            options.MapInboundClaims = false;
            options.TokenValidationParameters.IssuerValidator = (issuer, _, _) =>
                authority is not null && string.Equals(issuer, authority, StringComparison.Ordinal)
                    ? issuer
                    : throw new SecurityTokenInvalidIssuerException("The token issuer does not match the configured authority.");
        });
        services.AddAuthorization(options => options.AddPolicy("Admin", policy => policy
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .RequireClaim("sub")
            .RequireClaim("mydisk_roles", "admin")));
    }

}
