using AutoMapper;
using DesignPatternSamples.Application.DTO;
using DesignPatternSamples.Application.Services;
using DesignPatternSamples.WebAPI.Controllers.Detran;
using DesignPatternSamples.WebAPI.Models.Detran;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DesignPatternSamples.WebAPI.Tests.Controllers;

public class DebitosControllerTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IDetranVerificadorDebitosService> _serviceMock;
    private readonly DebitosController _controller;

    public DebitosControllerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _serviceMock = new Mock<IDetranVerificadorDebitosService>();
        _controller = new DebitosController(_mapperMock.Object, _serviceMock.Object);
    }

    [Fact(DisplayName = "Get - Deve retornar OK com lista de débitos")]
    public async Task Get_DeveRetornarOK_ComListaDeDebitos()
    {
        // Arrange
        var modelInput = new VeiculoModel { Placa = "ABC1234", UF = "SP" };
        var veiculo = new Veiculo { Placa = "ABC1234", UF = "SP" };
        var debitos = new List<DebitoVeiculo>
        {
            new() { DataOcorrencia = DateTime.Now, Descricao = "IPVA", Valor = 1500.00 }
        };
        var debitosModel = new List<DebitoVeiculoModel>
        {
            new() { DataOcorrencia = DateTime.Now, Descricao = "IPVA", Valor = 1500.00 }
        };

        _mapperMock.Setup(m => m.Map<Veiculo>(modelInput)).Returns(veiculo);
        _serviceMock.Setup(s => s.ConsultarDebitos(veiculo)).ReturnsAsync(debitos);
        _mapperMock.Setup(m => m.Map<IEnumerable<DebitoVeiculoModel>>(debitos)).Returns(debitosModel);

        // Act
        var resultado = await _controller.Get(modelInput);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado);
        Assert.NotNull(okResult.Value);
        
        _mapperMock.Verify(m => m.Map<Veiculo>(modelInput), Times.Once);
        _serviceMock.Verify(s => s.ConsultarDebitos(veiculo), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<DebitoVeiculoModel>>(debitos), Times.Once);
    }

    [Fact(DisplayName = "Get - Deve retornar OK com lista vazia quando não houver débitos")]
    public async Task Get_DeveRetornarOK_ComListaVazia()
    {
        // Arrange
        var modelInput = new VeiculoModel { Placa = "XYZ9876", UF = "RJ" };
        var veiculo = new Veiculo { Placa = "XYZ9876", UF = "RJ" };
        var debitosVazios = new List<DebitoVeiculo>();
        var debitosModelVazios = new List<DebitoVeiculoModel>();

        _mapperMock.Setup(m => m.Map<Veiculo>(modelInput)).Returns(veiculo);
        _serviceMock.Setup(s => s.ConsultarDebitos(veiculo)).ReturnsAsync(debitosVazios);
        _mapperMock.Setup(m => m.Map<IEnumerable<DebitoVeiculoModel>>(debitosVazios)).Returns(debitosModelVazios);

        // Act
        var resultado = await _controller.Get(modelInput);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado);
        Assert.NotNull(okResult.Value);
    }

    [Theory(DisplayName = "Get - Deve consultar débitos para diferentes UFs")]
    [InlineData("ABC1234", "SP")]
    [InlineData("XYZ5678", "RJ")]
    [InlineData("DEF9012", "PE")]
    [InlineData("GHI3456", "RS")]
    public async Task Get_DeveConsultarDebitosParaDiferentesUFs(string placa, string uf)
    {
        // Arrange
        var modelInput = new VeiculoModel { Placa = placa, UF = uf };
        var veiculo = new Veiculo { Placa = placa, UF = uf };
        var debitos = new List<DebitoVeiculo>();
        var debitosModel = new List<DebitoVeiculoModel>();

        _mapperMock.Setup(m => m.Map<Veiculo>(modelInput)).Returns(veiculo);
        _serviceMock.Setup(s => s.ConsultarDebitos(veiculo)).ReturnsAsync(debitos);
        _mapperMock.Setup(m => m.Map<IEnumerable<DebitoVeiculoModel>>(debitos)).Returns(debitosModel);

        // Act
        var resultado = await _controller.Get(modelInput);

        // Assert
        Assert.IsType<OkObjectResult>(resultado);
        _serviceMock.Verify(s => s.ConsultarDebitos(It.Is<Veiculo>(v => v.Placa == placa && v.UF == uf)), Times.Once);
    }

    [Fact(DisplayName = "Get - Deve usar mapper corretamente para conversões")]
    public async Task Get_DeveUsarMapperCorretamente()
    {
        // Arrange
        var modelInput = new VeiculoModel { Placa = "TEST123", UF = "SP" };
        var veiculo = new Veiculo { Placa = "TEST123", UF = "SP" };
        var debitos = new List<DebitoVeiculo>
        {
            new() { DataOcorrencia = DateTime.Now.AddDays(-10), Descricao = "Débito 1", Valor = 100.00 },
            new() { DataOcorrencia = DateTime.Now.AddDays(-5), Descricao = "Débito 2", Valor = 200.00 }
        };
        var debitosModel = new List<DebitoVeiculoModel>
        {
            new() { DataOcorrencia = DateTime.Now.AddDays(-10), Descricao = "Débito 1", Valor = 100.00 },
            new() { DataOcorrencia = DateTime.Now.AddDays(-5), Descricao = "Débito 2", Valor = 200.00 }
        };

        _mapperMock.Setup(m => m.Map<Veiculo>(It.IsAny<VeiculoModel>())).Returns(veiculo);
        _serviceMock.Setup(s => s.ConsultarDebitos(It.IsAny<Veiculo>())).ReturnsAsync(debitos);
        _mapperMock.Setup(m => m.Map<IEnumerable<DebitoVeiculoModel>>(It.IsAny<IEnumerable<DebitoVeiculo>>())).Returns(debitosModel);

        // Act
        await _controller.Get(modelInput);

        // Assert
        _mapperMock.Verify(m => m.Map<Veiculo>(It.Is<VeiculoModel>(v => v.Placa == "TEST123")), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<DebitoVeiculoModel>>(It.Is<IEnumerable<DebitoVeiculo>>(d => d.Count() == 2)), Times.Once);
    }
}
