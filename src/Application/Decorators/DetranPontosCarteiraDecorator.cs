using DesignPatternSamples.Application.DTO;
using DesignPatternSamples.Application.Services;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Threading.Tasks;
using Workbench.IDistributedCache.Extensions;

namespace DesignPatternSamples.Application.Decorators
{
    public class DetranPontosCarteiraDecorator : IDetranPontosCarteiraService
    {
        private readonly DetranPontosCarteiraDecorator _Inner;
        private readonly IDistributedCache _Cache;

        public DetranPontosCarteiraDecorator(
            DetranPontosCarteiraDecorator inner,
            IDistributedCache cache)
        {
            _Inner = inner;
            _Cache = cache;
        }

        public Task<IEnumerable<PontosCarteira>> ConsultarPontos(Carteira carteira)
        {
            return Task.FromResult(_Cache.GetOrCreate($"{carteira.UF}_{carteira.Numero}", () => _Inner.ConsultarPontos(carteira).Result));
        }
    }
}