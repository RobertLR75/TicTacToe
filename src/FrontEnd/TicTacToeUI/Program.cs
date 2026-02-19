using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TicTacToeUI;
using TicTacToeUI.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient<GameApiClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7082");
});

builder.Services.AddScoped<GameService>();

await builder.Build().RunAsync();
