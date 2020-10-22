using DesignPatternSamples.Application.DTO;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesignPatternSamples.Infra.Repository.Detran
{
    public class DetranRSVerificadorDebitosRepository : DetranVerificadorDebitosRepositoryCrawlerBase
    {
        private readonly ILogger _Logger;

        public DetranRSVerificadorDebitosRepository(ILogger<DetranRJVerificadorDebitosRepository> logger)
        {
            _Logger = logger;
        }

        protected override Task<IEnumerable<DebitoVeiculo>> PadronizarResultado(string html)
        {
            _Logger.LogDebug($"Padronizando o Resultado {html}.");
            return Task.FromResult<IEnumerable<DebitoVeiculo>>(new List<DebitoVeiculo>() { new DebitoVeiculo() });
        }

        protected override Task<string> RealizarAcesso(Veiculo veiculo)
        {
            _Logger.LogDebug($"Consultando débitos do veículo placa {veiculo.Placa} para o estado de RS.");
            return Task.FromResult("CONTEUDO DO SITE DO DETRAN/RS");
        }
    }
}
