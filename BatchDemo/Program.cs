using BatchDemo.DataAccess;
using BatchDemo.DataAccess.Repository;
using BatchDemo.DataAccess.Repository.IRepository;
using BatchDemo.Logger;
using BatchDemo.Services;
using BatchDemo.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Azure.Identity;
using Microsoft.Extensions.Azure;
using Azure.Security.KeyVault.Secrets;
using Azure.Extensions.AspNetCore.Configuration.Secrets;

var builder = WebApplication.CreateBuilder(args);

//var keyVaultEndpoint = new Uri(Environment.GetEnvironmentVariable("VaultUri"));
//builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());
//try
//{
var logger = new LoggerConfiguration()
.ReadFrom.Configuration(builder.Configuration)
.Enrich.FromLogContext()
.CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
//}
//finally
//{
//  Log.CloseAndFlush();
//}

builder.Services.AddControllers();
// Register custom defined Correlation ID Generator
//builder.Services.AddCorrelationIdGenerator();
// Disable automatic validate response
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddScoped<IKeyVaultManager, KeyVaultManager>();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}
else
{
    string? kvUrl = builder.Configuration["KeyVaultConfig:KVUrl"];
    string? tenantId = builder.Configuration["KeyVaultConfig:TenantId"];
    string? clientId = builder.Configuration["KeyVaultConfig:ClientId"];
    string? clientSecretId = builder.Configuration["KeyVaultConfig:ClientSecretId"];

    var credential = new ClientSecretCredential(tenantId, clientId, clientSecretId);
    var client = new SecretClient(new Uri(kvUrl!), credential);

    builder.Configuration.AddAzureKeyVault(client, new KeyVaultSecretManager());

    builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    ////var dbConnectionString = new KeyVaultManager(builder.Configuration).GetDbConnectionFromAzureVault();//builder.Configuration.Configuration[Configuration[DBConnectionStringSecretIdentifierKey]];
    ////if (string.IsNullOrEmpty(dbConnectionString))
    ////{
    ////    throw new ApplicationException(message: "Failed to get database connection string");
    ////}
    ////builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(dbConnectionString));
}

//builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

////var dbConnectionString = new KeyVaultManager(builder.Configuration).GetDbConnectionFromAzureVault();//builder.Configuration.Configuration[Configuration[DBConnectionStringSecretIdentifierKey]];
////if (string.IsNullOrEmpty(dbConnectionString))
////{
////    throw new ApplicationException(message: "Failed to get database connection string");
////}
////builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(dbConnectionString));
//builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IBatchUtility, BatchUtility>();
builder.Services.AddScoped<IBatchBlobService, BatchBlobService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Batch Demo API",
        Description = "An ASP.NET Core Web API for creation of batch to upload and get files",
    });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
//builder.Services.AddAzureClients(clientBuilder =>
//{
//    clientBuilder.AddBlobServiceClient(builder.Configuration["AzureSettings:StorageAccount:blob"], preferMsi: true);
//    clientBuilder.AddQueueServiceClient(builder.Configuration["AzureSettings:StorageAccount:queue"], preferMsi: true);
//});

var app = builder.Build();
app.UseSwagger();
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
// Register for exception handling. Order is important to call
//app.ConfigureExceptionHandler(logger);
app.UseGlobalExceptionMiddleware(logger);
// Add Correlation ID Middleware
// app.AddCorrelationIdMiddleware();
app.MapControllers();
app.Run();

/// <summary>
/// 
/// </summary>
[ExcludeFromCodeCoverage]
public partial class Program { }