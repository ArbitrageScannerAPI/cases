using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using DemoBlazor;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

// ИСПРАВЛЕННАЯ регистрация HttpClient - БЕЗ BaseAddress для внешних API
builder.Services.AddScoped(sp => new HttpClient());
builder.Services.AddScoped<SolanaAddressValidator>();

await builder.Build().RunAsync();
