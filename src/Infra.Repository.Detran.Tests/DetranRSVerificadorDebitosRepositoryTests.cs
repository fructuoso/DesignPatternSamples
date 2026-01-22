using DesignPatternSamples.Application.DTO;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DesignPatternSamples.Infra.Repository.Detran.Tests;

public class DetranRSVerificadorDebitosRepositoryTests
{
    private readonly Mock<ILogger<DetranRSVerificadorDebitosRepository>> _loggerMock;
    private readonly DetranRSVerificadorDebitosRepository _repository;

    public DetranRSVerificadorDebitosRepositoryTests()
    {
        _loggerMock = new Mock<ILogger<DetranRSVerificadorDebitosRepository>>();
        _repository = new DetranRSVerificadorDebitosRepository(_loggerMock.Object);
    }

    [Fact(DisplayName = "ConsultarDebitos - Deve retornar débitos de RS")]
    public async Task ConsultarDebitos_DeveRetornarDebitosDeRS()
    {
        // Arrange
        var veiculo = new Veiculo { Placa = "IRS-1234", UF = "RS" };

        // Act
        var result = await _repository.ConsultarDebitos(veiculo);

        // Assert
        Assert.NotNull(result);
        var debitos = result.ToList();
        Assert.Single(debitos);
        
        var debito = debitos.First();
        Assert.Equal("Débito RS", debito.Descricao);
        Assert.Equal(180.00, debito.Valor);
        Assert.NotEqual(default, debito.DataOcorrencia);
    }

    [Fact(DisplayName = "ConsultarDebitos - Deve registrar log de debug")]
    public async Task ConsultarDebitos_DeveRegistrarLogDeDebug()
    {
        // Arrange
        var veiculo = new Veiculo { Placa = "POA-789", UF = "RS" };

        // Act
        await _repository.ConsultarDebitos(veiculo);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("POA-789")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    [Theory(DisplayName = "ConsultarDebitos - Deve funcionar com diferentes placas")]
    [InlineData("IRS-1111")]
    [InlineData("POA-2222")]
    [InlineData("GAU-3333")]
    public async Task ConsultarDebitos_DeveFuncionarComDiferentesPlacas(string placa)
    {
        // Arrange
        var veiculo = new Veiculo { Placa = placa, UF = "RS" };

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

    [Fact(DisplayName = "ConsultarDebitos - Deve retornar débito com valor específico de RS")]
    public async Task ConsultarDebitos_DeveRetornarDebitoComValorEspecificoDeRS()
    {
        // Arrange
        var veiculo = new Veiculo { Placa = "TEST-RS", UF = "RS" };

        // Act
        var result = await _repository.ConsultarDebitos(veiculo);

        // Assert
        var debito = result.First();
        Assert.Equal(180.00, debito.Valor);
    }

    [Fact(DisplayName = "ConsultarDebitos - Deve incluir descrição específica de RS")]
    public async Task ConsultarDebitos_DeveIncluirDescricaoEspecificaDeRS()
    {
        // Arrange
        var veiculo = new Veiculo { Placa = "RS-TEST", UF = "RS" };

        // Act
        var result = await _repository.ConsultarDebitos(veiculo);

        // Assert
        var debito = result.First();
        Assert.Contains("RS", debito.Descricao);
    }
}
