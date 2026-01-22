using DesignPatternSamples.Application.DTO;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DesignPatternSamples.Infra.Repository.Detran.Tests;

public class DetranPEVerificadorDebitosRepositoryTests
{
    private readonly Mock<ILogger<DetranPEVerificadorDebitosRepository>> _loggerMock;
    private readonly DetranPEVerificadorDebitosRepository _repository;

    public DetranPEVerificadorDebitosRepositoryTests()
    {
        _loggerMock = new Mock<ILogger<DetranPEVerificadorDebitosRepository>>();
        _repository = new DetranPEVerificadorDebitosRepository(_loggerMock.Object);
    }

    [Fact(DisplayName = "ConsultarDebitos - Deve retornar débitos de PE")]
    public async Task ConsultarDebitos_DeveRetornarDebitosDePE()
    {
        // Arrange
        var veiculo = new Veiculo { Placa = "ABC-1234", UF = "PE" };

        // Act
        var result = await _repository.ConsultarDebitos(veiculo);

        // Assert
        Assert.NotNull(result);
        var debitos = result.ToList();
        Assert.Single(debitos);
        
        var debito = debitos.First();
        Assert.Equal("Débito PE", debito.Descricao);
        Assert.Equal(150.00, debito.Valor);
        Assert.NotEqual(default, debito.DataOcorrencia);
    }

    [Fact(DisplayName = "ConsultarDebitos - Deve registrar log de debug")]
    public async Task ConsultarDebitos_DeveRegistrarLogDeDebug()
    {
        // Arrange
        var veiculo = new Veiculo { Placa = "XYZ-9876", UF = "PE" };

        // Act
        await _repository.ConsultarDebitos(veiculo);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("XYZ-9876")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    [Theory(DisplayName = "ConsultarDebitos - Deve funcionar com diferentes placas")]
    [InlineData("ABC-1234")]
    [InlineData("XYZ-9999")]
    [InlineData("DEF-5678")]
    public async Task ConsultarDebitos_DeveFuncionarComDiferentesPlacas(string placa)
    {
        // Arrange
        var veiculo = new Veiculo { Placa = placa, UF = "PE" };

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

    [Fact(DisplayName = "ConsultarDebitos - Deve retornar débito com valor positivo")]
    public async Task ConsultarDebitos_DeveRetornarDebitoComValorPositivo()
    {
        // Arrange
        var veiculo = new Veiculo { Placa = "TEST-001", UF = "PE" };

        // Act
        var result = await _repository.ConsultarDebitos(veiculo);

        // Assert
        var debito = result.First();
        Assert.True(debito.Valor > 0);
    }

    [Fact(DisplayName = "ConsultarDebitos - Deve incluir descrição específica de PE")]
    public async Task ConsultarDebitos_DeveIncluirDescricaoEspecificaDePE()
    {
        // Arrange
        var veiculo = new Veiculo { Placa = "PE-TEST", UF = "PE" };

        // Act
        var result = await _repository.ConsultarDebitos(veiculo);

        // Assert
        var debito = result.First();
        Assert.Contains("PE", debito.Descricao);
    }
}
