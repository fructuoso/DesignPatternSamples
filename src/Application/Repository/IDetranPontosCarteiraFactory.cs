using System;

namespace DesignPatternSamples.Application.Repository
{
    public interface IDetranPontosCarteiraFactory
    {
        public IDetranPontosCarteiraFactory Register(string UF, Type repository);
        public IDetranPontosCarteiraRepository Create(string UF);
    }
}