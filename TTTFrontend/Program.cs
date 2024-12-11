using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TTTFrontend.Services;
using TTTFrontend;
using Blazored.LocalStorage;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddSingleton(new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    Converters = { new JsonStringEnumConverter() }
});

builder.Services.AddHttpClient<SceneService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7041");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
}).AddHttpMessageHandler<AuthorizationMessageHandler>();

builder.Services.AddHttpClient<UserSoundsService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7041");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
}).AddHttpMessageHandler<AuthorizationMessageHandler>();

builder.Services.AddScoped<AuthorizationMessageHandler>();
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
    DefaultRequestHeaders = { { "Accept", "application/json" } }
});

builder.Services.AddScoped(sp =>
    new AuthorizationMessageHandler(
        sp.GetRequiredService<ILocalStorageService>(),
        sp.GetRequiredService<NavigationManager>(),
        sp.GetRequiredService<CustomAuthenticationStateProvider>()
    ));

builder.Services.AddScoped<SelectedSceneService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<SelectedUserSoundsService>();
builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();
