using DesignPatternSamples.Application.DTO;
using DesignPatternSamples.Application.Repository;
using Microsoft.Extensions.Logging;

namespace DesignPatternSamples.Infra.Repository.Detran;

public class DetranSPVerificadorDebitosRepository : IDetranVerificadorDebitosRepository
{
    private readonly ILogger _logger;

    public DetranSPVerificadorDebitosRepository(ILogger<DetranSPVerificadorDebitosRepository> logger)
    {
        _logger = logger;
    }

    public Task<IEnumerable<DebitoVeiculo>> ConsultarDebitos(Veiculo veiculo)
    {
        _logger.LogDebug("Consultando débitos do veículo placa {Placa} para o estado de SP.", veiculo.Placa);
        return Task.FromResult<IEnumerable<DebitoVeiculo>>([new DebitoVeiculo
        {
            DataOcorrencia = DateTime.Now,
            Descricao = "Débito exemplo",
            Valor = 100.00
        }]);
    }
}
