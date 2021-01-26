using DesignPatternSamples.Application.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesignPatternSamples.Application.Repository
{
    public interface IDetranPontosCarteiraRepository
    {
        Task<IEnumerable<PontosCarteira>> ConsultarPontos(Carteira carteira);
    }
}
