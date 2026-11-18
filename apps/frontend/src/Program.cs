using Microsoft.AspNetCore.Components.Web;
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
        options.UserOptions.NameClaim = "preferred_username";
    });
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
builder.Services.AddScoped<DiskApiClient>();
await builder.Build().RunAsync();
