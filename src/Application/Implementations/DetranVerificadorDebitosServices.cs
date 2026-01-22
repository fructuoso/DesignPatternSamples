using DesignPatternSamples.Application.DTO;
using DesignPatternSamples.Application.Repository;
using DesignPatternSamples.Application.Services;

namespace DesignPatternSamples.Application.Implementations;

public class DetranVerificadorDebitosServices : IDetranVerificadorDebitosService
{
    private readonly IDetranVerificadorDebitosFactory _factory;

    public DetranVerificadorDebitosServices(IDetranVerificadorDebitosFactory factory)
    {
        _factory = factory;
    }

    public Task<IEnumerable<DebitoVeiculo>> ConsultarDebitos(Veiculo veiculo)
    {
        var repository = _factory.Create(veiculo.UF);
        if (repository is null)
        {
            throw new InvalidOperationException($"Nenhum repositório encontrado para UF: {veiculo.UF}");
        }
        return repository.ConsultarDebitos(veiculo);
    }
}
