using System.Collections.Generic;
using System.Threading.Tasks;
using DesignPatternSamples.Application.DTO;

namespace DesignPatternSamples.Application.Services
{
    public interface IDetranVerificadorPontosCarteiraService
    {
        Task<IEnumerable<PontosCarteira>> ConsultarPontos(Condutor condutor);
    }
}