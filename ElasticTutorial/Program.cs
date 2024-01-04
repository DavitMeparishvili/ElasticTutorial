using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using System.Reflection;
using System;
using ElasticTutorial;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var configuration = builder.Configuration;
var uriString = configuration["ElasticSearch:Uri"];

builder.Services.AddHttpContextAccessor();

Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .Enrich.WithMachineName()
            .WriteTo.Console()
            .WriteTo.Debug()
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(uriString))
            {
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name!.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
            })
            .ReadFrom.Configuration(configuration)
            .Enrich.With(new TenantIdEnricher(builder.Services.BuildServiceProvider().GetService<IHttpContextAccessor>()))
            .CreateLogger();

builder.Services.AddSingleton(Log.Logger);

builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "KibanaPractice", Version = "v1" });

    // Register the SwaggerHeaderParameterFilter
    c.OperationFilter<SwaggerHeaderParameterFilter>(new List<SwaggerHeaderParameter>
        {
            new SwaggerHeaderParameter
            {
                Name = "TenantId",
                Description = "Set tenant header here",
                Required = true
            }
            // Add more header parameters as needed
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
