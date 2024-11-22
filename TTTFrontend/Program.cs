using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TTTFrontend.Services;
using TTTFrontend;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthorizationMessageHandler>();
builder.Services.AddHttpClient<SceneService>(client =>
{
	client.BaseAddress = new Uri("https://localhost:7041");
}).AddHttpMessageHandler<AuthorizationMessageHandler>();

// Register other services
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();