using System.Security.Claims;
using MyDisks.Api.Exceptions;
using MyDisks.Services;

namespace MyDisks.Api.Controllers;

internal static class DiskOwnerClaims
{
    public static DiskOwner GetRequiredDiskOwner(this ClaimsPrincipal user)
    {
        var issuer = user.FindFirst("iss")?.Value;
        var subject = user.FindFirst("sub")?.Value;
        return string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(subject)
            ? throw new ForbiddenAccessException()
            : new DiskOwner(issuer, subject);
    }
}
