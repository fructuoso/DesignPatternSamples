namespace DesignPatternSamples.Application.Repository;

public interface IDetranVerificadorDebitosFactory
{
    IDetranVerificadorDebitosFactory Register(string uf, Type repository);
    IDetranVerificadorDebitosRepository? Create(string uf);
}
