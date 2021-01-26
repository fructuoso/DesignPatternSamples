using DesignPatternSamples.Application.Repository;
using DesignPatternSamples.Infra.Repository.Detran;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace DesignPatternsSamples.Infra.Repository.Detran.Tests
{
    public class DetranPontosCarteiraFactoryTests : IClassFixture<DependencyInjectionFixture>
    {
        private readonly IDetranPontosCarteiraFactory _Factory;

        public DetranPontosCarteiraFactoryTests(DependencyInjectionFixture dependencyInjectionFixture)
        {
            var serviceProvider = dependencyInjectionFixture.ServiceProvider;
            _Factory = serviceProvider.GetService<IDetranPontosCarteiraFactory>();
        }
    }
}
