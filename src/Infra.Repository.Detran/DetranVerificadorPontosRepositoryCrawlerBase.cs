using System.Collections.Generic;
using System.Threading.Tasks;
using DesignPatternSamples.Application.DTO;
using DesignPatternSamples.Application.Repository;

namespace DesignPatternSamples.Infra.Repository.Detran
{
    public abstract class DetranVerificadorPontosRepositoryCrawlerBase : IDetranVerificadorPontosRepository
    {
        public async Task<IEnumerable<PontosCarteira>> ConsultarPontos(Condutor condutor)
        {
            var html = await RealizarAcesso(condutor);
            return await PadronizarResultado(html);
        }

        protected abstract Task<string> RealizarAcesso(Condutor condutor);
        protected abstract Task<IEnumerable<PontosCarteira>> PadronizarResultado(string html);
    }
}