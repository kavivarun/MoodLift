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
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, HttpContextCurrentUserService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IMoodEntryService, MoodEntryService>();

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

// Set up Google authentication with custom cookie scheme
builder.Services.AddAuthentication(Constant.Scheme)
    .AddCookie(Constant.Scheme, options =>
    {
        options.Cookie.Name = Constant.Scheme;
        options.ExpireTimeSpan = TimeSpan.FromHours(12);
        options.LoginPath = "/login";
    })
    .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
    {
        options.ClientId = builder.Configuration["Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Google:ClientSecret"]!;
        options.SignInScheme = Constant.Scheme;
        options.Scope.Add("profile");
        options.ClaimActions.MapJsonKey("picture", "picture", "url");
    });
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();

// Configure Azure OpenAI client for chat features
builder.Services.AddSingleton<IChatClient>(sp =>
{
    var endpoint = builder.Configuration["AzureOpenAI:Endpoint"] ?? throw new InvalidOperationException("Missing AzureOpenAI:Endpoint");
    var deployment = builder.Configuration["AzureOpenAI:Deployment"] ?? "gpt-4.1";
    return new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(builder.Configuration["AzureOpenAI:ApiKey"]!) )
        .GetChatClient(deployment)
        .AsIChatClient();
});

// Initialize Spotify service
builder.Services.AddSingleton<SpotifyService>();

var app = builder.Build();

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
