using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;

namespace MoodLift.Auth
{
    /// <summary>
    /// Provides extension methods for configuring authentication-related endpoints, including Google sign-in,
    /// authentication success handling, and logout functionality.
    /// </summary>
    public static class AuthEndpoint
    {
        /// <summary>
        /// Stores the URL to redirect to after successful authentication.
        /// </summary>
        public static string ReturnUrl = string.Empty;

        /// <summary>
        /// Maps and configures the authentication endpoints for the MoodLift application.
        /// </summary>
        /// <param name="builder">The endpoint route builder to configure authentication routes.</param>
        /// <returns>An <see cref="IEndpointConventionBuilder"/> that can be used to further customize the authentication endpoints.</returns>
        /// <remarks>
        /// This method configures three main authentication endpoints:
        /// 1. POST /authentication/google-signin
        ///    Initiates Google OAuth authentication flow
        ///    Captures return URL for post-authentication redirect
        ///    Configures authentication properties and challenge
        /// 2. GET /authentication/success
        ///    Handles OAuth callback from Google
        ///    Validates authentication state
        ///    Extracts user information (email, name, ID, picture)
        ///    Creates application-specific claims
        ///    Establishes user session
        ///    Redirects to original requested URL
        /// 3. POST /authentication/logout
        ///    Terminates user session
        ///    Redirects to login page
        /// </remarks>
        public static IEndpointConventionBuilder MapAuthEndpoints(this IEndpointRouteBuilder builder)
        {
            var accountGroup = builder.MapGroup("/authentication");

            // Google Sign-in endpoint handler
            accountGroup.MapPost("google-signin",
                /// <summary>
                /// Handles the initiation of Google OAuth authentication.
                /// </summary>
                /// <param name="context">The HTTP context for the request.</param>
                /// <param name="returnUrl">The URL to redirect to after successful authentication.</param>
                /// <returns>A challenge result that redirects to Google's authentication page.</returns>
                async (HttpContext context, [FromForm] string returnUrl) =>
                {
                    ReturnUrl = returnUrl;
                    var authProp = new AuthenticationProperties
                    {
                        RedirectUri = "authentication/success"
                    };
                    var result = TypedResults.Challenge(authProp, [GoogleDefaults.AuthenticationScheme]);
                    await result.ExecuteAsync(context);
                });

            // Authentication success callback handler
            accountGroup.MapGet("success",
                /// <summary>
                /// Processes the OAuth callback from Google and establishes the user session.
                /// </summary>
                /// <param name="context">The HTTP context containing the authentication result.</param>
                /// <returns>A redirect result to the originally requested page or home page.</returns>
                async (HttpContext context) =>
                {
                    if (!context.User.Identity!.IsAuthenticated)
                        return Results.Unauthorized();

                    // Extract user claims from Google authentication
                    var email = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)!.Value;
                    var name = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)!.Value;
                    var googleId = context.User.Claims.FirstOrDefault(x => x.Type == "sub")?.Value
                        ?? context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                    var picture = context.User.Claims.FirstOrDefault(x => x.Type == "picture")?.Value;

                    // Create application-specific claims
                    Claim[] claims = [
                        new(ClaimTypes.Name, name),
                        new(ClaimTypes.Email, email),
                        new(ClaimTypes.NameIdentifier, googleId!),
                        new("picture", picture ?? "")
                    ];

                    // Establish user session
                    var identity = new ClaimsIdentity(claims, Constant.Scheme);
                    var principal = new ClaimsPrincipal(identity);
                    await context.SignInAsync(principal);

                    // Redirect to original URL or home
                    string returnUrl = string.IsNullOrEmpty(ReturnUrl) ? "/" : ReturnUrl;
                    return Results.LocalRedirect($"~{returnUrl}");
                });

            // Logout endpoint handler
            accountGroup.MapPost("/logout",
                /// <summary>
                /// Handles user logout by terminating the current session.
                /// </summary>
                /// <param name="context">The HTTP context for the logout request.</param>
                /// <returns>A redirect result to the login page.</returns>
                (HttpContext context) =>
                {
                    context.SignOutAsync(Constant.Scheme);
                    return Results.LocalRedirect("~/login");
                });

            return accountGroup;
        }
    }
}
