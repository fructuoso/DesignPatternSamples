using DesignPatternSamples.Application.DTO;
using Microsoft.Extensions.Logging;

namespace DesignPatternSamples.Infra.Repository.Detran;

public class DetranRJVerificadorDebitosRepository : DetranVerificadorDebitosRepositoryCrawlerBase
{
    private readonly ILogger _logger;

    public DetranRJVerificadorDebitosRepository(ILogger<DetranRJVerificadorDebitosRepository> logger)
    {
        _logger = logger;
    }

    protected override Task<IEnumerable<DebitoVeiculo>> PadronizarResultado(string html)
    {
        _logger.LogDebug("Padronizando o Resultado {Html}.", html);
        return Task.FromResult<IEnumerable<DebitoVeiculo>>([new DebitoVeiculo
        {
            DataOcorrencia = DateTime.Now,
            Descricao = "Débito RJ",
            Valor = 200.00
        }]);
    }

    protected override Task<string> RealizarAcesso(Veiculo veiculo)
    {
        _logger.LogDebug("Consultando débitos do veículo placa {Placa} para o estado de RJ.", veiculo.Placa);
        return Task.FromResult("CONTEUDO DO SITE DO DETRAN/RJ");
    }
}
