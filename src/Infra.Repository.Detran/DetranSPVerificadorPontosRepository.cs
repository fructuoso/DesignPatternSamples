using System.Collections.Generic;
using System.Threading.Tasks;
using DesignPatternSamples.Application.DTO;
using DesignPatternSamples.Application.Repository;
using Microsoft.Extensions.Logging;

namespace DesignPatternSamples.Infra.Repository.Detran
{
    public class DetranSPVerificadorPontosRepository : IDetranVerificadorPontosRepository
    {
        private readonly ILogger _logger;

        public DetranSPVerificadorPontosRepository(ILogger<DetranSPVerificadorPontosRepository> logger)
        {
            _logger = logger;
        }
        
        public Task<IEnumerable<PontosCarteira>> ConsultarPontos(Condutor condutor)
        {
            _logger.LogDebug(($"Consultando pontos na carteira do condutor {condutor.Nome} para o estado de SP."));
            return Task.FromResult<IEnumerable<PontosCarteira>>(new List<PontosCarteira>() {new PontosCarteira()});
        }
    }
}