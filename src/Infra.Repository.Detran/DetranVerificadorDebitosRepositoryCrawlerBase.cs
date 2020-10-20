using DesignPatternSamples.Application.DTO;
using DesignPatternSamples.Application.Repository;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesignPatternSamples.Infra.Repository.Detran
{
    public abstract class DetranVerificadorDebitosRepositoryCrawlerBase : IDetranVerificadorDebitosRepository
    {
        private readonly ILogger _Logger;

        protected DetranVerificadorDebitosRepositoryCrawlerBase(ILogger logger)
        {
            _Logger = logger;
        }

        public async Task<IEnumerable<DebitoVeiculo>> ConsultarDebitos(Veiculo veiculo)
        {
            var html = await RealizarAcesso(veiculo);
            return await PadronizarResultado(html);
        }

        protected abstract Task<string> RealizarAcesso(Veiculo veiculo);
        protected abstract Task<IEnumerable<DebitoVeiculo>> PadronizarResultado(string html);
    }
}
