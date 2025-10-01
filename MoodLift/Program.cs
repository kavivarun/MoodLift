using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using MoodLift.Auth;
using MoodLift.Components;
using MoodLift.Core.Interfaces;
using MoodLift.Infrastructure.Repositories;
using MoodLift.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// DI registrations
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, HttpContextCurrentUserService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IMoodEntryService, MoodEntryService>();

// Add services to the container.
builder.Services.AddDbContext<MoodLiftDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("MoodLiftDb") ?? "Data Source=moodlift.db",
        b => b.MigrationsAssembly("MoodLift.Infrastructure")
    ));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();
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
    });
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();
app.MapAuthEndpoints();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
