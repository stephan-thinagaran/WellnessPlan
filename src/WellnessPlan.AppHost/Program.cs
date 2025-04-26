var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.WellnessPlan_WebApi>("api")
       .WithExternalHttpEndpoints(); 

await builder.Build().RunAsync();
