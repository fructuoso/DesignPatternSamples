using DesignPatternSamples.Application.DTO;
using DesignPatternSamples.Application.Repository;
using DesignPatternSamples.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesignPatternSamples.Application.Implementations
{
    public class DetranVerificadorDebitosServices : IDetranVerificadorDebitosService
    {
        private readonly IDetranVerificadorDebitosFactory _Factory;

        public DetranVerificadorDebitosServices(IDetranVerificadorDebitosFactory factory)
        {
            _Factory = factory;
        }

        public Task<IEnumerable<DebitoVeiculo>> ConsultarDebitos(Veiculo veiculo)
        {
            IDetranVerificadorDebitosRepository repository = _Factory.Create(veiculo.UF);
            return repository.ConsultarDebitos(veiculo);
        }
    }
}
