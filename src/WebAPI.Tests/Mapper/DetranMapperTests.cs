using AutoMapper;
using DesignPatternSamples.Application.DTO;
using DesignPatternSamples.WebAPI.Mapper;
using DesignPatternSamples.WebAPI.Models.Detran;
using Xunit;

namespace DesignPatternSamples.WebAPI.Tests.Mapper;

public class DetranMapperTests
{
    private readonly IMapper _mapper;

    public DetranMapperTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<DetranMapper>();
        });
        _mapper = config.CreateMapper();
    }

    [Fact(DisplayName = "Map - Deve mapear VeiculoModel para Veiculo corretamente")]
    public void Map_DevMapearVeiculoModel_ParaVeiculo()
    {
        // Arrange
        var veiculoModel = new VeiculoModel
        {
            Placa = "ABC1234",
            UF = "SP"
        };

        // Act
        var veiculo = _mapper.Map<Veiculo>(veiculoModel);

        // Assert
        Assert.NotNull(veiculo);
        Assert.Equal("ABC1234", veiculo.Placa);
        Assert.Equal("SP", veiculo.UF);
    }

    [Fact(DisplayName = "Map - Deve mapear DebitoVeiculo para DebitoVeiculoModel corretamente")]
    public void Map_DeveMapearDebitoVeiculo_ParaDebitoVeiculoModel()
    {
        // Arrange
        var dataOcorrencia = DateTime.Now.AddDays(-15);
        var debitoVeiculo = new DebitoVeiculo
        {
            DataOcorrencia = dataOcorrencia,
            Descricao = "IPVA 2024",
            Valor = 1500.50
        };

        // Act
        var debitoModel = _mapper.Map<DebitoVeiculoModel>(debitoVeiculo);

        // Assert
        Assert.NotNull(debitoModel);
        Assert.Equal(dataOcorrencia, debitoModel.DataOcorrencia);
        Assert.Equal("IPVA 2024", debitoModel.Descricao);
        Assert.Equal(1500.50, debitoModel.Valor);
    }

    [Fact(DisplayName = "Map - Deve mapear lista de DebitoVeiculo para lista de DebitoVeiculoModel")]
    public void Map_DeveMapearListaDebitoVeiculo_ParaListaDebitoVeiculoModel()
    {
        // Arrange
        var debitos = new List<DebitoVeiculo>
        {
            new() { DataOcorrencia = DateTime.Now.AddDays(-30), Descricao = "IPVA", Valor = 1500.00 },
            new() { DataOcorrencia = DateTime.Now.AddDays(-15), Descricao = "Multa", Valor = 195.23 },
            new() { DataOcorrencia = DateTime.Now.AddDays(-5), Descricao = "Licenciamento", Valor = 120.50 }
        };

        // Act
        var debitosModel = _mapper.Map<IEnumerable<DebitoVeiculoModel>>(debitos);

        // Assert
        Assert.NotNull(debitosModel);
        Assert.Equal(3, debitosModel.Count());
        Assert.Contains(debitosModel, d => d.Descricao == "IPVA");
        Assert.Contains(debitosModel, d => d.Descricao == "Multa");
        Assert.Contains(debitosModel, d => d.Descricao == "Licenciamento");
    }

    [Theory(DisplayName = "Map - Deve mapear diferentes veículos corretamente")]
    [InlineData("ABC1234", "SP")]
    [InlineData("XYZ9876", "RJ")]
    [InlineData("DEF5555", "PE")]
    [InlineData("GHI0000", "RS")]
    public void Map_DeveMapearDiferentesVeiculos(string placa, string uf)
    {
        // Arrange
        var veiculoModel = new VeiculoModel { Placa = placa, UF = uf };

        // Act
        var veiculo = _mapper.Map<Veiculo>(veiculoModel);

        // Assert
        Assert.Equal(placa, veiculo.Placa);
        Assert.Equal(uf, veiculo.UF);
    }

    [Fact(DisplayName = "Configuration - Deve ter configuração válida")]
    public void Configuration_DeveTerConfiguracaoValida()
    {
        // Arrange & Act & Assert
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<DetranMapper>();
        });

        config.AssertConfigurationIsValid();
    }
}
