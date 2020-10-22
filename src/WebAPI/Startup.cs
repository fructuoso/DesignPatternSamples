using AutoMapper;
using DesignPatternSamples.Application.Decorators;
using DesignPatternSamples.Application.Implementations;
using DesignPatternSamples.Application.Repository;
using DesignPatternSamples.Application.Services;
using DesignPatternSamples.Infra.Repository.Detran;
using DesignPatternSamples.WebAPI.Middlewares;
using DesignPatternSamples.WebAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Linq;
using Workbench.DependencyInjection.Extensions;

namespace DesignPatternSamples.WebAPI
{
    public class Startup
    {
        protected const string HEALTH_PATH = "/health";

        protected readonly IConfiguration _Configuration;

        public Startup(IConfiguration configuration)
        {
            _Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region HealthCheck
            services.AddHealthChecks();
            #endregion

            #region Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DesignPatternSamples", Version = "v1" });
            });
            #endregion

            services.AddDependencyInjection()
                .AddAutoMapper();

            /*Cache distribuído FAKE*/
            services.AddDistributedMemoryCache();
            
            services.AddControllers();

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
                options.Filters.Add(new ProducesResponseTypeAttribute(typeof(FailureResultModel), 500));
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            #region HealthCheck
            app.UseHealthChecks(HEALTH_PATH);
            #endregion

            #region Swagger
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
            });
            #endregion

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseDetranVerificadorDebitosFactory();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseMvc();
        }
    }

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
            app.ApplicationServices.GetService<IDetranVerificadorDebitosFactory>()
                .Register("PE", typeof(DetranPEVerificadorDebitosRepository))
                .Register("RJ", typeof(DetranRJVerificadorDebitosRepository))
                .Register("SP", typeof(DetranSPVerificadorDebitosRepository))
                .Register("RS", typeof(DetranRSVerificadorDebitosRepository));

            return app;
        }
    }
}
