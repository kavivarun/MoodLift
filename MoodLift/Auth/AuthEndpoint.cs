using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Google;

namespace MoodLift.Auth
{
    public static class AuthEndpoint
    {
        public static string ReturnUrl = string.Empty;
        public static IEndpointConventionBuilder MapAuthEndpoints(this IEndpointRouteBuilder builder)
        {
            var accountGroup = builder.MapGroup("/authentication");
            accountGroup.MapPost("google-signin",
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
            accountGroup.MapGet("success",
                async (HttpContext context) =>
                {
                    if (!context.User.Identity!.IsAuthenticated)
                        return Results.Unauthorized();

                    var email = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)!.Value;
                    var name = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)!.Value;
                    var googleId = context.User.Claims.FirstOrDefault(x => x.Type == "sub")?.Value
               ?? context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                    var picture = context.User.Claims.FirstOrDefault(x => x.Type == "picture")?.Value;
                    Claim[] claims = [
                        new(ClaimTypes.Name, name),
                        new(ClaimTypes.Email, email),
                        new(ClaimTypes.NameIdentifier, googleId!),
                        new("picture", picture ?? "")
                    ];

                    var identity = new ClaimsIdentity(claims, Constant.Scheme);
                    var principal = new ClaimsPrincipal(identity);
                    await context.SignInAsync(principal);
                    string returnUrl = string.IsNullOrEmpty(ReturnUrl) ? "/" : ReturnUrl;
                    return Results.LocalRedirect($"~{returnUrl}");
                });
            accountGroup.MapPost("/logout", (HttpContext context) =>
            {
                context.SignOutAsync(Constant.Scheme);
                return Results.LocalRedirect("~/login");
            });

            return accountGroup;
        }
    }
}
