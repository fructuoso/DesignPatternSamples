using DesignPatternSamples.WebAPI.Models;
using Xunit;

namespace DesignPatternSamples.WebAPI.Tests.Models;

public class ResultModelTests
{
    [Fact(DisplayName = "SuccessResultModel - Deve criar resultado de sucesso com dados")]
    public void SuccessResultModel_DeveCriarResultadoDeSucesso_ComDados()
    {
        // Arrange
        var data = "Teste de dados";

        // Act
        var result = new SuccessResultModel<string>(data);

        // Assert
        Assert.True(result.HasSucceeded);
        Assert.Equal(data, result.Data);
        Assert.Null(result.Details);
    }

    [Fact(DisplayName = "SuccessResultModel - Deve criar resultado de sucesso com dados e detalhes")]
    public void SuccessResultModel_DeveCriarResultadoDeSucesso_ComDadosEDetalhes()
    {
        // Arrange
        var data = "Teste";
        var details = new List<ResultDetail>
        {
            new ResultDetail("Detalhe 1"),
            new ResultDetail("Detalhe 2")
        };

        // Act
        var result = new SuccessResultModel<string>(data, details);

        // Assert
        Assert.True(result.HasSucceeded);
        Assert.Equal(data, result.Data);
        Assert.Equal(2, result.Details.Count());
    }

    [Fact(DisplayName = "SuccessResultModel - Deve criar resultado vazio sem dados")]
    public void SuccessResultModel_DeveCriarResultadoVazio()
    {
        // Act
        var result = new SuccessResultModel();

        // Assert
        Assert.True(result.HasSucceeded);
        Assert.Null(result.Data);
    }

    [Fact(DisplayName = "FailureResultModel - Deve criar resultado de falha com detalhes")]
    public void FailureResultModel_DeveCriarResultadoDeFalha_ComDetalhes()
    {
        // Arrange
        var details = new List<ResultDetail>
        {
            new ResultDetail("Erro 1"),
            new ResultDetail("Erro 2")
        };

        // Act
        var result = new FailureResultModel(details);

        // Assert
        Assert.False(result.HasSucceeded);
        Assert.Null(result.Data);
        Assert.Equal(2, result.Details.Count());
    }

    [Fact(DisplayName = "FailureResultModel - Deve criar resultado de falha com um detalhe")]
    public void FailureResultModel_DeveCriarResultadoDeFalha_ComUmDetalhe()
    {
        // Arrange
        var detail = new ResultDetail("Erro único");

        // Act
        var result = new FailureResultModel(detail);

        // Assert
        Assert.False(result.HasSucceeded);
        Assert.Single(result.Details);
        Assert.Equal("Erro único", result.Details.First().Message);
    }

    [Fact(DisplayName = "FailureResultModel - Deve criar resultado de falha com string")]
    public void FailureResultModel_DeveCriarResultadoDeFalha_ComString()
    {
        // Arrange
        var mensagem = "Mensagem de erro";

        // Act
        var result = new FailureResultModel(mensagem);

        // Assert
        Assert.False(result.HasSucceeded);
        Assert.Single(result.Details);
        Assert.Equal(mensagem, result.Details.First().Message);
    }

    [Fact(DisplayName = "ResultDetail - Deve criar detalhe com mensagem")]
    public void ResultDetail_DeveCriarDetalheComMensagem()
    {
        // Arrange
        var mensagem = "Mensagem de teste";

        // Act
        var detail = new ResultDetail(mensagem);

        // Assert
        Assert.Equal(mensagem, detail.Message);
    }

    [Fact(DisplayName = "FailureResultModel tipado - Deve criar resultado de falha com dados e detalhes")]
    public void FailureResultModelTipado_DeveCriarResultadoDeFalha()
    {
        // Arrange
        var data = 123;
        var details = new List<ResultDetail>
        {
            new ResultDetail("Erro ao processar")
        };

        // Act
        var result = new FailureResultModel<int>(data, details);

        // Assert
        Assert.False(result.HasSucceeded);
        Assert.Equal(123, result.Data);
        Assert.Single(result.Details);
    }

    [Fact(DisplayName = "IResultModel - Deve implementar interface corretamente")]
    public void IResultModel_DeveImplementarInterfaceCorretamente()
    {
        // Arrange & Act
        IResultModel<string> successResult = new SuccessResultModel<string>("Sucesso");
        IResultModel<string> failureResult = new FailureResultModel<string>(null!, new List<ResultDetail> { new ResultDetail("Falha") });

        // Assert
        Assert.True(successResult.HasSucceeded);
        Assert.False(failureResult.HasSucceeded);
    }
}
