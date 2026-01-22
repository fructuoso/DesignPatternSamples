using DesignPatternSamples.Application.DTO;
using DesignPatternSamples.Application.Services;
using Microsoft.Extensions.Caching.Distributed;
using Workbench.IDistributedCache.Extensions;

namespace DesignPatternSamples.Application.Decorators;

public class DetranVerificadorDebitosDecoratorCache : IDetranVerificadorDebitosService
{
    private readonly IDetranVerificadorDebitosService _inner;
    private readonly IDistributedCache _cache;

    private const int DuracaoCache = 20;

    public DetranVerificadorDebitosDecoratorCache(
        IDetranVerificadorDebitosService inner,
        IDistributedCache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public Task<IEnumerable<DebitoVeiculo>> ConsultarDebitos(Veiculo veiculo)
    {
        return _cache.GetOrCreateAsync($"{veiculo.UF}_{veiculo.Placa}", () => _inner.ConsultarDebitos(veiculo), DuracaoCache);
    }
}
