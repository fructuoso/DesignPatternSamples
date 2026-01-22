using DesignPatternSamples.Application.Decorators;
using DesignPatternSamples.Application.DTO;
using DesignPatternSamples.Application.Services;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Xunit;

namespace DesignPatternSamples.Application.Tests.Decorators;

public class DetranVerificadorDebitosDecoratorCacheTests
{
    private readonly Mock<IDetranVerificadorDebitosService> _innerServiceMock;
    private readonly Mock<IDistributedCache> _cacheMock;
    private readonly DetranVerificadorDebitosDecoratorCache _decorator;

    public DetranVerificadorDebitosDecoratorCacheTests()
    {
        _innerServiceMock = new Mock<IDetranVerificadorDebitosService>();
        _cacheMock = new Mock<IDistributedCache>();
        _decorator = new DetranVerificadorDebitosDecoratorCache(_innerServiceMock.Object, _cacheMock.Object);
    }

    [Fact(DisplayName = "ConsultarDebitos - Deve consultar serviço interno quando cache não existe")]
    public async Task ConsultarDebitos_DeveConsultarServicoInterno_QuandoCacheNaoExiste()
    {
        // Arrange
        var veiculo = new Veiculo { Placa = "ABC1234", UF = "SP" };
        var debitos = new List<DebitoVeiculo>
        {
            new() { DataOcorrencia = DateTime.Now, Descricao = "Teste", Valor = 100.00 }
        };

        _cacheMock
            .Setup(c => c.GetAsync($"SP_ABC1234", It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        _innerServiceMock
            .Setup(s => s.ConsultarDebitos(veiculo))
            .ReturnsAsync(debitos);

        // Act
        var resultado = await _decorator.ConsultarDebitos(veiculo);

        // Assert
        Assert.Equal(debitos, resultado);
        _innerServiceMock.Verify(s => s.ConsultarDebitos(veiculo), Times.Once);
        _cacheMock.Verify(c => c.SetAsync(
            It.IsAny<string>(), 
            It.IsAny<byte[]>(), 
            It.IsAny<DistributedCacheEntryOptions>(), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "ConsultarDebitos - Deve gerar chave de cache correta")]
    public async Task ConsultarDebitos_DeveGerarChaveDeCacheCorreta()
    {
        // Arrange
        var veiculo = new Veiculo { Placa = "XYZ9876", UF = "RJ" };
        var debitos = new List<DebitoVeiculo>();

        _cacheMock
            .Setup(c => c.GetAsync("RJ_XYZ9876", It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        _innerServiceMock
            .Setup(s => s.ConsultarDebitos(veiculo))
            .ReturnsAsync(debitos);

        // Act
        await _decorator.ConsultarDebitos(veiculo);

        // Assert
        _cacheMock.Verify(c => c.GetAsync("RJ_XYZ9876", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory(DisplayName = "ConsultarDebitos - Deve usar cache para diferentes veículos")]
    [InlineData("SP", "ABC1234")]
    [InlineData("RJ", "XYZ5678")]
    [InlineData("PE", "DEF9012")]
    [InlineData("RS", "GHI3456")]
    public async Task ConsultarDebitos_DeveUsarCacheParaDiferentesVeiculos(string uf, string placa)
    {
        // Arrange
        var veiculo = new Veiculo { Placa = placa, UF = uf };
        var debitos = new List<DebitoVeiculo>
        {
            new() { DataOcorrencia = DateTime.Now, Descricao = $"Débito {uf}", Valor = 100.00 }
        };

        _cacheMock
            .Setup(c => c.GetAsync($"{uf}_{placa}", It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        _innerServiceMock
            .Setup(s => s.ConsultarDebitos(veiculo))
            .ReturnsAsync(debitos);

        // Act
        await _decorator.ConsultarDebitos(veiculo);

        // Assert
        _cacheMock.Verify(c => c.GetAsync($"{uf}_{placa}", It.IsAny<CancellationToken>()), Times.Once);
        _innerServiceMock.Verify(s => s.ConsultarDebitos(veiculo), Times.Once);
    }
}
