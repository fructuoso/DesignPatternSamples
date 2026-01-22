using DesignPatternSamples.Application.DTO;
using Microsoft.Extensions.Logging;

namespace DesignPatternSamples.Infra.Repository.Detran;

public class DetranPEVerificadorDebitosRepository : DetranVerificadorDebitosRepositoryCrawlerBase
{
    private readonly ILogger _logger;

    public DetranPEVerificadorDebitosRepository(ILogger<DetranPEVerificadorDebitosRepository> logger)
    {
        _logger = logger;
    }

    protected override Task<IEnumerable<DebitoVeiculo>> PadronizarResultado(string html)
    {
        _logger.LogDebug("Padronizando o Resultado {Html}.", html);
        return Task.FromResult<IEnumerable<DebitoVeiculo>>([new DebitoVeiculo
        {
            DataOcorrencia = DateTime.UtcNow,
            Descricao = "Débito PE",
            Valor = 150.00
        }]);
    }

    protected override async Task<string> RealizarAcesso(Veiculo veiculo)
    {
        await Task.Delay(5000); // Deixando o serviço mais lento para evidenciar o uso do CACHE.
        _logger.LogDebug("Consultando débitos do veículo placa {Placa} para o estado de PE.", veiculo.Placa);
        return "CONTEUDO DO SITE DO DETRAN/PE";
    }
}
