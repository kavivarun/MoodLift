using Microsoft.AspNetCore.Http;
using MoodLift.Core;
using MoodLift.Core.Interfaces;
using System.Security.Claims;

namespace MoodLift.Infrastructure.Services;

/// <summary>
/// Implementation of ICurrentUserService that retrieves user information from the HTTP context.
/// Used to access the currently authenticated user's Google ID in a web application context.
/// </summary>
public class HttpContextCurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _http;

    /// <summary>
    /// Initializes a new instance of the HttpContextCurrentUserService class.
    /// </summary>
    /// <param name="http">The HTTP context accessor to access the current HTTP context.</param>
    public HttpContextCurrentUserService(IHttpContextAccessor http) => _http = http;

    /// <summary>
    /// Retrieves the Google user ID of the currently authenticated user from their claims.
    /// </summary>
    /// <returns>The Google user ID as a string.</returns>
    public string GetGoogleUserId()
    {
        var user = _http.HttpContext?.User;
        if (user is null || !user.Identity?.IsAuthenticated == true)
            throw new InvalidOperationException("User is not authenticated.");

        var id = user.FindFirst("sub")?.Value
              ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return id ?? throw new InvalidOperationException("Google user id not found in claims.");
    }
}
