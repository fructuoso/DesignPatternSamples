using AutoMapper;
using DesignPatternSamples.Application.Decorators;
using DesignPatternSamples.Application.Implementations;
using DesignPatternSamples.Application.Repository;
using DesignPatternSamples.Application.Services;
using DesignPatternSamples.Infra.Repository.Detran;
using DesignPatternSamples.WebAPI.Middlewares;
using DesignPatternSamples.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Serilog;
using Workbench.DependencyInjection.Extensions;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build())
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add Serilog
    builder.Host.UseSerilog();

    // Add services to the container
    var services = builder.Services;

    // Health checks
    services.AddHealthChecks();

    // Swagger
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "DesignPatternSamples", Version = "v1" });
    });

    // Add dependency injection and AutoMapper
    services.AddDependencyInjection()
        .AddAutoMapper();

    // Add distributed memory cache (FAKE distributed cache)
    services.AddDistributedMemoryCache();

    // Add controllers
    services.AddControllers(options =>
    {
        options.Filters.Add(new ProducesResponseTypeAttribute(typeof(FailureResultModel), 500));
    });

    // Add API explorer for Swagger
    services.AddEndpointsApiExplorer();

    var app = builder.Build();

    // Configure the HTTP request pipeline
    const string HEALTH_PATH = "/health";

    // Health checks
    app.UseHealthChecks(HEALTH_PATH);

    // Swagger
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
    });

    // Development exception page
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    // HTTPS redirection
    app.UseHttpsRedirection();

    // Routing
    app.UseRouting();

    // Authorization
    app.UseAuthorization();

    // Custom middleware
    app.UseDetranVerificadorDebitosFactory();
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    // Map controllers
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// Extension methods
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
    {
        return services
            .AddTransient<IDetranVerificadorDebitosService, DetranVerificadorDebitosServices>()
            .Decorate<IDetranVerificadorDebitosService, DetranVerificadorDebitosDecoratorCache>()
            .Decorate<IDetranVerificadorDebitosService, DetranVerificadorDebitosDecoratorLogger>()
            .AddSingleton<IDetranVerificadorDebitosFactory, DetranVerificadorDebitosFactory>()
            .AddTransient<DetranPEVerificadorDebitosRepository>()
            .AddTransient<DetranSPVerificadorDebitosRepository>()
            .AddTransient<DetranRJVerificadorDebitosRepository>()
            .AddTransient<DetranRSVerificadorDebitosRepository>()
            .AddScoped<ExceptionHandlingMiddleware>();
    }

    public static IServiceCollection AddAutoMapper(this IServiceCollection services)
    {
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsSubclassOf(typeof(Profile)));

        return services.AddAutoMapper(types.ToArray());
    }
}

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseDetranVerificadorDebitosFactory(this IApplicationBuilder app)
    {
        app.ApplicationServices.GetService<IDetranVerificadorDebitosFactory>()?
            .Register("PE", typeof(DetranPEVerificadorDebitosRepository))
            .Register("RJ", typeof(DetranRJVerificadorDebitosRepository))
            .Register("SP", typeof(DetranSPVerificadorDebitosRepository))
            .Register("RS", typeof(DetranRSVerificadorDebitosRepository));

        return app;
    }
}
