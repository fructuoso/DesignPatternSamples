using DesignPatternSamples.Application.DTO;

namespace DesignPatternSamples.Application.Repository;

public interface IDetranVerificadorDebitosRepository
{
    Task<IEnumerable<DebitoVeiculo>> ConsultarDebitos(Veiculo veiculo);
}
