using DesignPatternSamples.Application.DTO;
using Microsoft.Extensions.Logging;

namespace DesignPatternSamples.Infra.Repository.Detran;

public class DetranRSVerificadorDebitosRepository : DetranVerificadorDebitosRepositoryCrawlerBase
{
    private readonly ILogger _logger;

    public DetranRSVerificadorDebitosRepository(ILogger<DetranRSVerificadorDebitosRepository> logger)
    {
        _logger = logger;
    }

    protected override Task<IEnumerable<DebitoVeiculo>> PadronizarResultado(string html)
    {
        _logger.LogDebug("Padronizando o Resultado {Html}.", html);
        return Task.FromResult<IEnumerable<DebitoVeiculo>>([new DebitoVeiculo
        {
            DataOcorrencia = DateTime.Now,
            Descricao = "Débito RS",
            Valor = 180.00
        }]);
    }

    protected override Task<string> RealizarAcesso(Veiculo veiculo)
    {
        _logger.LogDebug("Consultando débitos do veículo placa {Placa} para o estado de RS.", veiculo.Placa);
        return Task.FromResult("CONTEUDO DO SITE DO DETRAN/RS");
    }
}
