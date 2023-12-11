using AircraftTracker;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder();

builder.ConfigureServices(Bootstrap.ConfigureStatupServices);

var app = builder.Build();

await app.RunAsync();
