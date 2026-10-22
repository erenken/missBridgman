var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.MissBridgman_Web>("web")
    .WithHttpHealthCheck("/healthz")
    .WithEnvironment("Admin__Password", builder.Configuration["Admin:Password"] ?? string.Empty)
    .WithEnvironment("AzureWebJobsStorage", builder.Configuration["AzureWebJobsStorage"] ?? string.Empty);

builder.Build().Run();
