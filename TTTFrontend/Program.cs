using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TTTFrontend;
using TTTFrontend.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { 
    BaseAddress = new Uri( /*replace this with the address your api will be hosted*/ "https://localhost:7041")
});
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddBlazoredLocalStorage();


await builder.Build().RunAsync();
