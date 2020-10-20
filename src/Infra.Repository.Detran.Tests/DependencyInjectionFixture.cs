using DesignPatternSamples.Application.Repository;
using DesignPatternSamples.Infra.Repository.Detran;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DesignPatternsSamples.Infra.Repository.Detran.Tests
{
    public class DependencyInjectionFixture
    {
        public readonly IServiceProvider ServiceProvider;

        public DependencyInjectionFixture()
        {
            var services = new ServiceCollection()
                .AddLogging()
                .AddTransient<DetranPEVerificadorDebitosRepository>()
                .AddTransient<DetranSPVerificadorDebitosRepository>()
                .AddTransient<DetranRJVerificadorDebitosRepository>()
                .AddTransient<DetranRSVerificadorDebitosRepository>()
                .AddSingleton<IDetranVerificadorDebitosFactory, DetranVerificadorDebitosFactory>();

            #region IConfiguration
            services.AddTransient<IConfiguration>((services) =>
                new ConfigurationBuilder()

                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", false, true)
                .Build()
            );
            #endregion

            ServiceProvider = services.BuildServiceProvider();

            ServiceProvider.GetService<IDetranVerificadorDebitosFactory>()
                .Register("PE", typeof(DetranPEVerificadorDebitosRepository))
                .Register("RJ", typeof(DetranRJVerificadorDebitosRepository))
                .Register("SP", typeof(DetranSPVerificadorDebitosRepository))
                .Register("RS", typeof(DetranRSVerificadorDebitosRepository));
        }
    }
}