var builder = DistributedApplication.CreateBuilder(args);

// Use SQL Server connection string (LocalDB or any SQL Server instance)
var sqlConnectionString = builder.AddConnectionString("OsServiceDb");

var apiService = builder.AddProject<Projects.OsService_ApiService>("apiservice")
    .WithReference(sqlConnectionString)
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.OsService_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
