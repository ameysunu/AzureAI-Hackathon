var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.PacifyAspire_ApiService>("apiservice");

builder.AddProject<Projects.PacifyAspire_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
