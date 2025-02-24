using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using PacifyAspire.Web;
using PacifyAspire.Web.Components;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddOutputCache();

builder.Services.AddHttpClient<WeatherApiClient>(client =>
    {
        // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
        // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
        client.BaseAddress = new("https+http://apiservice");
    });


builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme).AddMicrosoftIdentityWebApp(
    options =>
    {
        builder.Configuration.Bind("AzureAdB2C", options);
        options.Events = new OpenIdConnectEvents
        {
            OnRedirectToIdentityProvider = async cxt =>
            {
                await Task.Yield();
            },

            OnAuthenticationFailed = async cxt =>
            {
                await Task.Yield();
            },

            OnSignedOutCallbackRedirect = async cxt =>
            {
                cxt.HttpContext.Response.Redirect(cxt.Options.SignedOutRedirectUri);
                cxt.HandleResponse();
                await Task.Yield();
            },

            OnTicketReceived = async cxt =>
            {
                if (cxt.Principal != null)
                {
                    if (cxt.Principal.Identity is ClaimsIdentity identity)
                    {
                        var colClaims = cxt.Principal.Claims.ToList();
                        var IdentityProvider = colClaims.FirstOrDefault(
                            c => c.Type == "http://schemas.microsoft.com/identity/claims/identityprovider")?.Value;
                        var Objectidentifier = colClaims.FirstOrDefault(
                            c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                        var EmailAddress = colClaims.FirstOrDefault(
                            c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
                        var FirstName = colClaims.FirstOrDefault(
                            c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname")?.Value;
                        var LastName = colClaims.FirstOrDefault(
                            c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname")?.Value;
                        var AzureB2CFlow = colClaims.FirstOrDefault(
                            c => c.Type == "http://schemas.microsoft.com/claims/authnclassreference")?.Value;
                        var auth_time = colClaims.FirstOrDefault(
                            c => c.Type == "auth_time")?.Value;
                        var DisplayName = colClaims.FirstOrDefault(
                            c => c.Type == "name")?.Value;
                        var idp_access_token = colClaims.FirstOrDefault(
                            c => c.Type == "idp_access_token")?.Value;
                    }
                }
                await Task.Yield();
            },
        };
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseOutputCache();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
