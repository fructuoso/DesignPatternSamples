using DesignPatternSamples.Application.DTO;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DesignPatternSamples.Infra.Repository.Detran.Tests;

public class DetranSPVerificadorDebitosRepositoryTests
{
    private readonly Mock<ILogger<DetranSPVerificadorDebitosRepository>> _loggerMock;
    private readonly DetranSPVerificadorDebitosRepository _repository;

    public DetranSPVerificadorDebitosRepositoryTests()
    {
        _loggerMock = new Mock<ILogger<DetranSPVerificadorDebitosRepository>>();
        _repository = new DetranSPVerificadorDebitosRepository(_loggerMock.Object);
    }

    [Fact(DisplayName = "ConsultarDebitos - Deve retornar débitos de SP")]
    public async Task ConsultarDebitos_DeveRetornarDebitosDeSP()
    {
        // Arrange
        var veiculo = new Veiculo { Placa = "ABC-1234", UF = "SP" };

        // Act
        var result = await _repository.ConsultarDebitos(veiculo);

        // Assert
        Assert.NotNull(result);
        var debitos = result.ToList();
        Assert.Single(debitos);

        var debito = debitos.First();
        Assert.Equal("Débito exemplo", debito.Descricao);
        Assert.Equal(100.00, debito.Valor);
        Assert.NotEqual(default, debito.DataOcorrencia);
    }

    [Fact(DisplayName = "ConsultarDebitos - Deve registrar log de debug")]
    public async Task ConsultarDebitos_DeveRegistrarLogDeDebug()
    {
        // Arrange
        var veiculo = new Veiculo { Placa = "SAO-123", UF = "SP" };

        // Act
        await _repository.ConsultarDebitos(veiculo);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("SAO-123") && v.ToString()!.Contains("SP")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory(DisplayName = "ConsultarDebitos - Deve funcionar com diferentes placas")]
    [InlineData("ABC-1234")]
    [InlineData("XYZ-5678")]
    [InlineData("DEF-9999")]
    public async Task ConsultarDebitos_DeveFuncionarComDiferentesPlacas(string placa)
    {
        // Arrange
        var veiculo = new Veiculo { Placa = placa, UF = "SP" };

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
            Times.Once);
    }

    [Fact(DisplayName = "ConsultarDebitos - Deve retornar débito com valor específico de SP")]
    public async Task ConsultarDebitos_DeveRetornarDebitoComValorEspecificoDeSP()
    {
        // Arrange
        var veiculo = new Veiculo { Placa = "TEST-SP", UF = "SP" };

        // Act
        var result = await _repository.ConsultarDebitos(veiculo);

        // Assert
        var debito = result.First();
        Assert.Equal(100.00, debito.Valor);
    }

    [Fact(DisplayName = "ConsultarDebitos - Deve ser mais rápido que outros repositórios")]
    public async Task ConsultarDebitos_DeveSerMaisRapidoQueOutrosRepositorios()
    {
        // Arrange
        var veiculo = new Veiculo { Placa = "FAST-SP", UF = "SP" };
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        await _repository.ConsultarDebitos(veiculo);
        stopwatch.Stop();

        // Assert - SP não tem delay, deve ser rápido (menos de 100ms)
        Assert.True(stopwatch.ElapsedMilliseconds < 100);
    }

    [Fact(DisplayName = "ConsultarDebitos - Deve incluir data de ocorrência")]
    public async Task ConsultarDebitos_DeveIncluirDataDeOcorrencia()
    {
        // Arrange
        var veiculo = new Veiculo { Placa = "SP-DATE", UF = "SP" };
        var dataAntes = DateTime.Now.AddMinutes(-1);

        // Act
        var result = await _repository.ConsultarDebitos(veiculo);
        var dataDepois = DateTime.Now.AddMinutes(1);

        // Assert
        var debito = result.First();
        Assert.InRange(debito.DataOcorrencia, dataAntes, dataDepois);
    }
}
