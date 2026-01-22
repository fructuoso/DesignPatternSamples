using DesignPatternSamples.Application.DTO;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DesignPatternSamples.Infra.Repository.Detran.Tests;

public class DetranRJVerificadorDebitosRepositoryTests
{
    private readonly Mock<ILogger<DetranRJVerificadorDebitosRepository>> _loggerMock;
    private readonly DetranRJVerificadorDebitosRepository _repository;

    public DetranRJVerificadorDebitosRepositoryTests()
    {
        _loggerMock = new Mock<ILogger<DetranRJVerificadorDebitosRepository>>();
        _repository = new DetranRJVerificadorDebitosRepository(_loggerMock.Object);
    }

    [Fact(DisplayName = "ConsultarDebitos - Deve retornar débitos de RJ")]
    public async Task ConsultarDebitos_DeveRetornarDebitosDeRJ()
    {
        // Arrange
        var veiculo = new Veiculo { Placa = "ABC-1234", UF = "RJ" };

        // Act
        var result = await _repository.ConsultarDebitos(veiculo);

        // Assert
        Assert.NotNull(result);
        var debitos = result.ToList();
        Assert.Single(debitos);
        
        var debito = debitos.First();
        Assert.Equal("Débito RJ", debito.Descricao);
        Assert.Equal(200.00, debito.Valor);
        Assert.NotEqual(default, debito.DataOcorrencia);
    }

    [Fact(DisplayName = "ConsultarDebitos - Deve registrar log de debug")]
    public async Task ConsultarDebitos_DeveRegistrarLogDeDebug()
    {
        // Arrange
        var veiculo = new Veiculo { Placa = "RIO-123", UF = "RJ" };

        // Act
        await _repository.ConsultarDebitos(veiculo);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("RIO-123")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    [Theory(DisplayName = "ConsultarDebitos - Deve funcionar com diferentes placas")]
    [InlineData("RIO-1234")]
    [InlineData("ERJ-5678")]
    [InlineData("RJO-9999")]
    public async Task ConsultarDebitos_DeveFuncionarComDiferentesPlacas(string placa)
    {
        // Arrange
        var veiculo = new Veiculo { Placa = placa, UF = "RJ" };

        // Act
        var result = await _repository.ConsultarDebitos(veiculo);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(placa)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    [Fact(DisplayName = "ConsultarDebitos - Deve retornar débito com valor específico de RJ")]
    public async Task ConsultarDebitos_DeveRetornarDebitoComValorEspecificoDeRJ()
    {
        // Arrange
        var veiculo = new Veiculo { Placa = "TEST-RJ", UF = "RJ" };

        // Act
        var result = await _repository.ConsultarDebitos(veiculo);

        // Assert
        var debito = result.First();
        Assert.Equal(200.00, debito.Valor);
    }

    [Fact(DisplayName = "ConsultarDebitos - Deve incluir descrição específica de RJ")]
    public async Task ConsultarDebitos_DeveIncluirDescricaoEspecificaDeRJ()
    {
        // Arrange
        var veiculo = new Veiculo { Placa = "RJ-TEST", UF = "RJ" };

        // Act
        var result = await _repository.ConsultarDebitos(veiculo);

        // Assert
        var debito = result.First();
        Assert.Contains("RJ", debito.Descricao);
    }
}
