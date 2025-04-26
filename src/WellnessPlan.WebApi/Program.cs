using Carter;

var assembly = typeof(Program).Assembly;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));
builder.Services.AddCarter();
builder.AddServiceDefaults();

var app = builder.Build();

// Mapp Services
app.MapCarter();

await app.RunAsync();
