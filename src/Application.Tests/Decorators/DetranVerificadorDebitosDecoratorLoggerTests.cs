using DesignPatternSamples.Application.Decorators;
using DesignPatternSamples.Application.DTO;
using DesignPatternSamples.Application.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DesignPatternSamples.Application.Tests.Decorators;

public class DetranVerificadorDebitosDecoratorLoggerTests
{
    private readonly Mock<IDetranVerificadorDebitosService> _innerServiceMock;
    private readonly Mock<ILogger<DetranVerificadorDebitosDecoratorLogger>> _loggerMock;
    private readonly DetranVerificadorDebitosDecoratorLogger _decorator;

    public DetranVerificadorDebitosDecoratorLoggerTests()
    {
        _innerServiceMock = new Mock<IDetranVerificadorDebitosService>();
        _loggerMock = new Mock<ILogger<DetranVerificadorDebitosDecoratorLogger>>();
        _decorator = new DetranVerificadorDebitosDecoratorLogger(_innerServiceMock.Object, _loggerMock.Object);
    }

    [Fact(DisplayName = "ConsultarDebitos - Deve logar início e fim da execução")]
    public async Task ConsultarDebitos_DeveLogarInicioEFim()
    {
        // Arrange
        var veiculo = new Veiculo { Placa = "ABC1234", UF = "SP" };
        var debitos = new List<DebitoVeiculo>
        {
            new() { DataOcorrencia = DateTime.Now, Descricao = "Teste", Valor = 100.00 }
        };

        _innerServiceMock
            .Setup(s => s.ConsultarDebitos(veiculo))
            .ReturnsAsync(debitos);

        // Act
        var resultado = await _decorator.ConsultarDebitos(veiculo);

        // Assert
        Assert.Equal(debitos, resultado);

        // Verificar log de início
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Iniciando a execução do método ConsultarDebitos")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        // Verificar log de fim
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Encerrando a execução do método ConsultarDebitos")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _innerServiceMock.Verify(s => s.ConsultarDebitos(veiculo), Times.Once);
    }

    [Fact(DisplayName = "ConsultarDebitos - Deve propagar exceções e ainda logar")]
    public async Task ConsultarDebitos_DevePropagarExcecoes()
    {
        // Arrange
        var veiculo = new Veiculo { Placa = "ABC1234", UF = "SP" };
        var exception = new InvalidOperationException("Erro de teste");

        _innerServiceMock
            .Setup(s => s.ConsultarDebitos(veiculo))
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _decorator.ConsultarDebitos(veiculo));

        // Verificar que o log de início foi chamado
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Iniciando a execução")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact(DisplayName = "ConsultarDebitos - Deve retornar resultado do serviço interno")]
    public async Task ConsultarDebitos_DeveRetornarResultadoDoServicoInterno()
    {
        // Arrange
        var veiculo = new Veiculo { Placa = "XYZ9876", UF = "RJ" };
        var debitosEsperados = new List<DebitoVeiculo>
        {
            new() { DataOcorrencia = DateTime.Now.AddDays(-30), Descricao = "IPVA", Valor = 2000.00 },
            new() { DataOcorrencia = DateTime.Now.AddDays(-15), Descricao = "Licenciamento", Valor = 150.00 }
        };

        _innerServiceMock
            .Setup(s => s.ConsultarDebitos(veiculo))
            .ReturnsAsync(debitosEsperados);

        // Act
        var resultado = await _decorator.ConsultarDebitos(veiculo);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count());
        Assert.Same(debitosEsperados, resultado);
    }
}
