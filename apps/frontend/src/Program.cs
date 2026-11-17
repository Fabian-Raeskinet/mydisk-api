using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using MyDisks.Frontend;
using MyDisks.Frontend.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
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
