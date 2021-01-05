using System.Collections.Generic;
using System.Threading.Tasks;
using DesignPatternSamples.Application.DTO;

namespace DesignPatternSamples.Application.Repository
{
    public interface IDetranVerificadorPontosRepository
    {
        Task<IEnumerable<PontosCarteira>> ConsultarPontos(Condutor condutor);
    }
}