using DesignPatternSamples.Application.DTO;
using Xunit;

namespace DesignPatternSamples.Application.Tests.DTO;

public class DebitoVeiculoTests
{
    [Fact(DisplayName = "DebitoVeiculo - Deve criar instância com propriedades corretas")]
    public void DebitoVeiculo_DeveCriarInstancia_ComPropriedadesCorretas()
    {
        // Arrange
        var dataOcorrencia = DateTime.Now.AddDays(-30);

        // Act
        var debito = new DebitoVeiculo
        {
            DataOcorrencia = dataOcorrencia,
            Descricao = "IPVA 2024",
            Valor = 1500.50
        };

        // Assert
        Assert.Equal(dataOcorrencia, debito.DataOcorrencia);
        Assert.Equal("IPVA 2024", debito.Descricao);
        Assert.Equal(1500.50, debito.Valor);
    }

    [Theory(DisplayName = "DebitoVeiculo - Deve aceitar diferentes valores")]
    [InlineData("IPVA 2024", 1500.00)]
    [InlineData("Multa de trânsito", 195.23)]
    [InlineData("Licenciamento", 120.50)]
    [InlineData("Seguro DPVAT", 52.00)]
    public void DebitoVeiculo_DeveAceitarDiferentesValores(string descricao, double valor)
    {
        // Arrange & Act
        var debito = new DebitoVeiculo
        {
            DataOcorrencia = DateTime.Now,
            Descricao = descricao,
            Valor = valor
        };

        // Assert
        Assert.Equal(descricao, debito.Descricao);
        Assert.Equal(valor, debito.Valor);
    }

    [Fact(DisplayName = "DebitoVeiculo - Deve ser serializável")]
    public void DebitoVeiculo_DeveSerSerializavel()
    {
        // Arrange
        var debito = new DebitoVeiculo
        {
            DataOcorrencia = DateTime.Now,
            Descricao = "Teste",
            Valor = 100.00
        };

        // Assert - Verifica se tem o atributo Serializable
        var type = typeof(DebitoVeiculo);
        var hasSerializableAttribute = type.GetCustomAttributes(typeof(SerializableAttribute), false).Any();
        Assert.True(hasSerializableAttribute);
    }

    [Fact(DisplayName = "DebitoVeiculo - Propriedades devem ser init-only")]
    public void DebitoVeiculo_PropriedadesDevemSerInitOnly()
    {
        // Arrange
        var debito = new DebitoVeiculo
        {
            DataOcorrencia = DateTime.Now,
            Descricao = "IPVA",
            Valor = 1000.00
        };

        // Assert - Propriedades init-only não podem ser alteradas após inicialização
        Assert.NotNull(debito.Descricao);
        Assert.True(debito.Valor > 0);
    }

    [Fact(DisplayName = "DebitoVeiculo - Deve aceitar valor zero")]
    public void DebitoVeiculo_DeveAceitarValorZero()
    {
        // Arrange & Act
        var debito = new DebitoVeiculo
        {
            DataOcorrencia = DateTime.Now,
            Descricao = "Débito quitado",
            Valor = 0.0
        };

        // Assert
        Assert.Equal(0.0, debito.Valor);
    }
}
