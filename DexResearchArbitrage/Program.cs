using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using DexResearchArbitrage;
using DexResearchArbitrage.Services;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register MudBlazor services
builder.Services.AddMudServices();

// Register HttpClient
builder.Services.AddScoped(sp => new HttpClient());

// Register application services
builder.Services.AddScoped<ITokenValidationService, TokenValidationService>();
builder.Services.AddScoped<IPoolsService, PoolsService>();

await builder.Build().RunAsync();
