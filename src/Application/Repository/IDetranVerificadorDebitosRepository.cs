using DesignPatternSamples.Application.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesignPatternSamples.Application.Repository
{
    public interface IDetranVerificadorDebitosRepository
    {
        Task<IEnumerable<DebitoVeiculo>> ConsultarDebitos(Veiculo veiculo);
    }
}
