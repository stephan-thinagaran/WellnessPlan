using Carter;

using WellnessPlan.WebApi.Dependency;


var builder = WebApplication.CreateBuilder(args);

// Add logging defaults
builder.AddLoggingDefaults();

var configuration = builder.Configuration;
builder.CoreBuilder(configuration);


var app = builder.Build();
app.MapServices(configuration);

await app.RunAsync();
