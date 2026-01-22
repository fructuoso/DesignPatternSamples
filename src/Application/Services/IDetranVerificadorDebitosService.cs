using DesignPatternSamples.Application.DTO;

namespace DesignPatternSamples.Application.Services;

public interface IDetranVerificadorDebitosService
{
    Task<IEnumerable<DebitoVeiculo>> ConsultarDebitos(Veiculo veiculo);
}
