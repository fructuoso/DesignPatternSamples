using DesignPatternSamples.Application.Repository;
using DesignPatternSamples.Infra.Repository.Detran;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DesignPatternsSamples.Infra.Repository.Detran.Tests;

public class DetranVerificadorDebitosFactoryTests : IClassFixture<DependencyInjectionFixture>
{
    private readonly IDetranVerificadorDebitosFactory _factory;

    public DetranVerificadorDebitosFactoryTests(DependencyInjectionFixture dependencyInjectionFixture)
    {
        var serviceProvider = dependencyInjectionFixture.ServiceProvider;
        _factory = serviceProvider.GetService<IDetranVerificadorDebitosFactory>()!;
    }

    [Theory(DisplayName = "Dado um UF que está devidamente registrado no Factory devemos receber a sua implementação correspondente")]
    [InlineData("PE", typeof(DetranPEVerificadorDebitosRepository))]
    [InlineData("SP", typeof(DetranSPVerificadorDebitosRepository))]
    [InlineData("RJ", typeof(DetranRJVerificadorDebitosRepository))]
    [InlineData("RS", typeof(DetranRSVerificadorDebitosRepository))]
    public void InstanciarServicoPorUFRegistrado(string uf, Type implementacao)
    {
        var resultado = _factory.Create(uf);

        Assert.NotNull(resultado);
        Assert.IsType(implementacao, resultado);
    }

    [Fact(DisplayName = "Dado um UF que não está registrado no Factory devemos receber NULL")]
    public void InstanciarServicoPorUFNaoRegistrado()
    {
        var implementacao = _factory.Create("CE");

        Assert.Null(implementacao);
    }
}
