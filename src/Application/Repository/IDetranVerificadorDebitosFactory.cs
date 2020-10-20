using System;

namespace DesignPatternSamples.Application.Repository
{
    public interface IDetranVerificadorDebitosFactory
    {
        public IDetranVerificadorDebitosFactory Register(string UF, Type repository);
        public IDetranVerificadorDebitosRepository Create(string UF);
    }
}
