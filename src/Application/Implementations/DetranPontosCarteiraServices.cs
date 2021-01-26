using DesignPatternSamples.Application.DTO;
using DesignPatternSamples.Application.Repository;
using DesignPatternSamples.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesignPatternSamples.Application.Implementations
{
    public class DetranPontosCarteiraServices : IDetranPontosCarteiraService
    {
        private readonly IDetranPontosCarteiraFactory _Factory;

        public DetranPontosCarteiraServices(IDetranPontosCarteiraFactory factory)
        {
            _Factory = factory;
        }

        public Task<IEnumerable<PontosCarteira>> ConsultarPontos(Carteira Carteira)
        {
            IDetranPontosCarteiraRepository repository = _Factory.Create(carteira.UF);
            return repository.ConsultarPontos(carteira);
        }
    }
}