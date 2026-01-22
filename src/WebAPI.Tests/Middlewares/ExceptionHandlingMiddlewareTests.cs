using System.Net;
using System.Text.Json;
using DesignPatternSamples.WebAPI.Middlewares;
using DesignPatternSamples.WebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DesignPatternSamples.WebAPI.Tests.Middlewares;

public class ExceptionHandlingMiddlewareTests
{
    private readonly Mock<ILogger<ExceptionHandlingMiddleware>> _loggerMock;
    private readonly ExceptionHandlingMiddleware _middleware;

    public ExceptionHandlingMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        _middleware = new ExceptionHandlingMiddleware(_loggerMock.Object);
    }

    [Fact(DisplayName = "InvokeAsync - Deve chamar próximo middleware quando não há exceção")]
    public async Task InvokeAsync_DeveChamarProximoMiddleware_QuandoNaoHaExcecao()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var nextCalled = false;
        RequestDelegate next = (ctx) =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        // Act
        await _middleware.InvokeAsync(context, next);

        // Assert
        Assert.True(nextCalled);
    }

    [Fact(DisplayName = "InvokeAsync - Deve capturar exceção e retornar erro 500")]
    public async Task InvokeAsync_DeveCapturarExcecao_ERetornarErro500()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var exception = new InvalidOperationException("Erro de teste");
        RequestDelegate next = (ctx) => throw exception;

        // Act
        await _middleware.InvokeAsync(context, next);

        // Assert
        Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.Is<Exception>(ex => ex == exception),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact(DisplayName = "InvokeAsync - Deve retornar JSON com mensagem de erro")]
    public async Task InvokeAsync_DeveRetornarJsonComMensagemDeErro()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        RequestDelegate next = (ctx) => throw new Exception("Erro de teste");

        // Act
        await _middleware.InvokeAsync(context, next);

        // Assert
        responseBody.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(responseBody);
        var responseText = await reader.ReadToEndAsync();

        Assert.Contains("Ocorreu um erro inesperado", responseText);
        Assert.Contains("\"HasSucceeded\":false", responseText);
    }

    [Theory(DisplayName = "InvokeAsync - Deve capturar diferentes tipos de exceções")]
    [InlineData(typeof(InvalidOperationException))]
    [InlineData(typeof(ArgumentException))]
    [InlineData(typeof(NullReferenceException))]
    [InlineData(typeof(Exception))]
    public async Task InvokeAsync_DeveCapturarDiferentesTiposDeExcecoes(Type exceptionType)
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var exception = (Exception)Activator.CreateInstance(exceptionType, "Erro de teste")!;
        RequestDelegate next = (ctx) => throw exception;

        // Act
        await _middleware.InvokeAsync(context, next);

        // Assert
        Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.Is<Exception>(ex => ex.GetType() == exceptionType),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact(DisplayName = "InvokeAsync - Deve logar mensagem de exceção")]
    public async Task InvokeAsync_DeveLogarMensagemDeExcecao()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var mensagemErro = "Mensagem de erro específica";
        var exception = new Exception(mensagemErro);
        RequestDelegate next = (ctx) => throw exception;

        // Act
        await _middleware.InvokeAsync(context, next);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.Is<Exception>(ex => ex.Message == mensagemErro),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
