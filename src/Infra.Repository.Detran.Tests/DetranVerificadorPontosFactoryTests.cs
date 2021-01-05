using System;
using DesignPatternSamples.Application.Repository;
using DesignPatternSamples.Infra.Repository.Detran;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DesignPatternsSamples.Infra.Repository.Detran.Tests
{
    public class DetranVerificadorPontosFactoryTests : IClassFixture<DependencyInjectionFixture>
    {
        private readonly IDetranVerificadorPontosFactory _factory;

        public DetranVerificadorPontosFactoryTests(DependencyInjectionFixture dependencyInjectionFixture)
        {
            var serviceProvider = dependencyInjectionFixture.ServiceProvider;
            _factory = serviceProvider.GetService<IDetranVerificadorPontosFactory>();
        }

        [Theory(DisplayName = "Dado uma UF que está devidamente registrada no Factory, devemos receber a sua implementação correspondente")]
        [InlineData("SP", typeof(DetranSPVerificadorPontosRepository))]
        public void InstanciarServicoPorUFRegistrada(string UF, Type implementacao)
        {
            var resultado = _factory.Create(UF);
            
            Assert.NotNull((resultado));
            Assert.IsType(implementacao, resultado);
        }

        [Fact(DisplayName = "Dado uma UF que não está reigstrada no Factory, devemos receber NULL")]
        public void InstanciarServicoPorUFNaoRegistrada()
        {
            IDetranVerificadorPontosRepository implementacao = _factory.Create("UF");
            
            Assert.Null(implementacao);
        }

    }
}