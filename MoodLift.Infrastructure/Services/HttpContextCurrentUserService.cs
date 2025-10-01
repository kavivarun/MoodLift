using Microsoft.AspNetCore.Http;
using MoodLift.Core;
using MoodLift.Core.Interfaces;
using System.Security.Claims;

namespace MoodLift.Infrastructure.Services;

public class HttpContextCurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _http;
    public HttpContextCurrentUserService(IHttpContextAccessor http) => _http = http;

    public string GetGoogleUserId()
    {
        var user = _http.HttpContext?.User;
        if (user is null || !user.Identity?.IsAuthenticated == true)
            throw new InvalidOperationException("User is not authenticated.");

        // Prefer OpenID Connect "sub" claim; fallback to NameIdentifier
        var id = user.FindFirst("sub")?.Value
              ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return id ?? throw new InvalidOperationException("Google user id not found in claims.");
    }
}
