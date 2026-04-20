using GoodHamburger.Web;
using GoodHamburger.Web.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5000")
});

builder.Services.AddScoped<OrderApiService>();
builder.Services.AddScoped<MenuApiService>();

await builder.Build().RunAsync();
