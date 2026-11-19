using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using MyDisks.Frontend;
using MyDisks.Frontend.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var authentication = new AuthenticationSettings(
    builder.Configuration["Authentication:Authority"],
    builder.Configuration["Authentication:ClientId"]);
builder.Services.AddSingleton(authentication);

if (authentication.IsConfigured)
{
    builder.Services.AddCascadingAuthenticationState();
    builder.Services.AddOidcAuthentication(options =>
    {
        options.ProviderOptions.Authority = authentication.Authority;
        options.ProviderOptions.ClientId = authentication.ClientId;
        options.ProviderOptions.ResponseType = "code";
        options.ProviderOptions.DefaultScopes.Add("email");
        options.UserOptions.NameClaim = "preferred_username";
        options.UserOptions.RoleClaim = "mydisk_roles";
        options.AuthenticationPaths.LogOutSucceededPath = RemoteAuthenticationDefaults.LoginPath;
    }).AddAccountClaimsPrincipalFactory<RoleClaimsPrincipalFactory>();
    builder.Services.AddScoped<DiskApiClient>();
}

builder.Services.AddMudServices();
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
var apiAddress = builder.Configuration["ApiBaseUrl"];
builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri(string.IsNullOrWhiteSpace(apiAddress) ? builder.HostEnvironment.BaseAddress : apiAddress),
    Timeout = TimeSpan.FromSeconds(60)
});
await builder.Build().RunAsync();
