using System;

namespace DesignPatternSamples.Application.Repository
{
    public interface IDetranVerificadorPontosFactory
    {
        public IDetranVerificadorPontosFactory Register(string UF, Type repository);
        public IDetranVerificadorPontosRepository Create(string UF);
    }
}