using Carter;

using WellnessPlan.WebApi.Dependency;

var builder = WebApplication.CreateBuilder(args);
builder.CoreBuilder();

var app = builder.Build();
app.MapServices();

await app.RunAsync();
