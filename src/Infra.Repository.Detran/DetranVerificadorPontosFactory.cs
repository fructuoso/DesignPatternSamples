using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DesignPatternSamples.Application.DTO;
using DesignPatternSamples.Application.Repository;

namespace DesignPatternSamples.Infra.Repository.Detran
{
    public class DetranVerificadorPontosFactory : IDetranVerificadorPontosFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDictionary<string, Type> _repositories = new Dictionary<string, Type>();

        public DetranVerificadorPontosFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public IDetranVerificadorPontosFactory Register(string UF, Type repository)
        {
            if (!_repositories.TryAdd(UF, repository))
            {
                _repositories[UF] = repository;
            }
            return this;
        }

        public IDetranVerificadorPontosRepository Create(string UF)
        {
            IDetranVerificadorPontosRepository resultado = null;
            if (_repositories.TryGetValue(UF, out Type type))
            {
                resultado = _serviceProvider.GetService(type) as IDetranVerificadorPontosRepository;
            }
            return resultado;
        }
    }
}