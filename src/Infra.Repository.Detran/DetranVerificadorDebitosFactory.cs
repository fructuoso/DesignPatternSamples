using DesignPatternSamples.Application.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DesignPatternSamples.Infra.Repository.Detran
{
    public class DetranVerificadorDebitosFactory : IDetranVerificadorDebitosFactory
    {
        private readonly IServiceProvider _ServiceProvider;

        public DetranVerificadorDebitosFactory(IServiceProvider serviceProvider)
        {
            _ServiceProvider = serviceProvider;
        }

        public IDetranVerificadorDebitosRepository Create(string UF)
        {
            IDetranVerificadorDebitosRepository result = null;
            switch (UF)
            {
                case "PE":
                    result = _ServiceProvider.GetService<DetranPEVerificadorDebitosRepository>();
                    break;
                case "SP":
                    result = _ServiceProvider.GetService<DetranSPVerificadorDebitosRepository>();
                    break;
                default:
                    break;
            }

            return result;
        }
    }
}
