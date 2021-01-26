using System.Threading.Tasks;

namespace DesignPatternSamples.Domain.Repository.Detran
{
    public interface IDetranPontosCarteira
    {
        Task<PontosCarteira> ConsultarPontos(Carteira carteira);
    }
}
