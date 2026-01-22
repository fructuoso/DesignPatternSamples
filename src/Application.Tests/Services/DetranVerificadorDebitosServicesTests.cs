using DesignPatternSamples.Application.DTO;
using DesignPatternSamples.Application.Implementations;
using DesignPatternSamples.Application.Repository;
using Moq;
using Xunit;

namespace DesignPatternSamples.Application.Tests.Services;

public class DetranVerificadorDebitosServicesTests
{
    private readonly Mock<IDetranVerificadorDebitosFactory> _factoryMock;
    private readonly Mock<IDetranVerificadorDebitosRepository> _repositoryMock;
    private readonly DetranVerificadorDebitosServices _service;

    public DetranVerificadorDebitosServicesTests()
    {
        _factoryMock = new Mock<IDetranVerificadorDebitosFactory>();
        _repositoryMock = new Mock<IDetranVerificadorDebitosRepository>();
        _service = new DetranVerificadorDebitosServices(_factoryMock.Object);
    }

    [Fact(DisplayName = "ConsultarDebitos - Deve retornar débitos quando repositório for encontrado")]
    public async Task ConsultarDebitos_DeveRetornarDebitos_QuandoRepositorioEncontrado()
    {
        // Arrange
        var veiculo = new Veiculo { Placa = "ABC1234", UF = "SP" };
        var debitosEsperados = new List<DebitoVeiculo>
        {
            new() { DataOcorrencia = DateTime.Now, Descricao = "IPVA 2024", Valor = 1500.00 },
            new() { DataOcorrencia = DateTime.Now, Descricao = "Multa", Valor = 195.23 }
        };

        _factoryMock
            .Setup(f => f.Create("SP"))
            .Returns(_repositoryMock.Object);

        _repositoryMock
            .Setup(r => r.ConsultarDebitos(veiculo))
            .ReturnsAsync(debitosEsperados);

        // Act
        var resultado = await _service.ConsultarDebitos(veiculo);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count());
        Assert.Contains(resultado, d => d.Descricao == "IPVA 2024");
        Assert.Contains(resultado, d => d.Descricao == "Multa");

        _factoryMock.Verify(f => f.Create("SP"), Times.Once);
        _repositoryMock.Verify(r => r.ConsultarDebitos(veiculo), Times.Once);
    }

    [Fact(DisplayName = "ConsultarDebitos - Deve lançar exceção quando repositório não for encontrado")]
    public async Task ConsultarDebitos_DeveLancarExcecao_QuandoRepositorioNaoEncontrado()
    {
        // Arrange
        var veiculo = new Veiculo { Placa = "XYZ9876", UF = "CE" };

        _factoryMock
            .Setup(f => f.Create("CE"))
            .Returns((IDetranVerificadorDebitosRepository?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.ConsultarDebitos(veiculo)
        );

        Assert.Contains("Nenhum repositório encontrado para UF: CE", exception.Message);
        _factoryMock.Verify(f => f.Create("CE"), Times.Once);
    }

    [Theory(DisplayName = "ConsultarDebitos - Deve consultar diferentes UFs corretamente")]
    [InlineData("SP")]
    [InlineData("RJ")]
    [InlineData("PE")]
    [InlineData("RS")]
    public async Task ConsultarDebitos_DeveConsultarDiferentesUFs(string uf)
    {
        // Arrange
        var veiculo = new Veiculo { Placa = "ABC1234", UF = uf };
        var debitos = new List<DebitoVeiculo>
        {
            new() { DataOcorrencia = DateTime.Now, Descricao = $"Débito {uf}", Valor = 100.00 }
        };

        _factoryMock.Setup(f => f.Create(uf)).Returns(_repositoryMock.Object);
        _repositoryMock.Setup(r => r.ConsultarDebitos(veiculo)).ReturnsAsync(debitos);

        // Act
        var resultado = await _service.ConsultarDebitos(veiculo);

        // Assert
        Assert.NotNull(resultado);
        Assert.Single(resultado);
        _factoryMock.Verify(f => f.Create(uf), Times.Once);
    }

    [Fact(DisplayName = "ConsultarDebitos - Deve retornar lista vazia quando não houver débitos")]
    public async Task ConsultarDebitos_DeveRetornarListaVazia_QuandoNaoHouverDebitos()
    {
        // Arrange
        var veiculo = new Veiculo { Placa = "ABC1234", UF = "SP" };
        var debitosVazios = new List<DebitoVeiculo>();

        _factoryMock.Setup(f => f.Create("SP")).Returns(_repositoryMock.Object);
        _repositoryMock.Setup(r => r.ConsultarDebitos(veiculo)).ReturnsAsync(debitosVazios);

        // Act
        var resultado = await _service.ConsultarDebitos(veiculo);

        // Assert
        Assert.NotNull(resultado);
        Assert.Empty(resultado);
    }
}
