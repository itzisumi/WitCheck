using APICaller.API;
using APICaller.Hub;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebGUI;
using WebGUI.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var baseUri = new Uri(builder.HostEnvironment.BaseAddress);
var apiUrl = $"{baseUri.Scheme}://{baseUri.Host}:5203";

builder.Services.AddSingleton<LobbyCaller>(sp => new LobbyCaller(apiUrl));
builder.Services.AddSingleton<UserCaller>(sp => new UserCaller(apiUrl));
builder.Services.AddSingleton<QuizCaller>(sp => new QuizCaller(apiUrl));
builder.Services.AddSingleton<PlayerHubCaller>(sp => new PlayerHubCaller(apiUrl));
builder.Services.AddSingleton<HostHubCaller>(sp => new HostHubCaller(apiUrl));
builder.Services.AddSingleton<QuestionTimerService>();
builder.Services.AddSingleton<PlayerStateService>();
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();
