using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using DemoBlazor;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

// registration HttpClient 
builder.Services.AddScoped(sp => new HttpClient());
builder.Services.AddScoped<SolanaAddressValidator>();
builder.Services.AddScoped<SolanaBalanceService>();

await builder.Build().RunAsync();
