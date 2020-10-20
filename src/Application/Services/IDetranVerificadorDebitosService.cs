using DesignPatternSamples.Application.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesignPatternSamples.Application.Services
{
    public interface IDetranVerificadorDebitosService
    {
        Task<IEnumerable<DebitoVeiculo>> ConsultarDebitos(Veiculo veiculo);
    }
}
