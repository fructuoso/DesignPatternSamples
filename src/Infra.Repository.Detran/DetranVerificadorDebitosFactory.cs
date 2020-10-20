using DesignPatternSamples.Application.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DesignPatternSamples.Infra.Repository.Detran
{
    public class DetranVerificadorDebitosFactory : IDetranVerificadorDebitosFactory
    {
        private readonly IServiceProvider _ServiceProvider;
        private readonly IDictionary<string, Type> _Repositories = new Dictionary<string, Type>();

        public DetranVerificadorDebitosFactory(IServiceProvider serviceProvider)
        {
            _ServiceProvider = serviceProvider;

            //Código MOCK, pode ser fornecido de forma externa.
            _Repositories.Add("PE", typeof(DetranPEVerificadorDebitosRepository));
            _Repositories.Add("RJ", typeof(DetranRJVerificadorDebitosRepository));
            _Repositories.Add("SP", typeof(DetranSPVerificadorDebitosRepository));
            _Repositories.Add("RS", typeof(DetranRSVerificadorDebitosRepository));
        }

        public IDetranVerificadorDebitosRepository Create(string UF)
        {
            IDetranVerificadorDebitosRepository result = null;

            if (_Repositories.TryGetValue(UF, out Type type))
            {
                result = _ServiceProvider.GetService(type) as IDetranVerificadorDebitosRepository;
            }

            return result;
        }
    }
}
