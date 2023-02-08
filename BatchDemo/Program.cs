using BatchDemo.DataAccess.Repository;
using BatchDemo.DataAccess.Repository.IRepository;
using BatchDemo.DataAccess;
using BatchDemo.Extensions;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using BatchDemo.Services;
using BatchDemo.Services.Interface;
using BatchDemo.Exceptions;

var builder = WebApplication.CreateBuilder(args);

try
{
    var logger = new LoggerConfiguration()
   .ReadFrom.Configuration(builder.Configuration)
   .Enrich.FromLogContext()
   .CreateLogger();

    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog(logger);
}
finally
{
    Log.CloseAndFlush();
}

builder.Services.AddControllers();
// Register custom defined Correlation ID Generator
//builder.Services.AddCorrelationIdGenerator();
// Disable automatic validate response
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IBatchUtility, BatchUtility>();

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
// Register for exception handling. Order is important to call
app.ConfigureBuiltInExceptionHandler();
app.MapControllers();
// Add Correlation ID Middleware
// app.AddCorrelationIdMiddleware();
app.Run();

[ExcludeFromCodeCoverage]
public partial class Program { }