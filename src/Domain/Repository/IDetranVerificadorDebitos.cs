using System.Threading.Tasks;

namespace DesignPatternSamples.Domain.Repository.Detran
{
    public interface IDetranVerificadorDebitos
    {
        Task<DebitoVeiculo> ConsultarDebitos(Veiculo veiculo);
    }
}
