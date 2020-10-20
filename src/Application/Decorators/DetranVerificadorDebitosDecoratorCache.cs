using DesignPatternSamples.Application.DTO;
using DesignPatternSamples.Application.Services;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Threading.Tasks;
using Workbench.IDistributedCache.Extensions;

namespace DesignPatternSamples.Application.Decorators
{
    public class DetranVerificadorDebitosDecoratorCache : IDetranVerificadorDebitosService
    {
        private readonly IDetranVerificadorDebitosService _Inner;
        private readonly IDistributedCache _Cache;

        public DetranVerificadorDebitosDecoratorCache(
            IDetranVerificadorDebitosService inner,
            IDistributedCache cache)
        {
            _Inner = inner;
            _Cache = cache;
        }

        public Task<IEnumerable<DebitoVeiculo>> ConsultarDebitos(Veiculo veiculo)
        {
            return Task.FromResult(_Cache.GetOrCreate($"{veiculo.UF}_{veiculo.Placa}", () => _Inner.ConsultarDebitos(veiculo).Result));
        }
    }
}