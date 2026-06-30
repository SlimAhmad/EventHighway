using EventHighway.Portal.Web.Components;
using EventHighway.Portal.Web.Components.Account;
using EventHighway.Portal.Web.Data;
using EventHighway.Portal.Web.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddPortalIdentity(builder.Configuration);
builder.Services.AddPortalBrokers(builder.Configuration);
builder.Services.AddPortalViewServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

// A seed failure (e.g. a transient database error on a cold start) is logged but must not stop
// the app from serving; the seed is idempotent and re-runs on the next start.
try
{
    await SeedData.SeedAsync(app.Services);
}
catch (Exception seedException)
{
    app.Logger.LogError(seedException, "Identity seed failed at startup; continuing.");
}

app.Run();
