using ApexCharts;
using Azure.AI.OpenAI;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using MoodLift.Auth;
using MoodLift.Components;
using MoodLift.Core.Interfaces;
using MoodLift.Infrastructure.Repositories;
using MoodLift.Infrastructure.Services;
using System.ClientModel;

var builder = WebApplication.CreateBuilder(args);

// Register core application services
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, HttpContextCurrentUserService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IMoodEntryService, MoodEntryService>();
builder.Services.AddScoped<ISavedPlaylistService, SavedPlaylistService>();

// Configure SQLite database
builder.Services.AddDbContext<MoodLiftDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("MoodLiftDb") ?? "Data Source=moodlift.db",
        b => b.MigrationsAssembly("MoodLift.Infrastructure")
    ));

// Configure Blazor and authentication
builder.Services.AddApexCharts();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();

// Set up authentication with custom cookie scheme
var googleClientId = builder.Configuration["Google:ClientId"];
var googleClientSecret = builder.Configuration["Google:ClientSecret"];
var hasGoogleAuth = !string.IsNullOrWhiteSpace(googleClientId) && !string.IsNullOrWhiteSpace(googleClientSecret);

var authBuilder = builder.Services.AddAuthentication(Constant.Scheme)
    .AddCookie(Constant.Scheme, options =>
    {
        options.Cookie.Name = Constant.Scheme;
        options.ExpireTimeSpan = TimeSpan.FromHours(12);
        options.LoginPath = "/login";
    });

if (hasGoogleAuth)
{
    authBuilder.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
    {
        options.ClientId = googleClientId!;
        options.ClientSecret = googleClientSecret!;
        options.SignInScheme = Constant.Scheme;
        options.Scope.Add("profile");
        options.ClaimActions.MapJsonKey("picture", "picture", "url");
    });
}
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();

// Configure Azure OpenAI client for chat features when credentials are available.
var azureEndpoint = builder.Configuration["AzureOpenAI:Endpoint"];
var azureApiKey = builder.Configuration["AzureOpenAI:ApiKey"];
var hasAzureOpenAi = !string.IsNullOrWhiteSpace(azureEndpoint) && !string.IsNullOrWhiteSpace(azureApiKey);

if (hasAzureOpenAi)
{
    builder.Services.AddSingleton<IChatClient>(sp =>
    {
        var deployment = builder.Configuration["AzureOpenAI:Deployment"] ?? "gpt-4.1";
        return new AzureOpenAIClient(new Uri(azureEndpoint!), new ApiKeyCredential(azureApiKey!))
            .GetChatClient(deployment)
            .AsIChatClient();
    });
}

// Initialize Spotify service
builder.Services.AddSingleton<SpotifyService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MoodLiftDbContext>();
    db.Database.Migrate();
}

// Configure production environment settings
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// Configure middleware pipeline
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();
app.MapAuthEndpoints();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
