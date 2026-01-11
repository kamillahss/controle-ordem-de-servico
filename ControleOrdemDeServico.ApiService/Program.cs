using OsService.Infrastructure.Databases;
using OsService.Infrastructure.Repository;
using OsService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add MediatR and Services layer
builder.Services.AddServicesLayer();

// Configure database connection factories
var connectionString = builder.Configuration.GetConnectionString("OsServiceDb")
    ?? throw new InvalidOperationException("Connection string 'OsServiceDb' not found.");

// Master connection for creating database (connects to master database)
var masterConnectionString = connectionString.Replace("Database=OsServiceDb", "Database=master");

builder.Services.AddSingleton<IOsServiceDbConnectionFactory>(_ =>
    new SqlConnectionFactory(connectionString));

builder.Services.AddSingleton<IMasterDbConnectionFactory>(_ =>
    new SqlConnectionFactory(masterConnectionString));

// Register repositories
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IServiceOrderRepository, ServiceOrderRepository>();

// Register database generator
builder.Services.AddSingleton<DatabaseGenerator>();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var databaseGenerator = scope.ServiceProvider.GetRequiredService<DatabaseGenerator>();
    await databaseGenerator.EnsureCreatedAsync(CancellationToken.None);
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

// Map a root endpoint
app.MapGet("/", () => Results.Json(new
{
    name = "OsService API",
    version = "v1",
    status = "Running",
    endpoints = new
    {
        customers = "/api/v1/customers",
        serviceOrders = "/api/v1/serviceorders",
        openapi = "/openapi/v1.json",
        health = "/health"
    }
})).WithName("Root").WithTags("Info");

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // Enable Swagger UI
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "OsService API v1");
    });
}

app.MapDefaultEndpoints();

await app.RunAsync();
