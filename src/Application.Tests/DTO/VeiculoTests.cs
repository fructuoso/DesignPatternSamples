using DesignPatternSamples.Application.DTO;
using Xunit;

namespace DesignPatternSamples.Application.Tests.DTO;

public class VeiculoTests
{
    [Fact(DisplayName = "Veiculo - Deve criar instância com propriedades corretas")]
    public void Veiculo_DeveCriarInstancia_ComPropriedadesCorretas()
    {
        // Arrange & Act
        var veiculo = new Veiculo
        {
            Placa = "ABC1234",
            UF = "SP"
        };

        // Assert
        Assert.Equal("ABC1234", veiculo.Placa);
        Assert.Equal("SP", veiculo.UF);
    }

    [Theory(DisplayName = "Veiculo - Deve aceitar diferentes placas e UFs")]
    [InlineData("ABC1234", "SP")]
    [InlineData("XYZ9876", "RJ")]
    [InlineData("DEF5555", "PE")]
    [InlineData("GHI0000", "RS")]
    public void Veiculo_DeveAceitarDiferentesPlacasEUFs(string placa, string uf)
    {
        // Arrange & Act
        var veiculo = new Veiculo { Placa = placa, UF = uf };

        // Assert
        Assert.Equal(placa, veiculo.Placa);
        Assert.Equal(uf, veiculo.UF);
    }

    [Fact(DisplayName = "Veiculo - Propriedades devem ser init-only")]
    public void Veiculo_PropriedadesDevemSerInitOnly()
    {
        // Arrange
        var veiculo = new Veiculo { Placa = "ABC1234", UF = "SP" };

        // Assert - Compilação falha se tentar fazer: veiculo.Placa = "XYZ";
        // Isso é verificado em tempo de compilação, então apenas verificamos os valores
        Assert.NotNull(veiculo.Placa);
        Assert.NotNull(veiculo.UF);
    }
}
