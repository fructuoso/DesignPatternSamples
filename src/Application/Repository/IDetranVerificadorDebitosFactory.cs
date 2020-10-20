namespace DesignPatternSamples.Application.Repository
{
    public interface IDetranVerificadorDebitosFactory
    {
        public IDetranVerificadorDebitosRepository Create(string UF);
    }
}
