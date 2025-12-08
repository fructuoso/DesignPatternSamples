namespace DesignPatternSamples.Application.DTO;

[Serializable]
public class DebitoVeiculo
{
    public required DateTime DataOcorrencia { get; init; }
    public required string Descricao { get; init; }
    public required double Valor { get; init; }
}
