using DesignPatternSamples.Application.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace DesignPatternSamples.Infra.Repository.Detran;

public class DetranVerificadorDebitosFactory : IDetranVerificadorDebitosFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDictionary<string, Type> _repositories = new Dictionary<string, Type>();

    public DetranVerificadorDebitosFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IDetranVerificadorDebitosRepository? Create(string uf)
    {
        if (_repositories.TryGetValue(uf, out Type? type))
        {
            return _serviceProvider.GetService(type) as IDetranVerificadorDebitosRepository;
        }

        return null;
    }

    public IDetranVerificadorDebitosFactory Register(string uf, Type repository)
    {
        if (!_repositories.TryAdd(uf, repository))
        {
            _repositories[uf] = repository;
        }

        return this;
    }
}
